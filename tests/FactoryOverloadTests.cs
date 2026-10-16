namespace SGuard.Tests;

/// <summary>
/// The <c>Func&lt;TException&gt;</c> overloads of the guards that existed before 3.0.
/// </summary>
public sealed class FactoryOverloadTests
{
    public sealed class DomainException(string message) : Exception(message);

    public sealed record Order(string? Reference, List<int>? Lines);

    private int _calls;

    private DomainException Create()
    {
        _calls++;
        return new DomainException("domain rule broken");
    }

    public static TheoryData<string, Action<Func<DomainException>>> PassingGuards() => new()
    {
        { "GreaterThan", f => ThrowIf.GreaterThan(1, 2, f) },
        { "GreaterThanOrEqual", f => ThrowIf.GreaterThanOrEqual(1, 2, f) },
        { "LessThan", f => ThrowIf.LessThan(2, 1, f) },
        { "LessThanOrEqual", f => ThrowIf.LessThanOrEqual(2, 1, f) },
        { "Between", f => ThrowIf.Between(20, 1, 10, f) },
        { "Between string", f => ThrowIf.Between("z", "a", "m", StringComparison.Ordinal, f) },
        { "NullOrEmpty", f => ThrowIf.NullOrEmpty("value", f) },
        { "NullOrEmpty span", f => ThrowIf.NullOrEmpty(new ReadOnlySpan<int>([1]), f) },
        { "NullOrEmpty selector", f => ThrowIf.NullOrEmpty(new Order("A-1", null), o => o.Reference, f) },
        { "Any", f => ThrowIf.Any(new[] { 1, 2 }, x => x > 5, f) },
        { "Any span", f => ThrowIf.Any(new ReadOnlySpan<int>([1, 2]), x => x > 5, f) },
        { "All", f => ThrowIf.All(new[] { 1, 2 }, x => x > 1, f) },
        { "All span", f => ThrowIf.All(new ReadOnlySpan<int>([1, 2]), x => x > 1, f) },
    };

    public static TheoryData<string, Action<Func<DomainException>>> FailingGuards() => new()
    {
        { "GreaterThan", f => ThrowIf.GreaterThan(2, 1, f) },
        { "GreaterThan NaN", f => ThrowIf.GreaterThan(double.NaN, 1d, f) },
        { "GreaterThanOrEqual", f => ThrowIf.GreaterThanOrEqual(2, 2, f) },
        { "LessThan", f => ThrowIf.LessThan(1, 2, f) },
        { "LessThanOrEqual", f => ThrowIf.LessThanOrEqual(2, 2, f) },
        { "Between", f => ThrowIf.Between(5, 1, 10, f) },
        { "Between string", f => ThrowIf.Between("c", "a", "m", StringComparison.Ordinal, f) },
        { "NullOrEmpty null", f => ThrowIf.NullOrEmpty((string?)null, f) },
        { "NullOrEmpty empty", f => ThrowIf.NullOrEmpty("", f) },
        { "NullOrEmpty span", f => ThrowIf.NullOrEmpty(ReadOnlySpan<int>.Empty, f) },
        { "NullOrEmpty selector", f => ThrowIf.NullOrEmpty(new Order(null, null), o => o.Reference, f) },
        { "NullOrEmpty selector null root", f => ThrowIf.NullOrEmpty((Order?)null, o => o!.Reference, f) },
        { "Any", f => ThrowIf.Any(new[] { 1, 9 }, x => x > 5, f) },
        { "Any span", f => ThrowIf.Any(new ReadOnlySpan<int>([1, 9]), x => x > 5, f) },
        { "All", f => ThrowIf.All(new[] { 2, 3 }, x => x > 1, f) },
        { "All span", f => ThrowIf.All(new ReadOnlySpan<int>([2, 3]), x => x > 1, f) },
    };

    [Theory]
    [MemberData(nameof(PassingGuards))]
    public void PassingGuard_DoesNotRunTheFactory(string name, Action<Func<DomainException>> guard)
    {
        guard(Create);

        Assert.True(_calls == 0, $"{name} ran the factory.");
    }

