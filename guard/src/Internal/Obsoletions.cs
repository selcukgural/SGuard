namespace SGuard;

/// <summary>
/// Messages and diagnostic IDs of the APIs marked <see cref="ObsoleteAttribute"/>. Each ID has a section in the 3.0
/// upgrade guide that <see cref="UrlFormat"/> links to.
/// </summary>
internal static class Obsoletions
{
    public const string UrlFormat = "https://selcukgural.github.io/SGuard/docs/migration/upgrading-to-3.0#{0}";

    public const string ConstructorArgsDiagnosticId = "SGUARD001";

    public const string ConstructorArgsMessage =
        "Creating the exception from constructor arguments uses reflection, which fails under trimming and Native AOT and " +
        "can't be checked by the compiler. Pass a factory instead, for example () => new MyException(\"message\"). " +
        "This overload will be removed in SGuard 4.0.";

    public const string ExceptionActivatorDiagnosticId = "SGUARD002";

    public const string ExceptionActivatorMessage =
        "ExceptionActivator creates exceptions with reflection, which fails under trimming and Native AOT. Create the " +
        "exception directly, or pass a Func<TException> factory to a guard. This type will be removed in SGuard 4.0.";
}
