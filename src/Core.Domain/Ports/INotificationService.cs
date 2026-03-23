namespace Core.Domain.Ports;

/// <summary>
/// Porto para envio de notificações transacionais
/// </summary>
public interface INotificationService
{
    Task SendTransactionNotificationAsync(string email, string message);
    Task SendAccountStatementAsync(string email, string statement);
    Task SendTransferConfirmationAsync(string email, decimal amount, string targetAccount);
}
