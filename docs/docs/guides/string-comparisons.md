---
sidebar_position: 4
---

# String Comparisons

Learn culture-aware string validation with SGuard's comparison guards.

## Overview

All comparison guards (`LessThan`, `LessThanOrEqual`, `GreaterThan`, `GreaterThanOrEqual`, `Between`) have **string-specific overloads** that accept `StringComparison` for proper cultural and ordinal handling.

## Why StringComparison Matters

Different comparison modes produce different results:

```csharp
// These may give different results depending on culture
"straße" vs "strasse"
"Apple" vs "apple"
"café" vs "cafe"
```

Always specify `StringComparison` to ensure predictable behavior across cultures and environments.

## StringComparison Options

- **`Ordinal`**: Binary comparison, case-sensitive, no culture rules
- **`OrdinalIgnoreCase`**: Binary comparison, case-insensitive, no culture rules
- **`CurrentCulture`**: Culture-aware, case-sensitive
- **`CurrentCultureIgnoreCase`**: Culture-aware, case-insensitive
- **`InvariantCulture`**: Invariant culture rules, case-sensitive
- **`InvariantCultureIgnoreCase`**: Invariant culture rules, case-insensitive

## Ordinal Comparisons

**Best for**: File paths, identifiers, configuration keys, protocol values

```csharp
// Case-sensitive ordinal comparison
bool before = Is.LessThan("apple", "banana", StringComparison.Ordinal);

// Case-insensitive ordinal comparison
bool equal = Is.LessThan("Apple", "apple", StringComparison.OrdinalIgnoreCase); // false

// Throw if ordering is wrong
ThrowIf.GreaterThan("config.dev", "config.prod", StringComparison.Ordinal);
```

### Why Ordinal?

- **Fastest**: No culture lookup or special rules
- **Predictable**: Same result everywhere
- **Safe**: Avoids culture-specific quirks

## Culture-Aware Comparisons

**Best for**: User-facing strings, sorting for display, localized content

```csharp
// Current culture
bool less = Is.LessThan("café", "cafe", StringComparison.CurrentCulture);

// Invariant culture (predictable across systems)
bool before = Is.LessThan("straße", "strasse", StringComparison.InvariantCulture);
```

## Between with Strings

String `Between` guards use inclusive comparisons:

```csharp
// Check if version is in range
bool inRange = Is.Between("2.5", "2.0", "3.0", StringComparison.Ordinal);

// Throw if version is NOT in allowed range
if (!Is.Between(version, "2.0", "3.0", StringComparison.Ordinal))
{
    throw new InvalidOperationException("Version out of range");
}
```

## Real-World Examples

### Version Comparison

```csharp
public class VersionValidator
{
    public void ValidateVersion(string version)
    {
        const string MinVersion = "2.0.0";
        const string MaxVersion = "3.0.0";
        
        ThrowIf.LessThan(version, MinVersion, StringComparison.Ordinal,
            new InvalidOperationException($"Version must be {MinVersion} or higher"));
        
        ThrowIf.GreaterThan(version, MaxVersion, StringComparison.Ordinal,
            new InvalidOperationException($"Version must be {MaxVersion} or lower"));
    }
}
```

### File Path Validation

```csharp
public void ValidatePath(string path)
{
    const string AllowedPrefix = "/app/data/";
    
    // Ensure path starts with allowed prefix (case-insensitive on Windows)
    if (Is.LessThan(path, AllowedPrefix, StringComparison.OrdinalIgnoreCase))
    {
        throw new UnauthorizedAccessException("Path outside allowed directory");
    }
}
```

### Username Validation

```csharp
public class UserValidator
{
    public void ValidateUsername(string username, string existingUsername)
    {
        ThrowIf.NullOrEmpty(username);
        
        // Case-insensitive check for duplicates
        if (!Is.LessThan(username, existingUsername, StringComparison.OrdinalIgnoreCase) &&
            !Is.GreaterThan(username, existingUsername, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Username already exists");
        }
    }
}
```

### Locale-Aware Sorting Validation

