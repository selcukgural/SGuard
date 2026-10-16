---
sidebar_position: 3
title: Analyzers
description: The diagnostics SGuard reports while you write code, and how to configure them.
---

# Analyzers

From 3.0, the SGuard package includes Roslyn analyzers. They run in the IDE and during `dotnet build`, without any
setup, and point out guard calls that don't do what they look like they do.

| ID | Severity | What it finds |
|---|---|---|
| [SGUARD101](#SGUARD101) | Info | `NullOrEmpty` on a number or `bool`, where `0` or `false` would be rejected |
| [SGUARD102](#SGUARD102) | Info | An exception instance created on every call; has a code fix |
| [SGUARD103](#SGUARD103) | Warning | `Between` or `OutOfRange` with a constant minimum greater than the maximum |
| [SGUARD104](#SGUARD104) | Info | A `NullOrEmpty` selector that selects a whole object |

`SGUARD001` and `SGUARD002` are not analyzers but obsoletion warnings; see
[Obsolete APIs](../migration/upgrading-to-3.0#obsolete-apis).

## Configuring the rules

Change a rule's severity, or turn it off, in `.editorconfig`:

```ini
[*.cs]
dotnet_diagnostic.SGUARD101.severity = warning   # make it a build warning
dotnet_diagnostic.SGUARD104.severity = none      # turn it off
```

To turn off all of SGuard's analyzers, exclude them from the package reference:
`<PackageReference Include="SGuard" Version="3.0.0" ExcludeAssets="analyzers" />`.

## SGUARD101: NullOrEmpty rejects zero and false {/* #SGUARD101 */}

`NullOrEmpty` counts the default value of a value type as empty, so for a number it rejects `0`, and for a `bool` it
rejects `false`. That is rarely what a quantity, a discount or a flag needs.

```csharp
ThrowIf.NullOrEmpty(request.Quantity);  // SGUARD101: also throws for 0
ThrowIf.NullOrEmpty(request.Discount);  // SGUARD101 (decimal?): also throws for 0m
```

Say what is invalid instead:

```csharp
ThrowIf.Negative(request.Quantity);         // 0 is valid
ThrowIf.NegativeOrZero(request.Quantity);   // 0 is invalid, on purpose
var discount = ThrowIf.Null(request.Discount); // only a missing value is invalid
```

If rejecting `0` is what you mean, `ThrowIf.Zero` or `ThrowIf.NegativeOrZero` says so and clears the diagnostic.
`Guid`, dates, enums and other value types aren't reported: for them, the default value usually is "not set".

## SGUARD102: Pass a factory instead of an exception instance {/* #SGUARD102 */}

An exception passed to a guard is an argument, so it is created before the guard runs, on every call, also when the
guard passes:

```csharp
ThrowIf.GreaterThan(amount, balance, new InsufficientFundsException(accountId)); // SGUARD102
```

Pass a factory, which runs only when the guard fails:

```csharp
ThrowIf.GreaterThan(amount, balance, () => new InsufficientFundsException(accountId));
```

The code fix adds the `() =>`, and the call binds to the factory overload. It can fix every occurrence in a document,
project or solution at once.

## SGUARD103: The minimum is greater than the maximum {/* #SGUARD103 */}

`Between` and `OutOfRange` throw `ArgumentException` when `min` is greater than `max`. With constant bounds, that
happens on every call, so the guard can never pass or fail as intended:

```csharp
ThrowIf.OutOfRange(port, 65535, 1); // SGUARD103: the bounds are swapped
```

Swap the bounds: `ThrowIf.OutOfRange(port, 1, 65535)`. Only numeric constants are checked; string bounds depend on
the `StringComparison`.

## SGUARD104: The selector selects a whole object {/* #SGUARD104 */}

A `NullOrEmpty` selector that selects an object, rather than a value, checks all of the object's readable properties,
and fails only when **every** one of them is null or empty:

```csharp
ThrowIf.NullOrEmpty(command, c => c.ShippingAddress); // SGUARD104: passes when only City is missing
```

Guard each member you require:

```csharp
ThrowIf.NullOrEmpty(command, c => c.ShippingAddress!.City);
ThrowIf.NullOrEmpty(command, c => c.ShippingAddress!.PostalCode);
```

Collections, strings, value types and framework classes such as `Uri` aren't reported. If you only want to know that
the object exists, use `ThrowIf.Null(command.ShippingAddress)`.
