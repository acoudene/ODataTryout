// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

using Domain.EFCore.DbContexts;
using Domain.EFCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domain.EFCore.Seeding;

public static class DomainDbContextExtensions
{
  public static async Task SeedDataAsync(this DomainDbContext context)
  {
    if (!await context.Products.AnyAsync())
    {
      var electronics = new Category { Id = new Guid("ced4043c-104f-44fd-8ffb-18feee86aef5"), Name = "Electronics", Description = "Electronic devices" };
      var books = new Category { Id = new Guid("3f8b0347-ac89-470c-a24b-93cf58a538eb"), Name = "Books", Description = "Books and literature" };

      context.Categories.AddRange(electronics, books);

      context.Products.AddRange(
          new Product { Id = new Guid("d15bbdf5-084b-45c3-a415-a2fad410718f"), Name = "Laptop", Price = 1200.00m, Stock = 15, CategoryId = electronics.Id },
          new Product { Id = new Guid("9add3ed2-c871-42ea-a3db-347fb77ff4c4"), Name = "Smartphone", Price = 800.00m, Stock = 25, CategoryId = electronics.Id },
          new Product { Id = new Guid("0855d688-6529-4c8c-b55d-9f51fb2e9fb9"), Name = "Clean Code", Price = 45.00m, Stock = 50, CategoryId = books.Id },
          new Product { Id = new Guid("e99e36fe-490d-4228-a745-75679cb993db"), Name = "Design Patterns", Price = 55.00m, Stock = 30, CategoryId = books.Id }
      );

      await context.SaveChangesAsync();
    }

    if (!await context.Orders.AnyAsync())
    {
      var order1 = new Order
      {
        Id = new Guid("a1f5c6e4-0b2f-4c5b-8f3e-5c5f5c5f5c5f"),
        OrderDate = DateTime.UtcNow,
        TotalAmount = 2000.00m,
        Items = new List<OrderItem>
        {
          new OrderItem { ProductId = new Guid("d15bbdf5-084b-45c3-a415-a2fad410718f"), Quantity = 1, UnitPrice = 1200.00m },
          new OrderItem { ProductId = new Guid("9add3ed2-c871-42ea-a3db-347fb77ff4c4"), Quantity = 2, UnitPrice = 800.00m }
        }
      };

      var order2 = new Order
      {
        Id = new Guid("b2e6d7f5-1c3f-4d6c-9f4e-6d6f6d6f6d6f"),
        OrderDate = DateTime.UtcNow,
        TotalAmount = 100.00m,
        Items = new List<OrderItem>
        {
          new OrderItem { ProductId = new Guid("0855d688-6529-4c8c-b55d-9f51fb2e9fb9"), Quantity = 1, UnitPrice = 45.00m },
          new OrderItem { ProductId = new Guid("e99e36fe-490d-4228-a745-75679cb993db"), Quantity = 1, UnitPrice = 55.00m }
        }
      };

      context.Orders.AddRange(order1, order2);
      await context.SaveChangesAsync();
    }
  }
}
