using BenchmarkDotNet.Attributes;

namespace SGuard.Benchmark.NullOrEmpty;
[Config(typeof(Config))]
public class IsNullOrEmptyBenchmark
{
    private string emptyStr = "";
    private string nonEmptyStr = "hello";
    private int[] emptyArr = new int[0];
    private int[] nonEmptyArr = new[] { 1, 2, 3 };
    private List<int> emptyList = new();
    private List<int> nonEmptyList = new() { 1 };
    private object? nullObj = null;
    private int zero = 0;
    private int nonZero = 42;
    private SGuardCallback callback;

    private string whitespaceStr = "   ";
    private Guid emptyGuid = Guid.Empty;
    private Guid nonEmptyGuid = Guid.NewGuid();
    private DateTime emptyDateTime = new DateTime(0);
    private DateTime nonEmptyDateTime = DateTime.Now;
    private TimeSpan emptyTimeSpan = new TimeSpan(0);
    private TimeSpan nonEmptyTimeSpan = TimeSpan.FromMinutes(5);
    private DateOnly emptyDateOnly = DateOnly.MinValue;
    private DateOnly nonEmptyDateOnly = DateOnly.FromDateTime(DateTime.Now);
    private TimeOnly emptyTimeOnly = TimeOnly.MinValue;
    private TimeOnly nonEmptyTimeOnly = TimeOnly.FromDateTime(DateTime.Now);
    private DateTimeOffset emptyDateTimeOffset = new DateTimeOffset(0, TimeSpan.Zero);
    private DateTimeOffset nonEmptyDateTimeOffset = DateTimeOffset.Now;
    private Dictionary<int, int> emptyDict = new();
    private Dictionary<int, int> singleDict = new() { { 1, 1 } };
    private Dictionary<int, int> multiDict = Enumerable.Range(1, 1000).ToDictionary(x => x, x => x);
    private IReadOnlyCollection<int> emptyReadOnlyColl = new List<int>();
    private IReadOnlyCollection<int> singleReadOnlyColl = new List<int> { 1 };
    private IReadOnlyCollection<int> multiReadOnlyColl = Enumerable.Range(1, 1000).ToList();
    private IEnumerable<int> emptyEnumerable = Enumerable.Empty<int>();
    private IEnumerable<int> singleEnumerable = new List<int> { 1 };
    private IEnumerable<int> multiEnumerable = Enumerable.Range(1, 1000);
    private int[] arr1 = new int[1];
    private int[] arr10 = Enumerable.Range(1, 10).ToArray();
    private int[] arr1000 = Enumerable.Range(1, 1000).ToArray();
    private int[] arr10000 = Enumerable.Range(1, 10000).ToArray();
    private List<int> list1 = new() { 1 };
    private List<int> list10 = Enumerable.Range(1, 10).ToList();
    private List<int> list1000 = Enumerable.Range(1, 1000).ToList();
    private List<int> list10000 = Enumerable.Range(1, 10000).ToList();

    private class ComplexTypeAllNull { public string? A = null; public int[]? B = null; }
    private class ComplexTypeSomeNonNull { public string? A = "x"; public int[]? B = null; }
    private class ComplexTypeAllNonNull { public string? A = "x"; public int[]? B = new[] { 1 }; }
    private ComplexTypeAllNull complexAllNull = new();
    private ComplexTypeSomeNonNull complexSomeNonNull = new();
    private ComplexTypeAllNonNull complexAllNonNull = new();

    [GlobalSetup]
    public void Setup() { callback = outcome => { }; }

