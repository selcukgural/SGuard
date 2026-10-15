namespace SGuard;

/// <summary>
/// Messages of the <see cref="System.Diagnostics.CodeAnalysis.RequiresUnreferencedCodeAttribute"/> and
/// <see cref="System.Diagnostics.CodeAnalysis.RequiresDynamicCodeAttribute"/> annotations, which the trimming and Native
/// AOT analyzers show at the call site.
/// </summary>
internal static class AotMessages
{
    public const string Selector =
        "A NullOrEmpty selector is compiled at run time and reads the members it reaches with reflection: trimming can " +
        "remove those members, and selectors don't work under Native AOT. Check the member directly instead, for example " +
        "ThrowIf.NullOrEmpty(order.Customer?.Email).";
}
