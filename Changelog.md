# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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