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
| `null` string for `LongerThan` or `ShorterThan`, or a negative length | `ArgumentNullException`, `ArgumentOutOfRangeException` |
| `Is.Match` with a `null` or invalid pattern, or a match that runs past its timeout | `ArgumentException`, `RegexMatchTimeoutException` |

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

- `null`, or `default(T)`: so `0`, `false`, `Guid.Empty`, `default(DateTime)`, the default enum value and any default
  struct count as empty, also when they are boxed or held by a nullable (`int? x = 0` and `DayOfWeek? d = default` are
  empty too);
- an empty string (`""`); a whitespace-only string is **not** empty;
- an empty array, collection or enumerable, or an empty `Memory<T>` or `ReadOnlyMemory<T>`;
- a `DateTime`, `TimeSpan`, `TimeOnly` or `DateTimeOffset` with zero ticks, or `DateOnly.MinValue`.

A non-null class instance is not inspected property by property; use a selector for that.

The `ReadOnlySpan<T>` overload returns `true` only for an empty span. As with arrays and collections, a span with
elements is not empty, even if all of its elements are `null`. (Arrays bind to the generic `T` overload; both give the
same result.)

With a selector:

- `null` anywhere on the path (including `value` itself) counts as empty. The path can go through members, indexers,
  array elements and instance methods (`o => o.Items[0].Name.Trim()`); other expressions, such as method arguments or
  the operands of `+`, are evaluated as written;
- the selected member is checked with the rules above; a default `ImmutableArray<T>` counts as empty;
- a member of a complex type counts as empty only if **all** of its readable properties are null or empty
  (recursively; a type already being inspected on the same path, or nested more than 8 levels deep, is only
  null-checked; indexers are skipped);
- a framework class that is not a collection (a type in a `System` namespace, such as `Uri` or `Version`) is only
  checked for null; its properties are not read.

Selectors are compiled once and cached by expression structure, including selectors that capture local variables or
use operators such as `+` and `??`; see [Expression Caching](../core-concepts/expression-caching).

:::warning Selectors and Native AOT
Selectors don't work under Native AOT, and trimming can remove the properties they read. The selector overloads are
marked `[RequiresUnreferencedCode]` and `[RequiresDynamicCode]`, so trimmed and AOT apps get a warning. Check the member
directly instead: `Is.NullOrEmpty(order.Customer?.Email)`.
:::

### Examples

```csharp
bool isEmpty = Is.NullOrEmpty(username);
bool hasNoItems = Is.NullOrEmpty(items);
bool missing = Is.NullOrEmpty(order, o => o.Customer.Email);
bool noTags = Is.NullOrEmpty(new ReadOnlySpan<string>(tags));
```

## Null and NullOrWhiteSpace

```csharp
bool Is.Null<T>(T? value, SGuardCallback? callback = null);
bool Is.NullOrWhiteSpace(string? value, SGuardCallback? callback = null);
```

`Is.Null` returns `true` only for `null`: unlike `NullOrEmpty`, `""`, empty collections and default values such as `0`
are not null. `Is.NullOrWhiteSpace` returns `true` for `null`, `""` and strings of white space only. Both are annotated
`[NotNullWhen(false)]`, so after `if (Is.NullOrWhiteSpace(name)) return;` the compiler knows `name` isn't null.

```csharp
if (Is.NullOrWhiteSpace(request.Name))
{
    errors["name"] = ["A name is required."];
}
```

## Comparison Guards

All comparison guards compare with `CompareTo` (the `StringComparison` overloads with `string.Compare`) and throw
`ArgumentNullException` when an operand is `null`.

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

### OutOfRange

Returns `true` if `value < min` or `value > max`, the opposite of `Between`. Both bounds are inclusive, and a NaN
operand is outside every range.

```csharp
bool Is.OutOfRange<TValue, TMin, TMax>(TValue value, TMin min, TMax max, SGuardCallback? callback = null)
    where TValue : IComparable<TMin>, IComparable<TMax>;

bool Is.OutOfRange(string value, string min, string max, StringComparison comparison, SGuardCallback? callback = null);
```

