using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using MilitaryDraftSystem.API.Auth;
using MilitaryDraftSystem.API.Middleware;
using MilitaryDraftSystem.Application.Common;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Application.Draft.Behaviors;
using MilitaryDraftSystem.Application.Population.Services;
using MilitaryDraftSystem.Application.Population.Services.Interfaces;
using MilitaryDraftSystem.Infrastructure.BackgroundServices;
using MilitaryDraftSystem.Infrastructure.Persistence;
using MilitaryDraftSystem.Infrastructure.Persistence.Interceptors;
using MilitaryDraftSystem.Infrastructure.Services;
using Serilog;

// Bootstrap logger: active only until the host's configured Serilog pipeline
// takes over below, so that failures occurring during host/configuration
// startup are still captured somewhere instead of being silently lost.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting MilitaryDraftSystem.API.");

    // Create application builder.
    var builder = WebApplication.CreateBuilder(args);

    // Replace the default logging providers with Serilog, configured entirely
    // from appsettings.json / appsettings.{Environment}.json (see the
    // "Serilog" section). Console/file sinks, minimum levels and file
    // retention are all controlled by configuration, not code.
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

    #region MVC

    // Add MVC controllers.
    builder.Services.AddControllers();

    // OpenAPI (Swagger alternative).
    // builder.Services.AddOpenApi();

    #endregion

    #region Exception Handling

    // Standard ASP.NET Core exception-handling pipeline: uncaught exceptions
    // are routed to GlobalExceptionHandler and rendered as RFC 7807
    // ProblemDetails. AddProblemDetails() also ensures framework-generated
    // responses (e.g. 404 for unmatched routes) use the same contract.
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    #endregion

    #region Authentication

    // Register API key authentication used to protect the draft endpoints.
    builder.Services
        .AddAuthentication(ApiKeyAuthenticationOptions.SchemeName)
        .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
            ApiKeyAuthenticationOptions.SchemeName,
            _ => { });

    builder.Services.AddAuthorization();

    #endregion

    #region Application

    // Register MediatR handlers.
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

    // Register FluentValidation validators.
    builder.Services.AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly);

    // Register application services.
    builder.Services.AddScoped<IPopulationGenerationService, PopulationGenerationService>();

    // Register the centralized random provider used by all simulation randomness.
    builder.Services.AddSingleton<IRandomProvider, SystemRandomProvider>();

    // Register world narration for lively console output.
    builder.Services.AddSingleton<IWorldNarrator, ConsoleWorldNarrator>();

    #endregion

    #region Infrastructure

    // Register domain events interceptor.
    builder.Services.AddScoped<DomainEventsInterceptor>();

    // Register Entity Framework Core.
    builder.Services.AddDbContext<AppDbContext>((sp, options) =>
    {
        options.UseSqlite(
            builder.Configuration.GetConnectionString("DefaultConnection"));

        // Register EF Core interceptors.
        options.AddInterceptors(
            sp.GetRequiredService<DomainEventsInterceptor>());
    });

    // Register database abstraction.
    builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

    #endregion

    #region Health Checks

    // "database" check is tagged "ready" so it only gates readiness, not
    // liveness: the process itself is alive even if SQLite is temporarily
    // unavailable. The DbContext check only verifies connectivity (opens a
    // connection); it does not run business queries or migrations.
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<AppDbContext>("database", tags: new[] { "ready" });

    #endregion

    #region Hosted Services

    // Initialize the database during application startup.
    builder.Services.AddHostedService<DatabaseInitializationService>();

    // Execute automatic population generation.
    builder.Services.AddHostedService<PopulationGenerationHostedService>();

    // Execute automatic military recruitment.
    builder.Services.AddHostedService<AutomaticRecruitmentHostedService>();

    // Simulate the passage of time for the living population.
    builder.Services.AddHostedService<PopulationSimulationHostedService>();

    // Simulate the passage of time for active recruitment officers.
    builder.Services.AddHostedService<OfficerLifeSimulationHostedService>();

    // Physically purge deceased citizens once their retention window elapses.
    builder.Services.AddHostedService<CitizenCleanupHostedService>();

    #endregion

    #region Pipeline Behaviors

    // Register MediatR pipeline behaviors executed before and after each request.
    builder.Services.AddTransient(
        typeof(IPipelineBehavior<,>),
        typeof(ValidationBehavior<,>));

    builder.Services.AddTransient(
        typeof(IPipelineBehavior<,>),
        typeof(LoggingBehavior<,>));

    builder.Services.AddTransient(
        typeof(IPipelineBehavior<,>),
        typeof(TransactionBehavior<,>));

    #endregion

    // Build the application.
    var app = builder.Build();

    #region HTTP Pipeline

    // Single place where unhandled exceptions are caught, logged once, and
    // turned into a standardized ProblemDetails response. Must run before
    // any other middleware that could throw.
    app.UseExceptionHandler();

    // Structured per-request completion logging (method, path, status code,
    // elapsed time) tagged with ASP.NET Core's built-in TraceIdentifier, so
    // related log entries for a single HTTP request can be correlated
    // without a custom correlation mechanism. Health endpoints are excluded
    // so frequent monitoring/orchestrator polling doesn't spam the logs.
    app.UseSerilogRequestLogging(options =>
    {
        options.GetLevel = (httpContext, _, _) =>
            httpContext.Request.Path.StartsWithSegments("/health")
                ? Serilog.Events.LogEventLevel.Verbose
                : Serilog.Events.LogEventLevel.Information;
    });

    if (app.Environment.IsDevelopment())
    {
        // app.MapOpenApi();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    // Infrastructure endpoints for process monitoring/orchestration, not
    // protected by API-key authentication so Docker/deployment health
    // probes can reach them. Liveness never depends on external
    // dependencies; readiness also checks the ones tagged "ready" (e.g.
    // the database).
    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = _ => false
    });

    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = HealthCheckResponseWriter.WriteAsync
    });

    #endregion

    Log.Information("MilitaryDraftSystem.API started successfully.");

    // Start the application.
    app.Run();

    Log.Information("MilitaryDraftSystem.API shut down gracefully.");
}
catch (Exception ex)
{
    Log.Fatal(ex, "MilitaryDraftSystem.API terminated unexpectedly during startup.");
}
finally
{
    // Ensure all buffered log events are flushed to every sink before exit.
    Log.CloseAndFlush();
}

// Exposes the implicit top-level Program class to WebApplicationFactory<Program>
// for integration testing.
public partial class Program
{
}
