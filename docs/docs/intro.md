---
sidebar_position: 1
slug: /
---

# Welcome to SGuard

SGuard is a lightweight, extensible guard clause library for .NET, providing expressive and robust validation for method arguments, object state, and business rules.

[![CI](https://github.com/selcukgural/SGuard/actions/workflows/ci.yml/badge.svg)](https://github.com/selcukgural/SGuard/actions)
![Coverage](https://raw.githubusercontent.com/selcukgural/SGuard/gh-pages/badges/badge_linecoverage.svg)
[![NuGet](https://img.shields.io/nuget/v/SGuard.svg)](https://www.nuget.org/packages/SGuard)
[![NuGet Downloads](https://img.shields.io/nuget/dt/SGuard.svg)](https://www.nuget.org/packages/SGuard)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/selcukgural/SGuard/blob/main/LICENSE)
[![Matrix Chat](https://img.shields.io/badge/chat-on%20matrix-4fc08d)](https://matrix.to/#/#sguard:gitter.im)

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
