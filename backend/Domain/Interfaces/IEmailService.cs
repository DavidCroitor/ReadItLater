using Domain.Primitives.Result;

namespace Domain.Interfaces
{
    public interface IEmailService
    {
        Task<Result> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task<Result> SendPasswordResetEmailAsync(string to, string resetLink);
        Task<Result> SendAccountVerificationEmailAsync(string to, string verificationLink);
    }
}