namespace PayrollManagementAPI.Models
{
    public class Event
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime EventDate { get; set; }

        public string? ImagePath { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ICollection<EventRegistration>? Registrations { get; set; }
    }
}