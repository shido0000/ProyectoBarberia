using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Get notifications for navbar dropdown (AJAX endpoint)
        [HttpGet]
        public async Task<IActionResult> GetNotifications(int count = 5)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(count)
                .ToListAsync();

            var unreadCount = await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);

            return Ok(new
            {
                notifications = notifications.Select(n => new
                {
                    n.Id,
                    n.Message,
                    n.Type,
                    n.RelatedUrl,
                    n.IsRead,
                    CreatedAt = n.CreatedAt.ToString("dd/MM/yyyy HH:mm")
                }).ToList(),
                unreadCount
            });
        }

        // Mark notification as read
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (notification == null)
            {
                return NotFound();
            }

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok();
        }

        // Mark all notifications as read
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        // View all notifications page
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View(notifications);
        }
    }
}
