using System.Collections;
using System.Collections.Immutable;
using SGuard.Exceptions;
#pragma warning disable CS8603 // Possible null reference return.

namespace SGuard.Tests;

public sealed class NullOrEmptyVisitorOptimizedTests
{

    private sealed class WithStrings
    {
        public string? S1 { get; init; }
        public string S2 { get; init; } = string.Empty;
    }

    private sealed class WithArrays
    {
        public int[]? A1 { get; init; }
        public string[] A2 { get; init; } = [];
        public byte[] A3 { get; init; } = [1, 2];
    }

    private sealed class WithCollections
    {
        // ICollection (non-generic)
        public ArrayList? NonGeneric { get; init; }

        // ICollection<T>
        public List<int>? List { get; init; }

        // IReadOnlyCollection<T>
        public IReadOnlyCollection<int>? ReadOnlyCollection { get; init; }

        // IDictionary (non-generic)
        public Hashtable? NonGenericDict { get; init; }

        // IReadOnlyDictionary<TKey,TValue>
        public IReadOnlyDictionary<string, int>? ReadOnlyDict { get; init; }
    }

    private sealed class EnumerableWithCount(IEnumerable<int> items) : IEnumerable
    {
        private readonly List<int> _inner = [..items];
        public int Count => _inner.Count;
        public IEnumerator GetEnumerator() => _inner.GetEnumerator();
    }

    private sealed class WithEnumerables
    {
        // IEnumerable with Count property (fast-path)
        public EnumerableWithCount? HasCount { get; init; }

        // IEnumerable without Count property (enumeration path)
        public IEnumerable<int>? NoCount { get; init; }
    }

    private sealed class WithValueTypes
    {
        public Guid GuidProp { get; init; }         // struct, default(Guid) empty
        public DateTime DateTimeProp { get; init; } // handled by ticks == 0
        public int IntProp { get; init; }           // primitive (not part of visitor's default(T) branch)
        public decimal DecimalProp { get; init; }   // excluded from default(T) visitor branch
        public int? NullableInt { get; init; }      // Nullable<T>
    }

    private sealed class NestedLevel2
    {
        public string? Inner { get; init; }
        public int[]? InnerArray { get; init; }
    }

    private sealed class NestedLevel1
    {
        public NestedLevel2? L2 { get; init; }
    }

    private sealed class WithNested
    {
        public NestedLevel1? L1 { get; init; }
    }

    [Fact]
    public void String_Null_Empty_NonEmpty()
    {
        var m1 = new WithStrings { S1 = null };
        var m2 = new WithStrings { S1 = "" };
        var m3 = new WithStrings { S1 = "x" };

        Assert.True(Is.NullOrEmpty(m1, x => x.S1));
        Assert.True(Is.NullOrEmpty(m2, x => x.S1));
        Assert.False(Is.NullOrEmpty(m3, x => x.S1));

        // Pre-initialized empty
        var m4 = new WithStrings(); // S2 = ""
        Assert.True(Is.NullOrEmpty(m4, x => x.S2));
    }

    [Fact]
    public void Arrays_Empty_And_NonEmpty()
    {
        var m1 = new WithArrays { A1 = null, A2 = [], A3 = [1, 2] };
        var m2 = new WithArrays { A1 = [1], A2 = ["a"], A3 = [] };

        Assert.True(Is.NullOrEmpty(m1, x => x.A1));  // null
        Assert.True(Is.NullOrEmpty(m1, x => x.A2));  // empty
        Assert.False(Is.NullOrEmpty(m1, x => x.A3)); // non-empty

        Assert.False(Is.NullOrEmpty(m2, x => x.A1)); // non-empty
        Assert.False(Is.NullOrEmpty(m2, x => x.A2)); // non-empty
        Assert.True(Is.NullOrEmpty(m2, x => x.A3));  // empty
    }

    [Fact]
    public void Collections_ICollection_IReadOnlyCollection_IDictionary_IReadOnlyDictionary()
    {
        var m1 = new WithCollections
        {
            NonGeneric = null,
            List = [],
            ReadOnlyCollection = [],
            NonGenericDict = new Hashtable(),
            ReadOnlyDict = new Dictionary<string, int>()
        };

        var m2 = new WithCollections
        {
            NonGeneric = new ArrayList { 1 },
            List = [42],
            ReadOnlyCollection = new List<int> { 1, 2 },
            NonGenericDict = new Hashtable { { "a", 1 } },
            ReadOnlyDict = new Dictionary<string, int> { { "k", 5 } }
        };

        Assert.True(Is.NullOrEmpty(m1, x => x.NonGeneric));         // null
        Assert.True(Is.NullOrEmpty(m1, x => x.List));               // Count == 0
        Assert.True(Is.NullOrEmpty(m1, x => x.ReadOnlyCollection)); // Count == 0
        Assert.True(Is.NullOrEmpty(m1, x => x.NonGenericDict));     // Count == 0
        Assert.True(Is.NullOrEmpty(m1, x => x.ReadOnlyDict));       // Count == 0

        Assert.False(Is.NullOrEmpty(m2, x => x.NonGeneric));         // Count > 0
        Assert.False(Is.NullOrEmpty(m2, x => x.List));               // Count > 0
        Assert.False(Is.NullOrEmpty(m2, x => x.ReadOnlyCollection)); // Count > 0
        Assert.False(Is.NullOrEmpty(m2, x => x.NonGenericDict));     // Count > 0
        Assert.False(Is.NullOrEmpty(m2, x => x.ReadOnlyDict));       // Count > 0
    }