    [Theory]
    [MemberData(nameof(FailingGuards))]
    public void FailingGuard_ThrowsTheFactoryException_Once(string name, Action<Func<DomainException>> guard)
    {
        var ex = Assert.Throws<DomainException>(() => guard(Create));

        Assert.Equal("domain rule broken", ex.Message);
        Assert.True(_calls == 1, $"{name} ran the factory {_calls} times.");
    }

    [Theory]
    [MemberData(nameof(FailingGuards))]
    public void FactoryReturningNull_ThrowsInvalidOperationException(string name, Action<Func<DomainException>> guard)
    {
        var ex = Assert.Throws<InvalidOperationException>(() => guard(() => null!));

        Assert.True(ex.Message == "The exception factory returned null.", name);
    }

    [Theory]
    [MemberData(nameof(PassingGuards))]
    public void NullFactory_ThrowsArgumentNullException_EvenWhenTheGuardPasses(string name, Action<Func<DomainException>> guard)
    {
        var ex = Assert.Throws<ArgumentNullException>(() => guard(null!));

        Assert.True(ex.ParamName == "exceptionFactory", name);
    }

    [Fact]
    public void Factory_ReportsFailureToTheCallback_BeforeItRuns()
    {
        var events = new List<string>();

        Assert.Throws<DomainException>(() => ThrowIf.GreaterThan(2, 1, () =>
        {
            events.Add("factory");
            return new DomainException("x");
        }, outcome => events.Add(outcome.ToString())));

        Assert.Equal(["Failure", "factory"], events);
    }

    [Fact]
    public void Factory_CanCaptureState()
    {
        var accountId = Guid.NewGuid();
        decimal amount = 150, balance = 100;

        var ex = Assert.Throws<DomainException>(() =>
            ThrowIf.GreaterThan(amount, balance, () => new DomainException($"Account {accountId} has insufficient funds.")));

        Assert.Contains(accountId.ToString(), ex.Message);
    }

    [Fact]
    public void OverloadResolution_PicksTheFactory_ForALambda_AndTheInstanceOverload_ForAnException()
    {
        // A lambda returning an exception binds to the factory overload, not to the instance overload with
        // TException = Func<...>, which the Exception constraint rules out.
        Assert.Throws<DomainException>(() => ThrowIf.LessThan(1, 2, () => new DomainException("factory")));
        Assert.Throws<DomainException>(() => ThrowIf.LessThan(1, 2, new DomainException("instance")));
        Assert.Throws<InvalidOperationException>(() => ThrowIf.GreaterThan<int, int, InvalidOperationException>(2, 1));
    }

    [Fact]
    public void ObsoleteConstructorArgsOverloads_StillWork()
    {
        Assert.Throws<ArgumentException>(() => ThrowIf.NullOrEmpty<string?, ArgumentException>(null, ["message", "param"]));
        Assert.Throws<ArgumentException>(() => ThrowIf.GreaterThan<int, int, ArgumentException>(2, 1, ["message"]));
    }

    [Fact]
    public void ObsoleteOverloads_AreMarkedWithTheirDiagnosticId()
    {
        var obsolete = typeof(ThrowIf).GetMethods()
            .Where(m => m.GetParameters().Any(p => p.Name == "constructorArgs"))
            .Select(m => (m.Name, Attribute: m.GetCustomAttributes(typeof(ObsoleteAttribute), false).Cast<ObsoleteAttribute>().SingleOrDefault()))
            .ToList();

        Assert.Equal(7, obsolete.Count);
        Assert.All(obsolete, m =>
        {
            Assert.NotNull(m.Attribute);
            Assert.Equal("SGUARD001", m.Attribute!.DiagnosticId);
            Assert.Equal("https://selcukgural.github.io/SGuard/docs/migration/upgrading-to-3.0#{0}", m.Attribute.UrlFormat);
        });

        var activator = typeof(ExceptionActivator).GetCustomAttributes(typeof(ObsoleteAttribute), false).Cast<ObsoleteAttribute>().Single();
        Assert.Equal("SGUARD002", activator.DiagnosticId);
    }
}
