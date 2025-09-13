using System.Collections;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace SGuard;

public sealed partial class Is
{
    /// <summary>
    /// A static instance of the <see cref="NullOrEmptyVisitor"/> class used for visiting expressions
    /// to determine if they are null or empty.
    /// </summary>
    private static readonly NullOrEmptyVisitor NullOrEmptyVisitor = new();

    /// <summary>
    /// Evaluates whether the specified value is null or empty based on predefined patterns, and optionally invokes a callback with the result.
    /// </summary>
    /// <typeparam name="T">The type of the value to evaluate.</typeparam>
    /// <param name="value">The value to evaluate for null or emptiness.</param>
    /// <param name="callback">An optional callback to invoke with the outcome of the evaluation.</param>
    /// <returns>
    /// <c>true</c> if the value is determined to be null, the default value for its type, or matches predefined empty patterns; otherwise, <c>false</c>.
    /// </returns>
    public static bool NullOrEmpty<T>(T? value, SGuardCallback? callback = null)
    {
        var result = InternalIsNullOrEmpty(value);

        SGuard.InvokeCallbackSafely(result, callback);

        return result;
    }

    /// <summary>
    /// Determines whether the specified value is null or empty based on general patterns or a provided property selector.
    /// </summary>
    /// <typeparam name="T">The type of the value to be analyzed.</typeparam>
    /// <param name="value">The object to be checked for null or emptiness.</param>
    /// <param name="selector">An expression selecting a specific property or field of the object to be checked for null or emptiness.</param>
    /// <param name="callback">An optional callback invoked with the outcome of the check, indicating either success or failure.</param>
    /// <returns>
    /// <c>true</c> if the value or the selected property is null, empty, or matches predefined empty patterns; otherwise, <c>false</c>.
    /// </returns>
    public static bool NullOrEmpty<T>(T? value, Expression<Func<T, object>> selector, SGuardCallback? callback = null)
    {
        ArgumentNullException.ThrowIfNull(selector);

        if (value is null)
        {
            SGuard.InvokeCallbackSafely(true, callback);
            return true;
        }

        var expression = NullOrEmptyVisitor.Visit(selector) as Expression<Func<T, object>>;

        var isNullOrEmpty = expression?.Compile().Invoke(value) is null;

        SGuard.InvokeCallbackSafely(isNullOrEmpty, callback);

        return isNullOrEmpty;
    }

    /// <summary>
    /// Determines whether the specified value is null or empty by checking against various patterns.
    /// </summary>
    /// <typeparam name="T">The type of the value to check.</typeparam>
    /// <param name="value">The value to check for null or emptiness.</param>
    /// <returns>
    /// <c>true</c> if the value is null, the default value for its type, or matches predefined empty patterns; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool InternalIsNullOrEmpty<T>(T? value)
    {
        return value is null || IsDefaultValue(value) || MatchesEmptyPatterns(value);

        // Checks if the given value matches specific patterns that define it as empty.
        // Returns: True if the value matches any of the predefined empty patterns; false otherwise.
        // The method uses a switch expression to handle various types:
        // - Strings are empty if they are null or empty.
        // - Numeric types are empty if they are zero (handled per-type to avoid Convert exceptions).
        // - Booleans are empty if they are false.
        // - GUIDs are empty if they are Guid.Empty.
        // - Arrays, collections, and enumerables are empty if they have no elements.
        // - DateTime, TimeSpan, DateOnly, TimeOnly, and DateTimeOffset are empty if their ticks are zero or at their minimum value.
        // - Complex types are checked using the <see cref="IsEmptyComplexType{T}"/> method.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool MatchesEmptyPatterns(T? value)
        {
            switch (value)
            {
                case string str:
                    return string.IsNullOrEmpty(str);
                case decimal d:
                    return d == 0m;
                case double d:
                    return d == 0d;
                case float f:
                    return f == 0f;
                case byte b:
                    return b == 0;
                case sbyte sb:
                    return sb == 0;
                case short s:
                    return s == 0;
                case ushort us:
                    return us == 0;
                case int i:
                    return i == 0;
                case uint ui:
                    return ui == 0;
                case long l:
                    return l == 0;
                case ulong ul:
                    return ul == 0;
                case bool b:
                    return !b;
                case Guid g:
                    return g == Guid.Empty;
                case DateTime dt:
                    return dt.Ticks == 0;
                case TimeSpan ts:
                    return ts.Ticks == 0;
                case DateOnly dateOnly:
                    return dateOnly == DateOnly.MinValue;
                case TimeOnly timeOnly:
                    return timeOnly.Ticks == 0;
                case DateTimeOffset dto:
                    return dto.Ticks == 0;
                case ICollection collection:
                    return collection.Count == 0;
                case IEnumerable enumerable:
                {
                    var enumerator = enumerable.GetEnumerator();

                    try
                    {
                        return !enumerator.MoveNext();
                    }
                    finally
                    {
                        (enumerator as IDisposable)?.Dispose();
                    }
                }
                default:
                    return false;
            }
        }

        // Determines if the given value is the default value for its type.
        // True if the value is the default value for its type, false otherwise.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool IsDefaultValue(T value)
        {
            return EqualityComparer<T>.Default.Equals(value, default);
        }
    }
}