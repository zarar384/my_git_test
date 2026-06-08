using LeaveMeAloneCSharp.Functional.Monads;

namespace LeaveMeAloneCSharp.Test
{
    public class TryTests
    {
        // Factory methods

        [Fact]
        public void FromValue_ShouldBeSuccess()
        {
            // Arrange & Act
            var result = Try<int>.FromValue(42);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.IsFailure);
            Assert.Equal(42, result.Value);
            Assert.Null(result.Exception);
        }

        [Fact]
        public void FromValue_WithNull_ShouldBeSuccess()
        {
            // Arrange & Act
            var result = Try<string?>.FromValue(null);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.Value);
            Assert.Null(result.Exception);
        }

        [Fact]
        public void FromException_ShouldBeFailure()
        {
            // Arrange
            var ex = new InvalidOperationException("oops");

            // Act
            var result = Try<int>.FromException(ex);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.True(result.IsFailure);
            Assert.Same(ex, result.Exception);
        }

        [Fact]
        public void FromException_ValueShouldBeDefault()
        {
            // Arrange & Act
            var result = Try<int>.FromException(new Exception());

            // Assert
            Assert.Equal(default, result.Value);
        }

        // Try.Of

        [Fact]
        public void Of_WithSuccessfulFunc_ShouldCaptureValue()
        {
            // Arrange & Act
            var result = Try.Of(() => 1 + 1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value);
        }

        [Fact]
        public void Of_WithThrowingFunc_ShouldCaptureException()
        {
            // Arrange & Act
            var result = Try.Of<int>(() => throw new DivideByZeroException());

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<DivideByZeroException>(result.Exception);
        }

        [Fact]
        public void Of_PreservesOriginalExceptionTypeAndMetadata()
        {
            // Arrange & Act
            var result = Try.Of<string>(() => throw new ArgumentNullException("param"));

            // Assert
            Assert.IsType<ArgumentNullException>(result.Exception);
            Assert.Equal("param", ((ArgumentNullException)result.Exception!).ParamName);
        }

        // Static helper methods on non-generic Try

        [Fact]
        public void StaticFromValue_ShouldDelegateToGenericClass()
        {
            // Arrange & Act
            var result = Try.FromValue(99);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(99, result.Value);
        }

        [Fact]
        public void StaticFromException_ShouldDelegateToGenericClass()
        {
            // Arrange
            var ex = new NotSupportedException();

            // Act
            var result = Try.FromException<double>(ex);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Same(ex, result.Exception);
        }

        // Map - success path

        [Fact]
        public void Map_OnSuccess_TransformsValue()
        {
            // Arrange
            var sut = Try<int>.FromValue(5);

            // Act
            var result = sut.Map(x => x * 2);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(10, result.Value);
        }

        [Fact]
        public void Map_OnSuccess_CanChangeType()
        {
            // Arrange
            var sut = Try<int>.FromValue(42);

            // Act
            var result = sut.Map(x => x.ToString());

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("42", result.Value);
        }

        [Fact]
        public void Map_OnSuccess_ChainedMultipleTimes_AppliesAll()
        {
            // Arrange
            var sut = Try<int>.FromValue(3);

            // Act
            var result = sut
                .Map(x => x + 1)   // 4
                .Map(x => x * 10)  // 40
                .Map(x => x - 5);  // 35

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(35, result.Value);
        }

        // Map - failure path

        [Fact]
        public void Map_OnFailure_PropagatesExceptionWithoutCallingMapper()
        {
            // Arrange
            var ex = new Exception("original");
            var mapperCalled = false;
            var sut = Try<int>.FromException(ex);

            // Act
            var result = sut.Map(x => { mapperCalled = true; return x * 2; });

            // Assert
            Assert.True(result.IsFailure);
            Assert.Same(ex, result.Exception);
            Assert.False(mapperCalled);
        }

        [Fact]
        public void Map_WhenMapperThrows_CapturesException()
        {
            // Arrange
            var sut = Try<int>.FromValue(0);

            // Act
            var result = sut.Map(_ => int.Parse("not_a_number"));

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<FormatException>(result.Exception);
        }

        [Fact]
        public void Map_OnFailure_SubsequentMapsAreSkipped()
        {
            // Arrange
            var callCount = 0;
            var sut = Try<int>.FromException(new Exception());

            // Act
            var result = sut
                .Map(x => { callCount++; return x; })
                .Map(x => { callCount++; return x; })
                .Map(x => { callCount++; return x; });

            // Assert
            Assert.Equal(0, callCount);
            Assert.True(result.IsFailure);
        }

        // Bind - success path

        [Fact]
        public void Bind_OnSuccess_ReturnsBinderResult()
        {
            // Arrange
            var sut = Try<int>.FromValue(10);

            // Act
            var result = sut.Bind(x => Try<string>.FromValue($"value={x}"));

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("value=10", result.Value);
        }

        [Fact]
        public void Bind_OnSuccess_ChainedSuccessfully()
        {
            // Arrange
            var sut = Try.Of(() => "  hello  ");

            // Act
            var result = sut
                .Bind(s => Try.Of(() => s.Trim()))
                .Bind(s => Try.Of(() => s.ToUpper()));

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("HELLO", result.Value);
        }

        [Fact]
        public void Bind_OnSuccess_WhenBinderReturnsFailed_PropagatesInnerException()
        {
            // Arrange
            var innerEx = new InvalidOperationException("inner");
            var sut = Try<int>.FromValue(5);

            // Act
            var result = sut.Bind(_ => Try<string>.FromException(innerEx));

            // Assert
            Assert.True(result.IsFailure);
            Assert.Same(innerEx, result.Exception);
        }

        // Bind - failure path

        [Fact]
        public void Bind_OnFailure_PropagatesExceptionWithoutCallingBinder()
        {
            // Arrange
            var ex = new Exception("original");
            var binderCalled = false;
            var sut = Try<int>.FromException(ex);

            // Act
            var result = sut.Bind(x => { binderCalled = true; return Try<int>.FromValue(x); });

            // Assert
            Assert.True(result.IsFailure);
            Assert.Same(ex, result.Exception);
            Assert.False(binderCalled);
        }

        [Fact]
        public void Bind_WhenBinderThrows_CapturesException()
        {
            // Arrange
            var sut = Try<int>.FromValue(1);

            // Act
            var result = sut.Bind<string>(_ => throw new OverflowException("boom"));

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<OverflowException>(result.Exception);
        }

        // GetOrElse

        [Fact]
        public void GetOrElse_OnSuccess_ReturnsValue()
        {
            // Arrange
            var sut = Try<int>.FromValue(7);

            // Act
            var result = sut.GetOrElse(-1);

            // Assert
            Assert.Equal(7, result);
        }

        [Fact]
        public void GetOrElse_OnFailure_ReturnsFallback()
        {
            // Arrange
            var sut = Try<int>.FromException(new Exception());

            // Act
            var result = sut.GetOrElse(-1);

            // Assert
            Assert.Equal(-1, result);
        }

        [Fact]
        public void GetOrElse_WhenValueIsNull_OnSuccess_ReturnsNull()
        {
            // Arrange - null value must not be confused with the fallback
            var sut = Try<string?>.FromValue(null);

            // Act
            var result = sut.GetOrElse("fallback");

            // Assert
            Assert.Null(result);
        }

        // Match

        [Fact]
        public void Match_OnSuccess_CallsOnSuccess()
        {
            // Arrange
            int? captured = null;
            var sut = Try<int>.FromValue(42);

            // Act
            sut.Match(
                onSuccess: v => captured = v,
                onFailure: _ => { });

            // Assert
            Assert.Equal(42, captured);
        }

        [Fact]
        public void Match_OnSuccess_DoesNotCallOnFailure()
        {
            // Arrange
            var failureCalled = false;
            var sut = Try<int>.FromValue(1);

            // Act
            sut.Match(
                onSuccess: _ => { },
                onFailure: _ => failureCalled = true);

            // Assert
            Assert.False(failureCalled);
        }

        [Fact]
        public void Match_OnFailure_CallsOnFailure()
        {
            // Arrange
            var ex = new Exception("fail");
            Exception? captured = null;
            var sut = Try<int>.FromException(ex);

            // Act
            sut.Match(
                onSuccess: _ => { },
                onFailure: e => captured = e);

            // Assert
            Assert.Same(ex, captured);
        }

        [Fact]
        public void Match_OnFailure_DoesNotCallOnSuccess()
        {
            // Arrange
            var successCalled = false;
            var sut = Try<int>.FromException(new Exception());

            // Act
            sut.Match(
                onSuccess: _ => successCalled = true,
                onFailure: _ => { });

            // Assert
            Assert.False(successCalled);
        }

        // Integration: realistic pipelines

        [Fact]
        public void Pipeline_ParseAndDivide_HappyPath()
        {
            // Arrange & Act
            var result = Try.Of(() => int.Parse("10"))
                .Map(x => x / 2);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(5, result.Value);
        }

        [Fact]
        public void Pipeline_ParseInvalid_ErrorPropagatesAcrossSubsequentSteps()
        {
            // Arrange & Act
            var result = Try.Of(() => int.Parse("bad"))
                .Map(x => x / 2)
                .Map(x => x.ToString());

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<FormatException>(result.Exception);
        }

        [Fact]
        public void Pipeline_BindChain_FirstFailureShortCircuits()
        {
            // Arrange
            var step = 0;

            // Act
            var result = Try.Of(() => { step = 1; return 100; })
                .Bind(x => { step = 2; return Try<int>.FromException(new Exception("stop")); })
                .Bind(x => { step = 3; return Try<int>.FromValue(x); });

            // Assert
            Assert.Equal(2, step);
            Assert.True(result.IsFailure);
            Assert.Equal("stop", result.Exception!.Message);
        }

        [Fact]
        public void Pipeline_FailedEarly_GetOrElseRecoversWithFallback()
        {
            // Arrange & Act
            var output = Try.Of<int>(() => throw new Exception())
                .Map(x => x * 10)
                .Bind(x => Try<int>.FromValue(x + 1))
                .GetOrElse(0);

            // Assert
            Assert.Equal(0, output);
        }

        [Fact]
        public void Pipeline_StringToUri_ValidUrl_ExtractsHost()
        {
            // Arrange & Act
            var result = Try.Of(() => "https://example.com/path")
                .Bind(url => Try.Of(() => new Uri(url)))
                .Map(uri => uri.Host);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("example.com", result.Value);
        }

        [Fact]
        public void Pipeline_StringToUri_MalformedUrl_CapturesUriFormatException()
        {
            // Arrange & Act
            var result = Try.Of(() => "not a uri :::??")
                .Bind(url => Try.Of(() => new Uri(url)))
                .Map(uri => uri.Host);

            // Assert
            Assert.True(result.IsFailure);
            Assert.IsType<UriFormatException>(result.Exception);
        }
    }
}