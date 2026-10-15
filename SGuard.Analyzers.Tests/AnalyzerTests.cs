namespace SGuard.Analyzers.Tests;

public sealed class NullOrEmptyOnNumberTests
{
    [Fact]
    public Task Int_IsReported() => AnalyzerTest.Verify("SGUARD101", """
        class C { void M(int quantity) => ThrowIf.NullOrEmpty([|quantity|]); }
        """);

    [Fact]
    public Task NullableDecimal_AndIs_AreReported() => AnalyzerTest.Verify("SGUARD101", """
        class C
        {
            bool M(decimal? price, bool active, double ratio)
            {
                ThrowIf.NullOrEmpty([|price|]);
                ThrowIf.NullOrEmpty([|active|]);
                return Is.NullOrEmpty([|ratio|]);
            }
        }
        """);

    [Fact]
    public Task OtherValueTypes_AreNotReported() => AnalyzerTest.Verify("SGUARD101", """
        class C
        {
            void M(Guid id, DateTime at, DayOfWeek day, string name, List<int> items, int? maybe)
            {
                ThrowIf.NullOrEmpty(id);
                ThrowIf.NullOrEmpty(at);
                ThrowIf.NullOrEmpty(day);
                ThrowIf.NullOrEmpty(name);
                ThrowIf.NullOrEmpty(items);
                ThrowIf.Null(maybe);
                ThrowIf.Negative(maybe ?? 0);
            }
        }
        """);

    [Fact]
    public async Task Message_SuggestsTheRightGuard()
    {
        var diagnostics = await AnalyzerTest.Diagnose("""
            class C
            {
                void M(int count, int? maybe, bool flag)
                {
                    ThrowIf.NullOrEmpty(count);
                    ThrowIf.NullOrEmpty(maybe);
                    ThrowIf.NullOrEmpty(flag);
                }
            }
            """);

        Assert.Collection(diagnostics.Select(d => d.GetMessage()),
            m => Assert.Equal("NullOrEmpty treats 0 as empty, so it rejects 'count' when it is 0. If 0 is a valid value, use ThrowIf.Negative, or ThrowIf.Zero or ThrowIf.NegativeOrZero to reject 0 on purpose.", m),
            m => Assert.Equal("NullOrEmpty treats 0 as empty, so it rejects 'maybe' when it is 0. If 0 is a valid value, use ThrowIf.Null to reject only a missing value.", m),
            m => Assert.Equal("NullOrEmpty treats false as empty, so it rejects 'flag' when it is false. If false is a valid value, use no guard: a bool always has a value.", m));
    }
}

public sealed class ExceptionInstanceTests
{
    [Fact]
    public Task ExceptionInstance_IsReported() => AnalyzerTest.Verify("SGUARD102", """
        class C
        {
            void M(int amount, int balance, string name, List<int> lines)
            {
                ThrowIf.GreaterThan(amount, balance, [|new InvalidOperationException("Insufficient funds.")|]);
                ThrowIf.NullOrEmpty(name, [|new ArgumentException("Name is required.")|]);
                ThrowIf.Any(lines, l => l < 0, [|new ArgumentException("Negative line.")|]);
            }
        }
        """);

    [Fact]
    public Task FactoriesAndVariables_AreNotReported() => AnalyzerTest.Verify("SGUARD102", """
        class C
        {
            static readonly Exception Shared = new InvalidOperationException();

            void M(int amount, int balance, Exception existing)
            {
                ThrowIf.GreaterThan(amount, balance, () => new InvalidOperationException("Insufficient funds."));
                ThrowIf.GreaterThan(amount, balance, existing);
                ThrowIf.GreaterThan<int, int, InvalidOperationException>(amount, balance);
                Throw.That(new InvalidOperationException());
            }
        }
        """);

    [Fact]
    public Task Fix_WrapsTheInstanceInALambda() => AnalyzerTest.VerifyFix("""
        class C
        {
            void M(int amount, int balance, string name)
            {
                ThrowIf.GreaterThan(amount, balance, new InvalidOperationException($"Need {amount - balance} more."));
                ThrowIf.NullOrEmpty(name,
                    new ArgumentException("Name is required.", nameof(name)));
            }
        }
        """, """
        class C
        {
            void M(int amount, int balance, string name)
            {
                ThrowIf.GreaterThan(amount, balance, () => new InvalidOperationException($"Need {amount - balance} more."));
                ThrowIf.NullOrEmpty(name,
                    () => new ArgumentException("Name is required.", nameof(name)));
            }
        }
        """);
}

public sealed class ReversedBoundsTests
{
    [Fact]
    public Task ConstantReversedBounds_AreReported() => AnalyzerTest.Verify("SGUARD103", """
        class C
        {
            const int Max = 10;

            bool M(int port, double ratio)
            {
                [|ThrowIf.OutOfRange(port, 65535, 1)|];
                [|ThrowIf.Between(ratio, 1.0, 0.5)|];
                return [|Is.Between(port, Max + 1, Max)|];
            }
        }
        """);

    [Fact]
    public Task OrderedOrNonConstantBounds_AreNotReported() => AnalyzerTest.Verify("SGUARD103", """
        class C
        {
            void M(int port, int min, int max, string code)
            {
                ThrowIf.OutOfRange(port, 1, 65535);
                ThrowIf.OutOfRange(port, 5, 5);
                ThrowIf.OutOfRange(port, max, min);
                ThrowIf.OutOfRange(code, "z", "a", StringComparison.Ordinal);
            }
        }
        """);
}

public sealed class SelectorSelectsObjectTests
{
    [Fact]
    public Task SelectingAnObject_IsReported() => AnalyzerTest.Verify("SGUARD104", """
        public sealed class Address { public string? City { get; set; } public string? Street { get; set; } }
        public sealed class Order { public Address? ShippingAddress { get; set; } public string? Reference { get; set; } }

        class C
        {
            void M(Order order)
            {
                ThrowIf.NullOrEmpty(order, o => [|o.ShippingAddress|]);
                ThrowIf.NullOrEmpty(order, o => [|o.ShippingAddress!|]);
            }
        }
        """);

    [Fact]
    public Task SelectingAMemberOrACollection_IsNotReported() => AnalyzerTest.Verify("SGUARD104", """
        public sealed class Address { public string? City { get; set; } }
        public sealed class Order
        {
            public Address? ShippingAddress { get; set; }
            public List<string>? Lines { get; set; }
            public Uri? Website { get; set; }
            public Guid Id { get; set; }
        }

        class C
        {
            void M(Order order)
            {
                ThrowIf.NullOrEmpty(order, o => o.ShippingAddress!.City);
                ThrowIf.NullOrEmpty(order, o => o.Lines);
                ThrowIf.NullOrEmpty(order, o => o.Website);
                ThrowIf.NullOrEmpty(order, o => o.Id);
            }
        }
        """);

    [Fact]
    public async Task Message_SuggestsAMember()
    {
        var diagnostic = Assert.Single(await AnalyzerTest.Diagnose("""
            public sealed class Address { public string? City { get; set; } }
            public sealed class Order { public Address? ShippingAddress { get; set; } }

            class C { void M(Order order) => ThrowIf.NullOrEmpty(order, o => o.ShippingAddress); }
            """));

        Assert.Equal(
            "The selector selects 'o.ShippingAddress', an object of type 'Address': the guard fails only when it is null or all " +
            "of its properties are empty. Select each member you require, such as o => o.ShippingAddress.City.",
            diagnostic.GetMessage());
    }
}
