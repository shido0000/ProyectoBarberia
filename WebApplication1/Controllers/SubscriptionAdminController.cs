using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SubscriptionAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public SubscriptionAdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // List all subscription plans
        public async Task<IActionResult> Index()
        {
            var plans = await _context.SubscriptionPlans
                .Include(p => p.Subscriptions)
                .ToListAsync();
            
            // Order by price in memory since SQLite doesn't support decimal in ORDER BY
            plans = plans.OrderByDescending(p => p.MonthlyPrice).ToList();
            
            ViewBag.TotalSubscriptions = await _context.Subscriptions.CountAsync(s => s.IsActive);
            ViewBag.TotalRevenue = await _context.Subscriptions
                .Join(_context.SubscriptionPlans, 
                    s => s.SubscriptionPlanId, 
                    p => p.Id, 
                    (s, p) => p.MonthlyPrice)
                .SumAsync();
            
            return View(plans);
        }
        
        // Create new plan
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubscriptionPlan model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            model.CreatedAt = DateTime.Now;
            
            // If this is set as default, unset others
            if (model.IsDefault)
            {
                var allPlans = await _context.SubscriptionPlans.ToListAsync();
                foreach (var plan in allPlans)
                {
                    plan.IsDefault = false;
                }
            }
            
            _context.SubscriptionPlans.Add(model);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Plan de suscripción creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        
        // Edit plan
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(id);
            if (plan == null)
            {
                return NotFound();
            }
            
            return View(plan);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubscriptionPlan model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }
            
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var existingPlan = await _context.SubscriptionPlans.FindAsync(id);
            if (existingPlan == null)
            {
                return NotFound();
            }
            
            // If this is set as default, unset others
            if (model.IsDefault && !existingPlan.IsDefault)
            {
                var allPlans = await _context.SubscriptionPlans
                    .Where(p => p.Id != id)
                    .ToListAsync();
                foreach (var plan in allPlans)
                {
                    plan.IsDefault = false;
                }
            }
            
            existingPlan.Name = model.Name;
            existingPlan.Description = model.Description;
            existingPlan.MonthlyPrice = model.MonthlyPrice;
            existingPlan.AnnualPrice = model.AnnualPrice;
            existingPlan.DurationDays = model.DurationDays;
            existingPlan.MaxBarbers = model.MaxBarbers;
            existingPlan.MaxServicesPerBarber = model.MaxServicesPerBarber;
            existingPlan.IncludeAnalytics = model.IncludeAnalytics;
            existingPlan.PrioritySupport = model.PrioritySupport;
            existingPlan.CustomBranding = model.CustomBranding;
            existingPlan.AutoReminders = model.AutoReminders;
            existingPlan.ExportReports = model.ExportReports;
            existingPlan.IsActive = model.IsActive;
            existingPlan.IsDefault = model.IsDefault;
            
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Plan de suscripción actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        
        // Delete plan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(id);
            if (plan == null)
            {
                return NotFound();
            }
            
            // Check if there are active subscriptions
            var activeSubscriptions = await _context.Subscriptions
                .CountAsync(s => s.SubscriptionPlanId == id && s.IsActive);
            
            if (activeSubscriptions > 0)
            {
                TempData["Error"] = "No se puede eliminar el plan porque tiene suscripciones activas";
                return RedirectToAction(nameof(Index));
            }
            
            _context.SubscriptionPlans.Remove(plan);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Plan de suscripción eliminado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        
        // Toggle plan active status
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePlanStatus(int id)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(id);
            if (plan == null)
            {
                return NotFound();
            }
            
            plan.IsActive = !plan.IsActive;
            await _context.SaveChangesAsync();
            
            TempData["Success"] = $"Plan {(plan.IsActive ? "activado" : "desactivado")} exitosamente";
            return RedirectToAction(nameof(Index));
        }
        
        // Assign plan to user
        [HttpGet]
        public async Task<IActionResult> Assign()
        {
            var plans = await _context.SubscriptionPlans
                .Where(p => p.IsActive)
                .ToListAsync();
            
            var barbers = await _context.Users
                .Where(u => u.Role == "Barber")
                .Include(u => u.BarberProfile)
                .ToListAsync();
            
            ViewBag.Plans = plans;
            ViewBag.Barbers = barbers;
            
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(string userId, int planId, int months = 1)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(planId);
            var user = await _context.Users.FindAsync(userId);
            
            if (plan == null || user == null)
            {
                TempData["Error"] = "Plan o usuario no válido";
                return RedirectToAction(nameof(Assign));
            }
            
            // Check if user already has an active subscription
            var existingSubscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive);
            
            if (existingSubscription != null)
            {
                // Update existing subscription
                existingSubscription.SubscriptionPlanId = planId;
                existingSubscription.StartDate = DateTime.Now;
                existingSubscription.EndDate = DateTime.Now.AddDays(plan.DurationDays * months);
                existingSubscription.IsActive = true;
            }
            else
            {
                // Create new subscription
                var newSubscription = new Subscription
                {
                    UserId = userId,
                    SubscriptionPlanId = planId,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(plan.DurationDays * months),
                    IsActive = true
                };
                _context.Subscriptions.Add(newSubscription);
            }
            
            await _context.SaveChangesAsync();
            
            TempData["Success"] = $"Suscripción '{plan.Name}' asignada a {user.FullName}";
            return RedirectToAction(nameof(Assign));
        }
        
        // View all active subscriptions
        public async Task<IActionResult> Subscriptions()
        {
            var subscriptions = await _context.Subscriptions
                .Include(s => s.User)
                .Include(s => s.SubscriptionPlan)
                .Where(s => s.IsActive)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
            
            return View(subscriptions);
        }
        
        // Cancel subscription
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelSubscription(int id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }
            
            subscription.IsActive = false;
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Suscripción cancelada exitosamente";
            return RedirectToAction(nameof(Subscriptions));
        }
    }
}
