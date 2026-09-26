---
sidebar_position: 1
---

# Null/Empty Checks

Master null and empty validation for various types with SGuard.

## Overview

The `NullOrEmpty` guard works with any value. What counts as "empty" depends on the type:

| Value | Counts as null or empty |
|-------|-------------------------|
| Reference types | `null` |
| `string` | `null` or `""` (whitespace-only strings are **not** empty) |
| Arrays, collections, other `IEnumerable` | `null` or no elements |
| Value types | `default(T)`: `0`, `0m`, `false`, `Guid.Empty`, `default(DateTime)`, any default struct |
| Nullable value types | no value, **or** a value equal to the default (`int? x = 0` is empty) |
| Date/time types | zero ticks (`DateTime`, `TimeSpan`, `TimeOnly`, `DateTimeOffset`), `DateOnly.MinValue` |
| Other class instances | `null` only; a non-null object is **not** inspected property by property |

`ThrowIf.NullOrEmpty` throws a `NullOrEmptyException` (an `ArgumentException`) when the value is null or empty;
`Is.NullOrEmpty` returns `true` in that case.

## String Validation

```csharp
ThrowIf.NullOrEmpty(username);
ThrowIf.NullOrEmpty(email);

bool isEmpty = Is.NullOrEmpty(name);
```

**What's checked:**
- `null` reference
- Empty string (`""`)

Whitespace is not treated as empty: `Is.NullOrEmpty("   ")` returns `false`. If blank input must be rejected, check it
yourself:

```csharp
if (string.IsNullOrWhiteSpace(username))
{
    throw new ArgumentException("Username is required.", nameof(username));
}
```

## Collection Validation

```csharp
ThrowIf.NullOrEmpty(items);
ThrowIf.NullOrEmpty(dictionary);

bool hasItems = !Is.NullOrEmpty(list);
```

**What's checked:**
- `null` reference
- Empty collection (`ICollection.Count == 0`, otherwise the sequence yields no element)

For a lazy `IEnumerable` that isn't a collection, the guard starts enumerating it to see whether it has a first element.

:::warning
Starting the enumeration runs the code behind the sequence. An `IQueryable` (Entity Framework, for example) sends a
query to the database, an iterator method runs up to its first `yield`, and a sequence that can only be read once
(a network or file reader) loses its first element. Materialize the sequence first (`ToList()`) or check it with a call
that is cheap for your source, such as `await query.AnyAsync()`.
:::

## Value Types and Nullable Value Types

```csharp
int? count = GetCount();
ThrowIf.NullOrEmpty(count);   // throws if count is null OR 0

bool isEmpty = Is.NullOrEmpty(0);         // true
bool isFalse = Is.NullOrEmpty(false);     // true
bool noId    = Is.NullOrEmpty(Guid.Empty); // true
```

**What's checked:**
- `.HasValue == false` for nullable value types
- The type's default value: `0` for numbers, `false`, `Guid.Empty`, `default(DateTime)` and any default struct

:::warning
`NullOrEmpty` rejects legitimate zeros and `false`. Don't use it for values where `0` or `false` is valid input, such as
a quantity that may be zero, a discount of `0m` or an `IsActive = false` flag. Check `HasValue` (or `is null`) for
"was it provided?" and use the [comparison guards](./comparison-guards) for ranges:

```csharp
int? discount = request.Discount;

if (discount is null)
{
    throw new ArgumentException("Discount is required.", nameof(request.Discount));
}

ThrowIf.LessThan(discount.Value, 0); // 0 is allowed, negative values are not
```
:::

## Reference Types

```csharp
ThrowIf.NullOrEmpty(user);
ThrowIf.NullOrEmpty(order);

bool exists = !Is.NullOrEmpty(entity);
```

**What's checked:**
- `null` reference

