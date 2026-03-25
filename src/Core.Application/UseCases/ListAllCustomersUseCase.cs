using Core.Application.DTOs;
using Core.Domain.Ports;

namespace Core.Application.UseCases;

/// <summary>
/// Caso de uso para listar todos os clientes
/// </summary>
public class ListAllCustomersUseCase
{
    private readonly ICustomerRepository _customerRepository;

    public ListAllCustomersUseCase(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<CustomerDTO>> ExecuteAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        
        return customers.Select(c => new CustomerDTO
        {
            Id = c.Id,
            CPF = c.CPF.Formatted,
            Name = c.Name,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            CreatedAt = c.CreatedAt,
            IsActive = c.IsActive
        }).ToList();
    }
}
