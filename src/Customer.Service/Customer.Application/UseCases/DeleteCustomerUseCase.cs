using Customer.Application.DTOs;
using Customer.Domain.Ports;

namespace Customer.Application.UseCases;

/// <summary>
/// Use case para deletar um cliente
/// </summary>
public class DeleteCustomerUseCase
{
    private readonly ICustomerRepository _repository;

    public DeleteCustomerUseCase(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Guid id)
    {
        var exists = await _repository.ExistsAsync(id);
        
        if (!exists)
            throw new InvalidOperationException($"Cliente com ID {id} não encontrado");

        await _repository.DeleteAsync(id);
    }
}
