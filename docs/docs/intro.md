---
sidebar_position: 1
slug: /
---

# Welcome to SGuard

SGuard is a lightweight, extensible guard clause library for .NET, providing expressive and robust validation for method arguments, object state, and business rules.

[![CI](https://github.com/selcukgural/SGuard/actions/workflows/ci.yml/badge.svg)](https://github.com/selcukgural/SGuard/actions)
[![NuGet](https://img.shields.io/nuget/v/SGuard.svg)](https://www.nuget.org/packages/SGuard)
[![NuGet Downloads](https://img.shields.io/nuget/dt/SGuard.svg)](https://www.nuget.org/packages/SGuard)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/selcukgural/SGuard/blob/main/LICENSE)
[![Matrix Chat](https://img.shields.io/badge/chat-on%20matrix-4fc08d)](https://matrix.to/#/#sguard:gitter.im)

## 🚀 Features

- **Boolean Guards (`Is.*`)**: Check conditions and get a `bool` back instead of an exception.
- **Throwing Guards (`ThrowIf.*`)**: Throw when a condition is true, with `CallerArgumentExpression`-powered messages.
- **Any & All Guards**: Predicate-based validation for collections (`IEnumerable<T>` and `ReadOnlySpan<T>`).
- **Comparison Guards**: `OutOfRange` and `Between` (inclusive), `LessThan`, `LessThanOrEqual`, `GreaterThan`, `GreaterThanOrEqual` for any `IComparable<T>` type, `Negative`, `NegativeOrZero` and `Zero` for any number type, and `Equal`/`NotEqual`. Strings can be compared with a `StringComparison`. With a floating-point `NaN` operand, `Is.*` comparisons return `false` and `ThrowIf.*` comparisons throw.
- **Null/Empty Checks**: `Null`, `NullOrWhiteSpace`, and `NullOrEmpty` for null, default values (`0`, `Guid.Empty`, ...), empty strings (whitespace is not empty), collections and spans. With a selector (`o => o.Customer.Email`, `o => o.Items[0].Sku`), SGuard follows the path through members, indexers and instance method calls, and a `null` on it counts as empty; a complex-type member counts as empty only when all of its readable properties are null or empty.
- **String and Enum Guards**: `LongerThan`, `ShorterThan` and `NotDefined`.
- **Input Validators**: `Is.Email`, `Is.Url` (`http`/`https` only), `Is.CreditCard` (Luhn) and `Is.Match` (a regex with a match timeout).
- **Return Values**: A passing `ThrowIf.*` guard returns the checked value: `Sku = ThrowIf.NullOrEmpty(sku);`.
- **Custom Exception Support**: Pass a factory (`() => new MyException(...)`) that runs only when the guard fails, an exception instance, or an exception type.
- **Native AOT**: Every guard except `NullOrEmpty` selectors works in trimmed and Native AOT apps, checked in CI.
- **Callback Model**: Unified `SGuardCallback` and `GuardOutcome` for success/failure handling.
- **Expression Caching**: Selectors are compiled once and cached by expression structure (thread-safe), including selectors that capture local variables or use operators such as `+` and `??`.
- **Allocation-free Guards**: A passing guard costs about as much as a hand-written `if` (around a nanosecond) and allocates nothing.
- **Clear Exception Messages**: Built-in exceptions derive from `ArgumentException` (range checks from `ArgumentOutOfRangeException`) and name the failing argument expression; checked values are left out of messages by default.
- **Multi-targeting**: Supports .NET 8, 9, and 10.

## 📦 Quick Install

```bash
dotnet add package SGuard
```

## 🎯 Quick Example

```csharp
public User CreateUser(CreateUserRequest req)
{
    ThrowIf.Null(req);

    return new User(
        ThrowIf.NullOrWhiteSpace(req.Username),
        ThrowIf.LessThan(req.Age, 13, () => new ArgumentException("User must be 13+.", nameof(req.Age))),
        ThrowIf.NullOrEmpty(req.Email));
}
```

Upgrading from 0.x? See [Upgrading to 3.0](./migration/upgrading-to-3.0).

## 🗺️ Documentation Structure

- **[Getting Started](./getting-started/installation)**: Installation, quick start, and why choose SGuard
- **[Core Concepts](./core-concepts/guard-methods)**: Understanding guard methods, callbacks, custom exceptions
- **[Guides](./guides/null-empty-checks)**: Practical guides for common scenarios
- **[Advanced](./advanced/performance)**: Performance tuning and best practices
- **[Migration](./migration/upgrading-to-3.0)**: Upgrading from 0.x to 3.0
- **[API Reference](./api/throwif)**: Complete API documentation
- **[Community](./community/contributing)**: Contributing, code of conduct, changelog

## 💬 Get Help

- **Matrix Chat**: [#sguard:gitter.im](https://matrix.to/#/#sguard:gitter.im)
- **GitHub Issues**: [Report bugs or request features](https://github.com/selcukgural/SGuard/issues)
- **GitHub Discussions**: [Ask questions and share ideas](https://github.com/selcukgural/SGuard/discussions)

## 🤝 Contributing

We welcome contributions! See our [Contributing Guide](./community/contributing) to get started.

## 📜 License

SGuard is licensed under the [MIT License](https://github.com/selcukgural/SGuard/blob/main/LICENSE).
