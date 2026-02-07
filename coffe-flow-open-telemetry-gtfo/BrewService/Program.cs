using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Diagnostics.Metrics;


var builder2 = WebApplication.CreateBuilder(args);


builder2.Services.AddOpenTelemetry()
    // Set service name for telemetry
    .ConfigureResource(r => r.AddService("brew-service"))
    // Tracing pipeline
    .WithTracing(t =>
    {
        t.AddAspNetCoreInstrumentation();   // Trace incoming HTTP requests
        t.AddOtlpExporter();                // Send traces via OTLP
    })
    // Metrics pipeline
    .WithMetrics(m =>
    {
        m.AddAspNetCoreInstrumentation();   // HTTP request metrics
        m.AddRuntimeInstrumentation();      // GC, threads, memory metrics

        // custom metrics
        m.AddMeter("brew-service");        // Enable custom metrics from this assembly

        // Export metrics to Prometheus/Mimir via OTLP
        m.AddOtlpExporter(o =>              // Send metrics via OTLP
        {
            o.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf; // Use HTTP Protobuf for better performance
        });  
    });


var app2 = builder2.Build();

var meter = new Meter("brew-service");

// Average latency (mean). Calculates the average brew time over the last 5 minutes:
//   rate(coffee_brew_duration_seconds_sum[5m]) / rate(coffee_brew_duration_seconds_count[5m])
//
// P95 / P99 latency. Shows how long the slowest requests are. P95 means 95% of brews finish faster than this value.:
//   histogram_quantile(
//     0.95,                                   // or 0.99
//     sum by (le) (
//       increase(coffee_brew_duration_seconds_bucket[15m])
//     )
//   )
var brewDuration = meter.CreateHistogram<double>("coffee_brew_duration_seconds"); // _sum(total time spent brewing), _count(number of brews), _bucket(latency buckets for percentiles) 
var brewErrors = meter.CreateCounter<long>("coffee_brew_errors_total");

var random = new Random();

// Minimal API endpoints
app2.MapGet("/", () => "Brew Service is running");

app2.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app2.MapPost("/brew", async (BrewRequest request) =>
{
    var sw = Stopwatch.StartNew();


    var delay = request.CoffeeType switch
    {
        "espresso" => random.Next(300, 800),
        "latte" => random.Next(700, 1500),
        "cappuccino" => random.Next(600, 1200),
        _ => random.Next(500, 1000)
    };


    await Task.Delay(delay);


    if (random.NextDouble() < 0.1)
    {
        brewErrors.Add(1);
        return Results.Problem("Machine failure", statusCode: 500);
    }


    sw.Stop();
    brewDuration.Record(sw.Elapsed.TotalSeconds);


    return Results.Ok(new { request.OrderId, status = "ready" });
});


// explicit URL binding; launchSettings.json is ignored
app2.Run("http://0.0.0.0:8080");