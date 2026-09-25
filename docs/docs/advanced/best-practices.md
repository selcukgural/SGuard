---
sidebar_position: 2
---

# Best Practices

Guidelines for effective validation with SGuard.

## Validation Principles

### 1. Fail Fast

Validate inputs as early as possible:

```csharp
public User CreateUser(string username, int age, string email)
{
    // Validate FIRST, before any logic
    ThrowIf.NullOrEmpty(username);
    ThrowIf.NullOrEmpty(email);
    ThrowIf.LessThan(age, 0);
    
    // Then proceed with business logic
    return new User(username, age, email);
}
```

### 2. Validate at Boundaries

Add validation at system boundaries:
- **Public APIs**: All public methods and constructors
- **External inputs**: Web requests, file reads, user input
- **Cross-service calls**: Before sending data to other services

```csharp
[HttpPost]
public IActionResult CreateProduct([FromBody] CreateProductRequest request)
{
    // Validate at API boundary
    ThrowIf.NullOrEmpty(request);
    ThrowIf.NullOrEmpty(request.Name);
    
    // ...
}
```

### 3. Don't Over-Validate

Avoid validation in private methods that only internal code calls:

```csharp
public class OrderService
{
    public void CreateOrder(Order order)
    {
        // Validate: public method
        ThrowIf.NullOrEmpty(order);
        
        ProcessOrder(order);
    }
    
    private void ProcessOrder(Order order)
    {
        // No validation: private, already validated
        // ...
    }
}
```

## Choosing Between Is.* and ThrowIf.*

### Use ThrowIf.* When:

