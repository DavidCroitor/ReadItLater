using System;
using System.Net;
using System.Net.Mail;
using Domain.Interfaces;
using Domain.Primitives;
using Domain.Primitives.Result;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class SmtpEmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(
        IOptions<SmtpSettings> smtpSettings, 
        IConfiguration configuration,
        ILogger<SmtpEmailService> logger)
    {
        _smtpSettings = smtpSettings.Value ?? throw new ArgumentNullException(nameof(smtpSettings));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result> SendEmailAsync(
        string to, 
        string subject, 
        string htmlBody,
        bool isHtml = true)
    {
        try
        {
            using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = _smtpSettings.EnableSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.FromAddress, _smtpSettings.FromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = isHtml,
            };

            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent successfully to {To}", to);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            return Result.Fail("Failed to send email");
        }
    }


    public Task<Result> SendAccountVerificationEmailAsync(
        string to, 
        string verificationLink)
    {
        var appName = _configuration.GetValue<string>("AppSettings:AppName");
        var subject = $"Account Verification for {appName}";
        var body = $"""
        <html>
        <body>
            <p>Hi,</p>
            <p>Thank you for registering with {appName}!</p>
            <p>Please click the link below to verify your email address:</p>
            <p><a href="{verificationLink}">Verify Email</a></p>
            <p>If you cannot click the link, please copy and paste the following URL into your browser:</p>
            <p>{verificationLink}</p>
            <p>Thanks,<br/>The {appName} Team</p>
        </body>
        </html>
        """;

        bool isHtml = true;

        return SendEmailAsync(to, subject, body, isHtml);
    }

    public Task<Result> SendPasswordResetEmailAsync(
        string to, 
        string resetLink)
    {
        var appName = _configuration.GetValue<string>("AppSettings:AppName");
        var expirationMinutes = _configuration.GetValue<int>("Security:PasswordResetExpirationMinutes");
        var subject = $"Password Reset Request for {appName}";
        var body = $"""
        <html>
        <body>
            <p>Hi,</p>
            <p>We received a request to reset your password for your {appName} account.</p>
            <p>Please click the link below to set a new password:</p>
            <p><a href="{resetLink}">Reset Password</a></p>
            <p>If you cannot click the link, please copy and paste the following URL into your browser:</p>
            <p>{resetLink}</p>
            <p>This link is valid for the next {expirationMinutes} minutes.</p>
            <p><strong>If you did not request a password reset, please ignore this email.</strong></p>
            <p>Thanks,<br/>The {appName} Team</p>
        </body>
        </html>
        """;

        bool isHtml = true;

        return SendEmailAsync(to, subject, body, isHtml);

    }
}