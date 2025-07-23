using Microsoft.AspNetCore.Mvc;
using HRWorkForceSystemBackend.Data;
using HRWorkForceSystemBackend.Models.AuthModels;
using HRWorkForceSystemBackend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using HRWorkForceSystemBackend.DTOs.AuthDTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRWorkForceSystemBackend.Models; // For Employee model

namespace HRWorkForceSystemBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;
        private readonly EmailService _emailService;
        private static readonly Dictionary<string, (string Otp, DateTime ExpiresAt)> _otpStore = new();

        public AuthController(AppDbContext context, TokenService tokenService, EmailService emailService)
        {
            _context = context;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        private static string GenerateOtp() => new Random().Next(100000, 999999).ToString();

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email and password are required.");

            dynamic user = null;
            string role = string.Empty;

            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == request.Email);
            if (admin != null && BCrypt.Net.BCrypt.Verify(request.Password, admin.PasswordHash))
            {
                role = "Admin";
                user = admin;
            }

            if (user == null)
            {
                var hr = await _context.HRUsers.FirstOrDefaultAsync(h => h.Email == request.Email);
                if (hr != null && BCrypt.Net.BCrypt.Verify(request.Password, hr.PasswordHash))
                {
                    role = "HR";
                    user = hr;
                }
            }

            if (user == null)
            {
                var workforceUser = await _context.WorkforceUsers.FirstOrDefaultAsync(w => w.Email == request.Email);
                if (workforceUser != null && BCrypt.Net.BCrypt.Verify(request.Password, workforceUser.PasswordHash))
                {
                    var employee = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Email == request.Email);
                    role = employee?.Designation ?? "Workforce"; // Dynamic role from Designation
                    user = workforceUser;
                }
            }

            if (user == null || string.IsNullOrEmpty(role))
                return Unauthorized("Invalid email or password.");

            var token = _tokenService.CreateToken(user, role);
            return Ok(new { token });
        }

        [Authorize(Roles = "Admin, HR")]
        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterByRole([FromBody] CreateUserDto request)
        {
            var normalizedRole = request.Role?.Trim().ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(normalizedRole) ||
                (normalizedRole != "admin" && normalizedRole != "hr" && normalizedRole != "workforce"))
            {
                return BadRequest("Invalid role specified.");
            }

            var requestingUserRole = User.FindFirstValue(ClaimTypes.Role);
            if (requestingUserRole == "HR" && normalizedRole != "workforce")
            {
                return Forbid("HR users can only register Workforce users.");
            }

            var userExists = await _context.Admins.AnyAsync(a => a.Email == request.Email) ||
                             await _context.HRUsers.AnyAsync(h => h.Email == request.Email) ||
                             await _context.WorkforceUsers.AnyAsync(w => w.Email == request.Email);

            if (userExists)
                return Conflict("User with this email already exists.");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            switch (normalizedRole)
            {
                case "admin":
                    _context.Admins.Add(new Admin { FirstName = request.FirstName, LastName = request.LastName, Email = request.Email, PasswordHash = hashedPassword, PhoneNumber = request.PhoneNumber });
                    break;
                case "hr":
                    _context.HRUsers.Add(new HRUser { FirstName = request.FirstName, LastName = request.LastName, Email = request.Email, PasswordHash = hashedPassword, PhoneNumber = request.PhoneNumber });
                    break;
                case "workforce":
                    _context.WorkforceUsers.Add(new WorkforceUser { FirstName = request.FirstName, LastName = request.LastName, Email = request.Email, PasswordHash = hashedPassword, PhoneNumber = request.PhoneNumber });
                    break;
            }

            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(request.Email, "Welcome to Workforce System", $"<p>Hello {request.FirstName},</p><p>Your account has been created as <b>{normalizedRole}</b>.<p>Your password is: {request.Password}</p>");
            return Ok("User registered successfully.");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var userExists = await _context.Admins.AnyAsync(u => u.Email == dto.Email) ||
                             await _context.HRUsers.AnyAsync(u => u.Email == dto.Email) ||
                             await _context.WorkforceUsers.AnyAsync(u => u.Email == dto.Email);
            if (!userExists) return NotFound("User not found.");

            string otp = GenerateOtp();
            _otpStore[dto.Email] = (otp, DateTime.UtcNow.AddMinutes(10));

            await _emailService.SendEmailAsync(dto.Email, "Password Reset OTP", $"<h3>Hello,</h3><p>Your OTP is: <b>{otp}</b></p><p>This code will expire in 10 minutes.</p>");
            return Ok("OTP has been sent to your email.");
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequestDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email)) return BadRequest("Email is required.");

            var userExists = await _context.Admins.AnyAsync(u => u.Email == dto.Email) ||
                             await _context.HRUsers.AnyAsync(u => u.Email == dto.Email) ||
                             await _context.WorkforceUsers.AnyAsync(u => u.Email == dto.Email);
            if (!userExists) return NotFound("User not found.");

            if (_otpStore.TryGetValue(dto.Email, out var otpInfo) && otpInfo.ExpiresAt > DateTime.UtcNow)
            {
                var remainingTime = otpInfo.ExpiresAt - DateTime.UtcNow;
                return BadRequest($"An OTP has already been sent. Please wait {remainingTime.Minutes} minute(s) and {remainingTime.Seconds} second(s) before requesting a new one.");
            }

            string otp = GenerateOtp();
            _otpStore[dto.Email] = (otp, DateTime.UtcNow.AddMinutes(10));
            await _emailService.SendEmailAsync(dto.Email, "Password Reset OTP", $"<h3>Hello,</h3><p>Your new OTP is: <b>{otp}</b></p><p>This code will expire in 10 minutes.</p>");
            return Ok("A new OTP has been sent to your email.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!_otpStore.TryGetValue(dto.Email, out var otpInfo) || otpInfo.Otp != dto.Otp || otpInfo.ExpiresAt < DateTime.UtcNow)
                return BadRequest("Invalid or expired OTP.");

            string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            bool userFoundAndUpdated = false;

            var admin = await _context.Admins.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (admin != null) { admin.PasswordHash = newPasswordHash; userFoundAndUpdated = true; }
            else
            {
                var hr = await _context.HRUsers.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (hr != null) { hr.PasswordHash = newPasswordHash; userFoundAndUpdated = true; }
                else
                {
                    var workforce = await _context.WorkforceUsers.FirstOrDefaultAsync(u => u.Email == dto.Email);
                    if (workforce != null) { workforce.PasswordHash = newPasswordHash; userFoundAndUpdated = true; }
                }
            }

            if (userFoundAndUpdated)
            {
                await _context.SaveChangesAsync();
                _otpStore.Remove(dto.Email);
                return Ok("Password has been reset successfully.");
            }

            return NotFound("User not found.");


        }


