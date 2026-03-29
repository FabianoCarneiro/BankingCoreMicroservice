using Customer.Domain.Entities;
using Customer.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Customer.Infrastructure.Persistence;

/// <summary>
/// DbContext do Entity Framework Core para o Customer Service
/// </summary>
public class CustomerContext : DbContext
{
    public DbSet<Domain.Entities.Customer> Customers { get; set; }

    public CustomerContext(DbContextOptions<CustomerContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração de Customer
        modelBuilder.Entity<Domain.Entities.Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CPF)
                .HasConversion(new ValueConverter<CPF, string>(
                    v => v.Value,
                    v => new CPF(v)))
                .IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.PhoneNumber).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();

            // Index para CPF (único)
            entity.HasIndex(e => e.CPF).IsUnique();
        });
    }
}
