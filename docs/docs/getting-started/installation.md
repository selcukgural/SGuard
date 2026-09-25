---
sidebar_position: 1
---

# Installation

Get started with SGuard in your .NET project.

## Requirements

- .NET 8, 9, or 10 (the package targets `net8.0`, `net9.0` and `net10.0`)
- An IDE or editor that supports your target framework (for example Visual Studio, Rider or VS Code)

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
