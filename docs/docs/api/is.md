---
sidebar_position: 2
---

# Is API

Complete reference for boolean-returning guard methods.

## Overview

`Is` provides guard methods that return `bool` values for conditional logic. All methods support optional callbacks for side effects.

## NullOrEmpty

Returns `true` if the value is null or empty.

### Overloads

```csharp
// Direct validation
bool Is.NullOrEmpty<T>(T value);

// With callback
bool Is.NullOrEmpty<T>(T value, SGuardCallback? callback);

// With selector
bool Is.NullOrEmpty<T, TProperty>(T value, Expression<Func<T, TProperty>> selector);
```

### Examples

```csharp
bool isEmpty = Is.NullOrEmpty(username);
bool hasNoItems = Is.NullOrEmpty(items);
bool missing = Is.NullOrEmpty(order, o => o.Customer.Email);
```

## Comparison Guards

### LessThan

Returns `true` if `value < other`.

```csharp
// Generic (IComparable<T>)
bool Is.LessThan<T>(T value, T other) where T : IComparable<T>;

// String with StringComparison
bool Is.LessThan(string value, string other, StringComparison comparison);

// With callback
bool Is.LessThan<T>(T value, T other, SGuardCallback? callback) where T : IComparable<T>;
```

### LessThanOrEqual

Returns `true` if `value <= other`.

```csharp
bool Is.LessThanOrEqual<T>(T value, T other) where T : IComparable<T>;
bool Is.LessThanOrEqual(string value, string other, StringComparison comparison);
```

### GreaterThan

Returns `true` if `value > other`.

```csharp
bool Is.GreaterThan<T>(T value, T other) where T : IComparable<T>;
bool Is.GreaterThan(string value, string other, StringComparison comparison);
```

### GreaterThanOrEqual

Returns `true` if `value >= other`.

```csharp
bool Is.GreaterThanOrEqual<T>(T value, T other) where T : IComparable<T>;
bool Is.GreaterThanOrEqual(string value, string other, StringComparison comparison);
```

### Between

Returns `true` if `min <= value <= max` (inclusive).

```csharp
// Generic (IComparable<T>)
bool Is.Between<T>(T value, T min, T max) where T : IComparable<T>;

// String with StringComparison
bool Is.Between(string value, string min, string max, StringComparison comparison);

// With callback
bool Is.Between<T>(T value, T min, T max, SGuardCallback? callback) where T : IComparable<T>;
```

### Examples

```csharp
bool isNegative = Is.LessThan(value, 0);
bool inRange = Is.Between(age, 0, 130);
bool before = Is.LessThan("apple", "banana", StringComparison.Ordinal);

if (Is.GreaterThan(balance, cost))
{
    // Sufficient balance
}
```

## Collection Guards

### Any

Returns `true` if **at least one** element matches the predicate.

```csharp
// Basic
bool Is.Any<T>(IEnumerable<T> collection, Func<T, bool> predicate);

// With callback
bool Is.Any<T>(IEnumerable<T> collection, Func<T, bool> predicate, SGuardCallback? callback);
```

### All

Returns `true` if **all** elements match the predicate.

```csharp
// Basic
bool Is.All<T>(IEnumerable<T> collection, Func<T, bool> predicate);

// With callback
bool Is.All<T>(IEnumerable<T> collection, Func<T, bool> predicate, SGuardCallback? callback);
```

### Examples

```csharp
bool hasNull = Is.Any(items, i => i is null);
bool allPositive = Is.All(numbers, n => n > 0);

if (Is.Any(users, u => u.IsAdmin))
{
    // At least one admin exists
}
```

## Outcome Mapping

For `Is.*` methods:
- Returns `true` → Outcome = `Success` (callback invoked with Success)
- Returns `false` → Outcome = `Failure` (callback invoked with Failure)

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
bool canProceed = Is.GreaterThan(balance, cost) && 
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
    0, 
    maxResponseTime,
    SGuardCallbacks.OnSuccess(() => metrics.Increment("response.valid"))
    + SGuardCallbacks.OnFailure(() => metrics.Increment("response.slow")));
```

## Performance

`Is.*` methods are highly optimized:
- No exception allocation unless you throw based on the result
- Direct comparisons with minimal overhead
- Expression caching for selector-based validations

Typical overhead: **5-50ns** per call (depending on guard type).

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
