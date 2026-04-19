using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class ImageService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        
        public ImageService(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        
        // Barber Shop Images
        public async Task<List<BarberShopImage>> GetBarberShopImagesAsync(int barberProfileId)
        {
            return await _context.BarberShopImages
                .Where(i => i.BarberProfileId == barberProfileId)
                .OrderBy(i => i.DisplayOrder)
                .ToListAsync();
        }
        
        public async Task<BarberShopImage?> UploadBarberShopImageAsync(int barberProfileId, IFormFile file, string? description = null)
        {
            if (file == null || file.Length == 0)
                return null;
            
            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "barbershops");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }
            
            // Generate unique filename
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);
            
            // Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            
            // Create image record
            var image = new BarberShopImage
            {
                BarberProfileId = barberProfileId,
                ImageUrl = $"/uploads/barbershops/{uniqueFileName}",
                Description = description,
                IsMain = false,
                DisplayOrder = await _context.BarberShopImages.CountAsync(i => i.BarberProfileId == barberProfileId)
            };
            
            _context.BarberShopImages.Add(image);
            await _context.SaveChangesAsync();
            
            return image;
        }
        
        public async Task<bool> DeleteBarberShopImageAsync(int imageId)
        {
            var image = await _context.BarberShopImages.FindAsync(imageId);
            if (image == null)
                return false;
            
            // Delete physical file
            var filePath = Path.Combine(_environment.WebRootPath, image.ImageUrl.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            
            _context.BarberShopImages.Remove(image);
            await _context.SaveChangesAsync();
            
            return true;
        }
        
        public async Task<bool> SetMainImageAsync(int imageId)
        {
            var image = await _context.BarberShopImages.FindAsync(imageId);
            if (image == null)
                return false;
            
            // Unset all other main images for this barber
            var allImages = await _context.BarberShopImages
                .Where(i => i.BarberProfileId == image.BarberProfileId)
                .ToListAsync();
            
            foreach (var img in allImages)
            {
                img.IsMain = (img.Id == imageId);
            }
            
            await _context.SaveChangesAsync();
            return true;
        }
        
        // Service Images
        public async Task<List<ServiceImage>> GetServiceImagesAsync(int serviceId)
        {
            return await _context.ServiceImages
                .Where(i => i.ServiceId == serviceId)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }
        
        public async Task<ServiceImage?> UploadServiceImageAsync(int serviceId, IFormFile file, string? description = null)
        {
            if (file == null || file.Length == 0)
                return null;
            
            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "services");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }
            
            // Generate unique filename
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);
            
            // Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            
            // Create image record
            var image = new ServiceImage
            {
                ServiceId = serviceId,
                ImageUrl = $"/uploads/services/{uniqueFileName}",
                Description = description
            };
            
            _context.ServiceImages.Add(image);
            await _context.SaveChangesAsync();
            
            return image;
        }
        
        public async Task<bool> DeleteServiceImageAsync(int imageId)
        {
            var image = await _context.ServiceImages.FindAsync(imageId);
            if (image == null)
                return false;
            
            // Delete physical file
            var filePath = Path.Combine(_environment.WebRootPath, image.ImageUrl.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            
            _context.ServiceImages.Remove(image);
            await _context.SaveChangesAsync();
            
            return true;
        }
    }
}
