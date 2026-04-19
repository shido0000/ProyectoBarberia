using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string ClientId { get; set; } = string.Empty;
        
        [Required]
        public int BarberProfileId { get; set; }
        
        [Required]
        public int ServiceId { get; set; }
        
        [Required]
        [Display(Name = "Fecha")]
        public DateTime Date { get; set; }
        
        [Required]
        [Display(Name = "Hora")]
        public TimeSpan Time { get; set; }
        
        [Display(Name = "Estado")]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        
        /// <summary>
        /// ID de la barbería donde se realiza la cita (si el barbero pertenece a una).
        /// </summary>
        [Display(Name = "Barbería")]
        public int? BarbershopId { get; set; }
        
        [MaxLength(500)]
        [Display(Name = "Notas")]
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual ApplicationUser Client { get; set; } = null!;
        public virtual BarberProfile BarberProfile { get; set; } = null!;
        public virtual Service Service { get; set; } = null!;
        public virtual Barbershop? Barbershop { get; set; }
    }
    
    public enum AppointmentStatus
    {
        Pending,    // Pendiente
        Confirmed,  // Confirmada
        InProgress, // En progreso
        Completed,  // Completada
        Cancelled   // Cancelada
    }
}
