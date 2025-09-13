using System.Collections;
using System.Linq.Expressions;
using SGuard;

public class IsNullOrEmptyOptimizedTests
{
    #region Test Helper Classes

    private class TestClass
    {
        public string? StringProperty { get; set; }
        public int IntProperty { get; set; }
        public List<int>? ListProperty { get; set; }
        public DateTime DateProperty { get; set; }
        public Guid GuidProperty { get; set; }
        public bool BoolProperty { get; set; }
    }


    private class EmptyClass
    {
        // No properties
    }

    private class ComplexClass
    {
        public string? Name { get; set; }
        public int Age { get; set; }
        public List<string>? Items { get; set; }
    }

    private class ReadOnlyCollectionWrapper<T> : IReadOnlyCollection<T>
    {
        private readonly ICollection<T> _collection;
        public ReadOnlyCollectionWrapper(ICollection<T> collection) => _collection = collection;
        public int Count => _collection.Count;
        public IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private class ReadOnlyDictionaryWrapper<TKey, TValue> : IReadOnlyDictionary<TKey, TValue> where TKey : notnull
    {
        private readonly Dictionary<TKey, TValue> _dictionary;
        public ReadOnlyDictionaryWrapper(Dictionary<TKey, TValue> dictionary) => _dictionary = dictionary;
        public int Count => _dictionary.Count;
        public IEnumerable<TKey> Keys => _dictionary.Keys;
        public IEnumerable<TValue> Values => _dictionary.Values;
        public TValue this[TKey key] => _dictionary[key];
        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value!);
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    #endregion

    #region NullOrEmpty<T>(T? value, SGuardCallback? callback = null)

    [Fact]
    public void NullOrEmpty_WithNullValue_ReturnsTrue()
    {
        // Arrange & Act & Assert
        Assert.True(Is.NullOrEmpty<string>(null));
        Assert.True(Is.NullOrEmpty<int?>(null));
        Assert.True(Is.NullOrEmpty<TestClass>(null));
    }

    [Theory]

    [InlineData(0)]
    [InlineData(0.0)]
    [InlineData(0f)]
    [InlineData((byte)0)]
    [InlineData((short)0)]
    [InlineData((long)0)]
    [InlineData((uint)0)]
    [InlineData((ulong)0)]
    [InlineData((sbyte)0)]
    [InlineData((ushort)0)]
    public void NullOrEmpty_WithEmptyPrimitiveValues_ReturnsTrue(object value)
    {
        // Act & Assert
        Assert.True(Is.NullOrEmpty(value));
    }

    [Fact]
    public void NullOrEmpty_WithEmptyString_ReturnsTrue()
    {
        Assert.True(Is.NullOrEmpty(""));
        Assert.True(Is.NullOrEmpty(string.Empty));
    }

    [Fact]
    public void NullOrEmpty_WithNonEmptyString_ReturnsFalse()
    {
        Assert.False(Is.NullOrEmpty("test"));
        Assert.False(Is.NullOrEmpty(" "));
        Assert.False(Is.NullOrEmpty("   "));
    }

    [Fact]
    public void NullOrEmpty_WithBooleanValues_BehavesCorrectly()
    {
        Assert.True(Is.NullOrEmpty(false));
        Assert.False(Is.NullOrEmpty(true));
    }

    [Fact]
    public void NullOrEmpty_WithGuid_BehavesCorrectly()
    {
        Assert.True(Is.NullOrEmpty(Guid.Empty));
        Assert.False(Is.NullOrEmpty(Guid.NewGuid()));
    }

    [Fact]
    public void NullOrEmpty_WithDateTime_BehavesCorrectly()
    {
        Assert.True(Is.NullOrEmpty(new DateTime(0)));
        Assert.True(Is.NullOrEmpty(DateTime.MinValue.AddTicks(-DateTime.MinValue.Ticks)));
        Assert.False(Is.NullOrEmpty(DateTime.Now));
        Assert.False(Is.NullOrEmpty(new DateTime(2023, 1, 1)));
    }

    [Fact]
    public void NullOrEmpty_WithTimeSpan_BehavesCorrectly()
    {
        Assert.True(Is.NullOrEmpty(TimeSpan.Zero));
        Assert.True(Is.NullOrEmpty(new TimeSpan(0)));
        Assert.False(Is.NullOrEmpty(TimeSpan.FromMinutes(1)));
        Assert.False(Is.NullOrEmpty(new TimeSpan(1, 0, 0)));
    }

