using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace SGuard;

/// <summary>
/// Compiles selector expressions rewritten by <see cref="NullOrEmptyVisitor"/> and caches the resulting delegates
/// by expression structure.
/// </summary>
/// <remarks>
/// The C# compiler creates a new expression tree every time a lambda is evaluated, so the tree instance cannot be used
/// as a cache key. Selectors are keyed by their shape instead: parameters, member accesses, conversions, method calls and
/// primitive constants. Any other node, such as a captured variable (closure), makes the selector uncacheable, because
/// its compiled delegate would embed a value that differs between calls with the same shape. Uncacheable selectors are
/// compiled on every call.
/// </remarks>
internal static class SelectorCache
{
    /// <summary>
    /// The maximum number of cached delegates per selector input type. Keys are structural, so the number of entries is
    /// bounded by the number of distinct selectors in the calling code; the cap only guards against pathological callers.
    /// </summary>
    internal const int MaxEntriesPerType = 1000;

    private static readonly NullOrEmptyVisitor Visitor = new();

    /// <summary>
    /// Returns a delegate that evaluates the selector and yields <c>null</c> when the selected member (or any member on
    /// the path to it) is null or empty; otherwise, the selected value.
    /// </summary>
    /// <typeparam name="T">The selector input type.</typeparam>
    /// <param name="selector">The selector expression, typed as <c>Func&lt;T, object&gt;</c>.</param>
    /// <returns>The compiled evaluator, or <c>null</c> if the selector could not be rewritten.</returns>
    public static Func<T, object?>? GetNullOrEmptyEvaluator<T>(LambdaExpression selector)
    {
        if (!SelectorShapeComparer.IsCacheable(selector))
        {
            return Compile<T>(selector);
        }

        var cache = Cache<T>.Entries;

        if (cache.TryGetValue(selector, out var cached))
        {
            return cached;
        }

        var evaluator = Compile<T>(selector);

        if (evaluator is not null && cache.Count < MaxEntriesPerType)
        {
            cache.TryAdd(selector, evaluator);
        }

        return evaluator;
    }

    /// <summary>
    /// Gets the number of cached evaluators for the specified input type.
    /// </summary>
    internal static int Count<T>() => Cache<T>.Entries.Count;

    private static Func<T, object?>? Compile<T>(LambdaExpression selector)
        => Visitor.Visit(selector) is Expression<Func<T, object?>> rewritten ? rewritten.Compile() : null;

    private static class Cache<T>
    {
        public static readonly ConcurrentDictionary<LambdaExpression, Func<T, object?>> Entries = new(SelectorShapeComparer.Instance);
    }
}

/// <summary>
/// Compares selector expressions by structure rather than by reference.
/// </summary>
internal sealed class SelectorShapeComparer : IEqualityComparer<LambdaExpression>
{
    public static readonly SelectorShapeComparer Instance = new();

    /// <summary>
    /// Determines whether the selector consists only of nodes whose meaning is fully described by the structure this
    /// comparer compares.
    /// </summary>
    public static bool IsCacheable(LambdaExpression lambda) => IsCacheable(lambda.Body, lambda.Parameters);

    public bool Equals(LambdaExpression? x, LambdaExpression? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null || x.Type != y.Type || x.Parameters.Count != y.Parameters.Count)
        {
            return false;
        }

