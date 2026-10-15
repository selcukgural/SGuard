using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace SGuard.Analyzers;

/// <summary>
/// Reports calls to SGuard guards that don't do what they look like they do. See <see cref="Diagnostics"/>.
/// </summary>
[DiagnosticAnalyzer(Microsoft.CodeAnalysis.LanguageNames.CSharp)]
public sealed class SGuardAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        Diagnostics.NullOrEmptyOnNumber,
        Diagnostics.ExceptionInstance,
        Diagnostics.ReversedBounds,
        Diagnostics.SelectorSelectsObject);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(start =>
        {
            var types = GuardTypes.Create(start.Compilation);

            if (types is not null)
            {
                start.RegisterOperationAction(c => AnalyzeInvocation(c, types), OperationKind.Invocation);
            }
        });
    }

    private static void AnalyzeInvocation(OperationAnalysisContext context, GuardTypes types)
    {
        var invocation = (IInvocationOperation)context.Operation;
        var method = invocation.TargetMethod;
        var guardClass = method.ContainingType;

        var isThrowIf = SymbolEqualityComparer.Default.Equals(guardClass, types.ThrowIf);

        if (!isThrowIf && !SymbolEqualityComparer.Default.Equals(guardClass, types.Is))
        {
            return;
        }

        var prefix = isThrowIf ? "ThrowIf" : "Is";

        switch (method.Name)
        {
            case "NullOrEmpty" when HasParameter(method, "selector"):
                CheckSelector(context, invocation);
                break;
            case "NullOrEmpty":
                CheckNullOrEmptyOnNumber(context, invocation, prefix);
                break;
            case "Between" or "OutOfRange":
                CheckBounds(context, invocation, prefix);
                break;
        }

        if (isThrowIf)
        {
            CheckExceptionInstance(context, invocation, types);
        }
    }

    // SGUARD101
    private static void CheckNullOrEmptyOnNumber(OperationAnalysisContext context, IInvocationOperation invocation, string prefix)
    {
        var argument = ArgumentFor(invocation, "value");

        // The type of the checked value as the method sees it: for the span overload that is ReadOnlySpan<T>, which is
        // only rejected when it has no elements.
        if (argument?.Parameter is null)
        {
            return;
        }

        var type = argument.Parameter.Type;
        var nullable = false;

        if (type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullableType)
        {
            type = nullableType.TypeArguments[0];
            nullable = true;
        }

        string emptyValue;
        string advice;

        if (type.SpecialType == SpecialType.System_Boolean)
        {
            emptyValue = "false";
            advice = nullable
                ? $"{prefix}.Null to reject only a missing value"
                : "no guard: a bool always has a value";
        }
        else if (IsNumber(type))
        {
            emptyValue = "0";
            advice = nullable
                ? $"{prefix}.Null to reject only a missing value"
                : $"{prefix}.Negative, or {prefix}.Zero or {prefix}.NegativeOrZero to reject 0 on purpose";
        }
        else
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Diagnostics.NullOrEmptyOnNumber, argument.Syntax.GetLocation(),
                                                   argument.Value.Syntax.ToString(), emptyValue, advice));
    }

    // SGUARD102
    private static void CheckExceptionInstance(OperationAnalysisContext context, IInvocationOperation invocation, GuardTypes types)
    {
        var argument = ArgumentFor(invocation, "exception");

        if (argument is null || Unwrap(argument.Value) is not IObjectCreationOperation { Type: { } exceptionType } ||
            !DerivesFrom(exceptionType, types.Exception))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Diagnostics.ExceptionInstance, argument.Syntax.GetLocation(), exceptionType.Name));
    }

    // SGUARD103
    private static void CheckBounds(OperationAnalysisContext context, IInvocationOperation invocation, string prefix)
    {
        var min = ArgumentFor(invocation, "min");
        var max = ArgumentFor(invocation, "max");

        if (min is null || max is null || !TryGetNumber(min.Value, out var minValue) || !TryGetNumber(max.Value, out var maxValue))
        {
            return;
        }

        if (minValue > maxValue)
        {
            context.ReportDiagnostic(Diagnostic.Create(Diagnostics.ReversedBounds, invocation.Syntax.GetLocation(),
                                                       min.Value.Syntax.ToString(), max.Value.Syntax.ToString(),
                                                       $"{prefix}.{invocation.TargetMethod.Name}"));
        }
    }

    // SGUARD104
    private static void CheckSelector(OperationAnalysisContext context, IInvocationOperation invocation)
    {
        var selector = ArgumentFor(invocation, "selector");

        if (selector?.Syntax is not ArgumentSyntax { Expression: LambdaExpressionSyntax { ExpressionBody: { } body } lambda })
        {
            return;
        }

        var model = invocation.SemanticModel;

        if (model?.GetTypeInfo(body, context.CancellationToken).Type is not INamedTypeSymbol type || !IsComplexType(type))
        {
            return;
        }

        var property = type.GetMembers()
                           .OfType<IPropertySymbol>()
                           .FirstOrDefault(p => p is { DeclaredAccessibility: Accessibility.Public, IsStatic: false, IsIndexer: false, GetMethod: not null });

        if (property is null)
        {
            return;
        }

        var parameter = lambda switch
        {
            SimpleLambdaExpressionSyntax simple => simple.Parameter.Identifier.Text,
            ParenthesizedLambdaExpressionSyntax { ParameterList.Parameters.Count: 1 } parenthesized =>
                parenthesized.ParameterList.Parameters[0].Identifier.Text,
            _ => null
        };

        if (parameter is null)
        {
            return;
        }

        var bodyText = body.ToString();
        var example = $"{parameter} => {bodyText}.{property.Name}";

        context.ReportDiagnostic(Diagnostic.Create(Diagnostics.SelectorSelectsObject, body.GetLocation(), bodyText, type.Name, example));
    }

    // What the selector builder inspects property by property: a class that isn't a string, a collection, a delegate or a
    // framework type.
    private static bool IsComplexType(INamedTypeSymbol type)
    {
        if (type.TypeKind != TypeKind.Class || type.SpecialType != SpecialType.None)
        {
            return false;
        }

        var ns = type.ContainingNamespace?.ToDisplayString();

        if (ns is not null && (ns == "System" || ns.StartsWith("System.", System.StringComparison.Ordinal)))
        {
            return false;
        }

        return !type.AllInterfaces.Any(i => i.SpecialType == SpecialType.System_Collections_IEnumerable ||
                                            i.OriginalDefinition.SpecialType == SpecialType.System_Collections_Generic_IEnumerable_T);
    }

    private static bool IsNumber(ITypeSymbol type)
    {
        switch (type.SpecialType)
        {
            case SpecialType.System_SByte:
            case SpecialType.System_Byte:
            case SpecialType.System_Int16:
            case SpecialType.System_UInt16:
            case SpecialType.System_Int32:
            case SpecialType.System_UInt32:
            case SpecialType.System_Int64:
            case SpecialType.System_UInt64:
            case SpecialType.System_IntPtr:
            case SpecialType.System_UIntPtr:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            case SpecialType.System_Decimal:
                return true;
        }

        // Numbers without a SpecialType.
        return type.ContainingNamespace?.ToDisplayString() is "System" or "System.Numerics" or "System.Runtime.InteropServices" &&
               type.Name is "Half" or "Int128" or "UInt128" or "BigInteger" or "NFloat";
    }

    private static bool TryGetNumber(IOperation operation, out double value)
    {
        value = 0;
        var constant = operation.ConstantValue;

        if (!constant.HasValue || constant.Value is null or string or bool or char)
        {
            return false;
        }

        try
        {
            value = System.Convert.ToDouble(constant.Value, CultureInfo.InvariantCulture);
            return !double.IsNaN(value);
        }
        catch (System.Exception)
        {
            return false;
        }
    }

    private static IArgumentOperation? ArgumentFor(IInvocationOperation invocation, string parameterName)
        => invocation.Arguments.FirstOrDefault(a => a.Parameter?.Name == parameterName && a.ArgumentKind != ArgumentKind.DefaultValue);

    private static bool HasParameter(IMethodSymbol method, string name) => method.Parameters.Any(p => p.Name == name);

    private static IOperation Unwrap(IOperation operation)
    {
        while (operation is IConversionOperation { IsImplicit: true } conversion)
        {
            operation = conversion.Operand;
        }

        return operation;
    }

    private static bool DerivesFrom(ITypeSymbol type, INamedTypeSymbol baseType)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType))
            {
                return true;
            }
        }

        return false;
    }

    private sealed class GuardTypes(INamedTypeSymbol throwIf, INamedTypeSymbol @is, INamedTypeSymbol exception)
    {
        public INamedTypeSymbol ThrowIf { get; } = throwIf;
        public INamedTypeSymbol Is { get; } = @is;
        public INamedTypeSymbol Exception { get; } = exception;

        public static GuardTypes? Create(Compilation compilation)
        {
            var throwIf = compilation.GetTypeByMetadataName("SGuard.ThrowIf");
            var @is = compilation.GetTypeByMetadataName("SGuard.Is");
            var exception = compilation.GetTypeByMetadataName("System.Exception");

            return throwIf is null || @is is null || exception is null ? null : new GuardTypes(throwIf, @is, exception);
        }
    }
}
