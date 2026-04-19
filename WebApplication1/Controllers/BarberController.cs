using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Barber")]
    public class BarberController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BarberService _barberService;
        private readonly AppointmentService _appointmentService;
        private readonly SubscriptionService _subscriptionService;
        private readonly ImageService _imageService;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public BarberController(
            ApplicationDbContext context,
            BarberService barberService,
            AppointmentService appointmentService,
            SubscriptionService subscriptionService,
            ImageService imageService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _barberService = barberService;
            _appointmentService = appointmentService;
            _subscriptionService = subscriptionService;
            _imageService = imageService;
            _userManager = userManager;
        }
        
        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);
            var barberProfile = await _context.BarberProfiles
                .FirstOrDefaultAsync(b => b.UserId == userId);
            
            if (barberProfile == null)
            {
                return NotFound();
            }
            
            var todayAppointments = await _appointmentService.GetTodayAppointmentsAsync(barberProfile.Id);
            var upcomingAppointments = await _appointmentService.GetUpcomingAppointmentsAsync(barberProfile.Id);
            var services = await _barberService.GetServicesByBarberAsync(barberProfile.Id);
            var subscription = await _subscriptionService.GetActiveSubscriptionAsync(userId);
            
            var totalEarnings = todayAppointments
                .Where(a => a.Status == AppointmentStatus.Completed)
                .Sum(a => a.Service.Price);
            
            var totalAppointmentsThisMonth = await _context.Appointments
                .CountAsync(a => a.BarberProfileId == barberProfile.Id && 
                                a.Date.Month == DateTime.Now.Month && 
                                a.Date.Year == DateTime.Now.Year &&
                                a.Status != AppointmentStatus.Cancelled);
            
            var viewModel = new BarberDashboardViewModel
            {
                BarberProfile = barberProfile,
                TodayAppointments = todayAppointments,
                UpcomingAppointments = upcomingAppointments,
                Services = services,
                ActiveSubscription = subscription,
                TotalEarnings = totalEarnings,
                TotalAppointmentsThisMonth = totalAppointmentsThisMonth
            };
            
            return View(viewModel);
        }
        
        // Services Management
        public async Task<IActionResult> Services()
        {
            var userId = _userManager.GetUserId(User);
            var barberProfile = await _context.BarberProfiles
                .FirstOrDefaultAsync(b => b.UserId == userId);
            
            if (barberProfile == null)
            {
                return NotFound();
            }
            
            var services = await _barberService.GetServicesByBarberAsync(barberProfile.Id);
            return View(services);
        }
        
        [HttpGet]
        public IActionResult CreateService()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateService(ServiceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var userId = _userManager.GetUserId(User);
            var barberProfile = await _context.BarberProfiles
                .FirstOrDefaultAsync(b => b.UserId == userId);
            
            if (barberProfile == null)
            {
                return NotFound();
            }
            
            var service = new Service
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                DurationMinutes = model.DurationMinutes,
                BarberProfileId = barberProfile.Id,
                IsActive = true
            };
            
            await _barberService.CreateServiceAsync(service);
            TempData["Success"] = "Servicio creado exitosamente";
            
            return RedirectToAction(nameof(Services));
        }
        
        [HttpGet]
        public async Task<IActionResult> EditService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            
            var viewModel = new ServiceViewModel
            {
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                DurationMinutes = service.DurationMinutes
            };
            
            return View(viewModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditService(int id, ServiceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            
            service.Name = model.Name;
            service.Description = model.Description;
            service.Price = model.Price;
            service.DurationMinutes = model.DurationMinutes;
            
            await _barberService.UpdateServiceAsync(service);
            TempData["Success"] = "Servicio actualizado exitosamente";
            
            return RedirectToAction(nameof(Services));
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteService(int id)
        {
            await _barberService.DeleteServiceAsync(id);
            TempData["Success"] = "Servicio elimininado exitosamente";
            return RedirectToAction(nameof(Services));
        }
        
        // Appointments Management
        public async Task<IActionResult> Appointments(DateTime? date = null)
        {
            var userId = _userManager.GetUserId(User);
            var barberProfile = await _context.BarberProfiles
                .FirstOrDefaultAsync(b => b.UserId == userId);
            
            if (barberProfile == null)
            {
                return NotFound();
            }
            
            var appointments = date.HasValue 
                ? await _appointmentService.GetAppointmentsByBarberAsync(barberProfile.Id, date.Value)
                : await _appointmentService.GetTodayAppointmentsAsync(barberProfile.Id);
            
            ViewData["SelectedDate"] = date ?? DateTime.Now.Date;
            return View(appointments);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, AppointmentStatus status)
        {
            var success = await _appointmentService.UpdateAppointmentStatusAsync(id, status);
            if (success)
            {
                TempData["Success"] = "Estado actualizado exitosamente";
            }
            else
            {
                TempData["Error"] = "Error al actualizar el estado";
            }
            
            return RedirectToAction(nameof(Appointments));
        }
        
        // Profile Management
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = _userManager.GetUserId(User);
            var barberProfile = await _context.BarberProfiles
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.UserId == userId);
            
            if (barberProfile == null)
            {
                return NotFound();
            }
            
            return View(barberProfile);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(BarberProfile model)
        {
            var userId = _userManager.GetUserId(User);
            var barberProfile = await _context.BarberProfiles
                .FirstOrDefaultAsync(b => b.UserId == userId);
            
            if (barberProfile == null)
            {
                return NotFound();
            }
            
            barberProfile.ShopName = model.ShopName;
            barberProfile.Description = model.Description;
            barberProfile.Address = model.Address;
            barberProfile.Phone = model.Phone;
            barberProfile.StartTime = model.StartTime;
            barberProfile.EndTime = model.EndTime;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Perfil actualizado exitosamente";

            return RedirectToAction(nameof(Profile));
        }
        
        // ===== Image Management =====
        
        // Barber Shop Images
        [HttpGet]
        public async Task<IActionResult> ShopImages()
        {
            var userId = _userManager.GetUserId(User);
            var barberProfile = await _context.BarberProfiles
                .FirstOrDefaultAsync(b => b.UserId == userId);
            
            if (barberProfile == null)
            {
                return NotFound();
            }
            
            var images = await _imageService.GetBarberShopImagesAsync(barberProfile.Id);
            ViewBag.BarberProfileId = barberProfile.Id;
            
            return View(images);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadShopImage(IFormFile imageFile, string? description)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                TempData["Error"] = "Seleccione una imagen";
                return RedirectToAction(nameof(ShopImages));
            }
            
            var userId = _userManager.GetUserId(User);
            var barberProfile = await _context.BarberProfiles
                .FirstOrDefaultAsync(b => b.UserId == userId);
            
            if (barberProfile == null)
            {
                return NotFound();
            }
            
            // Validate file size (5MB max)
            if (imageFile.Length > 5 * 1024 * 1024)
            {
                TempData["Error"] = "La imagen no debe superar los 5MB";
                return RedirectToAction(nameof(ShopImages));
            }
            
            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                TempData["Error"] = "Formato de archivo no válido. Use: JPG, PNG, GIF o WEBP";
                return RedirectToAction(nameof(ShopImages));
            }
            
            await _imageService.UploadBarberShopImageAsync(barberProfile.Id, imageFile, description);
            TempData["Success"] = "Imagen subida exitosamente";
            
            return RedirectToAction(nameof(ShopImages));
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteShopImage(int imageId)
        {
            await _imageService.DeleteBarberShopImageAsync(imageId);
            TempData["Success"] = "Imagen eliminada";
            return RedirectToAction(nameof(ShopImages));
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetMainImage(int imageId)
        {
            await _imageService.SetMainImageAsync(imageId);
            TempData["Success"] = "Imagen principal actualizada";
            return RedirectToAction(nameof(ShopImages));
        }
        
        // Service Images
        [HttpGet]
        public async Task<IActionResult> ServiceImages(int serviceId)
        {
            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == serviceId);
            
            if (service == null)
            {
                return NotFound();
            }
            
            var userId = _userManager.GetUserId(User);
            var barberProfile = await _context.BarberProfiles
                .FirstOrDefaultAsync(b => b.UserId == userId);
            
            if (barberProfile == null || service.BarberProfileId != barberProfile.Id)
            {
                return Forbid();
            }
            
            var images = await _imageService.GetServiceImagesAsync(serviceId);
            ViewBag.ServiceId = serviceId;
            ViewBag.ServiceName = service.Name;
            
            return View(images);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadServiceImage(int serviceId, IFormFile imageFile, string? description)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                TempData["Error"] = "Seleccione una imagen";
                return RedirectToAction(nameof(ServiceImages), new { serviceId });
            }
            
            // Validate file size (5MB max)
            if (imageFile.Length > 5 * 1024 * 1024)
            {
                TempData["Error"] = "La imagen no debe superar los 5MB";
                return RedirectToAction(nameof(ServiceImages), new { serviceId });
            }
            
            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                TempData["Error"] = "Formato de archivo no válido. Use: JPG, PNG, GIF o WEBP";
                return RedirectToAction(nameof(ServiceImages), new { serviceId });
            }
            
            await _imageService.UploadServiceImageAsync(serviceId, imageFile, description);
            TempData["Success"] = "Imagen subida exitosamente";
            
            return RedirectToAction(nameof(ServiceImages), new { serviceId });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteServiceImage(int imageId, int serviceId)
        {
            await _imageService.DeleteServiceImageAsync(imageId);
            TempData["Success"] = "Imagen eliminada";
            return RedirectToAction(nameof(ServiceImages), new { serviceId });
        }
    }
}
