---
sidebar_position: 1
---

# Guard Methods

SGuard provides two complementary sets of guard methods for validation.

## The Two APIs

### ThrowIf.* (Exception Throwing)

Fail fast by throwing informative exceptions when validation fails.

```csharp
ThrowIf.NullOrEmpty(user.Email);
ThrowIf.LessThan(age, 0);
ThrowIf.Between(value, min, max); // Throws if value IS between min and max
```

**When to use:**
- Input validation at method boundaries
- Constructor validation
- Enforcing invariants that must never be violated

### Is.* (Boolean Returns)

Return boolean values for control-flow-friendly checks.

```csharp
if (Is.Between(value, min, max)) 
{
    // value is in range
}

bool isValid = Is.LessThan(a, b);
```

**When to use:**
- Conditional logic and branching
- Validation with custom error handling
- When you need to check multiple conditions before acting

## Available Guard Methods

### Null/Empty Validation

- **`NullOrEmpty`**: Validates primitives, collections, strings, and complex objects
- **Deep validation**: For objects with selectors (e.g., `ThrowIf.NullOrEmpty(order, o => o.Customer.Name)`)

```csharp
ThrowIf.NullOrEmpty(str);
ThrowIf.NullOrEmpty(list);
ThrowIf.NullOrEmpty(obj, x => x.Property);

bool isEmpty = Is.NullOrEmpty(collection);
```

### Comparison Guards

All comparison guards support generic types and string-specific overloads with `StringComparison`:

- **`LessThan`**: `value < other`
- **`LessThanOrEqual`**: `value <= other`
- **`GreaterThan`**: `value > other`
- **`GreaterThanOrEqual`**: `value >= other`
- **`Between`**: Inclusive range check (`min <= value <= max`)

```csharp
ThrowIf.LessThan(age, 0);
ThrowIf.Between(value, 10, 20); // Throws if 10 <= value <= 20

bool inRange = Is.Between(score, 0, 100);
bool isLess = Is.LessThan("apple", "banana", StringComparison.Ordinal);
```

### Collection Guards

Predicate-based validation for collections:

- **`Any`**: At least one element matches the predicate
- **`All`**: All elements match the predicate

```csharp
ThrowIf.Any(items, i => i is null);
ThrowIf.All(numbers, n => n < 0); // Throws if all numbers are negative

bool hasPositive = Is.Any(numbers, n => n > 0);
bool allValid = Is.All(items, i => i.IsValid());
```

## Guard Semantics

### Between is Inclusive

The `Between` guard uses **inclusive** comparisons by design:

```csharp
Is.Between(5, 1, 10);   // true (5 is between 1 and 10, inclusive)
Is.Between(1, 1, 10);   // true (min is allowed)
Is.Between(10, 1, 10);  // true (max is allowed)
```

For **`ThrowIf.Between`**, it throws if the value **is** within the range:

```csharp
ThrowIf.Between(5, 1, 10);  // Throws (5 is in range)
ThrowIf.Between(0, 1, 10);  // Does NOT throw (0 is outside range)
```

### CallerArgumentExpression

SGuard uses `CallerArgumentExpression` to automatically capture argument names:

```csharp
ThrowIf.NullOrEmpty(user.Email);
// Exception: "Value cannot be null or empty. (Parameter 'user.Email')"
```

No manual parameter name strings required!

## Next Steps

- [Callbacks](./callbacks) - Add side effects on success/failure
- [Custom Exceptions](./custom-exceptions) - Use your own exception types
- [API Reference](../api/throwif) - Complete method signatures
