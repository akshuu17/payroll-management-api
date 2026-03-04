using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollManagementAPI.Data;
using PayrollManagementAPI.Models;

namespace PayrollManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Employee
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        // GET: api/Employee/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // POST: api/Employee (Add Employee + Create Login)
        [HttpPost]
        public async Task<ActionResult<Employee>> AddEmployee(Employee employee)
        {
            // Get Salary Grade
            var grade = await _context.SalaryGrades.FirstOrDefaultAsync();

            if (grade != null)
            {
                switch (employee.Position.ToLower())
                {
                    case "manager":
                        employee.BasicSalary = grade.Manager;
                        break;

                    case "analyst":
                        employee.BasicSalary = grade.Analyst;
                        break;

                    case "technician":
                        employee.BasicSalary = grade.Technician;
                        break;

                    default:
                        employee.BasicSalary = 20000;
                        break;
                }
            }
            else
            {
                employee.BasicSalary = 20000;
            }

            // Save Employee
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Create Login for Employee
            var user = new User
            {
                UserId = null,
                EPFNo = employee.EPFNo,
                Password = employee.Password,
                Role = "Employee"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(employee);
        }

        [HttpGet("Profile/{epfNo}")]
        public async Task<IActionResult> GetProfile(int epfNo)
        {
            var emp = await _context.Employees
                .Where(e => e.EPFNo == epfNo)
                .Select(e => new
                {
                    e.EPFNo,
                    e.Name,
                    e.Position,
                    e.Address,
                    e.BasicSalary,
                    e.WorkingDays,
                    e.TravellingAllowance,
                    e.ProfileImage
                })
                .FirstOrDefaultAsync();

            if (emp == null)
                return NotFound("Employee not found");

            return Ok(emp);
        }

        [HttpPost("UploadImage/{epfNo}")]
        public async Task<IActionResult> UploadImage(int epfNo, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file");

            var emp = await _context.Employees.FindAsync(epfNo);

            if (emp == null)
                return NotFound();

            var folder = Path.Combine("wwwroot", "profiles");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = epfNo + "_" + file.FileName;

            var path = Path.Combine(folder, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            emp.ProfileImage = "/profiles/" + fileName;

            await _context.SaveChangesAsync();

            return Ok(emp.ProfileImage);
        }

        // PUT: api/Employee/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
        {
            if (id != employee.EPFNo)
            {
                return BadRequest();
            }

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Employee/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EPFNo == id);
        }
    }
}