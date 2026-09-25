---
sidebar_position: 4
---

# String Comparisons

Learn culture-aware string validation with SGuard's comparison guards.

## Overview

These guards have **string-specific overloads** that take a `StringComparison` for proper cultural and ordinal handling:

- `Is.LessThan`, `Is.LessThanOrEqual`, `Is.GreaterThan`, `Is.GreaterThanOrEqual`, `Is.Between`
- `ThrowIf.Between`

`ThrowIf.LessThan`, `ThrowIf.LessThanOrEqual`, `ThrowIf.GreaterThan` and `ThrowIf.GreaterThanOrEqual` have **no**
`StringComparison` overload. To throw on a string ordering, test with `Is.*` and throw yourself:

```csharp
if (Is.GreaterThan(current, next, StringComparison.Ordinal))
{
    throw new InvalidOperationException("Items are not in order.");
}
```

:::warning Ordering is not a prefix, path, access or version check
These guards compare strings **lexicographically** (like sorting). They don't tell you whether a string starts with a
prefix, whether a path is inside a directory, or which version number is newer:

- `"/zzz"` and `"/app/data/../../etc"` are both *not less than* `"/app/data/"`, so a `LessThan` check lets them through.
- `"10.0.0"` is *less than* `"2.0.0"` in ordinal order.

