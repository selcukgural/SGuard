using System.Collections;
using System.Linq.Expressions;
using SGuard.Visitor;

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
    internal static bool InternalIsNullOrEmpty<T>(T? value)
    {
        return value is null || IsDefaultValue(value) || MatchesEmptyPatterns(value);


        // Determines if a complex type object is empty.
        // True if the object is null or all its readable properties are null or empty; false otherwise.
        static bool IsEmptyComplexType(T? value)
        {
            if (value == null)
            {
                return true;
            }

            var valueType = value.GetType();
            var properties = valueType.GetProperties();

            if (properties.Length == 0)
            {
                return false;
            }

            return (valueType is { IsValueType: true, IsEnum: false } || valueType.IsClass || IsClassOrAnonymousType(valueType)) &&
                   !properties.Any(e => e.CanRead && !string.IsNullOrEmpty(e.GetValue(value)?.ToString()));

            // Determines if the given type is a class or an anonymous type.
            // True if the type is a class or an anonymous type; false otherwise.
            static bool IsClassOrAnonymousType(Type type)
            {
                return type.IsClass || (Attribute.IsDefined(type, typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false) &&
                                        type.IsGenericType && (type.Name.Contains("AnonymousType") ||
                                                               type.Name.StartsWith("<>f__AnonymousType", StringComparison.Ordinal)));
            }
        }


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
        static bool MatchesEmptyPatterns(T? value)
        {
            return value switch
            {
                string str => string.IsNullOrEmpty(str),

                decimal d => d == 0m,
                double d  => d == 0d,
                float f   => f == 0f,
                byte b    => b == 0,
                sbyte sb  => sb == 0,
                short s   => s == 0,
                ushort us => us == 0,
                int i     => i == 0,
                uint ui   => ui == 0,
                long l    => l == 0,
                ulong ul  => ul == 0,

                bool b => !b,
                Guid g => g == Guid.Empty,

                Array array            => array.Length == 0,
                IDictionary dictionary => dictionary.Count == 0,
                IList list             => list.Count == 0,
                ICollection collection => collection.Count == 0,

                IEnumerable e when TryGetReadOnlyCount(e, out var roCount) => roCount == 0,

                DateTime dt        => dt.Ticks == 0,
                TimeSpan ts        => ts.Ticks == 0,
                DateOnly dateOnly  => dateOnly == DateOnly.MinValue,
                TimeOnly timeOnly  => timeOnly.Ticks == 0,
                DateTimeOffset dto => dto.Ticks == 0,

                IEnumerable enumerable => !enumerable.Cast<object?>().Any(),

                _ => IsEmptyComplexType(value)
            };
        }

        // Determines if the given value is the default value for its type.
        // True if the value is the default value for its type, false otherwise.
        static bool IsDefaultValue(T value)
        {
            return EqualityComparer<T>.Default.Equals(value, default);
        }

        // Try to get Count for IReadOnlyCollection<> or IReadOnlyDictionary<,> via reflection.
        // Returns true if a Count property was found and read successfully.
        static bool TryGetReadOnlyCount(IEnumerable instance, out int count)
        {
            count = 0;
            var type = instance.GetType();

            // Check IReadOnlyCollection<T>
            var roCollectionIface = type.GetInterfaces()
                                        .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IReadOnlyCollection<>));

            if (roCollectionIface is not null)
            {
                var countProp = roCollectionIface.GetProperty(nameof(IReadOnlyCollection<object>.Count));
                var value = countProp?.GetValue(instance);

                if (value is int c)
                {
                    count = c;
                    return true;
                }
            }

            // Check IReadOnlyDictionary<TKey, TValue>
            var roDictIface = type.GetInterfaces()
                                  .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>));

            if (roDictIface is not null)
            {
                var countProp = roDictIface.GetProperty(nameof(IReadOnlyDictionary<object, object>.Count));

                var value = countProp?.GetValue(instance);

                if (value is not int c)
                {
                    return false;
                }

                count = c;
                return true;
            }

            return false;
        }
    }
}