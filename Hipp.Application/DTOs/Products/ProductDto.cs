using System.ComponentModel.DataAnnotations;

namespace Hipp.Application.DTOs.Products;

public class ProductDto
{
    public string Id { get; set; }
    public string ProductCode { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public int TotalQuantity { get; set; }
    public int UnlabeledQuantity { get; set; }
    public int LabeledQuantity { get; set; }
    public bool IsPriority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<string> IngredientIds { get; set; } = new();
} 