using System.Collections.Concurrent;
using System.Linq.Expressions;
using SGuard.Exceptions;

namespace SGuard;

public sealed partial class ThrowIf
{
    /// <summary>
    /// A visitor class used to evaluate expressions for null or empty checks.
    /// </summary>
    private static readonly NullOrEmptyVisitor NullOrEmptyVisitor = new();

    /// <summary>
    /// A predefined exception instance used for null or empty value checks.
    /// </summary>
    private static readonly NullOrEmptyException NullOrEmptyException = new();

    /// <summary>
    /// A thread-safe cache for storing compiled expressions to improve performance.
    /// Limits the cache size to avoid memory leaks.
    /// </summary>
    private static readonly ConcurrentDictionary<Expression, Delegate> CompiledExpressionCache = new();

    /// <summary>
    /// The maximum size of the compiled expression cache.
    /// When the cache exceeds this size, it is cleared to prevent memory leaks.
    /// </summary>
    private const int MaxCompiledExpressionCacheSize = 1000;

    /// <summary>
    /// Checks if the specified value is null or empty.
    /// If the value is null or empty, throws a predefined exception.
    /// </summary>
    /// <typeparam name="T">The type of the value to check.</typeparam>
    /// <param name="value">The value to check for null or emptiness.</param>
    /// <param name="callback">An optional callback to execute if the value is null or empty.</param>
    public static void NullOrEmpty<T>(T value, SGuardCallback? callback = null)
    {
        var isNullOrEmpty = value is null || Is.InternalIsNullOrEmpty(value);
        SGuard.Guard(isNullOrEmpty, () => Throw.NullOrEmptyException(value), callback);
    }

