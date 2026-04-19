using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    /// <summary>
    /// Representa una barbería (salón) que puede tener múltiples barberos asociados.
    /// </summary>
    public class Barbershop
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        [Display(Name = "Nombre de la Barbería")]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        [Display(Name = "Descripción")]
        public string? Description { get; set; }
        
        [MaxLength(200)]
        [Display(Name = "Dirección")]
        public string? Address { get; set; }
        
        [MaxLength(20)]
        [Display(Name = "Teléfono")]
        public string? Phone { get; set; }
        
        [MaxLength(500)]
        [Display(Name = "Logo")]
        public string? LogoUrl { get; set; }
        
        [MaxLength(500)]
        [Display(Name = "URL de Imagen de Portada")]
        public string? CoverImageUrl { get; set; }
        
        [Required]
        [Display(Name = "Plan de Suscripción")]
        public int BarbershopSubscriptionPlanId { get; set; }
        
        [Required]
        [Display(Name = "Dueño/Propietario")]
        public int OwnerBarberId { get; set; }
        
        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        public virtual BarbershopSubscriptionPlan Plan { get; set; } = null!;
        public virtual BarberProfile Owner { get; set; } = null!;
        public virtual ICollection<BarberProfile> Members { get; set; } = new List<BarberProfile>();
        public virtual ICollection<BarbershopMembershipRequest> MembershipRequests { get; set; } = new List<BarbershopMembershipRequest>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
    
    /// <summary>
    /// Planes de suscripción para barberías (todos de pago).
    /// </summary>
    public class BarbershopSubscriptionPlan
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
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Precio Mensual")]
        public decimal MonthlyPrice { get; set; }

        [Display(Name = "Máximo de Barberos", Description = "Dejar en null para ilimitado")]
        public int? MaxBarbers { get; set; }
        
        [MaxLength(1000)]
        [Display(Name = "Características (JSON)")]
        public string? FeaturesJson { get; set; }
        
        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation property
        public virtual ICollection<Barbershop> Barbershops { get; set; } = new List<Barbershop>();
    }
    
    /// <summary>
    /// Solicitud de un barbero para unirse a una barbería.
    /// </summary>
    public class BarbershopMembershipRequest
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int BarbershopId { get; set; }
        
        [Required]
        public int BarberId { get; set; }
        
        [Required]
        public DateTime RequestDate { get; set; } = DateTime.Now;
        
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = MembershipRequestStatus.Pending;
        
        public DateTime? ResponseDate { get; set; }
        
        [MaxLength(500)]
        [Display(Name = "Notas del Dueño")]
        public string? OwnerNotes { get; set; }
        
        // Navigation properties
        public virtual Barbershop Barbershop { get; set; } = null!;
        public virtual BarberProfile Barber { get; set; } = null!;
    }
    
    /// <summary>
    /// Estados posibles de una solicitud de membresía.
    /// </summary>
    public static class MembershipRequestStatus
    {
        public const string Pending = "Pending";
        public const string Accepted = "Accepted";
        public const string Rejected = "Rejected";
        public const string Cancelled = "Cancelled";
    }
    
    /// <summary>
    /// Notificación del sistema para usuarios.
    /// </summary>
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string Type { get; set; } = NotificationType.Info;
        
        [MaxLength(500)]
        public string? RelatedUrl { get; set; }
        
        public bool IsRead { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation property
        public virtual ApplicationUser User { get; set; } = null!;
    }
    
    /// <summary>
    /// Tipos de notificaciones.
    /// </summary>
    public static class NotificationType
    {
        public const string Info = "Info";
        public const string NewMembershipRequest = "NewMembershipRequest";
        public const string RequestAccepted = "RequestAccepted";
        public const string RequestRejected = "RequestRejected";
        public const string BarberLeft = "BarberLeft";
        public const string AppointmentReminder = "AppointmentReminder";
        public const string System = "System";
    }
}
