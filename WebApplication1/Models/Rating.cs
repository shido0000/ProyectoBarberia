using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    /// <summary>
    /// Representa una calificación y testimonio de un cliente hacia un barbero o barbería.
    /// </summary>
    public class Rating
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ClientId { get; set; } = string.Empty;
        
        [Display(Name = "Cliente")]
        public virtual ApplicationUser Client { get; set; } = null!;
        
        /// <summary>
        /// ID del barbero calificado (null si es a barbería).
        /// </summary>
        [Display(Name = "Barbero")]
        public int? BarberId { get; set; }
        
        [Display(Name = "Barbero")]
        public virtual BarberProfile? Barber { get; set; }
        
        /// <summary>
        /// ID de la barbería calificada (null si es a barbero).
        /// </summary>
        [Display(Name = "Barbería")]
        public int? BarbershopId { get; set; }
        
        [Display(Name = "Barbería")]
        public virtual Barbershop? Barbershop { get; set; }
        
        /// <summary>
        /// Calificación numérica de 1 a 5 estrellas.
        /// </summary>
        [Required]
        [Range(1, 5)]
        [Display(Name = "Calificación")]
        public int Score { get; set; }
        
        /// <summary>
        /// Comentario opcional del cliente.
        /// </summary>
        [MaxLength(1000)]
        [Display(Name = "Comentario")]
        public string? Comment { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime? UpdatedAt { get; set; }
        
        /// <summary>
        /// Indica si el testimonio ha sido reportado por otros usuarios.
        /// </summary>
        [Display(Name = "Reportado")]
        public bool IsReported { get; set; } = false;
        
        /// <summary>
        /// Número de veces que ha sido reportado.
        /// </summary>
        [Display(Name = "Reportes")]
        public int ReportCount { get; set; } = 0;
    }
}
