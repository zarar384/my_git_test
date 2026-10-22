using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MilitaryDraftSystem.API.Middleware;
using MilitaryDraftSystem.Application.Common.Exceptions;

namespace MilitaryDraftSystem.Tests.Api.Middleware
{
    public class GlobalExceptionHandlerTests
    {
        private static (GlobalExceptionHandler Handler, DefaultHttpContext Context) CreateSut()
        {
            var handler = new GlobalExceptionHandler(Microsoft.Extensions.Logging.Abstractions.NullLogger<GlobalExceptionHandler>.Instance);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            return (handler, context);
        }

        [Fact]
        public async Task TryHandleAsync_ShouldReturn400WithFieldErrors_ForValidationException()
        {
            // Arrange
            var (handler, context) = CreateSut();
            var failures = new List<ValidationFailure>
            {
                new("CitizenId", "CitizenId is required.")
            };
            var exception = new ValidationException(failures);

            // Act
            var handled = await handler.TryHandleAsync(context, exception, CancellationToken.None);

            // Assert
            Assert.True(handled);
            Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        }

        [Fact]
        public async Task TryHandleAsync_ShouldReturn404_ForNotFoundException()
        {
            // Arrange
            var (handler, context) = CreateSut();
            var exception = new NotFoundException("Citizen was not found.");

            // Act
            var handled = await handler.TryHandleAsync(context, exception, CancellationToken.None);

            // Assert
            Assert.True(handled);
            Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
        }

        [Fact]
        public async Task TryHandleAsync_ShouldReturn409_ForInvalidOperationException()
        {
            // Arrange
            var (handler, context) = CreateSut();
            var exception = new InvalidOperationException("Officer is not active and cannot draft citizens.");

            // Act
            var handled = await handler.TryHandleAsync(context, exception, CancellationToken.None);

            // Assert
            Assert.True(handled);
            Assert.Equal(StatusCodes.Status409Conflict, context.Response.StatusCode);
        }

        [Fact]
        public async Task TryHandleAsync_ShouldReturn500WithSafeMessage_ForUnexpectedException()
        {
            // Arrange
            var (handler, context) = CreateSut();
            var exception = new Exception("Connection string leaked here: Data Source=secret.db");

            // Act
            var handled = await handler.TryHandleAsync(context, exception, CancellationToken.None);

            // Assert
            Assert.True(handled);
            Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(context.Response.Body);
            var body = await reader.ReadToEndAsync();

            Assert.DoesNotContain("secret.db", body);
        }
    }
}
