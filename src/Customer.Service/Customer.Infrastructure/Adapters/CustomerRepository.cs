using Customer.Domain.Entities;
using Customer.Domain.Ports;
using Customer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Customer.Infrastructure.Adapters;

/// <summary>
/// Adaptador que implementa o repositório de Customer
/// </summary>
public class CustomerRepository : ICustomerRepository
{
    private readonly CustomerContext _context;

    public CustomerRepository(CustomerContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.Customer?> GetByIdAsync(Guid id)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Domain.Entities.Customer?> GetByCpfAsync(string cpf)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.CPF.Value == cpf);
    }

    public async Task<IEnumerable<Domain.Entities.Customer>> GetAllAsync()
    {
        return await _context.Customers.ToListAsync();
    }

    public async Task AddAsync(Domain.Entities.Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Domain.Entities.Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var customer = await GetByIdAsync(id);
        if (customer != null)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Customers.AnyAsync(c => c.Id == id);
    }
}
