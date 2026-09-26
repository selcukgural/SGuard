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

Measured with BenchmarkDotNet (Apple M3 Max, .NET 10 / .NET 8) for a passing `ThrowIf.NullOrEmpty` guard:

| Selector | Compiled on every call | Cached |
|---|---|---|
| `o => o.Customer.Address.City` | ~80 µs, ~8 KB | ~0.42 µs / ~0.58 µs, 880 B |
| `o => captured.Customer.Name` (captured local) | ~85 µs, ~10 KB | ~0.52 µs / ~0.64 µs, ~1 KB |
| *Building the expression tree alone* | | ~0.30 µs / ~0.41 µs, 824 B |

Times are per call; sizes are memory allocated per call. Most of what remains is
the expression tree the C# compiler builds at the call site on every call (last
row), which no cache can avoid. These figures come from that measurement, not
from the committed BenchmarkDotNet results in `SGuard.Benchmark/benchmarks/`,
which were recorded before the cache existed.

## Limitations

- **Caching is per-expression structure**: Different selectors create
  separate cache entries. `Is.NullOrEmpty` and `ThrowIf.NullOrEmpty` share the
  same entry for the same selector.
- **Captured variables are cached too**: A selector that reads a captured
  variable (e.g. `_ => someLocal.Name` or `o => o.Items[index]` with a local
  `index`) is compiled once; each call passes its own captured values to the
  compiled delegate. Operators (`+`, `==`, `!`, ...), `??`, `?:`, `is`,
  `new` and array creation are cached as well. Only a selector containing a
  nested lambda (`o => o.Items.First(i => i.Active)`), an invocation, or a
  member or collection initializer (`new Dto { Name = o.Name }`) is compiled on
  every call; it still works, but costs tens of microseconds.
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
