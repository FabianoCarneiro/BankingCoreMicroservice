using Core.Domain.Ports;

namespace Core.Application.UseCases;

/// <summary>
/// Caso de uso para transferências (TED/PIX)
/// </summary>
public class TransferUseCase
{
    private readonly IBankAccountRepository _accountRepository;
    private readonly INotificationService _notificationService;
    private readonly ICustomerRepository _customerRepository;

    public TransferUseCase(
        IBankAccountRepository accountRepository,
        INotificationService notificationService,
        ICustomerRepository customerRepository)
    {
        _accountRepository = accountRepository;
        _notificationService = notificationService;
        _customerRepository = customerRepository;
    }

    public async Task ExecuteAsync(string fromAccountNumber, string toAccountNumber, decimal amount)
    {
        // Buscar contas
        var sourceAccount = await _accountRepository.GetByAccountNumberAsync(fromAccountNumber);
        var targetAccount = await _accountRepository.GetByAccountNumberAsync(toAccountNumber);

        if (sourceAccount == null || targetAccount == null)
            throw new InvalidOperationException("Uma ou ambas as contas não foram encontradas");

        // Executar transferência (lógica de domínio)
        sourceAccount.Transfer(targetAccount, amount);

        // Persistir
        await _accountRepository.UpdateAsync(sourceAccount);
        await _accountRepository.UpdateAsync(targetAccount);
        await _accountRepository.SaveChangesAsync();

        // Enviar notificações
        var sourceCustomer = await _customerRepository.GetByIdAsync(sourceAccount.CustomerId);
        var targetCustomer = await _customerRepository.GetByIdAsync(targetAccount.CustomerId);

        if (sourceCustomer != null)
            await _notificationService.SendTransferConfirmationAsync(
                sourceCustomer.Email, amount, targetAccount.AccountNumber);

        if (targetCustomer != null)
            await _notificationService.SendTransferConfirmationAsync(
                targetCustomer.Email, amount, sourceAccount.AccountNumber);
    }
}
