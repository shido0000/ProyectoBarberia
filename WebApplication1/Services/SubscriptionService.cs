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
                
                var existingSubscription = await GetActiveSubscriptionAsync(userId);
                
                if (existingSubscription != null)
                {
                    // Upgrade existing subscription
                    existingSubscription.SubscriptionPlanId = planId;
                    existingSubscription.EndDate = DateTime.Now.AddDays(plan.DurationDays);
                }
                else
                {
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
                var subscription = await GetActiveSubscriptionAsync(userId);
                if (subscription != null)
                {
                    subscription.IsActive = false;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
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
    }
}