- Validating method arguments or constructor parameters
- Enforcing invariants that must never be violated
- Validation failure is exceptional (shouldn't happen in normal operation)

```csharp
public Money(decimal amount, string currency)
{
    ThrowIf.LessThan(amount, 0m); // 0m: a decimal needs a decimal bound
    ThrowIf.NullOrEmpty(currency);
}
```

### Use Is.* When:

- Implementing conditional logic
- Validation failure is expected (normal control flow)
- You need to check multiple conditions before acting

```csharp
if (Is.Between(discount, 0, 100) && Is.GreaterThan(total, minOrder))
{
    ApplyDiscount(discount);
}
else
{
    ShowError("Invalid discount or order total");
}
```

## Exception Handling

### Use Appropriate Exception Types

```csharp
// ArgumentException for invalid arguments
ThrowIf.NullOrEmpty(username, 
    new ArgumentException("Username is required", nameof(username)));

// InvalidOperationException for state violations
ThrowIf.LessThan(user.Credits, cost,
    new InvalidOperationException("Insufficient credits"));

// Custom exceptions for domain rules
ThrowIf.Any(order.Items, i => i.IsBackordered,
    new OrderValidationException("Cannot checkout with backordered items"));
```

### Document Exceptions

```csharp
/// <summary>
/// Creates a new user.
/// </summary>
/// <param name="username">The username.</param>
/// <param name="age">The user's age.</param>
/// <exception cref="SGuard.Exceptions.NullOrEmptyException">
/// Thrown when <paramref name="username"/> is null or empty.
/// </exception>
/// <exception cref="SGuard.Exceptions.LessThanException">
/// Thrown when <paramref name="age"/> is negative.
/// </exception>
public User CreateUser(string username, int age)
{
    ThrowIf.NullOrEmpty(username);
    ThrowIf.LessThan(age, 0);
    // ...
}
```

Both built-in exceptions derive from `ArgumentException`, so callers that only catch `ArgumentException` are
covered; documenting the concrete types lets them be more specific.

## String Comparisons

### Always Specify StringComparison

```csharp
// Good: Explicit comparison
Is.LessThan(key, "config.", StringComparison.Ordinal);

// Bad: Ambiguous
key.CompareTo("config.") < 0;
```

### Use Ordinal for Non-User Strings

```csharp
// Identifiers, keys, protocol tokens
bool inRange = Is.Between(code, "A000", "A999", StringComparison.Ordinal);
```

Don't use string ordering for version numbers ("10.0" sorts before "9.0"), path-prefix or access-control checks.
Compare versions as `System.Version`:

```csharp
ThrowIf.LessThan(Version.Parse(apiVersion), new Version(1, 0));
```

### Use CurrentCulture for Display

```csharp
// User-facing strings
bool sorted = Is.LessThan(name1, name2, StringComparison.CurrentCulture);
```

## Callback Usage

### Use Callbacks for Side Effects Only

```csharp
// Good: Logging, metrics, notifications
ThrowIf.NullOrEmpty(email, 
    SGuardCallbacks.OnFailure(() => logger.LogWarning("Missing email")));

// Bad: Critical logic in callbacks
ThrowIf.NullOrEmpty(email, 
    SGuardCallbacks.OnFailure(() => user.Email = "default@example.com"));
```

### Keep Callbacks Lightweight

```csharp
// Good: Quick logging
SGuardCallbacks.OnFailure(() => logger.LogWarning("Validation failed"));

// Bad: Expensive operation
SGuardCallbacks.OnFailure(() => SendEmailNotification());
```

## Constructor Validation

### Validate All Parameters

```csharp
public class User
{
    public User(string username, string email, int age)
    {
        ThrowIf.NullOrEmpty(username);
        ThrowIf.NullOrEmpty(email);
        ThrowIf.LessThan(age, 0);
        
        Username = username;
        Email = email;
        Age = age;
    }
}
```

### Use Records for Simple DTOs

```csharp
// A positional record already has a primary constructor with these parameters,
// so declaring another one with the same signature doesn't compile (CS0111).
// Use a nominal record with an explicit constructor instead.
public record CreateUserRequest
{
    public CreateUserRequest(string username, string email, int age)
    {
        ThrowIf.NullOrEmpty(username);
        ThrowIf.NullOrEmpty(email);
        ThrowIf.LessThan(age, 0);

        Username = username;
        Email = email;
        Age = age;
    }

    public string Username { get; }
    public string Email { get; }
    public int Age { get; }
}
```

The properties are get-only, so a `with` expression can't assign new values that skip these checks.

## Testing

### Test Both Valid and Invalid Cases

```csharp
[Fact]
public void CreateUser_ValidInput_CreatesUser()
{
    var user = new User("john", "john@example.com", 25);
    Assert.NotNull(user);
}

[Fact]
public void CreateUser_NegativeAge_ThrowsException()
{
    // Assert.Throws checks the exact type; the guard throws LessThanException
    Assert.Throws<LessThanException>(() => 
        new User("john", "john@example.com", -1));

    // Or accept any ArgumentException, including derived types
    Assert.ThrowsAny<ArgumentException>(() => 
        new User("john", "john@example.com", -1));
}
```

### Test Exception Types

```csharp
[Fact]
public void ValidateOrder_InsufficientStock_ThrowsCorrectException()
{
    var ex = Assert.Throws<InsufficientStockException>(() => 
        service.ValidateOrder(order));
    
    Assert.Equal("SKU123", ex.Sku);
}
```

## Performance Considerations

### Use Selectors in Loops Carefully

Compiled selectors are cached, but each selector call still builds an expression tree and hashes it to find the cached delegate, which costs
microseconds rather than the nanoseconds of a direct check (see [Expression Caching](../core-concepts/expression-caching)):

```csharp
// Fastest: direct check (but throws NullReferenceException if order.Customer is null)
foreach (var order in orders)
{
    ThrowIf.NullOrEmpty(order.Customer.Email);
}

// Slower, null-safe: a null Customer counts as empty
foreach (var order in orders)
{
    ThrowIf.NullOrEmpty(order, o => o.Customer.Email);
}
```

### Avoid Validation in Hot Loops

```csharp
// Bad: Validating inside loop
for (int i = 0; i < 1000000; i++)
{
    ThrowIf.LessThan(i, 0);  // Unnecessary
    Process(i);
}

// Good: Validate once
ThrowIf.LessThan(maxIterations, 0);
for (int i = 0; i < maxIterations; i++)
{
    Process(i);
}
```

## Code Organization

### Group Related Validations

```csharp
public void ProcessOrder(Order order)
{
    // Structure validation
    ValidateOrderStructure(order);
    
    // Business rules validation
    ValidateBusinessRules(order);
    
    // Process
    ExecuteOrder(order);
}

private void ValidateOrderStructure(Order order)
{
    ThrowIf.NullOrEmpty(order);                  // null check only for a class instance
    ThrowIf.NullOrEmpty(order, o => o.Customer); // null, or every Customer property empty
    ThrowIf.NullOrEmpty(order, o => o.Items);    // null or no items
}
```

### Create Reusable Validators

```csharp
public static class OrderValidators
{
    public static void ValidateQuantity(int quantity)
    {
        ThrowIf.LessThanOrEqual(quantity, 0,
            new ArgumentException("Quantity must be positive"));
    }
    
    public static void ValidatePrice(decimal price)
    {
        ThrowIf.LessThan(price, 0m,
            new ArgumentException("Price cannot be negative"));
    }
}
```

## Common Anti-Patterns

### ❌ Swallowing Validation Exceptions

```csharp
// Bad: Hiding validation failures
try
{
    ThrowIf.NullOrEmpty(username);
}
catch
{
    // Silently continue—validation failed but we ignore it
}
```

### ❌ Validating After Using

```csharp
// Bad: Using before validating
var length = username.Length;  // Might throw NullReferenceException
ThrowIf.NullOrEmpty(username);
```

### ❌ Checking a Whole Object When You Mean One Member

A complex member counts as empty only if **all** of its properties are null or empty:

```csharp
// Bad: passes as long as any Address property is set, even if Street is empty
ThrowIf.NullOrEmpty(order, o => o.Customer.Address);

// Good: check the member you need; null anywhere on the path also counts as empty
ThrowIf.NullOrEmpty(order, o => o.Customer.Address.Street);
```

## Next Steps

- [Performance](./performance) - Optimization guidelines
- [Real-World Examples](../guides/real-world-examples) - See best practices in action
- [API Reference](../api/throwif) - Complete API documentation
