using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace FileTable.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Sends a password reset email to the specified email address with a reset link
    /// </summary>
    public async Task SendPasswordResetEmailAsync(string email, string userName, string resetToken, string resetUrl)
    {
        try
        {
            // Encode the reset token for URL safety
            var encodedToken = HttpUtility.UrlEncode(resetToken);
            var fullResetUrl = $"{resetUrl}?token={encodedToken}&email={HttpUtility.UrlEncode(email)}";

            var subject = "Password Reset Request - VIA Assurance";
            var defaultPassword = _configuration["UserDefaults:DefaultPassword"]?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(defaultPassword))
            {
                _logger.LogWarning("Default password is not configured. Email will not include default password.");
            }
            var htmlBody = GeneratePasswordResetEmailHtml(userName, fullResetUrl, defaultPassword);

            await SendEmailAsync(email, subject, htmlBody);
            _logger.LogInformation($"Password reset email sent successfully to: {email}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending password reset email to {email}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Sends a password reset confirmation email after successful password reset
    /// </summary>
    public async Task SendPasswordResetConfirmationEmailAsync(string email, string userName)
    {
        try
        {
            var subject = "Password Reset Confirmation - VIA Assurance";
            var htmlBody = GeneratePasswordResetConfirmationEmailHtml(userName);

            await SendEmailAsync(email, subject, htmlBody);
            _logger.LogInformation($"Password reset confirmation email sent successfully to: {email}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending password reset confirmation email to {email}: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Sends an email using SMTP
    /// </summary>
    private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        try
        {
            var smtpServer = _configuration["Email:SmtpServer"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var senderEmail = _configuration["Email:SenderEmail"] ?? "airmanitra@gmail.com";
            var senderPassword = _configuration["Email:SenderPassword"];
            var senderName = _configuration["Email:SenderName"] ?? "VIA Assurance";

            if (string.IsNullOrWhiteSpace(senderPassword))
            {
                _logger.LogWarning("SMTP sender password not configured. Email will not be sent.");
                await LogEmailAsync(toEmail, subject, htmlBody);
                return;
            }

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(senderEmail, senderPassword);
                client.Timeout = 10000;

                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(senderEmail, senderName);
                    mailMessage.To.Add(new MailAddress(toEmail));
                    mailMessage.Subject = subject;
                    mailMessage.Body = htmlBody;
                    mailMessage.IsBodyHtml = true;

                    _logger.LogInformation($"Attempting to send email to {toEmail} via {smtpServer}:{smtpPort}");
                    await client.SendMailAsync(mailMessage);
                }
            }
        }
        catch (SmtpException smtpEx)
        {
            _logger.LogError($"SMTP Error: {smtpEx.StatusCode} - {smtpEx.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"General error sending email: {ex.Message}");
            throw;
        }
    }

    private string GeneratePasswordResetEmailHtml(string userName, string resetUrl, string defaultPassword)
    {
        var defaultPasswordSection = string.IsNullOrWhiteSpace(defaultPassword)
            ? "<p><strong>Default Password:</strong> Not configured. Please contact support.</p>"
            : $"<p><strong>Default Password (required for reset):</strong> {WebUtility.HtmlEncode(defaultPassword)}</p>";

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px; }}
        .header {{ background-color: #1e318c; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ padding: 20px; }}
        .button {{ display: inline-block; padding: 12px 24px; background-color: #007bff; color: white; text-decoration: none; border-radius: 4px; margin: 20px 0; }}
        .footer {{ background-color: #f9f9f9; padding: 15px; text-align: center; font-size: 12px; color: #666; border-radius: 0 0 8px 8px; }}
        .warning {{ background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 10px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>VIA Assurance</h1>
            <p>Password Reset Request</p>
        </div>
        <div class='content'>
            <p>Hello {userName},</p>
            <p>You have been created as a user in the VIA Assurance system. To activate your account and set your password, please click the button below:</p>
            
            <center>
                <a href='{resetUrl}' class='button'>Reset Your Password</a>
            </center>
            
            <p>Or copy and paste this link in your browser:</p>
            <p style='word-break: break-all;'><code>{resetUrl}</code></p>
            
            {defaultPasswordSection}

            <div class='warning'>
                <strong>⚠️ Important:</strong> This link will expire in 24 hours for security reasons. If you did not request this, please contact the administrator.
            </div>
            
            <p>Best regards,<br/>VIA Assurance Team</p>
        </div>
        <div class='footer'>
            <p>&copy; 2026 VIA Assurance. All rights reserved.</p>
            <p>This is an automated message. Please do not reply to this email.</p>
        </div>
    </div>
</body>
</html>";
    }

    private string GeneratePasswordResetConfirmationEmailHtml(string userName)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 8px; }}
        .header {{ background-color: #1e318c; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ padding: 20px; }}
        .footer {{ background-color: #f9f9f9; padding: 15px; text-align: center; font-size: 12px; color: #666; border-radius: 0 0 8px 8px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>VIA Assurance</h1>
            <p>Password Reset Confirmation</p>
        </div>
        <div class='content'>
            <p>Hello {userName},</p>
            <p>Your password has been successfully reset. You can now log in to your account using your new password.</p>
            
            <p>If you did not initiate this change, please contact the administrator immediately.</p>
            
            <p>Best regards,<br/>VIA Assurance Team</p>
        </div>
        <div class='footer'>
            <p>&copy; 2026 VIA Assurance. All rights reserved.</p>
            <p>This is an automated message. Please do not reply to this email.</p>
        </div>
    </div>
</body>
</html>";
    }

    private async Task LogEmailAsync(string email, string subject, string htmlBody)
    {
        // Log the email for development/testing purposes
        _logger.LogInformation($"=== EMAIL LOGGED (NOT SENT) ===");
        _logger.LogInformation($"To: {email}");
        _logger.LogInformation($"Subject: {subject}");
        _logger.LogInformation($"Body Preview: {htmlBody.Substring(0, Math.Min(200, htmlBody.Length))}...");
        _logger.LogInformation($"================================");

        await Task.CompletedTask;
    }
}
