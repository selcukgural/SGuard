---
sidebar_position: 3
---

# Callbacks API

Complete reference for the callback system.

## Overview

SGuard provides a unified callback model for adding side effects to guard validations. Every `ThrowIf.*` and `Is.*`
method takes an optional `SGuardCallback? callback = null` parameter. What `Success` and `Failure` mean differs
between the two APIs; see [Callback Behavior](#callback-behavior).

## SGuardCallback

Delegate type for callbacks (namespace `SGuard`):

```csharp
public delegate void SGuardCallback(GuardOutcome outcome);
```

Receives a `GuardOutcome` describing the result of the guard call. A parameterless lambda (`() => ...`) is not an
`SGuardCallback`; wrap it with `SGuardCallbacks.OnSuccess` / `OnFailure`, or take the outcome parameter.

## GuardOutcome

```csharp
public enum GuardOutcome
{
    Success,  // ThrowIf.*: the guard passed.     Is.*: the method returned true.
    Failure   // ThrowIf.*: the guard threw.      Is.*: the method returned false.
}
```

For `Is.*`, `Success` is not "the input is valid": `Is.NullOrEmpty(value)` reports `Success` when the value **is**
null or empty.

## SGuardCallbacks

Static factory class for creating callbacks.

### OnSuccess

Creates a callback that runs the action only for `GuardOutcome.Success`:

```csharp
public static SGuardCallback OnSuccess(Action action);
```

**Example:**
```csharp
var onSuccess = SGuardCallbacks.OnSuccess(() =>
    logger.LogInformation("Validation passed"));

ThrowIf.NullOrEmpty(value, onSuccess);
```

### OnFailure

Creates a callback that runs the action only for `GuardOutcome.Failure`:

```csharp
public static SGuardCallback OnFailure(Action action);
```

**Example:**
```csharp
var onFailure = SGuardCallbacks.OnFailure(() =>
    logger.LogError("Value out of range"));

bool inRange = Is.Between(value, min, max, onFailure);
```

## Combining Callbacks

Use the `+` operator to combine multiple callbacks:

```csharp
var onSuccess = SGuardCallbacks.OnSuccess(() => metrics.Increment("success"));
var onFailure = SGuardCallbacks.OnFailure(() => metrics.Increment("failure"));

SGuardCallback combined = onSuccess + onFailure;

ThrowIf.NullOrEmpty(value, combined);
```

## Callback Behavior

### With ThrowIf.*

- **Success** → the condition was false, no exception thrown
- **Failure** → the condition was true; the callback runs, then the exception propagates

```csharp
ThrowIf.LessThan(1, 2,
    SGuardCallbacks.OnFailure(() => Console.WriteLine("About to throw")));
// Output: "About to throw"
// Then throws LessThanException (1 < 2)
```

### With Is.*

- **Success** → the method returns `true`
- **Failure** → the method returns `false`

```csharp
bool result = Is.Between(5, 1, 10,
    SGuardCallbacks.OnSuccess(() => Console.WriteLine("In range")));
// Output: "In range"
// Returns: true
```

## Inline Callbacks

To access the outcome directly, pass a lambda that takes it:

```csharp
GuardOutcome? observed = null;

try
{
    ThrowIf.LessThan(1, 2, outcome => observed = outcome);
}
catch (LessThanException)
{
    // observed is Failure: the callback ran before the exception propagated
}
```

## Exception Handling

Exceptions thrown within callbacks are **swallowed** so they can't disrupt the guard:

```csharp
ThrowIf.NullOrEmpty(value, SGuardCallbacks.OnFailure(() =>
{
    throw new Exception("This won't propagate");
}));
// Callback exception is caught and ignored
// NullOrEmptyException is still thrown if value is null or empty
```

## Use Cases

### Logging

```csharp
var logFailure = SGuardCallbacks.OnFailure(() =>
    logger.LogWarning("Validation failed for {Parameter}", nameof(username)));

ThrowIf.NullOrEmpty(username, logFailure);
```

### Metrics

```csharp
var trackMetrics =
    SGuardCallbacks.OnSuccess(() => metrics.Increment("validation.success")) +
    SGuardCallbacks.OnFailure(() => metrics.Increment("validation.failure"));

bool isValid = Is.Between(value, min, max, trackMetrics);
```

### Auditing

```csharp
var audit = SGuardCallbacks.OnFailure(() =>
    auditService.RecordEvent(new ValidationFailureEvent
    {
        Timestamp = DateTime.UtcNow,
        Parameter = nameof(age)
    }));

ThrowIf.LessThan(age, 18, audit);
```

Record which argument failed rather than its value, unless you know the value can't be personal data. Exceptions
thrown by the callback are swallowed, so use this for best-effort records only; see
[Callback Exceptions Are Swallowed](#3-callback-exceptions-are-swallowed).

### Notifications

```csharp
var notify = SGuardCallbacks.OnFailure(() =>
    notificationService.Send("Validation failed"));

ThrowIf.Any(items, i => i.IsInvalid, notify);
```

### Debugging

```csharp
#if DEBUG
SGuardCallback? debug = SGuardCallbacks.OnFailure(() =>
    Debug.WriteLine($"Validation failed at {DateTime.Now}"));
#else
SGuardCallback? debug = null;
#endif

ThrowIf.NullOrEmpty(value, debug);
```

Declare the variable as `SGuardCallback`: a lambda assigned to `var` gets the natural type `Action<GuardOutcome>`,
which doesn't convert to `SGuardCallback`.

## Callback Order

When combining callbacks, they execute in the order they were added:

```csharp
var callback1 = SGuardCallbacks.OnFailure(() => Console.WriteLine("1"));
var callback2 = SGuardCallbacks.OnFailure(() => Console.WriteLine("2"));
var callback3 = SGuardCallbacks.OnFailure(() => Console.WriteLine("3"));

var combined = callback1 + callback2 + callback3;

ThrowIf.NullOrEmpty("", combined);
// Output:
// 1
// 2
// 3
// Then throws NullOrEmptyException
```

If one of the combined callbacks throws, the exception is swallowed and the callbacks after it don't run.

## Performance Impact

A callback is a plain delegate invocation, so the cost is that of your action. When no callback is passed, the guard
only checks it for `null`. Keep callbacks cheap in hot paths.

## Important Notes

### 1. Callbacks Run Before Exceptions Propagate

For `ThrowIf.*`, the failure callback runs **before** the exception reaches your `catch`:

```csharp
try
{
    ThrowIf.NullOrEmpty("", SGuardCallbacks.OnFailure(() =>
        Console.WriteLine("Callback runs first")));
}
catch (NullOrEmptyException)
{
    Console.WriteLine("Exception caught second");
}
// Output:
// Callback runs first
// Exception caught second
```

### 2. When the Callback Is Not Invoked

If a guard rejects its own arguments before evaluating the condition, the callback is **not invoked**. This covers:

- `null` operands, bounds, sources, predicates, selectors or exception instances (`ArgumentNullException`);
- reversed `Between` bounds (`ArgumentException`);
- `Is.Email` with a `null` or empty email or pattern, and a custom pattern that times out
  (`RegexMatchTimeoutException`).

```csharp
try
{
    ThrowIf.LessThan(age, 0, (ArgumentOutOfRangeException)null!, // null exception
        SGuardCallbacks.OnFailure(() => logger.LogError("Won't run")));
}
catch (ArgumentNullException)
{
    // Callback was NOT invoked
}
```

One case differs: with the `constructorArgs` overloads, the exception is created only when the guard fails. If no
constructor matches the arguments, the callback has already been invoked with `Failure`, and then an
`InvalidOperationException` is thrown instead of your exception type:

```csharp
try
{
    ThrowIf.GreaterThan<int, int, ArgumentOutOfRangeException>(quantity, 100,
        [42, 43, 44], // no matching constructor
        SGuardCallbacks.OnFailure(() => logger.LogError("Runs")));
}
catch (InvalidOperationException)
{
    // Callback WAS invoked with Failure (when quantity > 100)
}
```

### 3. Callback Exceptions Are Swallowed

Callback exceptions don't disrupt validation:

```csharp
ThrowIf.NullOrEmpty(value, SGuardCallbacks.OnFailure(() =>
{
    throw new Exception("Ignored");
}));
// The guard's own exception is still thrown normally
```

Don't use callbacks for records that must not be lost, such as audit or security logs: a failing writer is silently
ignored. See [Callback Exceptions Are Swallowed](../core-concepts/callbacks#callback-exceptions-are-swallowed).

## Best Practices

1. **Keep callbacks lightweight**: Avoid expensive operations
2. **Use for side effects only**: Don't put critical logic in callbacks
3. **Combine wisely**: Use `+` operator for clarity
4. **Mind the `Is.*` meaning**: `Success` means "returned true", not "input is valid"
5. **Don't rely on exceptions**: Callback exceptions are swallowed

## Next Steps

- [Core Concepts: Callbacks](../core-concepts/callbacks) - Detailed callback concepts
- [ThrowIf API](./throwif) - ThrowIf with callbacks
- [Is API](./is) - Is with callbacks
