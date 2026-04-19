using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AppointmentService _appointmentService;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public ClientController(
            ApplicationDbContext context,
            AppointmentService appointmentService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _appointmentService = appointmentService;
            _userManager = userManager;
        }
        
        public async Task<IActionResult> MyAppointments()
        {
            var userId = _userManager.GetUserId(User);
            var appointments = await _appointmentService.GetAppointmentsByClientAsync(userId!);
            return View(appointments);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            
            if (appointment == null || appointment.ClientId != _userManager.GetUserId(User))
            {
                return NotFound();
            }
            
            await _appointmentService.CancelAppointmentAsync(id);
            TempData["Success"] = "Reservación cancelada exitosamente";
            
            return RedirectToAction(nameof(MyAppointments));
        }
        
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId!);
            
            if (user == null)
            {
                return NotFound();
            }
            
            return View(user);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string fullName, string phone)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId!);
            
            if (user == null)
            {
                return NotFound();
            }
            
            user.FullName = fullName;
            user.PhoneNumber = phone;
            
            var result = await _userManager.UpdateAsync(user);
            
            if (result.Succeeded)
            {
                TempData["Success"] = "Perfil actualizado exitosamente";
            }
            
            return RedirectToAction(nameof(Profile));
        }
    }
}
