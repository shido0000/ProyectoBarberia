using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public enum SubscriptionTargetType
    {
        [Display(Name = "Barbero")]
        Barber = 1,
        [Display(Name = "Barbería")]
        Barbershop = 2
    }

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

        [Required]
        [Display(Name = "Tipo de Suscriptor")]
        public SubscriptionTargetType TargetType { get; set; } = SubscriptionTargetType.Barber;

        [Display(Name = "Permite Postear Productos")]
        public bool CanPostProducts { get; set; }

        [Display(Name = "Permite Recibir Reservas")]
        public bool CanReceiveBookings { get; set; }

        [Display(Name = "Acceso a Analytics")]
        public bool CanAccessAnalytics { get; set; }

        [Display(Name = "Acceso a Contabilidad")]
        public bool CanAccessAccounting { get; set; }

        [Display(Name = "Acceso a Inventario")]
        public bool CanAccessInventory { get; set; }

        [Display(Name = "Permite Banners")]
        public bool CanUseBanners { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation property
        public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
