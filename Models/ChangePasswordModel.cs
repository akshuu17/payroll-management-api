namespace PayrollManagementAPI.Models
{
    public class ChangePasswordModel
    {
        public int EPFNo { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
