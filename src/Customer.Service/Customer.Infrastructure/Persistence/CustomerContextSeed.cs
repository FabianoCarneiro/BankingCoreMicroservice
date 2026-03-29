using Customer.Domain.Entities;
using Customer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Customer.Infrastructure.Persistence;

/// <summary>
/// Seed data para popular o banco de dados com dados de teste
/// </summary>
public class CustomerContextSeed
{
    /// <summary>
    /// Seed inicial com dados de teste
    /// </summary>
    public static async Task SeedAsync(CustomerContext context)
    {
        try
        {
            // Se já houver dados, não adiciona novamente
            if (await context.Customers.AnyAsync())
            {
                return;
            }

            // Criar clientes de teste
            var customers = new List<Domain.Entities.Customer>
            {
                new Domain.Entities.Customer(
                    "11144477735",
                    "João Silva",
                    "joao@example.com",
                    "11999999999"
                ),
                new Domain.Entities.Customer(
                    "12345678901",
                    "Maria Santos",
                    "maria@example.com",
                    "11988888888"
                ),
                new Domain.Entities.Customer(
                    "98765432100",
                    "Pedro Oliveira",
                    "pedro@example.com",
                    "11977777777"
                ),
                new Domain.Entities.Customer(
                    "55555555555",
                    "Ana Costa",
                    "ana@example.com",
                    "11966666666"
                )
            };

            await context.Customers.AddRangeAsync(customers);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Erro ao seed do banco de dados", ex);
        }
    }
}
