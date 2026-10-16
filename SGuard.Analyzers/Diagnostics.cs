using Microsoft.CodeAnalysis;

namespace SGuard.Analyzers;

/// <summary>
/// The diagnostics SGuard reports. IDs from SGUARD101 on; SGUARD001 and SGUARD002 are the obsoletion warnings of the
/// library itself. Each ID has a section on the documentation site that <see cref="HelpLink"/> points to.
/// </summary>
internal static class Diagnostics
{
    private const string Category = "Usage";

    private static string HelpLink(string id) => $"https://selcukgural.github.io/SGuard/docs/advanced/analyzers#{id}";

    public static readonly DiagnosticDescriptor NullOrEmptyOnNumber = new(
        "SGUARD101",
        "NullOrEmpty rejects zero and false",
        "NullOrEmpty treats {1} as empty, so it rejects '{0}' when it is {1}. If {1} is a valid value, use {2}.",
        Category,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "NullOrEmpty counts the default value of a value type as empty: 0 for numbers and false for bool. " +
                     "For a number or flag where that value is valid, say what is invalid instead.",
        helpLinkUri: HelpLink("SGUARD101"));

    public static readonly DiagnosticDescriptor ExceptionInstance = new(
        "SGUARD102",
        "Pass a factory instead of an exception instance",
        "The {0} is created on every call, also when the guard passes. Pass a factory instead: () => new {0}(...).",
        Category,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "An exception passed as an argument is created before the guard runs. A factory runs only when the " +
                     "guard fails, and a lambda that doesn't capture variables allocates nothing.",
        helpLinkUri: HelpLink("SGUARD102"));

    public static readonly DiagnosticDescriptor ReversedBounds = new(
        "SGUARD103",
        "The minimum is greater than the maximum",
        "The minimum {0} is greater than the maximum {1}, so {2} throws ArgumentException on every call",
        Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Between and OutOfRange throw ArgumentException when min is greater than max.",
        helpLinkUri: HelpLink("SGUARD103"));

    public static readonly DiagnosticDescriptor SelectorSelectsObject = new(
        "SGUARD104",
        "The selector selects a whole object",
        "The selector selects '{0}', an object of type '{1}': the guard fails only when it is null or all of its properties " +
        "are empty. Select each member you require, such as {2}.",
        Category,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "A NullOrEmpty selector that selects an object checks all of its readable properties and fails only " +
                     "when every one of them is null or empty, so an object with one empty required property passes.",
        helpLinkUri: HelpLink("SGUARD104"));
}
