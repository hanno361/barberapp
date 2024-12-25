using Microsoft.AspNetCore.Identity;

namespace barberapp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
} 