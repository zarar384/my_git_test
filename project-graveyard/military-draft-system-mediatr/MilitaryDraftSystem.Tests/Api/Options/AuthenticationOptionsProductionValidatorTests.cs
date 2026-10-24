using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using MilitaryDraftSystem.API.Options;

namespace MilitaryDraftSystem.Tests.API.Options
{
    public class AuthenticationOptionsProductionValidatorTests
    {
        private sealed class FakeHostEnvironment : IHostEnvironment
        {
            public string EnvironmentName { get; set; } = string.Empty;
            public string ApplicationName { get; set; } = "MilitaryDraftSystem.Tests";
            public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
            public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        }

        private static AuthenticationOptionsProductionValidator CreateValidator(string environmentName)
        {
            var environment = new FakeHostEnvironment { EnvironmentName = environmentName };
            return new AuthenticationOptionsProductionValidator(environment);
        }

        [Fact]
        public void Validate_ProductionWithRealApiKey_Succeeds()
        {
            var validator = CreateValidator(Environments.Production);
            var options = new AuthenticationOptions { ApiKey = "a-real-production-secret" };

            var result = validator.Validate(null, options);

            Assert.True(result.Succeeded);
        }

        [Fact]
        public void Validate_ProductionWithDevelopmentDefault_Fails()
        {
            var validator = CreateValidator(Environments.Production);
            var options = new AuthenticationOptions { ApiKey = AuthenticationOptions.DevelopmentApiKey };

            var result = validator.Validate(null, options);

            Assert.True(result.Failed);
            Assert.Contains("development default", result.FailureMessage);
        }

        [Fact]
        public void Validate_DevelopmentWithDevelopmentDefault_Succeeds()
        {
            var validator = CreateValidator(Environments.Development);
            var options = new AuthenticationOptions { ApiKey = AuthenticationOptions.DevelopmentApiKey };

            var result = validator.Validate(null, options);

            Assert.True(result.Succeeded);
        }
    }
}
