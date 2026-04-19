using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class BarberProfile
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Display(Name = "Nombre de la Barbería")]
        [MaxLength(100)]
        public string? ShopName { get; set; }
        
        [Display(Name = "Descripción")]
        public string? Description { get; set; }
        
        [Display(Name = "Dirección")]
        [MaxLength(200)]
        public string? Address { get; set; }
        
        [Display(Name = "Teléfono")]
        [MaxLength(20)]
        public string? Phone { get; set; }
        
        [Display(Name = "Foto de Perfil")]
        [MaxLength(500)]
        public string? ProfilePhoto { get; set; }
        
        [Display(Name = "Horario de Inicio")]
        public TimeSpan StartTime { get; set; } = new TimeSpan(9, 0, 0); // 9:00 AM
        
        [Display(Name = "Horario de Fin")]
        public TimeSpan EndTime { get; set; } = new TimeSpan(18, 0, 0); // 6:00 PM
        
        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Plan de suscripción actual del barbero. Determina las funcionalidades disponibles.
        /// </summary>
        [Display(Name = "Plan de Suscripción")]
        public SubscriptionTier SubscriptionPlan { get; set; } = SubscriptionTier.Free;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<BarberAvailability> Availability { get; set; } = new List<BarberAvailability>();
        public virtual ICollection<BarberShopImage> Images { get; set; } = new List<BarberShopImage>();
        public virtual ICollection<BarberSchedule> Schedules { get; set; } = new List<BarberSchedule>();
        public virtual ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
        public virtual ICollection<ClientNote> ClientNotes { get; set; } = new List<ClientNote>();
    }
    
    /// <summary>
    /// Niveles de suscripción que determinan las funcionalidades disponibles para cada barbero.
    /// </summary>
    public enum SubscriptionTier
    {
        /// <summary>
        /// Plan gratuito: Solo perfil y servicios visibles. Sin agenda ni citas.
        /// </summary>
        Free = 1,
        
        /// <summary>
        /// Plan intermedio: Agenda completa, citas y estadísticas básicas de caja.
        /// </summary>
        Media = 2,
        
        /// <summary>
        /// Plan premium: Todas las funcionalidades incluyendo CRM, inventario, señas y múltiples agendas.
        /// </summary>
        Premium = 3
    }
}