    [Fact]
    public void Enumerables_With_And_Without_Count()
    {
        var m1 = new WithEnumerables
        {
            HasCount = new EnumerableWithCount([]),
            NoCount = []
        };

        var m2 = new WithEnumerables
        {
            HasCount = new EnumerableWithCount([1, 2]),
            NoCount = [10]
        };

        // With Count property -> Count == 0 is empty; > 0 is not empty
        Assert.True(Is.NullOrEmpty(m1, x => x.HasCount));
        Assert.False(Is.NullOrEmpty(m2, x => x.HasCount));

        // Without Count -> uses Any()
        Assert.True(Is.NullOrEmpty(m1, x => x.NoCount));
        Assert.False(Is.NullOrEmpty(m2, x => x.NoCount));
    }

    [Fact]
    public void ValueTypes_Default_And_NonDefault()
    {
        var m1 = new WithValueTypes
        {
            GuidProp = default,
            DateTimeProp = default, // ticks==0
            IntProp = 0,
            DecimalProp = 0m,
            NullableInt = null
        };

        var m2 = new WithValueTypes
        {
            GuidProp = Guid.NewGuid(),
            DateTimeProp = DateTime.UtcNow, // ticks>0
            IntProp = 1,
            DecimalProp = 1m,
            NullableInt = 10
        };

        Assert.True(Is.NullOrEmpty(m1, x => x.GuidProp));     // default(Guid)
        Assert.True(Is.NullOrEmpty(m1, x => x.DateTimeProp)); // ticks == 0
        Assert.True(Is.NullOrEmpty(m1, x => x.IntProp));      // 0
        Assert.True(Is.NullOrEmpty(m1, x => x.DecimalProp));  // 0m
        Assert.True(Is.NullOrEmpty(m1, x => x.NullableInt));  // null

        Assert.False(Is.NullOrEmpty(m2, x => x.GuidProp));     // non-default Guid
        Assert.False(Is.NullOrEmpty(m2, x => x.DateTimeProp)); // ticks > 0
        Assert.False(Is.NullOrEmpty(m2, x => x.IntProp));      // 1
        Assert.False(Is.NullOrEmpty(m2, x => x.DecimalProp));  // 1m
        Assert.False(Is.NullOrEmpty(m2, x => x.NullableInt));  // has value
    }

    [Fact]
    public void Nested_Member_Access_With_Null_ShortCircuit()
    {
        // L1 null -> inner member access should short-circuit and be considered "empty"
        var n1 = new WithNested { L1 = null };
        Assert.True(Is.NullOrEmpty(n1, x => x.L1!.L2!.Inner)); // null-safe, true beklenir
        Assert.True(Is.NullOrEmpty(n1, x => x.L1!.L2!.InnerArray));

        // L1 var, L2 null
        var n2 = new WithNested { L1 = new NestedLevel1 { L2 = null } };
        Assert.True(Is.NullOrEmpty(n2, x => x.L1!.L2!.Inner));
        Assert.True(Is.NullOrEmpty(n2, x => x.L1!.L2!.InnerArray));

        // Tamamı dolu ve non-empty
        var n3 = new WithNested
        {
            L1 = new NestedLevel1
            {
                L2 = new NestedLevel2
                {
                    Inner = "ok",
                    InnerArray = [1]
                }
            }
        };
        Assert.False(Is.NullOrEmpty(n3, x => x.L1!.L2!.Inner));
        Assert.False(Is.NullOrEmpty(n3, x => x.L1!.L2!.InnerArray));
    }

    [Fact]
    public void Selector_Null_Throws_ArgumentNullException()
    {
        var obj = new WithStrings { S1 = "x" };
        Assert.Throws<ArgumentNullException>(() => Is.NullOrEmpty(obj,selector:null!));
    }

    [Fact]
    public void Value_Null_ShortCircuit_Returns_True()
    {
        WithStrings? obj = null;
        Assert.True(Is.NullOrEmpty(obj, x => x.S1));
    }

