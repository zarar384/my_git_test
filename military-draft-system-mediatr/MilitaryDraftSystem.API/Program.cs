using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MilitaryDraftSystem.Application.Common.Interfaces;
using MilitaryDraftSystem.Application.Draft.Behaviors;
using MilitaryDraftSystem.Infrastructure.Persistence;
using MilitaryDraftSystem.Infrastructure.Persistence.Interceptors;
using MilitaryDraftSystem.Application.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi(); // OpenAPI (Swagger alternative)

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly));

// Domain events interceptor
builder.Services.AddScoped<DomainEventsInterceptor>();

// DbContext
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.UseSqlite("Data Source=draft.db");

    // Add interceptor
    options.AddInterceptors(sp.GetRequiredService<DomainEventsInterceptor>());
});

// Abstraction
builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly);

// Pipeline behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

var app = builder.Build();

// Auto apply migrations and create database if it doesn't exist 
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();