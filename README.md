# SGuard

[![Tests and Update README](https://github.com/selcukgural/sguard/actions/workflows/coverage-readme.yml/badge.svg?branch=main)](https://github.com/selcukgural/sguard/actions/workflows/coverage-readme.yml)
[![NuGet](https://img.shields.io/nuget/v/SGuard.svg)](https://www.nuget.org/packages/SGuard)
[![NuGet Downloads](https://img.shields.io/nuget/dt/SGuard.svg)](https://www.nuget.org/packages/SGuard)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Matrix Chat](https://img.shields.io/badge/chat-on%20matrix-4fc08d)](https://matrix.to/#/#sguard:gitter.im)

## 💬 Join the Community Chat

Join our community chat to ask questions, share feedback, or get involved: [#sguard:gitter.im](https://matrix.to/#/#sguard:gitter.im)

SGuard is a lightweight, extensible guard clause library for .NET, providing expressive and robust validation for method arguments, object state, and business rules. It offers both boolean checks (`Is.*`) and exception-throwing guards (`ThrowIf.*`), with a unified callback model and rich exception diagnostics.

## 🆕 What’s New in 0.1.0

- Versioning reset: starting fresh at `0.1.0`. Older NuGet versions have been unlisted/removed.
  - **No functional breaking changes are expected for consumers adopting this version.**
- Targets: .NET 6, 7, 8, and 9.
- Packaging: README, LICENSE, and package icon included in the NuGet package.

## 🚀 Features

- **Boolean Guards (`Is.*`)**: Check conditions without throwing exceptions.
- **Throwing Guards (`ThrowIf.*`)**: Throw exceptions when conditions are met, with `CallerArgumentExpression`-powered messages.
- **Any & All Guards**: Predicate-based validation for collections.
- **Comprehensive Comparison Guards**: `Between`, `LessThan`, `LessThanOrEqual`, `GreaterThan`, `GreaterThanOrEqual` for generics and strings (with `StringComparison`).
- **Null/Empty Checks**: Deep and type-safe null/empty validation for primitives, collections, and complex types.
- **Custom Exception Support**: Overloads for custom exception types, with constructor argument support.
- **Callback Model**: Unified `SGuardCallback` and `GuardOutcome` for success/failure handling.
- **Expression Caching**: Efficient, thread-safe caching for compiled expressions.
- **Rich Exception Messages**: Informative diagnostics using `CallerArgumentExpression`.
- **Multi-targeting**: Supports .NET 6, 7, 8, and 9.

## 📦 Installation
`dotnet add package SGuard`

## 🤔 Why SGuard?

- Clear diagnostics
    - Uses CallerArgumentExpression to produce precise, helpful error messages that point to the exact argument/expression that failed.

- Consistent callback model
    - A single SGuardCallback(outcome) works across both APIs:
        - ThrowIf.* invokes with Failure when it’s about to throw, Success when it passes.
        - Is.* invokes with Success when the result is true, Failure when false.
    - Callback exceptions are safely swallowed, so your validation flow isn’t disrupted.

- Rich exception surface
    - Throw built-in exceptions for common guards, or supply your own:
        - Pass a custom exception instance, use a generic TException, or provide constructor arguments for detailed messages.

- Expressive, dual API
    - Choose the style that fits your code:
        - Is.* returns booleans for control-flow friendly checks.
        - ThrowIf.* fails fast with informative exceptions when rules are violated.

- Culture-aware comparisons and inclusive ranges
    - String overloads accept StringComparison for correct cultural/ordinal semantics.
    - Between checks are inclusive by design for predictable validation.

- Performance and ergonomics
    - Expression caching reduces overhead for repeated checks.
    - Minimal allocations and thread-safe evaluation where applicable.

- Modern .NET support
    - Targets .NET 6, 7, 8, and 9 with multi-targeting, ensuring broad compatibility.

## ⚡ Quick Start

SGuard helps you validate inputs and state with two complementary APIs:
- ThrowIf.*: fail fast by throwing informative exceptions.
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
}
```
### 2) Check conditions (boolean style)
```csharp
if (Is.Between(value, min, max)) { /* ... */ }
if (Is.LessThan(a, b)) { /* ... */ }
if (Is.Any(list, x => x > 0)) { /* ... */ }
if (!Is.Between(req.Age, 13, 130))
{
    throw new ArgumentOutOfRangeException(nameof(req.Age), "Age seems invalid.");
}

// Numeric comparisons 
bool inRange = Is.Between(value, min, max); 
bool isLess = Is.LessThan(a, b); 
bool isGreaterOrEqual = Is.GreaterThanOrEqual(a, b);
bool before = Is.LessThan("straße", "strasse", StringComparison.InvariantCulture); // culture-aware

// Collections 
bool anyPositive = Is.Any(numbers, n => n > 0); 
bool allNonNull = Is.All(items, it => it is not null);

// Strings (culture/ordinal aware)
bool lessOrdinal = Is.LessThan("apple", "banana", StringComparison.Ordinal);
bool lessIgnoreCase = Is.LessThan("Apple", "banana", StringComparison.OrdinalIgnoreCase)
```

### 3) Callbacks (side effects on success/failure)
```csharp
// ThrowIf: run side effects on the outcome
ThrowIf.LessThan(1, 2, SGuardCallbacks.OnFailure(() => logger.LogWarning("a < b failed")));
ThrowIf.LessThan(5, 2, SGuardCallbacks.OnSuccess(() => logger.LogInformation("a >= b OK")));

// Is: outcome maps to the boolean result (true=Success, false=Failure)
bool ok = Is.Between(5, 1, 10, SGuardCallbacks.OnSuccess(() => metrics.Increment("is.between.true")));
```

### 4) Custom exceptions

```csharp
ThrowIf.LessThanOrEqual(a, b, new MyCustomException("Invalid!"));
ThrowIf.Between<string, string, string, MyCustomException>(value, min, max, new MyCustomException("Out of range!"));

// Throw using your own exception type
ThrowIf.Any(items, i => i is null, new DomainValidationException("Collection contains null item(s)."));

// Another example with range validation
ThrowIf.LessThanOrEqual(quantity, 0, new DomainValidationException("Quantity must be greater than zero."));
```

### 5) String comparisons (culture/ordinal aware)
```csharp
// Ordinal comparisons
bool before = Is.LessThan("apple", "banana", StringComparison.Ordinal);

// Throw if the ordering violates your rule
ThrowIf.GreaterThan("zebra", "apple", StringComparison.Ordinal); // throws (zebra > apple)
```

### 6) Notes

- Between is inclusive (min and max are allowed).
- ThrowIf invokes callbacks with Failure when it’s about to throw, Success when it passes.
- Is.* invokes callbacks with Success when the result is true, Failure when false.
- Callback exceptions are swallowed (they won’t break your validation flow).

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
    - Return a boolean and never throw for the check itself.
    - Outcome = Success when the result is true, Outcome = Failure when the result is false.

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

**Note:** The callback is invoked regardless of the outcome of the guard.
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

ThrowIf.LessThan(1, 2, outcome => observed = outcome); // throws -> observed remains null (callback still runs with Failure before exception propagation)
```




### More Examples

#### Throwing Guards

```csharp
ThrowIf.NullOrEmpty(str);
ThrowIf.NullOrEmpty(obj, x => x.Property);
ThrowIf.Between(value, min, max); // Throws if value is between min and max
ThrowIf.LessThan(a, b, () => Console.WriteLine("Failed!"));
ThrowIf.Any(list, x => x == null);

// Optionally run a callback on failure (e.g., logging/metrics/cleanup)
ThrowIf.GreaterThan(total, limit, () => logger.LogWarning("Limit exceeded"));

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

        // Totals
        ThrowIf.LessThanOrEqual(cart.TotalAmount, 0m, new ArgumentOutOfRangeException(nameof(cart.TotalAmount), "Total must be greater than zero."));
    }
}

public void SaveUser(string username)
{
    var callback = SGuardCallbacks.OnFailure(() =>
        logger.LogWarning("Validation failed: username is required"));

    // When username is null or empty, throw an exception with a custom message and invoke the callback.
    ThrowIf.NullOrEmpty(username, callback);
    
    // Proceed with saving the user...
}

public void UpdateEmail(string email)
{
    var onSuccess = SGuardCallbacks.OnSuccess(() =>
        audit.Record("Email validation succeeded"));

    // If valid, onSuccess is called; if not, an exception is thrown
    ThrowIf.NullOrEmpty(email, onSuccess);

    // Proceed with updating the email...
}


```

## ✅ Test and Coverage Status

<!-- TEST-RESULTS:START -->
## Test and Coverage Status

### Test Results

| Total | Passed | Failed | Skipped |
|------:|-------:|-------:|--------:|
| 367 | 367 | 0 | 0 |

### Code Coverage
# Summary
|||
|:---|:---|
| Generated on: | 09/05/2025 - 10:39:42 |
| Coverage date: | 09/03/2025 - 19:56:53 - 09/05/2025 - 10:39:39 |
| Parser: | MultiReport (12x Cobertura) |
| Assemblies: | 1 |
| Classes: | 15 |
| Files: | 50 |
| **Line coverage:** | 86.9% (815 of 937) |
| Covered lines: | 815 |
| Uncovered lines: | 122 |
| Coverable lines: | 937 |
| Total lines: | 4360 |
| **Branch coverage:** | 83% (186 of 224) |
| Covered branches: | 186 |
| Total branches: | 224 |
| **Method coverage:** | [Feature is only available for sponsors](https://reportgenerator.io/pro) |

|**Name**|**Covered**|**Uncovered**|**Coverable**|**Total**|**Line coverage**|**Covered**|**Total**|**Branch coverage**|
|:---|---:|---:|---:|---:|---:|---:|---:|---:|
|**SGuard**|**815**|**122**|**937**|**4360**|**86.9%**|**186**|**224**|**83%**|
|SGuard.ExceptionActivator|15|0|15|56|100%|6|8|75%|
|SGuard.Exceptions.AllException|0|4|4|44|0%|0|0||
|SGuard.Exceptions.AnyException|2|2|4|36|50%|0|0||
|SGuard.Exceptions.BetweenException|23|6|29|135|79.3%|0|0||
|SGuard.Exceptions.GreaterThanException|17|6|23|108|73.9%|0|0||
|SGuard.Exceptions.GreaterThanOrEqualException|17|6|23|110|73.9%|0|0||
|SGuard.Exceptions.LessThanException|15|6|21|111|71.4%|0|0||
|SGuard.Exceptions.LessThanOrEqualException|15|6|21|111|71.4%|0|0||
|SGuard.Exceptions.NullOrEmptyException|15|4|19|98|78.9%|0|0||
|SGuard.Is|181|0|181|1100|100%|22|24|91.6%|
|SGuard.SGuard|31|1|32|124|96.8%|12|12|100%|
|SGuard.SGuardCallbacks|2|2|4|68|50%|0|0||
|SGuard.Throw|21|0|21|243|100%|0|0||
|SGuard.ThrowIf|311|19|330|1558|94.2%|52|56|92.8%|
|SGuard.Visitor.NullOrEmptyVisitor|150|60|210|458|71.4%|94|124|75.8%|
<!-- TEST-RESULTS:END -->

## 🔢 Versioning

This project follows Semantic Versioning. As of this release, versioning restarts at `0.1.0`. If you previously consumed older versions, please upgrade to the latest package.

## 🤝 Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## 🌐 Code of Conduct

This project adheres to the [.NET Foundation Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code.

## 📜 License

This project is licensed under the MIT License, a permissive open source license. See the [LICENSE](LICENSE) file for details.

## 🔗 Links

- [NuGet Package](https://www.nuget.org/packages/SGuard)
- [Releases](https://github.com/selcukgural/sguard/releases)