    [Fact]
    public void Unary_Conversion_Boxing_Path_Works()
    {
        var obj = new WithStrings { S1 = "x" };
        // Visitor, Convert düğümlerini doğru ele almalı
        Assert.False(Is.NullOrEmpty(obj, x => x.S1!));

        obj = new WithStrings { S1 = "" };
        Assert.True(Is.NullOrEmpty(obj, x => x.S1));
    }

    private sealed class Node
    {
        public Node? Next { get; init; }
        public string? Name { get; init; }
    }

    private sealed class Chain<T>
    {
        public Chain<Chain<T>>? Next { get; init; }
        public string? Name { get; init; }
    }

    private sealed class WithIndexer
    {
        public string? S { get; init; }
        public int this[int i] => i;
    }

    private sealed class HolderOf<T>
    {
        public T? Inner { get; init; }
    }

    [Fact]
    public void SelfReferencingType_DoesNotRecurseForever()
    {
        Assert.True(Is.NullOrEmpty(new HolderOf<Node> { Inner = new Node() }, x => x.Inner!));
        Assert.False(Is.NullOrEmpty(new HolderOf<Node> { Inner = new Node { Next = new Node { Name = "x" } } }, x => x.Inner!));
        Assert.False(Is.NullOrEmpty(new HolderOf<Node> { Inner = new Node { Name = "x" } }, x => x.Inner!));
    }

    [Fact]
    public void RecursivelyGenericType_StopsAtMaxDepth()
    {
        Assert.True(Is.NullOrEmpty(new HolderOf<Chain<int>> { Inner = new Chain<int>() }, x => x.Inner!));
        Assert.False(Is.NullOrEmpty(new HolderOf<Chain<int>> { Inner = new Chain<int> { Name = "x" } }, x => x.Inner!));
    }

    [Fact]
    public void IndexedProperties_AreIgnored()
    {
        Assert.True(Is.NullOrEmpty(new HolderOf<WithIndexer> { Inner = new WithIndexer() }, x => x.Inner!));
        Assert.False(Is.NullOrEmpty(new HolderOf<WithIndexer> { Inner = new WithIndexer { S = "x" } }, x => x.Inner!));
    }

    private sealed class TrackingEnumerable(params int[] items) : IEnumerable<int>
    {
        public int DisposeCount { get; private set; }

        public IEnumerator<int> GetEnumerator() => new Enumerator(this, items);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private sealed class Enumerator(TrackingEnumerable owner, int[] items) : IEnumerator<int>
        {
            private int _index = -1;

            public int Current => items[_index];
            object IEnumerator.Current => Current;
            public bool MoveNext() => ++_index < items.Length;
            public void Reset() => _index = -1;
            public void Dispose() => owner.DisposeCount++;
        }
    }

    [Fact]
    public void Enumerable_Selector_DisposesEnumerator()
    {
        var empty = new TrackingEnumerable();
        var nonEmpty = new TrackingEnumerable(1);

        Assert.True(Is.NullOrEmpty(new HolderOf<TrackingEnumerable> { Inner = empty }, x => x.Inner!));
        Assert.False(Is.NullOrEmpty(new HolderOf<TrackingEnumerable> { Inner = nonEmpty }, x => x.Inner!));

        Assert.True(empty.DisposeCount >= 1);
        Assert.True(nonEmpty.DisposeCount >= 1);
    }

    private struct PlainStruct
    {
        public int X;
    }

    private sealed class WithStructs
    {
        public PlainStruct Plain { get; init; }
        public PlainStruct? NullablePlain { get; init; }
        public (int, string?) Tuple { get; init; }
        public KeyValuePair<string, int> Pair { get; init; }
        public char Char { get; init; }
        public nint NativeInt { get; init; }
        public DayOfWeek Day { get; init; }
        public DayOfWeek? NullableDay { get; init; }
    }

    [Fact]
    public void Structs_WithoutEqualityOperator_AreEmpty_WhenDefault()
    {
        var empty = new WithStructs { NullablePlain = default(PlainStruct) };
        var filled = new WithStructs
        {
            Plain = new PlainStruct { X = 1 },
            NullablePlain = new PlainStruct { X = 1 },
            Tuple = (1, "a"),
            Pair = new KeyValuePair<string, int>("k", 1)
        };

        Assert.True(Is.NullOrEmpty(empty, x => x.Plain));
        Assert.True(Is.NullOrEmpty(empty, x => x.NullablePlain!));
        Assert.True(Is.NullOrEmpty(new WithStructs(), x => x.NullablePlain!));
        Assert.True(Is.NullOrEmpty(empty, x => x.Tuple));
        Assert.True(Is.NullOrEmpty(empty, x => x.Pair));

        Assert.False(Is.NullOrEmpty(filled, x => x.Plain));
        Assert.False(Is.NullOrEmpty(filled, x => x.NullablePlain!));
        Assert.False(Is.NullOrEmpty(filled, x => x.Tuple));
        Assert.False(Is.NullOrEmpty(filled, x => x.Pair));
        Assert.Throws<NullOrEmptyException>(() => ThrowIf.NullOrEmpty(empty, x => x.Plain));
    }

