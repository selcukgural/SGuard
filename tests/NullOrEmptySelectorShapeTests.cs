using SGuard.Exceptions;

namespace SGuard.Tests;

/// <summary>
/// Selectors that go beyond a plain member path: indexers, method calls and operators. A null on the path to the selected
/// value counts as empty; other expressions are evaluated as written.
/// </summary>
public sealed class NullOrEmptySelectorShapeTests
{
    private sealed class Model
    {
        public string? First { get; init; }
        public string? Second { get; init; }
        public int Number { get; init; }
        public List<string?>? Items { get; init; }
        public string?[]? Array { get; init; }
        public Model? Child { get; init; }
    }

    private sealed class Counting
    {
        private readonly Model? _child;

        public Counting(Model? child) => _child = child;

        public int Reads { get; private set; }

        public Model? Child
        {
            get
            {
                Reads++;
                return _child;
            }
        }
    }

    private static readonly Model Filled = new()
    {
        First = "first",
        Second = "",
        Number = 3,
        Items = ["item", ""],
        Array = ["element", null],
        Child = new Model { First = "child" },
    };

    private static readonly Model Empty = new();

    [Fact]
    public void ListIndexer_ChecksTheElement()
    {
        Assert.False(Is.NullOrEmpty(Filled, m => m.Items![0]!));
        Assert.True(Is.NullOrEmpty(Filled, m => m.Items![1]!));
    }

    [Fact]
    public void ListIndexer_NullList_IsEmpty()
    {
        Assert.True(Is.NullOrEmpty(Empty, m => m.Items![0]!));
        Assert.Throws<NullOrEmptyException>(() => ThrowIf.NullOrEmpty(Empty, m => m.Items![0]));
    }

    [Fact]
    public void ArrayIndex_ChecksTheElement()
    {
        Assert.False(Is.NullOrEmpty(Filled, m => m.Array![0]!));
        Assert.True(Is.NullOrEmpty(Filled, m => m.Array![1]!));
        Assert.True(Is.NullOrEmpty(Empty, m => m.Array![0]!));
    }

    [Fact]
    public void ArrayLength_ZeroIsEmpty()
    {
        Assert.False(Is.NullOrEmpty(Filled, m => m.Array!.Length));
        Assert.True(Is.NullOrEmpty(new Model { Array = [] }, m => m.Array!.Length));
        Assert.True(Is.NullOrEmpty(Empty, m => m.Array!.Length));
    }

    [Fact]
    public void InstanceMethod_OnPath_NullInstanceIsEmpty()
    {
        Assert.False(Is.NullOrEmpty(Filled, m => m.First!.Trim()));
        Assert.True(Is.NullOrEmpty(new Model { First = "   " }, m => m.First!.Trim()));
        Assert.True(Is.NullOrEmpty(Empty, m => m.First!.Trim()));
    }

    [Fact]
    public void MethodOnValueType_IsEvaluated()
    {
        Assert.False(Is.NullOrEmpty(Filled, m => m.Number.ToString()));
    }

    [Fact]
    public void Concatenation_IsEvaluatedAsWritten()
    {
        Assert.False(Is.NullOrEmpty(Filled, m => m.First + m.Second));
        Assert.True(Is.NullOrEmpty(Empty, m => m.First + m.Second));
    }

    [Fact]
    public void StaticMethod_IsEvaluatedAsWritten()
    {
        Assert.False(Is.NullOrEmpty(Filled, m => string.Concat(m.First, m.Second)));
        Assert.True(Is.NullOrEmpty(Empty, m => string.Concat(m.First, m.Second)));
    }

    [Fact]
    public void Coalesce_IsAndThrowIfAgree()
    {
        var bothEmpty = new Model { First = null, Second = "" };

        Assert.True(Is.NullOrEmpty(bothEmpty, m => m.First ?? m.Second!));
        Assert.Throws<NullOrEmptyException>(() => ThrowIf.NullOrEmpty(bothEmpty, m => m.First ?? m.Second));
        Assert.False(Is.NullOrEmpty(Filled, m => m.First ?? m.Second!));
    }

    [Fact]
    public void CapturedRoot_Null_IsEmpty()
    {
        Model? captured = null;

        Assert.True(Is.NullOrEmpty(Filled, _ => captured!.First!));

        captured = Filled;

        Assert.False(Is.NullOrEmpty(Filled, _ => captured.First!));
    }

    [Fact]
    public void PathValues_AreReadOnce()
    {
        var counting = new Counting(new Model { First = "value" });

        Assert.False(Is.NullOrEmpty(counting, c => c.Child!.First!));
        Assert.Equal(1, counting.Reads);
    }

    [Fact]
    public void NullableValueType_DefaultValue_IsEmpty_ForIsAndThrowIf()
    {
        var zero = new NullableHolder { Value = 0 };

        Assert.True(Is.NullOrEmpty(zero, h => h.Value!));
        Assert.Throws<NullOrEmptyException>(() => ThrowIf.NullOrEmpty(zero, h => h.Value));
        Assert.False(Is.NullOrEmpty(new NullableHolder { Value = 1 }, h => h.Value!));
    }

    private sealed class NullableHolder
    {
        public int? Value { get; init; }
    }
}
