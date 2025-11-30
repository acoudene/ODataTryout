// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

using OtherDomain.EFCore.DbContexts;
using OtherDomain.EFCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace OtherDomain.EFCore.Seeding;

public static class OtherDomainDbContextExtensions
{
  public static async Task SeedDataAsync(this OtherDomainDbContext context)
  {
    if (!await context.SuperHeroes.AnyAsync())
    {
      context.SuperHeroes.AddRange(
          new SuperHero { Id = new Guid("d15bbdf5-084b-45c3-a415-a2fad410718f"), Name = "Superman" },
          new SuperHero { Id = new Guid("9add3ed2-c871-42ea-a3db-347fb77ff4c4"), Name = "Batman" },
          new SuperHero { Id = new Guid("0855d688-6529-4c8c-b55d-9f51fb2e9fb9"), Name = "Wonder Woman" },
          new SuperHero { Id = new Guid("e99e36fe-490d-4228-a745-75679cb993db"), Name = "Flash" }
      );

      await context.SaveChangesAsync();
    }
  }
}
