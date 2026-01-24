---
sidebar_position: 1
---

# Null/Empty Checks

Master null and empty validation for various types with SGuard.

## Overview

The `NullOrEmpty` guard is one of the most versatile in SGuard, supporting:
- Strings
- Collections (arrays, lists, dictionaries, etc.)
- Nullable value types
- Reference types
- Complex objects with selectors

## String Validation

```csharp
ThrowIf.NullOrEmpty(username);
ThrowIf.NullOrEmpty(email);

bool isEmpty = Is.NullOrEmpty(name);
```

**What's checked:**
- `null` reference
- Empty string (`""`)

## Collection Validation

```csharp
ThrowIf.NullOrEmpty(items);
ThrowIf.NullOrEmpty(dictionary);

bool hasItems = !Is.NullOrEmpty(list);
```

**What's checked:**
- `null` reference
- Empty collection (`.Count == 0` or `.Any() == false`)

## Nullable Value Types

```csharp
int? count = GetCount();
ThrowIf.NullOrEmpty(count);

bool hasValue = !Is.NullOrEmpty(nullableInt);
```

**What's checked:**
- `.HasValue == false`

## Reference Types

```csharp
ThrowIf.NullOrEmpty(user);
ThrowIf.NullOrEmpty(order);

bool exists = !Is.NullOrEmpty(entity);
```

**What's checked:**
- `null` reference

## Deep Validation with Selectors

Use selectors to validate nested properties:

```csharp
// Validate nested property
ThrowIf.NullOrEmpty(order, o => o.Customer.Name);
ThrowIf.NullOrEmpty(user, u => u.Profile.Email);

// Multiple levels deep
ThrowIf.NullOrEmpty(order, o => o.Customer.Address.City);
```

**Benefits:**
- **Expression caching**: Compiled once, reused efficiently
- **Precise error messages**: `CallerArgumentExpression` captures the full selector path
- **Type-safe**: Compile-time checking of property access

### Selector Example: Order Validation

```csharp
public class Order
{
    public Customer Customer { get; set; }
    public List<OrderItem> Items { get; set; }
}

public class Customer
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public void ValidateOrder(Order order)
{
    ThrowIf.NullOrEmpty(order);
    ThrowIf.NullOrEmpty(order, o => o.Customer);
    ThrowIf.NullOrEmpty(order, o => o.Customer.Name);
    ThrowIf.NullOrEmpty(order, o => o.Customer.Email);
    ThrowIf.NullOrEmpty(order, o => o.Items);
}
```

**Error message for nested validation:**
```
Value cannot be null or empty. (Parameter 'order, o => o.Customer.Email')
```

## Complex Type Validation

For types with custom "empty" semantics, SGuard checks:

1. **Null reference**
2. **String properties**: If empty or whitespace
3. **Collection properties**: If empty
4. **Nullable properties**: If no value

```csharp
public class User
{
    public string Username { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
}

ThrowIf.NullOrEmpty(user); 
// Checks: user != null, Username not empty, Email not empty, Roles not empty
```

## Real-World Examples

### API Request Validation

```csharp
public record CreateUserRequest(string Username, string Email, int Age);

public User CreateUser(CreateUserRequest req)
{
    ThrowIf.NullOrEmpty(req);
    ThrowIf.NullOrEmpty(req.Username);
    ThrowIf.NullOrEmpty(req.Email);
    
    // Continue with user creation...
    return new User(req.Username, req.Email, req.Age);
}
```

### Constructor Validation

```csharp
public class User
{
    public string Username { get; }
    public string Email { get; }
    
    public User(string username, string email)
    {
        ThrowIf.NullOrEmpty(username);
        ThrowIf.NullOrEmpty(email);
        
        Username = username;
        Email = email;
    }
}
```

### Service Method Validation

```csharp
public class OrderService
{
    public void ProcessOrder(Order order)
    {
        ThrowIf.NullOrEmpty(order);
        ThrowIf.NullOrEmpty(order, o => o.Items);
        ThrowIf.NullOrEmpty(order, o => o.Customer);
        ThrowIf.NullOrEmpty(order, o => o.Customer.Email);
        
        // Process order...
    }
}
```

## Best Practices

1. **Validate early**: Check at method boundaries and constructors
2. **Use selectors for nested properties**: Benefit from expression caching
3. **Combine with other guards**: Mix with comparison guards for complete validation
4. **Fail fast**: Place `NullOrEmpty` checks before other validations

## Performance Tips

- **Selectors are cached**: Don't worry about performance with repeated selector-based validations
- **Direct checks are fastest**: When you don't need selectors, use direct checks
- See [Performance](../advanced/performance) for benchmarks

## Next Steps

- [Comparison Guards](./comparison-guards) - Range and comparison validation
- [Collection Validation](./collection-validation) - Any/All guards
- [Real-World Examples](./real-world-examples) - Complete validation scenarios
