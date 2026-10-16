using System.Runtime.CompilerServices;
using SGuard.Exceptions;

namespace SGuard;

public sealed partial class ThrowIf
{
    /// <summary>
    /// Throws a <see cref="NotDefinedException"/> if the enum value is not one of the values its type defines, and returns
    /// the value otherwise.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="Enum.IsDefined{TEnum}(TEnum)"/>, so a combination of <see cref="FlagsAttribute"/> values fails
    /// unless the enum declares a member with that exact value.
    /// </remarks>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <param name="valueExpression">The expression passed as <paramref name="value"/>, captured by the compiler.</param>
    /// <returns><paramref name="value"/>.</returns>
    /// <exception cref="NotDefinedException"><typeparamref name="TEnum"/> has no member with this value.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum NotDefined<TEnum>(TEnum value, SGuardCallback? callback = null,
        [CallerArgumentExpression(nameof(value))] string? valueExpression = null) where TEnum : struct, Enum
    {
        if (SGuard.Fails(!Enum.IsDefined(value), callback))
        {
            Throw.NotDefinedException(value, valueExpression);
        }

        return value;
    }

    /// <summary>
    /// Throws the exception created by <paramref name="exceptionFactory"/> if the enum value is not one of the values its
    /// type defines, and returns the value otherwise. The factory runs only when the guard fails.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <typeparam name="TException">The type of the exception to throw.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="exceptionFactory">Creates the exception to throw.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the guard.</param>
    /// <returns><paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="exceptionFactory"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="exceptionFactory"/> returned <c>null</c>.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum NotDefined<TEnum, TException>(TEnum value, Func<TException> exceptionFactory, SGuardCallback? callback = null)
        where TEnum : struct, Enum where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        if (SGuard.Fails(!Enum.IsDefined(value), callback))
        {
            Throw.From(exceptionFactory);
        }

        return value;
    }
}
