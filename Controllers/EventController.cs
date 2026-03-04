using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollManagementAPI.Data;
using PayrollManagementAPI.Models;
using PayrollManagementAPI.Services;

namespace PayrollManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        // ✅ KEEP ONLY THIS CONSTRUCTOR
        public EventController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // ✅ 1️⃣ CREATE EVENT
        [HttpPost("Create")]
        public async Task<IActionResult> CreateEvent([FromBody] Event model)
        {
            if (model == null)
                return BadRequest("Invalid Data");

            model.CreatedDate = DateTime.Now;

            _context.Events.Add(model);
            await _context.SaveChangesAsync();

            // 🔥 Get All Employees
            var employees = await _context.Employees.ToListAsync();

            foreach (var emp in employees)
            {
                // 🔔 Add In-App Notification
                _context.Notifications.Add(new Notification
                {
                    EPFNo = emp.EPFNo,
                    Title = "New Event",
                    Message = $"New event '{model.Title}' on {model.EventDate:dd-MM-yyyy}"
                });

                // 📧 Send Email
                if (!string.IsNullOrEmpty(emp.Email))
                {
                    await _emailService.SendEventEmail(
                        emp.Email,
                        model.Title,
                        model.EventDate
                    );
                }
            }

            await _context.SaveChangesAsync();

            return Ok("Event Created, Notification & Email Sent");
        }



        // ✅ 2️⃣ GET ALL EVENTS
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _context.Events
                .OrderByDescending(e => e.EventDate)
                .ToListAsync();

            return Ok(events);
        }

        // ✅ 3️⃣ GET EVENTS WITH REGISTRATION COUNT
        [HttpGet("GetWithCount")]
        public async Task<IActionResult> GetEventsWithCount()
        {
            var data = await _context.Events
                .Select(e => new
                {
                    e.Id,
                    e.Title,
                    e.Description,
                    e.EventDate,
                    e.ImagePath,
                    RegistrationCount = _context.EventRegistrations
                        .Count(r => r.EventId == e.Id)
                })
                .OrderByDescending(e => e.EventDate)
                .ToListAsync();

            return Ok(data);
        }

        // ✅ 4️⃣ DELETE EVENT
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null)
                return NotFound("Event Not Found");

            if (ev.Registrations != null)
                _context.EventRegistrations.RemoveRange(ev.Registrations);

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();

            return Ok("Event Deleted Successfully");
        }

        // ✅ 5️⃣ UPCOMING EVENTS
        [HttpGet("Upcoming")]
        public async Task<IActionResult> GetUpcomingEvents()
        {
            var today = DateTime.Today;

            var events = await _context.Events
                .Where(e => e.EventDate >= today)
                .OrderBy(e => e.EventDate)
                .ToListAsync();

            return Ok(events);
        }

        // ✅ 6️⃣ REGISTER
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterEvent(int eventId, int epfNo)
        {
            var exists = await _context.EventRegistrations
                .AnyAsync(r => r.EventId == eventId && r.EPFNo == epfNo);

            if (exists)
                return BadRequest("Already Registered");

            var registration = new EventRegistration
            {
                EventId = eventId,
                EPFNo = epfNo,
                RegisteredDate = DateTime.Now
            };

            _context.EventRegistrations.Add(registration);
            await _context.SaveChangesAsync();

            return Ok("Registered Successfully");
        }

        // ✅ 7️⃣ MY EVENTS
        [HttpGet("MyEvents/{epfNo}")]
        public async Task<IActionResult> GetMyEvents(int epfNo)
        {
            var events = await _context.EventRegistrations
                .Where(r => r.EPFNo == epfNo)
                .Include(r => r.Event)
                .Select(r => new
                {
                    r.Event.Id,
                    r.Event.Title,
                    r.Event.Description,
                    r.Event.EventDate
                })
                .ToListAsync();

            return Ok(events);
        }

        // ✅ 8️⃣ REGISTERED EMPLOYEES
        [HttpGet("Registered/{eventId}")]
        public async Task<IActionResult> GetRegisteredEmployees(int eventId)
        {
            var data = await _context.EventRegistrations
                .Where(r => r.EventId == eventId)
                .Join(_context.Employees,
                    r => r.EPFNo,
                    e => e.EPFNo,
                    (r, e) => new
                    {
                        e.EPFNo,
                        e.Name,
                        e.Position,
                        r.RegisteredDate
                    })
                .ToListAsync();

            return Ok(new
            {
                TotalCount = data.Count,
                Employees = data
            });
        }

        // ✅ 9️⃣ DOWNLOAD PDF
        [HttpGet("DownloadPdf/{eventId}")]
        public async Task<IActionResult> DownloadPdf(int eventId)
        {
            var eventData = await _context.Events.FindAsync(eventId);

            if (eventData == null)
                return NotFound("Event Not Found");

            var employees = await _context.EventRegistrations
                .Where(r => r.EventId == eventId)
                .Join(_context.Employees,
                    r => r.EPFNo,
                    e => e.EPFNo,
                    (r, e) => new
                    {
                        e.EPFNo,
                        e.Name,
                        e.Position
                    })
                .ToListAsync();

            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            var boldFont = iText.Kernel.Font.PdfFontFactory
                .CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);

            document.Add(new Paragraph($"Event: {eventData.Title}")
                .SetFontSize(18)
                .SetFont(boldFont));

            document.Add(new Paragraph($"Date: {eventData.EventDate:dd-MM-yyyy}")
                .SetMarginBottom(10));

            document.Add(new Paragraph($"Total Registered: {employees.Count}")
                .SetMarginBottom(10));

            var table = new Table(UnitValue.CreatePercentArray(3))
                .UseAllAvailableWidth();

            table.AddHeaderCell("EPF No");
            table.AddHeaderCell("Name");
            table.AddHeaderCell("Position");

            foreach (var emp in employees)
            {
                table.AddCell(emp.EPFNo.ToString());
                table.AddCell(emp.Name);
                table.AddCell(emp.Position);
            }

            document.Add(table);
            document.Close();

            return File(
                ms.ToArray(),
                "application/pdf",
                $"{eventData.Title}_RegisteredList.pdf"
            );
        }
        // ✅ 🔄 UPDATE EVENT
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] Event model)
        {
            var ev = await _context.Events.FindAsync(id);

            if (ev == null)
                return NotFound("Event Not Found");

            ev.Title = model.Title;
            ev.Description = model.Description;
            ev.EventDate = model.EventDate;

            await _context.SaveChangesAsync();

            return Ok("Event Updated Successfully");
        }
    }
}