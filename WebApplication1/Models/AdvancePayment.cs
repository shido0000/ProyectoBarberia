using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    /// <summary>
    /// Registro de seña o anticipo pagado por un cliente al reservar una cita (Premium).
    /// </summary>
    public class AdvancePayment
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int AppointmentId { get; set; }
        
        [Required]
        [Display(Name = "Monto de la Seña")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        [Display(Name = "Porcentaje del Total")]
        public int? PercentageOfTotal { get; set; }
        
        [Display(Name = "Método de Pago")]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        
        [Display(Name = "Fecha de Pago")]
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        
        [MaxLength(500)]
        [Display(Name = "Notas")]
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation property
        public virtual Appointment Appointment { get; set; } = null!;
    }
}
