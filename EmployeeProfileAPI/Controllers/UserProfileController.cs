using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using BCrypt.Net;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Net.Mail;

namespace EmployeeProfileAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public UserProfileController(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _environment = environment;
            _configuration = configuration;
        }

        // GET: api/userprofile/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserProfile(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var query = @"
                        SELECT UserID, FullName, Department, EmployeeID, Email, 
                               SkillLevel, StartDate, ProjectsCompleted, ProfileImage
                        FROM [EmployeeProfileDB].[dbo].[UserProfile]
                        WHERE UserID = @UserID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var userProfile = new
                                {
                                    userID = reader.GetInt32("UserID"),
                                    fullName = reader.IsDBNull("FullName") ? null : reader.GetString("FullName"),
                                    department = reader.IsDBNull("Department") ? null : reader.GetString("Department"),
                                    employeeID = reader.IsDBNull("EmployeeID") ? null : reader.GetString("EmployeeID"),
                                    email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                                    skillLevel = reader.IsDBNull("SkillLevel") ? "Junior" : reader.GetString("SkillLevel"),
                                    startDate = reader.IsDBNull("StartDate") ? (DateTime?)null : reader.GetDateTime("StartDate"),
                                    projectsCompleted = reader.IsDBNull("ProjectsCompleted") ? 0 : reader.GetInt32("ProjectsCompleted"),
                                    profileImage = reader.IsDBNull("ProfileImage") ? null : reader.GetString("ProfileImage")
                                };

                                return Ok(userProfile);
                            }
                            else
                            {
                                return NotFound(new { message = "User profile not found" });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving user profile", error = ex.Message });
            }
        }

        // PUT: api/userprofile/updateprofile/{id}
        [HttpPut("updateprofile/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateProfileRequest request)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(request.FullName) || 
                    string.IsNullOrWhiteSpace(request.Department) || 
                    string.IsNullOrWhiteSpace(request.Email))
                {
                    return BadRequest(new { message = "Name, department, and email are required" });
                }

                // Validate email format
                if (!IsValidEmail(request.Email))
                {
                    return BadRequest(new { message = "Please enter a valid email address" });
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Check if user exists
                    var checkQuery = "SELECT COUNT(*) FROM [EmployeeProfileDB].[dbo].[UserProfile] WHERE UserID = @UserID";
                    using (var checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@UserID", id);
                        var exists = (int)await checkCommand.ExecuteScalarAsync() > 0;
                        
                        if (!exists)
                        {
                            return NotFound(new { message = "User not found" });
                        }
                    }

                    var updateQuery = @"
                        UPDATE [EmployeeProfileDB].[dbo].[UserProfile]
                        SET FullName = @FullName,
                            Department = @Department,
                            Email = @Email,
                            SkillLevel = @SkillLevel,
                            StartDate = @StartDate,
                            ProjectsCompleted = @ProjectsCompleted
                        WHERE UserID = @UserID";

                    using (var command = new SqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", id);
                        command.Parameters.AddWithValue("@FullName", request.FullName);
                        command.Parameters.AddWithValue("@Department", request.Department);
                        command.Parameters.AddWithValue("@Email", request.Email);
                        command.Parameters.AddWithValue("@SkillLevel", request.SkillLevel ?? "Junior");
                        command.Parameters.AddWithValue("@StartDate", request.StartDate?.Date ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ProjectsCompleted", request.ProjectsCompleted);

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        
                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Profile updated successfully" });
                        }
                        else
                        {
                            return BadRequest(new { message = "Failed to update profile" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating profile", error = ex.Message });
            }
        }

        // PUT: api/userprofile/updateprofilepicture/{id}
        [HttpPut("updateprofilepicture/{id}")]
        public async Task<IActionResult> UpdateProfilePicture(int id, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "No file uploaded" });
                }

                // Validate file type
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                {
                    return BadRequest(new { message = "Only image files (JPEG, PNG, GIF) are allowed" });
                }

                // Validate file size (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { message = "File size cannot exceed 5MB" });
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Check if user exists
                    var checkQuery = "SELECT COUNT(*) FROM [EmployeeProfileDB].[dbo].[UserProfile] WHERE UserID = @UserID";
                    using (var checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@UserID", id);
                        var exists = (int)await checkCommand.ExecuteScalarAsync() > 0;
                        
                        if (!exists)
                        {
                            return NotFound(new { message = "User not found" });
                        }
                    }

                    // Create uploads directory if it doesn't exist
                    var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "profiles");
                    if (!Directory.Exists(uploadsPath))
                    {
                        Directory.CreateDirectory(uploadsPath);
                    }

                    // Generate unique filename
                    var fileExtension = Path.GetExtension(file.FileName);
                    var fileName = $"profile_{id}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    // Save file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Update database
                    var relativePath = $"/uploads/profiles/{fileName}";
                    var updateQuery = @"
                        UPDATE [EmployeeProfileDB].[dbo].[UserProfile]
                        SET ProfileImage = @ProfileImage
                        WHERE UserID = @UserID";

                    using (var command = new SqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", id);
                        command.Parameters.AddWithValue("@ProfileImage", relativePath);

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        
                        if (rowsAffected > 0)
                        {
                            return Ok(new { 
                                message = "Profile image updated successfully", 
                                profileImage = relativePath 
                            });
                        }
                        else
                        {
                            return BadRequest(new { message = "Failed to update profile image" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating profile image", error = ex.Message });
            }
        }

        // POST: api/userprofile/requestpasswordchange/{id}
        [HttpPost("requestpasswordchange/{id}")]
        public async Task<IActionResult> RequestPasswordChange(int id, [FromBody] PasswordChangeRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.CurrentPassword) || 
                    string.IsNullOrWhiteSpace(request.NewPassword))
                {
                    return BadRequest(new { message = "Current password and new password are required" });
                }

                if (request.NewPassword.Length < 6)
                {
                    return BadRequest(new { message = "New password must be at least 6 characters long" });
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Get user details and verify current password
                    var getUserQuery = @"
                        SELECT Email, FullName, PasswordHash 
                        FROM [EmployeeProfileDB].[dbo].[UserProfile] 
                        WHERE UserID = @UserID";

                    using (var command = new SqlCommand(getUserQuery, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", id);
                        
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var email = reader.GetString("Email");
                                var fullName = reader.GetString("FullName");
                                var currentPasswordHash = reader.GetString("PasswordHash");

                                // Verify current password
                                if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, currentPasswordHash))
                                {
                                    return BadRequest(new { message = "Current password is incorrect" });
                                }

                                // Generate verification token
                                var verificationToken = GenerateVerificationToken();
                                var tokenExpiry = DateTime.UtcNow.AddMinutes(30); // Token expires in 30 minutes

                                // Hash the new password temporarily
                                var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

                                reader.Close();

                                // Store verification token in database
                                var insertTokenQuery = @"
                                    INSERT INTO [EmployeeProfileDB].[dbo].[PasswordChangeVerification]
                                    (UserID, VerificationToken, NewPasswordHash, TokenExpiry, IsUsed)
                                    VALUES (@UserID, @VerificationToken, @NewPasswordHash, @TokenExpiry, 0)";

                                using (var insertCommand = new SqlCommand(insertTokenQuery, connection))
                                {
                                    insertCommand.Parameters.AddWithValue("@UserID", id);
                                    insertCommand.Parameters.AddWithValue("@VerificationToken", verificationToken);
                                    insertCommand.Parameters.AddWithValue("@NewPasswordHash", newPasswordHash);
                                    insertCommand.Parameters.AddWithValue("@TokenExpiry", tokenExpiry);

                                    await insertCommand.ExecuteNonQueryAsync();
                                }

                                // Send verification email
                                await SendVerificationEmail(email, fullName, verificationToken, id);

                                return Ok(new { 
                                    message = "Verification email sent. Please check your email to confirm the password change.",
                                    tokenExpiry = tokenExpiry
                                });
                            }
                            else
                            {
                                return NotFound(new { message = "User not found" });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error processing password change request", error = ex.Message });
            }
        }

        // POST: api/userprofile/verifypasswordchange
        [HttpPost("verifypasswordchange")]
        public async Task<IActionResult> VerifyPasswordChange([FromBody] VerifyPasswordChangeRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Token))
                {
                    return BadRequest(new { message = "Verification token is required" });
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Get token details
                    var getTokenQuery = @"
                        SELECT UserID, NewPasswordHash, TokenExpiry, IsUsed
                        FROM [EmployeeProfileDB].[dbo].[PasswordChangeVerification]
                        WHERE VerificationToken = @Token";

                    using (var command = new SqlCommand(getTokenQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Token", request.Token);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var userId = reader.GetInt32("UserID");
                                var newPasswordHash = reader.GetString("NewPasswordHash");
                                var tokenExpiry = reader.GetDateTime("TokenExpiry");
                                var isUsed = reader.GetBoolean("IsUsed");

                                reader.Close();

                                // Check if token is expired
                                if (DateTime.UtcNow > tokenExpiry)
                                {
                                    return BadRequest(new { message = "Verification token has expired" });
                                }

                                // Check if token is already used
                                if (isUsed)
                                {
                                    return BadRequest(new { message = "Verification token has already been used" });
                                }

                                // Update password in UserProfile table
                                var updatePasswordQuery = @"
                                    UPDATE [EmployeeProfileDB].[dbo].[UserProfile]
                                    SET PasswordHash = @PasswordHash
                                    WHERE UserID = @UserID";

                                using (var updateCommand = new SqlCommand(updatePasswordQuery, connection))
                                {
                                    updateCommand.Parameters.AddWithValue("@UserID", userId);
                                    updateCommand.Parameters.AddWithValue("@PasswordHash", newPasswordHash);

                                    var rowsAffected = await updateCommand.ExecuteNonQueryAsync();
                                    
                                    if (rowsAffected > 0)
                                    {
                                        // Mark token as used
                                        var markTokenUsedQuery = @"
                                            UPDATE [EmployeeProfileDB].[dbo].[PasswordChangeVerification]
                                            SET IsUsed = 1
                                            WHERE VerificationToken = @Token";

                                        using (var markUsedCommand = new SqlCommand(markTokenUsedQuery, connection))
                                        {
                                            markUsedCommand.Parameters.AddWithValue("@Token", request.Token);
                                            await markUsedCommand.ExecuteNonQueryAsync();
                                        }

                                        return Ok(new { message = "Password changed successfully" });
                                    }
                                    else
                                    {
                                        return BadRequest(new { message = "Failed to change password" });
                                    }
                                }
                            }
                            else
                            {
                                return BadRequest(new { message = "Invalid verification token" });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error verifying password change", error = ex.Message });
            }
        }

        // POST: api/userprofile/resendverification/{id}
        [HttpPost("resendverification/{id}")]
        public async Task<IActionResult> ResendVerification(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Get the latest unused verification token
                    var getLatestTokenQuery = @"
                        SELECT TOP 1 v.VerificationToken, u.Email, u.FullName
                        FROM [EmployeeProfileDB].[dbo].[PasswordChangeVerification] v
                        INNER JOIN [EmployeeProfileDB].[dbo].[UserProfile] u ON v.UserID = u.UserID
                        WHERE v.UserID = @UserID AND v.IsUsed = 0 AND v.TokenExpiry > GETUTCDATE()
                        ORDER BY v.TokenExpiry DESC";

                    using (var command = new SqlCommand(getLatestTokenQuery, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var token = reader.GetString("VerificationToken");
                                var email = reader.GetString("Email");
                                var fullName = reader.GetString("FullName");

                                reader.Close();

                                // Resend verification email
                                await SendVerificationEmail(email, fullName, token, id);

                                return Ok(new { message = "Verification email resent successfully" });
                            }
                            else
                            {
                                return BadRequest(new { message = "No pending verification request found" });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error resending verification email", error = ex.Message });
            }
        }

        // Helper method to generate verification token
        private string GenerateVerificationToken()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[32];
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
            }
        }

        // Helper method to send verification email
        private async Task SendVerificationEmail(string email, string fullName, string token, int userId)
        {
            try
            {
                // Get SMTP configuration from appsettings.json
                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var smtpUsername = _configuration["EmailSettings:SmtpUsername"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
                var fromEmail = _configuration["EmailSettings:FromEmail"];
                var fromName = _configuration["EmailSettings:FromName"];

                var verificationUrl = $"{_configuration["AppSettings:BaseUrl"]}/verify-password-change?token={token}&userId={userId}";

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = "Password Change Verification",
                    Body = $@"
                        <html>
                        <body>
                            <h2>Password Change Verification</h2>
                            <p>Dear {fullName},</p>
                            <p>We received a request to change your password. To complete this request, please click the verification link below:</p>
                            <p><a href='{verificationUrl}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Verify Password Change</a></p>
                            <p>If you didn't request this password change, please ignore this email.</p>
                            <p>This link will expire in 30 minutes.</p>
                            <p>Best regards,<br>Your Company Team</p>
                        </body>
                        </html>",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    smtpClient.EnableSsl = true;
                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                // Log the error (you might want to use a proper logging framework)
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw new Exception("Failed to send verification email");
            }
        }

        // Helper method to validate email
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }

    // Request models
    public class UpdateProfileRequest
    {
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string SkillLevel { get; set; }
        public DateTime? StartDate { get; set; }
        public int ProjectsCompleted { get; set; }
    }

    public class PasswordChangeRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class VerifyPasswordChangeRequest
    {
        public string Token { get; set; }
    }
}