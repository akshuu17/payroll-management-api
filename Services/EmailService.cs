using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace PayrollManagementAPI.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;


        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendOtpEmail(string toEmail, string otp)
        {
            try
            {
                string host = _config["EmailSettings:Host"];
                int port = int.Parse(_config["EmailSettings:Port"]);
                string fromEmail = _config["EmailSettings:Email"];
                string password = _config["EmailSettings:Password"];

                var smtp = new SmtpClient(host)
                {
                    Port = port,
                    Credentials = new NetworkCredential(fromEmail, password),
                    EnableSsl = true
                };

                var message = new MailMessage
                {
                    From = new MailAddress(fromEmail, "Payroll System"),
                    Subject = "Password Reset OTP",
                    Body = $@"
Hello,

Your OTP for password reset is: {otp}

This OTP will expire in 5 minutes.

If you did not request this, please ignore this email.

Regards,
Payroll Management Team",
                    IsBodyHtml = false
                };

                message.To.Add(toEmail);

                await smtp.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                throw new Exception("Email sending failed: " + ex.Message);
            }
        }

        public async Task SendEventEmail(string toEmail, string title, DateTime date)
        {
            var smtp = new SmtpClient(_config["EmailSettings:Host"])
            {
                Port = int.Parse(_config["EmailSettings:Port"]),
                Credentials = new NetworkCredential(
                    _config["EmailSettings:Email"],
                    _config["EmailSettings:Password"]
                ),
                EnableSsl = true
            };

            var message = new MailMessage(
                _config["EmailSettings:Email"],
                toEmail,
                "New Event Notification",
                $"New Event: {title}\nDate: {date:dd-MM-yyyy}"
            );

            await smtp.SendMailAsync(message);
        }
    }
}