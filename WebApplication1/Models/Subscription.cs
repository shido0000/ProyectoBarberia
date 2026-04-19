using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Subscription
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Display(Name = "Plan de Suscripción")]
        public int? SubscriptionPlanId { get; set; }
        
        [Display(Name = "Fecha de Inicio")]
        public DateTime StartDate { get; set; } = DateTime.Now;
        
        [Display(Name = "Fecha de Fin")]
        public DateTime? EndDate { get; set; }
        
        [Display(Name = "Activa")]
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual SubscriptionPlan? SubscriptionPlan { get; set; }
    }
}
