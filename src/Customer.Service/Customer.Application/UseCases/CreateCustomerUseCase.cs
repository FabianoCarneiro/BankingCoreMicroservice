using Customer.Application.DTOs;
using Customer.Domain.Ports;

namespace Customer.Application.UseCases;

/// <summary>
/// Use case para criar um novo cliente
/// </summary>
public class CreateCustomerUseCase
{
    private readonly ICustomerRepository _repository;

    public CreateCustomerUseCase(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerDTO> ExecuteAsync(CreateCustomerDTO dto)
    {
        // Validar se cliente já existe
        var existing = await _repository.GetByCpfAsync(dto.CPF);
        if (existing != null)
            throw new InvalidOperationException($"Cliente com CPF {dto.CPF} já existe");

        // Criar nova entidade
        var customer = new Domain.Entities.Customer(dto.CPF, dto.Name, dto.Email, dto.PhoneNumber);
        
        // Persistir
        await _repository.AddAsync(customer);

        // Retornar DTO
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
