using LeaveMeAloneFuncSkillForge.Functional;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class ReaderTests
    {
        [Fact]
        public void Run_ReturnsFunctionResult()
        {
            // Arrange
            var reader = new Reader<int, int>(env => env * 2);

            // Act
            var result = reader.Run(10);

            // Assert
            Assert.Equal(20, result);
        }

        [Fact]
        public void Bind_TransformsResult()
        {
            // Arrange
            var reader = new Reader<int, int>(env => env + 1)
                    .Bind(x => new Reader<int, int>(_ => x * 10));

            // Act
            var result = reader.Run(5);

            // Assert
            Assert.Equal(60, result); // (5 + 1) * 10
        }

        [Fact]
        public void Bind_PreservesEnvironment()
        {
            // Arrange
            var reader = new Reader<int, int>(env => env * 2)
                    .Bind(x => new Reader<int, int>(env => x + env));

            // Act
            var result = reader.Run(10);

            // Assert
            Assert.Equal(30, result); // (10 * 2) + 10
        }

        [Fact]
        public void MultipleBinds_WorkInSequence()
        {
            // Arrange 
            var reader = new Reader<int, int>(env => env)
                    .Bind(x => new Reader<int, int>(_ => x + 1))
                    .Bind(x => new Reader<int, int>(_ => x * 2))
                    .Bind(x => new Reader<int, string>(_ => $"Value: {x}"));

            // Act
            var result = reader.Run(5);

            // Assert
            Assert.Equal("Value: 12", result); // (5 + 1) * 2
        }

        [Fact]
        public void Reader_IsLazyUntilRun()
        {
            // Arrange
            var called = false;

            // Act
            var reader = new Reader<int, int>(env =>
            {
                called = true;
                return env;
            });

            // Assert
            Assert.False(called);

            reader.Run(1);

            Assert.True(called);
        }

        [Fact]
        public void Binder_IsNotExecutedUntilRun()
        {
            // Arrange
            var binderCalled = false;

            // Act
            var reader = new Reader<int, int>(env => env)
                    .Bind(x =>
                    {
                        binderCalled = true;
                        return new Reader<int, int>(_ => x);
                    });

            // Assert
            Assert.False(binderCalled);

            reader.Run(1);

            Assert.True(binderCalled);
        }

        [Fact]
        public void Run_PropagatesExceptionFromFunction()
        {
            // Arrange
            var reader = new Reader<int, int>(_ =>
                throw new InvalidOperationException("boom"));

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() =>
                reader.Run(1));

            // Assert
            Assert.Equal("boom", ex.Message);
        }

        [Fact]
        public void Bind_PropagatesExceptionFromBinder()
        {
            // Arrange
            var reader = new Reader<int, int>(env => env)
                    .Bind<int>(_ => throw new ArgumentException("bad"));

            // Act
            var ex = Assert.Throws<ArgumentException>(() =>
                reader.Run(1));

            // Assert
            Assert.Equal("bad", ex.Message);
        }

        [Fact]
        public void Bind_PropagatesExceptionFromNestedReader()
        {
            // Arrange 
            var reader = new Reader<int, int>(env => env)
                    .Bind(x => new Reader<int, int>(_ =>
                        throw new Exception("nested fail")));

            // Act 
            var ex = Assert.Throws<Exception>(() =>
                reader.Run(1));

            // Assert
            Assert.Equal("nested fail", ex.Message);
        }

        [Fact]
        public void Run_AllowsNullResult()
        {
            // Arrange
            var reader = new Reader<int, string>(_ => null);

            // Act
            var result = reader.Run(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Run_AllowsNullEnvironment()
        {
            // Arrange
            var reader = new Reader<string, int>(env => env == null ? 0 : env.Length);

            // Act
            var result = reader.Run(null);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Run_ExecutesFunctionOnce()
        {
            // Arrange
            var count = 0;

            var reader = new Reader<int, int>(env =>
                {
                    count++;
                    return env;
                });

            // Act
            reader.Run(1);

            // Assert
            Assert.Equal(1, count);
        }
    }
}
