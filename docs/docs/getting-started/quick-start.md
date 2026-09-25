---
sidebar_position: 2
---

# Quick Start

Learn the basics of SGuard with these quick examples.

## Two APIs, One Goal

SGuard provides two complementary APIs for validation:

- **`ThrowIf.*`**: Fail fast by throwing informative exceptions
- **`Is.*`**: Return booleans for control-flow-friendly checks

## Basic Usage

### 1. Validate Inputs (Fail Fast)

```csharp
public record CreateUserRequest(string Username, int Age, string Email);

public User CreateUser(CreateUserRequest req) 
{ 
    ThrowIf.NullOrEmpty(req);
    ThrowIf.NullOrEmpty(req.Email);
    ThrowIf.NullOrEmpty(req.Username);
    ThrowIf.LessThan(req.Age, 13, new ArgumentException("User must be 13+.", nameof(req.Age)));
    
    return new User(req.Username, req.Age, req.Email);
}
```

### 2. Check Conditions (Boolean Style)

```csharp
if (Is.Between(value, min, max)) 
{ 
    // value is in range
}

if (!Is.Between(req.Age, 13, 130))
{
    throw new ArgumentOutOfRangeException(nameof(req.Age), "Age seems invalid.");
}

// Numeric comparisons
bool inRange = Is.Between(value, min, max);
bool isLess = Is.LessThan(a, b);

// Email format (built-in ASCII pattern; not a full RFC 5322 parser)
bool validEmail = Is.Email("jane.doe@example.com"); // true
```

### 3. Collection Validation

```csharp
// Check if any element matches (false for an empty collection)
bool hasPositive = Is.Any(numbers, n => n > 0);

// Check if all elements match (true for an empty collection)
bool allNonNull = Is.All(items, it => it is not null);

// Throw if any element matches
ThrowIf.Any(items, i => i is null, 
    new InvalidOperationException("Collection contains null items"));
```

### 4. String Comparisons (Culture-Aware)

```csharp
// Ordinal comparisons
bool before = Is.LessThan("apple", "banana", StringComparison.Ordinal); // true

// ThrowIf.Between has a StringComparison overload (throws when the value is inside the range)
ThrowIf.Between("kiwi", "a", "m", StringComparison.OrdinalIgnoreCase); // throws BetweenException
```

String overloads exist for the `Is.*` comparisons and `ThrowIf.Between` only; `ThrowIf.LessThan`/`GreaterThan` and their `OrEqual` variants have none.

## Key Points

- **ThrowIf throws when the condition is true**: `ThrowIf.LessThan(x, 0)` throws if `x < 0`; `ThrowIf.Between(v, a, b)` throws if `v` is inside the range
- **Between is inclusive**: Both min and max values are allowed; `min > max` throws `ArgumentException`
- **CallerArgumentExpression**: Automatic, precise error messages such as `Value 'req.Email' is null or empty.` (checked values are left out unless `SGuardOptions.IncludeValuesInExceptions` is enabled)
- **Built-in exceptions derive from `ArgumentException`**: `ParamName` holds the argument expression
- **NaN fails comparisons**: `Is.*` returns `false` and `ThrowIf.*` throws
- **Custom exceptions**: Pass your own exception types when needed
- **Callbacks**: Add side effects on success/failure (logging, metrics, etc.)

## Next Steps

- [Why SGuard?](./why-sguard) - Understand the benefits
- [Guard Methods](../core-concepts/guard-methods) - Explore all available methods
- [Real-World Examples](../guides/real-world-examples) - See practical scenarios
