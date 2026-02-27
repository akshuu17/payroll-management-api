using System.ComponentModel.DataAnnotations;

namespace PayrollManagementAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        // Admin Login
        public int? UserId { get; set; }

        // Employee Login
        public int? EPFNo { get; set; }

        public string Password { get; set; }

        public string Role { get; set; } // Admin / Employee

        public string? Email { get; set; }

        public string? ResetOtp { get; set; }

        public DateTime? OtpExpiry { get; set; }
    }
}