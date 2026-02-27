using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollManagementAPI.Models
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EPFNo { get; set; }

        public string Name { get; set; }

        public string Position { get; set; }

        public string Address { get; set; }

        public int BasicSalary { get; set; }

        public int TravellingAllowance { get; set; }

        public int SalaryAdvance { get; set; }

        public int WorkingDays { get; set; }

        // 🔥 This must be here
        public string Password { get; set; }

        public string? Email { get; set; }

        public string? ProfileImage { get; set; }
    }
}