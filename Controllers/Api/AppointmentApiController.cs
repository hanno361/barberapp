using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using barberapp.Models;
using barberapp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppointmentApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Tüm randevuları getir
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {
            return await _context.Appointments
                .Include(a => a.User)
                .ToListAsync();
        }

        // Yeni randevu oluştur
        [HttpPost]
        public async Task<ActionResult<Appointment>> CreateAppointment(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return Ok(appointment);
        }

        // Randevuları tarihe göre filtrele
        [HttpGet("bydate")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsByDate([FromQuery] DateTime date)
        {
            return await _context.Appointments
                .Where(a => a.Date.Date == date.Date)
                .Include(a => a.User)
                .ToListAsync();
        }

        // Berbere göre randevuları getir
        [HttpGet("bybarber")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsByBarber([FromQuery] string barber)
        {
            return await _context.Appointments
                .Where(a => a.Barber == barber)
                .Include(a => a.User)
                .ToListAsync();
        }

        // PUT: api/AppointmentApi/updatestatus/5
        [HttpPut("updatestatus/{id}")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] string status)
        {
            try 
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    return NotFound($"ID: {id} olan randevu bulunamadı.");
                }

                appointment.Status = status;
                await _context.SaveChangesAsync();

                // Debug bilgisi
                Console.WriteLine($"Randevu güncellendi - ID: {id}, Yeni Durum: {status}");

                return Ok(new { message = $"Randevu durumu '{status}' olarak güncellendi." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata oluştu: {ex.Message}");
                return StatusCode(500, new { message = "Randevu güncellenirken bir hata oluştu.", error = ex.Message });
            }
        }

        // Berbere göre randevuları filtrele ve tarihe göre sırala
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Appointment>>> SearchAppointments(
            [FromQuery] string? barber = null,
            [FromQuery] string? status = null,
            [FromQuery] DateTime? startDate = null)
        {
            var query = _context.Appointments
                .Include(a => a.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(barber))
            {
                query = query.Where(a => a.Barber == barber);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(a => a.Status == status);
            }

            if (startDate.HasValue)
            {
                // Tarih karşılaştırması için başlangıç ve bitiş zamanları
                var startOfDay = startDate.Value.Date;
                var endOfDay = startDate.Value.Date.AddDays(1).AddTicks(-1);

                query = query.Where(a => a.Date >= startOfDay && a.Date <= endOfDay);
            }

            var appointments = await query
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            // Debug bilgisi
            Console.WriteLine($"Arama kriterleri - Berber: {barber}, Status: {status}, Tarih: {startDate}");
            Console.WriteLine($"Bulunan randevu sayısı: {appointments.Count}");

            return appointments;
        }

        // Kullanıcı arama
        [HttpGet("searchusers")]
        public async Task<ActionResult<IEnumerable<User>>> SearchUsers([FromQuery] string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return await _context.Users
                    .Where(u => u.Role != "Admin")
                    .Take(10)
                    .ToListAsync();
            }

            // LINQ ile kullanıcı arama
            return await _context.Users
                .Where(u => u.Role != "Admin" && 
                    (u.FullName.Contains(searchTerm) || 
                     u.Email.Contains(searchTerm)))
                .ToListAsync();
        }
    }
} 