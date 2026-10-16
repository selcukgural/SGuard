using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace SGuard.Analyzers.Tests;

/// <summary>
/// Compiles a snippet against the running runtime and SGuard, runs <see cref="SGuardAnalyzer"/> on it, and applies
/// <see cref="ExceptionFactoryCodeFix"/>. Markers <c>[|...|]</c> in the source give the expected diagnostic spans.
/// </summary>
internal static class AnalyzerTest
{
    private static readonly ImmutableArray<MetadataReference> References =
    [
        .. ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path)),
        MetadataReference.CreateFromFile(typeof(ThrowIf).Assembly.Location),
    ];

    private const string Usings = "using System;\nusing System.Collections.Generic;\nusing SGuard;\n";

    /// <summary>
    /// Asserts that the analyzer reports <paramref name="id"/> exactly at the marked spans, and nothing else of its own.
    /// </summary>
    public static async Task Verify(string id, string markedSource)
    {
        var (source, expected) = ParseMarkers(Usings + markedSource);
        var diagnostics = await Analyze(source);

        var actual = diagnostics.Where(d => d.Id.StartsWith("SGUARD1", StringComparison.Ordinal)).ToList();

        Assert.All(actual, d => Assert.Equal(id, d.Id));
        Assert.Equal(expected.OrderBy(s => s.Start), actual.Select(d => d.Location.SourceSpan).OrderBy(s => s.Start));
    }

    /// <summary>
    /// Returns the messages the analyzer reports for the source.
    /// </summary>
    public static async Task<IReadOnlyList<Diagnostic>> Diagnose(string source)
        => (await Analyze(Usings + source)).Where(d => d.Id.StartsWith("SGUARD1", StringComparison.Ordinal)).ToList();

    /// <summary>
    /// Applies every SGUARD102 fix and asserts the result, which must compile without errors.
    /// </summary>
    public static async Task VerifyFix(string source, string expectedSource)
    {
        using var workspace = new AdhocWorkspace();
        var project = workspace.AddProject("Test", LanguageNames.CSharp)
            .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable))
            .WithParseOptions(new CSharpParseOptions(LanguageVersion.Latest))
            .AddMetadataReferences(References);
        var document = project.AddDocument("Test.cs", Usings + source);

        var fix = new ExceptionFactoryCodeFix();

        for (var round = 0; round < 10; round++)
        {
            var compilation = (await document.Project.GetCompilationAsync())!;
            var diagnostic = (await AnalyzeCompilation(compilation)).FirstOrDefault(d => d.Id == "SGUARD102");

            if (diagnostic is null)
            {
                break;
            }

            var actions = new List<CodeAction>();
            await fix.RegisterCodeFixesAsync(new CodeFixContext(document, diagnostic, (a, _) => actions.Add(a), CancellationToken.None));
            var operations = await Assert.Single(actions).GetOperationsAsync(CancellationToken.None);
            var changed = Assert.Single(operations.OfType<ApplyChangesOperation>()).ChangedSolution;
            document = changed.GetDocument(document.Id)!;
        }

        var text = (await document.GetTextAsync()).ToString();
        Assert.Equal(Usings + expectedSource, text);

        var errors = (await document.Project.GetCompilationAsync())!.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error);
        Assert.Empty(errors);
    }

    private static async Task<ImmutableArray<Diagnostic>> Analyze(string source)
    {
        var compilation = CSharpCompilation.Create(
            "Test",
            [CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest))],
            References,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

        var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToList();
        Assert.True(errors.Count == 0, "The test source doesn't compile:\n" + string.Join("\n", errors));

        return await AnalyzeCompilation(compilation);
    }

    private static Task<ImmutableArray<Diagnostic>> AnalyzeCompilation(Compilation compilation)
        => compilation.WithAnalyzers([new SGuardAnalyzer()]).GetAnalyzerDiagnosticsAsync();

    private static (string Source, List<TextSpan> Spans) ParseMarkers(string marked)
    {
        var spans = new List<TextSpan>();
        var source = new System.Text.StringBuilder();
        var starts = new Stack<int>();

        for (var i = 0; i < marked.Length; i++)
        {
            if (marked.AsSpan(i).StartsWith("[|"))
            {
                starts.Push(source.Length);
                i++;
            }
            else if (marked.AsSpan(i).StartsWith("|]"))
            {
                var start = starts.Pop();
                spans.Add(TextSpan.FromBounds(start, source.Length));
                i++;
            }
            else
            {
                source.Append(marked[i]);
            }
        }

        return (source.ToString(), spans);
    }
}
