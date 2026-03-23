using Core.Domain.Entities;
using Core.Domain.ValueObjects;

namespace Core.Domain.Ports;

/// <summary>
/// Porto (interface) para persistência de clientes
/// Implementação será fornecida pela infraestrutura
/// </summary>
public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id);
    Task<Customer?> GetByCPFAsync(CPF cpf);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(Guid id);
    Task SaveChangesAsync();
}