    [Benchmark] public bool NullObj() => Is.NullOrEmpty(nullObj);
    [Benchmark] public bool EmptyString() => Is.NullOrEmpty(emptyStr);
    [Benchmark] public bool NonEmptyString() => Is.NullOrEmpty(nonEmptyStr);
    [Benchmark] public bool EmptyArray() => Is.NullOrEmpty(emptyArr);
    [Benchmark] public bool NonEmptyArray() => Is.NullOrEmpty(nonEmptyArr);
    [Benchmark] public bool EmptyList() => Is.NullOrEmpty(emptyList);
    [Benchmark] public bool NonEmptyList() => Is.NullOrEmpty(nonEmptyList);
    [Benchmark] public bool ZeroInt() => Is.NullOrEmpty(zero);
    [Benchmark] public bool NonZeroInt() => Is.NullOrEmpty(nonZero);
    [Benchmark] public bool NullObj_WithCallback() => Is.NullOrEmpty(nullObj, callback);
    [Benchmark] public bool EmptyString_WithCallback() => Is.NullOrEmpty(emptyStr, callback);
    [Benchmark] public bool NonEmptyString_WithCallback() => Is.NullOrEmpty(nonEmptyStr, callback);
    [Benchmark] public bool EmptyArray_WithCallback() => Is.NullOrEmpty(emptyArr, callback);
    [Benchmark] public bool NonEmptyArray_WithCallback() => Is.NullOrEmpty(nonEmptyArr, callback);
    [Benchmark] public bool EmptyList_WithCallback() => Is.NullOrEmpty(emptyList, callback);
    [Benchmark] public bool NonEmptyList_WithCallback() => Is.NullOrEmpty(nonEmptyList, callback);
    [Benchmark] public bool ZeroInt_WithCallback() => Is.NullOrEmpty(zero, callback);
    [Benchmark] public bool NonZeroInt_WithCallback() => Is.NullOrEmpty(nonZero, callback);
    [Benchmark] public bool WhitespaceString() => Is.NullOrEmpty(whitespaceStr);
    [Benchmark] public bool EmptyGuid() => Is.NullOrEmpty(emptyGuid);
    [Benchmark] public bool NonEmptyGuid() => Is.NullOrEmpty(nonEmptyGuid);
    [Benchmark] public bool EmptyDateTime() => Is.NullOrEmpty(emptyDateTime);
    [Benchmark] public bool NonEmptyDateTime() => Is.NullOrEmpty(nonEmptyDateTime);
    [Benchmark] public bool EmptyTimeSpan() => Is.NullOrEmpty(emptyTimeSpan);
    [Benchmark] public bool NonEmptyTimeSpan() => Is.NullOrEmpty(nonEmptyTimeSpan);
    [Benchmark] public bool EmptyDateOnly() => Is.NullOrEmpty(emptyDateOnly);
    [Benchmark] public bool NonEmptyDateOnly() => Is.NullOrEmpty(nonEmptyDateOnly);
    [Benchmark] public bool EmptyTimeOnly() => Is.NullOrEmpty(emptyTimeOnly);
    [Benchmark] public bool NonEmptyTimeOnly() => Is.NullOrEmpty(nonEmptyTimeOnly);
    [Benchmark] public bool EmptyDateTimeOffset() => Is.NullOrEmpty(emptyDateTimeOffset);
    [Benchmark] public bool NonEmptyDateTimeOffset() => Is.NullOrEmpty(nonEmptyDateTimeOffset);
    [Benchmark] public bool EmptyDict() => Is.NullOrEmpty(emptyDict);
    [Benchmark] public bool SingleDict() => Is.NullOrEmpty(singleDict);
    [Benchmark] public bool MultiDict() => Is.NullOrEmpty(multiDict);
    [Benchmark] public bool EmptyReadOnlyColl() => Is.NullOrEmpty(emptyReadOnlyColl);
    [Benchmark] public bool SingleReadOnlyColl() => Is.NullOrEmpty(singleReadOnlyColl);
    [Benchmark] public bool MultiReadOnlyColl() => Is.NullOrEmpty(multiReadOnlyColl);
    [Benchmark] public bool EmptyEnumerable() => Is.NullOrEmpty(emptyEnumerable);
    [Benchmark] public bool SingleEnumerable() => Is.NullOrEmpty(singleEnumerable);
    [Benchmark] public bool MultiEnumerable() => Is.NullOrEmpty(multiEnumerable);
    [Benchmark] public bool Array1() => Is.NullOrEmpty(arr1);
    [Benchmark] public bool Array10() => Is.NullOrEmpty(arr10);
    [Benchmark] public bool Array1000() => Is.NullOrEmpty(arr1000);
    [Benchmark] public bool Array10000() => Is.NullOrEmpty(arr10000);
    [Benchmark] public bool List1() => Is.NullOrEmpty(list1);
    [Benchmark] public bool List10() => Is.NullOrEmpty(list10);
    [Benchmark] public bool List1000() => Is.NullOrEmpty(list1000);
    [Benchmark] public bool List10000() => Is.NullOrEmpty(list10000);
    [Benchmark] public bool ComplexAllNull() => Is.NullOrEmpty(complexAllNull);
    [Benchmark] public bool ComplexSomeNonNull() => Is.NullOrEmpty(complexSomeNonNull);
    [Benchmark] public bool ComplexAllNonNull() => Is.NullOrEmpty(complexAllNonNull);
}
