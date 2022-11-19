using Microsoft.EntityFrameworkCore;
using Order.Database.Entities;

namespace Order.Database;

public class OrderDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Carts> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Orders> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    public OrderDbContext()
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>()
            .HasMany(x => x.Carts)
            .WithOne(y => y.Customer)
            .HasForeignKey(y => y.CustomerId);

        modelBuilder.Entity<Customer>()
            .HasMany(x => x.Orders)
            .WithOne(y => y.Customer)
            .HasForeignKey(y => y.CustomerId);

        modelBuilder.Entity<Carts>()
            .HasMany(x => x.Items)
            .WithOne(y => y.Carts)
            .HasForeignKey(y => y.CartsId);

        modelBuilder.Entity<Orders>()
            .HasMany(x => x.Items)
            .WithOne(y => y.Orders)
            .HasForeignKey(y => y.OrdersId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=ordersdb;Trusted_Connection=True;");
    }
}