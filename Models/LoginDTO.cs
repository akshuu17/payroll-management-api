namespace PayrollManagementAPI.Models
{
    public class LoginDTO
    {
        public int LoginId { get; set; } // UserId OR EPFNo
        public string Password { get; set; }
    }
}