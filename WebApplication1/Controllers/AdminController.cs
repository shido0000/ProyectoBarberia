using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        
        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        
        public async Task<IActionResult> Index()
        {
            var totalBarbers = await _context.Users.CountAsync(u => u.Role == "Barber");
            var totalClients = await _context.Users.CountAsync(u => u.Role == "Client");
            var totalAppointments = await _context.Appointments.CountAsync();
            var totalActiveSubscriptions = await _context.Subscriptions.CountAsync(s => s.IsActive);
            
            ViewBag.TotalBarbers = totalBarbers;
            ViewBag.TotalClients = totalClients;
            ViewBag.TotalAppointments = totalAppointments;
            ViewBag.TotalActiveSubscriptions = totalActiveSubscriptions;
            
            return View();
        }
        
        public async Task<IActionResult> Barbers()
        {
            var barbers = await _context.Users
                .Where(u => u.Role == "Barber")
                .Include(u => u.BarberProfile)
                .ToListAsync();
            
            return View(barbers);
        }
        
        public async Task<IActionResult> Clients()
        {
            var clients = await _context.Users
                .Where(u => u.Role == "Client")
                .ToListAsync();
            
            return View(clients);
        }
        
        public async Task<IActionResult> Appointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Client)
                .Include(a => a.BarberProfile)
                .Include(a => a.Service)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
            
            return View(appointments);
        }
    }
}