    [Fact]
    public void Enums_And_OtherPrimitives_AreEmpty_WhenDefault_LikeWithoutSelector()
    {
        var empty = new WithStructs { NullableDay = DayOfWeek.Sunday };
        var filled = new WithStructs { Char = 'a', NativeInt = 1, Day = DayOfWeek.Monday, NullableDay = DayOfWeek.Monday };

        Assert.True(Is.NullOrEmpty(empty, x => x.Char));
        Assert.True(Is.NullOrEmpty(empty, x => x.NativeInt));
        Assert.True(Is.NullOrEmpty(empty, x => x.Day));
        Assert.True(Is.NullOrEmpty(empty, x => x.NullableDay!));
        Assert.True(Is.NullOrEmpty(new WithStructs(), x => x.NullableDay!));

        Assert.False(Is.NullOrEmpty(filled, x => x.Char));
        Assert.False(Is.NullOrEmpty(filled, x => x.NativeInt));
        Assert.False(Is.NullOrEmpty(filled, x => x.Day));
        Assert.False(Is.NullOrEmpty(filled, x => x.NullableDay!));

        Assert.Equal(Is.NullOrEmpty(empty.Char), Is.NullOrEmpty(empty, x => x.Char));
        Assert.Equal(Is.NullOrEmpty(empty.NullableDay), Is.NullOrEmpty(empty, x => x.NullableDay!));
    }

    private sealed class WithImmutable
    {
        public ImmutableArray<int> Array { get; init; }
        public ImmutableList<int>? List { get; init; }
    }

    [Fact]
    public void ImmutableCollections_Default_Empty_And_NonEmpty()
    {
        Assert.True(Is.NullOrEmpty(new WithImmutable(), x => x.Array));
        Assert.True(Is.NullOrEmpty(new WithImmutable { Array = ImmutableArray<int>.Empty }, x => x.Array));
        Assert.False(Is.NullOrEmpty(new WithImmutable { Array = [1] }, x => x.Array));
        Assert.Throws<NullOrEmptyException>(() => ThrowIf.NullOrEmpty(new WithImmutable(), x => x.Array));

        Assert.True(Is.NullOrEmpty(new WithImmutable(), x => x.List!));
        Assert.True(Is.NullOrEmpty(new WithImmutable { List = ImmutableList<int>.Empty }, x => x.List!));
        Assert.False(Is.NullOrEmpty(new WithImmutable { List = [1] }, x => x.List!));
    }

    private sealed class WithFrameworkTypes
    {
        public Uri? Link { get; init; }
        public Version? Version { get; init; }
    }

    private sealed class LinkHolder
    {
        public Uri? Link { get; init; }
        public List<string> Tags { get; init; } = [];
    }

    private sealed class WithLinkHolder
    {
        public LinkHolder? Holder { get; init; }
    }

    [Fact]
    public void FrameworkClasses_AreOnlyCheckedForNull()
    {
        var relative = new WithFrameworkTypes { Link = new Uri("/home", UriKind.Relative), Version = new Version() };

        // Uri.AbsolutePath throws for a relative URI, and Version's properties are not data to validate.
        Assert.False(Is.NullOrEmpty(relative, x => x.Link!));
        Assert.False(Is.NullOrEmpty(relative, x => x.Version!));
        ThrowIf.NullOrEmpty(relative, x => x.Link);

        Assert.True(Is.NullOrEmpty(new WithFrameworkTypes(), x => x.Link!));
        Assert.True(Is.NullOrEmpty(new WithFrameworkTypes(), x => x.Version!));
    }

    [Fact]
    public void ComplexType_WithFrameworkClassAndCollectionProperties()
    {
        var empty = new WithLinkHolder { Holder = new LinkHolder() };
        var withRelativeLink = new WithLinkHolder { Holder = new LinkHolder { Link = new Uri("/home", UriKind.Relative) } };
        var withTag = new WithLinkHolder { Holder = new LinkHolder { Tags = ["a"] } };

        Assert.True(Is.NullOrEmpty(empty, x => x.Holder!));
        Assert.False(Is.NullOrEmpty(withRelativeLink, x => x.Holder!));
        Assert.False(Is.NullOrEmpty(withTag, x => x.Holder!));
    }
}
