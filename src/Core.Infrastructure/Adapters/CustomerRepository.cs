using Core.Domain.Entities;
using Core.Domain.Ports;
using Core.Domain.ValueObjects;
using Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Adapters;

/// <summary>
/// Adaptador de persistência para Customer
/// Implementa a porta ICustomerRepository
/// </summary>
public class CustomerRepository : ICustomerRepository
{
    private readonly BankingContext _context;

    public CustomerRepository(BankingContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Customer?> GetByCPFAsync(CPF cpf)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.CPF == cpf);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _context.Customers.ToListAsync();
    }

    public async Task AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
    }

    public async Task UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var customer = await GetByIdAsync(id);
        if (customer != null)
            _context.Customers.Remove(customer);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
