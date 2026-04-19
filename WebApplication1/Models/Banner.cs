using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    /// <summary>
    /// Banner promocional creado por un barbero Premium para mostrarse en la web.
    /// </summary>
    public class Banner
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Barbero")]
        public int BarberId { get; set; }
        
        [Display(Name = "Barbero")]
        public virtual BarberProfile Barber { get; set; } = null!;
        
        [Required]
        [MaxLength(200)]
        [Display(Name = "Título")]
        public string Title { get; set; } = string.Empty;
        
        [MaxLength(500)]
        [Display(Name = "Descripción")]
        public string? Description { get; set; }
        
        [Required]
        [MaxLength(500)]
        [Display(Name = "Imagen")]
        public string ImageUrl { get; set; } = string.Empty;
        
        [MaxLength(500)]
        [Display(Name = "Enlace de Destino")]
        public string? LinkUrl { get; set; }
        
        [Required]
        [Display(Name = "Fecha de Inicio")]
        public DateTime StartDate { get; set; }
        
        [Required]
        [Display(Name = "Fecha de Fin")]
        public DateTime EndDate { get; set; }
        
        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Indica si el banner ha sido aprobado por el administrador.
        /// Para barberos Premium, se aprueba automáticamente (true por defecto).
        /// </summary>
        [Display(Name = "Aprobado")]
        public bool IsApproved { get; set; } = true;
        
        [Display(Name = "Prioridad")]
        public int Priority { get; set; } = 0; // Mayor número = mayor prioridad
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        /// <summary>
        /// Verifica si el banner está dentro del rango de fechas activo.
        /// </summary>
        [NotMapped]
        public bool IsWithinDateRange => DateTime.Now >= StartDate && DateTime.Now <= EndDate;
    }
}
