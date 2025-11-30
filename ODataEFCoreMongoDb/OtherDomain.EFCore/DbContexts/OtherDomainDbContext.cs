// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

using OtherDomain.EFCore.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace OtherDomain.EFCore.DbContexts;

public class OtherDomainDbContext : DbContext
{
  public OtherDomainDbContext(DbContextOptions<OtherDomainDbContext> options) : base(options) { }

  public DbSet<SuperHero> SuperHeroes => Set<SuperHero>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Configuration pour MongoDB
    modelBuilder.Entity<SuperHero>()
        .ToCollection("superHeros")
        .HasKey(p => p.Id);

    // Relations

  }
}