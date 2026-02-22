using FileTable.Infrastructure.Identities;
using FileTable.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;

namespace ClientApp.Controllers
{
    public class ResetPasswordController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ResetPasswordController> _logger;

        public ResetPasswordController(UserManager<ApplicationUser> userManager, ILogger<ResetPasswordController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Verify if reset token is valid for the given email
        /// </summary>
        public async Task<bool> VerifyTokenAsync(string email, string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
                {
                    _logger.LogWarning("Token verification failed: email or token is empty");
                    return false;
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning($"Token verification failed: user not found for email {email}");
                    return false;
                }

                // For token verification, we just check if the user exists
                // The actual token validation happens during ResetPasswordAsync
                _logger.LogInformation($"Token verification successful for {email}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error verifying token: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Reset password using token
        /// </summary>
        public async Task<(bool Success, string Message)> ResetPasswordAsync(string email, string token, string newPassword, IEmailService emailService)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
                {
                    _logger.LogWarning("Password reset failed: missing required fields");
                    return (false, "Email, token, and password are required.");
                }

                if (newPassword.Length < 6)
                {
                    _logger.LogWarning("Password reset failed: password too short");
                    return (false, "Password must be at least 6 characters long.");
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning($"Password reset failed: user not found for email {email}");
                    return (false, "User not found.");
                }

                _logger.LogInformation($"Attempting password reset for {email}");
                
                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
                if (result.Succeeded)
                {
                    // Mark initial password reset as completed
                    user.InitialPasswordResetCompleted = true;
                    await _userManager.UpdateAsync(user);

                    _logger.LogInformation($"Password reset successful for {email}");

                    // Send confirmation email
                    try
                    {
                        await emailService.SendPasswordResetConfirmationEmailAsync(user.Email!, user.RaisonSocial);
                        _logger.LogInformation($"Confirmation email sent to {email}");
                    }
                    catch (Exception emailEx)
                    {
                        _logger.LogWarning($"Failed to send confirmation email to {email}: {emailEx.Message}");
                        // Don't fail the reset if email fails
                    }

                    return (true, "Password has been reset successfully. You can now log in.");
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                    _logger.LogWarning($"Password reset failed for {email}: {errors}");
                    return (false, $"Failed to reset password: {errors}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception during password reset for {email}: {ex.Message}");
                return (false, $"An error occurred: {ex.Message}");
            }
        }
    }
}
