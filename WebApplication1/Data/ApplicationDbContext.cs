using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<BarberProfile> BarberProfiles { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<BarberAvailability> BarberAvailabilities { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<BarberShopImage> BarberShopImages { get; set; }
        public DbSet<ServiceImage> ServiceImages { get; set; }
        public DbSet<Barbershop> Barbershops { get; set; }
        public DbSet<BarbershopSubscriptionPlan> BarbershopSubscriptionPlans { get; set; }
        public DbSet<BarbershopMembershipRequest> BarbershopMembershipRequests { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        
        // New entities for Ratings, Products, and Banners
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductReservation> ProductReservations { get; set; }
        public DbSet<ProductInventoryMovement> ProductInventoryMovements { get; set; }
        public DbSet<Banner> Banners { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Configure relationships
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.BarberProfile)
                .WithOne(b => b.User)
                .HasForeignKey<BarberProfile>(b => b.UserId);
            
            builder.Entity<Appointment>()
                .HasOne(a => a.Client)
                .WithMany(c => c.ClientAppointments)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Appointment>()
                .HasOne(a => a.BarberProfile)
                .WithMany(b => b.Appointments)
                .HasForeignKey(a => a.BarberProfileId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Service>()
                .HasOne(s => s.BarberProfile)
                .WithMany(b => b.Services)
                .HasForeignKey(s => s.BarberProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<BarberAvailability>()
                .HasOne(ba => ba.BarberProfile)
                .WithMany(b => b.Availability)
                .HasForeignKey(ba => ba.BarberProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<Subscription>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<Subscription>()
                .HasOne(s => s.SubscriptionPlan)
                .WithMany(sp => sp.Subscriptions)
                .HasForeignKey(s => s.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.SetNull);
            
            builder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Image relationships
            builder.Entity<BarberShopImage>()
                .HasOne(i => i.BarberProfile)
                .WithMany()
                .HasForeignKey(i => i.BarberProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<ServiceImage>()
                .HasOne(i => i.Service)
                .WithMany(s => s.Images)
                .HasForeignKey(i => i.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Barbershop relationships
            builder.Entity<Barbershop>()
                .HasOne(b => b.Plan)
                .WithMany(p => p.Barbershops)
                .HasForeignKey(b => b.BarbershopSubscriptionPlanId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Barbershop>()
                .HasOne(b => b.Owner)
                .WithMany()
                .HasForeignKey(b => b.OwnerBarberId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<BarberProfile>()
                .HasOne(b => b.CurrentBarbershop)
                .WithMany(bs => bs.Members)
                .HasForeignKey(b => b.CurrentBarbershopId)
                .OnDelete(DeleteBehavior.SetNull);
            
            builder.Entity<BarbershopMembershipRequest>()
                .HasOne(r => r.Barbershop)
                .WithMany(bs => bs.MembershipRequests)
                .HasForeignKey(r => r.BarbershopId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<BarbershopMembershipRequest>()
                .HasOne(r => r.Barber)
                .WithMany()
                .HasForeignKey(r => r.BarberId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Appointment to Barbershop relationship
            builder.Entity<Appointment>()
                .HasOne(a => a.Barbershop)
                .WithMany(bs => bs.Appointments)
                .HasForeignKey(a => a.BarbershopId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Rating relationships and constraints
            builder.Entity<Rating>()
                .HasOne(r => r.Client)
                .WithMany()
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Rating>()
                .HasOne(r => r.Barber)
                .WithMany()
                .HasForeignKey(r => r.BarberId)
                .OnDelete(DeleteBehavior.SetNull);
            
            builder.Entity<Rating>()
                .HasOne(r => r.Barbershop)
                .WithMany()
                .HasForeignKey(r => r.BarbershopId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Unique constraint: A client can only rate a barber once
            builder.Entity<Rating>()
                .HasIndex(r => new { r.ClientId, r.BarberId })
                .IsUnique()
                .HasFilter("[BarberId] IS NOT NULL");
            
            // Unique constraint: A client can only rate a barbershop once
            builder.Entity<Rating>()
                .HasIndex(r => new { r.ClientId, r.BarbershopId })
                .IsUnique()
                .HasFilter("[BarbershopId] IS NOT NULL");
            
            // Product relationships
            builder.Entity<Product>()
                .HasOne(p => p.Barber)
                .WithMany()
                .HasForeignKey(p => p.BarberId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // ProductReservation relationships
            builder.Entity<ProductReservation>()
                .HasOne(pr => pr.Product)
                .WithMany()
                .HasForeignKey(pr => pr.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<ProductReservation>()
                .HasOne(pr => pr.Client)
                .WithMany()
                .HasForeignKey(pr => pr.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // ProductInventoryMovement relationships
            builder.Entity<ProductInventoryMovement>()
                .HasOne(m => m.Product)
                .WithMany()
                .HasForeignKey(m => m.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Banner relationships
            builder.Entity<Banner>()
                .HasOne(b => b.Barber)
                .WithMany()
                .HasForeignKey(b => b.BarberId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración para ClientNote
            builder.Entity<ClientNote>()
                     .HasOne(cn => cn.BarberProfile)
                    .WithMany(bp => bp.ClientNotes) // Ajusta el nombre de la colección si existe
                    .HasForeignKey(cn => cn.BarberProfileId)
                    .OnDelete(DeleteBehavior.Restrict); // ? Evita la cascada
        }
    }
}
