# SGuard

[![CI](https://github.com/selcukgural/SGuard/actions/workflows/ci.yml/badge.svg)](https://github.com/selcukgural/SGuard/actions)
[![NuGet](https://img.shields.io/nuget/v/SGuard.svg)](https://www.nuget.org/packages/SGuard)
[![NuGet Downloads](https://img.shields.io/nuget/dt/SGuard.svg)](https://www.nuget.org/packages/SGuard)
[![Docs](https://img.shields.io/badge/docs-selcukgural.github.io%2FSGuard-blue)](https://selcukgural.github.io/SGuard/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Matrix Chat](https://img.shields.io/badge/chat-on%20matrix-4fc08d)](https://matrix.to/#/#sguard:gitter.im)

Guard clauses for .NET 8, 9 and 10. Validate arguments, domain invariants and incoming requests in one line each,
either by throwing (`ThrowIf.*`) or by getting a `bool` back (`Is.*`). Exceptions name the failing expression, a
passing guard costs about as much as a hand-written `if` and allocates nothing, and the package has no dependencies.

```csharp
ThrowIf.NullOrEmpty(command, c => c.Customer.Email);
// NullOrEmptyException: Value 'c => c.Customer.Email' is null or empty.   (a null Customer counts as empty too)
```

📖 **Documentation:** [selcukgural.github.io/SGuard](https://selcukgural.github.io/SGuard/): [Quick Start](https://selcukgural.github.io/SGuard/docs/getting-started/quick-start), [`Is.*` API](https://selcukgural.github.io/SGuard/docs/api/is), [`ThrowIf.*` API](https://selcukgural.github.io/SGuard/docs/api/throwif), [performance guide](https://selcukgural.github.io/SGuard/docs/advanced/performance)

## 📦 Installation

```bash
dotnet add package SGuard
```

```csharp
using SGuard;             // ThrowIf, Is, SGuardCallbacks, SGuardOptions
using SGuard.Exceptions;  // NullOrEmptyException, BetweenException, GreaterThanException, ...
```

## ⚡ Quick start

```csharp
public sealed class Product
{
    public Product(string sku, string name, decimal price, int stock)
    {
        ThrowIf.NullOrEmpty(sku);          // null or ""  -> NullOrEmptyException (ParamName = "sku")
        ThrowIf.NullOrEmpty(name);
        ThrowIf.LessThanOrEqual(price, 0m); // price <= 0 -> LessThanOrEqualException
        ThrowIf.LessThan(stock, 0);         // stock < 0  -> LessThanException

        (Sku, Name, Price, Stock) = (sku, name, price, stock);
    }

    public string Sku { get; }
    public string Name { get; }
    public decimal Price { get; }
    public int Stock { get; }
}
```

Read every `ThrowIf.X(a, b)` as "throw if `a` is X `b`". All built-in exceptions derive from `ArgumentException`.

### `ThrowIf.*` or `Is.*`?

| Use | When | Example |
|---|---|---|
| `ThrowIf.*` | Only a bug would make the value invalid: constructor and method arguments, domain invariants | `ThrowIf.NullOrEmpty(customerId);` |
| `Is.*` | Invalid input is expected and handled: user input, request bodies, messages from a queue | `if (!Is.Between(qty, 1, 100)) return BadRequest();` |

A failing `ThrowIf.*` costs a thrown exception (about 2 µs on .NET 10, 13 µs on .NET 8), so don't use exceptions for
input that is often invalid.

## 🧰 Guards at a glance

| Guard | `Is.*` returns `true` when | `ThrowIf.*` throws when | Example |
|---|---|---|---|
| `NullOrEmpty` | the value is null, empty or default | the same | `ThrowIf.NullOrEmpty(order.Lines);` |
| `NullOrEmpty` + selector | anything on the path is null, or the selected value is empty | the same | `ThrowIf.NullOrEmpty(order, o => o.Customer.Email);` |
| `GreaterThan` / `GreaterThanOrEqual` | `a > b` / `a >= b` | the same | `ThrowIf.GreaterThan(pageSize, 100);` |
| `LessThan` / `LessThanOrEqual` | `a < b` / `a <= b` | the same | `ThrowIf.LessThanOrEqual(amount, 0m);` |
| `Between` | `min <= value <= max` | the value is **inside** the range | `if (!Is.Between(port, 1, 65535)) ...` |
| `Any` / `All` | some / every element matches | the same | `ThrowIf.Any(lines, l => l.Quantity <= 0);` |
| `Email` (`Is` only) | the string looks like an email address | — | `Is.Email(request.Email)` |

What counts as empty: `null`; `""` (a whitespace-only string is **not** empty); an empty array, collection, span or
enumerable; and the default value of a value type: `0`, `false`, `Guid.Empty`, `default(DateTime)`, the default enum
value or any default struct (also inside a nullable, so `int? x = 0` is empty).

Comparisons work with any `IComparable<T>`: numbers, `decimal`, `DateTime`, `DateOnly`, `TimeSpan`, `Version`, your own
types. The `Is.*` comparisons and `ThrowIf.Between` have string overloads that take a `StringComparison`.

## 📝 Real-world scenarios

### Domain entity: protect invariants

```csharp
public sealed class Order
{
    private readonly List<OrderLine> _lines;

    public Order(Guid customerId, IReadOnlyList<OrderLine> lines)
    {
        ThrowIf.NullOrEmpty(customerId); // Guid.Empty counts as empty
        ThrowIf.NullOrEmpty(lines);
        ThrowIf.Any(lines, l => l.Quantity <= 0, new DomainException("Every order line needs a positive quantity."));

        CustomerId = customerId;
        _lines = [..lines];
    }

    public Guid CustomerId { get; }
    public IReadOnlyList<OrderLine> Lines => _lines;
    public decimal DiscountPercent { get; private set; }

    public void ApplyDiscount(decimal percent)
    {
        ThrowIf.LessThan(percent, 0m);
        ThrowIf.GreaterThan(percent, 50m, new DomainException("Discounts above 50% need manager approval."));

        DiscountPercent = percent;
    }
}

public sealed record OrderLine(string Sku, int Quantity, decimal UnitPrice);

public sealed class DomainException(string message) : Exception(message);
```

### Command handler: check nested data with a selector

A selector walks the path like C#'s `?.`: a `null` anywhere on it counts as empty, so you get a `NullOrEmptyException`
that names the path instead of a `NullReferenceException`.

```csharp
public sealed record PlaceOrder(Customer? Customer, Address? ShippingAddress, List<OrderLine>? Lines);
public sealed record Customer(Guid Id, string? Email);
public sealed record Address(string? Street, string? City, string? PostalCode);

public sealed class PlaceOrderHandler(IOrderRepository orders)
{
    public async Task<Guid> Handle(PlaceOrder command, CancellationToken ct)
    {
        ThrowIf.NullOrEmpty(command, c => c.Customer!.Email);
        ThrowIf.NullOrEmpty(command, c => c.ShippingAddress!.City);
        ThrowIf.NullOrEmpty(command, c => c.ShippingAddress!.PostalCode);
        ThrowIf.NullOrEmpty(command, c => c.Lines);
        ThrowIf.NullOrEmpty(command, c => c.Lines![0].Sku); // indexers and instance methods work too

        var order = new Order(command.Customer!.Id, command.Lines!);
        await orders.Add(order, ct);
        return order.CustomerId;
    }
}

public interface IOrderRepository
{
    Task Add(Order order, CancellationToken ct);
}
```

Selectors are compiled once and cached, so a repeated check costs well under a microsecond. Guard each property you
require: a selector that points at a whole object (`c => c.ShippingAddress`) only fails when that object is null or
**all** of its properties are empty.

### Minimal API: reject bad input without exceptions

User input is often invalid, so check it with `Is.*` and return a `400` instead of throwing.

```csharp
app.MapGet("/products", (string? term, int page, int pageSize) =>
{
    var errors = new Dictionary<string, string[]>();

    if (Is.LessThan(page, 1))
        errors[nameof(page)] = ["Page starts at 1."];

    if (!Is.Between(pageSize, 1, 100))
        errors[nameof(pageSize)] = ["Page size must be between 1 and 100."];

    if (!Is.NullOrEmpty(term) && Is.GreaterThan(term!.Length, 200))
        errors[nameof(term)] = ["Search term is too long."];

    return errors.Count > 0
        ? Results.ValidationProblem(errors)
        : Results.Ok(/* search(term, page, pageSize) */);
});

app.MapPost("/newsletter", (SubscribeRequest request) =>
    !Is.NullOrEmpty(request.Email) && Is.Email(request.Email!) // Is.Email throws for null or ""
        ? Results.NoContent()
        : Results.ValidationProblem(new Dictionary<string, string[]> { ["email"] = ["Enter a valid email address."] }));

public sealed record SubscribeRequest(string? Email);
```

### Configuration: fail at startup, not at the first request

```csharp
builder.Services.AddOptions<SmtpOptions>()
    .Bind(builder.Configuration.GetSection("Smtp"))
    .Validate(o => !Is.NullOrEmpty(o.Host), "Smtp:Host is required.")
    .Validate(o => Is.Between(o.Port, 1, 65535), "Smtp:Port must be between 1 and 65535.")
    .Validate(o => Is.Between(o.Timeout, TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(2)), "Smtp:Timeout is out of range.")
    .ValidateOnStart();

public sealed class SmtpOptions
{
    public string? Host { get; set; }
    public int Port { get; set; } = 587;
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}
```

### Message consumer: count bad batches, keep going

Callbacks run side effects such as metrics or logging on the outcome of a guard, without an extra `if`.

```csharp
public sealed class PaymentBatchConsumer(ILogger<PaymentBatchConsumer> logger, Counter<long> rejectedBatches)
{
    public bool TryProcess(IReadOnlyList<PaymentEvent> batch)
    {
        var onRejected = SGuardCallbacks.OnFailure(() => rejectedBatches.Add(1));

        if (Is.NullOrEmpty(batch) || !Is.All(batch, e => e.Amount > 0 && e.Currency.Length == 3, onRejected))
        {
            logger.LogWarning("Rejected a payment batch of {Count} events", batch?.Count ?? 0);
            return false; // move it to the dead-letter queue
        }

        // process the batch...
        return true;
    }
}

public sealed record PaymentEvent(Guid Id, decimal Amount, string Currency);
```

### Web API: turn guard exceptions into `400 Bad Request`

Built-in exceptions derive from `ArgumentException` and set `ParamName` to the failing expression, so one handler maps
them all.

```csharp
public sealed class ArgumentExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not ArgumentException argument)
            return false;

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Invalid request",
            Detail = argument.Message,
            Extensions = { ["param"] = argument.ParamName } // e.g. "c => c.Customer.Email"
        }, ct);

        return true;
    }
}

// Program.cs
builder.Services.AddExceptionHandler<ArgumentExceptionHandler>();
builder.Services.AddProblemDetails();
app.UseExceptionHandler();
```

### Your own exception types

Every `ThrowIf.*` guard except the span `NullOrEmpty` has an overload that takes an exception instance.
`NullOrEmpty`, `Between`, `GreaterThan`, `GreaterThanOrEqual` and `LessThanOrEqual` also take an exception type, created
with its parameterless constructor or with constructor arguments.

```csharp
ThrowIf.GreaterThan(amount, balance, new InsufficientFundsException(accountId));             // an instance
ThrowIf.GreaterThan<int, int, RateLimitExceededException>(requestsThisMinute, limit);        // new()
ThrowIf.NullOrEmpty<string?, ArgumentException>(apiKey, ["An API key is required.", "apiKey"]); // constructor arguments
```

The instance overloads create the exception before the check, even when the guard passes. On a hot path, prefer the
type overloads, which create it only when they throw.

## ⚠️ Things that surprise people

- **`ThrowIf.Between` throws when the value is *inside* the range.** To reject values outside a range, use
  `if (!Is.Between(...))` or a `ThrowIf.LessThan` plus `ThrowIf.GreaterThan` pair.
- **Both bounds of `Between` are inclusive**, and `min > max` throws `ArgumentException`.
- **Whitespace is not empty**: `ThrowIf.NullOrEmpty("   ")` passes. Trim first, or use a selector: `u => u.Name.Trim()`.
- **`NaN` never passes a comparison**: `Is.*` returns `false` and `ThrowIf.*` throws. So
  `if (Is.GreaterThan(x, max)) reject();` lets `NaN` through; use `ThrowIf.*` or `!Is.Between(...)` for floating-point
  input.
- **A callback's `Success` means the method returned `true`.** For `Is.*` that's the result
  (`Is.NullOrEmpty(null)` reports `Success`); for `ThrowIf.*` it means the guard didn't throw.
- **Exceptions thrown by a callback are swallowed**, so a callback can't break validation. Don't put work that must not
  be lost (an audit record) in a callback.
- **`Is.*` still throws for invalid arguments**: a `null` operand, predicate or source throws `ArgumentNullException`,
  and `Is.Email` throws for `null` or `""`.
- **A selector pointing at an object reads all of its public properties**, so lazy-loading proxies load and getters run.
  Point it at the scalar member you need (`o => o.Customer.Email`). Framework classes such as `Uri` are only checked
  for null.

## 🔒 Exception messages and personal data

Messages name the expressions and leave the checked values out, because exception messages end up in logs and error
trackers:

```csharp
ThrowIf.GreaterThan(request.Age, limit);
// GreaterThanException: Left value is greater than right value. Actual: left=request.Age, right=limit.
```

To include values (converted with `ToString()` and truncated to 64 characters) in `Message` and `Exception.Data`, opt
in once at startup. Only do this when the checked values can't be secrets or personal data:

```csharp
SGuardOptions.IncludeValuesInExceptions = true;
// GreaterThanException: '4217' is greater than '1000'. Actual: left=request.Age, right=limit.
```

## 🔁 Callbacks

```csharp
// ThrowIf: Failure just before it throws, Success when it passes
ThrowIf.NullOrEmpty(username, SGuardCallbacks.OnFailure(() => logger.LogWarning("Username missing")));

// Is: Success when it returns true, Failure when it returns false
bool inRange = Is.Between(score, 0, 100, SGuardCallbacks.OnFailure(() => metrics.Increment("score.out_of_range")));

// Combine them, or take the outcome directly
SGuardCallback both = SGuardCallbacks.OnSuccess(() => passed++) + SGuardCallbacks.OnFailure(() => failed++);
ThrowIf.LessThan(quantity, 1, outcome => lastOutcome = outcome);
```

A callback isn't invoked when the call itself is invalid, for example a `null` exception instance.

## 📊 Performance

Results for every guard, on .NET 8 and .NET 10 and with allocations, are in
[SGuard.Benchmark/benchmarks/](SGuard.Benchmark/benchmarks/). In short (Apple M3 Max, .NET 10 / .NET 8):

| Scenario | Time | Allocated |
|---|---|---|
| Passing guard (`ThrowIf.*`, `Is.*`) | ~1 ns / ~1–3 ns | none |
| Failing `ThrowIf.*` guard (throw + catch) | ~2 µs / ~13 µs | the exception |
| Cached selector (`ThrowIf.NullOrEmpty(order, o => o.Customer.Name)`) | ~0.2–0.5 µs / ~0.3–0.7 µs | the expression tree built at the call site |

See the [performance guide](https://selcukgural.github.io/SGuard/docs/advanced/performance) for details.

## 💬 Community

Ask questions or share feedback in the chat: [#sguard:gitter.im](https://matrix.to/#/#sguard:gitter.im).

## 🤝 Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## 🌐 Code of Conduct

This project adheres to the [.NET Foundation Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to
uphold this code.

## 📜 License

MIT. See the [LICENSE](LICENSE) file for details.

## 🔗 Links

- [Documentation](https://selcukgural.github.io/SGuard/)
- [NuGet Package](https://www.nuget.org/packages/SGuard)
- [Releases](https://github.com/selcukgural/sguard/releases)
- [Changelog](Changelog.md)
