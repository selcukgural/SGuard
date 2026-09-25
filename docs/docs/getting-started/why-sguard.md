---
sidebar_position: 3
---

# Why SGuard?

Discover what makes SGuard a powerful choice for guard clauses in .NET.

## Clear Diagnostics

Uses `CallerArgumentExpression` to produce precise, helpful error messages that point to the exact argument/expression that failed.

```csharp
ThrowIf.NullOrEmpty(user.Email);
// NullOrEmptyException: Value 'user.Email' is null or empty.
// ex.ParamName == "user.Email"
```

No more manual message crafting—SGuard does it for you. Built-in exceptions derive from `ArgumentException`, and checked values are left out of messages unless you set `SGuardOptions.IncludeValuesInExceptions = true`.

## Consistent Callback Model

A single `SGuardCallback(outcome)` works across both APIs:

- **`ThrowIf.*`** invokes with `Failure` when it's about to throw, `Success` when it passes.
- **`Is.*`** invokes with `Success` when the result is true, `Failure` when false (the method's result, not "validation passed").

Callback exceptions are safely swallowed, so your validation flow isn't disrupted.

```csharp
ThrowIf.LessThan(1, 2, SGuardCallbacks.OnFailure(() => 
    logger.LogWarning("Validation failed")));

bool ok = Is.Between(5, 1, 10, SGuardCallbacks.OnSuccess(() => 
    metrics.Increment("validation.success")));
```

## Rich Exception Surface

Throw built-in exceptions for common guards or supply your own:

- Pass a custom exception instance
- Use a generic `TException`
- Provide constructor arguments for detailed messages

```csharp
ThrowIf.LessThanOrEqual(quantity, 0, 
    new DomainValidationException("Quantity must be greater than zero."));
```

## Expressive, Dual API

Choose the style that fits your code:

- **`Is.*`** returns booleans for control-flow friendly checks
- **`ThrowIf.*`** fails fast with informative exceptions when rules are violated

Both share the same underlying logic and performance characteristics.

## Culture-Aware Comparisons

The string overloads of the `Is.*` comparisons and `ThrowIf.Between` accept `StringComparison` for correct cultural/ordinal semantics.

```csharp
bool ordinal = Is.LessThan("apple", "Banana", StringComparison.Ordinal);           // false ('a' > 'B' by code point)
bool culture = Is.LessThan("apple", "Banana", StringComparison.InvariantCulture);  // true

ThrowIf.Between("kiwi", "a", "m", StringComparison.OrdinalIgnoreCase); // throws: "kiwi" is inside the range
```

**Between checks are inclusive by design** for predictable validation.

## Performance and Ergonomics

- **Expression caching**: selector expressions are compiled once and cached by expression structure in a thread-safe cache (selectors that capture local variables are still compiled on every call)
- **No work when you don't need it**: plain comparisons and null/empty checks without a selector don't compile expressions
- **Custom exception types** created from a `TException` type or `constructorArgs` are instantiated through reflection only when the guard fails; pass an exception instance if you want to avoid that

See [Performance](../advanced/performance) for benchmarks and details.

## Modern .NET Support

Targets .NET 8, 9, and 10 with multi-targeting.

## Next Steps

- [Core Concepts](../core-concepts/guard-methods) - Dive deeper into guard methods
- [Callbacks](../core-concepts/callbacks) - Learn about the callback model
- [Performance](../advanced/performance) - See benchmarks and optimization tips