Like `Between`, it throws `ArgumentException` when `min` is greater than `max`.

### Sign checks

```csharp
bool Is.Negative<T>(T value, SGuardCallback? callback = null) where T : INumber<T>;        // value < 0
bool Is.NegativeOrZero<T>(T value, SGuardCallback? callback = null) where T : INumber<T>;  // value <= 0
bool Is.Zero<T>(T value, SGuardCallback? callback = null) where T : INumberBase<T>;        // value == 0
```

They work with every number type. Negative zero (`-0.0`) is zero, not negative. Like the other comparisons, NaN returns
`false`.

### Equal and NotEqual

```csharp
bool Is.Equal<T>(T lValue, T rValue, SGuardCallback? callback = null);
bool Is.Equal(string? lValue, string? rValue, StringComparison comparison, SGuardCallback? callback = null);
bool Is.NotEqual<T>(T lValue, T rValue, SGuardCallback? callback = null);
bool Is.NotEqual(string? lValue, string? rValue, StringComparison comparison, SGuardCallback? callback = null);
```

Values are compared with `EqualityComparer<T>.Default`. `null` operands are allowed, and two `null` values are equal.

### NaN

If any operand is a floating-point NaN (`double`, `float`, `Half` or `NFloat`), every comparison, `Between` and the
sign checks return `false`, and `OutOfRange` returns `true`.
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

## Enums and lengths

```csharp
bool Is.NotDefined<TEnum>(TEnum value, SGuardCallback? callback = null) where TEnum : struct, Enum;
bool Is.LongerThan(string value, int maxLength, SGuardCallback? callback = null);
bool Is.ShorterThan(string value, int minLength, SGuardCallback? callback = null);
```

`NotDefined` returns `true` if the enum has no member with this value (`Enum.IsDefined`; a combination of `[Flags]`
values counts as defined only if the enum declares it). `LongerThan` and `ShorterThan` compare `string.Length`, the
number of UTF-16 code units.

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

## Url, CreditCard and Match

These validators return `false` for `null` and `""` and are annotated `[NotNullWhen(true)]`, unlike `Is.Email`, which
throws for them.

```csharp
bool Is.Url(string? value, SGuardCallback? callback = null);
bool Is.CreditCard(string? value, SGuardCallback? callback = null);
bool Is.Match(string? value, Regex regex, SGuardCallback? callback = null);
bool Is.Match(string? value, string pattern, RegexOptions options = RegexOptions.None, SGuardCallback? callback = null);
bool Is.Match(string? value, string pattern, RegexOptions options, TimeSpan matchTimeout, SGuardCallback? callback = null);

static TimeSpan Is.DefaultRegexTimeout { get; } // 1 second
```

- **`Url`** returns `true` for an absolute `http` or `https` URL with a host. Every other scheme fails, including
  `javascript:`, `data:` and `file:`, so a value that passes is safe to render as a link. Relative and
  protocol-relative URLs (`//example.com`) fail, and so does a string with leading or trailing white space. It parses
  the string with `Uri.TryCreate`, so unlike most guards it allocates.
- **`CreditCard`** returns `true` for 12 to 19 digits that pass the Luhn checksum, with optional single spaces or
  hyphens between digits. It checks the format only: it can't tell whether the card exists or can be charged. Don't
  log card numbers.
- **`Match`** returns `true` if the string matches the regular expression. The pattern is not anchored for you:
  `[0-9]+` matches `"abc1"`, so write `^...\z` to match the whole string. A pattern string is matched with
  `Is.DefaultRegexTimeout` or the `matchTimeout` you pass; a `Regex` instance uses its own timeout. Prefer a cached or
  `[GeneratedRegex]` instance on hot paths, and don't build patterns from user input.

```csharp
if (!Is.Url(request.Website))
{
    errors["website"] = ["Enter an http or https address."];
}

[GeneratedRegex(@"^[A-Z]{3}\z")]
private static partial Regex CurrencyCode();

bool known = Is.Match(request.Currency, CurrencyCode());
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
