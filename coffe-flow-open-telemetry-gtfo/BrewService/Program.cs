using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
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
        m.AddOtlpExporter();                // Send metrics via OTLP
    });


var app2 = builder2.Build();

var meter = new Meter("brew-service");

var brewDuration = meter.CreateHistogram<double>("coffee_brew_duration_seconds");
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


app2.Run("http://0.0.0.0:8080");