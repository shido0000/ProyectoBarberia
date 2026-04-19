using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    /// <summary>
    /// Registro de transacción de caja para estadísticas (Media/Premium).
    /// </summary>
    public class CashTransaction
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int BarberProfileId { get; set; }
        
        [Required]
        [Display(Name = "Tipo de Transacción")]
        public TransactionType Type { get; set; }
        
        [Required]
        [Display(Name = "Monto")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        [Display(Name = "Método de Pago")]
        public PaymentMethod Method { get; set; } = PaymentMethod.Cash;
        
        [MaxLength(500)]
        [Display(Name = "Descripción")]
        public string? Description { get; set; }
        
        [Display(Name = "Fecha de la Transacción")]
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        
        // Relación opcional con cita
        public int? AppointmentId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual BarberProfile BarberProfile { get; set; } = null!;
        public virtual Appointment? Appointment { get; set; }
    }
    
    public enum TransactionType
    {
        Income,     // Ingreso por servicio
        Expense,    // Gasto (insumos, servicios, etc.)
        Refund      // Reembolso
    }
}
