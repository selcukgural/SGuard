---
sidebar_position: 1
---

# ThrowIf API

Complete reference for exception-throwing guard methods.

## Overview

`ThrowIf` provides guard methods that throw when their condition is **true**: `ThrowIf.LessThan(x, 0)` throws if
`x < 0`, `ThrowIf.NullOrEmpty(v)` throws if `v` is null or empty. Each guard comes in several variants:

- **Built-in exception**: throws one of the exceptions in `SGuard.Exceptions`. These overloads (for `NullOrEmpty`,
  the comparison guards and `Between`) capture the caller's argument expressions with `[CallerArgumentExpression]`
  parameters, which you don't pass yourself.
- **Exception instance**: `TException exception` is thrown as-is.
- **`new()`**: `TException` is created with its parameterless constructor.
- **Constructor arguments**: `TException` is created from `object[]? constructorArgs` via
  [`ExceptionActivator`](#exceptionactivator).

Every overload takes an optional `SGuardCallback? callback = null`; see [Callbacks API](./callbacks).

### Exception messages and checked values

Built-in exceptions (`BetweenException`, `GreaterThanException`, `GreaterThanOrEqualException`, `LessThanException`,
`LessThanOrEqualException`, `NullOrEmptyException`) name the argument expressions from the call site, but leave the
checked values out, because messages usually end up in logs and error trackers:

```csharp
ThrowIf.GreaterThan(request.Age, limit);
// GreaterThanException: Left value is greater than right value. Actual: left=request.Age, right=limit.

ThrowIf.NullOrEmpty(request.Name);
// NullOrEmptyException: Value 'request.Name' is null or empty.

ThrowIf.NullOrEmpty(order, o => o.Customer.Email);
// NullOrEmptyException: Value 'o => o.Customer.Email' is null or empty.

ThrowIf.Between(x, a, b);
// BetweenException: Value is between the specified bounds. Actual: value=x, min=a, max=b.
```

To include the values, set the option once at startup. Each value is written with `ToString()` and truncated to 64
characters, and `Exception.Data` holds these strings (keys such as `left`, `right`, `value`, `min`, `max`):

```csharp
SGuardOptions.IncludeValuesInExceptions = true;
// GreaterThanException: '4217' is greater than '1000'. Actual: left=request.Age, right=limit.
```

Only enable it when the guarded values can't be passwords, tokens or personal data.

## NullOrEmpty

Throws if the value is null or empty. "Empty" follows the same rules as
[`Is.NullOrEmpty`](./is#what-counts-as-empty): `null`, `default(T)` (including `0`, `false`, `Guid.Empty`), `""`,
empty collections, zero-tick dates and times; with a selector, `null` anywhere on the path counts as empty.

### Overloads

```csharp
// Built-in NullOrEmptyException
ThrowIf.NullOrEmpty<T>(T value, SGuardCallback? callback = null,
    [CallerArgumentExpression(nameof(value))] string? valueExpression = null);

ThrowIf.NullOrEmpty<T>(ReadOnlySpan<T> value, SGuardCallback? callback = null,
    [CallerArgumentExpression(nameof(value))] string? valueExpression = null);

ThrowIf.NullOrEmpty<TValue>(TValue value, Expression<Func<TValue, object?>> selector, SGuardCallback? callback = null,
    [CallerArgumentExpression(nameof(selector))] string? selectorExpression = null);

// Custom exception
ThrowIf.NullOrEmpty<T, TException>(T value, TException exception, SGuardCallback? callback = null)
    where TException : Exception;

ThrowIf.NullOrEmpty<T, TException>(T value, object[]? constructorArgs, SGuardCallback? callback = null)
    where TException : Exception;

ThrowIf.NullOrEmpty<TValue, TException>(TValue value, Expression<Func<TValue, object?>> selector,
    TException exception, SGuardCallback? callback = null)
    where TException : Exception;

ThrowIf.NullOrEmpty<TValue, TException>(TValue value, Expression<Func<TValue, object?>> selector,
    SGuardCallback? callback = null)
    where TException : Exception, new();

ThrowIf.NullOrEmpty<TValue, TException>(TValue value, Expression<Func<TValue, object?>> selector,
    object?[] constructorArgs, SGuardCallback? callback = null)
    where TException : Exception;
```

The `ReadOnlySpan<T>` overload throws only for an empty span; as with arrays and collections, a span whose elements
are all `null` is not empty. A `null` value
throws `NullOrEmptyException` like an empty one. A `null` selector or exception instance throws
`ArgumentNullException`.

### Examples

```csharp
ThrowIf.NullOrEmpty(username);
ThrowIf.NullOrEmpty(items);
ThrowIf.NullOrEmpty(order, o => o.Customer.Email);
ThrowIf.NullOrEmpty(value, new CustomException("Value required"));
ThrowIf.NullOrEmpty<string?, ArgumentException>(name, ["Name is required", nameof(name)]);
ThrowIf.NullOrEmpty<Order, InvalidOperationException>(order, o => o.Customer);
```

## Comparison Guards

All comparison guards throw when the comparison is true, or when either operand is a floating-point NaN (`double`,
`float`, `Half`). A `null` operand throws `ArgumentNullException`. There are no `StringComparison` overloads for
`ThrowIf.LessThan`, `LessThanOrEqual`, `GreaterThan` or `GreaterThanOrEqual`; use
[`Is.*`](./is#comparison-guards) with a `StringComparison`, or `ThrowIf.Between`.

The constraint is `TLeft : IComparable<TRight>`, so `ThrowIf.LessThan(price, 0)` with a `decimal` price doesn't
compile; write `ThrowIf.LessThan(price, 0m)`.

### LessThan

Throws if `lValue < rValue`.

```csharp
ThrowIf.LessThan<TLeft, TRight>(TLeft lValue, TRight rValue, SGuardCallback? callback = null,
    [CallerArgumentExpression(nameof(lValue))] string? lValueExpression = null,
    [CallerArgumentExpression(nameof(rValue))] string? rValueExpression = null)
    where TLeft : IComparable<TRight>;

ThrowIf.LessThan<TLeft, TRight, TException>(TLeft lValue, TRight rValue, TException exception,
    SGuardCallback? callback = null)
    where TLeft : IComparable<TRight> where TException : Exception;
```

`LessThan` has no `new()` or `constructorArgs` overload.

### LessThanOrEqual, GreaterThan, GreaterThanOrEqual

Throw if `lValue <= rValue`, `lValue > rValue` and `lValue >= rValue` respectively. Each has the same four overloads,
shown here for `GreaterThan`:

```csharp
ThrowIf.GreaterThan<TLeft, TRight>(TLeft lValue, TRight rValue, SGuardCallback? callback = null,
    [CallerArgumentExpression(nameof(lValue))] string? lValueExpression = null,
    [CallerArgumentExpression(nameof(rValue))] string? rValueExpression = null)
    where TLeft : IComparable<TRight>;

ThrowIf.GreaterThan<TLeft, TRight, TException>(TLeft lValue, TRight rValue, TException exception,
    SGuardCallback? callback = null)
    where TLeft : IComparable<TRight> where TException : Exception;

ThrowIf.GreaterThan<TLeft, TRight, TException>(TLeft lValue, TRight rValue, SGuardCallback? callback = null)
    where TLeft : IComparable<TRight> where TException : Exception, new();

ThrowIf.GreaterThan<TLeft, TRight, TException>(TLeft lValue, TRight rValue, object[]? constructorArgs,
    SGuardCallback? callback = null)
    where TLeft : IComparable<TRight> where TException : Exception;
```

### Between

Throws if `min <= value <= max` (inclusive), i.e. when the value **is** inside the range. Throws
`ArgumentException` if `min` is greater than `max` (bounds of the same type, and the string overloads).

:::warning
This is the opposite of a range check. To reject values *outside* a range, use `ThrowIf.LessThan(value, min)` and
`ThrowIf.GreaterThan(value, max)`, or `!Is.Between(value, min, max)`.
:::

```csharp
// Generic
ThrowIf.Between<TValue, TMin, TMax>(TValue value, TMin min, TMax max, SGuardCallback? callback = null,
    [CallerArgumentExpression(nameof(value))] string? valueExpression = null,
    [CallerArgumentExpression(nameof(min))] string? minExpression = null,
    [CallerArgumentExpression(nameof(max))] string? maxExpression = null)
    where TValue : IComparable<TMin>, IComparable<TMax>;

ThrowIf.Between<TValue, TMin, TMax, TException>(TValue value, TMin min, TMax max, TException exception,
    SGuardCallback? callback = null)
    where TValue : IComparable<TMin>, IComparable<TMax> where TException : Exception;

ThrowIf.Between<TValue, TMin, TMax, TException>(TValue value, TMin min, TMax max, SGuardCallback? callback = null)
    where TValue : IComparable<TMin>, IComparable<TMax> where TException : Exception, new();

ThrowIf.Between<TValue, TMin, TMax, TException>(TValue value, TMin min, TMax max, object[]? constructorArgs,
    SGuardCallback? callback = null)
    where TValue : IComparable<TMin>, IComparable<TMax> where TException : Exception;

// String with StringComparison
ThrowIf.Between(string value, string min, string max, StringComparison comparison, SGuardCallback? callback = null,
    [CallerArgumentExpression(nameof(value))] string? valueExpression = null,
    [CallerArgumentExpression(nameof(min))] string? minExpression = null,
    [CallerArgumentExpression(nameof(max))] string? maxExpression = null);

ThrowIf.Between<TException>(string value, string min, string max, StringComparison comparison,
    TException exception, SGuardCallback? callback = null)
    where TException : Exception;

ThrowIf.Between<TException>(string value, string min, string max, StringComparison comparison,
    SGuardCallback? callback = null)
    where TException : Exception, new();

ThrowIf.Between<TException>(string value, string min, string max, StringComparison comparison,
    object[]? constructorArgs, SGuardCallback? callback = null)
    where TException : Exception;
```

### Examples

```csharp
ThrowIf.LessThan(age, 0);
ThrowIf.GreaterThan(price, maxPrice);
ThrowIf.Between(value, 10, 20);  // Throws if 10 <= value <= 20
ThrowIf.LessThan(price, 0m, new ArgumentOutOfRangeException(nameof(price)));
ThrowIf.GreaterThan<int, int, ArgumentOutOfRangeException>(quantity, 100);
ThrowIf.Between("m", "a", "z", StringComparison.Ordinal);  // Throws: "m" is inside ["a", "z"]
```

## Collection Guards

### Any

Throws if **at least one** element matches the predicate. An empty collection never throws.

```csharp
ThrowIf.Any<T>(IEnumerable<T> source, Func<T, bool> predicate, SGuardCallback? callback = null);

ThrowIf.Any<T, TException>(IEnumerable<T> source, Func<T, bool> predicate, TException exception,
    SGuardCallback? callback = null)
    where TException : Exception;

ThrowIf.Any<T, TException>(ReadOnlySpan<T> source, Func<T, bool> predicate, TException exception,
    SGuardCallback? callback = null)
    where TException : Exception;
```

### All

Throws if **all** elements match the predicate. Following `Enumerable.All`, an empty collection counts as "all match",
so `ThrowIf.All` throws on empty input.

```csharp
ThrowIf.All<T>(IEnumerable<T> source, Func<T, bool> predicate, SGuardCallback? callback = null);

ThrowIf.All<T, TException>(IEnumerable<T> source, Func<T, bool> predicate, TException exception,
    SGuardCallback? callback = null)
    where TException : Exception;

ThrowIf.All<T, TException>(ReadOnlySpan<T> source, Func<T, bool> predicate, TException exception,
    SGuardCallback? callback = null)
    where TException : Exception;
```

The `ReadOnlySpan<T>` overloads exist only with an exception instance; on C# 14, arrays bind to them. A `null`
source, predicate or exception throws `ArgumentNullException`.

### Examples

```csharp
ThrowIf.Any(items, i => i is null);
ThrowIf.All(numbers, n => n < 0);
ThrowIf.Any(orders, o => o.IsInvalid, new OrderValidationException("Invalid order"));
```

## Common Parameters

### Argument expressions

The built-in-exception overloads have optional `[CallerArgumentExpression]` parameters (`valueExpression`,
`selectorExpression`, `lValueExpression`, `rValueExpression`, `minExpression`, `maxExpression`). The compiler fills
them with the source text of your arguments; they end up in the exception message and in `ParamName`:

```csharp
ThrowIf.NullOrEmpty(user.Email);
// NullOrEmptyException.ParamName == "user.Email"
```

Overloads that take a custom exception don't capture expressions.

### exception

Custom exception instance to throw instead of the default. It is thrown as-is, so create a new instance per call.

```csharp
ThrowIf.LessThan(age, 0, new ArgumentException("Age must be non-negative", nameof(age)));
```

### constructorArgs

Arguments for a `TException` constructor, matched at the time the guard throws. If no constructor matches, an
`InvalidOperationException` is thrown instead (after the callback has run with `Failure`).

```csharp
ThrowIf.GreaterThan<int, int, ArgumentOutOfRangeException>(quantity, 100, [nameof(quantity), "At most 100."]);
```

### callback

Optional `SGuardCallback`, invoked with `Failure` when the guard throws (before the exception propagates) and with
`Success` when it passes.

```csharp
ThrowIf.NullOrEmpty(value, SGuardCallbacks.OnFailure(() => logger.LogWarning("Null value")));
```

## Default Exception Types

When no custom exception is specified:

| Guard | Exception |
|-------|-----------|
| `NullOrEmpty` | `NullOrEmptyException` (also for a `null` value) |
| `LessThan` | `LessThanException` |
| `LessThanOrEqual` | `LessThanOrEqualException` |
| `GreaterThan` | `GreaterThanException` |
| `GreaterThanOrEqual` | `GreaterThanOrEqualException` |
| `Between` | `BetweenException` |
| `Any` | `AnyException` |
| `All` | `AllException` |

All of them live in `SGuard.Exceptions`, are `sealed` and derive from `ArgumentException`, so an existing
`catch (ArgumentException)` also catches them. For all but `AnyException` and `AllException`, `ParamName` is the
caller's argument expression (the left operand for comparisons, the value for `Between`, the selector text for a
selector) and the message has no `(Parameter '...')` suffix. `AnyException` and `AllException` carry a fixed
message and no `ParamName`.

Invalid arguments are reported with standard exceptions: `ArgumentNullException` for `null` operands, sources,
predicates, selectors or exception instances, and `ArgumentException` for reversed `Between` bounds.

## Related Types

### SGuardOptions

```csharp
public static class SGuardOptions
{
    public static bool IncludeValuesInExceptions { get; set; } // default false
    public const int MaxValueLength = 64;
}
```

See [Exception messages and checked values](#exception-messages-and-checked-values).

### ExceptionActivator

```csharp
public static TException ExceptionActivator.Create<TException>(object?[]? args) where TException : Exception;
```

Creates `TException` with the constructor that matches `args` (`null` means the parameterless constructor). Throws
`InvalidOperationException` if no constructor matches. The `constructorArgs` overloads use it.

### Throw

`Throw` holds the `[DoesNotReturn]` helpers the guards use, public so you can raise the same exceptions yourself:

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

## Next Steps

- [Is API](./is) - Boolean-returning guards
- [Callbacks API](./callbacks) - Callback reference
- [Real-World Examples](../guides/real-world-examples) - See the API in action
