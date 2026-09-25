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
- **Comparison Guards**: `Between` (inclusive), `LessThan`, `LessThanOrEqual`, `GreaterThan`, `GreaterThanOrEqual` for any `IComparable<T>` type. The `Is.*` comparisons and `ThrowIf.Between` also have string overloads that take a `StringComparison`. With a floating-point `NaN` operand, `Is.*` comparisons return `false` and `ThrowIf.*` comparisons throw.
- **Null/Empty Checks**: Null, default values (`0`, `Guid.Empty`, ...), empty strings (whitespace is not empty), collections and spans. With a selector (`o => o.Customer.Email`), SGuard follows the member path; a complex-type member counts as empty only when all of its readable properties are null or empty.
- **Email Validation**: `Is.Email` with a built-in pattern or your own regex (with a match timeout).
- **Custom Exception Support**: Overloads for custom exception instances and types, with constructor argument support.
- **Callback Model**: Unified `SGuardCallback` and `GuardOutcome` for success/failure handling.
- **Expression Caching**: Selectors are compiled once and cached by expression structure (thread-safe).
- **Clear Exception Messages**: Built-in exceptions derive from `ArgumentException` and name the failing argument expression; checked values are left out of messages by default.
- **Multi-targeting**: Supports .NET 8, 9, and 10.

## 📦 Quick Install

```bash
dotnet add package SGuard
```

## 🎯 Quick Example

```csharp
public User CreateUser(CreateUserRequest req) 
{ 
    ThrowIf.NullOrEmpty(req);
    ThrowIf.NullOrEmpty(req.Email);
    ThrowIf.NullOrEmpty(req.Username);
    ThrowIf.LessThan(req.Age, 13, new ArgumentException("User must be 13+.", nameof(req.Age)));
    
    return new User(req.Username, req.Age, req.Email);
}
```

## 🗺️ Documentation Structure

- **[Getting Started](./getting-started/installation)**: Installation, quick start, and why choose SGuard
- **[Core Concepts](./core-concepts/guard-methods)**: Understanding guard methods, callbacks, custom exceptions
- **[Guides](./guides/null-empty-checks)**: Practical guides for common scenarios
- **[Advanced](./advanced/performance)**: Performance tuning and best practices
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
