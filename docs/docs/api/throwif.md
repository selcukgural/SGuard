---
sidebar_position: 1
---

# ThrowIf API

Complete reference for exception-throwing guard methods.

## Overview

`ThrowIf` provides guard methods that throw when their condition is **true**: `ThrowIf.LessThan(x, 0)` throws if
`x < 0`, `ThrowIf.NullOrEmpty(v)` throws if `v` is null or empty. Read every `ThrowIf.X(a, b)` as "throw if `a` is X
`b`".

When the guard passes, it **returns the checked value** (the left operand for comparisons), so it can initialize a
field or property:

```csharp
public sealed class Product(string sku, decimal price, int stock)
{
    public string Sku { get; } = ThrowIf.NullOrEmpty(sku);
    public decimal Price { get; } = ThrowIf.NegativeOrZero(price);
    public int Stock { get; } = ThrowIf.Negative(stock);
}
```

`ThrowIf.Any`, `ThrowIf.All` and the `Throw.*` helpers return `void`. Returning values is new in 1.0; see
[Upgrading to 1.0](../migration/upgrading-to-1.0#return-values).

Each guard comes in several variants:

- **Built-in exception**: throws one of the exceptions in `SGuard.Exceptions`. These overloads capture the caller's
  argument expressions with `[CallerArgumentExpression]` parameters, which you don't pass yourself.
- **Factory** (recommended for custom exceptions): `Func<TException> exceptionFactory` runs only when the guard fails.
  Every guard has one.
- **Exception instance**: `TException exception` is thrown as-is. The guards that existed before 1.0 have it.
- **`new()`**: `TException` is created with its parameterless constructor. Available on `NullOrEmpty` with a selector,
  `GreaterThan`, `GreaterThanOrEqual`, `LessThanOrEqual` and `Between`.
- **Constructor arguments** (obsolete, `SGUARD001`): `TException` is created from `object[]? constructorArgs` with
  reflection. Use a factory instead; see [Obsolete APIs](../migration/upgrading-to-1.0#obsolete-apis).

Every overload takes an optional `SGuardCallback? callback = null`; see [Callbacks API](./callbacks).

The results and arguments are annotated for nullable reference types: after `ThrowIf.NullOrEmpty(name)`, both `name`
and the returned value are known not to be null.

### Exception messages and checked values

Built-in exceptions name the argument expressions from the call site, but leave the checked values out, because
messages usually end up in logs and error trackers:

```csharp
ThrowIf.GreaterThan(request.Age, limit);
// GreaterThanException: Left value is greater than right value. Actual: left=request.Age, right=limit.

ThrowIf.NullOrEmpty(request.Name);
// NullOrEmptyException: Value 'request.Name' is null or empty.

ThrowIf.NullOrEmpty(order, o => o.Customer.Email);
// NullOrEmptyException: Value 'o => o.Customer.Email' is null or empty.

ThrowIf.OutOfRange(port, 1, 65535);
// OutOfRangeException: Value is outside the specified bounds. Actual: value=port, min=1, max=65535.

ThrowIf.Negative(balance);
// NegativeException: Value 'balance' is negative.

ThrowIf.LongerThan(name, MaxNameLength);
// LongerThanException: Value 'name' is longer than 'MaxNameLength' characters.
```

To include the values, set the option once at startup. Each value is written with `ToString()` and truncated to 64
characters, and `Exception.Data` holds these strings (keys such as `left`, `right`, `value`, `min`, `max`, `length`):

```csharp
SGuardOptions.IncludeValuesInExceptions = true;
// GreaterThanException: '4217' is greater than '1000'. Actual: left=request.Age, right=limit.
```

Only enable it when the guarded values can't be passwords, tokens or personal data. The length guards never include
the string itself, only its length.

## Null and empty

### NullOrEmpty

Throws if the value is null or empty. "Empty" follows the same rules as
[`Is.NullOrEmpty`](./is#what-counts-as-empty): `null`, `default(T)` (including `0`, `false`, `Guid.Empty`), `""`,
empty collections and `Memory<T>`, zero-tick dates and times; with a selector, `null` anywhere on the path counts as
empty.

:::tip
`0`, `false` and `Guid.Empty` count as empty. When they are valid values, use [`ThrowIf.Null`](#null) or a
[sign guard](#sign-guards) instead.
:::

```csharp
// Built-in NullOrEmptyException; returns the value
T ThrowIf.NullOrEmpty<T>(T value, SGuardCallback? callback = null,
    [CallerArgumentExpression(nameof(value))] string? valueExpression = null);

ReadOnlySpan<T> ThrowIf.NullOrEmpty<T>(ReadOnlySpan<T> value, SGuardCallback? callback = null,
    [CallerArgumentExpression(nameof(value))] string? valueExpression = null);

TValue ThrowIf.NullOrEmpty<TValue>(TValue? value, Expression<Func<TValue, object?>> selector,
    SGuardCallback? callback = null,
    [CallerArgumentExpression(nameof(selector))] string? selectorExpression = null);

// Factory
T ThrowIf.NullOrEmpty<T, TException>(T value, Func<TException> exceptionFactory, SGuardCallback? callback = null)
    where TException : Exception;

ReadOnlySpan<T> ThrowIf.NullOrEmpty<T, TException>(ReadOnlySpan<T> value, Func<TException> exceptionFactory,
    SGuardCallback? callback = null)
    where TException : Exception;

TValue ThrowIf.NullOrEmpty<TValue, TException>(TValue? value, Expression<Func<TValue, object?>> selector,
    Func<TException> exceptionFactory, SGuardCallback? callback = null)
    where TException : Exception;

// Exception instance, new()
T ThrowIf.NullOrEmpty<T, TException>(T value, TException exception, SGuardCallback? callback = null)
    where TException : Exception;

TValue ThrowIf.NullOrEmpty<TValue, TException>(TValue? value, Expression<Func<TValue, object?>> selector,
    TException exception, SGuardCallback? callback = null)
    where TException : Exception;

TValue ThrowIf.NullOrEmpty<TValue, TException>(TValue? value, Expression<Func<TValue, object?>> selector,
    SGuardCallback? callback = null)
    where TException : Exception, new();
```

The `ReadOnlySpan<T>` overloads throw only for an empty span; as with arrays and collections, a span whose elements are
all `null` is not empty. A `null` value throws `NullOrEmptyException` like an empty one. A `null` selector, factory or
exception instance throws `ArgumentNullException`. With a selector, the guard returns the root value, not the selected
member.

:::warning Selectors and Native AOT
Selectors are compiled at run time and read properties with reflection. They don't work under Native AOT, and the
overloads are marked `[RequiresUnreferencedCode]` and `[RequiresDynamicCode]`, so trimmed and AOT apps get a warning.
Check the member directly instead: `ThrowIf.NullOrEmpty(order.Customer?.Email)`.
:::

```csharp
var name = ThrowIf.NullOrEmpty(request.Name);
ThrowIf.NullOrEmpty(items);
ThrowIf.NullOrEmpty(order, o => o.Customer.Email);
ThrowIf.NullOrEmpty(value, () => new DomainException("A value is required."));
ThrowIf.NullOrEmpty<Order, InvalidOperationException>(order, o => o.Customer);
```

### Null

Throws `NullException` if the value is `null`. Unlike `NullOrEmpty`, `""`, empty collections, `0` and other default
values pass. For a nullable value type, the guard returns the underlying value.

```csharp
T ThrowIf.Null<T>(T value, SGuardCallback? callback = null,
    [CallerArgumentExpression(nameof(value))] string? valueExpression = null);

T ThrowIf.Null<T>(T? value, SGuardCallback? callback = null,
    [CallerArgumentExpression(nameof(value))] string? valueExpression = null) where T : struct;

T ThrowIf.Null<T, TException>(T value, Func<TException> exceptionFactory, SGuardCallback? callback = null)
    where TException : Exception;

T ThrowIf.Null<T, TException>(T? value, Func<TException> exceptionFactory, SGuardCallback? callback = null)
    where T : struct where TException : Exception;
```

```csharp
_repository = ThrowIf.Null(repository);
int retries = ThrowIf.Null(options.Retries); // int? -> int
```

### NullOrWhiteSpace

Throws `NullOrWhiteSpaceException` if the string is `null`, empty or only white space (as
`string.IsNullOrWhiteSpace` defines it).

```csharp
string ThrowIf.NullOrWhiteSpace(string? value, SGuardCallback? callback = null,
    [CallerArgumentExpression(nameof(value))] string? valueExpression = null);

string ThrowIf.NullOrWhiteSpace<TException>(string? value, Func<TException> exceptionFactory,
    SGuardCallback? callback = null)
    where TException : Exception;
```

## Comparison guards

All comparison guards throw when the comparison is true, or when either operand is a floating-point NaN (`double`,
`float`, `Half`, `NFloat`). A `null` operand throws `ArgumentNullException`. They return the left value.

The constraint is `TLeft : IComparable<TRight>`, so `ThrowIf.LessThan(price, 0)` with a `decimal` price doesn't
compile; write `ThrowIf.LessThan(price, 0m)`, or use [`ThrowIf.Negative(price)`](#sign-guards). There are no
`StringComparison` overloads for these four; use [`Is.*`](./is#comparison-guards) with a `StringComparison`, or
`ThrowIf.OutOfRange`.

| Guard | Throws when |
|---|---|
| `LessThan` | `lValue < rValue` |
| `LessThanOrEqual` | `lValue <= rValue` |
| `GreaterThan` | `lValue > rValue` |
| `GreaterThanOrEqual` | `lValue >= rValue` |

`GreaterThan` and `LessThan` are strict: `ThrowIf.GreaterThan(10, 10)` doesn't throw.

The overloads, shown for `GreaterThan`:

```csharp
TLeft ThrowIf.GreaterThan<TLeft, TRight>(TLeft lValue, TRight rValue, SGuardCallback? callback = null,
    [CallerArgumentExpression(nameof(lValue))] string? lValueExpression = null,
    [CallerArgumentExpression(nameof(rValue))] string? rValueExpression = null)
    where TLeft : IComparable<TRight>;

TLeft ThrowIf.GreaterThan<TLeft, TRight, TException>(TLeft lValue, TRight rValue, Func<TException> exceptionFactory,
    SGuardCallback? callback = null)
    where TLeft : IComparable<TRight> where TException : Exception;

TLeft ThrowIf.GreaterThan<TLeft, TRight, TException>(TLeft lValue, TRight rValue, TException exception,
    SGuardCallback? callback = null)
    where TLeft : IComparable<TRight> where TException : Exception;

TLeft ThrowIf.GreaterThan<TLeft, TRight, TException>(TLeft lValue, TRight rValue, SGuardCallback? callback = null)
    where TLeft : IComparable<TRight> where TException : Exception, new();
```

`GreaterThanOrEqual` and `LessThanOrEqual` have the same overloads. `LessThan` has all but the `new()` one.

```csharp
ThrowIf.LessThan(age, 0);
var pageSize = ThrowIf.GreaterThan(request.PageSize, 100);
ThrowIf.GreaterThan(amount, balance, () => new InsufficientFundsException(accountId));
ThrowIf.GreaterThan<int, int, RateLimitExceededException>(requests, limit);
```

## Sign guards

For any number type (`int`, `long`, `decimal`, `double`, `Half`, `BigInteger`, `Int128`, ...): the constraint is
`INumber<T>` (`INumberBase<T>` for `Zero`). They return the value.

| Guard | Throws when | Exception |
|---|---|---|
| `Negative` | `value < 0` or NaN | `NegativeException` |
| `NegativeOrZero` | `value <= 0` or NaN | `NegativeOrZeroException` |
| `Zero` | `value == 0` | `ZeroException` |

Negative zero (`-0.0`) equals zero: it passes `Negative` and fails `NegativeOrZero` and `Zero`. NaN fails `Negative` and
`NegativeOrZero`, like the other comparisons, and passes `Zero`.

```csharp
T ThrowIf.Negative<T>(T value, SGuardCallback? callback = null,
    [CallerArgumentExpression(nameof(value))] string? valueExpression = null) where T : INumber<T>;

T ThrowIf.Negative<T, TException>(T value, Func<TException> exceptionFactory, SGuardCallback? callback = null)
    where T : INumber<T> where TException : Exception;

// NegativeOrZero: the same two overloads. Zero: the same, with T : INumberBase<T>.
```

```csharp
var quantity = ThrowIf.NegativeOrZero(request.Quantity);
ThrowIf.Negative(balance, () => new OverdrawnException(accountId));
ThrowIf.Zero(divisor);
```

## Range guards

### OutOfRange

Throws `OutOfRangeException` if the value is **outside** the range: `value < min` or `value > max`. Both bounds are
inclusive, so `min` and `max` themselves pass. A NaN operand fails, and `min > max` throws `ArgumentException`. Returns
the value.

```csharp
TValue ThrowIf.OutOfRange<TValue, TMin, TMax>(TValue value, TMin min, TMax max, SGuardCallback? callback = null,
    [CallerArgumentExpression(nameof(value))] string? valueExpression = null,
    [CallerArgumentExpression(nameof(min))] string? minExpression = null,
    [CallerArgumentExpression(nameof(max))] string? maxExpression = null)
    where TValue : IComparable<TMin>, IComparable<TMax>;

TValue ThrowIf.OutOfRange<TValue, TMin, TMax, TException>(TValue value, TMin min, TMax max,
    Func<TException> exceptionFactory, SGuardCallback? callback = null)
    where TValue : IComparable<TMin>, IComparable<TMax> where TException : Exception;

string ThrowIf.OutOfRange(string value, string min, string max, StringComparison comparison,
    SGuardCallback? callback = null, /* caller argument expressions */);

string ThrowIf.OutOfRange<TException>(string value, string min, string max, StringComparison comparison,
    Func<TException> exceptionFactory, SGuardCallback? callback = null)
    where TException : Exception;
```

```csharp
var port = ThrowIf.OutOfRange(options.Port, 1, 65535);
ThrowIf.OutOfRange(timeout, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(2));
```

### Between

Throws `BetweenException` if `min <= value <= max` (inclusive), i.e. when the value **is** inside the range: the
opposite of `OutOfRange`. Throws `ArgumentException` if `min` is greater than `max`. Returns the value.

:::warning
`ThrowIf.Between` is not a range check. To reject values *outside* a range, use [`ThrowIf.OutOfRange`](#outofrange).
:::

It has the same overloads as the comparison guards (factory, instance, `new()`), for generic values and for strings
with a `StringComparison`:

```csharp
TValue ThrowIf.Between<TValue, TMin, TMax>(TValue value, TMin min, TMax max, SGuardCallback? callback = null,
    /* caller argument expressions */)
    where TValue : IComparable<TMin>, IComparable<TMax>;

string ThrowIf.Between(string value, string min, string max, StringComparison comparison,
    SGuardCallback? callback = null, /* caller argument expressions */);
```

```csharp
ThrowIf.Between(statusCode, 500, 599);                      // throws for a server error
ThrowIf.Between("m", "a", "z", StringComparison.Ordinal);   // throws: "m" is inside ["a", "z"]
```

## Equality guards

`Equal` throws `EqualException` if the values are equal; `NotEqual` throws `NotEqualException` if they aren't. Values
are compared with `EqualityComparer<T>.Default`, strings optionally with a `StringComparison`. `null` operands are
allowed: two `null` values are equal. They return the left value.

```csharp
T ThrowIf.Equal<T>(T lValue, T rValue, SGuardCallback? callback = null, /* caller argument expressions */);
T ThrowIf.Equal<T, TException>(T lValue, T rValue, Func<TException> exceptionFactory, SGuardCallback? callback = null)
    where TException : Exception;
string? ThrowIf.Equal(string? lValue, string? rValue, StringComparison comparison, SGuardCallback? callback = null,
    /* caller argument expressions */);
string? ThrowIf.Equal<TException>(string? lValue, string? rValue, StringComparison comparison,
    Func<TException> exceptionFactory, SGuardCallback? callback = null)
    where TException : Exception;

// NotEqual: the same four overloads.
```

```csharp
ThrowIf.Equal(transfer.TargetAccount, transfer.SourceAccount);
ThrowIf.NotEqual(payment.Currency, order.Currency, StringComparison.OrdinalIgnoreCase);
```

## Enum guard

### NotDefined

Throws `NotDefinedException` if the enum has no member with this value, using `Enum.IsDefined`. A combination of
`[Flags]` values fails unless the enum declares a member with exactly that value.

```csharp
TEnum ThrowIf.NotDefined<TEnum>(TEnum value, SGuardCallback? callback = null,
    [CallerArgumentExpression(nameof(value))] string? valueExpression = null) where TEnum : struct, Enum;

TEnum ThrowIf.NotDefined<TEnum, TException>(TEnum value, Func<TException> exceptionFactory,
    SGuardCallback? callback = null) where TEnum : struct, Enum where TException : Exception;
```

```csharp
var status = ThrowIf.NotDefined((OrderStatus)request.Status);
```

## Length guards

`LongerThan` throws `LongerThanException` if the string has more than `maxLength` characters; `ShorterThan` throws
`ShorterThanException` if it has fewer than `minLength`. The length is `string.Length` (UTF-16 code units, so an emoji
can count as two). A `null` string throws `ArgumentNullException` and a negative limit `ArgumentOutOfRangeException`.
The message never contains the string. They return the string.

```csharp
string ThrowIf.LongerThan(string value, int maxLength, SGuardCallback? callback = null,
    [CallerArgumentExpression(nameof(value))] string? valueExpression = null,
    [CallerArgumentExpression(nameof(maxLength))] string? maxLengthExpression = null);

string ThrowIf.LongerThan<TException>(string value, int maxLength, Func<TException> exceptionFactory,
    SGuardCallback? callback = null) where TException : Exception;

// ShorterThan: the same two overloads with minLength.
```

```csharp
Name = ThrowIf.LongerThan(name, 200);
ThrowIf.ShorterThan(password, 12);
```

## Collection guards

### Any

Throws `AnyException` if **at least one** element matches the predicate. An empty collection never throws.

```csharp
void ThrowIf.Any<T>(IEnumerable<T> source, Func<T, bool> predicate, SGuardCallback? callback = null);

void ThrowIf.Any<T, TException>(IEnumerable<T> source, Func<T, bool> predicate, Func<TException> exceptionFactory,
    SGuardCallback? callback = null) where TException : Exception;

void ThrowIf.Any<T, TException>(ReadOnlySpan<T> source, Func<T, bool> predicate, Func<TException> exceptionFactory,
    SGuardCallback? callback = null) where TException : Exception;

void ThrowIf.Any<T, TException>(IEnumerable<T> source, Func<T, bool> predicate, TException exception,
    SGuardCallback? callback = null) where TException : Exception;

void ThrowIf.Any<T, TException>(ReadOnlySpan<T> source, Func<T, bool> predicate, TException exception,
    SGuardCallback? callback = null) where TException : Exception;
```

### All

Throws `AllException` if **all** elements match the predicate. Following `Enumerable.All`, an empty collection counts
as "all match", so `ThrowIf.All` throws on empty input. It has the same overloads as `Any`.

The `ReadOnlySpan<T>` overloads exist only with a factory or an exception instance; on C# 14, arrays bind to them. A
`null` source, predicate, factory or exception throws `ArgumentNullException`.

```csharp
ThrowIf.Any(items, i => i is null);
ThrowIf.All(numbers, n => n < 0);
ThrowIf.Any(lines, l => l.Quantity <= 0, () => new DomainException("Every order line needs a positive quantity."));
```

## Common parameters

### Argument expressions

The built-in-exception overloads have optional `[CallerArgumentExpression]` parameters (`valueExpression`,
`selectorExpression`, `lValueExpression`, `rValueExpression`, `minExpression`, `maxExpression`,
`maxLengthExpression`, `minLengthExpression`). The compiler fills them with the source text of your arguments; they
end up in the exception message and in `ParamName`:

```csharp
ThrowIf.NullOrEmpty(user.Email);
// NullOrEmptyException.ParamName == "user.Email"
```

Overloads that take a custom exception don't capture expressions.

### exceptionFactory

Creates the exception to throw. It runs only when the guard fails, after the callback has been invoked with `Failure`,
so a passing guard creates nothing. A lambda that doesn't capture variables is cached by the compiler and allocates
nothing; one that captures variables allocates a closure per call.

```csharp
ThrowIf.GreaterThan(percent, 50m, () => new DomainException("Discounts above 50% need approval."));
ThrowIf.GreaterThan(amount, balance, () => new InsufficientFundsException(accountId)); // captures accountId
```

If the factory returns `null`, the guard throws `InvalidOperationException`. A `null` factory throws
`ArgumentNullException`, also when the guard would pass.

### exception

A custom exception instance, thrown as-is. Because it is an argument, it is created on every call, also when the guard
passes; prefer a factory. Create a new instance per call: a shared instance gets its stack trace overwritten.

### constructorArgs (obsolete)

Arguments for a `TException` constructor, matched with reflection when the guard throws. Obsolete since 1.0
(`SGUARD001`); use a factory. See [Obsolete APIs](../migration/upgrading-to-1.0#SGUARD001).

### callback

Optional `SGuardCallback`, invoked with `Failure` when the guard fails (before the exception is thrown, and before a
factory runs) and with `Success` when it passes.

```csharp
ThrowIf.NullOrEmpty(value, SGuardCallbacks.OnFailure(() => logger.LogWarning("Null value")));
```

## Default exception types

When no custom exception is specified:

| Guard | Exception | Base type |
|-------|-----------|-----------|
| `NullOrEmpty` | `NullOrEmptyException` (also for a `null` value) | `ArgumentException` |
| `Null` | `NullException` | `ArgumentNullException` |
| `NullOrWhiteSpace` | `NullOrWhiteSpaceException` | `ArgumentException` |
| `LessThan`, `LessThanOrEqual` | `LessThanException`, `LessThanOrEqualException` | `ArgumentOutOfRangeException` |
| `GreaterThan`, `GreaterThanOrEqual` | `GreaterThanException`, `GreaterThanOrEqualException` | `ArgumentOutOfRangeException` |
| `Between` | `BetweenException` | `ArgumentOutOfRangeException` |
| `OutOfRange` | `OutOfRangeException` | `ArgumentOutOfRangeException` |
| `Negative`, `NegativeOrZero`, `Zero` | `NegativeException`, `NegativeOrZeroException`, `ZeroException` | `ArgumentOutOfRangeException` |
| `Equal`, `NotEqual` | `EqualException`, `NotEqualException` | `ArgumentOutOfRangeException` |
| `NotDefined` | `NotDefinedException` | `ArgumentOutOfRangeException` |
| `LongerThan`, `ShorterThan` | `LongerThanException`, `ShorterThanException` | `ArgumentOutOfRangeException` |
| `Any` | `AnyException` | `ArgumentException` |
| `All` | `AllException` | `ArgumentException` |

All of them live in `SGuard.Exceptions` and are `sealed`, and all derive from `ArgumentException`, so a
`catch (ArgumentException)` catches every guard failure. The comparison and range exceptions derived directly from
`ArgumentException` before 1.0; see [Upgrading to 1.0](../migration/upgrading-to-1.0#exception-base-types).

For all but `AnyException` and `AllException`, `ParamName` is the caller's argument expression (the left operand for
comparisons, the value for ranges, the selector text for a selector) and the message has no `(Parameter '...')`
suffix. `ArgumentOutOfRangeException.ActualValue` is always `null`, so values reach the message and `Data` only through
`SGuardOptions.IncludeValuesInExceptions`. `AnyException` and `AllException` carry a fixed message and no `ParamName`.

Invalid arguments are reported with standard exceptions: `ArgumentNullException` for `null` operands, sources,
predicates, selectors, factories or exception instances, `ArgumentOutOfRangeException` for a negative length limit, and
`ArgumentException` for reversed bounds.

## Related types

### SGuardOptions

```csharp
public static class SGuardOptions
{
    public static bool IncludeValuesInExceptions { get; set; } // default false
    public const int MaxValueLength = 64;
}
```

See [Exception messages and checked values](#exception-messages-and-checked-values).

### ExceptionActivator (obsolete)

```csharp
[Obsolete] // SGUARD002
public static TException ExceptionActivator.Create<TException>(object?[]? args) where TException : Exception;
```

Creates `TException` with reflection, using the constructor that matches `args`. Obsolete since 1.0; create the
exception directly, or pass a factory to a guard.

### Throw

`Throw` holds the `[DoesNotReturn]` helpers for the exceptions of the guards that existed before 1.0, public so you can
raise the same exceptions yourself:

```csharp
Throw.NullOrEmptyException<T>(T value, [CallerArgumentExpression(nameof(value))] string? valueExpression = null);
Throw.LessThanException<TLeft, TRight>(TLeft lValue, TRight rValue,
    [CallerArgumentExpression(nameof(lValue))] string? lValueExpression = null,
    [CallerArgumentExpression(nameof(rValue))] string? rValueExpression = null);
// Also LessThanOrEqualException, GreaterThanException, GreaterThanOrEqualException (same shape)
Throw.BetweenException<TValue, TMin, TMax>(TValue value, TMin min, TMax max,
    [CallerArgumentExpression(nameof(value))] string? valueExpression = null,
    [CallerArgumentExpression(nameof(min))] string? minExpression = null,
    [CallerArgumentExpression(nameof(max))] string? maxExpression = null);
Throw.That<TException>(TException exception) where TException : Exception;
```

The exceptions of the guards added in 1.0, except `NullException`, have public constructors that take the checked values
and capture their expressions in the same way.

## Next Steps

- [Is API](./is) - Boolean-returning guards
- [Callbacks API](./callbacks) - Callback reference
- [Upgrading to 1.0](../migration/upgrading-to-1.0) - What changed since 0.x
- [Real-World Examples](../guides/real-world-examples) - See the API in action
