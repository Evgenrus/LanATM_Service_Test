using Catalog.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Database;

public sealed class CatalogDbContext : DbContext
{
    public DbSet<Item> Items { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Category> Categories { get; set; }

    public CatalogDbContext()
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Brand>().HasMany(x => x.Items)
            .WithOne(y => y.Brand)
            .HasForeignKey(y => y.BrandId);

        modelBuilder.Entity<Category>().HasMany(x => x.Items)
            .WithOne(y => y.Category)
            .HasForeignKey(y => y.CategoryId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=catalogdb;Trusted_Connection=True;");
    }
}