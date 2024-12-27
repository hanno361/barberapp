using System.ComponentModel.DataAnnotations;

namespace barberapp.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        public required string Email { get; set; }
        
        [Required]
        public required string Password { get; set; }
        
        [Required]
        public required string FullName { get; set; }
        
        public string Role { get; set; } = "User";
    }
} 