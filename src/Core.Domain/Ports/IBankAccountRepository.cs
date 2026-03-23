using Core.Domain.Entities;

namespace Core.Domain.Ports;

/// <summary>
/// Porto para persistência de contas bancárias
/// </summary>
public interface IBankAccountRepository
{
    Task<BankAccount?> GetByIdAsync(Guid id);
    Task<BankAccount?> GetByAccountNumberAsync(string accountNumber);
    Task<IEnumerable<BankAccount>> GetByCustomerIdAsync(Guid customerId);
    Task AddAsync(BankAccount account);
    Task UpdateAsync(BankAccount account);
    Task SaveChangesAsync();
}
