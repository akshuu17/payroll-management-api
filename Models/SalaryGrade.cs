using System.ComponentModel.DataAnnotations;

namespace PayrollManagementAPI.Models
{
    public class SalaryGrade
    {
        [Key]
        public int Id { get; set; }

        public int SeniorManager { get; set; }
        public int Manager { get; set; }
        public int AssistantManager { get; set; }
        public int TeamLeader { get; set; }
        public int SeniorSpecialist { get; set; }
        public int Specialist { get; set; }
        public int Analyst { get; set; }
        public int Technician { get; set; }
    }
}