using Delivery.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Database;

public sealed class DeliveryDbContext : DbContext
{
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Courier> Couriers { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<OrderDelivery> Deliveries { get; set; }
    public DbSet<DeliveryItem> Items { get; set; }

    public DeliveryDbContext(DbContextOptions<DeliveryDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasMany(x => x.Addresses)
            .WithOne(y => y.Customer)
            .HasForeignKey(y => y.CustomerId);

        modelBuilder.Entity<Courier>()
            .HasMany(x => x.Deliveries)
            .WithOne(y => y.Courier)
            .HasForeignKey(y => y.CourierId);

        modelBuilder.Entity<OrderDelivery>()
            .HasMany(x => x.Items)
            .WithOne(y => y.Delivery)
            .HasForeignKey(y => y.DeliveryId);

        modelBuilder.Entity<OrderDelivery>()
            .HasOne(x => x.Address)
            .WithMany(y => y.Deliveries)
            .HasForeignKey(x => x.AddressId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=deliverydb;Trusted_Connection=True;",
                builder =>
                {
                    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
    }
}