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

For strings, `Is.LessThan`, `Is.LessThanOrEqual`, `Is.GreaterThan`, `Is.GreaterThanOrEqual`, `Is.Between` and
`ThrowIf.Between` have overloads that take a `StringComparison`. The other `ThrowIf` comparison guards don't; see
[String Comparisons](#string-comparisons).

`ThrowIf.*` throws when its condition is **true**: `ThrowIf.LessThan(x, 0)` throws if `x < 0`. The built-in exceptions
(`LessThanException`, `GreaterThanException`, `BetweenException`, ...) derive from `ArgumentException`.

## Numeric Comparisons

### Basic Usage

```csharp
ThrowIf.LessThan(age, 0);
ThrowIf.GreaterThan(quantity, maxQuantity);
ThrowIf.LessThanOrEqual(balance, 0m); // balance is a decimal

bool isValid = Is.Between(score, 0, 100);
bool isPositive = Is.GreaterThan(value, 0);
```

The generic guards require `TLeft : IComparable<TRight>`. `decimal` doesn't implement `IComparable<int>`, so
`ThrowIf.LessThan(price, 0)` with a `decimal` price doesn't compile (CS0315). Use a literal of the same type: `0m`,
`0.0` for `double`, `0f` for `float`.

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

If `min` is greater than `max`, both `Is.Between` and `ThrowIf.Between` throw an `ArgumentException` ("The minimum must be
less than or equal to the maximum.") instead of silently treating every value as out of range. This check runs when
`min` and `max` have the same type, and for the string overloads.

### NaN Values

If any operand is a floating-point NaN (`double`, `float` or `Half`), every `Is.*` comparison returns `false` and every
`ThrowIf.*` comparison throws:

```csharp
Is.GreaterThan(double.NaN, 100.0);         // false
Is.Between(double.NaN, 0.0, 100.0);        // false
ThrowIf.GreaterThan(double.NaN, 100.0);    // throws GreaterThanException
ThrowIf.Between(double.NaN, 0.0, 100.0);   // throws BetweenException
```

:::warning
A check written as "reject if too large" with `Is.*` lets NaN through, because the comparison returns `false`:

```csharp
if (Is.GreaterThan(amount, max)) { /* reject */ } // NaN is NOT rejected
```

Use `ThrowIf.GreaterThan(amount, max)`, or test that the value is inside the allowed range with
`if (!Is.Between(amount, min, max)) { /* reject */ }`, which also rejects NaN.
:::

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

`ThrowIf.Between` is the only `ThrowIf` guard with a `StringComparison` overload. For the others, test with `Is.*`:

```csharp
// Throws because "zebra" > "apple"
if (Is.GreaterThan("zebra", "apple", StringComparison.Ordinal))
{
    throw new InvalidOperationException("Out of order");
}

// Throws if the code IS inside the reserved range
ThrowIf.Between(code, "X00", "X99", StringComparison.Ordinal);
```

String comparisons are lexicographic. Don't use them for version numbers (`"10.0.0"` is less than `"2.0.0"`), prefixes
or paths; see [String Comparisons](./string-comparisons).

## DateTime Comparisons

```csharp
DateTime now = DateTime.UtcNow;
DateTime deadline = GetDeadline();

ThrowIf.GreaterThan(now, deadline, 
    new InvalidOperationException("Deadline has passed"));

bool isExpired = Is.LessThan(expiryDate, DateTime.UtcNow); // expiry date is in the past
```

## Custom IComparable Types

Any type implementing `IComparable<T>` works, including `System.Version`:

```csharp
Version current = new Version(2, 1);
Version minimum = new Version(2, 0);

ThrowIf.LessThan(current, minimum);
bool isNewer = Is.GreaterThan(current, minimum); // true
```

Your own types work the same way:

```csharp
public sealed class ApiLevel : IComparable<ApiLevel>
{
    public ApiLevel(int major, int minor)
    {
        Major = major;
        Minor = minor;
    }

    public int Major { get; }
    public int Minor { get; }

    public int CompareTo(ApiLevel? other)
    {
        if (other is null) return 1;
        int byMajor = Major.CompareTo(other.Major);
        return byMajor != 0 ? byMajor : Minor.CompareTo(other.Minor);
    }
}

ThrowIf.LessThan(new ApiLevel(2, 1), new ApiLevel(2, 0)); // doesn't throw
```

## Real-World Examples

### Age Validation

```csharp
public class User
{
    public string Username { get; }
    public int Age { get; }

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
        ThrowIf.LessThan(discountPercent, 0m);
        ThrowIf.GreaterThan(discountPercent, 100m);
        
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
2. **Specify StringComparison**: Always explicit with string comparisons, and never use string ordering for versions,
   prefixes or paths
3. **Prefer `ThrowIf.*` or `!Is.Between` for floating-point input**: They reject NaN; `if (Is.GreaterThan(...))` doesn't
4. **Consider inclusive semantics**: Remember Between is inclusive on both ends
5. **Combine with custom exceptions**: Provide meaningful error messages for domain rules

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
