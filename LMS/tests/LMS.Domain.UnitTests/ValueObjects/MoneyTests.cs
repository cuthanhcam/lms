using FluentAssertions;
using LMS.Domain.Exceptions;
using LMS.Domain.ValueObjects;

namespace LMS.Domain.UnitTests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowDomainException()
    {
        Action act = () => Money.Create(-1m, "USD");

        act.Should().Throw<DomainException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void Create_ShouldNormalizeCurrencyToUppercase()
    {
        var money = Money.Create(10m, "usd");

        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Add_WithSameCurrency_ShouldReturnSummedAmount()
    {
        var a = Money.Create(10m, "USD");
        var b = Money.Create(5m, "USD");

        var result = a.Add(b);

        result.Amount.Should().Be(15m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Add_WithDifferentCurrencies_ShouldThrowDomainException()
    {
        var usd = Money.Create(10m, "USD");
        var eur = Money.Create(5m, "EUR");

        Action act = () => usd.Add(eur);

        act.Should().Throw<DomainException>()
            .WithMessage("*different currencies*");
    }

    [Fact]
    public void Equality_WithSameAmountAndCurrency_ShouldBeTrue()
    {
        var left = Money.Create(100m, "USD");
        var right = Money.Create(100m, "USD");

        (left == right).Should().BeTrue();
        left.Equals(right).Should().BeTrue();
    }
}
