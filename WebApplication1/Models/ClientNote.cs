using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    /// <summary>
    /// Notas privadas que un barbero puede agregar sobre un cliente (CRM - Premium).
    /// </summary>
    public class ClientNote
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int BarberProfileId { get; set; }
        
        [Required]
        public string ClientId { get; set; } = string.Empty;
        //public int ClientId { get; set; }

        [Required]
        [MaxLength(1000)]
        [Display(Name = "Nota")]
        public string Note { get; set; } = string.Empty;
        
        [Display(Name = "Tipo de Nota")]
        public NoteType NoteType { get; set; } = NoteType.General;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public virtual BarberProfile BarberProfile { get; set; } = null!;
        public virtual ApplicationUser Client { get; set; } = null!;
    }
    
    public enum NoteType
    {
        General,        // Nota general
        Preference,     // Preferencia de corte/estilo
        Allergy,        // Alergia o sensibilidad
        Complaint,      // Queja o incidencia
        FollowUp        // Seguimiento
    }
}
