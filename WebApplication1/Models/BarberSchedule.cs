using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    /// <summary>
    /// Representa un horario de atención para un barbero. Permite múltiples agendas (Premium).
    /// </summary>
    public class BarberSchedule
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int BarberProfileId { get; set; }
        
        [Required]
        [Display(Name = "Día de la Semana")]
        public DayOfWeek DayOfWeek { get; set; }
        
        [Required]
        [Display(Name = "Hora de Inicio")]
        public TimeSpan StartTime { get; set; }
        
        [Required]
        [Display(Name = "Hora de Fin")]
        public TimeSpan EndTime { get; set; }
        
        [Display(Name = "Ubicación/Local")]
        [MaxLength(200)]
        public string? Location { get; set; }
        
        [Display(Name = "Nombre del Horario")]
        [MaxLength(100)]
        public string? ScheduleName { get; set; } // Ej: "Mañana", "Tarde", "Local A"
        
        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation property
        public virtual BarberProfile BarberProfile { get; set; } = null!;
    }
}
