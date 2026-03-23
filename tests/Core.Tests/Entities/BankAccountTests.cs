using Core.Domain.Entities;
using Core.Domain.ValueObjects;
using Xunit;

namespace Core.Tests.Entities;

public class BankAccountTests
{
    private readonly Guid _customerId = Guid.NewGuid();

    [Fact]
    public void CreateAccount_ShouldInitializeWithZeroBalance()
    {
        var account = new BankAccount(_customerId);
        Assert.Equal(0, account.Balance.Amount);
        Assert.True(account.IsActive);
    }

    [Fact]
    public void Deposit_ShouldIncreaseBalance()
    {
        var account = new BankAccount(_customerId);
        account.Deposit(100);
        Assert.Equal(100, account.Balance.Amount);
        Assert.Single(account.Transactions);
    }

    [Fact]
    public void Withdraw_WithSufficientBalance_ShouldDecreaseBalance()
    {
        var account = new BankAccount(_customerId);
        account.Deposit(100);
        account.Withdraw(30);
        Assert.Equal(70, account.Balance.Amount);
    }

    [Fact]
    public void Withdraw_WithInsufficientBalance_ShouldThrow()
    {
        var account = new BankAccount(_customerId);
        account.Deposit(50);
        Assert.Throws<InvalidOperationException>(() => account.Withdraw(100));
    }

    [Fact]
    public void Transfer_ShouldUpdateBothAccounts()
    {
        var account1 = new BankAccount(_customerId);
        var account2 = new BankAccount(Guid.NewGuid());
        
        account1.Deposit(100);
        account1.Transfer(account2, 30);
        
        Assert.Equal(70, account1.Balance.Amount);
        Assert.Equal(30, account2.Balance.Amount);
    }
}
