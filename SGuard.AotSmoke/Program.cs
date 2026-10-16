using SGuard;
using SGuard.Exceptions;

// Runs each guard once passing and once failing in a Native AOT binary. Exits with 1 and lists the checks that failed.
var failures = new List<string>();

void Check(string name, bool condition)
{
    if (!condition)
    {
        failures.Add(name);
    }
}

void Throws<TException>(string name, Action guard) where TException : Exception
{
    try
    {
        guard();
        failures.Add($"{name}: didn't throw");
    }
    catch (TException)
    {
    }
    catch (Exception ex)
    {
        failures.Add($"{name}: threw {ex.GetType().Name}: {ex.Message}");
    }
}

string? missing = null;
int? count = 3;

Check("Null", ThrowIf.Null("a") == "a" && ThrowIf.Null(count) == 3);
Throws<NullException>("Null fails", () => ThrowIf.Null(missing));
Check("NullOrEmpty", ThrowIf.NullOrEmpty("a") == "a" && ThrowIf.NullOrEmpty(new[] { 1 }).Length == 1);
Throws<NullOrEmptyException>("NullOrEmpty fails", () => ThrowIf.NullOrEmpty(Guid.Empty));
Throws<NullOrEmptyException>("NullOrEmpty default struct", () => ThrowIf.NullOrEmpty((object)default(DateOnly)));
Throws<NullOrEmptyException>("NullOrEmpty boxed tuple", () => ThrowIf.NullOrEmpty((object)(0, (string?)null)));
Throws<NullOrEmptyException>("NullOrEmpty memory", () => ThrowIf.NullOrEmpty(new int[2].AsMemory(2)));
Throws<NullOrEmptyException>("NullOrEmpty boxed memory", () => ThrowIf.NullOrEmpty((object)new long[2].AsMemory(2)));
Check("NullOrEmpty boxed memory with elements", !Is.NullOrEmpty((object)new long[2].AsMemory()));
Throws<NullOrEmptyException>("NullOrEmpty enumerable", () => ThrowIf.NullOrEmpty(Enumerable.Empty<int>().Select(x => x)));
Throws<NullOrWhiteSpaceException>("NullOrWhiteSpace", () => ThrowIf.NullOrWhiteSpace(" "));
Check("GreaterThan", ThrowIf.GreaterThan(1, 2) == 1);
Throws<GreaterThanException>("GreaterThan fails", () => ThrowIf.GreaterThan(3m, 2m));
Throws<LessThanException>("LessThan NaN", () => ThrowIf.LessThan(double.NaN, 1d));
Throws<LessThanOrEqualException>("LessThanOrEqual", () => ThrowIf.LessThanOrEqual(TimeSpan.Zero, TimeSpan.Zero));
Throws<GreaterThanOrEqualException>("GreaterThanOrEqual", () => ThrowIf.GreaterThanOrEqual(new Version(2, 0), new Version(1, 0)));
Throws<BetweenException>("Between", () => ThrowIf.Between(5, 1, 10));
Throws<OutOfRangeException>("OutOfRange", () => ThrowIf.OutOfRange(50, 1, 10));
Throws<ArgumentException>("Between invalid range", () => Is.Between(5, 10, 1));
Throws<OutOfRangeException>("OutOfRange string", () => ThrowIf.OutOfRange("M", "a", "z", StringComparison.Ordinal));
Throws<NegativeException>("Negative", () => ThrowIf.Negative(-1));
Throws<NegativeOrZeroException>("NegativeOrZero", () => ThrowIf.NegativeOrZero(0m));
Throws<ZeroException>("Zero", () => ThrowIf.Zero(0d));
Throws<EqualException>("Equal", () => ThrowIf.Equal("a", "A", StringComparison.OrdinalIgnoreCase));
Throws<NotEqualException>("NotEqual", () => ThrowIf.NotEqual(1, 2));
Throws<NotDefinedException>("NotDefined", () => ThrowIf.NotDefined((DayOfWeek)42));
Throws<LongerThanException>("LongerThan", () => ThrowIf.LongerThan("abcd", 3));
Throws<ShorterThanException>("ShorterThan", () => ThrowIf.ShorterThan("ab", 3));
Throws<AnyException>("Any", () => ThrowIf.Any(new List<int> { 1, 5 }, x => x > 3));
Throws<AllException>("All", () => ThrowIf.All(new[] { 4, 5 }, x => x > 3));
Throws<InvalidOperationException>("Factory", () => ThrowIf.GreaterThan(2, 1, () => new InvalidOperationException()));
Throws<InvalidOperationException>("new()", () => ThrowIf.GreaterThan<int, int, InvalidOperationException>(2, 1));
Throws<ArgumentException>("Obsolete constructor arguments", () => ThrowIf.GreaterThan<int, int, ArgumentException>(2, 1, ["message"]));
Check("Email", Is.Email("someone@example.com") && !Is.Email("someone@"));
Check("Url", Is.Url("https://example.com") && !Is.Url("javascript:alert(1)"));
Check("CreditCard", Is.CreditCard("4111 1111 1111 1111") && !Is.CreditCard("4111 1111 1111 1112"));
Check("Match", Is.Match("EUR", "^[A-Z]{3}$") && !Is.Match("eur", "^[A-Z]{3}$"));

SGuardOptions.IncludeValuesInExceptions = true;
Throws<GreaterThanException>("Values in messages", () => ThrowIf.GreaterThan(new DateOnly(2026, 1, 2), new DateOnly(2026, 1, 1)));

var outcomes = new List<GuardOutcome>();
ThrowIf.LessThan(2, 1, outcomes.Add);
Is.Between(0, 1, 2, SGuardCallbacks.OnFailure(() => outcomes.Add(GuardOutcome.Failure)));
Check("Callbacks", outcomes is [GuardOutcome.Success, GuardOutcome.Failure]);

if (failures.Count > 0)
{
    Console.Error.WriteLine($"{failures.Count} checks failed under Native AOT:");
    failures.ForEach(f => Console.Error.WriteLine($"  {f}"));
    return 1;
}

Console.WriteLine("All Native AOT checks passed.");
return 0;
