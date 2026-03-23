using Core.Domain.Entities;
using Core.Domain.Ports;
using Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Adapters;

/// <summary>
/// Adaptador de persistência para BankAccount
/// Implementa a porta IBankAccountRepository
/// </summary>
public class BankAccountRepository : IBankAccountRepository
{
    private readonly BankingContext _context;

    public BankAccountRepository(BankingContext context)
    {
        _context = context;
    }

    public async Task<BankAccount?> GetByIdAsync(Guid id)
    {
        return await _context.BankAccounts
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<BankAccount?> GetByAccountNumberAsync(string accountNumber)
    {
        return await _context.BankAccounts
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
    }

    public async Task<IEnumerable<BankAccount>> GetByCustomerIdAsync(Guid customerId)
    {
        return await _context.BankAccounts
            .Include(a => a.Transactions)
            .Where(a => a.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task AddAsync(BankAccount account)
    {
        await _context.BankAccounts.AddAsync(account);
    }

    public async Task UpdateAsync(BankAccount account)
    {
        _context.BankAccounts.Update(account);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