[Authorize]
[HttpPost("change-password")]
public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
{
    if (string.IsNullOrWhiteSpace(dto.OldPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
        return BadRequest("Old and new passwords are required.");

    var email = User.FindFirstValue(ClaimTypes.Name);
    if (email == null) return Unauthorized();

    object user = await _context.Admins.FirstOrDefaultAsync(u => u.Email == email);
    if (user == null)
        user = await _context.HRUsers.FirstOrDefaultAsync(u => u.Email == email);
    if (user == null)
        user = await _context.WorkforceUsers.FirstOrDefaultAsync(u => u.Email == email);
    if (user == null) return NotFound("User not found.");

    string passwordHash = user switch
    {
        Admin admin => admin.PasswordHash,
        HRUser hr => hr.PasswordHash,
        WorkforceUser wf => wf.PasswordHash,
        _ => null
    };

    if (passwordHash == null || !BCrypt.Net.BCrypt.Verify(dto.OldPassword, passwordHash))
        return BadRequest("Old password is incorrect.");

    // Update password
    string newHashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
    switch (user)
    {
        case Admin admin:
            admin.PasswordHash = newHashedPassword;
            break;
        case HRUser hr:
            hr.PasswordHash = newHashedPassword;
            break;
        case WorkforceUser wf:
            wf.PasswordHash = newHashedPassword;
            break;
    }

    await _context.SaveChangesAsync();
    return Ok("Password changed successfully.");
}



        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var email = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(email)) return Unauthorized("User email not found in token.");
            var role = User.FindFirstValue(ClaimTypes.Role);
            return Ok(new { email, role });
        }
    }
}