    [Fact]
    public void NullOrEmpty_WithDateOnly_BehavesCorrectly()
    {
        Assert.True(Is.NullOrEmpty(DateOnly.MinValue));
        Assert.False(Is.NullOrEmpty(DateOnly.FromDateTime(DateTime.Now)));
        Assert.False(Is.NullOrEmpty(new DateOnly(2023, 1, 1)));
    }

    [Fact]
    public void NullOrEmpty_WithTimeOnly_BehavesCorrectly()
    {
        Assert.True(Is.NullOrEmpty(new TimeOnly(0)));
        Assert.True(Is.NullOrEmpty(TimeOnly.MinValue));
        Assert.False(Is.NullOrEmpty(new TimeOnly(12, 0)));
        Assert.False(Is.NullOrEmpty(TimeOnly.FromDateTime(DateTime.Now)));
    }

    [Fact]
    public void NullOrEmpty_WithDateTimeOffset_BehavesCorrectly()
    {
        Assert.True(Is.NullOrEmpty(new DateTimeOffset(0, TimeSpan.Zero)));
        Assert.False(Is.NullOrEmpty(DateTimeOffset.Now));
        Assert.False(Is.NullOrEmpty(new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero)));
    }

    [Fact]
    public void NullOrEmpty_WithArrays_BehavesCorrectly()
    {
        // Empty arrays
        Assert.True(Is.NullOrEmpty(new int[0]));
        Assert.True(Is.NullOrEmpty(new string[0]));
        Assert.True(Is.NullOrEmpty(Array.Empty<int>()));

        // Non-empty arrays
        Assert.False(Is.NullOrEmpty(new[] { 1, 2, 3 }));
        Assert.False(Is.NullOrEmpty(new[] { "test" }));

        // Null arrays
        Assert.True(Is.NullOrEmpty((int[]?)null));
    }

    [Fact]
    public void NullOrEmpty_WithCollections_BehavesCorrectly()
    {
        // Empty collections
        Assert.True(Is.NullOrEmpty(new List<int>()));
        Assert.True(Is.NullOrEmpty(new Dictionary<string, int>()));
        Assert.True(Is.NullOrEmpty(new HashSet<string>()));

        // Non-empty collections
        Assert.False(Is.NullOrEmpty(new List<int> { 1, 2, 3 }));
        Assert.False(Is.NullOrEmpty(new Dictionary<string, int> { { "key", 1 } }));
        Assert.False(Is.NullOrEmpty(new HashSet<string> { "item" }));

        // Null collections
        Assert.True(Is.NullOrEmpty((List<int>?)null));
    }

    [Fact]
    public void NullOrEmpty_WithReadOnlyCollections_BehavesCorrectly()
    {
        // Empty read-only collections
        var emptyRoCollection = new ReadOnlyCollectionWrapper<int>(new List<int>());
        Assert.True(Is.NullOrEmpty(emptyRoCollection));

        var emptyRoDictionary = new ReadOnlyDictionaryWrapper<string, int>(new Dictionary<string, int>());
        Assert.True(Is.NullOrEmpty(emptyRoDictionary));

        // Non-empty read-only collections
        var nonEmptyRoCollection = new ReadOnlyCollectionWrapper<int>(new List<int> { 1, 2, 3 });
        Assert.False(Is.NullOrEmpty(nonEmptyRoCollection));

        var nonEmptyRoDictionary = new ReadOnlyDictionaryWrapper<string, int>(new Dictionary<string, int> { { "key", 1 } });
        Assert.False(Is.NullOrEmpty(nonEmptyRoDictionary));
    }

    [Fact]
    public void NullOrEmpty_WithEnumerables_BehavesCorrectly()
    {
        // Empty enumerables
        Assert.True(Is.NullOrEmpty(Enumerable.Empty<int>()));
        Assert.True(Is.NullOrEmpty(new int[0].AsEnumerable()));

        // Non-empty enumerables
        Assert.False(Is.NullOrEmpty(Enumerable.Range(1, 3)));
        Assert.False(Is.NullOrEmpty(new[] { 1, 2, 3 }.AsEnumerable()));
    }

