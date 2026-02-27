namespace PayrollManagementAPI.Models
{
    public class EventRegistration
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public int EPFNo { get; set; }

        public DateTime RegisteredDate { get; set; } = DateTime.Now;

        public Event Event { get; set; }
    }
}