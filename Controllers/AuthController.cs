using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollManagementAPI.Data;
using PayrollManagementAPI.Models;
using PayrollManagementAPI.Services;

namespace PayrollManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public AuthController(
            ApplicationDbContext context,
            EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // 🔐 LOGIN (Admin + Employee)
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            if (model == null)
                return BadRequest("Invalid Data");

            if (string.IsNullOrWhiteSpace(model.Password))
                return BadRequest("Password required");

            string password = model.Password.Trim();

            var user = await _context.Users
                .Where(x =>
                    (x.UserId == model.LoginId || x.EPFNo == model.LoginId)
                    && x.Password == password)
                .Select(x => new
                {
                    x.Id,
                    x.Role,
                    x.EPFNo,
                    x.UserId
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return Unauthorized("Invalid User ID / EPF No or Password");

            return Ok(user);
        }

        // 🔐 CHANGE PASSWORD
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordModel model)
        {
            if (model == null)
                return BadRequest("Invalid Data");

            if (string.IsNullOrWhiteSpace(model.OldPassword) ||
                string.IsNullOrWhiteSpace(model.NewPassword))
            {
                return BadRequest("Password cannot be empty");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.EPFNo == model.EPFNo);

            if (user == null)
                return NotFound("User not found");

            if (user.Password != model.OldPassword.Trim())
                return Unauthorized("Old password is incorrect");

            user.Password = model.NewPassword.Trim();

            await _context.SaveChangesAsync();

            return Ok("Password Updated Successfully");
        }

        // 📩 SEND OTP
        [HttpPost("SendOtp")]
        public async Task<IActionResult> SendOtp(
            [FromBody] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email required");

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
                return NotFound("Email not found");

            var otp = new Random().Next(100000, 999999).ToString();

            user.ResetOtp = otp;
            user.OtpExpiry = DateTime.Now.AddMinutes(5);

            await _context.SaveChangesAsync();

            await _emailService.SendOtpEmail(email, otp);

            return Ok("OTP Sent Successfully");
        }

        // 🔎 VERIFY OTP
        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp(
            [FromBody] VerifyOtpModel model)
        {
            if (model == null)
                return BadRequest("Invalid Data");

            var user = await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Email == model.Email &&
                    x.ResetOtp == model.Otp &&
                    x.OtpExpiry > DateTime.Now);

            if (user == null)
                return BadRequest("Invalid or Expired OTP");

            return Ok("OTP Verified Successfully");
        }

        // 🔁 RESET PASSWORD
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(
            [FromBody] ResetPasswordModel model)
        {
            if (model == null)
                return BadRequest("Invalid Data");

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null)
                return NotFound("User not found");

            user.Password = model.NewPassword.Trim();
            user.ResetOtp = null;
            user.OtpExpiry = null;

            await _context.SaveChangesAsync();

            return Ok("Password Updated Successfully");
        }
    }
}