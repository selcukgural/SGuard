# SGuard

[![CI](https://github.com/selcukgural/SGuard/actions/workflows/ci.yml/badge.svg)](https://github.com/selcukgural/SGuard/actions)
[![NuGet](https://img.shields.io/nuget/v/SGuard.svg)](https://www.nuget.org/packages/SGuard)
[![NuGet Downloads](https://img.shields.io/nuget/dt/SGuard.svg)](https://www.nuget.org/packages/SGuard)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Matrix Chat](https://img.shields.io/badge/chat-on%20matrix-4fc08d)](https://matrix.to/#/#sguard:gitter.im)

SGuard is a lightweight, extensible guard clause library for .NET, providing expressive and robust validation for method arguments, object state, and business rules. It offers both boolean checks (`Is.*`) and exception-throwing guards (`ThrowIf.*`), with a unified callback model and rich exception diagnostics.

## 🚀 Features

- **Boolean Guards (`Is.*`)**: Check conditions and get a `bool` back instead of an exception.
- **Throwing Guards (`ThrowIf.*`)**: Throw when a condition is true, with `CallerArgumentExpression`-powered messages.
- **Any & All Guards**: Predicate-based validation for collections (`IEnumerable<T>` and `ReadOnlySpan<T>`).
- **Comparison Guards**: `Between` (inclusive), `LessThan`, `LessThanOrEqual`, `GreaterThan`, `GreaterThanOrEqual` for any `IComparable<T>` type. The `Is.*` comparisons and `ThrowIf.Between` also have string overloads that take a `StringComparison`. With a floating-point `NaN` operand, `Is.*` comparisons return `false` and `ThrowIf.*` comparisons throw.
- **Null/Empty Checks**: Null, default values (`0`, `Guid.Empty`, ...), empty strings (whitespace is not empty), collections and spans. With a selector (`o => o.Customer.Email`), SGuard follows the member path; a complex-type member counts as empty only when all of its readable properties are null or empty.
- **Email Validation**: `Is.Email` with a built-in pattern or your own regex (with a match timeout).
- **Custom Exception Support**: Overloads for custom exception instances and types, with constructor argument support.
- **Callback Model**: Unified `SGuardCallback` and `GuardOutcome` for success/failure handling.
- **Expression Caching**: Selectors are compiled once and cached by expression structure (thread-safe).
- **Clear Exception Messages**: Built-in exceptions derive from `ArgumentException` and name the failing argument expression; checked values are left out of messages by default.
- **Multi-targeting**: Supports .NET 8, 9, and 10.

## 📊 Benchmarks

Performance benchmarks for all guard methods are available in the [SGuard.Benchmark/benchmarks/](SGuard.Benchmark/benchmarks/) folder. Explore these to see real-world performance comparisons for `Is.*` and `ThrowIf.*` methods.

## 📦 Installation
`dotnet add package SGuard`

## 🤔 Why SGuard?

- Clear diagnostics
    - Uses CallerArgumentExpression to produce precise, helpful error messages that point to the exact argument/expression that failed.

- Consistent callback model
    - A single SGuardCallback(outcome) works across both APIs:
        - ThrowIf.* invokes with Failure when it’s about to throw, Success when it passes.
        - Is.* invokes with Success when the result is true, Failure when false (so `Is.NullOrEmpty((string?)null)` reports Success).
    - Callback exceptions are safely swallowed, so your validation flow isn’t disrupted.

- Rich exception surface
    - Throw built-in exceptions for common guards or supply your own:
        - Pass a custom exception instance, use a generic TException, or provide constructor arguments for detailed messages.

- Expressive, dual API
    - Choose the style that fits your code:
        - Is.* returns booleans for control-flow friendly checks.
        - ThrowIf.* fails fast with informative exceptions when rules are violated.

- Culture-aware comparisons and inclusive ranges
    - String overloads of the `Is.*` comparisons and `ThrowIf.Between` accept StringComparison for correct cultural/ordinal semantics.
    - Between checks are inclusive by design for predictable validation.

- Performance and ergonomics
    - Selector expressions are compiled once and cached by expression structure, so repeated checks don't pay the compilation cost again (selectors that capture local variables are still compiled on every call).
    - The selector cache is thread-safe.

- Modern .NET support
    - Targets .NET 8, 9, and 10 with multi-targeting.

## ⚡ Quick Start

SGuard helps you validate inputs and state with two complementary APIs:
- ThrowIf.*: fail fast by throwing informative exceptions when a condition is true.
- Is.*: return booleans for control-flow-friendly checks.

### 1) Validate inputs (fail fast)
```csharp
public record CreateUserRequest(string Username, int Age, string Email);

public User CreateUser(CreateUserRequest req)
{
    ThrowIf.NullOrEmpty(req);
    ThrowIf.NullOrEmpty(req.Email);
    ThrowIf.NullOrEmpty(req.Username);
    ThrowIf.LessThan(req.Age, 13, new ArgumentException("User must be 13+.", nameof(req.Age)));

    // Optionally check formats or ranges
    if (!Is.Email(req.Email))
        throw new ArgumentException("Email is not valid.", nameof(req.Email));

    if (!Is.Between(req.Age, 13, 130))
        throw new ArgumentOutOfRangeException(nameof(req.Age), "Age seems invalid.");

    return new User(req.Username, req.Age, req.Email);
}

public sealed class User
{
    public User(string username, int age, string email)
    {
        ThrowIf.LessThan(age, 0);
        ThrowIf.NullOrEmpty(email);
        ThrowIf.NullOrEmpty(username);

        Age = age;
        Email = email;
        Username = username;
    }

    public int Age { get; }
    public string Email { get; }
    public string Username { get; }
}
```
### 2) Check conditions (boolean style)
```csharp
if (Is.Between(value, min, max)) { /* ... */ }
if (Is.LessThan(a, b)) { /* ... */ }
if (Is.Any(list, x => x > 0)) { /* ... */ }

// Numeric comparisons
bool inRange = Is.Between(value, min, max);
bool isLess = Is.LessThan(a, b);
bool isGreaterOrEqual = Is.GreaterThanOrEqual(a, b);

// Collections (LINQ semantics: Is.All(empty) is true, Is.Any(empty) is false)
bool anyPositive = Is.Any(numbers, n => n > 0);
bool allNonNull = Is.All(items, it => it is not null);

// Strings (culture/ordinal aware)
bool lessOrdinal = Is.LessThan("apple", "banana", StringComparison.Ordinal);            // true
bool lessIgnoreCase = Is.LessThan("Apple", "banana", StringComparison.OrdinalIgnoreCase); // true

// Email (built-in ASCII pattern, at most 254 characters; not a full RFC 5322 parser)
bool validEmail = Is.Email("jane.doe@example.com"); // true
```

`Is.*` methods don't throw for the check itself, but they do throw for invalid arguments: `ArgumentNullException` for a null operand, predicate, source or `Is.Email(null)`; `ArgumentException` for `Is.Email("")` and for `Between` bounds where `min > max`; and `RegexMatchTimeoutException` when a custom `Is.Email` pattern times out.

### 3) Callbacks (side effects on success/failure)
```csharp
// ThrowIf: run side effects on the outcome
ThrowIf.LessThan(1, 2, SGuardCallbacks.OnFailure(() => logger.LogWarning("a < b failed")));   // logs, then throws
ThrowIf.LessThan(5, 2, SGuardCallbacks.OnSuccess(() => logger.LogInformation("a >= b OK"))); // logs, no throw

// Is: outcome maps to the boolean result (true=Success, false=Failure)
bool ok = Is.Between(5, 1, 10, SGuardCallbacks.OnSuccess(() => metrics.Increment("is.between.true")));
```

### 4) Custom exceptions

```csharp
ThrowIf.LessThanOrEqual(a, b, new MyCustomException("Invalid!"));

// ThrowIf.Between throws when the value is INSIDE the range (inclusive)
ThrowIf.Between(port, 0, 1023, new MyCustomException("Well-known ports are reserved."));

// Throw using your own exception type
ThrowIf.Any(items, i => i is null, new DomainValidationException("Collection contains null item(s)."));

// Another example with range validation
ThrowIf.LessThanOrEqual(quantity, 0, new DomainValidationException("Quantity must be greater than zero."));
```

### 5) String comparisons (culture/ordinal aware)
```csharp
// Ordinal comparisons
bool before = Is.LessThan("apple", "banana", StringComparison.Ordinal); // true

// ThrowIf.Between has a StringComparison overload (throws when the value is inside the range)
ThrowIf.Between("kiwi", "a", "m", StringComparison.OrdinalIgnoreCase); // throws BetweenException
```

`ThrowIf.LessThan`/`GreaterThan` and their `OrEqual` variants have no `StringComparison` overload. Don't use lexicographic string comparison for access-control, path-prefix or version checks: use `Path.GetFullPath` with an ordinal `StartsWith` on a root that ends with a separator for paths, and `System.Version` for versions.

### 6) Notes

- Between is inclusive (min and max are allowed), and throws `ArgumentException` when `min > max`.
- ThrowIf invokes callbacks with Failure when it’s about to throw, Success when it passes.
- Is.* invokes callbacks with Success when the result is true, Failure when false.
- Callback exceptions are swallowed (they won’t break your validation flow).
- A floating-point `NaN` operand (`double`, `float`, `Half`) makes `Is.*` comparisons return `false` and `ThrowIf.*` comparisons throw. A check such as `if (Is.GreaterThan(x, max)) reject();` therefore lets `NaN` through; prefer `ThrowIf.*` or `!Is.Between(...)`.

### Exception messages and options

Built-in exceptions (`NullOrEmptyException`, `BetweenException`, `GreaterThanException`, `LessThanException`, ... in `SGuard.Exceptions`) derive from `ArgumentException`, so existing `catch (ArgumentException)` blocks handle them. `ParamName` holds the caller's argument expression, and the message names the expressions but leaves the checked values out:

```csharp
ThrowIf.NullOrEmpty(request.Name);
// NullOrEmptyException: Value 'request.Name' is null or empty.

ThrowIf.GreaterThan(request.Age, limit);
// GreaterThanException: Left value is greater than right value. Actual: left=request.Age, right=limit.
```

To include the values (converted with `ToString()` and truncated to 64 characters) in `Message` and `Exception.Data`, opt in once at startup. Only do this when the checked values can't be secrets or personal data:

```csharp
SGuardOptions.IncludeValuesInExceptions = true;
// GreaterThanException: '4217' is greater than '1000'. Actual: left=request.Age, right=limit.
```

### Callbacks – When do they run?

- **ThrowIf methods:**
    - Outcome = Failure → the guard is about to throw (callback runs just before the exception propagates).
    - Outcome = Success → the guard passes (no exception is thrown).
    - If the API fails due to invalid arguments (e.g., null selector or null exception instance), the callback is NOT invoked.


#### Examples:
```csharp
// Failure → throws → OnFailure runs
ThrowIf.LessThan(1, 2, SGuardCallbacks.OnFailure(() => logger.LogWarning("a < b failed")));

// Success → no throw → OnSuccess runs
ThrowIf.LessThan(5, 2, SGuardCallbacks.OnSuccess(() => logger.LogInformation("a >= b OK")));
```


- **Is methods**:
    - Return a boolean; they throw only for invalid arguments (see above), not for the check itself.
    - Outcome = Success when the result is true, Outcome = Failure when the result is false. Success means "the method returned true", not "validation passed": `Is.NullOrEmpty((string?)null)` reports Success.

#### Examples
```csharp
// True → OnSuccess runs
bool inRange = Is.Between(5, 1, 10, SGuardCallbacks.OnSuccess(() => metrics.Increment("is.between.true")));

// False → OnFailure runs
bool isLess = Is.LessThan(5, 2, SGuardCallbacks.OnFailure(() => metrics.Increment("is.lt.false")));
```

#### Combine callbacks (Success + Failure)
```csharp
var onFailure = SGuardCallbacks.OnFailure(() => notifier.Notify("Validation failed"));
var onSuccess = SGuardCallbacks.OnSuccess(() => notifier.Notify("Validation passed"));
SGuardCallback combined = onFailure + onSuccess;

// If inside range -> throws -> Failure -> only onFailure runs
// If outside range -> no throw -> Success -> only onSuccess runs
ThrowIf.Between(value, min, max, combined);
```

**Note:** The callback runs for both outcomes, but not when the call itself is invalid:
```csharp
// Passing a null exception instance causes an immediate ArgumentNullException.
// The callback is NOT invoked in this case (no Success/Failure outcome is produced).
try
{
    ThrowIf.Between<int, int, int, InvalidOperationException>(
        5, 1, 10,
        (InvalidOperationException)null!, // invalid argument
        SGuardCallbacks.OnFailure(() => logger.LogError("won't run")));
}
catch (ArgumentNullException)
{
    // expected, and callback not called
}
```

Inline callback when you need the **outcome** value directly

```csharp
GuardOutcome? observed = null;

try
{
    ThrowIf.LessThan(1, 2, outcome => observed = outcome); // throws LessThanException
}
catch (LessThanException)
{
    // observed == GuardOutcome.Failure: the callback ran before the exception propagated
}
```




### More Examples

#### Throwing Guards

```csharp
ThrowIf.NullOrEmpty(str);
ThrowIf.NullOrEmpty(obj, x => x.Property);
ThrowIf.Between(value, min, max); // Throws if value is between min and max (inclusive)
ThrowIf.LessThan(a, b, SGuardCallbacks.OnFailure(() => Console.WriteLine("Failed!")));
ThrowIf.Any(list, x => x == null);

// Optionally run a callback on failure (e.g., logging/metrics/cleanup)
ThrowIf.GreaterThan(total, limit, SGuardCallbacks.OnFailure(() => logger.LogWarning("Limit exceeded")));

// With selector for nested properties (CallerArgumentExpression helps messages)
ThrowIf.NullOrEmpty(order, o => o.Customer.Name);
```


## 📝 Usage Examples (Real-life Scenarios)

```csharp
public static class CheckoutService
{
    public static void ValidateCart(Cart cart, IReadOnlyDictionary<string, int> stockBySku)
    {
        ThrowIf.NullOrEmpty(cart);
        ThrowIf.NullOrEmpty(cart.Items);

        // Every item must have positive quantity
        if (!Is.All(cart.Items, i => i.Quantity > 0))
            throw new ArgumentException("All items must have a positive quantity.", nameof(cart.Items));

        // Check stock levels
        foreach (var item in cart.Items)
        {
            var stock = stockBySku.TryGetValue(item.Sku, out var s) ? s : 0;
            ThrowIf.GreaterThan(item.Quantity, stock, new InvalidOperationException($"Insufficient stock for SKU '{item.Sku}'."));
        }

        // Totals (decimal needs a decimal literal: 0m)
        ThrowIf.LessThanOrEqual(cart.TotalAmount, 0m, new ArgumentOutOfRangeException(nameof(cart.TotalAmount), "Total must be greater than zero."));
    }
}

public void SaveUser(string username)
{
    var callback = SGuardCallbacks.OnFailure(() =>
        logger.LogWarning("Validation failed: username is required"));

    // When username is null or empty, the callback runs and a NullOrEmptyException is thrown.
    ThrowIf.NullOrEmpty(username, callback);

    // Proceed with saving the user...
}

public void UpdateEmail(string email)
{
    var onSuccess = SGuardCallbacks.OnSuccess(() =>
        audit.Record("Email validation succeeded"));

    // If email is not null or empty, onSuccess is called; otherwise an exception is thrown
    ThrowIf.NullOrEmpty(email, onSuccess);

    // Proceed with updating the email...
}

```
## 💬 Join the Community Chat

Join our community chat to ask questions, share feedback, or get involved: [#sguard:gitter.im](https://matrix.to/#/#sguard:gitter.im)


## 🤝 Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## 🌐 Code of Conduct

This project adheres to the [.NET Foundation Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code.

## 📜 License

This project is licensed under the MIT License, a permissive open source license. See the [LICENSE](LICENSE) file for details.

## 🔗 Links

- [NuGet Package](https://www.nuget.org/packages/SGuard)
- [Releases](https://github.com/selcukgural/sguard/releases)