        return AreEqual(x.Body, y.Body, x.Parameters, y.Parameters);
    }

    public int GetHashCode(LambdaExpression obj)
    {
        var hash = new HashCode();
        hash.Add(obj.Type);
        AddHash(ref hash, obj.Body, obj.Parameters);
        return hash.ToHashCode();
    }

    private static bool IsCacheable(Expression? node, IReadOnlyList<ParameterExpression> parameters)
    {
        switch (node)
        {
            case null:
                return true;
            case ParameterExpression parameter:
                return IndexOf(parameters, parameter) >= 0;
            case MemberExpression member:
                return IsCacheable(member.Expression, parameters);
            case UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked or ExpressionType.TypeAs or ExpressionType.ArrayLength } unary:
                return IsCacheable(unary.Operand, parameters);
            case BinaryExpression { NodeType: ExpressionType.ArrayIndex } binary:
                return IsCacheable(binary.Left, parameters) && IsCacheable(binary.Right, parameters);
            case MethodCallExpression call:
                if (!IsCacheable(call.Object, parameters))
                {
                    return false;
                }

                foreach (var argument in call.Arguments)
                {
                    if (!IsCacheable(argument, parameters))
                    {
                        return false;
                    }
                }

                return true;
            case ConstantExpression constant:
                return constant.Value is null || IsPrimitiveConstant(constant.Type);
            default:
                return false;
        }
    }

    private static bool IsPrimitiveConstant(Type type)
        => type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(decimal);

    private static bool AreEqual(Expression? x, Expression? y, IReadOnlyList<ParameterExpression> xParameters,
                                 IReadOnlyList<ParameterExpression> yParameters)
    {
        if (x is null || y is null)
        {
            return x is null && y is null;
        }

        if (x.NodeType != y.NodeType || x.Type != y.Type)
        {
            return false;
        }

        switch (x)
        {
            case ParameterExpression xParameter:
                return IndexOf(xParameters, xParameter) == IndexOf(yParameters, (ParameterExpression)y);
            case MemberExpression xMember:
                var yMember = (MemberExpression)y;
                return MembersEqual(xMember.Member, yMember.Member) &&
                       AreEqual(xMember.Expression, yMember.Expression, xParameters, yParameters);
            case UnaryExpression xUnary:
                var yUnary = (UnaryExpression)y;
                return xUnary.Method == yUnary.Method && AreEqual(xUnary.Operand, yUnary.Operand, xParameters, yParameters);
            case BinaryExpression xBinary:
                var yBinary = (BinaryExpression)y;
                return xBinary.Method == yBinary.Method &&
                       AreEqual(xBinary.Left, yBinary.Left, xParameters, yParameters) &&
                       AreEqual(xBinary.Right, yBinary.Right, xParameters, yParameters);
            case MethodCallExpression xCall:
                var yCall = (MethodCallExpression)y;

                if (xCall.Method != yCall.Method || xCall.Arguments.Count != yCall.Arguments.Count ||
                    !AreEqual(xCall.Object, yCall.Object, xParameters, yParameters))
                {
                    return false;
                }

                for (var i = 0; i < xCall.Arguments.Count; i++)
                {
                    if (!AreEqual(xCall.Arguments[i], yCall.Arguments[i], xParameters, yParameters))
                    {
                        return false;
                    }
                }

                return true;
            case ConstantExpression xConstant:
                return Equals(xConstant.Value, ((ConstantExpression)y).Value);
            default:
                // Only reachable for uncacheable selectors, which are never stored.
                return false;
        }
    }

    private static void AddHash(ref HashCode hash, Expression? node, IReadOnlyList<ParameterExpression> parameters)
    {
        if (node is null)
        {
            hash.Add(0);
            return;
        }

        hash.Add(node.NodeType);
        hash.Add(node.Type);

        switch (node)
        {
            case ParameterExpression parameter:
                hash.Add(IndexOf(parameters, parameter));
                break;
            case MemberExpression member:
                hash.Add(member.Member.MetadataToken);
                AddHash(ref hash, member.Expression, parameters);
                break;
            case UnaryExpression unary:
                AddHash(ref hash, unary.Operand, parameters);
                break;
            case BinaryExpression binary:
                AddHash(ref hash, binary.Left, parameters);
                AddHash(ref hash, binary.Right, parameters);
                break;
            case MethodCallExpression call:
                hash.Add(call.Method.MetadataToken);
                AddHash(ref hash, call.Object, parameters);

                foreach (var argument in call.Arguments)
                {
                    AddHash(ref hash, argument, parameters);
                }

                break;
            case ConstantExpression constant:
                hash.Add(constant.Value);
                break;
            default:
                // Only reachable for uncacheable selectors, which are never stored.
                break;
        }
    }

    private static bool MembersEqual(MemberInfo x, MemberInfo y)
        => x == y || (x.MetadataToken == y.MetadataToken && x.Module == y.Module && x.DeclaringType == y.DeclaringType);

    private static int IndexOf(IReadOnlyList<ParameterExpression> parameters, ParameterExpression parameter)
    {
        for (var i = 0; i < parameters.Count; i++)
        {
            if (ReferenceEquals(parameters[i], parameter))
            {
                return i;
            }
        }

        return -1;
    }
}
