using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using SGuard.Exceptions;

namespace SGuard;

public sealed partial class ThrowIf
{
    /// <summary>
    /// A static instance of the <see cref="NullOrEmptyVisitor"/> class used to process
    /// expressions that check for null or empty values.
    /// </summary>
    private static readonly NullOrEmptyVisitor NullOrEmptyVisitor = new();

    /// <summary>
    /// A thread-safe cache for storing compiled expressions.
    /// </summary>
    /// <remarks>
    /// This dictionary is used to store compiled delegates for expressions to avoid
    /// the performance overhead of recompiling the same expressions multiple times.
    /// The key is the original expression, and the value is the compiled delegate.
    /// </remarks>
    private static readonly ConcurrentDictionary<Expression, Delegate> CompiledExpressionCache = new();


#pragma warning disable CS8777 //Parameter 'value' must have a non-null value when exiting.
    /// <summary>
    /// Validates that the specified value is not null or empty and throws an appropriate exception if the condition is not met.
    /// </summary>
    /// <typeparam name="T">The type of the value to validate.</typeparam>
    /// <param name="value">The value to check for null or empty state.</param>
    /// <param name="callback">An optional callback to handle the result of the guard check.</param>
    /// <exception cref="NullOrEmptyException">Thrown when the value is null or empty.</exception>
    public static void NullOrEmpty<T>([NotNull] T value, SGuardCallback? callback = null)
    {
        var isNullOrEmpty = value is null || Is.InternalIsNullOrEmpty(value);
        SGuard.Guard(isNullOrEmpty, () => Throw.NullOrEmptyException(value), callback);
    }

    /// <summary>
    /// Ensures that the provided value is not null or empty and throws an exception to the specified type if the condition is not met.
    /// </summary>
    /// <typeparam name="T">The type of the value to check.</typeparam>
    /// <typeparam name="TException">The type of exception to throw if the condition is not met.</typeparam>
    /// <param name="value">The value to check for null or emptiness.</param>
    /// <param name="exception">The exception to throw when the value is null or empty.</param>
    /// <param name="callback">An optional callback to be invoked with the guard outcome.</param>
    /// <exception cref="TException">Thrown when the value is null or empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the exception parameter is null.</exception>
    public static void NullOrEmpty<T, TException>([NotNull] T value, [NotNull] TException exception, SGuardCallback? callback = null)
        where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(exception);
        
        var isNullOrEmpty = value is null || Is.InternalIsNullOrEmpty(value);
        
