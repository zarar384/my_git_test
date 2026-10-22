using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MilitaryDraftSystem.Application.Common.Exceptions;

namespace MilitaryDraftSystem.API.Middleware
{
    /// <summary>
    /// Single place where exceptions that escape the MediatR/controller
    /// pipeline are translated into a standardized <see cref="ProblemDetails"/>
    /// response, using the built-in ASP.NET Core exception-handling pipeline
    /// (<see cref="IExceptionHandler"/> + <c>app.UseExceptionHandler()</c>).
    /// Unexpected exceptions are logged here, once, with the full stack
    /// trace; expected/business failures are logged at a lower level since
    /// they are normal application behavior, not application failures.
    /// Application/domain code should not duplicate this logging.
    /// </summary>
    public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, title) = MapToResponse(exception);

            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                // Truly unexpected failure: full exception + stack trace,
                // correlated with the request via TraceIdentifier.
                logger.LogError(
                    exception,
                    "Unhandled exception while processing {Method} {Path} (TraceId: {TraceId}).",
                    httpContext.Request.Method,
                    httpContext.Request.Path,
                    httpContext.TraceIdentifier);
            }
            else
            {
                // Expected application/business failure: the application
                // continues operating normally, so this is a warning, not
                // an error. No stack trace needed beyond the message.
                logger.LogWarning(
                    "Request {Method} {Path} failed with {StatusCode}: {Message}",
                    httpContext.Request.Method,
                    httpContext.Request.Path,
                    statusCode,
                    exception.Message);
            }

            if (exception is ValidationException validationException)
            {
                var errors = validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                var validationProblem = new ValidationProblemDetails(errors)
                {
                    Status = statusCode,
                    Title = title,
                    Instance = httpContext.Request.Path
                };
                validationProblem.Extensions["traceId"] = httpContext.TraceIdentifier;

                httpContext.Response.StatusCode = statusCode;
                await httpContext.Response.WriteAsJsonAsync(validationProblem, cancellationToken);
                return true;
            }

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                // Expected failures carry a safe, client-facing message; the
                // message for truly unexpected exceptions is never exposed.
                Detail = statusCode == StatusCodes.Status500InternalServerError
                    ? "An unexpected error occurred. Please contact support if the problem persists."
                    : exception.Message,
                Instance = httpContext.Request.Path
            };
            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }

        private static (int StatusCode, string Title) MapToResponse(Exception exception) => exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Validation failed"),
            NotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
            // A syntactically valid request rejected by the current state of
            // an existing entity (lifecycle conflicts, business rules).
            InvalidOperationException => (StatusCodes.Status409Conflict, "Request conflicts with current state"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };
    }
}
