// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

namespace Domain.EFCore.Entities;

public class Category
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public ICollection<Product> Products { get; set; } = new List<Product>();
}