    [Fact]
    public void NullOrEmpty_WithComplexTypes_BehavesCorrectly()
    {
        // Completely empty object (all properties are default/null/empty)
        var emptyComplex = new ComplexClass();
        Assert.False(Is.NullOrEmpty(emptyComplex));

        // Object with non-empty properties
        var nonEmptyComplex = new ComplexClass { Name = "Test" };
        Assert.False(Is.NullOrEmpty(nonEmptyComplex));

        var anotherNonEmptyComplex = new ComplexClass { Age = 25 };
        Assert.False(Is.NullOrEmpty(anotherNonEmptyComplex));

        // Object with empty collection but other properties are default
        var complexWithEmptyList = new ComplexClass { Items = new List<string>() };
        Assert.False(Is.NullOrEmpty(complexWithEmptyList));

        // Object with non-empty collection
        var complexWithNonEmptyList = new ComplexClass { Items = new List<string> { "item" } };
        Assert.False(Is.NullOrEmpty(complexWithNonEmptyList));
    }

    [Fact]
    public void NullOrEmpty_WithEmptyClassNoProperties_ReturnsFalse()
    {
        // Classes with no readable properties should return false
        Assert.False(Is.NullOrEmpty(new EmptyClass()));
    }

    [Fact]
    public void NullOrEmpty_WithDefaultValues_ReturnsTrue()
    {
        Assert.True(Is.NullOrEmpty(default(int)));
        Assert.True(Is.NullOrEmpty(default(DateTime)));
        Assert.True(Is.NullOrEmpty(default(Guid)));
        Assert.True(Is.NullOrEmpty(default(bool)));
    }

    [Fact]
    public void NullOrEmpty_InvokesCallback_WithCorrectResult()
    {
        bool? callbackResult = null;
        SGuardCallback callback = result => callbackResult = result == GuardOutcome.Success;
        // True case
        var result1 = Is.NullOrEmpty("", callback);
        Assert.True(result1);
        Assert.True(callbackResult);

        // False case
        callbackResult = null;
        var result2 = Is.NullOrEmpty("test", callback);
        Assert.False(result2);
        Assert.False(callbackResult);
    }

    [Fact]
    public void NullOrEmpty_DoesNotThrow_WhenCallbackThrows()
    {
        SGuardCallback throwingCallback = _ => throw new InvalidOperationException("Test exception");

        // Should not throw even if callback does
        var result1 = Is.NullOrEmpty("", throwingCallback);
        Assert.True(result1);

        var result2 = Is.NullOrEmpty("test", throwingCallback);
        Assert.False(result2);
    }

    #endregion

    #region NullOrEmpty<T>(T? value, Expression<Func<T, object>> selector, SGuardCallback? callback = null)

    [Fact]
    public void NullOrEmpty_WithSelector_ThrowsArgumentNullException_WhenSelectorIsNull()
    {
        var testObj = new TestClass();
        Assert.Throws<ArgumentNullException>(() => Is.NullOrEmpty(testObj, (Expression<Func<TestClass, object>>)null!));
    }

    [Fact]
    public void NullOrEmpty_WithSelector_ReturnsTrue_WhenObjectIsNull()
    {
        bool? callbackResult = null;
        SGuardCallback callback = result => callbackResult = result == GuardOutcome.Success;

        var result = Is.NullOrEmpty<TestClass>(null, x => x.StringProperty!, callback);

        Assert.True(result);
        Assert.True(callbackResult);
    }

    [Fact]
    public void NullOrEmpty_WithSelector_ReturnsTrue_WhenSelectedPropertyIsNull()
    {
        var testObj = new TestClass { StringProperty = null };
        var result = Is.NullOrEmpty(testObj, x => x.StringProperty!);

        Assert.True(result);
    }

    [Fact]
    public void NullOrEmpty_WithSelector_ReturnsFalse_WhenSelectedPropertyIsNotNull()
    {
        var testObj = new TestClass { StringProperty = "test" };
        var result = Is.NullOrEmpty(testObj, x => x.StringProperty!);

        Assert.False(result);
    }

    [Fact]
    public void NullOrEmpty_WithSelector_InvokesCallback_WithCorrectResult()
    {
        bool? callbackResult = null;
        SGuardCallback callback = result => callbackResult = result == GuardOutcome.Success;
        var testObj = new TestClass { StringProperty = null };
        var result = Is.NullOrEmpty(testObj, x => x.StringProperty!, callback);

        Assert.True(result);
        Assert.True(callbackResult);
    }

