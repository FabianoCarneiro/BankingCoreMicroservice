namespace Core.Application.DTOs;

public class CreateBankAccountDTO
{
    public Guid CustomerId { get; set; }
}

public class BankAccountDTO
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "BRL";
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class TransactionDTO
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
