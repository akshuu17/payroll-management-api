using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollManagementAPI.Data;
using PayrollManagementAPI.Models;

namespace PayrollManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LeaveController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Employee Apply Leave
        [HttpPost("Apply")]
        public async Task<IActionResult> ApplyLeave(LeaveRequest model)
        {
            model.Status = "Pending";

            _context.LeaveRequests.Add(model);
            await _context.SaveChangesAsync();

            return Ok("Leave Applied Successfully");
        }

        // Get Leaves by Employee
        [HttpGet("ByEmployee/{epfNo}")]
        public async Task<IActionResult> GetByEmployee(int epfNo)
        {
            var leaves = await _context.LeaveRequests
                .Where(x => x.EPFNo == epfNo)
                .ToListAsync();

            return Ok(leaves);
        }

        // Admin Get All Leaves
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var leaves = await _context.LeaveRequests.ToListAsync();
            return Ok(leaves);
        }

        // Admin Approve / Reject
        [HttpPut("UpdateStatus/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var leave = await _context.LeaveRequests.FindAsync(id);

            if (leave == null)
                return NotFound();

            leave.Status = status;

            await _context.SaveChangesAsync();

            return Ok("Status Updated");
        }
    }
}