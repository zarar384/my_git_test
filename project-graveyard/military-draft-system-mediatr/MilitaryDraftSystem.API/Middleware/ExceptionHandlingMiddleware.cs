using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace MilitaryDraftSystem.API.Middleware
{
    /// <summary>
    /// Single place where unhandled exceptions escaping the MediatR/controller
    /// pipeline are caught, logged exactly once with full stack trace, and
    /// translated into a safe HTTP response. Application/domain code should
    /// not also log these same failures — this middleware is the last line
    /// of defense for anything that wasn't already handled closer to its
    /// source.
    /// </summary>
    public sealed class ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException ex)
            {
                // Expected, client-caused failure: log as a warning, no stack
                // trace noise needed beyond the validation summary.
                logger.LogWarning(
                    "Request {Method} {Path} failed validation: {ValidationErrors}",
                    context.Request.Method,
                    context.Request.Path,
                    string.Join("; ", ex.Errors.Select(e => e.ErrorMessage)));

                await WriteProblemAsync(
                    context,
                    HttpStatusCode.BadRequest,
                    "Validation failed",
                    ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // Expected domain/business-rule failure (not found, invalid
                // state transition, etc.). Logged as a warning since the
                // application continues operating normally.
                logger.LogWarning(
                    ex,
                    "Request {Method} {Path} failed: {ErrorMessage}",
                    context.Request.Method,
                    context.Request.Path,
                    ex.Message);

                await WriteProblemAsync(
                    context,
                    HttpStatusCode.BadRequest,
                    "Request failed",
                    ex.Message);
            }
            catch (Exception ex)
            {
                // Truly unexpected failure. Logged once, here, with full
                // stack trace and the ASP.NET Core TraceIdentifier so it can
                // be correlated with the originating request.
                logger.LogError(
                    ex,
                    "Unhandled exception while processing {Method} {Path} (TraceId: {TraceId}).",
                    context.Request.Method,
                    context.Request.Path,
                    context.TraceIdentifier);

                await WriteProblemAsync(
                    context,
                    HttpStatusCode.InternalServerError,
                    "An unexpected error occurred",
                    "Please contact support if the problem persists.");
            }
        }

        private static async Task WriteProblemAsync(
            HttpContext context,
            HttpStatusCode statusCode,
            string title,
            string detail)
        {
            if (context.Response.HasStarted)
            {
                return;
            }

            context.Response.Clear();
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(new
            {
                title,
                status = (int)statusCode,
                detail,
                traceId = context.TraceIdentifier
            });
        }
    }
}
