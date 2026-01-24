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

Callbacks receive a `GuardOutcome` parameter with two possible values:

- **`Success`**: The validation passed
- **`Failure`**: The validation failed

## Callback Behavior

### ThrowIf.* Methods

- **Outcome = Failure** → The guard is about to throw (callback runs just before the exception propagates)
- **Outcome = Success** → The guard passes (no exception is thrown)

```csharp
// Failure → throws → OnFailure runs
ThrowIf.LessThan(1, 2, SGuardCallbacks.OnFailure(() => 
    logger.LogWarning("a < b failed")));

// Success → no throw → OnSuccess runs
ThrowIf.LessThan(5, 2, SGuardCallbacks.OnSuccess(() => 
    logger.LogInformation("a >= b OK")));
```

### Is.* Methods

- **Outcome = Success** when the result is `true`
- **Outcome = Failure** when the result is `false`

```csharp
// True → OnSuccess runs
bool inRange = Is.Between(5, 1, 10, SGuardCallbacks.OnSuccess(() => 
    metrics.Increment("is.between.true")));

// False → OnFailure runs
bool isLess = Is.LessThan(5, 2, SGuardCallbacks.OnFailure(() => 
    metrics.Increment("is.lt.false")));
```

## Creating Callbacks

### OnSuccess

Runs when the validation succeeds:

```csharp
var onSuccess = SGuardCallbacks.OnSuccess(() => 
    audit.Record("Validation passed"));

ThrowIf.NullOrEmpty(email, onSuccess);
```

### OnFailure

Runs when the validation fails:

```csharp
var onFailure = SGuardCallbacks.OnFailure(() => 
    notifier.Notify("Validation failed"));

bool isValid = Is.Between(value, min, max, onFailure);
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

For simple cases, use inline callbacks to access the outcome directly:

```csharp
GuardOutcome? observed = null;

ThrowIf.LessThan(1, 2, outcome => observed = outcome);
```

## Important Notes

### Callbacks Are Swallowed

Exceptions thrown within callbacks are **safely swallowed** and do not disrupt the validation flow:

```csharp
ThrowIf.NullOrEmpty(value, SGuardCallbacks.OnFailure(() => 
{
    throw new Exception("This won't propagate");
}));
```

This ensures your validation logic remains robust even if side effects fail.

### Callbacks Not Invoked on Invalid Arguments

If the API fails due to invalid arguments (e.g., null selector or null exception instance), the callback is **NOT invoked**:

```csharp
try
{
    // Passing null exception causes ArgumentNullException
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
bool isValid = Is.Between(
    value, 
    min, 
    max, 
    SGuardCallbacks.OnSuccess(() => metrics.Increment("validation.success"))
    + SGuardCallbacks.OnFailure(() => metrics.Increment("validation.failure"))
);
```

## Next Steps

- [Custom Exceptions](./custom-exceptions) - Use your own exception types
- [Real-World Examples](../guides/real-world-examples) - See callbacks in action
- [API Reference](../api/callbacks) - Complete callback API
