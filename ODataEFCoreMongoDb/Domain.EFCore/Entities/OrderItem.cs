// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

namespace Domain.EFCore.Entities;

public class OrderItem
{
  public Guid ProductId { get; set; }
  public string ProductName { get; set; } = string.Empty;
  public int Quantity { get; set; }
  public decimal UnitPrice { get; set; }
}
