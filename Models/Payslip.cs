using System.ComponentModel.DataAnnotations;

namespace PayrollManagementAPI.Models
{
    public class Payslip
    {
        [Key]
        public int Id { get; set; }

        public int EPFNo { get; set; }

        public string EmployeeName { get; set; }

        public string Designation { get; set; }

        public string PayPeriod { get; set; }

        public DateTime PayDate { get; set; }

        public double BasicSalary { get; set; }

        public double TravellingAllowance { get; set; }

        public double PF { get; set; }

        public double Tax { get; set; }

        public double NetSalary { get; set; }
    }
}