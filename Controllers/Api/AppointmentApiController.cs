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
    }
} 