using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Role { get; set; } // Admin, Barber, Client
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual BarberProfile? BarberProfile { get; set; }
        public virtual ICollection<Appointment> ClientAppointments { get; set; } = new List<Appointment>();
    }
}
