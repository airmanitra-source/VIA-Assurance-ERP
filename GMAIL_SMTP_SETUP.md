# Gmail SMTP Configuration Guide for VIA Assurance

## Setup Instructions

### Step 1: Enable 2-Factor Authentication on Gmail

1. Go to: https://myaccount.google.com/security
2. Sign in to your Google account (airmanitra@gmail.com)
3. Look for "2-Step Verification" and enable it
4. Follow Google's verification process

### Step 2: Generate App Password

1. After enabling 2FA, go to: https://myaccount.google.com/apppasswords
2. Select "Mail" and "Windows Computer" (or your OS)
3. Google will generate a 16-character password
4. Copy this password (you'll need it in Step 3)

### Step 3: Update appsettings.json

Replace `your_app_password_here` with the 16-character password from Step 2:

```json
"Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "airmanitra@gmail.com",
    "SenderPassword": "xxxx xxxx xxxx xxxx",  // Paste the 16-char password here
    "SenderName": "VIA Assurance"
}
```

### Step 4: Security Best Practices

For production:
- Use environment variables instead of storing passwords in appsettings.json
- Use Azure Key Vault or similar secure storage
- Never commit passwords to source control

Example with environment variable:
```json
"SenderPassword": "${Email:SenderPassword}"  // Will read from environment variable
```

## Testing

When you create a new user:
1. Go to /admin/users
2. Click "Add New User"
3. Select role "employee" and select an employee with an email
4. Enter password and confirm
5. Click "Create User"
6. Check your mailbox for the password reset email

## Troubleshooting

### Email not received
- Check the application logs for SMTP errors
- Verify the Gmail app password is correct
- Ensure 2-Step Verification is enabled
- Check spam/junk folder

### SMTP Connection Error
- Verify firewall isn't blocking port 587
- Ensure 2FA is enabled (required for app passwords)
- Check that app password is correct (16 characters with spaces)

### Error: "Less secure app access"
- Google may have disabled this. Use app passwords method (Step 2) instead

## Credentials Used

- **Email**: airmanitra@gmail.com
- **SMTP Server**: smtp.gmail.com
- **SMTP Port**: 587 (TLS/STARTTLS)
- **Sender Name**: VIA Assurance

## Implementation Details

The EmailService now:
✅ Uses SMTP to send emails
✅ Supports Gmail with TLS/STARTTLS
✅ Generates HTML formatted emails
✅ Logs email sending attempts
✅ Has proper error handling
✅ Falls back to logging if password not configured

## File Changes

1. **FileTable.Infrastructure\Services\EmailService.cs**
   - Added SMTP implementation
   - Reads configuration from appsettings.json
   - Sends actual emails via Gmail

2. **ClientApp\appsettings.json**
   - Added Email configuration section
   - SMTP server settings
   - Gmail credentials (password placeholder)
