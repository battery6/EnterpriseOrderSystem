namespace OrderSystem.Infrastructure.Persistence;

using OrderSystem.Domain.Entities;
using OrderSystem.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

public sealed class OrderSystemDbContext(
    DbContextOptions<OrderSystemDbContext> options) 
    : DbContext(options), IUnitOfWork
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(customer => customer.Name)
                .HasMaxLength(Customer.MaxNameLength)
                .IsRequired();
            entity.Property(customer => customer.Email)
                .HasMaxLength(Customer.MaxEmailLength)
                .IsRequired();
            entity.HasIndex(customer => customer.Email)
                .IsUnique();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(product => product.Name)
                .HasMaxLength(Product.MaxNameLength)
                .IsRequired();
            entity.Property(product => product.CurrentPrice)
                .HasPrecision(18, 2)
                .IsRequired();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasOne(order => order.Customer)
                .WithMany()
                .HasForeignKey("CustomerId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasMany<OrderItem>("Items")
                .WithOne()
                .HasForeignKey("OrderId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(item => item.Id)
                .ValueGeneratedNever();
            entity.HasOne(item => item.Product)
            .WithMany()
            .HasForeignKey("ProductId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
            entity.Property(item => item.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();
        });
    }
}