using BenchmarkDotNet.Attributes;

namespace SGuard.Benchmark.Between;

[Config(typeof(Config))]
public class ThrowIfBetweenBenchmark
{
    private const int MinInt = 10;
    private const int MaxInt = 20;
    private const int BelowInt = 5;
    private const int AtMinInt = 10;
    private const int BetweenInt = 15;
    private const int AtMaxInt = 20;
    private const int AboveInt = 25;

    private const double MinDouble = 10.0;
    private const double MaxDouble = 20.0;
    private const double BelowDouble = 5.0;
    private const double AtMinDouble = 10.0;
    private const double BetweenDouble = 15.0;
    private const double AtMaxDouble = 20.0;
    private const double AboveDouble = 25.0;

    private const string MinStr = "apple";
    private const string MaxStr = "orange";
    private const string BelowStr = "ant";
    private const string AtMinStr = "apple";
    private const string BetweenStr = "banana";
    private const string AtMaxStr = "orange";
    private const string AboveStr = "pear";
    private const string EmptyStr = "";
    private const string WhitespaceStr = "   ";
    private readonly string _nullStr = null!;

    private SGuardCallback _callback = outcome => { };

    private sealed class CustomException : Exception
    {
        public CustomException() : base("Custom") { }
        public CustomException(string msg) : base(msg) { }
    }

    [GlobalSetup]
    public void Setup()
    {
        _callback = outcome => { };
    }

    // Int benchmarks
    [Benchmark]
    public void Int_Below()
    {
        try
        {
            ThrowIf.Between(BelowInt, MinInt, MaxInt);
        }
        catch { }
    }

    [Benchmark]
    public void Int_AtMin()
    {
        try
        {
            ThrowIf.Between(AtMinInt, MinInt, MaxInt);
        }
        catch { }
    }

    [Benchmark]
    public void Int_Between()
    {
        try
        {
            ThrowIf.Between(BetweenInt, MinInt, MaxInt);
        }
        catch { }
    }

    [Benchmark]
    public void Int_AtMax()
    {
        try
        {
            ThrowIf.Between(AtMaxInt, MinInt, MaxInt);
        }
        catch { }
    }

    [Benchmark]
    public void Int_Above()
    {
        try
        {
            ThrowIf.Between(AboveInt, MinInt, MaxInt);
        }
        catch { }
    }

    [Benchmark]
    public void Int_Between_WithCallback()
    {
        try
        {
            ThrowIf.Between(BetweenInt, MinInt, MaxInt, _callback);
        }
        catch { }
    }

    [Benchmark]
    public void Int_Between_CustomException()
    {
        try
        {
            ThrowIf.Between(BetweenInt, MinInt, MaxInt, new CustomException());
        }
        catch { }
    }

    [Benchmark]
    public void Int_Between_CustomException_WithCallback()
    {
        try
        {
            ThrowIf.Between(BetweenInt, MinInt, MaxInt, new CustomException(), _callback);
        }
        catch { }
    }

    // Double benchmarks
    [Benchmark]
    public void Double_Below()
    {
        try
        {
            ThrowIf.Between(BelowDouble, MinDouble, MaxDouble);
        }
        catch { }
    }

    [Benchmark]
    public void Double_AtMin()
    {
        try
        {
            ThrowIf.Between(AtMinDouble, MinDouble, MaxDouble);
        }
        catch { }
    }

    [Benchmark]
    public void Double_Between()
    {
        try
        {
            ThrowIf.Between(BetweenDouble, MinDouble, MaxDouble);
        }
        catch { }
    }

    [Benchmark]
    public void Double_AtMax()
    {
        try
        {
            ThrowIf.Between(AtMaxDouble, MinDouble, MaxDouble);
        }
        catch { }
    }

    [Benchmark]
    public void Double_Above()
    {
        try
        {
            ThrowIf.Between(AboveDouble, MinDouble, MaxDouble);
        }
        catch { }
    }

    [Benchmark]
    public void Double_Between_WithCallback()
    {
        try
        {
            ThrowIf.Between(BetweenDouble, MinDouble, MaxDouble, _callback);
        }
        catch { }
    }

    [Benchmark]
    public void Double_Between_CustomException()
    {
        try
        {
            ThrowIf.Between(BetweenDouble, MinDouble, MaxDouble, new CustomException());
        }
        catch { }
    }

    [Benchmark]
    public void Double_Between_CustomException_WithCallback()
    {
        try
        {
            ThrowIf.Between(BetweenDouble, MinDouble, MaxDouble, new CustomException(), _callback);
        }
        catch { }
    }

    // String benchmarks
    [Benchmark]
    public void String_Below()
    {
        try
        {
            ThrowIf.Between(BelowStr, MinStr, MaxStr, StringComparison.Ordinal);
        }
        catch { }
    }

    [Benchmark]
    public void String_AtMin()
    {
        try
        {
            ThrowIf.Between(AtMinStr, MinStr, MaxStr, StringComparison.Ordinal);
        }
        catch { }
    }

    [Benchmark]
    public void String_Between()
    {
        try
        {
            ThrowIf.Between(BetweenStr, MinStr, MaxStr, StringComparison.Ordinal);
        }
        catch { }
    }

    [Benchmark]
    public void String_AtMax()
    {
        try
        {
            ThrowIf.Between(AtMaxStr, MinStr, MaxStr, StringComparison.Ordinal);
        }
        catch { }
    }

    [Benchmark]
    public void String_Above()
    {
        try
        {
            ThrowIf.Between(AboveStr, MinStr, MaxStr, StringComparison.Ordinal);
        }
        catch { }
    }

    [Benchmark]
    public void String_Empty()
    {
        try
        {
            ThrowIf.Between(EmptyStr, MinStr, MaxStr, StringComparison.Ordinal);
        }
        catch { }
    }

    [Benchmark]
    public void String_Whitespace()
    {
        try
        {
            ThrowIf.Between(WhitespaceStr, MinStr, MaxStr, StringComparison.Ordinal);
        }
        catch { }
    }

    [Benchmark]
    public void String_Null()
    {
        try
        {
            ThrowIf.Between(_nullStr, MinStr, MaxStr, StringComparison.Ordinal);
        }
        catch { }
    }

    [Benchmark]
    public void String_Between_WithCallback()
    {
        try
        {
            ThrowIf.Between(BetweenStr, MinStr, MaxStr, StringComparison.Ordinal, _callback);
        }
        catch { }
    }

    [Benchmark]
    public void String_Between_CustomException()
    {
        try
        {
            ThrowIf.Between(BetweenStr, MinStr, MaxStr, StringComparison.Ordinal, new CustomException());
        }
        catch { }
    }

    [Benchmark]
    public void String_Between_CustomException_WithCallback()
    {
        try
        {
            ThrowIf.Between(BetweenStr, MinStr, MaxStr, StringComparison.Ordinal, new CustomException(), _callback);
        }
        catch { }
    }

    [Benchmark]
    public void String_Between_OrdinalIgnoreCase()
    {
        try
        {
            ThrowIf.Between(BetweenStr, MinStr, MaxStr, StringComparison.OrdinalIgnoreCase);
        }
        catch { }
    }

    [Benchmark]
    public void String_Between_InvariantCulture()
    {
        try
        {
            ThrowIf.Between(BetweenStr, MinStr, MaxStr, StringComparison.InvariantCulture);
        }
        catch { }
    }

    [Benchmark]
    public void String_Between_InvariantCultureIgnoreCase()
    {
        try
        {
            ThrowIf.Between(BetweenStr, MinStr, MaxStr, StringComparison.InvariantCultureIgnoreCase);
        }
        catch { }
    }
}