using Core.Domain.Ports;
using Serilog;

namespace Core.Infrastructure.Notifications;

/// <summary>
/// Adaptador de notificações por email
/// Implementa a porta INotificationService
/// </summary>
public class EmailNotificationService : INotificationService
{
    private readonly ILogger _logger;

    public EmailNotificationService()
    {
        _logger = Log.ForContext<EmailNotificationService>();
    }

    public async Task SendTransactionNotificationAsync(string email, string message)
    {
        // Implementação real integraria com provedor de email (SendGrid, AWS SES, etc)
        _logger.Information("Email de notificação enviado para {Email}: {Message}", email, message);
        await Task.Delay(100); // Simular delay de envio
    }

    public async Task SendAccountStatementAsync(string email, string statement)
    {
        _logger.Information("Extrato enviado para {Email}", email);
        await Task.Delay(100);
    }

    public async Task SendTransferConfirmationAsync(string email, decimal amount, string targetAccount)
    {
        var message = $"Transferência de R${amount:F2} confirmada para conta {targetAccount}";
        _logger.Information("Confirmação de transferência enviada para {Email}: {Message}", email, message);
        await Task.Delay(100);
    }
}
