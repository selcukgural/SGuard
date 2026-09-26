using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace SGuard;

/// <summary>
/// Compiles <c>NullOrEmpty</c> selectors with <see cref="NullOrEmptySelectorBuilder"/> and caches the resulting delegates
/// by expression structure.
/// </summary>
/// <remarks>
/// The C# compiler creates a new expression tree every time a lambda is evaluated, so the tree instance cannot be used
/// as a cache key. Selectors are keyed by their shape instead: parameters, member accesses, method calls, operators,
/// conditionals, <c>new</c> expressions and constants. Captured variables reach the tree as constants holding the
/// closure object; those are compiled as reads from an array argument and keyed by their type, so the delegate is shared
/// and each call passes its own closure. Primitive constants (such as an index) stay in the compiled code and are keyed
/// by value. A selector containing a nested lambda, a block, an invocation or a member or list initializer is not
/// cacheable and is compiled on every call.
/// </remarks>
internal static class SelectorCache
{
    /// <summary>
    /// The maximum number of cached delegates per selector input type. Keys are structural, so the number of entries is
    /// bounded by the number of distinct selectors in the calling code; the cap only guards against pathological callers.
    /// </summary>
    internal const int MaxEntriesPerType = 1000;

    /// <summary>
    /// Evaluates the selector and determines whether the selected member (or any member on the path to it) is null or
    /// empty.
    /// </summary>
    /// <typeparam name="T">The selector input type.</typeparam>
    /// <param name="value">The value the selector is applied to.</param>
    /// <param name="selector">The selector expression, typed as <c>Func&lt;T, object&gt;</c>.</param>
    public static bool IsNullOrEmpty<T>(T value, LambdaExpression selector)
    {
        if (!SelectorShapeComparer.IsCacheable(selector))
        {
            return Is.InternalIsNullOrEmpty(Compile<T>(selector, parameterizeConstants: false)(value, []));
        }

        var cache = Cache<T>.Entries;

        if (!cache.TryGetValue(selector, out var evaluator))
        {
            evaluator = Compile<T>(selector, parameterizeConstants: true);

            if (cache.Count < MaxEntriesPerType)
            {
                cache.TryAdd(selector, evaluator);
            }
        }

        return Is.InternalIsNullOrEmpty(evaluator(value, SelectorShapeComparer.CollectConstants(selector)));
    }

    /// <summary>
    /// Gets the number of cached evaluators for the specified input type.
    /// </summary>
    internal static int Count<T>() => Cache<T>.Entries.Count;

    private static Evaluator<T> Compile<T>(LambdaExpression selector, bool parameterizeConstants)
    {
        var constants = Expression.Parameter(typeof(object[]), "constants");
        var body = parameterizeConstants ? new ConstantParameterizer(constants).Visit(selector.Body) : selector.Body;

        return Expression.Lambda<Evaluator<T>>(NullOrEmptySelectorBuilder.Build(body), selector.Parameters[0], constants).Compile();
    }

    /// <summary>
    /// A compiled selector: yields <c>null</c> when the selected value is null or empty; otherwise, the value.
    /// </summary>
    private delegate object? Evaluator<in T>(T value, object?[] constants);

    private static class Cache<T>
    {
        public static readonly ConcurrentDictionary<LambdaExpression, Evaluator<T>> Entries = new(SelectorShapeComparer.Instance);
    }

    /// <summary>
    /// Replaces each parameterized constant with a read from the constants array, numbering them in the order
    /// <see cref="SelectorShapeComparer.CollectConstants"/> collects them.
    /// </summary>
    private sealed class ConstantParameterizer(ParameterExpression constants) : ExpressionVisitor
    {
        private int _next;

        protected override Expression VisitConstant(ConstantExpression node)
            => SelectorShapeComparer.IsParameterized(node)
                   ? Expression.Convert(Expression.ArrayIndex(constants, Expression.Constant(_next++)), node.Type)
                   : node;
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
        if (node is null)
        {
            return true;
        }

        if (!IsSupported(node) || (node is ParameterExpression parameter && IndexOf(parameters, parameter) < 0))
        {
            return false;
        }

