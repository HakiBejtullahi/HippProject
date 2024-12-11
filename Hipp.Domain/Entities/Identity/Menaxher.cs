namespace Hipp.Domain.Entities.Identity;

public class Menaxher : BaseUserRole
{
    public int CompletedTasksCount { get; set; }
    public virtual ICollection<Products.Product> Products { get; set; } = new List<Products.Product>();
} 