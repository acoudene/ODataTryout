using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using MyApi.Entities;

namespace MyApi.DbContexts;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

  public DbSet<Product> Products => Set<Product>();
  public DbSet<Category> Categories => Set<Category>();
  public DbSet<Order> Orders => Set<Order>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Configuration pour MongoDB
    modelBuilder.Entity<Product>()
        .ToCollection("products")
        .HasKey(p => p.Id);

    modelBuilder.Entity<Category>()
        .ToCollection("categories")
        .HasKey(c => c.Id);

    modelBuilder.Entity<Order>()
        .ToCollection("orders")
        .HasKey(o => o.Id);

    // Relations
    modelBuilder.Entity<Product>()
        .HasOne(p => p.Category)
        .WithMany(c => c.Products)
        .HasForeignKey(p => p.CategoryId);
  }
}