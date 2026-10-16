---
sidebar_position: 2
---

# Comparison Guards

Learn to use SGuard's comparison guards for range and relational validation.

## Overview

SGuard provides comparison guards that work with any `IComparable<T>` type:

- **`LessThan`**: `value < other`
- **`LessThanOrEqual`**: `value <= other`
- **`GreaterThan`**: `value > other`
- **`GreaterThanOrEqual`**: `value >= other`
- **`OutOfRange`**: outside an inclusive range (`value < min || value > max`)
- **`Between`**: inside an inclusive range (`min <= value <= max`)

For numbers there are also sign guards, **`Negative`** (`< 0`), **`NegativeOrZero`** (`<= 0`) and **`Zero`**, and for any
type **`Equal`** and **`NotEqual`**.

For strings, the `Is.*` comparisons, `OutOfRange`, `Between`, `Equal` and `NotEqual` have overloads that take a
`StringComparison`. The four `ThrowIf` relational guards don't; see [String Comparisons](#string-comparisons).

`ThrowIf.*` throws when its condition is **true**: `ThrowIf.LessThan(x, 0)` throws if `x < 0`, and returns `x`
otherwise. The built-in exceptions (`LessThanException`, `OutOfRangeException`, `NegativeException`, ...) derive from
`ArgumentOutOfRangeException`, and so from `ArgumentException`.

## Numeric Comparisons

### Basic Usage

```csharp
ThrowIf.LessThan(age, 0);
ThrowIf.GreaterThan(quantity, maxQuantity);
var pageSize = ThrowIf.OutOfRange(request.PageSize, 1, 100);

bool isValid = Is.Between(score, 0, 100);
bool isPositive = Is.GreaterThan(value, 0);
```

