using Core.Domain.ValueObjects;

namespace Core.Domain.Entities;

/// <summary>
/// Entidade que representa uma transação bancária
/// </summary>
public class Transaction
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public TransactionType Type { get; private set; }
    public Money Amount { get; private set; }
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsProcessed { get; private set; }

    public Transaction(TransactionType transactionType, Money amount, string description, Guid accountId)
    {
        Id = Guid.NewGuid();
        Type = transactionType;
        Amount = amount;
        Description = description;
        AccountId = accountId;
        CreatedAt = DateTime.UtcNow;
        IsProcessed = false;
    }

    // EF Core
    private Transaction() { }

    public void MarkAsProcessed()
    {
        IsProcessed = true;
    }
}
