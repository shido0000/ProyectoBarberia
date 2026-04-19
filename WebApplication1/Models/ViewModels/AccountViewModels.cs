using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "El nombre completo es requerido")]
        [Display(Name = "Nombre Completo")]
        public string FullName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;
        
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El rol es requerido")]
        [Display(Name = "Tipo de Usuario")]
        public string Role { get; set; } = "Client"; // Barber, Client
    }
    
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;
        
        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
    }
    
    public class AppointmentViewModel
    {
        [Required(ErrorMessage = "Seleccione un barbero")]
        [Display(Name = "Barbero")]
        public int BarberProfileId { get; set; }
        
        [Required(ErrorMessage = "Seleccione un servicio")]
        [Display(Name = "Servicio")]
        public int ServiceId { get; set; }
        
        [Required(ErrorMessage = "Seleccione una fecha")]
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;
        
        [Required(ErrorMessage = "Seleccione una hora")]
        [Display(Name = "Hora")]
        [DataType(DataType.Time)]
        public TimeSpan Time { get; set; }
        
        [Display(Name = "Notas")]
        public string? Notes { get; set; }
    }
    
    public class BarberDashboardViewModel
    {
        public BarberProfile BarberProfile { get; set; } = null!;
        public List<Appointment> TodayAppointments { get; set; } = new();
        public List<Appointment> UpcomingAppointments { get; set; } = new();
        public List<Service> Services { get; set; } = new();
        public Subscription? ActiveSubscription { get; set; }
        public decimal TotalEarnings { get; set; }
        public int TotalAppointmentsThisMonth { get; set; }
    }
    
    public class BarberListViewModel
    {
        public List<BarberProfile> Barbers { get; set; } = new();
        public string? SearchTerm { get; set; }
    }
    
    public class ServiceViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [Display(Name = "Nombre del Servicio")]
        public string Name { get; set; } = string.Empty;
        
        [Display(Name = "Descripción")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "El precio es requerido")]
        [Display(Name = "Precio")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "La duración es requerida")]
        [Display(Name = "Duración (minutos)")]
        public int DurationMinutes { get; set; }
    }
}
