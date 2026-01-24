---
sidebar_position: 3
---

# Callbacks API

Complete reference for the callback system.

## Overview

SGuard provides a unified callback model for adding side effects to guard validations. Callbacks work consistently across both `ThrowIf.*` and `Is.*` APIs.

## SGuardCallback

Delegate type for callbacks:

```csharp
public delegate void SGuardCallback(GuardOutcome outcome);
```

Receives a `GuardOutcome` parameter indicating success or failure.

## GuardOutcome

Enum representing the validation result:

```csharp
public enum GuardOutcome
{
    Success,  // Validation passed
    Failure   // Validation failed
}
```

## SGuardCallbacks

Static factory class for creating callbacks.

### OnSuccess

Creates a callback that runs only on success:

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

Creates a callback that runs only on failure:

```csharp
public static SGuardCallback OnFailure(Action action);
```

**Example:**
```csharp
var onFailure = SGuardCallbacks.OnFailure(() => 
    logger.LogError("Validation failed"));

bool isValid = Is.Between(value, min, max, onFailure);
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

- **Success** → Guard passes, no exception thrown
- **Failure** → Guard fails, callback runs just before exception is thrown

```csharp
ThrowIf.LessThan(1, 2, 
    SGuardCallbacks.OnFailure(() => Console.WriteLine("About to throw")));
// Output: "About to throw"
// Then throws exception
```

### With Is.*

- **Success** → Method returns `true`
- **Failure** → Method returns `false`

```csharp
bool result = Is.Between(5, 1, 10, 
    SGuardCallbacks.OnSuccess(() => Console.WriteLine("In range")));
// Output: "In range"
// Returns: true
```

## Inline Callbacks

For advanced scenarios, create callbacks inline to access the outcome directly:

```csharp
GuardOutcome? observed = null;

ThrowIf.LessThan(1, 2, outcome => observed = outcome);

// observed is now Failure (before exception is thrown)
```

## Exception Handling

Exceptions thrown within callbacks are **safely swallowed** to prevent disrupting validation flow:

```csharp
ThrowIf.NullOrEmpty(value, SGuardCallbacks.OnFailure(() => 
{
    throw new Exception("This won't propagate");
}));
// Callback exception is caught and ignored
// Validation exception is still thrown
```

## Use Cases

### Logging

```csharp
var logFailure = SGuardCallbacks.OnFailure(() => 
    logger.LogWarning("Validation failed for {Parameter}", paramName));

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
        Parameter = paramName,
        Value = value
    }));

ThrowIf.LessThan(age, 18, audit);
```

### Notifications

```csharp
var notify = SGuardCallbacks.OnFailure(() => 
    notificationService.Send("Validation failed"));

ThrowIf.Any(items, i => i.IsInvalid, notify);
```

### Debugging

```csharp
#if DEBUG
var debug = SGuardCallbacks.OnFailure(() => 
    Debug.WriteLine($"Validation failed at {DateTime.Now}"));
#else
var debug = (GuardOutcome _) => { };
#endif

ThrowIf.NullOrEmpty(value, debug);
```

## Callback Order

When combining callbacks, they execute in the order they were added:

```csharp
var callback1 = SGuardCallbacks.OnFailure(() => Console.WriteLine("1"));
var callback2 = SGuardCallbacks.OnFailure(() => Console.WriteLine("2"));
var callback3 = SGuardCallbacks.OnFailure(() => Console.WriteLine("3"));

var combined = callback1 + callback2 + callback3;

ThrowIf.NullOrEmpty(null, combined);
// Output:
// 1
// 2
// 3
// Then throws
```

## Performance Impact

Callbacks add minimal overhead:
- **No callback**: ~5-10ns
- **With callback**: ~10-20ns (simple action)
- **Complex callback**: Depends on action logic

The overhead is negligible for most applications. Only avoid callbacks in extremely hot loops if profiling shows an issue.

## Important Notes

### 1. Callbacks Run Before Exceptions

For `ThrowIf.*`, failure callbacks run **just before** the exception is thrown:

```csharp
try
{
    ThrowIf.NullOrEmpty(null, SGuardCallbacks.OnFailure(() => 
        Console.WriteLine("Callback runs first")));
}
catch
{
    Console.WriteLine("Exception caught second");
}
// Output:
// Callback runs first
// Exception caught second
```

### 2. Callbacks Not Invoked on Invalid Arguments

If the guard method itself receives invalid arguments, callbacks are **not invoked**:

```csharp
try
{
    ThrowIf.NullOrEmpty<string, Exception>(value, null!, // null exception
        SGuardCallbacks.OnFailure(() => logger.Log("Won't run")));
}
catch (ArgumentNullException)
{
    // Callback was NOT invoked
}
```

### 3. Callbacks Are Swallowed

Callback exceptions don't disrupt validation:

```csharp
ThrowIf.NullOrEmpty(value, SGuardCallbacks.OnFailure(() => 
{
    throw new Exception("Ignored");
}));
// Validation exception is still thrown normally
```

## Best Practices

1. **Keep callbacks lightweight**: Avoid expensive operations
2. **Use for side effects only**: Don't put critical logic in callbacks
3. **Combine wisely**: Use `+` operator for clarity
4. **Consider performance**: In hot paths, callbacks add small overhead
5. **Don't rely on exceptions**: Callback exceptions are swallowed

## Next Steps

- [Core Concepts: Callbacks](../core-concepts/callbacks) - Detailed callback concepts
- [ThrowIf API](./throwif) - ThrowIf with callbacks
- [Is API](./is) - Is with callbacks
