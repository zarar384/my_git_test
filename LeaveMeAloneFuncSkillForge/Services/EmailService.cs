using LeaveMeAloneFuncSkillForge.DiscriminatedUnions;

namespace LeaveMeAloneFuncSkillForge.Services
{
    public class EmailService
    {
        public EmailSendResult SendEmail(string to, string subject, string body)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(to))
                {
                    throw new ArgumentException("Recipient address is required.", nameof(to));
                }

                // Email sending logic would go here...
                return new EmailSendResult.EmailSuccess();
            }
            catch (Exception ex)
            {
                return new EmailSendResult.EmailFailure(ex);
            }
        }
    }
}
