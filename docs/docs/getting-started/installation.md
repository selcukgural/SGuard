---
sidebar_position: 1
---

# Installation

Get started with SGuard in your .NET project.

## Requirements

- .NET 6, 7, 8, or 9
- Visual Studio 2022+ or any compatible IDE

## Install via NuGet

### Using .NET CLI

```bash
dotnet add package SGuard
```

### Using Package Manager Console

```powershell
Install-Package SGuard
```

### Using Visual Studio

1. Right-click on your project in Solution Explorer
2. Select "Manage NuGet Packages"
3. Search for "SGuard"
4. Click "Install"

## Verify Installation

After installation, verify that SGuard is available by adding this using statement to your code:

```csharp
using SGuard;
```

You should now have access to `ThrowIf` and `Is` guard methods.

## Next Steps

- [Quick Start](./quick-start) - Learn the basics with simple examples
- [Why SGuard?](./why-sguard) - Understand what makes SGuard different
- [Guard Methods](../core-concepts/guard-methods) - Explore available guard methods
