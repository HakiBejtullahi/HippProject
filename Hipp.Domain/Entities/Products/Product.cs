using System.ComponentModel.DataAnnotations;
using Hipp.Domain.Entities.Identity;

namespace Hipp.Domain.Entities.Products;

public class Product
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    
    [Required]
    [MaxLength(50)]
    public string ProductCode { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    
    [MaxLength(500)]
    public string Description { get; set; }
    
    [MaxLength(1000)]
    public string Ingredients { get; set; }
    
    // Cloudinary image properties
    public string? ImageUrl { get; set; }
    public string? ImagePublicId { get; set; }
    
    [Required]
    public decimal TotalQuantity { get; set; }
    
    [Required]
    public decimal UnlabeledQuantity { get; set; }
    
    [Required]
    public decimal LabeledQuantity { get; set; }
    
    public bool IsPriority { get; set; }
    
    [Required]
    public string MenaxherId { get; set; }
    public virtual Menaxher Menaxher { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
} 