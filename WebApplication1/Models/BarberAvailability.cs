using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class BarberAvailability
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
        
        [Display(Name = "Está Disponible")]
        public bool IsAvailable { get; set; } = true;
        
        // Navigation property
        public virtual BarberProfile BarberProfile { get; set; } = null!;
    }
}
