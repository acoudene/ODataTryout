// Changelogs Date  | Author                | Description
// 2023-12-23       | Anthony Coudène       | Creation

using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using Domain.EFCore.Entities;

namespace Domain.EFCore.DbContexts;

public class DomainDbContext : DbContext
{
    public DomainDbContext(DbContextOptions<DomainDbContext> options) : base(options) { }

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