using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Sinks.Grafana.Loki;
using System.Diagnostics.Metrics;


var builder3 = WebApplication.CreateBuilder(args);


builder3.Services.AddOpenTelemetry()
    // Set service name for telemetry
    .ConfigureResource(r => r.AddService("inventory-service"))
    // Tracing pipeline
    .WithTracing(t =>
    {
        t.AddAspNetCoreInstrumentation();   // Trace incoming HTTP requests
        t.AddOtlpExporter();                // Send traces via OTLP -> Alloy
    })
    // Metrics pipeline
    .WithMetrics(m =>
    {
        m.AddAspNetCoreInstrumentation();   // HTTP request metrics
        m.AddRuntimeInstrumentation();      // GC, threads, memory metrics
        m.AddOtlpExporter();                // Send metrics via OTLP -> Alloy
    });


Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithSpan()                         // traceId / spanId
    .WriteTo.Console()
    .WriteTo.GrafanaLoki("http://loki:3100")   // Loki в docker-сети
    .CreateLogger();

builder3.Host.UseSerilog();

var app3 = builder3.Build();


var remainingBeans = 100;
var remainingMilk = 50;


var meter = new Meter("inventory-service");

var inventoryErrors = meter.CreateCounter<long>("inventory_errors_total");

var beansGauge = meter.CreateObservableGauge(
    "inventory_remaining_beans",
    () => new Measurement<int>(remainingBeans));

var milkGauge = meter.CreateObservableGauge(
    "inventory_remaining_milk",
    () => new Measurement<int>(remainingMilk));

var rand = new Random();

// Minimal API endpoints
app3.MapGet("/", () => "Inventory Service is running");

app3.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app3.MapPost("/reserve", (ReserveRequest request, ILogger<Program> logger) =>
{
    logger.LogInformation("Reserve request {OrderId} for {CoffeeType}", request.OrderId, request.CoffeeType);

    var beansNeeded = request.CoffeeType switch
    {
        "espresso" => 1,
        "latte" => 1,
        "cappuccino" => 1,
        _ => 1
    };


    var milkNeeded = request.CoffeeType switch
    {
        "latte" => 1,
        "cappuccino" => 1,
        _ => 0
    };


    if (remainingBeans < beansNeeded || remainingMilk < milkNeeded)
    {
        inventoryErrors.Add(1);
        return Results.Problem("Not enough resources", statusCode: 409);
    }


    if (rand.NextDouble() < 0.1)
    {
        inventoryErrors.Add(1);
        return Results.Problem("Random inventory failure", statusCode: 500);
    }


    remainingBeans -= beansNeeded;
    remainingMilk -= milkNeeded;


    return Results.Ok(new
    {
        request.OrderId,
        beansLeft = remainingBeans,
        milkLeft = remainingMilk
    });
});


app3.Run("http://0.0.0.0:8080");