namespace PayrollManagementAPI.Models
{
    public class SalaryResult
    {
        public int EPFNo { get; set; }

        public string Name { get; set; }

        public int BasicSalary { get; set; }

        public int TravellingAllowance { get; set; }

        public int SalaryAdvance { get; set; }

        public double PF { get; set; }

        public double Tax { get; set; }

        public double GrossSalary { get; set; }

        public double NetSalary { get; set; }
    }
}