using System.Runtime.CompilerServices;

namespace SGuard;

public sealed partial class Is
{
    /// <summary>
    /// Determines whether the enum value is not one of the values its type defines, and optionally invokes a callback
    /// with the result.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="Enum.IsDefined{TEnum}(TEnum)"/>, so a combination of <see cref="FlagsAttribute"/> values is not
    /// defined unless the enum declares a member with that exact value.
    /// </remarks>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="callback">An optional callback invoked with the outcome.</param>
    /// <returns><c>true</c> if <typeparamref name="TEnum"/> has no member with this value; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NotDefined<TEnum>(TEnum value, SGuardCallback? callback = null) where TEnum : struct, Enum
    {
        var isNotDefined = !Enum.IsDefined(value);

        SGuard.InvokeCallbackSafely(isNotDefined, callback);

        return isNotDefined;
    }
}
