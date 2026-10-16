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
ThrowIf.OutOfRange(port, 1, 65535); // Throws if port is outside 1..65535
```

A passing guard returns the checked value, so it can initialize a field or property:

```csharp
public sealed class Customer(string email)
{
    public string Email { get; } = ThrowIf.NullOrWhiteSpace(email);
}
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

- **`NullOrEmpty`**: Checks for `null`, `""` (whitespace is *not* empty), empty collections and arrays, and
  default values of value types (`0`, `false`, `Guid.Empty`, `default(DateTime)`, ...). A non-null class instance is
  never considered empty on its own; its properties are not inspected.
- **Selectors**: Check a member of an object (e.g., `ThrowIf.NullOrEmpty(order, o => o.Customer.Name)`). A `null`
  anywhere on the path counts as empty. When the selected member is itself a complex object, it counts as empty only
  if **all** of its readable properties are null or empty. Selectors don't work under Native AOT.
- **`Null`**: Checks for `null` only; empty strings and collections, `0` and `false` pass. A nullable value type is
  unwrapped: `int count = ThrowIf.Null(maybeCount);`.
- **`NullOrWhiteSpace`**: Checks for `null`, `""` and strings of white space only.

```csharp
ThrowIf.NullOrEmpty(str);
ThrowIf.NullOrEmpty(list);
ThrowIf.NullOrEmpty(obj, x => x.Property);
ThrowIf.Null(repository);
ThrowIf.NullOrWhiteSpace(displayName);

bool isEmpty = Is.NullOrEmpty(collection);
```

### Comparison Guards

All comparison guards are generic (`TLeft : IComparable<TRight>`). `Is.*` comparisons, `OutOfRange`, `Between`,
`Equal` and `NotEqual` have overloads that take a `StringComparison`; the four `ThrowIf` relational guards don't.

- **`LessThan`**: `value < other`
- **`LessThanOrEqual`**: `value <= other`
- **`GreaterThan`**: `value > other`
- **`GreaterThanOrEqual`**: `value >= other`
- **`OutOfRange`**: outside an inclusive range (`value < min || value > max`)
- **`Between`**: inside an inclusive range (`min <= value <= max`)
- **`Negative`**, **`NegativeOrZero`**, **`Zero`**: the sign of any number type
- **`Equal`**, **`NotEqual`**: equality with `EqualityComparer<T>.Default`

```csharp
ThrowIf.LessThan(age, 0);
ThrowIf.OutOfRange(score, 0, 100);  // Throws if score < 0 or score > 100
ThrowIf.Between(value, 10, 20);     // Throws if 10 <= value <= 20
ThrowIf.NegativeOrZero(quantity);   // Throws if quantity <= 0

bool inRange = Is.Between(score, 0, 100);
bool isLess = Is.LessThan("apple", "banana", StringComparison.Ordinal);
```

The left operand's type decides the comparison, so literals must match it: for a `decimal` use `0m`, not `0`
(`decimal` does not implement `IComparable<int>`).

**NaN:** if any operand is a floating-point NaN (`double`, `float`, `Half`, `NFloat`), `Is.*` comparisons return
`false` and `ThrowIf.*` comparisons throw. A check written as `if (Is.GreaterThan(x, max)) reject();` therefore lets
NaN through; prefer `ThrowIf.*` or `Is.OutOfRange(x, min, max)` for bound checks.

### Strings and Enums

- **`LongerThan`**, **`ShorterThan`**: the length of a string. The message never contains the string.
- **`NotDefined`**: an enum value that its type doesn't declare, such as `(OrderStatus)42`.
- **`Is.Email`**, **`Is.Url`** (`http`/`https` only), **`Is.CreditCard`** (Luhn) and **`Is.Match`** (a regular
  expression): input validators, `Is.*` only.

```csharp
Name = ThrowIf.LongerThan(name, 200);
var status = ThrowIf.NotDefined(request.Status);

if (!Is.Url(request.Website)) errors["website"] = ["Enter an http or https address."];
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

On an empty collection they follow LINQ: `Is.All` returns `true` and `Is.Any` returns `false`. So
`ThrowIf.All(empty, ...)` throws and `ThrowIf.Any(empty, ...)` does not.

## Guard Semantics

### Between is Inclusive

The `Between` guard uses **inclusive** comparisons by design:

```csharp
Is.Between(5, 1, 10);   // true (5 is between 1 and 10, inclusive)
Is.Between(1, 1, 10);   // true (min is allowed)
Is.Between(10, 1, 10);  // true (max is allowed)
```

For **`ThrowIf.Between`**, it throws if the value **is** within the range; `ThrowIf.OutOfRange` is its opposite:

```csharp
ThrowIf.Between(5, 1, 10);     // Throws (5 is in range)
ThrowIf.Between(0, 1, 10);     // Does NOT throw (0 is outside range)
ThrowIf.OutOfRange(0, 1, 10);  // Throws (0 is outside range)
ThrowIf.OutOfRange(10, 1, 10); // Does NOT throw (max is allowed)
```

### CallerArgumentExpression

SGuard uses `CallerArgumentExpression` to automatically capture argument names:

```csharp
ThrowIf.NullOrEmpty(user.Email);
// NullOrEmptyException: "Value 'user.Email' is null or empty."
// ex.ParamName == "user.Email"
```

The built-in exceptions derive from `ArgumentException` (the range and comparison ones through
`ArgumentOutOfRangeException`, `NullException` through `ArgumentNullException`). Messages name the expressions but leave out the checked
values unless you enable `SGuardOptions.IncludeValuesInExceptions` (see
[Custom Exceptions](./custom-exceptions#including-values-in-messages)).

No manual parameter name strings required!

## Next Steps

- [Callbacks](./callbacks) - Add side effects on success/failure
- [Custom Exceptions](./custom-exceptions) - Use your own exception types
- [API Reference](../api/throwif) - Complete method signatures
