# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Fixed

- Selector-based `NullOrEmpty` guards now cache compiled selectors by
  expression structure. The previous cache was keyed by expression instance
  and never hit, so every call recompiled the selector; calls are now roughly
  40–50x faster with about 90% less allocation. `Is.NullOrEmpty` and
  `ThrowIf.NullOrEmpty` share the cache. Selectors that read captured
  variables are still compiled on every call.
- Built-in exception messages now name the caller's argument expressions
  (e.g. `left=request.Age`). They previously always showed the guard's own
  parameter names (`value=value`).

### Added

- `SGuardOptions.IncludeValuesInExceptions` to include checked values in
  built-in exception messages and `Exception.Data`. Values are written with
  `ToString()` and truncated to 64 characters.

### Changed

- Built-in exceptions (`Between`, `GreaterThan`, `GreaterThanOrEqual`,
  `LessThan`, `LessThanOrEqual`, `NullOrEmpty`) no longer include the checked
  values in `Message` or `Exception.Data` unless
  `SGuardOptions.IncludeValuesInExceptions` is enabled. When enabled,
  `Exception.Data` holds the formatted strings instead of the values.
- `ThrowIf.Between`, `GreaterThan`, `GreaterThanOrEqual`, `LessThan`,
  `LessThanOrEqual`, `NullOrEmpty` and the matching `Throw.*Exception` helpers
  take optional `[CallerArgumentExpression]` parameters. Callers only need to
  recompile; assemblies compiled against the previous version must be rebuilt.
- Selector-based `NullOrEmpty` guards no longer overflow the stack on
  self-referencing or recursively generic types. A type already being inspected
  on the same path, or nested more than 8 complex types deep, is only checked
  for null. Indexed properties are skipped instead of throwing.
- `Is.Email` now rejects addresses followed by a line break
  (`"a@example.com\n"`) and addresses that contain non-ASCII letters such as
  the Kelvin sign (U+212A). Inputs longer than 254 characters are rejected
  before matching.

### Added

- `Is.Email(email, regex, regexOptions, matchTimeout, callback)` overload
  that limits how long a custom pattern may run.

### Changed

- `Is.Email` with a custom pattern now stops matching after
  `Is.DefaultEmailRegexTimeout` (1 second) and throws
  `RegexMatchTimeoutException`; previously matching had no time limit.

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