Nothing else: a `User` whose `Username` and `Email` are empty strings still passes `ThrowIf.NullOrEmpty(user)`. See
[Complex Type Validation](#complex-type-validation).

## Deep Validation with Selectors

Use selectors to validate nested properties:

```csharp
// Validate nested property
ThrowIf.NullOrEmpty(order, o => o.Customer.Name);
ThrowIf.NullOrEmpty(user, u => u.Profile.Email);

// Multiple levels deep
ThrowIf.NullOrEmpty(order, o => o.Customer.Address.City);
```

A `null` anywhere on the path (for example `order.Customer` being `null`) counts as empty, so the guard throws instead
of a `NullReferenceException`. The selected member is then checked with the rules in the table above.

**Benefits:**
- **Expression caching**: Selectors are compiled once and cached by expression structure. A selector that captures a
  local variable is recompiled on every call. See [Expression Caching](../core-concepts/expression-caching).
- **Precise error messages**: `CallerArgumentExpression` captures the selector text
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

**Error message for nested validation** (`ParamName` is the selector text, `o => o.Customer.Email`):
```
Value 'o => o.Customer.Email' is null or empty.
```

## Complex Type Validation

SGuard doesn't validate an object's properties unless you ask it to:

- **Without a selector**, `ThrowIf.NullOrEmpty(user)` only checks that `user` is not `null`. A non-null `User` with an
  empty `Username`, an empty `Email` and no `Roles` passes.
- **With a selector that points at a complex type**, for example `ThrowIf.NullOrEmpty(account, a => a.Owner)`, the
  member counts as empty only if it is `null` or **all** of its readable properties are null or empty (recursively). A
  `User` with an empty `Email` but a non-empty `Username` is not empty.

So neither form tells you that *each* required property is filled in. Guard each property you require:

```csharp
public class User
{
    public string Username { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
}

public void ValidateUser(User user)
{
    ThrowIf.NullOrEmpty(user);
    ThrowIf.NullOrEmpty(user.Username);
    ThrowIf.NullOrEmpty(user.Email);
    ThrowIf.NullOrEmpty(user.Roles);
}

// Or through a parent object, where a null parent is also reported as empty:
public void ValidateAccount(Account account)
{
    ThrowIf.NullOrEmpty(account, a => a.Owner.Username);
    ThrowIf.NullOrEmpty(account, a => a.Owner.Email);
    ThrowIf.NullOrEmpty(account, a => a.Owner.Roles);
}
```

When a selector points at a complex type, its readable properties are inspected recursively, with these limits:

- Indexed properties (`this[...]`) are skipped.
- A type that is already being inspected higher up the same path (for example `Node.Next` of type `Node`) is only checked
  for null, so self-referencing types don't recurse forever. A non-null reference counts as non-empty.
- The same null-only check applies below 8 levels of nested complex types.

:::warning
Inspecting a complex type calls the getter of every readable public property on it, and on the types below it. Getters
with side effects run: an Entity Framework lazy-loading proxy loads each navigation property from the database, and a
computed property does its work. Point the selector at the scalar member you need (`a => a.Owner.Email`) rather than
at an entity or a type with expensive getters.
:::

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
2. **Guard each required property**: `NullOrEmpty` on an object doesn't check its members one by one
3. **Don't use `NullOrEmpty` where `0` or `false` is valid**: use `is null` / `HasValue` and comparison guards instead
4. **Use selectors for nested properties**: A null parent is reported as empty instead of throwing `NullReferenceException`
5. **Fail fast**: Place `NullOrEmpty` checks before other validations

## Performance Tips

- **Selectors are cached**: Repeated selector-based validations reuse the compiled delegate, unless the selector
  captures local variables
- **Direct checks are fastest**: When you don't need selectors, use direct checks
- See [Performance](../advanced/performance) for benchmarks

## Next Steps

- [Comparison Guards](./comparison-guards) - Range and comparison validation
- [Collection Validation](./collection-validation) - Any/All guards
- [Real-World Examples](./real-world-examples) - Complete validation scenarios
