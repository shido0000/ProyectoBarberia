using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        [Display(Name = "Nombre del Servicio")]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        [Display(Name = "Descripción")]
        public string? Description { get; set; }
        
        [Required]
        [Display(Name = "Precio")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        [Required]
        [Display(Name = "Duración (minutos)")]
        public int DurationMinutes { get; set; }
        
        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;
        
        [Required]
        public int BarberProfileId { get; set; }
        
        // Navigation properties
        public virtual BarberProfile BarberProfile { get; set; } = null!;
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<ServiceImage> Images { get; set; } = new List<ServiceImage>();
    }
}
