using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using MilitaryDraftSystem.Application.Draft.Behaviors;

namespace MilitaryDraftSystem.Tests.Application.Draft.Behaviors
{
    public class ValidationBehaviorTests
    {
        public sealed record SampleRequest(string Name) : IRequest<string>;

        [Fact]
        public async Task Handle_ShouldInvokeNext_WhenNoValidatorsExist()
        {
            // Arrange
            var sut = new ValidationBehavior<SampleRequest, string>(Enumerable.Empty<IValidator<SampleRequest>>());
            var nextCalled = false;

            RequestHandlerDelegate<string> next = (_) =>
            {
                nextCalled = true;
                return Task.FromResult("ok");
            };

            // Act
            var result = await sut.Handle(new SampleRequest("Alice"), next, CancellationToken.None);

            // Assert
            Assert.True(nextCalled);
            Assert.Equal("ok", result);
        }

        [Fact]
        public async Task Handle_ShouldInvokeNext_WhenValidationSucceeds()
        {
            // Arrange
            var validatorMock = new Mock<IValidator<SampleRequest>>();
            validatorMock
                .Setup(v => v.Validate(It.IsAny<ValidationContext<SampleRequest>>()))
                .Returns(new ValidationResult());

            var sut = new ValidationBehavior<SampleRequest, string>(new[] { validatorMock.Object });
            var nextCalled = false;

            RequestHandlerDelegate<string> next = (_) =>
            {
                nextCalled = true;
                return Task.FromResult("ok");
            };

            // Act
            var result = await sut.Handle(new SampleRequest("Alice"), next, CancellationToken.None);

            // Assert
            Assert.True(nextCalled);
            Assert.Equal("ok", result);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
        {
            // Arrange
            var failure = new ValidationFailure(nameof(SampleRequest.Name), "Name is required.");
            var validatorMock = new Mock<IValidator<SampleRequest>>();
            validatorMock
                .Setup(v => v.Validate(It.IsAny<ValidationContext<SampleRequest>>()))
                .Returns(new ValidationResult(new[] { failure }));

            var sut = new ValidationBehavior<SampleRequest, string>(new[] { validatorMock.Object });

            RequestHandlerDelegate<string> next = (_) => Task.FromResult("ok");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                sut.Handle(new SampleRequest(""), next, CancellationToken.None));

            Assert.Single(exception.Errors);
            Assert.Equal("Name is required.", exception.Errors.First().ErrorMessage);
        }

        [Fact]
        public async Task Handle_ShouldAggregateErrors_FromMultipleValidators()
        {
            // Arrange
            var firstValidatorMock = new Mock<IValidator<SampleRequest>>();
            firstValidatorMock
                .Setup(v => v.Validate(It.IsAny<ValidationContext<SampleRequest>>()))
                .Returns(new ValidationResult(new[] { new ValidationFailure("Name", "Error from first validator.") }));

            var secondValidatorMock = new Mock<IValidator<SampleRequest>>();
            secondValidatorMock
                .Setup(v => v.Validate(It.IsAny<ValidationContext<SampleRequest>>()))
                .Returns(new ValidationResult(new[] { new ValidationFailure("Name", "Error from second validator.") }));

            var sut = new ValidationBehavior<SampleRequest, string>(new[] { firstValidatorMock.Object, secondValidatorMock.Object });

            RequestHandlerDelegate<string> next = (_) => Task.FromResult("ok");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                sut.Handle(new SampleRequest(""), next, CancellationToken.None));

            Assert.Equal(2, exception.Errors.Count());
        }

        [Fact]
        public async Task Handle_ShouldNotInvokeNext_WhenValidationFails()
        {
            // Arrange
            var failure = new ValidationFailure(nameof(SampleRequest.Name), "Name is required.");
            var validatorMock = new Mock<IValidator<SampleRequest>>();
            validatorMock
                .Setup(v => v.Validate(It.IsAny<ValidationContext<SampleRequest>>()))
                .Returns(new ValidationResult(new[] { failure }));

            var sut = new ValidationBehavior<SampleRequest, string>(new[] { validatorMock.Object });
            var nextCalled = false;

            RequestHandlerDelegate<string> next = (_) =>
            {
                nextCalled = true;
                return Task.FromResult("ok");
            };

            // Act
            try
            {
                await sut.Handle(new SampleRequest(""), next, CancellationToken.None);
            }
            catch (ValidationException)
            {
                // expected
            }

            // Assert
            Assert.False(nextCalled);
        }
    }
}