        SGuard.Guard(isNullOrEmpty, () => Throw.That(exception), callback);
    }


    /// <summary>
    /// Validates that the specified value is not null or empty and throws an appropriate exception if the condition is not met.
    /// </summary>
    /// <typeparam name="T">The type of the value to validate.</typeparam>
    /// <typeparam name="TException">The type of exception to throw if the validation fails.</typeparam>
    /// <param name="value">The value to check for null or empty state.</param>
    /// <param name="constructorArgs">Optional arguments used to construct the exception of type <typeparamref name="TException"/>.</param>
    /// <param name="callback">An optional callback to handle the result of the guard check.</param>
    /// <exception cref="TException">Thrown when the value is null or empty.</exception>
    public static void NullOrEmpty<T, TException>([NotNull] T value, object[]? constructorArgs, SGuardCallback? callback = null)
        where TException : Exception
    {
        var isNullOrEmpty =  value is null || Is.InternalIsNullOrEmpty(value);
        
        SGuard.Guard(isNullOrEmpty, () => Throw.That(ExceptionActivator.Create<TException>(constructorArgs)), callback);
    }


    /// <summary>
    /// Validates that the specified value is neither null nor empty. Throws an appropriate exception if the validation fails.
    /// </summary>
    /// <param name="value">The value to check for null or empty state.</param>
    /// <param name="callback">An optional callback to execute after the guard operation.</param>
    /// <exception cref="SGuard.Exceptions.NullOrEmptyException">Thrown when the value is null or empty.</exception>
    /// <typeparam name="TValue">The type of the value being validated.</typeparam>
    /// <param name="selector">An expression selecting the member to validate for null or empty.</param>
    public static void NullOrEmpty<TValue>([NotNull] TValue value, Expression<Func<TValue, object?>> selector, SGuardCallback? callback = null)
    {
        NullOrEmpty(value, selector, new NullOrEmptyException(), callback);
    }


    /// <summary>
    /// Validates that the specified value is not null or empty and throws an exception if the condition is not met.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to validate.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw if the validation fails.</typeparam>
    /// <param name="value">The value to check for null or empty state.</param>
    /// <param name="selector">An expression to select a member from the value for additional null or empty validation.</param>
    /// <param name="exception">The exception to throw if the value or selected member is null or empty.</param>
    /// <param name="callback">An optional callback to handle the result of the guard check.</param>
    /// <exception cref="ArgumentNullException">Thrown if the exception or selector arguments are null.</exception>
    /// <exception cref="TException">Thrown when the value or selected member is null or empty.</exception>
    public static void NullOrEmpty<TValue, TException>([NotNull] TValue value, Expression<Func<TValue, object?>> selector,
                                                       [NotNull] TException exception, SGuardCallback? callback = null) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentNullException.ThrowIfNull(exception);

        var isNullOrEmpty = value is null ||  CheckNullOrEmpty(value, selector);
        
        SGuard.Guard(isNullOrEmpty, () => Throw.That(exception), callback);
    }

    /// <summary>
    /// Validates that the specified value is neither null nor empty and throws an exception to the specified type if the condition is not met.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to validate.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw if the validation fails.</typeparam>
    /// <param name="value">The value to check for null or empty state.</param>
    /// <param name="selector">A lambda expression selecting the object to validate within the value.</param>
    /// <param name="callback">An optional callback to handle the result of the guard check.</param>
    /// <exception cref="TException">Thrown when the specified value is null or empty.</exception>
    public static void NullOrEmpty<TValue, TException>([NotNull] TValue value, Expression<Func<TValue, object?>> selector,
                                                       SGuardCallback? callback = null) where TException : Exception, new()
    {
        ArgumentNullException.ThrowIfNull(selector);
        
        var isNullOrEmpty = value is null || CheckNullOrEmpty(value, selector);
        
        SGuard.Guard(isNullOrEmpty,()=> Throw.That(new TException()), callback);
    }
    
    /// <summary>
    /// Validates that the specified value is not null or empty based on a selector and throws an exception to the specified type if the condition is not met.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to validate.</typeparam>
    /// <typeparam name="TException">The type of exception to throw when the value is null or empty.</typeparam>
    /// <param name="value">The value to check for null or empty state.</param>
    /// <param name="selector">An expression to select a part of the value to validate.</param>
    /// <param name="constructorArgs">Optional arguments used to construct the exception.</param>
    /// <param name="callback">An optional callback to handle the result of the guard check.</param>
    /// <exception cref="TException">Thrown when the value is null or empty.</exception>
    public static void NullOrEmpty<TValue, TException>([NotNull] TValue value, Expression<Func<TValue, object?>> selector, object?[] constructorArgs,
                                                       SGuardCallback? callback = null) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(selector);
        
        var isNullOrEmpty = value is null || CheckNullOrEmpty(value, selector);
        
        SGuard.Guard(isNullOrEmpty, () => Throw.That(ExceptionActivator.Create<TException>(constructorArgs)), callback);
    }
#pragma warning restore CS8777 //Parameter 'value' must have a non-null value when exiting.
    
    /// <summary>
    /// Helper method to check if an object or its property value is null or empty.
    /// </summary>
    /// <typeparam name="TValue">The type of the object to check.</typeparam>
    /// <param name="obj">The object to check.</param>
    /// <param name="valueExpression">The expression representing the property to check.</param>
    /// <returns>True if the object or its property value is null or empty; otherwise, false.</returns>
    private static bool CheckNullOrEmpty<TValue>(TValue obj, Expression<Func<TValue, object?>> valueExpression)
    {
        if (NullOrEmptyVisitor.Visit(valueExpression) is not Expression<Func<TValue, object?>> expression)
        {
            throw new InvalidOperationException("Unable to process the expression.");
        }

        var func = (Func<TValue, object?>)CompiledExpressionCache.GetOrAdd(expression, exp => ((Expression<Func<TValue, object?>>)exp).Compile());

        return Is.InternalIsNullOrEmpty(func(obj));
    }
}