using Microsoft.AspNetCore.Mvc;
using PayrollManagementAPI.Data;
using PayrollManagementAPI.Models;
using System.Linq;

namespace PayrollManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayslipController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PayslipController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Generate Payslip
        [HttpPost("Generate/{epfNo}")]
        public IActionResult GeneratePayslip(int epfNo)
        {
            var emp = _context.Employees.FirstOrDefault(e => e.EPFNo == epfNo);

            if (emp == null)
                return NotFound("Employee not found");

            double gross = emp.BasicSalary + emp.TravellingAllowance;
            double pf = emp.BasicSalary * 0.12;
            double tax = gross * 0.05;
            double net = gross - (pf + tax + emp.SalaryAdvance);

            var payslip = new Payslip
            {
                EPFNo = emp.EPFNo,
                EmployeeName = emp.Name,
                Designation = emp.Position,
                PayPeriod = "March 2026",
                PayDate = DateTime.Now,
                BasicSalary = emp.BasicSalary,
                TravellingAllowance = emp.TravellingAllowance,
                PF = pf,
                Tax = tax,
                NetSalary = net
            };

            _context.Payslips.Add(payslip);
            _context.SaveChanges();

            return Ok(payslip);
        }

        // Get Payslip by Employee
        [HttpGet("Employee/{epfNo}")]
        public IActionResult GetPayslip(int epfNo)
        {
            var payslip = _context.Payslips
                .Where(p => p.EPFNo == epfNo)
                .OrderByDescending(p => p.PayDate)
                .FirstOrDefault();

            if (payslip == null)
                return NotFound("Payslip not found");

            return Ok(payslip);
        }
    }
}