```csharp
public void ValidateSortOrder(List<string> names)
{
    for (int i = 0; i < names.Count - 1; i++)
    {
        // Ensure names are sorted by current culture
        ThrowIf.GreaterThan(
            names[i], 
            names[i + 1], 
            StringComparison.CurrentCulture,
            new InvalidOperationException($"Names are not sorted: '{names[i]}' > '{names[i + 1]}'"));
    }
}
```

### Configuration Key Validation

```csharp
public class ConfigValidator
{
    public void ValidateKey(string key)
    {
        // Configuration keys should use ordinal comparison
        const string MinKey = "app.";
        const string MaxKey = "app.~";
        
        if (!Is.Between(key, MinKey, MaxKey, StringComparison.Ordinal))
        {
            throw new ArgumentException($"Invalid config key: {key}");
        }
    }
}
```

### API Version Header Validation

```csharp
public class ApiVersionValidator
{
    public void ValidateApiVersion(string requestVersion)
    {
        const string MinSupported = "1.0";
        const string MaxSupported = "2.0";
        
        ThrowIf.LessThan(
            requestVersion, 
            MinSupported, 
            StringComparison.Ordinal,
            new NotSupportedException($"API version {requestVersion} is no longer supported"));
        
        ThrowIf.GreaterThan(
            requestVersion, 
            MaxSupported, 
            StringComparison.Ordinal,
            new NotSupportedException($"API version {requestVersion} is not yet supported"));
    }
}
```

## Combining with Callbacks

```csharp
bool isValidVersion = Is.Between(
    version, 
    "1.0", 
    "2.0", 
    StringComparison.Ordinal,
    SGuardCallbacks.OnSuccess(() => logger.LogInformation("Version validated"))
    + SGuardCallbacks.OnFailure(() => logger.LogWarning("Invalid version")));
```

## Best Practices

### 1. Always Specify StringComparison

**Don't:**
```csharp
// Ambiguous—uses default culture-dependent comparison
string.Compare(a, b) < 0
```

**Do:**
```csharp
// Clear and explicit
Is.LessThan(a, b, StringComparison.Ordinal)
```

### 2. Use Ordinal for Non-User Strings

For identifiers, keys, paths, and protocol values:

```csharp
ThrowIf.GreaterThan(configKey, "app.", StringComparison.Ordinal);
```

### 3. Use CurrentCulture for Display

For user-facing strings that should sort according to user's locale:

```csharp
bool sorted = Is.LessThan(displayName1, displayName2, StringComparison.CurrentCulture);
```

### 4. Use InvariantCulture for Consistency

For strings that need predictable behavior across systems but with culture rules:

```csharp
ThrowIf.LessThan(value, threshold, StringComparison.InvariantCulture);
```

## Performance

- **Ordinal is fastest**: No culture lookup
- **OrdinalIgnoreCase is fast**: Simple case folding
- **Culture-aware is slower**: Requires culture data and complex rules

For performance-critical paths, prefer `Ordinal` or `OrdinalIgnoreCase`.

## Common Pitfalls

### Pitfall 1: Using Default Comparison

```csharp
// DON'T: Culture-dependent, unpredictable
if (version.CompareTo("2.0") < 0) { }

// DO: Explicit and predictable
if (Is.LessThan(version, "2.0", StringComparison.Ordinal)) { }
```

### Pitfall 2: Case-Sensitive When You Mean Insensitive

```csharp
// DON'T: Case-sensitive
Is.LessThan("Apple", "banana", StringComparison.Ordinal); // true (uppercase < lowercase)

// DO: Use OrdinalIgnoreCase if case doesn't matter
Is.LessThan("Apple", "banana", StringComparison.OrdinalIgnoreCase);
```

### Pitfall 3: Culture Assumptions

```csharp
// DON'T: Assumes current culture
Is.LessThan(userInput, "threshold", StringComparison.CurrentCulture);

// DO: Use Ordinal for non-linguistic strings
Is.LessThan(userInput, "threshold", StringComparison.Ordinal);
```

## Next Steps

- [Real-World Examples](./real-world-examples) - Complete validation scenarios
- [Comparison Guards](./comparison-guards) - Overview of all comparison methods
- [Best Practices](../advanced/best-practices) - Guidelines for effective validation
