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
    ThrowIf.LessThan(amount, 0);
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
/// <exception cref="ArgumentException">
/// Thrown when username is null/empty or age is negative.
/// </exception>
public User CreateUser(string username, int age)
{
    ThrowIf.NullOrEmpty(username);
    ThrowIf.LessThan(age, 0);
    // ...
}
```

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
// Identifiers, keys, paths, protocols
ThrowIf.LessThan(apiVersion, "1.0", StringComparison.Ordinal);
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
// Records with validation in constructor
public record CreateUserRequest(string Username, string Email, int Age)
{
    public CreateUserRequest(string Username, string Email, int Age)
    {
        ThrowIf.NullOrEmpty(Username);
        ThrowIf.NullOrEmpty(Email);
        ThrowIf.LessThan(Age, 0);
        
        this.Username = Username;
        this.Email = Email;
        this.Age = Age;
    }
}
```

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
    Assert.Throws<ArgumentException>(() => 
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

Expression caching helps, but direct checks are fastest:

```csharp
// Good: Direct check (fastest)
foreach (var order in orders)
{
    ThrowIf.NullOrEmpty(order.Customer.Email);
}

// Acceptable: Selector (cached, still fast)
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
    ThrowIf.NullOrEmpty(order);
    ThrowIf.NullOrEmpty(order, o => o.Customer);
    ThrowIf.NullOrEmpty(order, o => o.Items);
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
        ThrowIf.LessThan(price, 0,
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

### ❌ Over-Nesting Selectors

```csharp
// Bad: Too complex
ThrowIf.NullOrEmpty(order, o => o.Customer.Address.Street.Name);

// Good: Validate at each level
ThrowIf.NullOrEmpty(order, o => o.Customer);
ThrowIf.NullOrEmpty(order, o => o.Customer.Address);
ThrowIf.NullOrEmpty(order, o => o.Customer.Address.Street);
```

## Next Steps

- [Performance](./performance) - Optimization guidelines
- [Real-World Examples](../guides/real-world-examples) - See best practices in action
- [API Reference](../api/throwif) - Complete API documentation
