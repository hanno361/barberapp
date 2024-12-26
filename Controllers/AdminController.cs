using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using barberapp.Models;
using barberapp.Data;

namespace barberapp.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var appointments = await _context.Appointments
                .Include(a => a.User)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = status;
                await _context.SaveChangesAsync();
                TempData["Message"] = $"Randevu durumu '{status}' olarak güncellendi.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Users()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var users = await _context.Users
                .Where(u => u.Role != "Admin")
                .ToListAsync();

            return View(users);
        }
    }
} 