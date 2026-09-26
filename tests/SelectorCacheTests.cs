using System.Linq.Expressions;
using SGuard.Exceptions;

namespace SGuard.Tests;

public sealed class SelectorCacheTests
{
    // Each test uses its own types: the cache is per selector input type and shared across the test run.

    private sealed class Person
    {
        public string? Name { get; init; }
        public Address? Address { get; init; }
    }

    private sealed class Address
    {
        public string? City { get; init; }
    }

    private sealed class RepeatedShape
    {
        public string? Name { get; init; }
    }

    private sealed class DistinctMembers
    {
        public string? First { get; init; }
        public string? Second { get; init; }
    }

    private sealed class ClosureHolder
    {
        public string? Value { get; init; }
    }

    private sealed class Indexed
    {
        public List<string?> Items { get; init; } = [];
    }

    private sealed class SharedByIsAndThrowIf
    {
        public string? Name { get; init; }
    }

    private sealed class CapturedIndex
    {
        public List<string?> Items { get; init; } = [];
    }

    private sealed class SeveralConstants;

    private sealed class KeyHolder
    {
        public string Key { get; init; } = "";
    }

    #region Cache hits

    [Fact]
    public void ThrowIfNullOrEmpty_SameSelectorShape_CompilesOnce()
    {
        for (var i = 0; i < 50; i++)
        {
            ThrowIf.NullOrEmpty(new RepeatedShape { Name = $"name-{i}" }, x => x.Name);
        }

        Assert.Equal(1, SelectorCache.Count<RepeatedShape>());
    }

    [Fact]
    public void IsAndThrowIf_SameSelectorShape_ShareOneEntry()
    {
        var value = new SharedByIsAndThrowIf { Name = "a" };

        Assert.False(Is.NullOrEmpty(value, x => x.Name!));
        ThrowIf.NullOrEmpty(value, x => x.Name);

        Assert.Equal(1, SelectorCache.Count<SharedByIsAndThrowIf>());
    }

    [Fact]
    public void Comparer_EqualShapes_WithDifferentParameterNames_AreEqual()
    {
        Expression<Func<Person, object?>> a = p => p.Address!.City;
        Expression<Func<Person, object?>> b = other => other.Address!.City;

        Assert.True(SelectorShapeComparer.IsCacheable(a));
        Assert.True(SelectorShapeComparer.Instance.Equals(a, b));
        Assert.Equal(SelectorShapeComparer.Instance.GetHashCode(a), SelectorShapeComparer.Instance.GetHashCode(b));
    }

    #endregion

    #region Correctness

    [Fact]
    public void DifferentMembers_OfSameType_AreNotConfused()
    {
        var value = new DistinctMembers { First = "set", Second = null };

        ThrowIf.NullOrEmpty(value, x => x.First);
        Assert.Throws<NullOrEmptyException>(() => ThrowIf.NullOrEmpty(value, x => x.Second));

        Assert.False(Is.NullOrEmpty(value, x => x.First!));
        Assert.True(Is.NullOrEmpty(value, x => x.Second!));

        Assert.Equal(2, SelectorCache.Count<DistinctMembers>());
    }

    [Fact]
    public void CapturedVariables_ShareOneEntry_AndEvaluateTheCurrentCapture()
    {
        var results = new List<bool>();

        foreach (var captured in new[] { new ClosureHolder { Value = "x" }, new ClosureHolder { Value = null } })
        {
            // Same shape on every iteration, but a different captured object.
            results.Add(Is.NullOrEmpty(new ClosureHolder(), _ => captured.Value!));
        }

        Assert.Equal([false, true], results);
        Assert.Equal(1, SelectorCache.Count<ClosureHolder>());
    }

    [Fact]
    public void Comparer_DifferentConstants_AreNotEqual()
    {
        Expression<Func<Indexed, object?>> first = x => x.Items[0];
        Expression<Func<Indexed, object?>> second = x => x.Items[1];

        Assert.False(SelectorShapeComparer.Instance.Equals(first, second));
    }

    [Fact]
    public void Comparer_CapturedVariables_WithDifferentValues_AreEqual()
    {
        var first = Capture(new Person { Name = "first" });
        var second = Capture(new Person { Name = null });

        Assert.True(SelectorShapeComparer.IsCacheable(first));
        Assert.True(SelectorShapeComparer.Instance.Equals(first, second));
        Assert.Equal(SelectorShapeComparer.Instance.GetHashCode(first), SelectorShapeComparer.Instance.GetHashCode(second));

        static Expression<Func<Person, object?>> Capture(Person captured) => _ => captured.Name;
    }

    [Fact]
    public void CapturedIndex_SharesOneEntry_AndReadsTheCurrentIndex()
    {
        var value = new CapturedIndex { Items = ["x", ""] };
        var results = new List<bool>();

        for (var i = 0; i < 2; i++)
        {
            results.Add(Is.NullOrEmpty(value, v => v.Items[i]!));
        }

        Assert.Equal([false, true], results);
        Assert.Equal(1, SelectorCache.Count<CapturedIndex>());
    }

    [Fact]
    public void SeveralCapturedConstants_ArePassedInOrder()
    {
        // Two constants of different types in one selector: a dictionary (the call's instance) and a holder (its argument).
        Assert.False(Evaluate(new Dictionary<string, string?> { ["a"] = "value" }, new KeyHolder { Key = "a" }));
        Assert.True(Evaluate(new Dictionary<string, string?> { ["b"] = "" }, new KeyHolder { Key = "b" }));
        Assert.Equal(1, SelectorCache.Count<SeveralConstants>());

        static bool Evaluate(Dictionary<string, string?> map, KeyHolder holder)
        {
            var parameter = Expression.Parameter(typeof(SeveralConstants), "x");
            var key = Expression.Property(Expression.Constant(holder), nameof(KeyHolder.Key));
            var lookup = Expression.Call(Expression.Constant(map), typeof(Dictionary<string, string?>).GetMethod("get_Item")!, key);
            var selector = Expression.Lambda<Func<SeveralConstants, object>>(lookup, parameter);

            return Is.NullOrEmpty(new SeveralConstants(), selector);
        }
    }

    [Fact]
    public void NestedSelector_NullIntermediate_IsNullOrEmpty_OnRepeatedCalls()
    {
        for (var i = 0; i < 3; i++)
        {
            Assert.True(Is.NullOrEmpty(new Person { Address = null }, p => p.Address!.City!));
            Assert.False(Is.NullOrEmpty(new Person { Address = new Address { City = "Izmir" } }, p => p.Address!.City!));
        }
    }

    #endregion
}
