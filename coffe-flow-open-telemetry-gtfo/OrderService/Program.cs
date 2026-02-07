using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Sinks.Grafana.Loki;
using System.Diagnostics.Metrics;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenTelemetry()
    // Set service name for traces, metrics, logs
    .ConfigureResource(r => r.AddService("order-service"))
    // Tracing pipeline
    .WithTracing(t =>
    {
        t.AddAspNetCoreInstrumentation();   // Trace incoming HTTP requests
        t.AddHttpClientInstrumentation();   // Trace outgoing HTTP calls
        t.AddOtlpExporter();                // Send traces via OTLP (to Alloy/Tempo)
    })
    // Metrics pipeline
    .WithMetrics(m =>
    {
        m.AddAspNetCoreInstrumentation();   // HTTP request metrics
        m.AddHttpClientInstrumentation();   // HTTP client metrics
        m.AddRuntimeInstrumentation();      // GC, threads, memory metrics

        // custom metrics
        m.AddMeter("order-service");        // Enable custom metrics from this assembly
        
        // Export metrics to Prometheus/Mimir via OTLP
        m.AddOtlpExporter(o =>              // Send metrics via OTLP (to Mimir/Prometheus)
        {
            o.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf; // Use HTTP Protobuf for better performance
        });               
    });

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()                 // Add scoped properties
    .Enrich.WithSpan()                       // Add traceId / spanId
    .WriteTo.Console()                       // Log to console
    .WriteTo.GrafanaLoki("http://loki:3100", // Send logs to Loki
         labels: new[]
        {
            new LokiLabel { Key = "service", Value = "order-service" }
        })
    .CreateLogger();

builder.Host.UseSerilog(); // Replace default logger with Serilog

builder.Services.AddHttpClient("brew", c => c.BaseAddress = new Uri("http://brewservice:8080"));
builder.Services.AddHttpClient("inventory", c => c.BaseAddress = new Uri("http://inventoryservice:8080"));

var app = builder.Build();

var meter = new Meter("order-service");

var ordersCounter = meter.CreateCounter<long>("coffee_orders_total");

// Minimal API endpoints
app.MapGet("/", () => "Order Service is running");

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// POST /order?coffeeType=latte
app.MapPost("/order", async (string coffeeType, IHttpClientFactory factory, ILogger<Program> logger) =>
{
    var orderId = Guid.NewGuid().ToString();

    logger.LogInformation("New order {OrderId} for {CoffeeType}", orderId, coffeeType);

    var inventoryClient = factory.CreateClient("inventory");
    var brewClient = factory.CreateClient("brew");

    var inventoryResponse = await inventoryClient.PostAsJsonAsync("/reserve", new
    {
        orderId,
        coffeeType
    });

    if (!inventoryResponse.IsSuccessStatusCode)
        return Results.Problem("Inventory error", statusCode: 409);

    var brewResponse = await brewClient.PostAsJsonAsync("/brew", new
    {
        orderId,
        coffeeType
    });

    if (!brewResponse.IsSuccessStatusCode)
        return Results.Problem("Brew error", statusCode: 500);

    return Results.Ok(new
    {
        orderId,
        coffeeType,
        status = "completed"
    });
});

app.MapPost("/orders", async (OrderRequest request, IHttpClientFactory factory, ILogger<Program> logger) =>
{
    ordersCounter.Add(1);

    var orderId = Guid.NewGuid().ToString();

    logger.LogInformation("New order {OrderId} for {CoffeeType}", orderId, request.CoffeeType);

    var brewClient = factory.CreateClient("brew");
    var inventoryClient = factory.CreateClient("inventory");


    var inventoryResponse = await inventoryClient.PostAsJsonAsync("/reserve", new
    {
        orderId,
        coffeeType = request.CoffeeType
    });


    if (!inventoryResponse.IsSuccessStatusCode)
        return Results.Problem("Inventory error", statusCode: 409);


    var brewResponse = await brewClient.PostAsJsonAsync("/brew", new
    {
        orderId,
        coffeeType = request.CoffeeType
    });


    if (!brewResponse.IsSuccessStatusCode)
        return Results.Problem("Brew error", statusCode: 500);


    return Results.Ok(new { orderId, status = "completed" });
});


// explicit URL binding; launchSettings.json is ignored
app.Run("http://0.0.0.0:8080");