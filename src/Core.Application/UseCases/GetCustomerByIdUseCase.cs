using Core.Application.DTOs;
using Core.Domain.Ports;

namespace Core.Application.UseCases;

/// <summary>
/// Caso de uso para obter um cliente pelo ID
/// </summary>
public class GetCustomerByIdUseCase
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdUseCase(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerDTO?> ExecuteAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("ID do cliente inválido", nameof(id));

        var customer = await _customerRepository.GetByIdAsync(id);
        
        if (customer == null)
            return null;

        return new CustomerDTO
        {
            Id = customer.Id,
            CPF = customer.CPF.Formatted,
            Name = customer.Name,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            CreatedAt = customer.CreatedAt,
            IsActive = customer.IsActive
        };
    }
}
