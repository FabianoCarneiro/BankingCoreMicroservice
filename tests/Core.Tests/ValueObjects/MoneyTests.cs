using Core.Domain.ValueObjects;
using Xunit;

namespace Core.Tests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void CreateMoney_WithValidAmount_ShouldSucceed()
    {
        var money = new Money(100.50m, "BRL");
        Assert.Equal(100.50m, money.Amount);
        Assert.Equal("BRL", money.Currency);
    }

    [Fact]
    public void CreateMoney_WithNegativeAmount_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => new Money(-50, "BRL"));
    }

    [Fact]
    public void AddMoney_WithSameCurrency_ShouldSucceed()
    {
        var money1 = new Money(100, "BRL");
        var money2 = new Money(50, "BRL");
        var result = money1.Add(money2);
        Assert.Equal(150, result.Amount);
    }
}
