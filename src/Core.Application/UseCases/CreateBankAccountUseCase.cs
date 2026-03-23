using Core.Application.DTOs;
using Core.Domain.Entities;
using Core.Domain.Ports;

namespace Core.Application.UseCases;

/// <summary>
/// Caso de uso para criar uma nova conta corrente
/// </summary>
public class CreateBankAccountUseCase
{
    private readonly IBankAccountRepository _accountRepository;
    private readonly ICustomerRepository _customerRepository;

    public CreateBankAccountUseCase(IBankAccountRepository accountRepository, ICustomerRepository customerRepository)
    {
        _accountRepository = accountRepository;
        _customerRepository = customerRepository;
    }

    public async Task<BankAccountDTO> ExecuteAsync(CreateBankAccountDTO input)
    {
        // Validar se cliente existe
        var customer = await _customerRepository.GetByIdAsync(input.CustomerId);
        if (customer == null || !customer.IsActive)
            throw new InvalidOperationException("Cliente não encontrado ou inativo");

        // Criar nova conta
        var account = new BankAccount(customerId: input.CustomerId);

        // Persistir
        await _accountRepository.AddAsync(account);
        await _accountRepository.SaveChangesAsync();

        // Retornar DTO
        return new BankAccountDTO
        {
            Id = account.Id,
            CustomerId = account.CustomerId,
            AccountNumber = account.AccountNumber,
            Branch = account.Branch,
            Balance = account.Balance.Amount,
            Currency = account.Balance.Currency,
            CreatedAt = account.CreatedAt,
            IsActive = account.IsActive
        };
    }
}
