using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    public class BarbershopController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public BarbershopController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        // List all barbershops (public)
        public async Task<IActionResult> Index(string? searchTerm, int? planId)
        {
            var query = _context.Barbershops
                .Include(b => b.Plan)
                .Include(b => b.Owner)
                .Where(b => b.IsActive)
                .AsQueryable();
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(b => b.Name.Contains(searchTerm) || 
                                        (b.Description != null && b.Description.Contains(searchTerm)) ||
                                        (b.Address != null && b.Address.Contains(searchTerm)));
            }
            
            if (planId.HasValue)
            {
                query = query.Where(b => b.BarbershopSubscriptionPlanId == planId.Value);
            }
            
            var barbershops = await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
            
            ViewBag.Plans = await _context.BarbershopSubscriptionPlans
                .Where(p => p.IsActive)
                .ToListAsync();
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SelectedPlanId = planId;
            
            return View(barbershops);
        }
        
        // Details of a specific barbershop (public)
        public async Task<IActionResult> Details(int id)
        {
            var barbershop = await _context.Barbershops
                .Include(b => b.Plan)
                .Include(b => b.Owner)
                    .ThenInclude(o => o.User)
                .Include(b => b.Members)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(b => b.Id == id);
            
            if (barbershop == null)
            {
                return NotFound();
            }
            
            // Check if current user is a barber and already belongs to a barbershop
            bool canRequestJoin = false;
            bool hasPendingRequest = false;
            bool isMember = false;
            
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("Barber"))
            {
                var userId = _userManager.GetUserId(User)!;
                var barberProfile = await _context.BarberProfiles
                    .FirstOrDefaultAsync(b => b.UserId == userId);
                
                if (barberProfile != null)
                {
                    if (barberProfile.CurrentBarbershopId != null)
                    {
                        isMember = true;
                    }
                    else
                    {
                        // Check capacity
                        if (barbershop.Plan.MaxBarbers == null || 
                            barbershop.Members.Count < barbershop.Plan.MaxBarbers)
                        {
                            canRequestJoin = true;
                            
                            // Check if already has a pending request
                            hasPendingRequest = await _context.BarbershopMembershipRequests
                                .AnyAsync(r => r.BarbershopId == id && 
                                              r.BarberId == barberProfile.Id && 
                                              r.Status == MembershipRequestStatus.Pending);
                        }
                    }
                }
            }
            
            ViewBag.CanRequestJoin = canRequestJoin;
            ViewBag.HasPendingRequest = hasPendingRequest;
            ViewBag.IsMember = isMember;
            
            return View(barbershop);
        }
        
        // Create a new barbershop (only for barbers)
        [Authorize(Roles = "Barber")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Plans = await _context.BarbershopSubscriptionPlans
                .Where(p => p.IsActive)
                .ToListAsync();
            
            return View();
        }
        
        [Authorize(Roles = "Barber")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Barbershop model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Plans = await _context.BarbershopSubscriptionPlans
                    .Where(p => p.IsActive)
                    .ToListAsync();
                return View(model);
            }
            
            var userId = _userManager.GetUserId(User)!;
            var barberProfile = await _context.BarberProfiles
                .FirstOrDefaultAsync(b => b.UserId == userId);
            
            if (barberProfile == null)
            {
                TempData["Error"] = "No se encontró tu perfil de barbero";
                return RedirectToAction(nameof(Create));
            }
            
            model.OwnerBarberId = barberProfile.Id;
            model.CreatedAt = DateTime.Now;
            model.IsActive = true;
            
            _context.Barbershops.Add(model);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = $"¡Barbería '{model.Name}' creada exitosamente!";
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }
        
        // Edit barbershop (only for owner or admin)
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var barbershop = await _context.Barbershops.FindAsync(id);
            if (barbershop == null)
            {
                return NotFound();
            }
            
            // Check if user is owner or admin
            var userId = _userManager.GetUserId(User)!;
            var barberProfile = await _context.BarberProfiles
                .FirstOrDefaultAsync(b => b.UserId == userId);
            
            if (!User.IsInRole("Admin") && (barberProfile == null || barberProfile.Id != barbershop.OwnerBarberId))
            {
                return Forbid();
            }
            
            ViewBag.Plans = await _context.BarbershopSubscriptionPlans
                .Where(p => p.IsActive)
                .ToListAsync();
            
            return View(barbershop);
        }
        
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Barbershop model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }
            
            var existingBarbershop = await _context.Barbershops.FindAsync(id);
            if (existingBarbershop == null)
            {
                return NotFound();
            }
            
            // Check if user is owner or admin
            var userId = _userManager.GetUserId(User)!;
            var barberProfile = await _context.BarberProfiles
                .FirstOrDefaultAsync(b => b.UserId == userId);
            
            if (!User.IsInRole("Admin") && (barberProfile == null || barberProfile.Id != existingBarbershop.OwnerBarberId))
            {
                return Forbid();
            }
            
            existingBarbershop.Name = model.Name;
            existingBarbershop.Description = model.Description;
            existingBarbershop.Address = model.Address;
            existingBarbershop.Phone = model.Phone;
            existingBarbershop.LogoUrl = model.LogoUrl;
            existingBarbershop.CoverImageUrl = model.CoverImageUrl;
            
            // Only admin can change plan
            if (User.IsInRole("Admin"))
            {
                existingBarbershop.BarbershopSubscriptionPlanId = model.BarbershopSubscriptionPlanId;
            }
            
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Barbería actualizada exitosamente";
            return RedirectToAction(nameof(Details), new { id = id });
        }
        
        // Manage membership requests (only for owner or admin)
        [Authorize]
        public async Task<IActionResult> ManageRequests(int id)
        {
            var barbershop = await _context.Barbershops
                .Include(b => b.MembershipRequests)
                    .ThenInclude(r => r.Barber)
                        .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);
            
            if (barbershop == null)
            {
                return NotFound();
            }
            
            // Check if user is owner or admin
            var userId = _userManager.GetUserId(User)!;
            var barberProfile = await _context.BarberProfiles
                .FirstOrDefaultAsync(b => b.UserId == userId);
            
            if (!User.IsInRole("Admin") && (barberProfile == null || barberProfile.Id != barbershop.OwnerBarberId))
            {
                return Forbid();
            }
            
            return View(barbershop);
        }
        
        // Accept membership request
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptRequest(int requestId)
        {
            var request = await _context.BarbershopMembershipRequests
                .Include(r => r.Barbershop)
                .Include(r => r.Barber)
                .FirstOrDefaultAsync(r => r.Id == requestId);
            
            if (request == null)
            {
                TempData["Error"] = "Solicitud no encontrada";
                return RedirectToAction(nameof(Index));
            }
            
            // Check if user is owner or admin
            var userId = _userManager.GetUserId(User)!;
            var barberProfile = await _context.BarberProfiles
                .FirstOrDefaultAsync(b => b.UserId == userId);
            
            if (!User.IsInRole("Admin") && (barberProfile == null || barberProfile.Id != request.Barbershop.OwnerBarberId))
            {
                return Forbid();
            }
            
            // Check capacity again
            if (request.Barbershop.Plan.MaxBarbers != null && 
                request.Barbershop.Members.Count >= request.Barbershop.Plan.MaxBarbers)
            {
                TempData["Error"] = "La barbería ha alcanzado su límite de barberos";
                return RedirectToAction(nameof(ManageRequests), new { id = request.BarbershopId });
            }
            
            request.Status = MembershipRequestStatus.Accepted;
            request.ResponseDate = DateTime.Now;
            
            // Update barber's barbershop
            request.Barber.CurrentBarbershopId = request.BarbershopId;
            
            // Create notification for the barber
            var notification = new Notification
            {
                UserId = request.Barber.UserId,
                Message = $"¡Felicidades! Tu solicitud para unirte a {request.Barbershop.Name} ha sido aceptada.",
                Type = NotificationType.RequestAccepted,
                RelatedUrl = $"/Barbershop/Details/{request.BarbershopId}",
                CreatedAt = DateTime.Now
            };
            _context.Notifications.Add(notification);
            
            await _context.SaveChangesAsync();
            
            TempData["Success"] = $"Solicitud aceptada. {request.Barber.User.FullName} ahora es miembro de {request.Barbershop.Name}";
            return RedirectToAction(nameof(ManageRequests), new { id = request.BarbershopId });
        }
        
        // Reject membership request
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRequest(int requestId, string? notes = null)
        {
            var request = await _context.BarbershopMembershipRequests
                .Include(r => r.Barbershop)
                .Include(r => r.Barber)
                .FirstOrDefaultAsync(r => r.Id == requestId);
            
            if (request == null)
            {
                TempData["Error"] = "Solicitud no encontrada";
                return RedirectToAction(nameof(Index));
            }
            
            // Check if user is owner or admin
            var userId = _userManager.GetUserId(User)!;
            var barberProfile = await _context.BarberProfiles
                .FirstOrDefaultAsync(b => b.UserId == userId);
            
            if (!User.IsInRole("Admin") && (barberProfile == null || barberProfile.Id != request.Barbershop.OwnerBarberId))
            {
                return Forbid();
            }
            
            request.Status = MembershipRequestStatus.Rejected;
            request.ResponseDate = DateTime.Now;
            request.OwnerNotes = notes;
            
            // Create notification for the barber
            var notification = new Notification
            {
                UserId = request.Barber.UserId,
                Message = $"Tu solicitud para unirte a {request.Barbershop.Name} ha sido rechazada.",
                Type = NotificationType.RequestRejected,
                CreatedAt = DateTime.Now
            };
            _context.Notifications.Add(notification);
            
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Solicitud rechazada";
            return RedirectToAction(nameof(ManageRequests), new { id = request.BarbershopId });
        }
        
        // Send membership request (for barbers)
        [Authorize(Roles = "Barber")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendRequest(int barbershopId)
        {
            var userId = _userManager.GetUserId(User)!;
            var barberProfile = await _context.BarberProfiles
                .FirstOrDefaultAsync(b => b.UserId == userId);
            
            if (barberProfile == null)
            {
                TempData["Error"] = "No se encontró tu perfil de barbero";
                return RedirectToAction(nameof(Index));
            }
            
            if (barberProfile.CurrentBarbershopId != null)
            {
                TempData["Error"] = "Ya perteneces a una barbería";
                return RedirectToAction(nameof(Index));
            }
            
            var barbershop = await _context.Barbershops
                .Include(b => b.Members)
                .FirstOrDefaultAsync(b => b.Id == barbershopId);
            
            if (barbershop == null)
            {
                TempData["Error"] = "Barbería no encontrada";
                return RedirectToAction(nameof(Index));
            }
            
            // Check capacity
            if (barbershop.Plan.MaxBarbers != null && 
                barbershop.Members.Count >= barbershop.Plan.MaxBarbers)
            {
                TempData["Error"] = "Esta barbería ha alcanzado su límite de barberos";
                return RedirectToAction(nameof(Details), new { id = barbershopId });
            }
            
            // Check if already has a pending request
            var existingRequest = await _context.BarbershopMembershipRequests
                .FirstOrDefaultAsync(r => r.BarbershopId == barbershopId && 
                                         r.BarberId == barberProfile.Id && 
                                         r.Status == MembershipRequestStatus.Pending);
            
            if (existingRequest != null)
            {
                TempData["Error"] = "Ya tienes una solicitud pendiente para esta barbería";
                return RedirectToAction(nameof(Details), new { id = barbershopId });
            }
            
            var request = new BarbershopMembershipRequest
            {
                BarbershopId = barbershopId,
                BarberId = barberProfile.Id,
                RequestDate = DateTime.Now,
                Status = MembershipRequestStatus.Pending
            };
            
            _context.BarbershopMembershipRequests.Add(request);
            
            // Create notification for the owner
            var ownerNotification = new Notification
            {
                UserId = barbershop.Owner.UserId,
                Message = $"{barberProfile.User.FullName} ha solicitado unirse a {barbershop.Name}",
                Type = NotificationType.NewMembershipRequest,
                RelatedUrl = $"/Barbershop/ManageRequests/{barbershopId}",
                CreatedAt = DateTime.Now
            };
            _context.Notifications.Add(ownerNotification);
            
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Solicitud enviada. El dueño de la barbería será notificado.";
            return RedirectToAction(nameof(Details), new { id = barbershopId });
        }
        
        // Cancel membership request (for barbers)
        [Authorize(Roles = "Barber")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelRequest(int requestId)
        {
            var userId = _userManager.GetUserId(User)!;
            var barberProfile = await _context.BarberProfiles
                .FirstOrDefaultAsync(b => b.UserId == userId);
            
            if (barberProfile == null)
            {
                TempData["Error"] = "No se encontró tu perfil de barbero";
                return RedirectToAction(nameof(Index));
            }
            
            var request = await _context.BarbershopMembershipRequests
                .FirstOrDefaultAsync(r => r.Id == requestId && r.BarberId == barberProfile.Id);
            
            if (request == null)
            {
                TempData["Error"] = "Solicitud no encontrada";
                return RedirectToAction(nameof(Index));
            }
            
            if (request.Status != MembershipRequestStatus.Pending)
            {
                TempData["Error"] = "Solo se pueden cancelar solicitudes pendientes";
                return RedirectToAction(nameof(Index));
            }
            
            request.Status = MembershipRequestStatus.Cancelled;
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Solicitud cancelada";
            return RedirectToAction(nameof(Details), new { id = request.BarbershopId });
        }
        
        // Leave barbershop (for members)
        [Authorize(Roles = "Barber")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LeaveBarbershop(int barbershopId)
        {
            var userId = _userManager.GetUserId(User)!;
            var barberProfile = await _context.BarberProfiles
                .FirstOrDefaultAsync(b => b.UserId == userId);
            
            if (barberProfile == null || barberProfile.CurrentBarbershopId != barbershopId)
            {
                TempData["Error"] = "No eres miembro de esta barbería";
                return RedirectToAction(nameof(Index));
            }
            
            var barbershop = await _context.Barbershops
                .FirstOrDefaultAsync(b => b.Id == barbershopId);
            
            if (barbershop == null)
            {
                TempData["Error"] = "Barbería no encontrada";
                return RedirectToAction(nameof(Index));
            }
            
            // Don't allow owner to leave (they should transfer ownership or delete)
            if (barberProfile.Id == barbershop.OwnerBarberId)
            {
                TempData["Error"] = "El dueño no puede dejar la barbería. Debe transferir la propiedad o eliminarla.";
                return RedirectToAction(nameof(Details), new { id = barbershopId });
            }
            
            barberProfile.CurrentBarbershopId = null;
            
            // Create notification for the owner
            var ownerNotification = new Notification
            {
                UserId = barbershop.Owner.UserId,
                Message = $"{barberProfile.User.FullName} ha dejado {barbershop.Name}",
                Type = NotificationType.BarberLeft,
                CreatedAt = DateTime.Now
            };
            _context.Notifications.Add(ownerNotification);
            
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Has dejado la barbería exitosamente";
            return RedirectToAction(nameof(Details), new { id = barbershopId });
        }
        
        // My Barbershop dashboard (for owners)
        [Authorize(Roles = "Barber")]
        public async Task<IActionResult> MyBarbershop()
        {
            var userId = _userManager.GetUserId(User)!;
            var barberProfile = await _context.BarberProfiles
                .FirstOrDefaultAsync(b => b.UserId == userId);
            
            if (barberProfile == null)
            {
                TempData["Error"] = "No se encontró tu perfil de barbero";
                return RedirectToAction(nameof(Index));
            }
            
            var barbershop = await _context.Barbershops
                .Include(b => b.Plan)
                .Include(b => b.Members)
                    .ThenInclude(m => m.User)
                .Include(b => b.MembershipRequests)
                    .ThenInclude(r => r.Barber)
                        .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(b => b.OwnerBarberId == barberProfile.Id);
            
            if (barbershop == null)
            {
                return View("NoBarbershop");
            }
            
            // Get upcoming appointments for all barbers in the shop
            var teamMemberIds = barbershop.Members.Select(m => m.Id).ToList();
            var upcomingAppointments = await _context.Appointments
                .Include(a => a.Client)
                .Include(a => a.Service)
                .Include(a => a.BarberProfile)
                .Where(a => teamMemberIds.Contains(a.BarberProfileId) &&
                           a.Date >= DateTime.Now &&
                           a.Status != AppointmentStatus.Cancelled)
                .OrderBy(a => a.Date)
                .Take(10)
                .ToListAsync();
            
            // Calculate stats for this month
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var totalAppointmentsThisMonth = await _context.Appointments
                .CountAsync(a => teamMemberIds.Contains(a.BarberProfileId) &&
                                a.Date >= startOfMonth &&
                                a.Status != AppointmentStatus.Cancelled);
            
            var totalRevenueThisMonth = await _context.Appointments
                .Where(a => teamMemberIds.Contains(a.BarberProfileId) &&
                           a.Date >= startOfMonth &&
                           a.Status == AppointmentStatus.Completed)
                .SumAsync(a => a.Service.Price);
            
            var pendingRequests = barbershop.MembershipRequests
                .Where(r => r.Status == MembershipRequestStatus.Pending)
                .ToList();
            
            var viewModel = new BarbershopOwnerDashboardViewModel
            {
                Barbershop = barbershop,
                TeamMembers = barbershop.Members.ToList(),
                PendingRequests = pendingRequests,
                UpcomingAppointments = upcomingAppointments,
                SubscriptionPlan = barbershop.Plan,
                TotalAppointmentsThisMonth = totalAppointmentsThisMonth,
                TotalRevenueThisMonth = totalRevenueThisMonth
            };
            
            return View("OwnerDashboard", viewModel);
        }
    }
}
