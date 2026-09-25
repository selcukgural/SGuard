---
sidebar_position: 2
---

# Callbacks

Learn how to add side effects to guard validations using the unified callback model.

## Overview

SGuard provides a unified callback model that works consistently across both `ThrowIf.*` and `Is.*` APIs. Callbacks allow you to:

- Log validation failures or successes
- Increment metrics and counters
- Perform cleanup operations
- Audit validation events

## GuardOutcome

Callbacks receive a `GuardOutcome` parameter with two possible values: `Success` and `Failure`. What they mean
depends on the API:

| API | `Success` | `Failure` |
|---|---|---|
| `ThrowIf.*` | The guard passed (nothing was thrown) | The guard is about to throw |
| `Is.*` | The method returned `true` | The method returned `false` |

For `Is.*`, the outcome reports the **return value**, not whether the input is "valid". Many `Is.*` checks
return `true` for *bad* input: for a `string? email = null`, `Is.NullOrEmpty(email)` returns `true` and therefore
reports `Success`.

## Callback Behavior

### ThrowIf.* Methods

- **Outcome = Failure** → The guard is about to throw (callback runs just before the exception propagates)
- **Outcome = Success** → The guard passes (no exception is thrown)

```csharp
// 1 < 2 → throws LessThanException; OnFailure runs first
ThrowIf.LessThan(1, 2, SGuardCallbacks.OnFailure(() => 
    logger.LogWarning("a < b failed")));

// 5 >= 2 → no throw → OnSuccess runs
ThrowIf.LessThan(5, 2, SGuardCallbacks.OnSuccess(() => 
    logger.LogInformation("a >= b OK")));
```

### Is.* Methods

- **Outcome = Success** when the method returns `true`
- **Outcome = Failure** when the method returns `false`

```csharp
// Returns true → OnSuccess runs
bool inRange = Is.Between(5, 1, 10, SGuardCallbacks.OnSuccess(() => 
    metrics.Increment("is.between.true")));

// Returns false → OnFailure runs
bool isLess = Is.LessThan(5, 2, SGuardCallbacks.OnFailure(() => 
    metrics.Increment("is.lt.false")));
```

Pick the callback by the return value you care about. To log a missing email with `Is.NullOrEmpty`, use
`OnSuccess`, because the method returns `true` when the email is missing:

```csharp
// Wrong: for a null or empty email, Is.NullOrEmpty returns true → Success, so this never logs
Is.NullOrEmpty(email, SGuardCallbacks.OnFailure(() => logger.LogWarning("Missing email")));

// Right: runs when the method returns true, i.e. when the email is null or empty
Is.NullOrEmpty(email, SGuardCallbacks.OnSuccess(() => logger.LogWarning("Missing email")));
```

The throwing counterpart reads the other way round: `ThrowIf.NullOrEmpty(email, callback)` with a null or empty
email reports `Failure` and throws.

## Creating Callbacks

### OnSuccess

Runs when the outcome is `Success` (a `ThrowIf.*` guard passed, or an `Is.*` method returned `true`):

```csharp
var onSuccess = SGuardCallbacks.OnSuccess(() => 
    audit.Record("Validation passed"));

ThrowIf.NullOrEmpty(email, onSuccess);
```

### OnFailure

Runs when the outcome is `Failure` (a `ThrowIf.*` guard is about to throw, or an `Is.*` method returned `false`):

```csharp
var onFailure = SGuardCallbacks.OnFailure(() => 
    notifier.Notify("Value is out of range"));

// Is.Between returns false (→ Failure) when value is outside [min, max]
bool inRange = Is.Between(value, min, max, onFailure);
```

### Combining Callbacks

You can combine Success and Failure callbacks using the `+` operator:

```csharp
var onFailure = SGuardCallbacks.OnFailure(() => 
    logger.LogError("Failed"));
var onSuccess = SGuardCallbacks.OnSuccess(() => 
    logger.LogInformation("Passed"));

SGuardCallback combined = onFailure + onSuccess;

ThrowIf.Between(value, min, max, combined);
```

### Inline Callbacks

A lambda that takes the outcome is an `SGuardCallback` too, so you can inspect the outcome directly
(a parameterless lambda is not; wrap it with `SGuardCallbacks.OnSuccess`/`OnFailure`):

```csharp
GuardOutcome? observed = null;

bool isLess = Is.LessThan(1, 2, outcome => observed = outcome);
// isLess == true, observed == GuardOutcome.Success
```

## Important Notes

### Callback Exceptions Are Swallowed

Exceptions thrown within callbacks are **swallowed** and do not change the guard's result:

```csharp
// value is null or empty: the callback's exception is ignored,
// and the guard still throws its NullOrEmptyException
ThrowIf.NullOrEmpty(value, SGuardCallbacks.OnFailure(() => 
{
    throw new Exception("This won't propagate");
}));
```

This ensures your validation logic remains robust even if side effects fail.

### When the Callback Is Not Invoked

If the method throws because of an invalid **argument**, it throws before evaluating the guard and the callback is
**not invoked**. This covers null operands, predicates, sources or exception instances (`ArgumentNullException`),
reversed `Between` bounds (`ArgumentException`), `Is.Email(null)` or `Is.Email("")` (`ArgumentException`), and a
custom `Is.Email` pattern that times out (`RegexMatchTimeoutException`). An exception thrown by your own predicate
in `Any`/`All` also skips the callback.

```csharp
try
{
    // Passing a null exception instance causes ArgumentNullException
    ThrowIf.Between<int, int, int, InvalidOperationException>(
        5, 1, 10,
        (InvalidOperationException)null!, 
        SGuardCallbacks.OnFailure(() => logger.LogError("won't run")));
}
catch (ArgumentNullException)
{
    // Callback was not invoked
}
```

One exception to this rule: with the `constructorArgs` overloads, the exception is created only when the guard
fails. If no constructor of `TException` matches the arguments, the callback has already been invoked with
`Failure` and the guard then throws `InvalidOperationException` instead of your exception:

```csharp
// ArgumentOutOfRangeException has no (int) constructor
ThrowIf.GreaterThan<int, int, ArgumentOutOfRangeException>(
    5, 1, 
    new object[] { 42 }, 
    SGuardCallbacks.OnFailure(() => logger.LogError("runs")));
// OnFailure runs, then InvalidOperationException ("No matching constructor found ...") is thrown
```

## Real-World Examples

### Logging Validation Failures

```csharp
public void SaveUser(string username)
{
    var callback = SGuardCallbacks.OnFailure(() =>
        logger.LogWarning("Validation failed: username is required"));

    ThrowIf.NullOrEmpty(username, callback);
    
    // Proceed with saving...
}
```

### Audit Successful Validations

```csharp
public void UpdateEmail(string email)
{
    var onSuccess = SGuardCallbacks.OnSuccess(() =>
        audit.Record("Email validation succeeded"));

    ThrowIf.NullOrEmpty(email, onSuccess);
    
    // Proceed with update...
}
```

### Metrics and Monitoring

```csharp
// Success = Is.Between returned true (in range), Failure = it returned false
bool inRange = Is.Between(
    value, 
    min, 
    max, 
    SGuardCallbacks.OnSuccess(() => metrics.Increment("value.in_range"))
    + SGuardCallbacks.OnFailure(() => metrics.Increment("value.out_of_range"))
);
```

## Next Steps

- [Custom Exceptions](./custom-exceptions) - Use your own exception types
- [Real-World Examples](../guides/real-world-examples) - See callbacks in action
- [API Reference](../api/callbacks) - Complete callback API
