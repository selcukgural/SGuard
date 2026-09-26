---
sidebar_position: 2
---

# Is API

Complete reference for boolean-returning guard methods.

## Overview

`Is` provides guard methods that return `bool` values for conditional logic. Every method takes an optional
`SGuardCallback` as its last regular parameter. The callback receives `GuardOutcome.Success` when the method returns
`true` and `GuardOutcome.Failure` when it returns `false`; see [Outcome mapping](#outcome-mapping).

`Is.*` does not throw for a `false` result, but it still throws for invalid *arguments*:

| Situation | Exception |
|-----------|-----------|
| `null` comparison operand, bound, collection or predicate | `ArgumentNullException` |
| `Between` with `min` greater than `max` (bounds of the same type, or the string overloads) | `ArgumentException` |
| `Is.Email` with a `null` email (or regex) | `ArgumentNullException` |
| `Is.Email` with an empty email (or regex) | `ArgumentException` |
| `Is.Email` custom pattern that runs past its match timeout | `RegexMatchTimeoutException` |

In these cases the callback is not invoked.

## NullOrEmpty

Returns `true` if the value is null or empty.

### Overloads

```csharp
bool Is.NullOrEmpty<T>(T? value, SGuardCallback? callback = null);

bool Is.NullOrEmpty<T>(ReadOnlySpan<T> value, SGuardCallback? callback = null);

bool Is.NullOrEmpty<T>(T? value, Expression<Func<T, object>> selector, SGuardCallback? callback = null);
```

### What counts as empty

Without a selector, a value is null or empty when it is:

- `null`, or `default(T)`: so `0`, `false`, `Guid.Empty`, `default(DateTime)` and any default struct count as empty
  (`int? x = 0` is empty too);
- an empty string (`""`); a whitespace-only string is **not** empty;
- an empty array, collection or enumerable;
- a `DateTime`, `TimeSpan`, `TimeOnly` or `DateTimeOffset` with zero ticks, or `DateOnly.MinValue`.

A non-null class instance is not inspected property by property; use a selector for that.

The `ReadOnlySpan<T>` overload returns `true` only for an empty span. As with arrays and collections, a span with
elements is not empty, even if all of its elements are `null`. (Arrays bind to the generic `T` overload; both give the
same result.)

With a selector:

- `null` anywhere on the path (including `value` itself) counts as empty. The path can go through members, indexers,
  array elements and instance methods (`o => o.Items[0].Name.Trim()`); other expressions, such as method arguments or
  the operands of `+`, are evaluated as written;
- the selected member is checked with the rules above;
- a member of a complex type counts as empty only if **all** of its readable properties are null or empty
  (recursively; a type already being inspected on the same path, or nested more than 8 levels deep, is only
  null-checked; indexers are skipped).

Selectors are compiled once and cached by expression structure, including selectors that capture local variables or
use operators such as `+` and `??`; see [Expression Caching](../core-concepts/expression-caching).

### Examples

```csharp
bool isEmpty = Is.NullOrEmpty(username);
bool hasNoItems = Is.NullOrEmpty(items);
bool missing = Is.NullOrEmpty(order, o => o.Customer.Email);
bool noTags = Is.NullOrEmpty(tags.AsSpan());
```

## Comparison Guards

All comparison guards compare with `CompareTo` and throw `ArgumentNullException` when an operand is `null`.

### Overloads

```csharp
bool Is.LessThan<TLeft, TRight>(TLeft lValue, TRight rValue, SGuardCallback? callback = null)
    where TLeft : IComparable<TRight>;
bool Is.LessThan(string lValue, string rValue, StringComparison comparison, SGuardCallback? callback = null);

bool Is.LessThanOrEqual<TLeft, TRight>(TLeft lValue, TRight rValue, SGuardCallback? callback = null)
    where TLeft : IComparable<TRight>;
bool Is.LessThanOrEqual(string lValue, string rValue, StringComparison comparison, SGuardCallback? callback = null);

bool Is.GreaterThan<TLeft, TRight>(TLeft lValue, TRight rValue, SGuardCallback? callback = null)
    where TLeft : IComparable<TRight>;
bool Is.GreaterThan(string lValue, string rValue, StringComparison comparison, SGuardCallback? callback = null);

bool Is.GreaterThanOrEqual<TLeft, TRight>(TLeft lValue, TRight rValue, SGuardCallback? callback = null)
    where TLeft : IComparable<TRight>;
bool Is.GreaterThanOrEqual(string lValue, string rValue, StringComparison comparison, SGuardCallback? callback = null);
```

`LessThan` returns `true` if `lValue < rValue`, `LessThanOrEqual` if `lValue <= rValue`, `GreaterThan` if
`lValue > rValue` and `GreaterThanOrEqual` if `lValue >= rValue`.

Because the constraint is `TLeft : IComparable<TRight>`, both operand types matter: `Is.LessThan(price, 0)` with a
`decimal` price does not compile (`decimal` doesn't implement `IComparable<int>`); write `Is.LessThan(price, 0m)`.

### Between

Returns `true` if `min <= value <= max` (inclusive).

```csharp
bool Is.Between<TValue, TMin, TMax>(TValue value, TMin min, TMax max, SGuardCallback? callback = null)
    where TValue : IComparable<TMin>, IComparable<TMax>;

bool Is.Between(string value, string min, string max, StringComparison comparison, SGuardCallback? callback = null);
```

If `min` is greater than `max`, `Between` throws `ArgumentException` instead of returning `false`. This is checked when
both bounds have the same type, and always for the string overload.

### NaN

If any operand is a floating-point NaN (`double`, `float` or `Half`), every comparison and `Between` returns `false`.
Keep this in mind when the check rejects on `true`:

```csharp
// NaN gets through: Is.GreaterThan(double.NaN, max) is false
if (Is.GreaterThan(amount, max)) return Reject();

// NaN is rejected
if (!Is.Between(amount, 0.0, max)) return Reject();
```

`ThrowIf.*` comparisons throw on NaN, so they are the safer choice for argument validation.

### Examples

```csharp
bool isNegative = Is.LessThan(value, 0);
bool inRange = Is.Between(age, 0, 130);
bool before = Is.LessThan("apple", "banana", StringComparison.Ordinal);

if (Is.GreaterThanOrEqual(balance, cost))
{
    // Sufficient balance
}
```

Don't use string comparisons for access-control or path-prefix checks, or to compare version numbers; see
[String Comparisons](../guides/string-comparisons).

## Collection Guards

### Overloads

```csharp
bool Is.Any<T>(IEnumerable<T> source, Func<T, bool> predicate, SGuardCallback? callback = null);
bool Is.Any<T>(ReadOnlySpan<T> source, Func<T, bool> predicate, SGuardCallback? callback = null);

bool Is.All<T>(IEnumerable<T> source, Func<T, bool> predicate, SGuardCallback? callback = null);
bool Is.All<T>(ReadOnlySpan<T> source, Func<T, bool> predicate, SGuardCallback? callback = null);
```

`Any` returns `true` if **at least one** element matches the predicate; `All` returns `true` if **every** element
matches. Both follow LINQ semantics for empty input: `Is.Any` returns `false` and `Is.All` returns `true`, for the
`IEnumerable<T>` and `ReadOnlySpan<T>` overloads alike. On C# 14, arrays bind to the `ReadOnlySpan<T>` overloads; the
result is the same.

### Examples

```csharp
bool hasNull = Is.Any(items, i => i is null);
bool allPositive = Is.All(numbers, n => n > 0);

if (Is.Any(users, u => u.IsAdmin))
{
    // At least one admin exists
}
```

## Email

Returns `true` if the string looks like an email address.

```csharp
bool Is.Email(string email, SGuardCallback? callback = null);

bool Is.Email(string email, string regex, RegexOptions? regexOptions = null, SGuardCallback? callback = null);

bool Is.Email(string email, string regex, RegexOptions regexOptions, TimeSpan matchTimeout,
              SGuardCallback? callback = null);

static TimeSpan Is.DefaultEmailRegexTimeout { get; } // 1 second
```

- The first overload uses a built-in ASCII pattern: at most 254 characters, at most 64 before the `@`, no trailing line
  break, a domain with a top-level part of two or more letters. It is a practical filter, not a complete RFC 5322
  validator.
- The custom-pattern overloads run your `regex` with `Is.DefaultEmailRegexTimeout` (1 second) or the `matchTimeout`
  you pass. When the match runs past the timeout, `RegexMatchTimeoutException` is thrown and the callback is not
  invoked.
- A `null` email or regex throws `ArgumentNullException`; an empty one throws `ArgumentException`.

```csharp
bool ok = Is.Email("user@example.com");   // true
bool bad = Is.Email("not-an-email");      // false

bool corporate = Is.Email(input, @"^[^@\s]+@example\.com\z", RegexOptions.IgnoreCase);
```

## Outcome Mapping

For `Is.*` methods the callback reports the return value, not whether the input is "valid":

- Returns `true` → callback invoked with `GuardOutcome.Success`
- Returns `false` → callback invoked with `GuardOutcome.Failure`

So `Is.NullOrEmpty(null, callback)` reports `Success`, because the method returned `true`.

```csharp
bool valid = Is.Between(value, min, max,
    SGuardCallbacks.OnSuccess(() => metrics.Increment("valid"))
    + SGuardCallbacks.OnFailure(() => metrics.Increment("invalid")));
```

## Usage Patterns

### Conditional Logic

```csharp
if (Is.Between(discount, 0, 100))
{
    ApplyDiscount(discount);
}
else
{
    ShowError("Invalid discount");
}
```

### Boolean Expressions

```csharp
bool canProceed = Is.GreaterThanOrEqual(balance, cost) &&
                  Is.All(items, i => i.IsAvailable);
```

### Validation with Custom Errors

```csharp
if (!Is.Between(age, 18, 100))
{
    throw new ValidationException("Age must be between 18 and 100");
}
```

### Metrics and Monitoring

```csharp
bool isValid = Is.Between(
    responseTime,
    TimeSpan.Zero,
    maxResponseTime,
    SGuardCallbacks.OnSuccess(() => metrics.Increment("response.valid"))
    + SGuardCallbacks.OnFailure(() => metrics.Increment("response.slow")));
```

## Performance

- `Is.*` never allocates an exception for a `false` result.
- Comparisons call `CompareTo` directly.
- Selector-based `NullOrEmpty` checks reuse cached compiled selectors, including selectors that capture local
  variables. See [Expression Caching](../core-concepts/expression-caching#benchmarks) for measurements.

## When to Use Is.* vs ThrowIf.*

### Use Is.* When:
- Validation failure is expected (normal control flow)
- Implementing conditional logic
- Need to check multiple conditions before acting

### Use ThrowIf.* When:
- Validation failure is exceptional
- Validating arguments or invariants
- Want to fail fast with clear exceptions

See [Best Practices](../advanced/best-practices#choosing-between-is-and-throwif) for detailed guidance.

## Next Steps

- [ThrowIf API](./throwif) - Exception-throwing guards
- [Callbacks API](./callbacks) - Callback reference
- [Best Practices](../advanced/best-practices) - Effective usage guidelines
