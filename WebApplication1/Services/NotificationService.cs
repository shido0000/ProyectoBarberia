using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface INotificationService
    {
        Task<List<Notification>> GetUnreadNotificationsAsync(string userId);
        Task<List<Notification>> GetAllNotificationsAsync(string userId, int count = 10);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(string userId);
        Task CreateNotificationAsync(string userId, string message, string type, string? relatedUrl = null);
        Task<int> GetUnreadCountAsync(string userId);
    }

    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetUnreadNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .Take(10)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetAllNotificationsAsync(string userId, int count = 10)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task CreateNotificationAsync(string userId, string message, string type, string? relatedUrl = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                Type = type,
                RelatedUrl = relatedUrl,
                CreatedAt = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }
    }
}
