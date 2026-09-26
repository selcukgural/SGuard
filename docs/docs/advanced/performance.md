---
sidebar_position: 1
---

# Performance

Understand SGuard's performance characteristics and optimization techniques.

## Benchmarks

BenchmarkDotNet results for all guard methods are available in the [SGuard.Benchmark/benchmarks/](https://github.com/selcukgural/SGuard/tree/main/SGuard.Benchmark/benchmarks/) folder.

These benchmarks compare:
- `Is.*` vs `ThrowIf.*` methods
- Calls with and without a callback
- Passing vs failing (throwing) guards
- Different guard types (`NullOrEmpty`, `Between`, `LessThan`, `GreaterThan`, `Any`, `All`) and collection sizes

The committed results were recorded in September 2026 on .NET 8 and .NET 10 (Apple M3 Max, BenchmarkDotNet 0.15.8,
short runs), with allocations. See [Expression Caching](../core-concepts/expression-caching#benchmarks) for a
cached-vs-uncached selector comparison.

## Key Performance Characteristics

### Expression Caching

Selector-based `NullOrEmpty` validations benefit from automatic expression caching:

```csharp
// First call: compiles and caches
ThrowIf.NullOrEmpty(order, o => o.Customer.Name);

// Subsequent calls: uses cached expression
ThrowIf.NullOrEmpty(anotherOrder, o => o.Customer.Name);
```

**Impact**: roughly 150–200x faster with about 90% less allocation than recompiling on every call. A cached selector
call still costs about half a microsecond and ~0.9 KB (see [Expression Caching](../core-concepts/expression-caching#benchmarks)),
mostly because the C# compiler builds a new expression tree at the call site on every call.

### Comparison Guards

Comparison guards (`LessThan`, `Between`, etc.) call `IComparable<T>.CompareTo` through generic type parameters, so
value types are not boxed. In the committed benchmarks, `Is.GreaterThan` and `Is.Between` on `int` take under 1 ns.

```csharp
// Direct comparison, no allocation
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

**Impact**: O(1) best case, O(n) worst case. In the committed benchmarks, a full pass over 1,000 elements takes
about 280 ns and grows linearly (about 4 µs for 15,000).

## Performance Tips

### 1. Use Direct Checks When Possible

Avoid selectors if you can validate directly:

```csharp
// Faster: Direct check
ThrowIf.NullOrEmpty(user.Email);

// Slower: Selector (cached, but still about half a microsecond per call)
ThrowIf.NullOrEmpty(user, u => u.Email);
```

### 2. Prefer Is.* for Hot Paths

In performance-critical code, `Is.*` avoids exception overhead. A passing `ThrowIf.*` guard costs about as much as a
hand-written `if` (under a nanosecond for a comparison), but a failing one throws, and a throw plus catch costs
about 2 µs on .NET 10 and 12–15 µs on .NET 8 (BenchmarkDotNet, Apple M3 Max):

```csharp
// Faster: No exception throwing
if (Is.Between(value, min, max))
{
    // handle the in-range case
}

// Slower when it fails: exception construction and throwing
try
{
    ThrowIf.Between(value, min, max);
}
catch (BetweenException)
{
    // handle the in-range case
}
```

However, **exceptions should be exceptional**. If validation failures are rare, `ThrowIf.*` is perfectly fine.

:::tip Input that fails often
Where invalid input is common or can be sent on purpose, such as a public endpoint or a message consumer, every
rejected request pays for a throw. A client sending invalid requests in a loop then costs you microseconds of CPU per
request instead of nanoseconds. Validate such input with `Is.*` and return the error (a `400` response, a rejected
message) without throwing; keep `ThrowIf.*` for arguments that only a bug would make invalid.
:::

### 3. Use Ordinal String Comparisons

The generic comparison overloads call `string.CompareTo`, which is culture-sensitive. For identifiers and keys, an
ordinal comparison is usually faster and gives the same result on every machine:

```csharp
// Binary comparison
Is.LessThan(a, b, StringComparison.Ordinal);

// Culture-aware comparison (for user-facing text)
Is.LessThan(a, b, StringComparison.CurrentCulture);
```

### 4. Avoid Unnecessary Callbacks

Callbacks add a small overhead. Only use when needed:

```csharp
// Faster: No callback
ThrowIf.NullOrEmpty(value);

// Slightly slower: the callback is invoked, and building it allocates a delegate
ThrowIf.NullOrEmpty(value, SGuardCallbacks.OnFailure(() => logger.Log("Failed")));
```

The committed benchmarks show a difference of well under a nanosecond for most guards. If you use the same
callback on a hot path, create it once and reuse it.

### 5. Short-Circuit Complex Validations

Run cheap checks before expensive ones:

```csharp
// Check cheap conditions first
ThrowIf.NullOrEmpty(items);  // Fast null/empty check
ThrowIf.Any(items, i => i.IsInvalid);  // More expensive predicate
```

## Benchmark Results Summary

From the committed results in `SGuard.Benchmark/benchmarks/` (Apple M3 Max):

| Guard | Scenario | .NET 10 | .NET 8 | Allocated |
|---|---|---|---|---|
| `Is.NullOrEmpty` | `null`, string, `int`, array, list | under 1 ns | ~0–2.5 ns | – |
| `Is.Between`, `Is.GreaterThan` | `int` | under 1 ns | under 1 ns | – |
| `Is.GreaterThan` | `string` (culture-sensitive `CompareTo`) | ~14–29 ns | ~14–27 ns | – |
| `Is.Any`, `Is.All` | 1,000 / 15,000 elements, full pass | ~280 ns / ~4 µs | ~540 ns / ~8 µs | – |
| `ThrowIf.*` | guard passes | ~0.6–1 ns | ~0.6–3 ns | – |
| `ThrowIf.*` | guard passes, with a callback | ~0.5–1.3 ns | ~1.3–4 ns | – |
| `ThrowIf.*` | guard throws (including the catch) | ~2 µs | ~13 µs | ~0.2–0.7 KB |
| `ThrowIf.NullOrEmpty` | cached selector, guard passes | ~230 ns | ~300 ns | 568 B |

*Actual numbers depend on hardware and runtime. See the benchmark folder for the full tables.*

## Real-World Performance

In typical applications:
- **Guard overhead is small** compared to I/O and business logic
- **Focus on correctness first**, optimize if profiling shows issues
- **Expression caching** removes the compilation cost of repeated selector validations; what remains is building
  the expression tree and evaluating it

### Example: API Endpoint

```csharp
[HttpPost]
public IActionResult CreateOrder([FromBody] CreateOrderRequest req)
{
    // A few guards: nanoseconds each, about half a microsecond for the selector
    ThrowIf.NullOrEmpty(req);
    ThrowIf.NullOrEmpty(req, r => r.Items);
    ThrowIf.Any(req.Items, i => i.Quantity <= 0);
    
    // Business logic: database calls, external services, ...
    var order = _orderService.Create(req);
    
    return Ok(order);
}
```

Guard overhead is typically a tiny fraction of total request time. The real bottlenecks are usually:
- Database queries
- External API calls
- Complex business logic

## When to Optimize

Profile your application first. Optimize guards only if:
1. Profiling shows guards are a bottleneck (rare)
2. You're in an extremely hot loop (millions of iterations)
3. Validation failures are frequent, so exceptions are thrown on a hot path

For most applications, SGuard's performance is more than adequate.

## Microbenchmarking

To run the benchmarks yourself (the project targets `net8.0`, `net9.0` and `net10.0`; `-f` picks the host):

```bash
cd SGuard.Benchmark

# Everything on .NET 8 and .NET 10, as the committed results were recorded (about two hours)
dotnet run -c Release -f net10.0 -- --filter '*' --runtimes net8.0 net10.0 --job short

# One class on the host runtime
dotnet run -c Release -f net10.0 -- --filter '*ThrowIfNullOrEmpt*'
```

BenchmarkDotNet writes its reports (including GitHub-flavored Markdown) to `BenchmarkDotNet.Artifacts/results/` in the
directory you run it from. The files in `SGuard.Benchmark/benchmarks/` are copies of a run and are not updated
automatically.

## Memory Allocations

The committed benchmarks report allocations in their `Allocated` column. In short:
- **`Is.*` comparisons and `Is.NullOrEmpty` without a selector** don't allocate.
- **A passing `ThrowIf.*` guard doesn't allocate** (except as noted below for selectors and exception instances). The
  test suite checks this for the comparison, `Between` and `NullOrEmpty` guards.
- **Failing guards** allocate the exception (and throwing it is far more expensive than the allocation).
- **Overloads that take an exception instance** allocate that instance at the call site on every call; the
  `TException` and `constructorArgs` overloads create it only on failure.
- **Selector-based `NullOrEmpty`** allocates the expression tree built at the call site on every call (about
  0.7–0.8 KB in the [Expression Caching](../core-concepts/expression-caching#benchmarks) measurement); the compiled
  delegate is reused.
- **Callbacks** built with `SGuardCallbacks.OnSuccess`/`OnFailure` or a capturing lambda allocate a delegate where
  they are created.

## Next Steps

- [Best Practices](./best-practices) - Guidelines for effective validation
- [Benchmarks](https://github.com/selcukgural/SGuard/tree/main/SGuard.Benchmark/benchmarks/) - Detailed benchmark results
- [Expression Caching](../core-concepts/expression-caching) - How caching works
