namespace barberapp.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Service { get; set; }  // Saç kesimi, sakal tıraşı vb.
        public DateTime Date { get; set; }
        public string Status { get; set; }   // Aktif, İptal edildi
        public string Barber { get; set; }  // Berber adı eklendi
        public User User { get; set; }       // Navigation property
    }
} 