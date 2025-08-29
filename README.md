# SGuard

SGuard is a lightweight, extensible guard clause library for .NET, providing expressive and robust validation for method arguments, object state, and business rules. It offers both boolean checks (`Is.*`) and exception-throwing guards (`ThrowIf.*`), with a unified callback model and rich exception diagnostics.

## 🚀 Features

- **Boolean Guards (`Is.*`)**: Check conditions without throwing exceptions.
- **Throwing Guards (`ThrowIf.*`)**: Throw exceptions when conditions are met.
- **Any & All Guards**: Predicate-based validation for collections.
- **Comprehensive Comparison Guards**: `Between`, `LessThan`, `LessThanOrEqual`, `GreaterThan`, `GreaterThanOrEqual` for generics and strings (with `StringComparison`).
- **Null/Empty Checks**: Deep and type-safe null/empty validation for primitives, collections, and complex types.
- **Custom Exception Support**: Overloads for custom exception types, with constructor argument support.
- **Callback Model**: Unified `SGuardCallback` and `GuardOutcome` for success/failure handling.
- **Expression Caching**: Efficient, thread-safe caching for compiled expressions.
- **Rich Exception Messages**: Informative diagnostics using `CallerArgumentExpression`.
- **Multi-targeting**: Supports .NET 6, 7, 8, and 9.

## 🆕 What's New in 2.0.4

- Added `LessThan` and `LessThanOrEqual` guards for generics and strings.
- Added `Any<T>` and `All<T>` guards with predicate support.
- String guards now support `StringComparison` for culture-aware checks.
- Unified callback support across all guards.
- Custom exception overloads and constructor argument support.
- Generic exception creation with `ExceptionActivator.Create<T>`, supporting both parameterless and parameterized constructors.
- Expression caching for improved performance in null/empty checks.
- Improved documentation and XML comments.

## 📦 Installation

```
dotnet add package SGuard --version 2.0.4
```

## 📝 Usage Examples

### Boolean Guards

```csharp
if (Is.Between(value, min, max)) { /* ... */ }
if (Is.LessThan(a, b)) { /* ... */ }
if (Is.Any(list, x => x > 0)) { /* ... */ }
```

### Throwing Guards

```csharp
ThrowIf.Between(value, min, max); // Throws if value is between min and max
ThrowIf.LessThan(a, b, () => Console.WriteLine("Failed!"));
ThrowIf.Any(list, x => x == null);
```

### Custom Exceptions

```csharp
ThrowIf.LessThanOrEqual(a, b, new MyCustomException("Invalid!"));
ThrowIf.Between<string, string, string, MyCustomException>(value, min, max, new MyCustomException("Out of range!"));
```

### Null or Empty Checks

```csharp
ThrowIf.NullOrEmpty(str);
ThrowIf.NullOrEmpty(obj, x => x.Property);
```

## Build & Test Status

![Build & Test](https://github.com/selcukgural/SGuard.Tests/blob/master/.github/workflows/blank.yml/badge.svg)


## 📄 License

This project is licensed under the GPL-3.0-or-later license. See the [LICENSE](./LICENSE) file for details.

## 🔗 Links

- [NuGet Package](https://www.nuget.org/packages/SGuard)
- [GitHub Repository](https://github.com/selcukgural/sguard)

