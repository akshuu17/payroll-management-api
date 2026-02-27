using Microsoft.AspNetCore.Mvc;
using PayrollManagementAPI.Data;
using PayrollManagementAPI.Models;

namespace PayrollManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalaryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SalaryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Salary/Calculate/101
        [HttpGet("Calculate/{epfNo}")]
        public IActionResult CalculateSalary(int epfNo)
        {
            var emp = _context.Employees.FirstOrDefault(e => e.EPFNo == epfNo);

            if (emp == null)
            {
                return NotFound("Employee not found");
            }

            double gross = emp.BasicSalary + emp.TravellingAllowance;

            double pf = emp.BasicSalary * 0.12;

            double tax = gross * 0.05;

            double net = gross - (pf + tax + emp.SalaryAdvance);

            var result = new SalaryResult
            {
                EPFNo = emp.EPFNo,
                Name = emp.Name,
                BasicSalary = emp.BasicSalary,
                TravellingAllowance = emp.TravellingAllowance,
                SalaryAdvance = emp.SalaryAdvance,

                GrossSalary = gross,
                PF = pf,
                Tax = tax,
                NetSalary = net
            };

            return Ok(result);
        }
    }
}