---
sidebar_position: 1
title: Upgrading to 3.0
description: What changes when you move from SGuard 0.x to 3.0, and what to do about it.
---

# Upgrading to SGuard 3.0

SGuard 3.0 adds guards, a factory for custom exceptions, return values and Native AOT support, and promises a stable
API from here on. Almost all of it is additive: **for most applications, upgrading means updating the package and
rebuilding, with no code changes.**

This guide lists every change that can affect existing code, how to tell whether it affects yours, and what to do.

## At a glance

| Change | Kind | What you do |
|---|---|---|
| [`ThrowIf.*` guards return the checked value](#return-values) | Binary breaking, source compatible | Rebuild. Libraries compiled against 0.x need a new build |
| [Comparison and range exceptions derive from `ArgumentOutOfRangeException`](#exception-base-types) | Behavior change | Check `catch (ArgumentOutOfRangeException)` blocks |
| [Constructor-argument overloads and `ExceptionActivator` are obsolete](#obsolete-apis) | Warning `SGUARD001`, `SGUARD002` | Pass a factory instead; they keep working until 4.0 |
| [`NullOrEmpty` selectors are marked as not trimming- or AOT-safe](#trimming-and-native-aot) | Warning `IL2026`, `IL3050` in trimmed and AOT apps only | Check the member directly |
| [Analyzers](../advanced/analyzers) SGUARD101–SGUARD104 | New suggestions (one warning) in the IDE and build | Review them; configure in `.editorconfig` |
| [New guards, factory overloads, nullable annotations](#adopting-the-new-apis) | Added | Optional |

Nothing else changed: the `Is.*` signatures, what counts as null or empty, exception messages and `ParamName`,
`SGuardOptions`, callbacks, and the supported target frameworks (.NET 8, 9 and 10) are the same as in 0.3.

## Upgrade in six steps

1. **Update the package.**

   ```bash
   dotnet add package SGuard --version 3.0.0
   ```

2. **Rebuild everything that uses SGuard**, including your own libraries that reference it. If a library you don't
   build yourself depends on SGuard 0.x, see [Libraries compiled against 0.x](#libraries-compiled-against-0x).

3. **Fix the `SGUARD001` and `SGUARD002` warnings**, if you get any. Each one links to [Obsolete APIs](#obsolete-apis),
   which shows the replacement. They are warnings, not errors, so this can wait.

4. **Search for `ArgumentOutOfRangeException`** in `catch` blocks, exception filters and exception-to-HTTP mappings,
   and check them against [the new exception base types](#exception-base-types).

5. **If you publish with trimming or Native AOT**, look for `IL2026` and `IL3050` warnings on `NullOrEmpty` selector
   calls; see [Trimming and Native AOT](#trimming-and-native-aot).

6. **Look at the new analyzer messages.** The package now includes [analyzers](../advanced/analyzers) that point out
   guard calls that don't do what they look like they do. Most are suggestions; SGUARD103 (bounds in the wrong order,
   which throws on every call) is a warning. The code fix for SGUARD102 turns exception instances into factories
   across a whole solution.

Then, when you touch the code anyway, [adopt the new APIs](#adopting-the-new-apis).

## Breaking changes

### `ThrowIf.*` guards return the checked value {/* #return-values */}

In 0.x every `ThrowIf.*` method returned `void`. In 3.0, `ThrowIf.NullOrEmpty`, `GreaterThan`, `GreaterThanOrEqual`,
`LessThan`, `LessThanOrEqual` and `Between` return the value they checked, as all the new guards do, so a guard can
initialize a field or property:

```csharp
public sealed class Product(string sku, decimal price)
{
    public string Sku { get; } = ThrowIf.NullOrEmpty(sku);
    public decimal Price { get; } = ThrowIf.NegativeOrZero(price);
}
```

The comparison guards return the left value, `Between` the value, and `NullOrEmpty` with a selector returns the root
object, not the selected member. `ThrowIf.Any`, `ThrowIf.All` and `Throw.*` still return `void`.

**Source compatibility.** Code that calls a guard as a statement compiles unchanged: C# discards the result.
`ThrowIf.NullOrEmpty(x);`, `() => ThrowIf.LessThan(a, b)` passed as an `Action`, and expression-bodied `void` members
all keep compiling. The full SGuard test suite of 0.3 compiles and passes against 3.0 without edits.

Two rare cases behave differently at compile time:

- A lambda that only calls a guard, passed to a method that has both an `Action` and a `Func<T>` overload, now binds to
  the `Func<T>` overload. xUnit's `Assert.Throws(() => ThrowIf...)` is one such method and works either way.
- Reflection that expects these methods to return `void` (`method.ReturnType == typeof(void)`) no longer matches.

**Binary compatibility.** The return type is part of a method's signature in compiled code. An assembly compiled
against SGuard 0.x that calls one of these methods fails with `MissingMethodException` when it runs against 3.0. If you
rebuild your application, there is nothing to do. This is the only binary breaking change in 3.0: comparing the 0.3.2
and 3.0 assemblies with [ApiCompat](https://learn.microsoft.com/dotnet/fundamentals/apicompat/overview) reports these
30 methods and nothing else.

#### Libraries compiled against 0.x

This matters when a package you depend on was itself built against SGuard 0.x and your application upgrades to 3.0:
NuGet then loads 3.0 for both, and the library's guard calls fail at run time.

- **If you maintain the library:** rebuild it against 3.0 and publish a new version with `SGuard >= 3.0.0` as the
  dependency. The source doesn't need to change.
- **If you don't:** stay on SGuard 0.3.2 until the library is updated, or ask its maintainers to rebuild it.

:::note Why not keep `void` and add new methods?
C# can't overload methods by return type, so returning values meant either new names for every guard or this change.
New names would have doubled the API, and the change keeps every existing call compiling, so 3.0, as a major version,
takes the binary break instead.
:::

### Comparison and range exceptions derive from `ArgumentOutOfRangeException` {/* #exception-base-types */}

These exceptions used to derive directly from `ArgumentException`:

| Exception | 0.x base type | 3.0 base type |
|---|---|---|
| `GreaterThanException` | `ArgumentException` | `ArgumentOutOfRangeException` |
| `GreaterThanOrEqualException` | `ArgumentException` | `ArgumentOutOfRangeException` |
| `LessThanException` | `ArgumentException` | `ArgumentOutOfRangeException` |
| `LessThanOrEqualException` | `ArgumentException` | `ArgumentOutOfRangeException` |
| `BetweenException` | `ArgumentException` | `ArgumentOutOfRangeException` |
| `NullOrEmptyException`, `AnyException`, `AllException` | `ArgumentException` | unchanged |

`ArgumentOutOfRangeException` derives from `ArgumentException`, so every `catch (ArgumentException)`, and the
[`IExceptionHandler` from the README](https://github.com/selcukgural/SGuard#web-api-turn-guard-exceptions-into-400-bad-request),
still catches them. Messages and `ParamName` are the same as before, and `ActualValue` is always `null`, so the checked
value doesn't reach your logs through it.

What changes is that code looking for `ArgumentOutOfRangeException` now sees these exceptions too:

- **`catch (ArgumentOutOfRangeException)`** now also catches guard failures. If such a block exists to handle a
  framework error (an index out of range, say) and a guard can fail inside the same `try`, the guard failure goes there
  now.
- **Catch blocks in the wrong order no longer compile.** A `catch (GreaterThanException)` placed *after*
  `catch (ArgumentOutOfRangeException)` is now unreachable, which is error CS0160. Move the more specific block first.
- **Exception filters and mappings**, such as `when (ex is ArgumentOutOfRangeException)` or a ProblemDetails mapping
  that gives `ArgumentOutOfRangeException` its own status code, now apply to these exceptions.
- The **parameterless constructors** (`new GreaterThanException()`) now have the default message of
  `ArgumentOutOfRangeException`, "Specified argument was out of the range of valid values.", instead of
  "Value does not fall within the expected range." The guards themselves never use these constructors.

The new guards follow the same rule: `NullException` derives from `ArgumentNullException`, `NullOrWhiteSpaceException`
from `ArgumentException`, and all the range, sign, equality, length and enum exceptions from
`ArgumentOutOfRangeException`.

## Obsolete APIs

These APIs still work in 3.0 and produce a warning. They will be removed in SGuard 4.0.

### Constructor-argument overloads (`SGUARD001`) {/* #SGUARD001 */}

The overloads that take `object[] constructorArgs` create the exception with reflection: the compiler can't check that
a matching constructor exists, a wrong argument list fails only when the guard fails, and trimming or Native AOT can
remove the constructor. Pass a factory instead. It runs only when the guard fails, and the compiler checks it:

```csharp
// Before: SGUARD001
ThrowIf.NullOrEmpty<string?, ArgumentException>(apiKey, ["An API key is required.", "apiKey"]);
ThrowIf.GreaterThan<int, int, RateLimitExceededException>(requests, limit, [clientId, limit]);

// After
ThrowIf.NullOrEmpty(apiKey, () => new ArgumentException("An API key is required.", nameof(apiKey)));
ThrowIf.GreaterThan(requests, limit, () => new RateLimitExceededException(clientId, limit));
```

Affected: the `constructorArgs` overloads of `NullOrEmpty` (with and without a selector), `GreaterThan`,
`GreaterThanOrEqual`, `LessThanOrEqual` and `Between` (generic and string).

The overloads that take a `TException` type with a parameterless constructor, such as
`ThrowIf.GreaterThan<int, int, RateLimitExceededException>(requests, limit)`, are not obsolete: `new()` needs no
reflection.

### `ExceptionActivator` (`SGUARD002`) {/* #SGUARD002 */}

`ExceptionActivator.Create<TException>(args)` is the reflection helper behind the constructor-argument overloads. Create
the exception directly (`new MyException(...)`), or give a guard a factory.

### Silencing the warnings

To upgrade first and migrate later, suppress the IDs project-wide:

```xml
<PropertyGroup>
  <NoWarn>$(NoWarn);SGUARD001;SGUARD002</NoWarn>
</PropertyGroup>
```

or around a single call with `#pragma warning disable SGUARD001` and `#pragma warning restore SGUARD001`.

## Trimming and Native AOT

SGuard 3.0 supports trimming and Native AOT, and CI checks this by publishing a test application with Native AOT and
running every guard in it. One feature can't be supported: **`NullOrEmpty` selectors** (`ThrowIf.NullOrEmpty(order,
o => o.Customer.Email)`) compile an expression tree at run time and read the properties of the objects they reach with
reflection. Under Native AOT they throw `InvalidOperationException`, and trimming can remove the properties they read.

The selector overloads are now marked `[RequiresUnreferencedCode]` and `[RequiresDynamicCode]`, so an app that
publishes with `PublishTrimmed` or `PublishAot` gets warning `IL2026` or `IL3050` where it calls one. Apps that don't
trim see no difference, and the selectors work there as before.

To make an AOT app safe, check the member directly. `?.` gives the same "null anywhere on the path counts as empty"
behavior, and the message names the whole expression:

```csharp
// Before: a selector, IL2026 and IL3050 under AOT
ThrowIf.NullOrEmpty(order, o => o.Customer!.Email);

// After: no reflection
ThrowIf.NullOrEmpty(order.Customer?.Email);
// NullOrEmptyException: Value 'order.Customer?.Email' is null or empty.
```

## Adopting the new APIs

None of this is required, but each replaces a pattern that 0.x users write by hand.

| In 0.x | In 3.0 |
|---|---|
| `ThrowIf.LessThan(port, 1); ThrowIf.GreaterThan(port, 65535);` | `ThrowIf.OutOfRange(port, 1, 65535)` |
| `if (!Is.Between(port, 1, 65535)) throw ...` | `ThrowIf.OutOfRange(port, 1, 65535)` |
| `ThrowIf.NullOrEmpty(name?.Trim())` | `ThrowIf.NullOrWhiteSpace(name)` |
| `ThrowIf.NullOrEmpty(customer)` when `0` or `""` would be valid | `ThrowIf.Null(customer)` |
| `ThrowIf.LessThan(amount, 0m)` | `ThrowIf.Negative(amount)` |
| `ThrowIf.LessThanOrEqual(quantity, 0)` | `ThrowIf.NegativeOrZero(quantity)` |
| `ThrowIf.GreaterThan(name.Length, 200)` | `ThrowIf.LongerThan(name, 200)` |
| `if (!Enum.IsDefined(status)) throw ...` | `ThrowIf.NotDefined(status)` |
| `if (source == target) throw ...` | `ThrowIf.Equal(target, source)` |
| `ThrowIf.GreaterThan(a, b, new DomainException(...))` | `ThrowIf.GreaterThan(a, b, () => new DomainException(...))` |
| `ThrowIf.NullOrEmpty(sku); Sku = sku;` | `Sku = ThrowIf.NullOrEmpty(sku);` |
| `int value = count!.Value;` after a null check | `int value = ThrowIf.Null(count);` |

The new `Is.*` validators cover input checks that used to need a regex or a helper:

| Check | 3.0 |
|---|---|
| Absolute `http`/`https` URL, safe to render as a link | `Is.Url(value)` |
| Payment card number format (Luhn) | `Is.CreditCard(value)` |
| Regular expression, with a match timeout | `Is.Match(value, pattern)` or `Is.Match(value, regex)` |

### Prefer factories over exception instances

An exception passed as an instance is created on every call, also when the guard passes:

```csharp
ThrowIf.GreaterThan(percent, 50m, new DomainException("Discounts above 50% need approval.")); // allocates every call
ThrowIf.GreaterThan(percent, 50m, () => new DomainException("Discounts above 50% need approval.")); // allocates nothing
```

A factory runs only when the guard fails. A lambda that doesn't capture variables is cached by the compiler and
allocates nothing; one that captures variables allocates a small closure per call, still far less than an exception.
The instance overloads aren't obsolete, so existing code keeps working.

### Nullable annotations

The guards now tell the compiler what they established. After `ThrowIf.NullOrEmpty(name)`, `ThrowIf.Null(name)` or
`ThrowIf.NullOrWhiteSpace(name)`, both `name` and the returned value are known not to be null, and `Is.Null`,
`Is.NullOrWhiteSpace` and the validators are annotated with `[NotNullWhen]`. If you added `!` after a guard to silence
a warning, you can remove it. A selector's parameter is no longer nullable when the root is, so
`ThrowIf.NullOrEmpty(maybeOrder, o => o.Reference)` doesn't warn inside the lambda.

## Questions

**Why 3.0 and not 1.0?**
The SGuard package ID was used before for an earlier, GPL-licensed line of the library, whose versions 1.0.0 to 2.1.0
are still on nuget.org (unlisted). NuGet never lets a version number be reused, so the release after 0.3.2 is 3.0.0.
Don't install a 1.x or 2.x version: they are a different code base and license.

**Why isn't `ThrowIf.Between` obsolete, given that it throws when the value is inside the range?**
Because that reading is consistent ("throw if the value is between"), and code that uses it on purpose is correct.
`ThrowIf.OutOfRange` is the guard most code wants; prefer it for range checks.

**Why do `ThrowIf.Any` and `ThrowIf.All` still return `void`?**
Returning the source would make it an `IEnumerable<T>`, losing its concrete type (`List<T>`, an array), and C# can't
infer the element type from a generic source type. Guard the collection with `ThrowIf.NullOrEmpty` when you want it
back.

**Does `Is.Email` now return `false` for `null`, like the new validators?**
No. To stay compatible, `Is.Email` still throws `ArgumentNullException` for `null` and `ArgumentException` for `""`. The
new `Is.Url`, `Is.CreditCard` and `Is.Match` return `false` for `null`, because a missing value is invalid input, not a
bug.

**What does "stable API" mean from 3.0?**
SGuard follows [Semantic Versioning](https://semver.org/): 3.x releases add APIs and fix bugs but don't remove or
change existing public APIs. Anything that is going to be removed is marked `[Obsolete]` with a diagnostic ID and a
link to this site at least one minor release before the next major version.