Use `StartsWith` for prefixes, the [path check](#file-path-validation) below for directories, and `System.Version` for
versions.
:::

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

**Best for**: Identifiers, configuration keys, protocol values

```csharp
// Case-sensitive ordinal comparison
bool before = Is.LessThan("apple", "banana", StringComparison.Ordinal); // true

// Case-insensitive ordinal comparison: "Apple" and "apple" are equal, so neither is less
bool less = Is.LessThan("Apple", "apple", StringComparison.OrdinalIgnoreCase); // false
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
// Is the (fixed-width) shelf code within the range A00..C99?
bool inRange = Is.Between("B42", "A00", "C99", StringComparison.Ordinal); // true

// ThrowIf.Between throws when the value IS inside the range
ThrowIf.Between(code, "X00", "X99", StringComparison.Ordinal); // reject reserved codes X00..X99
```

If `min` is greater than `max` under the given comparison, both `Is.Between` and `ThrowIf.Between` throw an
`ArgumentException` ("The minimum must be less than or equal to the maximum.").

Ranges like this only make sense when the strings are compared the way they sort, as with fixed-width codes. Don't use
them for version numbers (`"2.10"` sorts before `"2.9"`) or as a prefix check.

## Real-World Examples

### Version Comparison

Compare versions as `System.Version`, not as strings. `Version` implements `IComparable<Version>`, so the generic guards
work with it directly:

```csharp
public class VersionValidator
{
    private static readonly Version MinVersion = new(2, 0, 0);
    private static readonly Version MaxVersion = new(3, 0, 0);

    public void ValidateVersion(string versionText)
    {
        if (!Version.TryParse(versionText, out var version))
        {
            throw new ArgumentException("Invalid version.", nameof(versionText));
        }

        ThrowIf.LessThan(version, MinVersion,
            new InvalidOperationException($"Version must be {MinVersion} or higher"));

        ThrowIf.GreaterThan(version, MaxVersion,
            new InvalidOperationException($"Version must be {MaxVersion} or lower"));
    }
}
```

`Version` compares missing components as lower than present ones (`2.0` < `2.0.0`), so compare versions with the same
number of components.

### File Path Validation

To check that a path stays inside a directory, resolve it first and then compare it to a root that ends with a
directory separator. Don't use `LessThan`/`GreaterThan` for this:

```csharp
public string ResolveDataPath(string relativePath)
{
    // The root must end with a separator, otherwise "/app/data-other" would match "/app/data"
    string root = Path.GetFullPath("/app/data/");
    string fullPath = Path.GetFullPath(Path.Combine(root, relativePath)); // resolves "..", "." and rooted input

    if (!fullPath.StartsWith(root, StringComparison.Ordinal))
    {
        throw new UnauthorizedAccessException("Path outside allowed directory");
    }

    return fullPath;
}
```

Use `StringComparison.OrdinalIgnoreCase` only when the file system is case-insensitive (typically Windows). This check
doesn't follow symbolic links; if the directory can contain links, resolve them as well.

### Username Validation

Duplicate checks are equality checks, not ordering checks:

```csharp
public class UserValidator
{
    public void ValidateUsername(string username, string existingUsername)
    {
        ThrowIf.NullOrEmpty(username);
        
        // Case-insensitive check for duplicates
        if (string.Equals(username, existingUsername, StringComparison.OrdinalIgnoreCase))
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
        if (Is.GreaterThan(names[i], names[i + 1], StringComparison.CurrentCulture))
        {
            throw new InvalidOperationException($"Names are not sorted: '{names[i]}' > '{names[i + 1]}'");
        }
    }
}
```

### Configuration Key Validation

```csharp
public class ConfigValidator
{
    public void ValidateKey(string key)
    {
        // Configuration keys must start with "app." (a prefix check, so use StartsWith)
        if (!key.StartsWith("app.", StringComparison.Ordinal))
        {
            throw new ArgumentException($"Invalid config key: {key}");
        }
    }
}
```

An ordering check such as `Is.Between(key, "app.", "app.~", StringComparison.Ordinal)` would reject valid keys whose next
character sorts after `~`, for example `"app.é"`.

### API Version Header Validation

```csharp
public class ApiVersionValidator
{
    private static readonly Version MinSupported = new(1, 0);
    private static readonly Version MaxSupported = new(2, 0);

    public void ValidateApiVersion(string requestVersion)
    {
        if (!Version.TryParse(requestVersion, out var version))
        {
            throw new NotSupportedException("Invalid API version");
        }

        ThrowIf.LessThan(version, MinSupported,
            new NotSupportedException($"API version {version} is no longer supported"));

        ThrowIf.GreaterThan(version, MaxSupported,
            new NotSupportedException($"API version {version} is not yet supported"));
    }
}
```

## Combining with Callbacks

```csharp
bool isValidCode = Is.Between(
    code, 
    "A00", 
    "C99", 
    StringComparison.Ordinal,
    SGuardCallbacks.OnSuccess(() => logger.LogInformation("Code in range"))
    + SGuardCallbacks.OnFailure(() => logger.LogWarning("Code out of range")));
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

For identifiers, keys and protocol values:

```csharp
bool before = Is.LessThan(configKeyA, configKeyB, StringComparison.Ordinal);
```

### 3. Use CurrentCulture for Display

For user-facing strings that should sort according to user's locale:

```csharp
bool sorted = Is.LessThan(displayName1, displayName2, StringComparison.CurrentCulture);
```

### 4. Use InvariantCulture for Consistency

For strings that need predictable behavior across systems but with culture rules:

```csharp
if (Is.LessThan(value, threshold, StringComparison.InvariantCulture))
{
    throw new ArgumentException("Value sorts before the threshold.", nameof(value));
}
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
if (name.CompareTo("M") < 0) { }

// DO: Explicit and predictable
if (Is.LessThan(name, "M", StringComparison.Ordinal)) { }
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

### Pitfall 4: Ordering Instead of Prefix, Path or Version Checks

```csharp
// DON'T: Lexicographic ordering
Is.LessThan("10.0.0", "2.0.0", StringComparison.Ordinal);  // true, although 10.0.0 is newer
Is.LessThan("/zzz", "/app/data/", StringComparison.Ordinal); // false, so a "path < root" check lets it through

// DO
Is.LessThan(new Version("10.0.0"), new Version("2.0.0"));    // false
bool isConfigKey = key.StartsWith("app.", StringComparison.Ordinal);
```

## Next Steps

- [Real-World Examples](./real-world-examples) - Complete validation scenarios
- [Comparison Guards](./comparison-guards) - Overview of all comparison methods
- [Best Practices](../advanced/best-practices) - Guidelines for effective validation
