---
sidebar_position: 4
---

# Expression Caching

Learn how SGuard optimizes performance through efficient expression caching.

## Overview

SGuard caches the compiled selectors used by `Is.NullOrEmpty(value, selector)` and `ThrowIf.NullOrEmpty(value, selector)` to reduce overhead for repeated validations. Guards without a selector compile nothing and don't use the cache. This optimization is **automatic**, **thread-safe**, and requires no configuration.

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

### Without Caching (Before This Release)
- Every validation recompiles the selector expression (the earlier cache was keyed by expression instance and never hit)
- Increased CPU overhead
- Higher memory allocations

### With Caching (SGuard's Approach)
- Expression compiled once, reused many times
- Reduced CPU overhead
- Much lower allocations for repeated validations (the call site still builds an expression tree on every call)

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

Measured on .NET 10 with a two-level selector (`o => o.Customer.Name`),
averaged over 20,000 calls, comparing the previous behaviour (recompiling on every call) with the cache:

| Guard | Without caching | With caching |
|---|---|---|
| `ThrowIf.NullOrEmpty` | ~83 µs, ~8 KB | ~1.9 µs, ~0.8 KB |
| `Is.NullOrEmpty` | ~77 µs, ~7.7 KB | ~1.5 µs, ~0.7 KB |

Times are per call; sizes are memory allocated per call. The remaining
allocation is the expression tree the C# compiler builds at the call site on
every call. This matches the Changelog: selector-based calls are roughly 40–50x
faster with about 90% less allocation. These figures come from that measurement,
not from the committed BenchmarkDotNet results in `SGuard.Benchmark/benchmarks/`,
which were recorded before the cache existed.

## Limitations

- **Caching is per-expression structure**: Different selectors create
  separate cache entries. `Is.NullOrEmpty` and `ThrowIf.NullOrEmpty` share the
  same entry for the same selector.
- **Captured variables are not cached**: A selector that reads a captured
  variable (e.g. `_ => someLocal.Name` or `o => o.Items[index]` with a local
  `index`) embeds that variable's current value in its compiled form, so it is
  compiled on every call to stay correct. Selectors made only of the lambda
  parameter, member accesses, conversions, method calls, array indexing and
  constants of primitive types, enums, `string` or `decimal` are cached. Select
  from the lambda parameter (`x => x.Name`) to benefit from caching.
- **No cache eviction**: Entries remain for the application lifetime (this is
  usually fine as the cache size is bounded by the number of unique validation
  patterns in your code). Each selector input type holds at most 1,000
  entries; beyond that, selectors are compiled without being cached.

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

The cache stores, per selector input type:
- **Compiled delegates** (one per unique selector shape)
- **Cache keys** (the first expression tree seen for each shape, compared by structure)

In typical applications, the cache remains small because:
1. The number of unique validation patterns is limited
2. Expressions are lightweight once compiled
3. Cache growth is bounded by your codebase's validation patterns

## Next Steps

- [Performance](../advanced/performance) - See benchmarks and optimization tips
- [Best Practices](../advanced/best-practices) - Learn when to use selectors
- [Null/Empty Checks](../guides/null-empty-checks) - Master selector-based validation
