using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using SGuard.Exceptions;

namespace SGuard;

public sealed partial class ThrowIf
{
    /// <summary>
    /// Checks if the specified value is null or empty.
    /// If the value is null or empty, throws a predefined exception.
    /// </summary>
    /// <typeparam name="T">The type of the value to check.</typeparam>
    /// <param name="value">The value to check for null or emptiness.</param>
    /// <param name="callback">An optional callback to execute if the value is null or empty.</param>
    /// <param name="valueExpression">The expression passed as <paramref name="value"/>, captured by the compiler.</param>
    public static void NullOrEmpty<T>(T value, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
    {
        var isNullOrEmpty = value is null || Is.InternalIsNullOrEmpty(value);
        if (SGuard.Fails(isNullOrEmpty, callback))
        {
            Throw.NullOrEmptyException(value, valueExpression);
        }
    }

    /// <summary>
    /// Throws a <see cref="NullOrEmptyException"/> if the specified span is empty.
    /// Like arrays and collections, a span with elements is not empty, even if all of its elements are <c>null</c>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the ReadOnlySpan.</typeparam>
    /// <param name="value">The ReadOnlySpan to check for null or emptiness.</param>
    /// <param name="callback">An optional callback to execute when the value is null or empty.</param>
    /// <param name="valueExpression">The expression passed as <paramref name="value"/>, captured by the compiler.</param>
    public static void NullOrEmpty<T>(ReadOnlySpan<T> value, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
    {
        if (SGuard.Fails(value.IsEmpty, callback))
        {
            Throw.NullOrEmptyException<object?>(null, valueExpression);
        }
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
        if (SGuard.Fails(isNullOrEmpty, callback))
        {
            Throw.That(exception);
        }
    }

    /// <summary>
    /// Checks if the specified value is null or empty.
    /// If the value is null or empty, throws an exception to the specified type, created using the provided constructor arguments.
    /// </summary>
    /// <typeparam name="T">The type of the value to check.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw if the value is null or empty.</typeparam>
    /// <param name="value">The value to check for null or emptiness.</param>
    /// <param name="constructorArgs">An array of arguments to pass to the exception constructor.</param>
    /// <param name="callback">An optional callback to execute if the value is null or empty.</param>
    /// <exception cref="Exception">Thrown if the value is null or empty.</exception>
    public static void NullOrEmpty<T, TException>(T value, object[]? constructorArgs, SGuardCallback? callback = null) where TException : Exception
    {
        var isNullOrEmpty = value is null || Is.InternalIsNullOrEmpty(value);
        if (SGuard.Fails(isNullOrEmpty, callback))
        {
            Throw.That(ExceptionActivator.Create<TException>(constructorArgs));
        }
    }

    /// <summary>
    /// Checks if the specified value is null or empty based on the provided selector expression.
    /// If the value is null or empty, throws a <see cref="NullOrEmptyException"/> that names the selector.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to check.</typeparam>
    /// <param name="value">The value to check for null or emptiness.</param>
    /// <param name="selector">An expression to select a property or field from the value.</param>
    /// <param name="callback">An optional callback to execute if the value is null or empty.</param>
    /// <param name="selectorExpression">The expression passed as <paramref name="selector"/>, captured by the compiler.</param>
    public static void NullOrEmpty<TValue>(TValue value, Expression<Func<TValue, object?>> selector, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(selector))] string? selectorExpression = null)
    {
        ArgumentNullException.ThrowIfNull(selector);

        var isNullOrEmpty = value is null || SelectorCache.IsNullOrEmpty(value, selector);

        if (SGuard.Fails(isNullOrEmpty, callback))
        {
            Throw.NullOrEmptyException<object?>(null, selectorExpression);
        }
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

        var isNullOrEmpty = value is null || SelectorCache.IsNullOrEmpty(value, selector);

        if (SGuard.Fails(isNullOrEmpty, callback))
        {
            Throw.That(exception);
        }
    }

    /// <summary>
    /// Checks if the specified value is null or empty based on the provided selector expression.
    /// If the value is null or empty, throws a new exception to the specified type.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to check.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw if the value is null or empty.</typeparam>
    /// <param name="value">The value to check for null or emptiness.</param>
    /// <param name="selector">An expression to select a property or field from the value.</param>
    /// <param name="callback">An optional callback to execute if the value is null or empty.</param>
    /// <exception cref="ArgumentNullException">Thrown if the selector is null.</exception>
    /// <exception cref="Exception">Thrown if the value is null or empty.</exception>
    public static void NullOrEmpty<TValue, TException>(TValue value, Expression<Func<TValue, object?>> selector, SGuardCallback? callback = null)
        where TException : Exception, new()
    {
        ArgumentNullException.ThrowIfNull(selector);
        var isNullOrEmpty = value is null || SelectorCache.IsNullOrEmpty(value, selector);
        if (SGuard.Fails(isNullOrEmpty, callback))
        {
            Throw.That(new TException());
        }
    }

    /// <summary>
    /// Checks if the specified value is null or empty based on the provided selector expression.
    /// If the value is null or empty, throws an exception to the specified type, created using the provided constructor arguments.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to check.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw if the value is null or empty.</typeparam>
    /// <param name="value">The value to check for null or emptiness.</param>
    /// <param name="selector">An expression to select a property or field from the value.</param>
    /// <param name="constructorArgs">An array of arguments to pass to the exception constructor.</param>
    /// <param name="callback">An optional callback to execute if the value is null or empty.</param>
    /// <exception cref="ArgumentNullException">Thrown if the selector is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if no constructor of <typeparamref name="TException"/> matches <paramref name="constructorArgs"/>.</exception>
    /// <exception cref="Exception">Thrown if the value is null or empty.</exception>
    public static void NullOrEmpty<TValue, TException>(TValue value, Expression<Func<TValue, object?>> selector, object?[] constructorArgs,
                                                       SGuardCallback? callback = null) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(selector);
        var isNullOrEmpty = value is null || SelectorCache.IsNullOrEmpty(value, selector);
        if (SGuard.Fails(isNullOrEmpty, callback))
        {
            Throw.That(ExceptionActivator.Create<TException>(constructorArgs));
        }
    }
}