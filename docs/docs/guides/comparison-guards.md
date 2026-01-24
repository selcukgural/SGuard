---
sidebar_position: 2
---

# Comparison Guards

Learn to use SGuard's comparison guards for range and relational validation.

## Overview

SGuard provides five comparison guards that work with any `IComparable<T>` type:

- **`LessThan`**: `value < other`
- **`LessThanOrEqual`**: `value <= other`
- **`GreaterThan`**: `value > other`
- **`GreaterThanOrEqual`**: `value >= other`
- **`Between`**: Inclusive range (`min <= value <= max`)

All comparison guards have **string-specific overloads** that accept `StringComparison` for culture-aware comparisons.

## Numeric Comparisons

### Basic Usage

```csharp
ThrowIf.LessThan(age, 0);
ThrowIf.GreaterThan(quantity, maxQuantity);
ThrowIf.LessThanOrEqual(balance, 0);

bool isValid = Is.Between(score, 0, 100);
bool isPositive = Is.GreaterThan(value, 0);
```

### Range Validation with Between

The `Between` guard performs **inclusive** range checks:

```csharp
// Checks if value is in [min, max] (inclusive)
bool inRange = Is.Between(value, min, max);

// Throws if value IS within range
ThrowIf.Between(value, min, max);
```

**Examples:**
```csharp
Is.Between(5, 1, 10);   // true (5 is in range)
Is.Between(1, 1, 10);   // true (min is allowed)
Is.Between(10, 1, 10);  // true (max is allowed)
Is.Between(0, 1, 10);   // false (0 is outside range)
```

## String Comparisons

### Culture-Aware Comparisons

String comparison guards accept `StringComparison` for proper cultural/ordinal handling:

```csharp
// Ordinal comparison
bool before = Is.LessThan("apple", "banana", StringComparison.Ordinal);

// Case-insensitive comparison
bool less = Is.LessThan("Apple", "banana", StringComparison.OrdinalIgnoreCase);

// Culture-aware comparison
bool cultureLess = Is.LessThan("straße", "strasse", StringComparison.InvariantCulture);
```

### Throw on Invalid Ordering

```csharp
// Throws if "zebra" > "apple" (which it is)
ThrowIf.GreaterThan("zebra", "apple", StringComparison.Ordinal);

// Throws if version IS between range
ThrowIf.Between("2.5", "2.0", "3.0", StringComparison.Ordinal);
```

## DateTime Comparisons

```csharp
DateTime now = DateTime.UtcNow;
DateTime deadline = GetDeadline();

ThrowIf.GreaterThan(now, deadline, 
    new InvalidOperationException("Deadline has passed"));

bool isExpired = Is.GreaterThan(expiryDate, DateTime.UtcNow);
```

## Custom IComparable Types

Any type implementing `IComparable<T>` works:

```csharp
public class Version : IComparable<Version>
{
    public int Major { get; }
    public int Minor { get; }
    
    public int CompareTo(Version other) { /* ... */ }
}

Version current = new Version(2, 1);
Version minimum = new Version(2, 0);

ThrowIf.LessThan(current, minimum);
bool isNewer = Is.GreaterThan(current, minimum);
```

## Real-World Examples

### Age Validation

```csharp
public class User
{
    public User(string username, int age)
    {
        ThrowIf.NullOrEmpty(username);
        ThrowIf.LessThan(age, 0);
        ThrowIf.GreaterThan(age, 130, 
            new ArgumentOutOfRangeException(nameof(age), "Age seems unrealistic"));
        
        Username = username;
        Age = age;
    }
}
```

### Quantity Validation

```csharp
public void AddToCart(Product product, int quantity)
{
    ThrowIf.NullOrEmpty(product);
    ThrowIf.LessThanOrEqual(quantity, 0, 
        new ArgumentException("Quantity must be positive", nameof(quantity)));
    ThrowIf.GreaterThan(quantity, product.StockQuantity, 
        new InvalidOperationException("Insufficient stock"));
    
    // Add to cart...
}
```

### Price Range Validation

```csharp
public void SetPrice(decimal price)
{
    const decimal MinPrice = 0.01m;
    const decimal MaxPrice = 10000m;
    
    ThrowIf.LessThan(price, MinPrice);
    ThrowIf.GreaterThan(price, MaxPrice);
    
    // Or use Between (note: throws if IN range)
    // ThrowIf.Between throws when value IS in range
    // So use Is.Between for validation:
    if (!Is.Between(price, MinPrice, MaxPrice))
    {
        throw new ArgumentOutOfRangeException(nameof(price));
    }
    
    Price = price;
}
```

### Date Range Validation

```csharp
public void ScheduleMeeting(DateTime start, DateTime end)
{
    DateTime now = DateTime.UtcNow;
    
    ThrowIf.LessThan(start, now, 
        new ArgumentException("Start time must be in the future"));
    ThrowIf.LessThanOrEqual(end, start, 
        new ArgumentException("End time must be after start time"));
    
    // Schedule meeting...
}
```

### Discount Percentage Validation

```csharp
public class DiscountCalculator
{
    public decimal ApplyDiscount(decimal amount, decimal discountPercent)
    {
        ThrowIf.LessThan(discountPercent, 0);
        ThrowIf.GreaterThan(discountPercent, 100);
        
        return amount * (1 - discountPercent / 100);
    }
}
```

## Combining with Callbacks

```csharp
ThrowIf.LessThan(
    value, 
    threshold, 
    SGuardCallbacks.OnFailure(() => logger.LogWarning("Value below threshold")));

bool isValid = Is.Between(
    score, 
    0, 
    100,
    SGuardCallbacks.OnSuccess(() => metrics.Increment("valid.score")));
```

## Best Practices

1. **Use Between for range checks**: It's clearer than combining LessThan and GreaterThan
2. **Specify StringComparison**: Always explicit with string comparisons
3. **Consider inclusive semantics**: Remember Between is inclusive on both ends
4. **Combine with custom exceptions**: Provide meaningful error messages for domain rules

## Common Patterns

### Exclusive Range Check

Since `Between` is inclusive, use boolean checks for exclusive ranges:

```csharp
// Exclusive: min < value < max
if (Is.GreaterThan(value, min) && Is.LessThan(value, max))
{
    // value is in exclusive range
}
```

### Clamping Values

```csharp
// Ensure value stays within bounds
if (!Is.Between(value, min, max))
{
    value = Is.LessThan(value, min) ? min : max;
}
```

## Next Steps

- [Collection Validation](./collection-validation) - Any/All guards
- [String Comparisons](./string-comparisons) - Deep dive into string handling
- [Custom Exceptions](../core-concepts/custom-exceptions) - Use domain exceptions
