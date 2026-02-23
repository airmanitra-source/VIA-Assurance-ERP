using System;
using System.Collections.Generic;
using System.Text;

namespace FileTable.Infrastructure.Services
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string email, string userName, string resetToken, string resetUrl);
        Task SendPasswordResetConfirmationEmailAsync(string email, string userName);
    }
}
