using Core.Domain.Entities;
using Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Core.Infrastructure.Persistence;

/// <summary>
/// DbContext do Entity Framework Core para persistência
/// Implementa o adaptador de banco de dados
/// </summary>
public class BankingContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<BankAccount> BankAccounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public BankingContext(DbContextOptions<BankingContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração de Customer
        modelBuilder.Entity<Customer>(entity =>
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
        });

        // Configuração de BankAccount
        modelBuilder.Entity<BankAccount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AccountNumber).IsRequired();
            entity.Property(e => e.Balance)
                .HasConversion(new ValueConverter<Money, string>(
                    v => $"{v.Amount},{v.Currency}",
                    v => ParseMoney(v)))
                .IsRequired();
            entity.HasMany(e => e.Transactions)
                .WithOne()
                .HasForeignKey(t => t.AccountId);
        });

        // Configuração de Transaction
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Amount)
                .HasConversion(new ValueConverter<Money, string>(
                    v => $"{v.Amount},{v.Currency}",
                    v => ParseMoney(v)))
                .IsRequired();
            entity.Property(e => e.Description).IsRequired();
        });
    }

    private static Money ParseMoney(string value)
    {
        var parts = value.Split(',');
        return new Money(decimal.Parse(parts[0]), parts[1]);
    }
}
