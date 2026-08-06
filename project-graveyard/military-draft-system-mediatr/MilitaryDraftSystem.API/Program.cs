using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MilitaryDraftSystem.Application.Common;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Application.Draft.Behaviors;
using MilitaryDraftSystem.Application.Population.Services;
using MilitaryDraftSystem.Application.Population.Services.Interfaces;
using MilitaryDraftSystem.Infrastructure.BackgroundServices;
using MilitaryDraftSystem.Infrastructure.Persistence;
using MilitaryDraftSystem.Infrastructure.Persistence.Interceptors;
using MilitaryDraftSystem.Infrastructure.Services;

// Create application builder.
var builder = WebApplication.CreateBuilder(args);

#region MVC

// Add MVC controllers.
builder.Services.AddControllers();

// OpenAPI (Swagger alternative).
// builder.Services.AddOpenApi();

#endregion

#region Application

// Register MediatR handlers.
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

// Register FluentValidation validators.
builder.Services.AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly);

// Register application services.
builder.Services.AddScoped<IPopulationGenerationService, PopulationGenerationService>();

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

#region Hosted Services

// Initialize the database during application startup.
builder.Services.AddHostedService<DatabaseInitializationService>();

// Execute automatic population generation.
builder.Services.AddHostedService<PopulationGenerationHostedService>();

// Execute automatic military recruitment.
builder.Services.AddHostedService<AutomaticRecruitmentHostedService>();

// Simulate the passage of time for the living population.
builder.Services.AddHostedService<PopulationSimulationHostedService>();

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

if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

#endregion

// Start the application.
app.Run();