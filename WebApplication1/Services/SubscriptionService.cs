using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class SubscriptionService
    {
        private readonly ApplicationDbContext _context;
        
        public SubscriptionService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<Subscription?> GetActiveSubscriptionAsync(string userId)
        {
            return await _context.Subscriptions
                .Include(s => s.SubscriptionPlan)
                .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive && 
                                         (s.EndDate == null || s.EndDate > DateTime.Now));
        }
        
        public async Task<bool> UpgradeSubscriptionAsync(string userId, int planId)
        {
            try
            {
                var plan = await _context.SubscriptionPlans.FindAsync(planId);
                if (plan == null) return false;
                
                // Cancel ALL existing active subscriptions for the user
                var existingSubscriptions = await _context.Subscriptions
                    .Where(s => s.UserId == userId && s.IsActive)
                    .ToListAsync();
                
                foreach (var sub in existingSubscriptions)
                {
                    sub.IsActive = false;
                }
                
                // Create new subscription
                var newSubscription = new Subscription
                {
                    UserId = userId,
                    SubscriptionPlanId = planId,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(plan.DurationDays),
                    IsActive = true
                };
                _context.Subscriptions.Add(newSubscription);
                
                // Update BarberProfile with new plan reference
                var barberProfile = await _context.BarberProfiles
                    .FirstOrDefaultAsync(b => b.UserId == userId);
                
                if (barberProfile != null)
                {
                    barberProfile.SubscriptionPlanId = planId;
                }
                
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public async Task<bool> AssignFreeSubscriptionAsync(string userId)
        {
            try
            {
                var freePlan = await _context.SubscriptionPlans
                    .FirstOrDefaultAsync(p => p.TargetType == SubscriptionTargetType.Barber && p.IsDefault);
                
                if (freePlan == null) return false;
                
                var subscription = new Subscription
                {
                    UserId = userId,
                    SubscriptionPlanId = freePlan.Id,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(freePlan.DurationDays),
                    IsActive = true
                };
                _context.Subscriptions.Add(subscription);
                
                var barberProfile = await _context.BarberProfiles
                    .FirstOrDefaultAsync(b => b.UserId == userId);
                
                if (barberProfile != null)
                {
                    barberProfile.SubscriptionPlanId = freePlan.Id;
                }
                
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public async Task<bool> CancelSubscriptionAsync(string userId)
        {
            try
            {
                var subscriptions = await _context.Subscriptions
                    .Where(s => s.UserId == userId && s.IsActive)
                    .ToListAsync();
                
                foreach (var sub in subscriptions)
                {
                    sub.IsActive = false;
                }
                await _context.SaveChangesAsync();
                return subscriptions.Any();
            }
            catch
            {
                return false;
            }
        }
        
        public async Task<SubscriptionPlan?> GetCurrentPlanAsync(string userId)
        {
            var subscription = await GetActiveSubscriptionAsync(userId);
            return subscription?.SubscriptionPlan;
        }
        
        public async Task<List<SubscriptionPlan>> GetAvailablePlansAsync(SubscriptionTargetType targetType)
        {
            return await _context.SubscriptionPlans
                .Where(p => p.TargetType == targetType && p.IsActive)
                .ToListAsync();
        }
        
        public async Task<bool> HasFeatureAsync(string userId, Func<SubscriptionPlan, bool> featureCheck)
        {
            var plan = await GetCurrentPlanAsync(userId);
            return plan != null && featureCheck(plan);
        }
    }
}
