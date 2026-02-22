# Password Reset Troubleshooting Guide

## Issue: "Failed to reset password: Invalid token."

### ✅ **Fix Applied**

The issue was **double URL-decoding** of the reset token:

1. **EmailService** encodes the token:
   ```csharp
   var encodedToken = HttpUtility.UrlEncode(resetToken);
   var fullResetUrl = $"{resetUrl}?token={encodedToken}&email={HttpUtility.UrlEncode(email)}";
   ```

2. **Blazor's [SupplyParameterFromQuery]** automatically URL-decodes query parameters

3. **ResetPassword.razor.cs** was incorrectly decoding again:
   ```csharp
   Token = HttpUtility.UrlDecode(Token);  // ❌ WRONG - Double decode!
   ```

### ✅ **Solution**

Removed manual URL decoding in `ResetPassword.razor.cs`:
- Blazor automatically handles URL decoding for `[SupplyParameterFromQuery]` parameters
- Token is now passed directly to the controller without modification

### ✅ **Files Updated**

1. **ClientApp\Components\Pages\Routed\ResetPassword.razor.cs**
   - Removed `Token = HttpUtility.UrlDecode(Token);`
   - Removed `Email = HttpUtility.UrlDecode(Email);`
   - Added comments explaining Blazor's automatic decoding

2. **ClientApp\Controllers\ResetPasswordController.cs**
   - Added logging for debugging
   - Improved error messages with error codes
   - Better exception handling

### ✅ **How It Works Now**

**Flow:**
```
1. EmailService generates token
   Token = "abc+123/xyz=="
   
2. EmailService URL-encodes token for URL
   encodedToken = "abc%2B123%2Fxyz%3D%3D"
   
3. Email sent with URL:
   /reset-password?token=abc%2B123%2Fxyz%3D%3D&email=user@example.com
   
4. Browser receives URL and passes to Blazor
   
5. Blazor's [SupplyParameterFromQuery] auto-decodes
   Token = "abc+123/xyz==" ✅ (Correct!)
   
6. ResetPassword component receives decoded token
   
7. Controller uses token directly with UserManager
   ✅ Success!
```

### ✅ **Testing the Fix**

1. Go to `/admin/users`
2. Create a new employee user
3. Check inbox for password reset email
4. Click the reset link
5. Enter new password
6. Should now work! ✅

### ✅ **Logging for Debugging**

If you still encounter issues, check the console logs:

**ResetPasswordController logs:**
- `Token verification successful for {email}` - Token is valid
- `Failed to reset password: {errors}` - Specific error from Identity framework
- `Exception during password reset` - Detailed exception

**EmailService logs:**
- `Password reset email sent successfully to: {email}` - Email was sent
- `Error sending password reset email` - Email sending failed

### ⚠️ **If Issues Persist**

1. **Check Application Logs**
   - Look for detailed error messages from ResetPasswordController
   - Check email sending status in EmailService logs

2. **Verify SMTP Configuration**
   - Ensure `appsettings.json` has correct Gmail credentials
   - See `GMAIL_SMTP_SETUP.md` for Gmail app password setup

3. **Check Database**
   - Verify user was created with correct email
   - Check if `InitialPasswordResetCompleted` column exists (run migration)

4. **Verify URL Format**
   - Click email link and check address bar
   - Token should be URL-encoded (e.g., `%2B` for `+`)
   - Email should be readable

### ✅ **Build Status**

✅ All changes compiled successfully
✅ Ready to test password reset flow

## Summary

The password reset flow now properly handles token encoding/decoding:
- ✅ Token is URL-encoded for email link
- ✅ Blazor automatically decodes when receiving query parameter
- ✅ Controller receives properly decoded token
- ✅ Identity framework can validate and reset password
- ✅ User marked as having completed initial reset
