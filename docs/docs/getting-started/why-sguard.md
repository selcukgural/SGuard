---
sidebar_position: 3
---

# Why SGuard?

Discover what makes SGuard a powerful choice for guard clauses in .NET.

## Clear Diagnostics

Uses `CallerArgumentExpression` to produce precise, helpful error messages that point to the exact argument/expression that failed.

```csharp
ThrowIf.NullOrEmpty(user.Email);
// Exception message: "Value cannot be null or empty. (Parameter 'user.Email')"
```

No more manual message crafting—SGuard does it for you.

## Consistent Callback Model

A single `SGuardCallback(outcome)` works across both APIs:

- **`ThrowIf.*`** invokes with `Failure` when it's about to throw, `Success` when it passes.
- **`Is.*`** invokes with `Success` when the result is true, `Failure` when false.

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

String overloads accept `StringComparison` for correct cultural/ordinal semantics.

```csharp
bool less = Is.LessThan("straße", "strasse", StringComparison.InvariantCulture);

ThrowIf.GreaterThan("zebra", "apple", StringComparison.Ordinal);
```

**Between checks are inclusive by design** for predictable validation.

## Performance and Ergonomics

- **Expression caching** reduces overhead for repeated checks
- **Minimal allocations** and thread-safe evaluation where applicable
- **Zero reflection overhead** in hot paths

See [Performance](../advanced/performance) for benchmarks and details.

## Modern .NET Support

Targets .NET 6, 7, 8, and 9 with multi-targeting, ensuring broad compatibility across modern .NET versions.

## Next Steps

- [Core Concepts](../core-concepts/guard-methods) - Dive deeper into guard methods
- [Callbacks](../core-concepts/callbacks) - Learn about the callback model
- [Performance](../advanced/performance) - See benchmarks and optimization tips
