---
sidebar_position: 1
---

# ThrowIf API

Complete reference for exception-throwing guard methods.

## Overview

`ThrowIf` provides guard methods that throw exceptions when validation fails. All methods use `CallerArgumentExpression` to generate precise error messages automatically.

## NullOrEmpty

Validates that values are not null or empty.

### Overloads

```csharp
// Direct validation
ThrowIf.NullOrEmpty<T>(T value, [CallerArgumentExpression] string? paramName = null);

// With custom exception
ThrowIf.NullOrEmpty<T, TException>(T value, TException exception, [CallerArgumentExpression] string? paramName = null)
    where TException : Exception;

// With callback
ThrowIf.NullOrEmpty<T>(T value, SGuardCallback? callback, [CallerArgumentExpression] string? paramName = null);

// With selector
ThrowIf.NullOrEmpty<T, TProperty>(T value, Expression<Func<T, TProperty>> selector, [CallerArgumentExpression] string? paramName = null);
```

### Examples

```csharp
ThrowIf.NullOrEmpty(username);
ThrowIf.NullOrEmpty(items);
ThrowIf.NullOrEmpty(order, o => o.Customer.Email);
ThrowIf.NullOrEmpty(value, new CustomException("Value required"));
```

## Comparison Guards

### LessThan

Throws if `value < other`.

```csharp
// Generic (IComparable<T>)
ThrowIf.LessThan<T>(T value, T other, [CallerArgumentExpression] string? paramName = null)
    where T : IComparable<T>;

// String with StringComparison
ThrowIf.LessThan(string value, string other, StringComparison comparison, [CallerArgumentExpression] string? paramName = null);

// With custom exception
ThrowIf.LessThan<T, TException>(T value, T other, TException exception)
    where T : IComparable<T>
    where TException : Exception;
```

### LessThanOrEqual

Throws if `value <= other`.

```csharp
ThrowIf.LessThanOrEqual<T>(T value, T other, [CallerArgumentExpression] string? paramName = null)
    where T : IComparable<T>;

ThrowIf.LessThanOrEqual(string value, string other, StringComparison comparison, [CallerArgumentExpression] string? paramName = null);
```

### GreaterThan

Throws if `value > other`.

```csharp
ThrowIf.GreaterThan<T>(T value, T other, [CallerArgumentExpression] string? paramName = null)
    where T : IComparable<T>;

ThrowIf.GreaterThan(string value, string other, StringComparison comparison, [CallerArgumentExpression] string? paramName = null);
```

### GreaterThanOrEqual

Throws if `value >= other`.

```csharp
ThrowIf.GreaterThanOrEqual<T>(T value, T other, [CallerArgumentExpression] string? paramName = null)
    where T : IComparable<T>;

ThrowIf.GreaterThanOrEqual(string value, string other, StringComparison comparison, [CallerArgumentExpression] string? paramName = null);
```

### Between

Throws if `min <= value <= max` (inclusive).

```csharp
// Generic (IComparable<T>)
ThrowIf.Between<T>(T value, T min, T max, [CallerArgumentExpression] string? paramName = null)
    where T : IComparable<T>;

// String with StringComparison
ThrowIf.Between(string value, string min, string max, StringComparison comparison, [CallerArgumentExpression] string? paramName = null);

// With custom exception
ThrowIf.Between<T, TMin, TMax, TException>(T value, TMin min, TMax max, TException exception)
    where T : IComparable<T>, IComparable<TMin>, IComparable<TMax>
    where TException : Exception;
```

### Examples

```csharp
ThrowIf.LessThan(age, 0);
ThrowIf.GreaterThan(price, maxPrice);
ThrowIf.Between(value, 10, 20);  // Throws if 10 <= value <= 20
ThrowIf.LessThan("apple", "banana", StringComparison.Ordinal);
```

## Collection Guards

### Any

Throws if **at least one** element matches the predicate.

```csharp
// Basic
ThrowIf.Any<T>(IEnumerable<T> collection, Func<T, bool> predicate, [CallerArgumentExpression] string? paramName = null);

// With custom exception
ThrowIf.Any<T, TException>(IEnumerable<T> collection, Func<T, bool> predicate, TException exception)
    where TException : Exception;

// With callback
ThrowIf.Any<T>(IEnumerable<T> collection, Func<T, bool> predicate, SGuardCallback? callback, [CallerArgumentExpression] string? paramName = null);
```

### All

Throws if **all** elements match the predicate.

```csharp
// Basic
ThrowIf.All<T>(IEnumerable<T> collection, Func<T, bool> predicate, [CallerArgumentExpression] string? paramName = null);

// With custom exception
ThrowIf.All<T, TException>(IEnumerable<T> collection, Func<T, bool> predicate, TException exception)
    where TException : Exception;

// With callback
ThrowIf.All<T>(IEnumerable<T> collection, Func<T, bool> predicate, SGuardCallback? callback, [CallerArgumentExpression] string? paramName = null);
```

### Examples

```csharp
ThrowIf.Any(items, i => i is null);
ThrowIf.All(numbers, n => n < 0);
ThrowIf.Any(orders, o => o.IsInvalid, new OrderValidationException("Invalid order"));
```

## Common Parameters

### paramName

Automatically captured by `CallerArgumentExpression`. Contains the expression text of the validated argument.

```csharp
ThrowIf.NullOrEmpty(user.Email);
// paramName = "user.Email"
```

### exception

Custom exception instance to throw instead of the default.

```csharp
ThrowIf.LessThan(age, 0, new ArgumentException("Age must be non-negative"));
```

### callback

Optional `SGuardCallback` for side effects on success/failure.

```csharp
ThrowIf.NullOrEmpty(value, SGuardCallbacks.OnFailure(() => logger.LogWarning("Null value")));
```

## Default Exception Types

When no custom exception is specified:

- `NullOrEmpty` → `NullOrEmptyException` or `ArgumentNullException`
- `LessThan` → `LessThanException`
- `LessThanOrEqual` → `LessThanOrEqualException`
- `GreaterThan` → `GreaterThanException`
- `GreaterThanOrEqual` → `GreaterThanOrEqualException`
- `Between` → `BetweenException`
- `Any` → `AnyException`
- `All` → `AllException`

## Next Steps

- [Is API](./is) - Boolean-returning guards
- [Callbacks API](./callbacks) - Callback reference
- [Real-World Examples](../guides/real-world-examples) - See the API in action
