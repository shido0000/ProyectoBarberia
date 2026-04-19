using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        //public int UserId { get; set; }

        [Required]
        [Display(Name = "Monto")]
        public decimal Amount { get; set; }
        
        [Display(Name = "Fecha de Pago")]
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        
        [Display(Name = "Método de Pago")]
        public PaymentMethod Method { get; set; }
        
        [Display(Name = "Descripción")]
        public string? Description { get; set; }
        
        // Navigation property
        public virtual ApplicationUser User { get; set; } = null!;
    }
    
    public enum PaymentMethod
    {
        Cash,       // Efectivo
        Card,       // Tarjeta
        Transfer,   // Transferencia
        Other       // Otro
    }
}
