using Core.Application.DTOs;
using Core.Domain.Entities;
using Core.Domain.Ports;

namespace Core.Application.UseCases;

/// <summary>
/// Caso de uso para criar um novo cliente (KYC)
/// </summary>
public class CreateCustomerUseCase
{
    private readonly ICustomerRepository _customerRepository;

    public CreateCustomerUseCase(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<CustomerDTO> ExecuteAsync(CreateCustomerDTO input)
    {
        // Criar nova entidade de domínio
        var customer = new Customer(
            cpf: input.CPF,
            name: input.Name,
            email: input.Email,
            phoneNumber: input.PhoneNumber
        );

        // Persistir
        await _customerRepository.AddAsync(customer);
        await _customerRepository.SaveChangesAsync();

        // Retornar DTO
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