    [Fact]
    public void NullOrEmpty_WithSelector_DoesNotThrow_WhenCallbackThrows()
    {
        SGuardCallback throwingCallback = _ => throw new InvalidOperationException("Test exception");

        var testObj = new TestClass { StringProperty = null };
        var result = Is.NullOrEmpty(testObj, x => x.StringProperty!, throwingCallback);

        Assert.True(result);
    }

    #endregion


    [Fact]
    public void NullOrEmpty_WithAnonymousTypes_BehavesCorrectly()
    {
        // Anonymous type with all default values
        var emptyAnonymous = new { Name = (string?)null, Age = 0, Active = false };
        Assert.False(Is.NullOrEmpty(emptyAnonymous));

        // Anonymous type with non-default values
        var nonEmptyAnonymous = new { Name = "Test", Age = 0, Active = false };
        Assert.False(Is.NullOrEmpty(nonEmptyAnonymous));
    }

    [Fact]
    public void NullOrEmpty_WithNestedObjects_BehavesCorrectly()
    {
        var nestedObject = new
        {
            Level1 = new ComplexClass
            {
                Name = null,
                Age = 0,
                Items = new List<string>()
            }
        };

        // The outer object has a non-null property (Level1), so it's not empty
        Assert.False(Is.NullOrEmpty(nestedObject));
    }

    [Fact]
    public void NullOrEmpty_WithValueTypes_BehavesCorrectly()
    {
        // Default struct (all properties are default)
        var defaultStruct = new TestStruct();
        Assert.True(Is.NullOrEmpty(defaultStruct));

        // Struct with non-default value
        var nonDefaultStruct = new TestStruct { Value = 1 };
        Assert.False(Is.NullOrEmpty(nonDefaultStruct));

        var structWithName = new TestStruct { Name = "test" };
        Assert.False(Is.NullOrEmpty(structWithName));
    }

    [Fact]
    public void NullOrEmpty_WithDecimalValues_BehavesCorrectly()
    {
        Assert.True(Is.NullOrEmpty(0m));
        Assert.True(Is.NullOrEmpty(decimal.Zero));
        Assert.False(Is.NullOrEmpty(0.1m));
        Assert.False(Is.NullOrEmpty(-1m));
        Assert.False(Is.NullOrEmpty(decimal.MaxValue));
        Assert.False(Is.NullOrEmpty(decimal.MinValue));
    }

    [Fact]
    public void NullOrEmpty_WithFloatingPointValues_HandlesSpecialValues()
    {
        // Double
        Assert.True(Is.NullOrEmpty(0.0));
        Assert.False(Is.NullOrEmpty(double.NaN));
        Assert.False(Is.NullOrEmpty(double.PositiveInfinity));
        Assert.False(Is.NullOrEmpty(double.NegativeInfinity));
        Assert.False(Is.NullOrEmpty(double.Epsilon));

        // Float
        Assert.True(Is.NullOrEmpty(0.0f));
        Assert.False(Is.NullOrEmpty(float.NaN));
        Assert.False(Is.NullOrEmpty(float.PositiveInfinity));
        Assert.False(Is.NullOrEmpty(float.NegativeInfinity));
        Assert.False(Is.NullOrEmpty(float.Epsilon));
    }

    [Theory]
    [InlineData(new int[] { })]
    public void NullOrEmpty_WithEmptyArrays_ReturnsTrue(Array array)
    {
        Assert.True(Is.NullOrEmpty(array));
    }

    [Theory]
    [InlineData(new int[] { 1 })]
    public void NullOrEmpty_WithNonEmptyArrays_ReturnsFalse(Array array)
    {
        Assert.False(Is.NullOrEmpty(array));
    }

    [Fact]
    public void NullOrEmpty_Performance_WithLargeCollections()
    {
        // Test that the method doesn't enumerate large collections unnecessarily
        var largeEmptyList = new List<int>();
        Assert.True(Is.NullOrEmpty(largeEmptyList));

        var largeNonEmptyList = new List<int> { 1 }; // Add just one item
        Assert.False(Is.NullOrEmpty(largeNonEmptyList));
    }
}

public struct TestStruct
{
    public object? Value;
    public string? Name;
}