using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Muestra todos los testimonios (ratings con comentario) de clientes hacia barberos y barberías.
        /// </summary>
        public async Task<IActionResult> Testimonials(int page = 1)
        {
            const int pageSize = 12;
            
            var query = _context.Ratings
                .Include(r => r.Client)
                .Include(r => r.Barber)
                .Include(r => r.Barbershop)
                .Where(r => !string.IsNullOrEmpty(r.Comment))
                .OrderByDescending(r => r.CreatedAt);

            var totalItems = await query.CountAsync();
            var ratings = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["TotalPages"] = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewData["CurrentPage"] = page;
            ViewData["TotalItems"] = totalItems;

            return View(ratings);
        }

        /// <summary>
        /// Muestra los planes de suscripción disponibles para barberos y barberías.
        /// </summary>
        public async Task<IActionResult> Pricing()
        {
            var barberPlans = await _context.SubscriptionPlans
                .Where(p => p.IsActive)
                .OrderBy(p => p.MonthlyPrice)
                .ToListAsync();

            var barbershopPlans = await _context.BarbershopSubscriptionPlans
                .Where(p => p.IsActive)
                .OrderBy(p => p.MonthlyPrice)
                .ToListAsync();

            ViewBag.BarberPlans = barberPlans;
            ViewBag.BarbershopPlans = barbershopPlans;

            return View();
        }

        /// <summary>
        /// Muestra todas las características y funcionalidades del sistema para clientes.
        /// </summary>
        public IActionResult Features()
        {
            return View();
        }

        /// <summary>
        /// Muestra los términos y condiciones del servicio.
        /// </summary>
        public IActionResult Terms()
        {
            return View();
        }

        /// <summary>
        /// Muestra el centro de ayuda con guías por rol de usuario.
        /// </summary>
        public IActionResult Help()
        {
            return View();
        }

        /// <summary>
        /// Muestra la página de contacto.
        /// </summary>
        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
