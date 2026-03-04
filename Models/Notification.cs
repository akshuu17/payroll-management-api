namespace PayrollManagementAPI.Models
{
    public class Notification
    {
        public int Id { get; set; }

        public int EPFNo { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}