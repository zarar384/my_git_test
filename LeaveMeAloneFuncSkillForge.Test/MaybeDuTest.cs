using LeaveMeAloneFuncSkillForge.DiscriminatedUnions;
using LeaveMeAloneFuncSkillForge.Domain;
using LeaveMeAloneFuncSkillForge.DTOs;
using LeaveMeAloneFuncSkillForge.Common;
using LeaveMeAloneFuncSkillForge.Repositories.Interfaces;
using Moq;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class MaybeDuTest
    {
        [Fact]
        public void NothingExample_ShouldReturnNoFilmFoundMessage()
        {
            // Arrange
            var mockRepo = new Mock<IFilmRepository>();

            // Act
            var film = GetFilmFirstFilmByGenre("NonExistentGenre", mockRepo.Object);
            var message = film switch
            {
                Something<Film> something => $"Found film: {something.Value.Title}",
                Nothing<Film> _ => "No film found",
                _ => throw new NotImplementedException(),
            };

            // Assert
            Assert.Equal("No film found", message);
        }

        [Fact]
        public void SomethingExample_ShouldReturnFoundFilmMessage()
        {
            // Arrange
            var mockRepo = new Mock<IFilmRepository>();
            mockRepo.Setup(r => r.GetFilmsByGenre("Drama")).Returns(new List<Film>
            {
                new Film { Title = "Film I", BoxOfficeRevenue = 200 },
                new Film { Title = "Film HATE", BoxOfficeRevenue = 300 },
                new Film { Title = "Film MYSELF", BoxOfficeRevenue = 100 }
            });

            // Act
            var film = GetFilmFirstFilmByGenre("Drama", mockRepo.Object)!;
            var message = film switch
            {
                Something<Film> something => $"Found film: {something.Value.Title}",
                Nothing<Film> _ => "No film found",
                _ => throw new NotImplementedException(),
            };

            // Assert
            Assert.StartsWith("Found film: ", message);
        }

        [Fact]
        public void ErrorExample_ShouldReturnErrorMessage()
        {
            // Arrange
            var mockRepo = new Mock<IFilmRepository>();
            mockRepo
                .Setup(r => r.GetFilmsByGenre("Thriller"))
                .Throws(new Exception("Database is down"));

            // Act
            var film = GetFilmFirstFilmByGenre_ExceptionSafe("Thriller", mockRepo.Object);

            var message = film switch
            {
                Something<Film> s => $"Found film: {s.Value.Title}",
                Nothing<Film> _ => "No film found",
                Error<Film> e => $"Error happened: {e.CapturedError.Message}",
                _ => throw new NotImplementedException(),
            };

            // Assert
            Assert.Equal("Error happened: Database is down", message);
        }

        [Fact]
        public void ErrorExample_ShouldReturnErrorType()
        {
            // Arrange
            var mockRepo = new Mock<IFilmRepository>();
            mockRepo.Setup(r => r.GetFilmsByGenre("Comedy"))
                .Throws(new InvalidOperationException("Something bad"));

            // Act
            var result = GetFilmFirstFilmByGenre_ExceptionSafe("Comedy", mockRepo.Object);

            // Assert
            Assert.IsType<Error<Film>>(result);
            Assert.Equal("Something bad", ((Error<Film>)result).CapturedError.Message);
        }

        private Maybe<Film> GetFilmFirstFilmByGenre(string genre, 
            IFilmRepository filmRepository)
        {
            Maybe<Film> maybeFilm = filmRepository.GetFilmsByGenre(genre)
                .FirstOrDefault() is { } film
                    ? new Something<Film>(film)
                    : new Nothing<Film>();
            return maybeFilm;
        }

        private Maybe<Film> GetFilmFirstFilmByGenre_ExceptionSafe(string genre, 
            IFilmRepository filmRepository)
        {
            try
            {
                var films = filmRepository.GetFilmsByGenre(genre);

                return films.FirstOrDefault() is { } film
                    ? new Something<Film>(film)
                    : new Nothing<Film>();
            }
            catch (Exception ex)
            {
                return new Error<Film>(ex);
            }
        }

        [Fact]
        public void BindSafe_ShouldPropagateSuccessThroughChain()
        {
            // Arrange
            var input = new Something<string>("42");

            // Act
            var result = input
                    .BindSafe(EnsureNotEmpty)
                    .BindSafe(ParseToInt)
                    .BindSafe(ComputeHalf)
                    .BindSafe(FormatResult);

            // Assert
            Assert.True(result is Something<string>);
            Assert.Equal("Result is 21", ((Something<string>)result).Value);
        }

        [Fact]
        public void BindSafe_ShouldReturnNothingWhenAnyStepFails()
        {
            // Arrange
            var input = new Something<string>("not number");

            // Act
            var result =input
                    .BindSafe(EnsureNotEmpty)
                    .BindSafe(ParseToInt)      // fails here => Nothing<int>
                    .BindSafe(ComputeHalf)     // skipped 
                    .BindSafe(FormatResult);   // skipped 

            // Assert
            Assert.True(result is Nothing<string>);
        }

        [Fact]
        public void ComplexBindStrict_UserRegistrationPipeline_FinalMaybeAndLogsCorrectly()
        {
            // Arrange
            var log = new List<string>();

            // user input, could come from API request
            var input = new Something<UserInputDto>(new UserInputDto
            {
                Username = "john_doe",
                Password = "password123",
                Email = "john@example.com"
            });

            // Act
            var result = input
                // validate input
                .BindStrict(x =>
                {
                    if (string.IsNullOrWhiteSpace(x.Username) || string.IsNullOrWhiteSpace(x.Password))
                        throw new ArgumentException("Username or password is empty");
                    return x;
                })
                .OnSomething(x => log.Add($"Validated input for user: {x.Username}"))
                .OnNothing(() => log.Add("Input validation failed"))
                .OnError(ex => log.Add($"Validation error: {ex.Message}"))

                // check if username exists
                .BindStrict(x =>
                {
                    var existingUsers = new HashSet<string> { "alice", "bob" };
                    if (existingUsers.Contains(x.Username)) return x;
                    return x;
                })
                .OnSomething(x => log.Add($"Username {x.Username} is available"))
                .OnNothing(() => log.Add($"Username already exists"))
                .OnError(ex => log.Add($"Error checking username: {ex.Message}"))

                // hash password
                .BindStrict(x =>
                {
                    x.Password = $"hashed_{x.Password}";
                    return x;
                })
                .OnSomething(x => log.Add($"Password hashed for {x.Username}"))
                .OnNothing(() => log.Add("Skipping password hash"))
                .OnError(ex => log.Add($"Error hashing password: {ex.Message}"))

                // save user
                .BindStrict(x =>
                {
                    if (x.Username == "fail_save") throw new Exception("Database error");
                    return x;
                })
                .OnSomething(x => log.Add($"User {x.Username} saved to database"))
                .OnNothing(() => log.Add("User not saved"))
                .OnError(ex => log.Add($"Save error: {ex.Message}"))

                // send welcome email
                .BindStrict(x =>
                {
                    if (x.Email.EndsWith("@example.com")) throw new Exception("Email delivery failed");
                    return x;
                })
                .OnSomething(x => log.Add($"Welcome email sent to {x.Email}"))
                .OnNothing(() => log.Add("Email not sent"))
                .OnError(ex => log.Add($"Email error: {ex.Message}"));

            // Assert
            // final Maybe is UnhandledError because sending email fails for @example.com
            Assert.IsType<UnhandledError<UserInputDto>>(result);

            // that log contains expected entries
            Assert.Contains(log, l => l.Contains("Validated input"));
            Assert.Contains(log, l => l.Contains("Username john_doe is available"));
            Assert.Contains(log, l => l.Contains("Password hashed"));
            Assert.Contains(log, l => l.Contains("User john_doe saved to database"));
            Assert.Contains(log, l => l.Contains("Email error: Email delivery failed"));
        }


        [Fact]
        public void Complex_BindStrictNested_WithMultipleStages_WorksCorrectly()
        {
            // Arrange
            var log = new List<string>();

            Maybe<Maybe<UserInputDto>> input =
                new Something<Maybe<UserInputDto>>(
                    new Something<UserInputDto>(
                        new UserInputDto
                        {
                            Username = "john_doe",
                            Password = "secret123",
                            Email = "john@example.com"
                        }));

            // Act
            var result = input
                .BindStrict(x =>
                {
                    log.Add("Stage1: validate");
                    if (string.IsNullOrWhiteSpace(x.Username))
                        throw new ArgumentException("Username missing");

                    log.Add("Stage2: hash password");
                    x.Password = $"hashed_{x.Password}";

                    log.Add("Stage3: check username");
                    if (x.Username == "john_doe") throw new Exception("User exists");

                    log.Add("Stage4: final adjustments");
                    x.Email = x.Email.ToLower();

                    return x;
                })
                .OnSomething(x => log.Add("Result: Something"))
                .OnNothing(() => log.Add("Result: Nothing"))
                .OnError(e => log.Add($"Result: Error - {e.Message}"));

            // Assert
            // final Maybe is UnhandledError because username check fails - already exists
            Assert.IsType<UnhandledError<UserInputDto>>(result);
            var error = (Error<UserInputDto>)result;
            Assert.Equal("User exists", error.CapturedError.Message);

            // that log contains expected entries
            Assert.Equal("Stage1: validate", log[0]);
            Assert.Equal("Stage2: hash password", log[1]);
            Assert.Equal("Stage3: check username", log[2]);
            Assert.Equal("Result: Error - User exists", log[3]);
        }

        [Fact]
        public void BindStrict_ShouldReturnError_WhenInputIsError()
        {
            // Arrange
            var input = new Error<string>(new InvalidOperationException("Initial error"));

            // Act
            var result = input.BindStrict(x =>
            {
                return x.ToUpper();
            });

            // Assert
            Assert.IsType<Error<string>>(result);
            var error = (Error<string>)result;
            Assert.Equal("Initial error", error.CapturedError.Message);
        }

        // Left Identity Law
        // Wrapping a value and then binding a function is the same as just applying the function directly.
        [Fact]
        public void LeftIdentityLaw_ShouldReturnSameAsDirectFunction()
        {
            var input = "42";

            var direct = ParseToInt(input) as Something<int>;
            var viaMonad = new Something<string>(input).Bind(ParseToInt) as Something<int>;

            Assert.Equal(direct.Value, viaMonad.Value);
        }

        // Right Identity Law
        // Binding the identity function returns the original monad/value unchanged.
        [Fact]
        public void RightIdentityLaw_ShouldReturnOriginalValue()
        {
            var input = "100";
            Func<string, Maybe<string>> identity = s => new Something<string>(s);

            var result = new Something<string>(input).Bind(identity) as Something<string>;

            Assert.Equal(input, result.Value);
        }

        // Associativity Law
        // The order of nested binds does not matter; results are the same.
        [Fact]
        public void AssociativityLaw_ShouldProduceSameResultRegardlessOfNesting()
        {
            var input = "50";

            Func<string, Maybe<int>> parse = ParseToInt;
            Func<int, Maybe<double>> half = ComputeHalf;
            Func<double, Maybe<string>> format = FormatResult;

            var versionOne = new Something<string>(input)
                .Bind(parse)
                .Bind(half)
                .Bind(format) as Something<string>;

            var versionTwo = new Something<string>(input)
                .Bind(x => parse(x).Bind(half))
                .Bind(format) as Something<string>;

            Assert.Equal(versionOne.Value, versionTwo.Value);
        }

        // Example of chaining Maybe operations
        private Maybe<string> EnsureNotEmpty(string s) =>
            string.IsNullOrWhiteSpace(s)
            ? new Nothing<string>()
            : new Something<string>(s);

        private Maybe<int> ParseToInt(string s) =>
            int.TryParse(s, out var n)
                ? new Something<int>(n)
                : new Nothing<int>();

        private Maybe<double> ComputeHalf(int x) =>
            x != 0
                ? new Something<double>(x / 2.0)
                : new Nothing<double>();

        private Maybe<string> FormatResult(double d) =>
            new Something<string>($"Result is {d}");
    }
}
