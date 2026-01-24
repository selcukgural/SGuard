---
sidebar_position: 3
---

# Collection Validation

Master predicate-based validation for collections with Any and All guards.

## Overview

SGuard provides two powerful collection guards:

- **`Any`**: At least one element matches the predicate
- **`All`**: All elements match the predicate

Both work with any `IEnumerable<T>` and accept a predicate (`Func<T, bool>`).

## Any Guard

### ThrowIf.Any

Throws if **at least one** element matches the predicate:

```csharp
// Throws if any item is null
ThrowIf.Any(items, i => i is null);

// Throws if any number is negative
ThrowIf.Any(numbers, n => n < 0);

// Throws if any order has invalid quantity
ThrowIf.Any(orders, o => o.Quantity <= 0);
```

**Use case**: Ensuring no element violates a rule.

### Is.Any

Returns `true` if **at least one** element matches:

```csharp
// Check if collection has any positive numbers
bool hasPositive = Is.Any(numbers, n => n > 0);

// Check if any user is active
bool hasActiveUser = Is.Any(users, u => u.IsActive);

// Check if any item is out of stock
bool hasOutOfStock = Is.Any(products, p => p.Stock == 0);
```

**Use case**: Finding if a condition exists in the collection.

## All Guard

### ThrowIf.All

Throws if **all** elements match the predicate:

```csharp
// Throws if all numbers are negative
ThrowIf.All(numbers, n => n < 0);

// Throws if all users are inactive
ThrowIf.All(users, u => !u.IsActive);

// Throws if all items are out of stock
ThrowIf.All(products, p => p.Stock == 0);
```

**Use case**: Preventing situations where every element has an undesirable property.

### Is.All

Returns `true` if **all** elements match:

```csharp
// Check if all numbers are positive
bool allPositive = Is.All(numbers, n => n > 0);

// Check if all users have verified emails
bool allVerified = Is.All(users, u => u.EmailVerified);

// Check if all items are in stock
bool allInStock = Is.All(products, p => p.Stock > 0);
```

**Use case**: Verifying that every element satisfies a requirement.

## Real-World Examples

### Shopping Cart Validation

```csharp
public class CartValidator
{
    public void ValidateCart(Cart cart)
    {
        ThrowIf.NullOrEmpty(cart);
        ThrowIf.NullOrEmpty(cart.Items);
        
        // Ensure no negative quantities
        ThrowIf.Any(cart.Items, i => i.Quantity <= 0,
            new InvalidOperationException("Cart contains items with invalid quantities"));
        
        // Ensure at least one item is available
        if (!Is.Any(cart.Items, i => i.IsAvailable))
        {
            throw new InvalidOperationException("Cart contains no available items");
        }
    }
}
```

### User Collection Validation

```csharp
public void ProcessUsers(List<User> users)
{
    ThrowIf.NullOrEmpty(users);
    
    // Reject if any user is null
    ThrowIf.Any(users, u => u is null,
        new ArgumentException("User list contains null entries"));
    
    // Reject if all users are inactive
    ThrowIf.All(users, u => !u.IsActive,
        new InvalidOperationException("All users are inactive"));
    
    // Proceed with processing...
}
```

### Order Validation

```csharp
public class OrderService
{
    public void ValidateOrders(IEnumerable<Order> orders)
    {
        ThrowIf.NullOrEmpty(orders);
        
        // Ensure no orders have invalid totals
        ThrowIf.Any(orders, o => o.Total <= 0,
            new InvalidOperationException("One or more orders have invalid totals"));
        
        // Warn if all orders are on hold
        if (Is.All(orders, o => o.Status == OrderStatus.OnHold))
        {
            logger.LogWarning("All orders are on hold");
        }
    }
}
```

### Stock Level Validation

```csharp
public class InventoryManager
{
    public void CheckInventory(List<Product> products)
    {
        // Alert if any product is out of stock
        if (Is.Any(products, p => p.Stock == 0))
        {
            notificationService.Send("Some products are out of stock");
        }
        
        // Critical alert if all products are out of stock
        ThrowIf.All(products, p => p.Stock == 0,
            new InvalidOperationException("All products are out of stock"));
    }
}
```

### Permission Validation

```csharp
public void ExecuteAction(User user, string action)
{
    ThrowIf.NullOrEmpty(user);
    ThrowIf.NullOrEmpty(user, u => u.Roles);
    
    // Ensure user has at least one role with permission
    if (!Is.Any(user.Roles, r => r.HasPermission(action)))
    {
        throw new UnauthorizedAccessException($"User lacks permission for '{action}'");
    }
}
```

## Combining Any and All

You can combine both guards for comprehensive validation:

```csharp
public void ValidateTestResults(List<TestResult> results)
{
    ThrowIf.NullOrEmpty(results);
    
    // Ensure at least one test passed
    if (!Is.Any(results, r => r.Passed))
    {
        throw new Exception("All tests failed");
    }
    
    // Warn if any critical test failed
    if (Is.Any(results, r => !r.Passed && r.IsCritical))
    {
        logger.LogError("Critical test(s) failed");
    }
    
    // Success if all tests passed
    if (Is.All(results, r => r.Passed))
    {
        logger.LogInformation("All tests passed");
    }
}
```

## With Custom Exceptions

```csharp
public class DomainValidationException : Exception
{
    public DomainValidationException(string message) : base(message) { }
}

ThrowIf.Any(items, i => i.IsInvalid, 
    new DomainValidationException("Collection contains invalid items"));

ThrowIf.All(users, u => u.IsBlocked,
    new DomainValidationException("All users are blocked"));
```

## With Callbacks

```csharp
// Log when any item fails validation
ThrowIf.Any(
    items, 
    i => i.IsInvalid,
    SGuardCallbacks.OnFailure(() => logger.LogError("Invalid items detected")));

// Metrics for all-valid collections
bool allValid = Is.All(
    items, 
    i => i.IsValid,
    SGuardCallbacks.OnSuccess(() => metrics.Increment("validation.all.valid")));
```

## Empty Collections

Both guards handle empty collections gracefully:

```csharp
// Returns false (no elements match)
Is.Any(emptyList, x => true);   // false

// Returns true (vacuous truth: all zero elements match)
Is.All(emptyList, x => false);  // true
```

**Best practice**: Check for null/empty collections first:

```csharp
ThrowIf.NullOrEmpty(items);
ThrowIf.Any(items, i => i is null);  // Safe now
```

## Performance Considerations

- **Short-circuit evaluation**: Both guards stop as soon as the result is determined
  - `Any` stops at the first matching element
  - `All` stops at the first non-matching element
- **LINQ-compatible**: Works with any `IEnumerable<T>`, including LINQ queries
- **Predicate caching**: Predicates are not cached (unlike selectors in `NullOrEmpty`)

## Common Patterns

### At Least One Valid Item

```csharp
if (!Is.Any(items, i => i.IsValid))
{
    throw new Exception("No valid items found");
}
```

### No Invalid Items

```csharp
ThrowIf.Any(items, i => !i.IsValid);
```

### All Items Valid

```csharp
if (!Is.All(items, i => i.IsValid))
{
    throw new Exception("Some items are invalid");
}
```

## Next Steps

- [String Comparisons](./string-comparisons) - Culture-aware string validation
- [Real-World Examples](./real-world-examples) - Complete validation scenarios
- [API Reference](../api/is) - Complete Is API
