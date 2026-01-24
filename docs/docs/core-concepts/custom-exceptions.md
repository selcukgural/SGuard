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

**When to use:**
- You need a specific error message
- Your exception has special properties to set
- You want full control over the exception

### 2. Use Generic TException

Specify the exception type as a generic parameter:

```csharp
ThrowIf.Between<int, int, int, MyCustomException>(value, min, max);

ThrowIf.Any<MyItem, MyCustomException>(
    items, 
    i => i is null, 
    new MyCustomException("Collection contains null items"));
```

**When to use:**
- Your exception has a parameterless constructor or matches SGuard's activation pattern
- You want type safety at compile time
- You're using exceptions that follow standard .NET patterns

### 3. Constructor Arguments

For exceptions that need constructor arguments, SGuard can activate them for you:

```csharp
// Exception with message and paramName
ThrowIf.NullOrEmpty<string, MyException>(
    value, 
    "Value is required", 
    "customParamName");
```

**When to use:**
- Your exception follows standard .NET exception constructors
- You want SGuard to handle the exception instantiation

## Built-in Exception Types

SGuard provides specialized exception types for each guard:

- `NullOrEmptyException`
- `BetweenException`
- `LessThanException`
- `LessThanOrEqualException`
- `GreaterThanException`
- `GreaterThanOrEqualException`
- `AnyException`
- `AllException`

These are used by default when no custom exception is specified.

## Exception Requirements

For SGuard to activate your custom exception, it should have one of these constructor signatures:

```csharp
// Parameterless
public MyException() { }

// Message only
public MyException(string message) { }

// Message and parameter name
public MyException(string message, string paramName) { }

// Message and inner exception
public MyException(string message, Exception innerException) { }
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
    public ValidationContext Context { get; }
    
    public UserValidationException(
        string message, 
        string paramName, 
        ValidationContext context) 
        : base(message, paramName)
    {
        Context = context;
    }
}

// Usage
var context = new ValidationContext { /* ... */ };
ThrowIf.NullOrEmpty(
    username, 
    new UserValidationException(
        "Username is required", 
        nameof(username), 
        context));
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