    /// <summary>
    /// Checks if the specified value is null or empty.
    /// If the value is null or empty, throws the specified exception.
    /// </summary>
    /// <typeparam name="T">The type of the value to check.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw if the value is null or empty.</typeparam>
    /// <param name="value">The value to check for null or emptiness.</param>
    /// <param name="exception">The exception to throw if the value is null or empty.</param>
    /// <param name="callback">An optional callback to execute if the value is null or empty.</param>
    /// <exception cref="ArgumentNullException">Thrown if the exception is null.</exception>
    public static void NullOrEmpty<T, TException>(T value, TException exception, SGuardCallback? callback = null) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(exception);
        var isNullOrEmpty = value is null || Is.InternalIsNullOrEmpty(value);
        SGuard.Guard(isNullOrEmpty, () => Throw.That(exception), callback);
    }

    /// <summary>
    /// Checks if the specified value is null or empty.
    /// If the value is null or empty, throws an exception of the specified type, created using the provided constructor arguments.
    /// </summary>
    /// <typeparam name="T">The type of the value to check.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw if the value is null or empty.</typeparam>
    /// <param name="value">The value to check for null or emptiness.</param>
    /// <param name="constructorArgs">An array of arguments to pass to the exception constructor.</param>
    /// <param name="callback">An optional callback to execute if the value is null or empty.</param>
    /// <exception cref="TException">Thrown if the value is null or empty.</exception>
    public static void NullOrEmpty<T, TException>(T value, object[]? constructorArgs, SGuardCallback? callback = null) where TException : Exception
    {
        var isNullOrEmpty = value is null || Is.InternalIsNullOrEmpty(value);
        SGuard.Guard(isNullOrEmpty, () => Throw.That(ExceptionActivator.Create<TException>(constructorArgs)), callback);
    }

    /// <summary>
    /// Checks if the specified value is null or empty based on the provided selector expression.
    /// If the value is null or empty, throws a predefined exception.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to check.</typeparam>
    /// <param name="value">The value to check for null or emptiness.</param>
    /// <param name="selector">An expression to select a property or field from the value.</param>
    /// <param name="callback">An optional callback to execute if the value is null or empty.</param>
    public static void NullOrEmpty<TValue>(TValue value, Expression<Func<TValue, object?>> selector, SGuardCallback? callback = null)
    {
        NullOrEmpty(value, selector, NullOrEmptyException, callback);
    }

    /// <summary>
    /// Checks if the specified value is null or empty based on the provided selector expression.
    /// If the value is null or empty, throws the specified exception.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to check.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw if the value is null or empty.</typeparam>
    /// <param name="value">The value to check for null or emptiness.</param>
    /// <param name="selector">An expression to select a property or field from the value.</param>
    /// <param name="exception">The exception to throw if the value is null or empty.</param>
    /// <param name="callback">An optional callback to execute if the value is null or empty.</param>
    /// <exception cref="ArgumentNullException">Thrown if the selector or exception is null.</exception>
    public static void NullOrEmpty<TValue, TException>(TValue value, Expression<Func<TValue, object?>> selector, TException exception,
                                                       SGuardCallback? callback = null) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentNullException.ThrowIfNull(exception);

        var isNullOrEmpty = value is null || CheckNullOrEmpty(value, selector);

        SGuard.Guard(isNullOrEmpty, () => Throw.That(exception), callback);
    }

    /// <summary>
    /// Checks if the specified value is null or empty based on the provided selector expression.
    /// If the value is null or empty, throws a new exception of the specified type.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to check.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw if the value is null or empty.</typeparam>
    /// <param name="value">The value to check for null or emptiness.</param>
    /// <param name="selector">An expression to select a property or field from the value.</param>
    /// <param name="callback">An optional callback to execute if the value is null or empty.</param>
    /// <exception cref="ArgumentNullException">Thrown if the selector is null.</exception>
    /// <exception cref="TException">Thrown if the value is null or empty.</exception>
    public static void NullOrEmpty<TValue, TException>(TValue value, Expression<Func<TValue, object?>> selector, SGuardCallback? callback = null)
        where TException : Exception, new()
    {
        ArgumentNullException.ThrowIfNull(selector);
        var isNullOrEmpty = value is null || CheckNullOrEmpty(value, selector);
        SGuard.Guard(isNullOrEmpty, () => Throw.That(new TException()), callback);
    }

    /// <summary>
    /// Checks if the specified value is null or empty based on the provided selector expression.
    /// If the value is null or empty, throws an exception of the specified type, created using the provided constructor arguments.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to check.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw if the value is null or empty.</typeparam>
    /// <param name="value">The value to check for null or emptiness.</param>
    /// <param name="selector">An expression to select a property or field from the value.</param>
    /// <param name="constructorArgs">An array of arguments to pass to the exception constructor.</param>
    /// <param name="callback">An optional callback to execute if the value is null or empty.</param>
    /// <exception cref="ArgumentNullException">Thrown if the selector is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the selector expression cannot be processed.</exception>
    /// <exception cref="TException">Thrown if the value is null or empty.</exception>
    public static void NullOrEmpty<TValue, TException>(TValue value, Expression<Func<TValue, object?>> selector, object?[] constructorArgs,
                                                       SGuardCallback? callback = null) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(selector);
        var isNullOrEmpty = value is null || CheckNullOrEmpty(value, selector);
        SGuard.Guard(isNullOrEmpty, () => Throw.That(ExceptionActivator.Create<TException>(constructorArgs)), callback);
    }

    /// <summary>
    /// Evaluates whether the specified object is null or empty based on the provided expression.
    /// </summary>
    /// <typeparam name="TValue">The type of the object to evaluate.</typeparam>
    /// <param name="obj">The object to evaluate.</param>
    /// <param name="valueExpression">An expression to select a property or field from the object.</param>
    /// <returns>True if the selected value is null or empty; otherwise, false.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the expression cannot be processed.</exception>
    private static bool CheckNullOrEmpty<TValue>(TValue obj, Expression<Func<TValue, object?>> valueExpression)
    {
        if (NullOrEmptyVisitor.Visit(valueExpression) is not Expression<Func<TValue, object?>> expression)
            throw new InvalidOperationException("Unable to process the expression.");

        // Limit cache to avoid memory leak
        if (CompiledExpressionCache.Count > MaxCompiledExpressionCacheSize)
            CompiledExpressionCache.Clear();

        var func = (Func<TValue, object?>)CompiledExpressionCache.GetOrAdd(expression, exp => ((Expression<Func<TValue, object?>>)exp).Compile());

        return Is.InternalIsNullOrEmpty(func(obj));
    }
}