namespace Suprema.Content.Services;

internal static class MarkdownContentLoader
{
    /// <summary>
    /// Splits a Markdown file into YAML front-matter key-value pairs and the body text.
    /// Front matter must be fenced with --- on its own line.
    /// </summary>
    internal static (Dictionary<string, string> Meta, string Body) ParseFrontMatter(string fileContent)
    {
        var meta = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (!fileContent.StartsWith("---", StringComparison.Ordinal))
            return (meta, fileContent);

        // Find the closing ---
        var closingStart = fileContent.IndexOf("\n---", 3, StringComparison.Ordinal);
        if (closingStart < 0)
            return (meta, fileContent);

        var frontMatter = fileContent[4..closingStart];
        var body = fileContent[(closingStart + 4)..].TrimStart('\r', '\n');

        foreach (var rawLine in frontMatter.Split('\n'))
        {
            var line = rawLine.TrimEnd('\r');
            var colonIdx = line.IndexOf(':', StringComparison.Ordinal);
            if (colonIdx <= 0) continue;

            var key = line[..colonIdx].Trim();
            var value = line[(colonIdx + 1)..].Trim().Trim('"');
            if (!string.IsNullOrEmpty(key))
                meta[key] = value;
        }

        return (meta, body);
    }
}
