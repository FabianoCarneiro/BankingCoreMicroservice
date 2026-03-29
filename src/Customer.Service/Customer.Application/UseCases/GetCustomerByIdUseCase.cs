using Customer.Application.DTOs;
using Customer.Domain.Ports;

namespace Customer.Application.UseCases;

/// <summary>
/// Use case para obter um cliente por ID
/// </summary>
public class GetCustomerByIdUseCase
{
    private readonly ICustomerRepository _repository;

    public GetCustomerByIdUseCase(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerDTO?> ExecuteAsync(Guid id)
    {
        var customer = await _repository.GetByIdAsync(id);
        
        if (customer == null)
            return null;

        return MapToDTO(customer);
    }

    private CustomerDTO MapToDTO(Domain.Entities.Customer customer)
    {
        return new CustomerDTO
        {
            Id = customer.Id,
            CPF = customer.CPF.Value,
            Name = customer.Name,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt,
            IsActive = customer.IsActive
        };
    }
}
