using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class SubscriptionPlan
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        [Display(Name = "Nombre del Plan")]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        [Display(Name = "Descripción")]
        public string? Description { get; set; }
        
        [Required]
        [Display(Name = "Precio Mensual")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyPrice { get; set; }
        
        [Display(Name = "Precio Anual")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? AnnualPrice { get; set; }
        
        [Required]
        [Display(Name = "Duración (días)")]
        public int DurationDays { get; set; } = 30;
        
        [Display(Name = "Máximo de Barberos")]
        public int? MaxBarbers { get; set; }
        
        [Display(Name = "Máximo de Servicios por Barbero")]
        public int? MaxServicesPerBarber { get; set; }
        
        [Display(Name = "Incluir Analytics")]
        public bool IncludeAnalytics { get; set; }
        
        [Display(Name = "Soporte Prioritario")]
        public bool PrioritySupport { get; set; }
        
        [Display(Name = "Branding Personalizado")]
        public bool CustomBranding { get; set; }
        
        [Display(Name = "Recordatorios Automáticos")]
        public bool AutoReminders { get; set; }
        
        [Display(Name = "Exportar Reportes")]
        public bool ExportReports { get; set; }
        
        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;
        
        [Display(Name = "Es Plan por Defecto")]
        public bool IsDefault { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation property
        public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
