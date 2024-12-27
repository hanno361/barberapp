using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using barberapp.Models;
using barberapp.Data;

namespace barberapp.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Randevu listesi
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var appointments = await _context.Appointments
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View(appointments);
        }

        // Randevu oluşturma sayfası
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            // O günkü randevuları al
            var selectedDate = DateTime.Today;
            var bookedAppointments = _context.Appointments
                .Where(a => a.Date.Date == selectedDate.Date && a.Status == "Aktif")
                .Select(a => a.Date.ToString("HH:mm"))
                .ToList();

            ViewBag.BookedTimes = bookedAppointments;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetBookedTimes(string date, string barber)
        {
            var selectedDate = DateTime.Parse(date);
            var bookedTimes = await _context.Appointments
                .Where(a => a.Date.Date == selectedDate.Date && 
                            a.Status == "Aktif" && 
                            a.Barber == barber)
                .Select(a => a.Date.ToString("HH:mm"))
                .ToListAsync();

            return Json(bookedTimes);
        }

        // Randevu oluşturma işlemi
        [HttpPost]
        public async Task<IActionResult> Create(string service, string date, string time, string barber)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var appointmentDate = DateTime.Parse($"{date} {time}");
            
            var appointment = new Appointment
            {
                Service = service,
                Date = appointmentDate,
                Status = "Aktif",
                Barber = barber,
                UserId = userId.Value
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }

        // Randevu iptal etme
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null && appointment.UserId == userId)
            {
                appointment.Status = "İptal edildi";
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
} 