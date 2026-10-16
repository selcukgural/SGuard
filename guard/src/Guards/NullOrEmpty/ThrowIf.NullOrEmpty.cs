using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
    /// <returns><paramref name="value"/>, which is not <c>null</c>.</returns>
    [return: NotNull]
    public static T NullOrEmpty<T>([NotNull] T value, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
    {
        var isNullOrEmpty = value is null || Is.InternalIsNullOrEmpty(value);
        if (SGuard.Fails(isNullOrEmpty, callback))
        {
            Throw.NullOrEmptyException(value, valueExpression);
        }

        Debug.Assert(value is not null);
        return value;
    }

    /// <summary>
    /// Throws a <see cref="NullOrEmptyException"/> if the specified span is empty.
    /// Like arrays and collections, a span with elements is not empty, even if all of its elements are <c>null</c>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the ReadOnlySpan.</typeparam>
    /// <param name="value">The ReadOnlySpan to check for null or emptiness.</param>
    /// <param name="callback">An optional callback to execute when the value is null or empty.</param>
    /// <param name="valueExpression">The expression passed as <paramref name="value"/>, captured by the compiler.</param>
    /// <returns><paramref name="value"/>.</returns>
    public static ReadOnlySpan<T> NullOrEmpty<T>(ReadOnlySpan<T> value, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(value))] string? valueExpression = null)
    {
        if (SGuard.Fails(value.IsEmpty, callback))
        {
            Throw.NullOrEmptyExceptionWithoutValue(valueExpression);
        }

        return value;
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
    /// <returns><paramref name="value"/>, which is not <c>null</c>.</returns>
    [return: NotNull]
    public static T NullOrEmpty<T, TException>([NotNull] T value, TException exception, SGuardCallback? callback = null) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(exception);
        var isNullOrEmpty = value is null || Is.InternalIsNullOrEmpty(value);
        if (SGuard.Fails(isNullOrEmpty, callback))
        {
            Throw.That(exception);
        }

        Debug.Assert(value is not null);
        return value;
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
    /// <returns><paramref name="value"/>, which is not <c>null</c>.</returns>
    [Obsolete(Obsoletions.ConstructorArgsMessage, DiagnosticId = Obsoletions.ConstructorArgsDiagnosticId, UrlFormat = Obsoletions.UrlFormat)]
    [return: NotNull]
    public static T NullOrEmpty<T, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TException>([NotNull] T value, object[]? constructorArgs, SGuardCallback? callback = null) where TException : Exception
    {
        var isNullOrEmpty = value is null || Is.InternalIsNullOrEmpty(value);
        if (SGuard.Fails(isNullOrEmpty, callback))
        {
            Throw.That(ExceptionActivator.Create<TException>(constructorArgs));
        }

        Debug.Assert(value is not null);
        return value;
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
    /// <returns><paramref name="value"/>, which is not <c>null</c>.</returns>
    [RequiresUnreferencedCode(AotMessages.Selector)]
    [RequiresDynamicCode(AotMessages.Selector)]
    [return: NotNull]
    public static TValue NullOrEmpty<TValue>([NotNull] TValue? value, Expression<Func<TValue, object?>> selector, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(selector))] string? selectorExpression = null)
    {
        ArgumentNullException.ThrowIfNull(selector);

        var isNullOrEmpty = value is null || SelectorCache.IsNullOrEmpty(value, selector);

        if (SGuard.Fails(isNullOrEmpty, callback))
        {
            Throw.NullOrEmptyExceptionWithoutValue(selectorExpression);
        }

        Debug.Assert(value is not null);
        return value;
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
    /// <returns><paramref name="value"/>, which is not <c>null</c>.</returns>
    [RequiresUnreferencedCode(AotMessages.Selector)]
    [RequiresDynamicCode(AotMessages.Selector)]
    [return: NotNull]
    public static TValue NullOrEmpty<TValue, TException>([NotNull] TValue? value, Expression<Func<TValue, object?>> selector, TException exception,
                                                       SGuardCallback? callback = null) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentNullException.ThrowIfNull(exception);

        var isNullOrEmpty = value is null || SelectorCache.IsNullOrEmpty(value, selector);

        if (SGuard.Fails(isNullOrEmpty, callback))
        {
            Throw.That(exception);
        }

        Debug.Assert(value is not null);
        return value;
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
    /// <returns><paramref name="value"/>, which is not <c>null</c>.</returns>
    [RequiresUnreferencedCode(AotMessages.Selector)]
    [RequiresDynamicCode(AotMessages.Selector)]
    [return: NotNull]
    public static TValue NullOrEmpty<TValue, TException>([NotNull] TValue? value, Expression<Func<TValue, object?>> selector, SGuardCallback? callback = null)
        where TException : Exception, new()
    {
        ArgumentNullException.ThrowIfNull(selector);
        var isNullOrEmpty = value is null || SelectorCache.IsNullOrEmpty(value, selector);
        if (SGuard.Fails(isNullOrEmpty, callback))
        {
            Throw.That(new TException());
        }

        Debug.Assert(value is not null);
        return value;
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
    /// <returns><paramref name="value"/>, which is not <c>null</c>.</returns>
    [RequiresUnreferencedCode(AotMessages.Selector)]
    [RequiresDynamicCode(AotMessages.Selector)]
    [Obsolete(Obsoletions.ConstructorArgsMessage, DiagnosticId = Obsoletions.ConstructorArgsDiagnosticId, UrlFormat = Obsoletions.UrlFormat)]
    [return: NotNull]
    public static TValue NullOrEmpty<TValue, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TException>([NotNull] TValue? value, Expression<Func<TValue, object?>> selector, object?[] constructorArgs,
                                                       SGuardCallback? callback = null) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(selector);
        var isNullOrEmpty = value is null || SelectorCache.IsNullOrEmpty(value, selector);
        if (SGuard.Fails(isNullOrEmpty, callback))
        {
            Throw.That(ExceptionActivator.Create<TException>(constructorArgs));
        }

        Debug.Assert(value is not null);
        return value;
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/> if the value is null or empty, and returns the
    /// value otherwise. The factory runs only when the guard fails.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="exceptionFactory">Creates the exception to throw.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <returns><paramref name="value"/>, which is not <c>null</c>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="exceptionFactory"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="exceptionFactory"/> returned <c>null</c>.</exception>
    [return: NotNull]
    public static T NullOrEmpty<T, TException>([NotNull] T value, Func<TException> exceptionFactory, SGuardCallback? callback = null)
        where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        if (SGuard.Fails(value is null || Is.InternalIsNullOrEmpty(value), callback))
        {
            Throw.From(exceptionFactory);
        }

        Debug.Assert(value is not null);
        return value;
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/> if the span is empty, and returns the span
    /// otherwise. The factory runs only when the guard fails.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the span.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw.</typeparam>
    /// <param name="value">The span to check.</param>
    /// <param name="exceptionFactory">Creates the exception to throw.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <returns><paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="exceptionFactory"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="exceptionFactory"/> returned <c>null</c>.</exception>
    public static ReadOnlySpan<T> NullOrEmpty<T, TException>(ReadOnlySpan<T> value, Func<TException> exceptionFactory,
        SGuardCallback? callback = null) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        if (SGuard.Fails(value.IsEmpty, callback))
        {
            Throw.From(exceptionFactory);
        }

        return value;
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/> if the value, or the member the selector
    /// reaches, is null or empty, and returns the value otherwise. The factory runs only when the guard fails.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="selector">An expression that selects the member to check, such as <c>o => o.Customer.Email</c>.</param>
    /// <param name="exceptionFactory">Creates the exception to throw.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <returns><paramref name="value"/>, which is not <c>null</c>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> or <paramref name="exceptionFactory"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="exceptionFactory"/> returned <c>null</c>.</exception>
    [RequiresUnreferencedCode(AotMessages.Selector)]
    [RequiresDynamicCode(AotMessages.Selector)]
    [return: NotNull]
    public static TValue NullOrEmpty<TValue, TException>([NotNull] TValue? value, Expression<Func<TValue, object?>> selector,
        Func<TException> exceptionFactory, SGuardCallback? callback = null) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        if (SGuard.Fails(value is null || SelectorCache.IsNullOrEmpty(value, selector), callback))
        {
            Throw.From(exceptionFactory);
        }

        Debug.Assert(value is not null);
        return value;
    }
}
