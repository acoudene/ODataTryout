// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

using Microsoft.EntityFrameworkCore;
using Domain.EFCore.DbContexts;
using Domain.EFCore.Entities;

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
    }
}
