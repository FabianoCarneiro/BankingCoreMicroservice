using Customer.Application.DTOs;
using Customer.Domain.Ports;

namespace Customer.Application.UseCases;

/// <summary>
/// Use case para atualizar um cliente
/// </summary>
public class UpdateCustomerUseCase
{
    private readonly ICustomerRepository _repository;

    public UpdateCustomerUseCase(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerDTO> ExecuteAsync(Guid id, UpdateCustomerDTO dto)
    {
        var customer = await _repository.GetByIdAsync(id);
        
        if (customer == null)
            throw new InvalidOperationException($"Cliente com ID {id} não encontrado");

        customer.UpdateContact(dto.Email, dto.PhoneNumber);
        await _repository.UpdateAsync(customer);

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
