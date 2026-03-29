using Customer.Application.DTOs;
using Customer.Domain.Ports;

namespace Customer.Application.UseCases;

/// <summary>
/// Use case para listar todos os clientes
/// </summary>
public class ListAllCustomersUseCase
{
    private readonly ICustomerRepository _repository;

    public ListAllCustomersUseCase(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CustomerDTO>> ExecuteAsync()
    {
        var customers = await _repository.GetAllAsync();
        return customers.Select(MapToDTO).ToList();
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
