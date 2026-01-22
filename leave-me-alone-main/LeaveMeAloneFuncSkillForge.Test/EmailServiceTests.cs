
using LeaveMeAloneFuncSkillForge.DiscriminatedUnions;
using LeaveMeAloneFuncSkillForge.Services;

namespace LeaveMeAloneFuncSkillForge.Test
{
    public class EmailServiceTests
    {
        private readonly EmailService _emailService;

        public EmailServiceTests()
        {
            _emailService = new EmailService();
        }

        [Fact]
        public void SendEmail_ShouldReturnEmailSuccess_WhenRecipientIsValid()
        {
            // Arrange
            string to = "lebowski@pidor.com";
            string subject = "Wheres my money?";
            string body = "Hi, Suka Blyad. Are u fine?";

            // Act
            var result = _emailService.SendEmail(to, subject, body);

            // Assert 
            var message = result switch
            {
                EmailSendResult.EmailSuccess => "Email send successful",
                EmailSendResult.EmailFailure ef => "Error occurred sending the email: " + ef.Error.Message,
                _ => "Unknown Response"
            };

            Assert.Equal("Email send successful", message);
        }

        [Fact]
        public void SendEmail_ShouldReturnEmailFailure_WhenRecipientIsEmpty()
        {
            // Arrange
            string to = "";
            string subject = "Wheres my money?";
            string body = "Hi, Suka Blyad. Are u fine?";

            // Act
            var result = _emailService.SendEmail(to, subject, body);

            // Assert
            var message = result switch
            {
                EmailSendResult.EmailSuccess => "Email send successful",
                EmailSendResult.EmailFailure ef => "Error occurred sending the email: " + ef.Error.Message,
                _ => "Unknown Response"
            };

            Assert.Equal($"Error occurred sending the email: Recipient address is required. (Parameter 'to')", message);
        }
    }
}
