namespace SGuard.Tests;

public sealed class IsGreaterThanTests
{
    private sealed class LeftWrap : IComparable<int>
    {
        public int Value { get; }
        public LeftWrap(int value) => Value = value;
        public int CompareTo(int other) => Value.CompareTo(other);
    }

    // ---------------------------
    // GreaterThan<TLeft, TRight>
    // ---------------------------

    [Fact]
    public void GreaterThan_Generic_ReturnsTrue_WhenLeftIsGreater()
    {
        // int vs int
        var result = Is.GreaterThan(5, 3);
        Assert.True(result);

        // sınır değer
        Assert.True(Is.GreaterThan(int.MaxValue, int.MaxValue - 1));
    }

    [Fact]
    public void GreaterThan_Generic_ReturnsFalse_WhenLeftIsEqualOrLess()
    {
        Assert.False(Is.GreaterThan(3, 5));
        Assert.False(Is.GreaterThan(10, 10));
        Assert.False(Is.GreaterThan(int.MinValue, int.MinValue));
    }

    [Fact]
    public void GreaterThan_Generic_DoesNotThrow_WhenCallbackThrows()
    {
        SGuardCallback cb = _ => throw new InvalidOperationException("boom");
        var r1 = Is.GreaterThan(2, 1, cb);
        Assert.True(r1);

        var r2 = Is.GreaterThan(1, 2, cb);
        Assert.False(r2);
    }

    [Fact]
    public void GreaterThan_Generic_SupportsCrossTypeComparable()
    {
        var left = new LeftWrap(10); // IComparable<int>
        Assert.True(Is.GreaterThan(left, 5));
        Assert.False(Is.GreaterThan(left, 10));
        Assert.False(Is.GreaterThan(left, 20));
    }

    // ----------------------------------------------
    // GreaterThan(string, string, StringComparison)
    // ----------------------------------------------

    [Fact]
    public void GreaterThan_String_Ordinal_Behavior()
    {
        // Ordinal: 'b' > 'a'
        Assert.True(Is.GreaterThan("b", "a", StringComparison.Ordinal));
        // eşit
        Assert.False(Is.GreaterThan("abc", "abc", StringComparison.Ordinal));
        // küçük
        Assert.False(Is.GreaterThan("a", "b", StringComparison.Ordinal));
    }

    [Fact]
    public void GreaterThan_String_OrdinalIgnoreCase_Behavior()
    {
        // 'B' vs 'a' -> 'B' > 'a' ignore-case ile 'b' vs 'a' gibi
        Assert.True(Is.GreaterThan("B", "a", StringComparison.OrdinalIgnoreCase));
        // eşit (ignore case)
        Assert.False(Is.GreaterThan("Hello", "hello", StringComparison.OrdinalIgnoreCase));
        // küçük (ignore case)
        Assert.False(Is.GreaterThan("a", "B", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void GreaterThan_String_Throws_OnNulls()
    {
        Assert.Throws<ArgumentNullException>(() => Is.GreaterThan(null!, "x", StringComparison.Ordinal));
        Assert.Throws<ArgumentNullException>(() => Is.GreaterThan("x", null!, StringComparison.Ordinal));
    }

    [Fact]
    public void GreaterThan_String_InvokesCallback()
    {
        bool? observed = null;
        SGuardCallback cb = _ => observed = true;

        var r1 = Is.GreaterThan("z", "a", StringComparison.Ordinal);
        Assert.True(r1);

        var r2 = Is.GreaterThan("z", "a", StringComparison.Ordinal, cb);
        Assert.True(r2);
        Assert.True(observed);
    }

    [Fact]
    public void GreaterThan_String_DoesNotThrow_WhenCallbackThrows()
    {
        SGuardCallback cb = _ => throw new InvalidOperationException("boom");
        var r = Is.GreaterThan("z", "a", StringComparison.Ordinal, cb);
        Assert.True(r);
    }

    // -------------------------------
    // GreaterThanOrEqual<TLeft,TRight>
    // -------------------------------

    [Fact]
    public void GreaterThanOrEqual_Generic_ReturnsTrue_WhenLeftIsGreaterOrEqual()
    {
        Assert.True(Is.GreaterThanOrEqual(5, 3));
        Assert.True(Is.GreaterThanOrEqual(10, 10));
    }

    [Fact]
    public void GreaterThanOrEqual_Generic_ReturnsFalse_WhenLeftIsLess()
    {
        Assert.False(Is.GreaterThanOrEqual(3, 5));
    }

    [Fact]
    public void GreaterThanOrEqual_Generic_InvokesCallback_AndIsSafe()
    {
        bool? observed = null;
        SGuardCallback ok = _ => observed = true;

        var r1 = Is.GreaterThanOrEqual(2, 2, ok);
        Assert.True(r1);
        Assert.True(observed);

        SGuardCallback boom = _ => throw new InvalidOperationException("boom");
        var r2 = Is.GreaterThanOrEqual(1, 2, boom);
        Assert.False(r2);
    }

    [Fact]
    public void GreaterThanOrEqual_Generic_SupportsCrossTypeComparable()
    {
        var left = new LeftWrap(10); // IComparable<int>
        Assert.True(Is.GreaterThanOrEqual(left, 10));
        Assert.True(Is.GreaterThanOrEqual(left, 5));
        Assert.False(Is.GreaterThanOrEqual(left, 11));
    }

    // ----------------------------------------------------------
    // GreaterThanOrEqual(string, string, StringComparison)
    // ----------------------------------------------------------

    [Fact]
    public void GreaterThanOrEqual_String_Behavior()
    {
        Assert.True(Is.GreaterThanOrEqual("b", "a", StringComparison.Ordinal));
        Assert.True(Is.GreaterThanOrEqual("abc", "abc", StringComparison.Ordinal));
        Assert.False(Is.GreaterThanOrEqual("a", "b", StringComparison.Ordinal));
    }

    [Fact]
    public void GreaterThanOrEqual_String_Throws_OnNulls()
    {
        Assert.Throws<ArgumentNullException>(() => Is.GreaterThanOrEqual(null!, "x", StringComparison.Ordinal));
        Assert.Throws<ArgumentNullException>(() => Is.GreaterThanOrEqual("x", null!, StringComparison.Ordinal));
    }

    [Fact]
    public void GreaterThanOrEqual_String_InvokesCallback_AndIsSafe()
    {
        bool? observed = null;
        SGuardCallback ok = _ => observed = true;

        var r1 = Is.GreaterThanOrEqual("a", "A", StringComparison.OrdinalIgnoreCase, ok);
        Assert.True(r1);
        Assert.True(observed);

        SGuardCallback boom = _ => throw new InvalidOperationException("boom");
        var r2 = Is.GreaterThanOrEqual("a", "b", StringComparison.Ordinal, boom);
        Assert.False(r2);
    }
}