namespace SGuard.Tests;

/// <summary>
/// From 1.0, a passing <c>ThrowIf.*</c> guard returns the checked value, so it can initialize a field or property.
/// </summary>
public sealed class ReturnValueTests
{
    private sealed class DomainException(string message) : Exception(message);

    private sealed record Customer(string? Email);

    private sealed record Address(string? City);

    // Field initializers of a primary constructor: the reason guards return their value.
    private sealed class Product(string? sku, decimal price, int stock)
    {
        public string Sku { get; } = ThrowIf.NullOrEmpty(sku);
        public decimal Price { get; } = ThrowIf.NegativeOrZero(price);
        public int Stock { get; } = ThrowIf.LessThan(stock, 0);
    }

    [Fact]
    public void Guards_CanInitializeMembersOfAPrimaryConstructor()
    {
        var product = new Product("SKU-1", 9.99m, 3);

        Assert.Equal("SKU-1", product.Sku);
        Assert.Equal(9.99m, product.Price);
        Assert.Equal(3, product.Stock);
        Assert.Throws<Exceptions.NullOrEmptyException>(() => new Product("", 1m, 1));
    }

    [Fact]
    public void ComparisonGuards_ReturnTheLeftValue()
    {
        Assert.Equal(1, ThrowIf.GreaterThan(1, 2));
        Assert.Equal(1, ThrowIf.GreaterThanOrEqual(1, 2));
        Assert.Equal(2, ThrowIf.LessThan(2, 1));
        Assert.Equal(2, ThrowIf.LessThanOrEqual(2, 1));
        Assert.Equal(1.5m, ThrowIf.GreaterThan(1.5m, 2m, new DomainException("x")));
        Assert.Equal(1, ThrowIf.GreaterThan<int, int, DomainExceptionWithDefaultConstructor>(1, 2));
        Assert.Equal("b", ThrowIf.LessThan("b", "a"));
    }

    [Fact]
    public void BetweenGuards_ReturnTheValue()
    {
        Assert.Equal(20, ThrowIf.Between(20, 1, 10));
        Assert.Equal("z", ThrowIf.Between("z", "a", "m", StringComparison.Ordinal));
        Assert.Equal(20, ThrowIf.Between(20, 1, 10, new DomainException("x")));
    }

    [Fact]
    public void NullOrEmptyGuards_ReturnTheValue()
    {
        var lines = new List<int> { 1 };
        var customer = new Customer("a@example.com");
        ReadOnlySpan<int> span = [1, 2];

        Assert.Same(lines, ThrowIf.NullOrEmpty(lines));
        Assert.Same(customer, ThrowIf.NullOrEmpty(customer, c => c.Email));
        Assert.Same(customer, ThrowIf.NullOrEmpty(customer, c => c.Email, new DomainException("x")));
        Assert.Equal(2, ThrowIf.NullOrEmpty(span).Length);
        Assert.Equal(Guid.Parse("8f4f0a51-9e0e-4c41-9f07-56b1d0e7ac22"), ThrowIf.NullOrEmpty(Guid.Parse("8f4f0a51-9e0e-4c41-9f07-56b1d0e7ac22")));
    }

    [Fact]
    public void ReturnValues_AreNotNullForTheCompiler()
    {
        // Values the compiler can't see through, so it treats them as maybe-null.
        var email = MaybeNull("a@example.com");
        var address = MaybeNull(new Address("Berlin"));
        var name = MaybeNull("name");

        // These compile without nullable warnings (which fail this project's build): the guards tell the compiler that
        // their result, and their argument after the call, are not null, and a selector's parameter isn't nullable.
        string checkedEmail = ThrowIf.NullOrEmpty(email);
        Address checkedAddress = ThrowIf.NullOrEmpty(address, a => a.City);
        string checkedName = ThrowIf.NullOrWhiteSpace(name);

        Assert.Equal(email.Length, checkedEmail.Length);
        Assert.Equal("Berlin", checkedAddress.City);
        Assert.Equal(4, checkedName.Length);
        Assert.Equal(4, name.Length); // the argument is not null after the guard either
    }

    private static T? MaybeNull<T>(T value) where T : class => value;

    [Fact]
    public void GuardsCanStillBeUsedAsStatements_AndInVoidLambdas()
    {
        Action check = () => ThrowIf.GreaterThan(1, 2);
        check();

        ThrowIf.NullOrEmpty("value");
        _ = ThrowIf.Between(5, 10, 20);
    }

    private sealed class DomainExceptionWithDefaultConstructor : Exception;
}
