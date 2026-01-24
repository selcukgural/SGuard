---
sidebar_position: 4
---

# Expression Caching

Learn how SGuard optimizes performance through efficient expression caching.

## Overview

SGuard uses expression caching to reduce overhead for repeated validations, particularly when using selectors with `NullOrEmpty` guards. This optimization is **automatic**, **thread-safe**, and requires no configuration.

## What Is Cached?

When you use guards with selectors, SGuard compiles expressions for accessing nested properties:

```csharp
// First call: compiles and caches the selector expression
ThrowIf.NullOrEmpty(order, o => o.Customer.Name);

// Subsequent calls: uses cached compiled expression
ThrowIf.NullOrEmpty(anotherOrder, o => o.Customer.Name);
```

The compiled expression is stored in a thread-safe cache, eliminating the need to recompile on every call.

## Performance Benefits

### Without Caching (Naive Approach)
- Every validation recompiles the selector expression
- Increased CPU overhead
- Higher memory allocations

### With Caching (SGuard's Approach)
- Expression compiled once, reused many times
- Reduced CPU overhead
- Minimal allocations for repeated validations

## Cache Implementation

SGuard's cache is:

- **Thread-safe**: Uses `ConcurrentDictionary` for safe concurrent access
- **Automatic**: No configuration or manual cache management required
- **Efficient**: Keys are based on expression structure, not instance-specific values

## When Caching Helps Most

Expression caching provides the most benefit when:

1. **Validating in loops**: Processing collections with the same validation logic
2. **High-frequency validations**: API endpoints or event handlers with repeated guard patterns
3. **Deep property access**: Selectors that navigate multiple levels (e.g., `o => o.Customer.Address.City`)

### Example: Validating a Collection

```csharp
// Cache hit on every iteration after the first
foreach (var order in orders)
{
    ThrowIf.NullOrEmpty(order, o => o.Customer.Email);
}
```

## Benchmarks

See the [Performance](../advanced/performance) section for detailed benchmark results comparing cached vs. non-cached validations.

Typical improvements:
- **10-50% faster** for repeated selector-based validations
- **Reduced allocations** for expression compilation overhead

## Limitations

- **Caching is per-expression structure**: Different selectors create separate cache entries
- **No cache eviction**: Entries remain for the application lifetime (this is usually fine as the cache size is bounded by the number of unique validation patterns in your code)

## No Configuration Needed

Expression caching is enabled by default and requires no setup. Just use SGuard normally:

```csharp
// This is all you need—caching happens automatically
ThrowIf.NullOrEmpty(user, u => u.Profile.DisplayName);
```

## Thread Safety

All caching operations are thread-safe. Multiple threads can safely:

- Add new cache entries
- Read existing cache entries
- Execute validations concurrently

## Memory Considerations

The cache stores:
- **Compiled expressions** (one per unique selector pattern)
- **Cache keys** (expression structure metadata)

In typical applications, the cache remains small because:
1. The number of unique validation patterns is limited
2. Expressions are lightweight once compiled
3. Cache growth is bounded by your codebase's validation patterns

## Next Steps

- [Performance](../advanced/performance) - See benchmarks and optimization tips
- [Best Practices](../advanced/best-practices) - Learn when to use selectors
- [Null/Empty Checks](../guides/null-empty-checks) - Master selector-based validation
