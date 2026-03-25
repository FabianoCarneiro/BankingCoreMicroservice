using Core.Domain.Entities;
using Core.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Persistence;

/// <summary>
/// Seed data para popular o banco de dados com dados de teste
/// </summary>
public class BankingContextSeed
{
    /// <summary>
    /// Seed inicial com dados de teste
    /// </summary>
    public static async Task SeedAsync(BankingContext context)
    {
        try
        {
            // Aplicar migrations pendentes
            await context.Database.MigrateAsync();

            // Se já houver dados, não adiciona novamente
            if (await context.Customers.AnyAsync())
            {
                return;
            }

            // Criar clientes de teste
            // Usando CPFs válidos para teste
            var customers = new List<Customer>
            {
                new Customer(
                    GenerateValidCPF(),
                    "João Silva",
                    "joao@example.com",
                    "11999999999"
                ),
                new Customer(
                    GenerateValidCPF(),
                    "Maria Santos",
                    "maria@example.com",
                    "11988888888"
                ),
                new Customer(
                    GenerateValidCPF(),
                    "Pedro Oliveira",
                    "pedro@example.com",
                    "11977777777"
                ),
                new Customer(
                    GenerateValidCPF(),
                    "Ana Costa",
                    "ana@example.com",
                    "11966666666"
                )
            };

            await context.Customers.AddRangeAsync(customers);
            await context.SaveChangesAsync();

            // Criar contas bancárias para os clientes
            var bankAccounts = new List<BankAccount>();

            foreach (var customer in customers)
            {
                var account = new BankAccount(customer.Id);
                bankAccounts.Add(account);
            }

            await context.BankAccounts.AddRangeAsync(bankAccounts);
            await context.SaveChangesAsync();

            // Criar transações de exemplo
            if (bankAccounts.Count >= 2)
            {
                var transaction = new Transaction(
                    TransactionType.Transfer,
                    new Money(100.00m, "BRL"),
                    "Transferência de teste",
                    bankAccounts[0].Id
                );

                await context.Transactions.AddAsync(transaction);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao seed do banco de dados", ex);
        }
    }

    private static string GenerateAccountNumber()
    {
        return $"{Random.Shared.Next(100000, 999999)}-{Random.Shared.Next(0, 99)}";
    }

    /// <summary>
    /// Gera um CPF válido aleatoriamente para testes
    /// </summary>
    private static string GenerateValidCPF()
    {
        var random = Random.Shared;
        var digits = new int[11];

        // Gerar primeiros 9 dígitos aleatoriamente
        for (int i = 0; i < 9; i++)
        {
            digits[i] = random.Next(0, 10);
        }

        // Calcular primeiro dígito verificador
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            sum += digits[i] * (10 - i);
        }

        int firstDigit = 11 - (sum % 11);
        digits[9] = firstDigit > 9 ? 0 : firstDigit;

        // Calcular segundo dígito verificador
        sum = 0;
        for (int i = 0; i < 10; i++)
        {
            sum += digits[i] * (11 - i);
        }

        int secondDigit = 11 - (sum % 11);
        digits[10] = secondDigit > 9 ? 0 : secondDigit;

        return string.Concat(digits);
    }
}
