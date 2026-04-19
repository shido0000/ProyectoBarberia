using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    /// <summary>
    /// Representa un producto o insumo del inventario de la barbería (Premium).
    /// </summary>
    public class InventoryItem
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int BarberProfileId { get; set; }
        
        [Required]
        [MaxLength(100)]
        [Display(Name = "Nombre del Producto")]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        [Display(Name = "Descripción")]
        public string? Description { get; set; }
        
        [Required]
        [Display(Name = "Stock Actual")]
        public int CurrentStock { get; set; } = 0;
        
        [Display(Name = "Stock Mínimo")]
        public int MinStock { get; set; } = 5;
        
        [Display(Name = "Precio de Venta")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }
        
        [Display(Name = "Proveedor")]
        [MaxLength(200)]
        public string? Supplier { get; set; }
        
        [Display(Name = "Teléfono Proveedor")]
        [MaxLength(20)]
        public string? SupplierPhone { get; set; }
        
        [Display(Name = "Activo")]
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation property
        public virtual BarberProfile BarberProfile { get; set; } = null!;
    }
}