The generic guards require `TLeft : IComparable<TRight>`. `decimal` doesn't implement `IComparable<int>`, so
`ThrowIf.LessThan(price, 0)` with a `decimal` price doesn't compile (CS0315). Use a literal of the same type: `0m`,
`0.0` for `double`, `0f` for `float`. For comparisons with zero, the [sign guards](#sign-guards) avoid the question.

### Sign Guards

```csharp
ThrowIf.Negative(balance);            // throws if balance < 0
ThrowIf.NegativeOrZero(quantity);     // throws if quantity <= 0
ThrowIf.Zero(divisor);                // throws if divisor == 0

bool overdrawn = Is.Negative(balance);
```

They work with every number type (`int`, `decimal`, `double`, `BigInteger`, ...). Negative zero (`-0.0`) is zero:
`Negative` lets it pass, `NegativeOrZero` and `Zero` reject it. Like the other comparisons, NaN fails `Negative` and
`NegativeOrZero`.

### Range Validation with OutOfRange and Between

Both range guards are **inclusive**:

```csharp
// Throws if value is outside [min, max]: the range check you usually want
ThrowIf.OutOfRange(value, min, max);

// Checks if value is in [min, max]
bool inRange = Is.Between(value, min, max);

// Throws if value IS within range
ThrowIf.Between(value, min, max);
```

:::warning
`ThrowIf.Between` throws when the value is **inside** the range. It does not enforce a range: `ThrowIf.Between(age, 18,
120)` rejects every valid age and lets `5` through. To require `min <= value <= max`, use `OutOfRange`:

```csharp
ThrowIf.OutOfRange(age, 18, 120);

// or, with your own exception
ThrowIf.OutOfRange(age, 18, 120, () => new DomainException("Members must be between 18 and 120."));
```
:::

**Examples:**
```csharp
Is.Between(5, 1, 10);   // true (5 is in range)
Is.Between(1, 1, 10);   // true (min is allowed)
Is.Between(10, 1, 10);  // true (max is allowed)
Is.Between(0, 1, 10);   // false (0 is outside range)
```

If `min` is greater than `max`, `Between` and `OutOfRange` throw an `ArgumentException` ("The minimum must be
less than or equal to the maximum.") instead of silently treating every value as out of range. This check runs when
`min` and `max` have the same type, and for the string overloads.

### NaN Values

If any operand is a floating-point NaN (`double`, `float`, `Half` or `NFloat`), every `Is.*` comparison returns `false`
(`Is.OutOfRange` returns `true`) and every `ThrowIf.*` comparison throws:

```csharp
Is.GreaterThan(double.NaN, 100.0);          // false
Is.Between(double.NaN, 0.0, 100.0);         // false
ThrowIf.GreaterThan(double.NaN, 100.0);     // throws GreaterThanException
ThrowIf.OutOfRange(double.NaN, 0.0, 100.0); // throws OutOfRangeException
ThrowIf.Negative(double.NaN);               // throws NegativeException
```

:::warning
A check written as "reject if too large" with `Is.*` lets NaN through, because the comparison returns `false`:

```csharp
if (Is.GreaterThan(amount, max)) { /* reject */ } // NaN is NOT rejected
```

Use `ThrowIf.GreaterThan(amount, max)` or `ThrowIf.OutOfRange(amount, min, max)`, or test that the value is inside
the allowed range with `if (Is.OutOfRange(amount, min, max)) { /* reject */ }`, which also rejects NaN.
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

`ThrowIf.OutOfRange`, `ThrowIf.Between`, `ThrowIf.Equal` and `ThrowIf.NotEqual` take a `StringComparison`. For the
relational guards, test with `Is.*`:

```csharp
// Throws because "zebra" > "apple"
if (Is.GreaterThan("zebra", "apple", StringComparison.Ordinal))
{
    throw new InvalidOperationException("Out of order");
}

// Throws if the code is outside "A00".."W99"
ThrowIf.OutOfRange(code, "A00", "W99", StringComparison.Ordinal);

// Throws if the code IS inside the reserved range
ThrowIf.Between(code, "X00", "X99", StringComparison.Ordinal);

// Throws unless the currencies match, ignoring case
ThrowIf.NotEqual(payment.Currency, order.Currency, StringComparison.OrdinalIgnoreCase);
```

String comparisons are lexicographic. Don't use them for version numbers (`"10.0.0"` is less than `"2.0.0"`), prefixes
or paths; see [String Comparisons](./string-comparisons).

## DateTime Comparisons

```csharp
DateTime now = DateTime.UtcNow;
DateTime deadline = GetDeadline();

ThrowIf.GreaterThan(now, deadline, () => new InvalidOperationException("Deadline has passed"));

bool isExpired = Is.LessThan(expiryDate, DateTime.UtcNow); // expiry date is in the past
```

## Equality

`Equal` and `NotEqual` compare with `EqualityComparer<T>.Default`, so records and other types with value equality work:

```csharp
ThrowIf.Equal(transfer.TargetAccount, transfer.SourceAccount); // a transfer to the same account
ThrowIf.NotEqual(invoice.Total, payment.Amount, () => new PaymentMismatchException(invoice.Id));
```

`null` operands are allowed, and two `null` values are equal.

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
        Username = ThrowIf.NullOrWhiteSpace(username);
        Age = ThrowIf.OutOfRange(age, 0, 130);
    }
}
```

### Quantity Validation

```csharp
public void AddToCart(Product product, int quantity)
{
    ThrowIf.Null(product);
    ThrowIf.NegativeOrZero(quantity, () => new ArgumentException("Quantity must be positive", nameof(quantity)));
    ThrowIf.GreaterThan(quantity, product.StockQuantity, () => new InvalidOperationException("Insufficient stock"));
    
    // Add to cart...
}
```

### Price Range Validation

```csharp
public void SetPrice(decimal price)
{
    const decimal MinPrice = 0.01m;
    const decimal MaxPrice = 10000m;
    
    Price = ThrowIf.OutOfRange(price, MinPrice, MaxPrice);
}
```

### Date Range Validation

```csharp
public void ScheduleMeeting(DateTime start, DateTime end)
{
    DateTime now = DateTime.UtcNow;
    
    ThrowIf.LessThan(start, now, () => new ArgumentException("Start time must be in the future"));
    ThrowIf.LessThanOrEqual(end, start, () => new ArgumentException("End time must be after start time"));
    
    // Schedule meeting...
}
```

### Discount Percentage Validation

```csharp
public class DiscountCalculator
{
    public decimal ApplyDiscount(decimal amount, decimal discountPercent)
    {
        ThrowIf.OutOfRange(discountPercent, 0m, 100m);


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

1. **Use `ThrowIf.OutOfRange` for range checks**: It's clearer than combining LessThan and GreaterThan; remember that
   `ThrowIf.Between` throws when the value is *inside* the range
2. **Specify StringComparison**: Always explicit with string comparisons, and never use string ordering for versions,
   prefixes or paths
3. **Prefer `ThrowIf.*` or `Is.OutOfRange` for floating-point input**: They reject NaN; `if (Is.GreaterThan(...))` doesn't
4. **Consider inclusive semantics**: Remember Between is inclusive on both ends
5. **Combine with custom exceptions**: Pass a factory (`() => new DomainException(...)`) for domain rules

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
if (Is.OutOfRange(value, min, max))
{
    value = Is.LessThan(value, min) ? min : max;
}
```

## Next Steps

- [Collection Validation](./collection-validation) - Any/All guards
- [String Comparisons](./string-comparisons) - Deep dive into string handling
- [Custom Exceptions](../core-concepts/custom-exceptions) - Use domain exceptions
