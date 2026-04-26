using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            
            // Create roles if they don't exist
            string[] roles = { "Admin", "Barber", "Client" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            
            // Create default subscription plans for BARBERS
            if (!context.SubscriptionPlans.Any())
            {
                var freeBarberPlan = new SubscriptionPlan
                {
                    Name = "Free Barber",
                    Description = "Plan gratuito para barberos independientes. Solo perfil y servicios visibles, sin agenda ni citas.",
                    MonthlyPrice = 0,
                    AnnualPrice = 0,
                    DurationDays = 36500, // Practically unlimited
                    MaxBarbers = 1,
                    MaxServicesPerBarber = 5,
                    IncludeAnalytics = false,
                    PrioritySupport = false,
                    CustomBranding = false,
                    AutoReminders = false,
                    ExportReports = false,
                    IsActive = true,
                    IsDefault = true,
                    TargetType = SubscriptionTargetType.Barber,
                    CanReceiveBookings = false,
                    CanAccessAnalytics = false,
                    CanAccessAccounting = false,
                    CanAccessInventory = false,
                    CanPostProducts = false,
                    CanUseBanners = false
                };
                
                var popularBarberPlan = new SubscriptionPlan
                {
                    Name = "Popular Barber",
                    Description = "Plan popular para barberos: recibe reservas, análisis detallado y contabilidad.",
                    MonthlyPrice = 19.99m,
                    AnnualPrice = 199.99m,
                    DurationDays = 30,
                    MaxBarbers = 1,
                    MaxServicesPerBarber = 20,
                    IncludeAnalytics = true,
                    PrioritySupport = true,
                    CustomBranding = false,
                    AutoReminders = true,
                    ExportReports = true,
                    IsActive = true,
                    IsDefault = false,
                    TargetType = SubscriptionTargetType.Barber,
                    CanReceiveBookings = true,
                    CanAccessAnalytics = true,
                    CanAccessAccounting = true,
                    CanAccessInventory = false,
                    CanPostProducts = false,
                    CanUseBanners = false
                };
                
                var premiumBarberPlan = new SubscriptionPlan
                {
                    Name = "Premium Barber",
                    Description = "Plan premium con acceso total: productos en venta, banners, inventario y todas las funcionalidades.",
                    MonthlyPrice = 49.99m,
                    AnnualPrice = 499.99m,
                    DurationDays = 30,
                    MaxBarbers = null, // Unlimited for independent barber
                    MaxServicesPerBarber = null,
                    IncludeAnalytics = true,
                    PrioritySupport = true,
                    CustomBranding = true,
                    AutoReminders = true,
                    ExportReports = true,
                    IsActive = true,
                    IsDefault = false,
                    TargetType = SubscriptionTargetType.Barber,
                    CanReceiveBookings = true,
                    CanAccessAnalytics = true,
                    CanAccessAccounting = true,
                    CanAccessInventory = true,
                    CanPostProducts = true,
                    CanUseBanners = true
                };
                
                // Create default subscription plans for BARBERSHOPS
                var basicBarbershopPlan = new SubscriptionPlan
                {
                    Name = "Basic Barbershop",
                    Description = "Plan básico para barberías pequeñas. Hasta 3 barberos.",
                    MonthlyPrice = 59.99m,
                    AnnualPrice = 599.99m,
                    DurationDays = 30,
                    MaxBarbers = 3,
                    MaxServicesPerBarber = null,
                    IncludeAnalytics = true,
                    PrioritySupport = false,
                    CustomBranding = false,
                    AutoReminders = true,
                    ExportReports = false,
                    IsActive = true,
                    IsDefault = false,
                    TargetType = SubscriptionTargetType.Barbershop,
                    CanReceiveBookings = true,
                    CanAccessAnalytics = true,
                    CanAccessAccounting = true,
                    CanAccessInventory = false,
                    CanPostProducts = false,
                    CanUseBanners = false
                };
                
                var proBarbershopPlan = new SubscriptionPlan
                {
                    Name = "Pro Barbershop",
                    Description = "Plan profesional para barberías en crecimiento. Hasta 10 barberos.",
                    MonthlyPrice = 129.99m,
                    AnnualPrice = 1299.99m,
                    DurationDays = 30,
                    MaxBarbers = 10,
                    MaxServicesPerBarber = null,
                    IncludeAnalytics = true,
                    PrioritySupport = true,
                    CustomBranding = true,
                    AutoReminders = true,
                    ExportReports = true,
                    IsActive = true,
                    IsDefault = false,
                    TargetType = SubscriptionTargetType.Barbershop,
                    CanReceiveBookings = true,
                    CanAccessAnalytics = true,
                    CanAccessAccounting = true,
                    CanAccessInventory = true,
                    CanPostProducts = true,
                    CanUseBanners = true
                };
                
                var enterpriseBarbershopPlan = new SubscriptionPlan
                {
                    Name = "Enterprise Barbershop",
                    Description = "Solución completa para cadenas de barberías. Barberos ilimitados.",
                    MonthlyPrice = 299.99m,
                    AnnualPrice = 2999.99m,
                    DurationDays = 30,
                    MaxBarbers = null,
                    MaxServicesPerBarber = null,
                    IncludeAnalytics = true,
                    PrioritySupport = true,
                    CustomBranding = true,
                    AutoReminders = true,
                    ExportReports = true,
                    IsActive = true,
                    IsDefault = false,
                    TargetType = SubscriptionTargetType.Barbershop,
                    CanReceiveBookings = true,
                    CanAccessAnalytics = true,
                    CanAccessAccounting = true,
                    CanAccessInventory = true,
                    CanPostProducts = true,
                    CanUseBanners = true
                };
                
                context.SubscriptionPlans.AddRange(
                    freeBarberPlan, 
                    popularBarberPlan, 
                    premiumBarberPlan,
                    basicBarbershopPlan,
                    proBarbershopPlan,
                    enterpriseBarbershopPlan);
                await context.SaveChangesAsync();
            }
            
            // Create admin user
            var adminEmail = "admin@barbershop.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Administrador",
                    Role = "Admin",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
            
            // Create sample barber
            var barberEmail = "barber@barbershop.com";
            var barberUser = await userManager.FindByEmailAsync(barberEmail);
            if (barberUser == null)
            {
                barberUser = new ApplicationUser
                {
                    UserName = barberEmail,
                    Email = barberEmail,
                    FullName = "Carlos Barber",
                    Role = "Barber",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(barberUser, "Barber123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(barberUser, "Barber");
                    
                    // Get Free Barber plan
                    var freePlan = await context.SubscriptionPlans
                        .FirstOrDefaultAsync(p => p.TargetType == SubscriptionTargetType.Barber && p.IsDefault);
                    
                    // Create barber profile with FREE subscription
                    var barberProfile = new BarberProfile
                    {
                        UserId = barberUser.Id,
                        ShopName = "Barbería Elite",
                        Description = "Barbería profesional con los mejores servicios",
                        Address = "Calle Principal #123, Ciudad",
                        Phone = "+1234567890",
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(18, 0, 0),
                        IsActive = true,
                        SubscriptionPlanId = freePlan?.Id
                    };
                    context.BarberProfiles.Add(barberProfile);
                    await context.SaveChangesAsync();
                    
                    // Create subscription record for the barber
                    if (freePlan != null)
                    {
                        context.Subscriptions.Add(new Subscription
                        {
                            UserId = barberUser.Id,
                            SubscriptionPlanId = freePlan.Id,
                            StartDate = DateTime.Now,
                            EndDate = DateTime.Now.AddDays(freePlan.DurationDays),
                            IsActive = true
                        });
                    }
                    
                    // Create services for this barber
                    var services = new List<Service>
                    {
                        new Service
                        {
                            Name = "Corte de Cabello",
                            Description = "Corte clásico o moderno con acabado profesional",
                            Price = 15.00m,
                            DurationMinutes = 30,
                            BarberProfileId = barberProfile.Id
                        },
                        new Service
                        {
                            Name = "Corte + Barba",
                            Description = "Corte de cabello completo con arreglo de barba",
                            Price = 25.00m,
                            DurationMinutes = 45,
                            BarberProfileId = barberProfile.Id
                        },
                        new Service
                        {
                            Name = "Barba Completa",
                            Description = "Afeitado y diseño de barba profesional",
                            Price = 10.00m,
                            DurationMinutes = 20,
                            BarberProfileId = barberProfile.Id
                        },
                        new Service
                        {
                            Name = "Tratamiento Capilar",
                            Description = "Lavado, masaje y tratamiento para el cabello",
                            Price = 20.00m,
                            DurationMinutes = 40,
                            BarberProfileId = barberProfile.Id
                        }
                    };
                    context.Services.AddRange(services);
                    
                    // Create availability
                    var availability = new List<BarberAvailability>();
                    foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                    {
                        availability.Add(new BarberAvailability
                        {
                            BarberProfileId = barberProfile.Id,
                            DayOfWeek = day,
                            StartTime = new TimeSpan(9, 0, 0),
                            EndTime = new TimeSpan(18, 0, 0),
                            IsAvailable = true
                        });
                    }
                    context.BarberAvailabilities.AddRange(availability);
                    
                    await context.SaveChangesAsync();
                }
            }
            
            // Create sample client
            var clientEmail = "cliente@barbershop.com";
            var clientUser = await userManager.FindByEmailAsync(clientEmail);
            if (clientUser == null)
            {
                clientUser = new ApplicationUser
                {
                    UserName = clientEmail,
                    Email = clientEmail,
                    FullName = "Juan Cliente",
                    Role = "Client",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(clientUser, "Client123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(clientUser, "Client");
                }
            }
            
            await context.SaveChangesAsync();
        }
    }
}
