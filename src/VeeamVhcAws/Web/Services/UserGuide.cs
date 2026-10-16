using Markdig;

namespace VeeamVhcAws.Web.Services;

public static class UserGuide
{
    public const string ResourceName = "VeeamVhcAws.docs.USER_GUIDE.md";

    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .DisableHtml()
        .UsePipeTables()
        .UseAutoIdentifiers()
        .Build();

    private static readonly Lazy<string> Cached = new(() => Render(LoadMarkdown()));

    public static string Html => Cached.Value;

    public static string Render(string markdown) => Markdown.ToHtml(markdown, Pipeline);

    public static string LoadMarkdown()
    {
        using var stream = typeof(UserGuide).Assembly.GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException($"Embedded resource {ResourceName} is missing.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