        for (var i = 0; i < ChildCount(node); i++)
        {
            if (!IsCacheable(Child(node, i), parameters))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Determines whether the node's meaning is fully described by what this comparer compares: its node type, type,
    /// the member, method, constructor or type operand it refers to, and its children. Lambdas, blocks, invocations and
    /// member or list initializers are not; neither is a binary operator with a conversion lambda.
    /// </summary>
    private static bool IsSupported(Expression node)
        => node is ParameterExpression or ConstantExpression or DefaultExpression or MemberExpression or MethodCallExpression
                   or ConditionalExpression or NewExpression or NewArrayExpression or TypeBinaryExpression
                   or UnaryExpression { NodeType: not ExpressionType.Quote } or BinaryExpression { Conversion: null };

    /// <summary>
    /// Gets the number of child expressions of a supported node.
    /// </summary>
    private static int ChildCount(Expression node) => node switch
    {
        MemberExpression or UnaryExpression or TypeBinaryExpression => 1,
        BinaryExpression => 2,
        ConditionalExpression => 3,
        MethodCallExpression call => 1 + call.Arguments.Count,
        NewExpression newExpression => newExpression.Arguments.Count,
        NewArrayExpression newArray => newArray.Expressions.Count,
        _ => 0
    };

    /// <summary>
    /// Gets a child expression of a supported node, in the order <see cref="ExpressionVisitor"/> visits them. The
    /// constants of a cached selector are numbered in this order when it is compiled, so the two must agree.
    /// </summary>
    private static Expression? Child(Expression node, int index) => node switch
    {
        MemberExpression member => member.Expression,
        UnaryExpression unary => unary.Operand,
        TypeBinaryExpression typeBinary => typeBinary.Expression,
        BinaryExpression binary => index == 0 ? binary.Left : binary.Right,
        ConditionalExpression conditional => index switch { 0 => conditional.Test, 1 => conditional.IfTrue, _ => conditional.IfFalse },
        MethodCallExpression call => index == 0 ? call.Object : call.Arguments[index - 1],
        NewExpression newExpression => newExpression.Arguments[index],
        NewArrayExpression newArray => newArray.Expressions[index],
        _ => throw new ArgumentOutOfRangeException(nameof(index))
    };

    /// <summary>
    /// Determines whether the constant is passed to the compiled selector at run time rather than compiled into it: any
    /// non-null constant other than a primitive, enum, string or decimal value, such as the closure object that holds
    /// captured variables.
    /// </summary>
    public static bool IsParameterized(ConstantExpression constant)
        => constant.Value is not null && !IsPrimitiveConstant(constant.Type);

    /// <summary>
    /// Collects the values of the parameterized constants of a cacheable selector, in the order the compiled selector
    /// reads them.
    /// </summary>
    public static object?[] CollectConstants(LambdaExpression lambda)
    {
        List<object?>? constants = null;
        Collect(lambda.Body, ref constants);
        return constants?.ToArray() ?? [];

        static void Collect(Expression? node, ref List<object?>? constants)
        {
            if (node is ConstantExpression constant)
            {
                if (IsParameterized(constant))
                {
                    (constants ??= []).Add(constant.Value);
                }

                return;
            }

            if (node is null)
            {
                return;
            }

            for (var i = 0; i < ChildCount(node); i++)
            {
                Collect(Child(node, i), ref constants);
            }
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

        var sameNode = x switch
        {
            ParameterExpression xParameter => IndexOf(xParameters, xParameter) == IndexOf(yParameters, (ParameterExpression)y),
            // Parameterized constants are passed at run time, so only their type (compared above) matters.
            ConstantExpression xConstant => IsParameterized(xConstant)
                ? IsParameterized((ConstantExpression)y)
                : !IsParameterized((ConstantExpression)y) && Equals(xConstant.Value, ((ConstantExpression)y).Value),
            MemberExpression xMember => MembersEqual(xMember.Member, ((MemberExpression)y).Member),
            MethodCallExpression xCall => xCall.Method == ((MethodCallExpression)y).Method,
            UnaryExpression xUnary => xUnary.Method == ((UnaryExpression)y).Method,
            BinaryExpression xBinary => xBinary.Method == ((BinaryExpression)y).Method &&
                                        xBinary.IsLiftedToNull == ((BinaryExpression)y).IsLiftedToNull,
            NewExpression xNew => xNew.Constructor == ((NewExpression)y).Constructor,
            TypeBinaryExpression xTypeBinary => xTypeBinary.TypeOperand == ((TypeBinaryExpression)y).TypeOperand,
            DefaultExpression or ConditionalExpression or NewArrayExpression => true,
            // Only reachable for uncacheable selectors, which are never stored.
            _ => false
        };

        if (!sameNode || ChildCount(x) != ChildCount(y))
        {
            return false;
        }

        for (var i = 0; i < ChildCount(x); i++)
        {
            if (!AreEqual(Child(x, i), Child(y, i), xParameters, yParameters))
            {
                return false;
            }
        }

        return true;
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
            case ConstantExpression constant:
                hash.Add(IsParameterized(constant) ? null : constant.Value);
                break;
            case MemberExpression member:
                hash.Add(member.Member.MetadataToken);
                break;
            case MethodCallExpression call:
                hash.Add(call.Method.MetadataToken);
                break;
            case NewExpression newExpression:
                hash.Add(newExpression.Constructor?.MetadataToken);
                break;
            case TypeBinaryExpression typeBinary:
                hash.Add(typeBinary.TypeOperand);
                break;
        }

        if (!IsSupported(node))
        {
            // Only reachable for uncacheable selectors, which are never stored.
            return;
        }

        for (var i = 0; i < ChildCount(node); i++)
        {
            AddHash(ref hash, Child(node, i), parameters);
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
