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

    /// <summary>
    /// ⚠️ MÉTODO FICTÍCIO - SIMULA SQL INJECTION VULNERABILITY
    /// Este método é apenas para DEMONSTRAÇÃO de vulnerabilidade.
    /// NUNCA use string concatenation em queries SQL em código real!
    /// </summary>
    /// <param name="accountNumber">Account number - VULNERABLE to SQL injection</param>
    /// <returns>BankAccount or null</returns>
    public async Task<BankAccount?> GetByAccountNumberVulnerableAsync(string accountNumber)
    {
        // ❌ VULNERÁVEL: Concatenação direta de entrada do usuário
        // Exemplo de ataque: accountNumber = "' OR '1'='1"
        // Resultado: query maliciosa = SELECT * FROM BankAccounts WHERE AccountNumber = '' OR '1'='1'
        
        var query = $"SELECT * FROM BankAccounts WHERE AccountNumber = '{accountNumber}'";
        
        // Simulando execução raw SQL (NUNCA faça isso!)
        var result = await _context.BankAccounts
            .FromSqlRaw(query)
            .FirstOrDefaultAsync();
        
        return result;
    }

    /// <summary>
    /// ✅ MÉTODO CORRIGIDO - Usando Parameterized Query (Safe)
    /// Este é o modo correto de implementar a mesma funcionalidade.
    /// </summary>
    /// <param name="accountNumber">Account number - SAFE with parameters</param>
    /// <returns>BankAccount or null</returns>
    public async Task<BankAccount?> GetByAccountNumberSafeAsync(string accountNumber)
    {
        // ✅ SEGURO: Usando parameterized query
        // A entrada do usuário é separada da query SQL
        // SQL Injection é impossível porque o parâmetro é tratado como dado, não como SQL
        
        var result = await _context.BankAccounts
            .FromSqlInterpolated($"SELECT * FROM BankAccounts WHERE AccountNumber = {accountNumber}")
            .FirstOrDefaultAsync();
        
        return result;
    }
}
