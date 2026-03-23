using Core.Domain.ValueObjects;

namespace Core.Domain.Entities;

/// <summary>
/// Entidade que representa uma conta bancária (agregado raiz)
/// </summary>
public class BankAccount
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public string AccountNumber { get; private set; }
    public string Branch { get; private set; }
    public Money Balance { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public bool IsActive { get; private set; }
    private readonly List<Transaction> _transactions = new();
    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

    public BankAccount(Guid customerId, string branch = "0001")
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        Branch = branch;
        AccountNumber = GenerateAccountNumber();
        Balance = new Money(0, "BRL");
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    // EF Core
    private BankAccount() { }

    public void Deposit(decimal amount, string description = "Depósito")
    {
        if (!IsActive)
            throw new InvalidOperationException("Conta inativa não pode receber depósitos");

        var money = new Money(amount);
        Balance = Balance.Add(money);

        _transactions.Add(new Transaction(
            transactionType: TransactionType.Deposit,
            amount: money,
            description: description,
            accountId: Id
        ));
    }

    public void Withdraw(decimal amount, string description = "Saque")
    {
        if (!IsActive)
            throw new InvalidOperationException("Conta inativa não pode fazer saques");

        var money = new Money(amount);

        if (Balance.Amount < money.Amount)
            throw new InvalidOperationException("Saldo insuficiente");

        Balance = Balance.Subtract(money);

        _transactions.Add(new Transaction(
            transactionType: TransactionType.Withdrawal,
            amount: money,
            description: description,
            accountId: Id
        ));
    }

    public void Transfer(BankAccount targetAccount, decimal amount, string description = "Transferência")
    {
        if (!IsActive || !targetAccount.IsActive)
            throw new InvalidOperationException("Contas envolvidas devem estar ativas");

        var money = new Money(amount);

        if (Balance.Amount < money.Amount)
            throw new InvalidOperationException("Saldo insuficiente para transferência");

        // Debita da conta de origem
        Balance = Balance.Subtract(money);
        _transactions.Add(new Transaction(
            transactionType: TransactionType.Transfer,
            amount: money,
            description: $"Transferência enviada para {targetAccount.AccountNumber}",
            accountId: Id
        ));

        // Credita na conta de destino
        targetAccount.Balance = targetAccount.Balance.Add(money);
        targetAccount._transactions.Add(new Transaction(
            transactionType: TransactionType.Transfer,
            amount: money,
            description: $"Transferência recebida de {AccountNumber}",
            accountId: targetAccount.Id
        ));
    }

    public void Close()
    {
        if (Balance.Amount > 0)
            throw new InvalidOperationException("Não é possível fechar conta com saldo");

        IsActive = false;
        ClosedAt = DateTime.UtcNow;
    }

    private static string GenerateAccountNumber()
    {
        return Guid.NewGuid().ToString("N")[..10];
    }
}

public enum TransactionType
{
    Deposit,
    Withdrawal,
    Transfer,
    BillPayment
}
