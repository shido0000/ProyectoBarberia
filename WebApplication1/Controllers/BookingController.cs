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
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BarberService _barberService;
        private readonly AppointmentService _appointmentService;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public BookingController(
            ApplicationDbContext context,
            BarberService barberService,
            AppointmentService appointmentService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _barberService = barberService;
            _appointmentService = appointmentService;
            _userManager = userManager;
        }
        
        // List all barbers
        [HttpGet]
        public async Task<IActionResult> Index(string? searchTerm = null)
        {
            var barbers = await _barberService.GetAllActiveBarbersAsync();
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                barbers = barbers.Where(b => 
                    b.ShopName != null && b.ShopName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    b.Description != null && b.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    b.Address != null && b.Address.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }
            
            var viewModel = new BarberListViewModel
            {
                Barbers = barbers,
                SearchTerm = searchTerm
            };
            
            return View(viewModel);
        }
        
        // View barber details
        [HttpGet]
        public async Task<IActionResult> Barber(int id)
        {
            var barber = await _barberService.GetBarberByIdAsync(id);
            if (barber == null)
            {
                return NotFound();
            }
            
            return View(barber);
        }
        
        // Book appointment
        [Authorize(Roles = "Client")]
        [HttpGet]
        public async Task<IActionResult> Book(int barberId)
        {
            var barber = await _barberService.GetBarberByIdAsync(barberId);
            if (barber == null)
            {
                return NotFound();
            }
            
            var services = await _barberService.GetServicesByBarberAsync(barberId);
            if (!services.Any())
            {
                TempData["Error"] = "Este barbero no tiene servicios disponibles";
                return RedirectToAction(nameof(Barber), new { id = barberId });
            }
            
            ViewBag.Barber = barber;
            ViewBag.Services = services;
            
            var viewModel = new AppointmentViewModel
            {
                BarberProfileId = barberId,
                Date = DateTime.Now
            };
            
            return View(viewModel);
        }
        
        [Authorize(Roles = "Client")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(AppointmentViewModel model)
        {
            var barber = await _barberService.GetBarberByIdAsync(model.BarberProfileId);
            if (barber == null)
            {
                return NotFound();
            }
            
            var services = await _barberService.GetServicesByBarberAsync(model.BarberProfileId);
            ViewBag.Barber = barber;
            ViewBag.Services = services;
            
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var userId = _userManager.GetUserId(User);
            var service = services.FirstOrDefault(s => s.Id == model.ServiceId);
            
            if (service == null)
            {
                ModelState.AddModelError("ServiceId", "Servicio no válido");
                return View(model);
            }
            
            var appointment = new Appointment
            {
                ClientId = userId!,
                BarberProfileId = model.BarberProfileId,
                ServiceId = model.ServiceId,
                Date = model.Date,
                Time = model.Time,
                Notes = model.Notes
            };

            var success = await _appointmentService.CreateAppointmentAsync(appointment, service.DurationMinutes);
            
            if (success)
            {
                TempData["Success"] = "¡Reservación creada exitosamente!";
                return RedirectToAction("MyAppointments", "Client");
            }
            else
            {
                ModelState.AddModelError("", "El horario seleccionado no está disponible. Por favor seleccione otro.");
                return View(model);
            }
        }
        
        // Get available time slots (AJAX)
        [HttpGet]
        public async Task<JsonResult> GetAvailableTimeSlots(int barberId, DateTime date, int serviceId)
        {
            var service = await _context.Services.FindAsync(serviceId);
            if (service == null)
            {
                return Json(new { success = false, slots = new List<string>() });
            }
            
            var slots = await _appointmentService.GetAvailableTimeSlotsAsync(barberId, date, service.DurationMinutes);
            var formattedSlots = slots.Select(s => $"{s.Hours:D2}:{s.Minutes:D2}").ToList();
            
            return Json(new { success = true, slots = formattedSlots });
        }
    }
}
