namespace barberapp.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Appointment
    {
        public int Id { get; set; }
        
        [Required]
        public required string Service { get; set; }
        
        public DateTime Date { get; set; }
        
        [Required]
        public required string Status { get; set; }
        
        [Required]
        public required string Barber { get; set; }
        
        public int UserId { get; set; }
        public User? User { get; set; }
    }
} 