---
sidebar_position: 3
---

# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.2.0] - 2026-09-26

This release contains breaking changes; see **Changed** and **Removed**.
Assemblies compiled against 0.1.2 must be rebuilt.

### Added

- `Is.Email` validates email addresses with a built-in ASCII pattern (at most
  254 characters, no trailing line break), or with a custom pattern that
  stops after `Is.DefaultEmailRegexTimeout` (1 second) or an explicit
  `matchTimeout` and then throws `RegexMatchTimeoutException`.
- `ReadOnlySpan<T>` overloads for `Is.NullOrEmpty`, `ThrowIf.NullOrEmpty`,
  `Is.All`, `Is.Any`, `ThrowIf.All` and `ThrowIf.Any`.
- `SGuardOptions.IncludeValuesInExceptions` to include checked values in
  built-in exception messages and `Exception.Data`. Values are written with
  `ToString()` and truncated to 64 characters.
- The NuGet package now contains XML documentation (IntelliSense) and a
  symbol package (`.snupkg`) for Source Link.

### Changed

- Built-in exceptions (`NullOrEmptyException`, `BetweenException`,
  `GreaterThanException`, `GreaterThanOrEqualException`, `LessThanException`,
  `LessThanOrEqualException`, `AllException`, `AnyException`) now derive from
  `ArgumentException` instead of `Exception`. `ParamName` holds the caller's
  argument expression (e.g. `request.Age`); the message is unchanged.
- Built-in exceptions no longer include the checked values in `Message` or
  `Exception.Data` unless `SGuardOptions.IncludeValuesInExceptions` is
  enabled. When enabled, `Exception.Data` holds the formatted strings instead
  of the values.
- `ThrowIf.Between`, `GreaterThan`, `GreaterThanOrEqual`, `LessThan`,
  `LessThanOrEqual`, `NullOrEmpty` and the matching `Throw.*Exception` helpers
  take optional `[CallerArgumentExpression]` parameters.
- Comparison guards treat a floating-point NaN operand (`double`, `float`,
  `Half`) as failing: `Is.*` returns `false` and `ThrowIf.*` throws. NaN
  previously passed `ThrowIf.GreaterThan`/`GreaterThanOrEqual` and made
  `Is.LessThan` return `true`.
- `Is.Between` and `ThrowIf.Between` throw `ArgumentException` when `min` is
  greater than `max` (for bounds of the same type, and for the string
  overloads). Reversed bounds previously matched nothing, so the guard never
  fired.
- `Is.All` and `ThrowIf.All` on an empty `ReadOnlySpan<T>` (which arrays bind
  to on C# 14) now behave like the `IEnumerable<T>` overload and
  `Enumerable.All`: `Is.All` returns `true` and `ThrowIf.All` throws.
  Previously an empty array and an empty list gave opposite results.
- The span overloads of `Is.All`, `Is.Any`, `ThrowIf.All` and `ThrowIf.Any`
  validate their arguments even when the span is empty.
- `Is.NullOrEmpty` and `ThrowIf.NullOrEmpty` on a `ReadOnlySpan<T>` treat
  only an empty span as empty. A span whose elements were all `null` used to
  count as empty, while the same array or list did not.

### Removed

- The `net6.0` and `net7.0` targets. The package targets `net8.0`, `net9.0`
  and `net10.0`.

### Fixed

- Selector-based `NullOrEmpty` guards cache compiled selectors by expression
  structure. The previous cache was keyed by expression instance and never
  hit, so every call recompiled the selector; calls are now roughly 40–50x
  faster with about 90% less allocation. Selectors that read captured
  variables are still compiled on every call.
- Selector-based `NullOrEmpty` guards no longer overflow the stack on
  self-referencing or recursively generic types. A type already being
  inspected on the same path, or nested more than 8 complex types deep, is
  only checked for null. Indexed properties are skipped instead of throwing.
- Selector-based `NullOrEmpty` checks on enumerable members dispose the
  enumerator they create.
- `ThrowIf.NullOrEmpty(value, selector)` throws a new `NullOrEmptyException`
  on every failure, with a message naming the selector (e.g.
  `Value 'o => o.Name' is null or empty.`). It previously rethrew one shared
  instance, whose stack trace and data were overwritten by concurrent callers.
- Built-in exception messages name the caller's argument expressions (e.g.
  `left=request.Age`). They previously always showed the guard's own
  parameter names (`value=value`).
- `ThrowIf.Any` and `ThrowIf.All` invoke the callback once instead of twice.
- `ThrowIf.All` and `ThrowIf.Any` no longer allocate their default exception
  when the guard passes.

## [0.1.2] - 2025-10-14
### Added
* Added a "📊 Benchmarks" section to the main README.md, providing a direct link to the SGuard.Benchmark/benchmarks/ folder for easy developer access to performance results. (#28)
* Ensured all benchmark results are discoverable and documented for each guard method (Is.* and ThrowIf.*), including All, Any, Between, GreaterThan, LessThan, and NullOrEmpty.
* No breaking changes to the core library or APIs.
* Improved developer experience and documentation clarity.

This update makes it much easier for contributors and users to find and review performance benchmarks for all guard methods.

## [0.1.1] - 2025-09-05
### Changed
- Throw.cs has been released for public use.
- ExceptionActivator.cs has been released for public use.
- Improved code readability and maintainability.

### Added
- Added XML documentation comments.

### Notes
## [0.1.0] - 2025-09-04
### Changed
- Versioning reset: re-released the package starting from `0.1.0`.
- Previous NuGet versions have been unlisted/removed.

### Added
- README updates: badges, “What’s New in 0.1.0”, and a “Test and Coverage Status” section with auto-updated results.
- Continuous integration workflow that runs tests, generates coverage, and updates README badges/summary.
- Packaging ensures README, LICENSE, and icon are included.

### Notes
- No functional breaking changes are expected for consumers adopting this version.

## [2.1.0] - 2025-01-03
### Changed
- **BREAKING CHANGE**: Changed license from GPL-3.0 to MIT
- Updated assembly version to 2.1.0
- Updated package metadata

### Added
- CODE_OF_CONDUCT.md
- CONTRIBUTING.md
- Enhanced documentation

## [2.0.x] - Previous versions
- Previous functionality under GPL-3.0 license