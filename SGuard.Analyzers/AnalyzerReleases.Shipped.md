; Shipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

## Release 1.0.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
SGUARD101 | Usage | Info | NullOrEmpty rejects zero and false
SGUARD102 | Usage | Info | Pass a factory instead of an exception instance
SGUARD103 | Usage | Warning | The minimum is greater than the maximum
SGUARD104 | Usage | Info | The selector selects a whole object
