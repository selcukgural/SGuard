---
sidebar_position: 3
---

# Custom Exceptions

Learn how to use your own exception types with SGuard guards.

## Overview

While SGuard provides sensible default exceptions for each guard method, you often need to throw domain-specific exceptions. SGuard makes this easy with multiple approaches.

## Three Ways to Use Custom Exceptions

### 1. Pass a Custom Exception Instance

The simplest approach—provide a pre-configured exception:

```csharp
ThrowIf.LessThanOrEqual(
    quantity, 
    0, 
    new DomainValidationException("Quantity must be greater than zero."));
```

The exception object is created on every call, even when the guard passes. On hot paths, prefer the
`TException` or `constructorArgs` overloads, which create it only on failure.

**When to use:**
- You need a specific error message
- Your exception has special properties to set
- You want full control over the exception

### 2. Use Generic TException

Specify the exception type as a generic parameter and let SGuard create it with its parameterless constructor:

```csharp
ThrowIf.Between<int, int, int, MyCustomException>(value, min, max);

ThrowIf.GreaterThan<int, int, MyCustomException>(quantity, maxQuantity);
```

These overloads require `where TException : Exception, new()`, so the exception type needs a public parameterless
constructor. The exception is created only when the guard fails.

**When to use:**
- The exception type alone says what went wrong (no message needed)
- You want to avoid creating an exception object on every call

### 3. Constructor Arguments

For exceptions that need constructor arguments, pass them as an `object[]` and SGuard creates the exception when the
guard fails:

```csharp
// Calls ArgumentException(string message, string paramName)
ThrowIf.NullOrEmpty<string, ArgumentException>(
    value, 
    new object[] { "Value is required", "customParamName" });

// Calls OrderValidationException(string message)
ThrowIf.LessThanOrEqual<int, int, OrderValidationException>(
    quantity, 
    0, 
    new object[] { "Quantity must be positive" });
```

The arguments are matched against the exception's public constructors at run time (via `ExceptionActivator`), so any
constructor works as long as the argument types fit. If none matches, the guard throws `InvalidOperationException`
instead of your exception, and only when it fails, so cover these calls with a test.

**When to use:**
- Your exception needs constructor arguments
- You want the exception created only when the guard fails

### Which Guards Support Which Variant

| Guard | Exception instance | `TException : new()` | `constructorArgs` |
|---|---|---|---|
| `Between` | Yes | Yes | Yes |
| `GreaterThan`, `GreaterThanOrEqual` | Yes | Yes | Yes |
| `LessThanOrEqual` | Yes | Yes | Yes |
| `LessThan` | Yes | No | No |
| `NullOrEmpty` (value) | Yes | No | Yes |
| `NullOrEmpty` (with selector) | Yes | Yes | Yes |
| `Any`, `All` | Yes | No | No |

## Built-in Exception Types

SGuard provides specialized exception types for each guard (namespace `SGuard.Exceptions`):

- `NullOrEmptyException`
- `BetweenException`
- `LessThanException`
- `LessThanOrEqualException`
- `GreaterThanException`
- `GreaterThanOrEqualException`
- `AnyException`
- `AllException`

These are used by default when no custom exception is specified. They all derive from `ArgumentException`, so
`catch (ArgumentException)` handles them. `ParamName` holds the argument expression from the call site, and the
message names it too:

```csharp
try
{
    ThrowIf.GreaterThan(request.Age, limit);
}
catch (GreaterThanException ex)
{
    // ex.ParamName == "request.Age"
    // ex.Message   == "Left value is greater than right value. Actual: left=request.Age, right=limit."
}
```

### Including Values in Messages

By default the checked **values** are left out of built-in exception messages and `Exception.Data`, because messages
end up in logs and error trackers and values may be passwords, tokens or personal data. To include them, set this
once at startup:

```csharp
SGuardOptions.IncludeValuesInExceptions = true;

// ThrowIf.GreaterThan(request.Age, 1000) with request.Age == 4217 now throws:
// "'4217' is greater than '1000'. Actual: left=request.Age, right=1000."
```

Each value is written with `ToString()` and truncated to 64 characters (`SGuardOptions.MaxValueLength`), and
`Exception.Data` holds those formatted strings. Enable it only when the checked values can't contain secrets or
personal data. The option affects only the built-in exceptions, not exceptions you supply.

## Exception Requirements

What your exception type needs depends on the overload you use:

- **Exception instance**: nothing; you construct it yourself.
- **`TException` with `new()`**: a public parameterless constructor (enforced at compile time).
- **`constructorArgs`**: a public constructor whose parameters accept the arguments you pass (checked at run time;
  `InvalidOperationException` if none matches).

Following the standard .NET exception constructors keeps all three options open:

```csharp
public class MyException : Exception
{
    public MyException() { }
    public MyException(string message) : base(message) { }
    public MyException(string message, Exception innerException) : base(message, innerException) { }
}
```

## Real-World Examples

### Domain Validation Exception

```csharp
public class OrderValidationException : Exception
{
    public OrderValidationException(string message) : base(message) { }
}

// Usage
ThrowIf.LessThanOrEqual(
    order.Quantity, 
    0, 
    new OrderValidationException("Order quantity must be positive"));
```

### Business Rule Exception

```csharp
public class InsufficientStockException : InvalidOperationException
{
    public string Sku { get; }
    
    public InsufficientStockException(string message, string sku) 
        : base(message)
    {
        Sku = sku;
    }
}

// Usage
ThrowIf.GreaterThan(
    item.Quantity, 
    stock, 
    new InsufficientStockException(
        $"Insufficient stock for SKU '{item.Sku}'.", 
        item.Sku));
```

### Validation Exception with Context

```csharp
public class UserValidationException : ArgumentException
{
    public string TenantId { get; }
    
    public UserValidationException(
        string message, 
        string paramName, 
        string tenantId) 
        : base(message, paramName)
    {
        TenantId = tenantId;
    }
}

// Usage
ThrowIf.NullOrEmpty(
    username, 
    new UserValidationException(
        "Username is required", 
        nameof(username), 
        tenantId));
```

## Combining with Callbacks

Custom exceptions work seamlessly with callbacks:

```csharp
ThrowIf.LessThan(
    value, 
    threshold, 
    new BusinessRuleException("Value below threshold"),
    SGuardCallbacks.OnFailure(() => logger.LogError("Business rule violated")));
```

## Best Practices

1. **Use meaningful exception types**: Create domain-specific exceptions that clearly communicate what went wrong
2. **Include context**: Add properties to your exceptions that help with debugging and error handling
3. **Follow .NET conventions**: Implement standard exception constructors for better interoperability
4. **Don't over-engineer**: Sometimes `ArgumentException` or `InvalidOperationException` is enough

## Next Steps

- [Real-World Examples](../guides/real-world-examples) - See custom exceptions in action
- [API Reference](../api/throwif) - Complete ThrowIf API
- [Best Practices](../advanced/best-practices) - Exception handling guidelines
