using Microsoft.AspNetCore.Identity;
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
            
            // Create default subscription plans
            if (!context.SubscriptionPlans.Any())
            {
                var freePlan = new SubscriptionPlan
                {
                    Name = "Gratis",
                    Description = "Plan básico para empezar",
                    MonthlyPrice = 0,
                    AnnualPrice = 0,
                    DurationDays = 30,
                    MaxBarbers = 1,
                    MaxServicesPerBarber = 5,
                    IncludeAnalytics = false,
                    PrioritySupport = false,
                    CustomBranding = false,
                    AutoReminders = false,
                    ExportReports = false,
                    IsActive = true,
                    IsDefault = true
                };
                
                var premiumPlan = new SubscriptionPlan
                {
                    Name = "Premium",
                    Description = "Para barberos profesionales que quieren crecer",
                    MonthlyPrice = 29.99m,
                    AnnualPrice = 299.99m,
                    DurationDays = 30,
                    MaxBarbers = 5,
                    MaxServicesPerBarber = 20,
                    IncludeAnalytics = true,
                    PrioritySupport = true,
                    CustomBranding = false,
                    AutoReminders = true,
                    ExportReports = true,
                    IsActive = true,
                    IsDefault = false
                };
                
                var enterprisePlan = new SubscriptionPlan
                {
                    Name = "Enterprise",
                    Description = "Solución completa para negocios grandes",
                    MonthlyPrice = 99.99m,
                    AnnualPrice = 999.99m,
                    DurationDays = 30,
                    MaxBarbers = null, // Unlimited
                    MaxServicesPerBarber = null, // Unlimited
                    IncludeAnalytics = true,
                    PrioritySupport = true,
                    CustomBranding = true,
                    AutoReminders = true,
                    ExportReports = true,
                    IsActive = true,
                    IsDefault = false
                };
                
                context.SubscriptionPlans.AddRange(freePlan, premiumPlan, enterprisePlan);
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
                    
                    // Create barber profile
                    var barberProfile = new BarberProfile
                    {
                        UserId = barberUser.Id,
                        ShopName = "Barbería Elite",
                        Description = "Barbería profesional con los mejores servicios",
                        Address = "Calle Principal #123, Ciudad",
                        Phone = "+1234567890",
                        StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(18, 0, 0),
                        IsActive = true
                    };
                    context.BarberProfiles.Add(barberProfile);
                    await context.SaveChangesAsync();
                    
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
