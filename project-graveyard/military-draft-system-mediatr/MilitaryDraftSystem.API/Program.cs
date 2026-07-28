using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MilitaryDraftSystem.Application.Common;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Application.Draft.Behaviors;
using MilitaryDraftSystem.Infrastructure.Persistence;
using MilitaryDraftSystem.Infrastructure.Persistence.Interceptors;
using MilitaryDraftSystem.Infrastructure.Services;

// Builder block
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
//builder.Services.AddOpenApi(); // OpenAPI (Swagger alternative)

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

// Domain events interceptor
builder.Services.AddScoped<DomainEventsInterceptor>();

// DbContext
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));

    // Add interceptor
    options.AddInterceptors(sp.GetRequiredService<DomainEventsInterceptor>());
});

// Database initialization service
builder.Services.AddHostedService<DatabaseInitializationService>();

// Abstraction
builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly);

// Pipeline behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

// Application block
var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();