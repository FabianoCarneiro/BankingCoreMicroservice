using Customer.Domain.Entities;

namespace Customer.Domain.Ports;

/// <summary>
/// Porto que define as operações de persistência para Customer
/// </summary>
public interface ICustomerRepository
{
    Task<Entities.Customer?> GetByIdAsync(Guid id);
    Task<Entities.Customer?> GetByCpfAsync(string cpf);
    Task<IEnumerable<Entities.Customer>> GetAllAsync();
    Task AddAsync(Entities.Customer customer);
    Task UpdateAsync(Entities.Customer customer);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
