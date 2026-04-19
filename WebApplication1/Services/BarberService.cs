using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class BarberService
    {
        private readonly ApplicationDbContext _context;
        
        public BarberService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<BarberProfile>> GetAllActiveBarbersAsync()
        {
            return await _context.BarberProfiles
                .Include(b => b.User)
                .Include(b => b.Images)
                .Include(b => b.Services.Where(s => s.IsActive))
                    .ThenInclude(s => s.Images)
                .Where(b => b.IsActive)
                .ToListAsync();
        }

        public async Task<BarberProfile?> GetBarberByIdAsync(int id)
        {
            return await _context.BarberProfiles
                .Include(b => b.User)
                .Include(b => b.Images)
                .Include(b => b.Services.Where(s => s.IsActive))
                    .ThenInclude(s => s.Images)
                .Include(b => b.Availability)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
        
        public async Task<List<Service>> GetServicesByBarberAsync(int barberProfileId)
        {
            return await _context.Services
                .Include(s => s.Images)
                .Where(s => s.BarberProfileId == barberProfileId && s.IsActive)
                .ToListAsync();
        }
        
        public async Task<bool> CreateServiceAsync(Service service)
        {
            try
            {
                _context.Services.Add(service);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public async Task<bool> UpdateServiceAsync(Service service)
        {
            try
            {
                _context.Services.Update(service);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public async Task<bool> DeleteServiceAsync(int serviceId)
        {
            try
            {
                var service = await _context.Services.FindAsync(serviceId);
                if (service != null)
                {
                    service.IsActive = false;
                    _context.Services.Update(service);
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
    }
}
