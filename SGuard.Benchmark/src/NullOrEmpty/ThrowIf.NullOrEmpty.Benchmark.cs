using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;
using SGuard.Exceptions;

namespace SGuard.Benchmark.NullOrEmpty;

[Config(typeof(Config))]
public class ThrowIfNullOrEmptBenchmark
{
    private const string NonEmptyString = "hello";
    private const string EmptyString = "";
    private readonly string? _nullString = null;
    private readonly int[] _nonEmptyArray = { 1, 2, 3 };
    private readonly int[] _emptyArray = [];
    private readonly List<int> _nonEmptyList = [1];
    private readonly List<int> _emptyList = [];
    private SGuardCallback? _callback;

    private readonly int[] _array1000 = Enumerable.Range(1, 1000).ToArray();
    private readonly int[] _array5000 = Enumerable.Range(1, 5000).ToArray();
    private readonly int[] _array10000 = Enumerable.Range(1, 10000).ToArray();
    private readonly int[] _array15000 = Enumerable.Range(1, 15000).ToArray();
    private readonly List<int> _list1000 = Enumerable.Range(1, 1000).ToList();
    private readonly List<int> _list5000 = Enumerable.Range(1, 5000).ToList();
    private readonly List<int> _list10000 = Enumerable.Range(1, 10000).ToList();
    private readonly List<int> _list15000 = Enumerable.Range(1, 15000).ToList();

    [GlobalSetup]
    public void Setup()
    {
        _callback = _ => { };
    }

    // String checks
    [Benchmark]
    public void String_NonEmpty() => ThrowIf.NullOrEmpty(NonEmptyString);

    [Benchmark]
    public void String_Empty() => RunGuard(EmptyString);

    [Benchmark]
    public void String_Null() => RunGuard(_nullString);

    [Benchmark]
    public void String_WithCallback() => ThrowIf.NullOrEmpty(NonEmptyString, _callback);

    // Array checks
    [Benchmark]
    public void Array_NonEmpty() => ThrowIf.NullOrEmpty(_nonEmptyArray);

    [Benchmark]
    public void Array_Empty() => RunGuard(_emptyArray);

    [Benchmark]
    public void Array_WithCallback() => ThrowIf.NullOrEmpty(_nonEmptyArray, _callback);

    // List checks
    [Benchmark]
    public void List_NonEmpty() => ThrowIf.NullOrEmpty(_nonEmptyList);

    [Benchmark]
    public void List_Empty() => RunGuard(_emptyList);

    [Benchmark]
    public void List_WithCallback() => ThrowIf.NullOrEmpty(_nonEmptyList, _callback);

    // Custom exception
    [Benchmark]
    public void String_CustomException() => RunGuard(EmptyString, new NullOrEmptyException("Custom"));

    [Benchmark]
    public void Array_CustomException() => RunGuard(_emptyArray, new NullOrEmptyException("Custom"));

    // Exception with constructor args
    [Benchmark]
    public void String_ExceptionWithArgs() => RunGuardWithArgs(EmptyString, ["Custom"]);

    [Benchmark]
    public void Array_ExceptionWithArgs() => RunGuardWithArgs(_emptyArray, ["Custom"]);

    // Selector expression
    [Benchmark]
    public void Object_Selector_NonEmpty() => ThrowIf.NullOrEmpty(new TestObj { Value = "abc" }, x => x.Value);

    [Benchmark]
    public void Object_Selector_Empty() => RunGuard(new TestObj { Value = "" }, x => x.Value);

    [Benchmark]
    public void Object_Selector_Null() => RunGuard(new TestObj { Value = null }, x => x.Value);

    [Benchmark]
    public void Object_Selector_WithCallback() => ThrowIf.NullOrEmpty(new TestObj { Value = "abc" }, x => x.Value, _callback);

    // Selector with custom exception
    [Benchmark]
    public void Object_Selector_CustomException() => RunGuard(new TestObj { Value = "" }, x => x.Value, new NullOrEmptyException("Custom"));

    // Selector with exception args
    [Benchmark]
    public void Object_Selector_ExceptionWithArgs() =>
        RunGuardWithArgs<TestObj, NullOrEmptyException>(new TestObj { Value = "" }, x => x.Value, new object[] { "Custom" });

    // Array edge cases
    [Benchmark]
    public void Array_1000() => ThrowIf.NullOrEmpty(_array1000);

    [Benchmark]
    public void Array_5000() => ThrowIf.NullOrEmpty(_array5000);

    [Benchmark]
    public void Array_10000() => ThrowIf.NullOrEmpty(_array10000);

    [Benchmark]
    public void Array_15000() => ThrowIf.NullOrEmpty(_array15000);

    [Benchmark]
    public void Array_1000_WithCallback() => ThrowIf.NullOrEmpty(_array1000, _callback);

    [Benchmark]
    public void Array_5000_WithCallback() => ThrowIf.NullOrEmpty(_array5000, _callback);

    [Benchmark]
    public void Array_10000_WithCallback() => ThrowIf.NullOrEmpty(_array10000, _callback);

    [Benchmark]
    public void Array_15000_WithCallback() => ThrowIf.NullOrEmpty(_array15000, _callback);

    // List edge cases
    [Benchmark]
    public void List_1000() => ThrowIf.NullOrEmpty(_list1000);

    [Benchmark]
    public void List_5000() => ThrowIf.NullOrEmpty(_list5000);

    [Benchmark]
    public void List_10000() => ThrowIf.NullOrEmpty(_list10000);

    [Benchmark]
    public void List_15000() => ThrowIf.NullOrEmpty(_list15000);

    [Benchmark]
    public void List_1000_WithCallback() => ThrowIf.NullOrEmpty(_list1000, _callback);

    [Benchmark]
    public void List_5000_WithCallback() => ThrowIf.NullOrEmpty(_list5000, _callback);

    [Benchmark]
    public void List_10000_WithCallback() => ThrowIf.NullOrEmpty(_list10000, _callback);

    [Benchmark]
    public void List_15000_WithCallback() => ThrowIf.NullOrEmpty(_list15000, _callback);

    private static void RunGuard<T>(T value)
    {
        try
        {
            ThrowIf.NullOrEmpty(value);
        }
        catch (Exception) { }
    }

    private static void RunGuard<T, TException>(T value, TException exception) where TException : Exception
    {
        try
        {
            ThrowIf.NullOrEmpty(value, exception);
        }
        catch (Exception) { }
    }

    private static void RunGuard<TValue>(TValue value, Expression<Func<TValue, object?>> selector)
    {
        try
        {
            ThrowIf.NullOrEmpty(value, selector);
        }
        catch (Exception) { }
    }

    private static void RunGuard<TValue, TException>(TValue value, Expression<Func<TValue, object?>> selector, TException exception)
        where TException : Exception
    {
        try
        {
            ThrowIf.NullOrEmpty(value, selector, exception);
        }
        catch (Exception) { }
    }

    private static void RunGuardWithArgs<TValue, TException>(TValue value, Expression<Func<TValue, object?>> selector, object[] args)
        where TException : Exception
    {
        try
        {
            ThrowIf.NullOrEmpty<TValue, TException>(value, selector, args);
        }
        catch (Exception) { }
    }

    private static void RunGuardWithArgs<T>(T value, object[] args)
    {
        try
        {
            ThrowIf.NullOrEmpty<T, NullOrEmptyException>(value, args);
        }
        catch (Exception) { }
    }

    public sealed class TestObj
    {
        public string? Value { get; set; }
    }
}