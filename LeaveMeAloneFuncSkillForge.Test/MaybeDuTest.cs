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
        public void BindStrict_ShouldPropagateSuccessThroughChain()
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
        public void BindStrict_ShouldReturnNothingWhenAnyStepFails()
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
