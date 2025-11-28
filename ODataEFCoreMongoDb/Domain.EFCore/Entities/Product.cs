// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

namespace Domain.EFCore.Entities;

public class Product
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public decimal Price { get; set; }
  public int Stock { get; set; }
  public Guid CategoryId { get; set; }
  public Category? Category { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
