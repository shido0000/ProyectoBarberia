using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class BarberProfile
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Display(Name = "Nombre de la Barbería")]
        [MaxLength(100)]
        public string? ShopName { get; set; }
        
        [Display(Name = "Descripción")]
        public string? Description { get; set; }
        
        [Display(Name = "Dirección")]
        [MaxLength(200)]
        public string? Address { get; set; }
        
        [Display(Name = "Teléfono")]
        [MaxLength(20)]
        public string? Phone { get; set; }
        
        [Display(Name = "Foto de Perfil")]
        [MaxLength(500)]
        public string? ProfilePhoto { get; set; }
        
        [Display(Name = "Horario de Inicio")]
        public TimeSpan StartTime { get; set; } = new TimeSpan(9, 0, 0); // 9:00 AM
        
        [Display(Name = "Horario de Fin")]
        public TimeSpan EndTime { get; set; } = new TimeSpan(18, 0, 0); // 6:00 PM
        
        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<BarberAvailability> Availability { get; set; } = new List<BarberAvailability>();
        public virtual ICollection<BarberShopImage> Images { get; set; } = new List<BarberShopImage>();
    }
}
