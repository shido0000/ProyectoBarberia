using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    /// <summary>
    /// Producto publicado por un barbero Premium para que los clientes lo reserven.
    /// </summary>
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Barbero")]
        public int BarberId { get; set; }

        [Display(Name = "Barbero")]
        public virtual BarberProfile Barber { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        [Display(Name = "Nombre del Producto")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Precio")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Stock Disponible")]
        public int Stock { get; set; }

        [MaxLength(500)]
        [Display(Name = "Imagen")]
        public string? ImageUrl { get; set; }

        [MaxLength(100)]
        [Display(Name = "Categoría")]
        public string? Category { get; set; }

        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Reserva de un producto realizada por un cliente.
    /// </summary>
    public class ProductReservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Display(Name = "Producto")]
        public virtual Product Product { get; set; } = null!;

        [Required]
        public string ClientId { get; set; } = string.Empty;

        [Display(Name = "Cliente")]
        public virtual ApplicationUser Client { get; set; } = null!;

        [Required]
        [Display(Name = "Cantidad")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Fecha de Reserva")]
        public DateTime ReservationDate { get; set; } = DateTime.Now;

        [Display(Name = "Fecha de Recogida")]
        public DateTime? PickupDate { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "Estado")]
        public string Status { get; set; } = ProductReservationStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? DeliveredAt { get; set; }

        public DateTime? CancelledAt { get; set; }
    }

    /// <summary>
    /// Movimiento de inventario de un producto (entradas/salidas).
    /// </summary>
    public class ProductInventoryMovement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Display(Name = "Producto")]
        public virtual Product Product { get; set; } = null!;

        [Required]
        [Display(Name = "Cantidad")]
        public int Quantity { get; set; } // Positiva para entradas, negativa para salidas

        [Required]
        [MaxLength(50)]
        [Display(Name = "Tipo de Movimiento")]
        public string Type { get; set; } = string.Empty; // "StockEntry", "Reservation", "Sale", "Adjustment"

        [MaxLength(500)]
        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Estados posibles de una reserva de producto.
    /// </summary>
    public static class ProductReservationStatus
    {
        public const string Pending = "Pending";      // Pendiente de confirmación
        public const string Confirmed = "Confirmed";  // Confirmada por el barbero
        public const string Delivered = "Delivered";  // Entregada al cliente
        public const string Cancelled = "Cancelled";  // Cancelada
    }
}
