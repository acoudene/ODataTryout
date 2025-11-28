// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

namespace Domain.EFCore.Entities;

public class Order
{
  public Guid Id { get; set; }
  public string CustomerName { get; set; } = string.Empty;
  public DateTime OrderDate { get; set; } = DateTime.UtcNow;
  public decimal TotalAmount { get; set; }
  public List<OrderItem> Items { get; set; } = new();
}