---
sidebar_position: 1
---

# Performance

Understand SGuard's performance characteristics and optimization techniques.

## Benchmarks

Performance benchmarks for all guard methods are available in the [SGuard.Benchmark/benchmarks/](https://github.com/selcukgural/SGuard/tree/main/SGuard.Benchmark/benchmarks/) folder.

These benchmarks compare:
- `Is.*` vs `ThrowIf.*` methods
- Cached vs non-cached selector validations
- Different guard types (`NullOrEmpty`, `Between`, `LessThan`, `Any`, `All`, etc.)

## Key Performance Characteristics

### Expression Caching

Selector-based `NullOrEmpty` validations benefit from automatic expression caching:

```csharp
// First call: compiles and caches
ThrowIf.NullOrEmpty(order, o => o.Customer.Name);

// Subsequent calls: uses cached expression
ThrowIf.NullOrEmpty(anotherOrder, o => o.Customer.Name);
```

**Impact**: 10-50% faster for repeated selector-based validations.

### Comparison Guards

Comparison guards (`LessThan`, `Between`, etc.) are highly optimized:
- Direct `IComparable<T>.CompareTo` calls
- No boxing for value types
- Minimal allocations

```csharp
// Extremely fast—direct comparison
bool inRange = Is.Between(value, min, max);
```

### Collection Guards

`Any` and `All` use short-circuit evaluation:

```csharp
// Stops at first match
bool hasNull = Is.Any(largeList, x => x is null);

// Stops at first non-match
bool allValid = Is.All(largeList, x => x.IsValid);
```

**Impact**: O(1) best case, O(n) worst case.

## Performance Tips

### 1. Use Direct Checks When Possible

Avoid selectors if you can validate directly:

```csharp
// Faster: Direct check
ThrowIf.NullOrEmpty(user.Email);

// Slower: Selector (but still fast due to caching)
ThrowIf.NullOrEmpty(user, u => u.Email);
```

### 2. Prefer Is.* for Hot Paths

In performance-critical code, `Is.*` avoids exception overhead:

```csharp
// Faster: No exception throwing
if (Is.Between(value, min, max))
{
    // handle valid case
}

// Slower: Exception construction and throwing
try
{
    ThrowIf.Between(value, min, max);
}
catch
{
    // handle invalid case
}
```

However, **exceptions should be exceptional**. If validation failures are rare, `ThrowIf.*` is perfectly fine.

### 3. Use Ordinal String Comparisons

`StringComparison.Ordinal` is fastest for string comparisons:

```csharp
// Fastest: Binary comparison
Is.LessThan(a, b, StringComparison.Ordinal);

// Slower: Culture-aware comparison
Is.LessThan(a, b, StringComparison.CurrentCulture);
```

### 4. Avoid Unnecessary Callbacks

Callbacks add a small overhead. Only use when needed:

```csharp
// Faster: No callback
ThrowIf.NullOrEmpty(value);

// Slightly slower: Callback overhead
ThrowIf.NullOrEmpty(value, SGuardCallbacks.OnFailure(() => logger.Log("Failed")));
```

The overhead is minimal, but matters in extremely hot loops.

### 5. Short-Circuit Complex Validations

Order validations from most likely to fail to least:

```csharp
// Check cheap conditions first
ThrowIf.NullOrEmpty(items);  // Fast null check
ThrowIf.Any(items, i => i.IsInvalid);  // More expensive predicate
```

## Benchmark Results Summary

Based on SGuard.Benchmark results:

| Guard Type | Operation | Time (ns) | Notes |
|-----------|-----------|-----------|-------|
| NullOrEmpty | Direct check | ~5-10 | Extremely fast |
| NullOrEmpty | Selector (cached) | ~20-30 | Fast with caching |
| Between | Numeric | ~5-10 | Direct comparison |
| LessThan | Numeric | ~5-10 | Direct comparison |
| Any | Short-circuit | ~10-100 | Depends on match position |
| All | Short-circuit | ~10-100 | Depends on match position |
| String comparison | Ordinal | ~10-20 | Fast binary compare |
| String comparison | Culture | ~50-100 | Culture lookup overhead |

*Note: Actual numbers depend on hardware and runtime. See benchmark folder for detailed results.*

## Real-World Performance

In typical applications:
- **Guard overhead is negligible** compared to business logic
- **Focus on correctness first**, optimize if profiling shows issues
- **Expression caching** makes repeated validations essentially free

### Example: API Endpoint

```csharp
[HttpPost]
public IActionResult CreateOrder([FromBody] CreateOrderRequest req)
{
    // Validation overhead: ~100-500ns total
    ThrowIf.NullOrEmpty(req);
    ThrowIf.NullOrEmpty(req, r => r.Items);
    ThrowIf.Any(req.Items, i => i.Quantity <= 0);
    
    // Business logic: ~1-100ms
    var order = _orderService.Create(req);
    
    return Ok(order);
}
```

Guard overhead is **0.001% - 0.05%** of total request time. The real bottlenecks are:
- Database queries
- External API calls
- Complex business logic

## When to Optimize

Profile your application first. Optimize guards only if:
1. Profiling shows guards are a bottleneck (rare)
2. You're in an extremely hot loop (millions of iterations)
3. You're targeting sub-millisecond response times

For 99% of applications, SGuard's performance is more than adequate.

## Microbenchmarking

To run benchmarks yourself:

```bash
cd SGuard.Benchmark
dotnet run -c Release
```

Results are saved to the `benchmarks/` folder with detailed statistics.

## Memory Allocations

SGuard is designed to minimize allocations:
- **No allocations** for simple checks (null, comparison guards)
- **One allocation** for exception creation (only when validation fails)
- **Cached expressions** reuse compiled delegates

Typical allocation overhead: **0 bytes** for successful validations, **~100-500 bytes** for failures (exception object).

## Next Steps

- [Best Practices](./best-practices) - Guidelines for effective validation
- [Benchmarks](https://github.com/selcukgural/SGuard/tree/main/SGuard.Benchmark/benchmarks/) - Detailed benchmark results
- [Expression Caching](../core-concepts/expression-caching) - How caching works
