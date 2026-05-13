using System.Text.Json;

namespace Suprema.Content.Services;

internal static class JsonContentLoader
{
    /// <summary>
    /// Deserializes all *.json files in <paramref name="directory"/> (non-recursive)
    /// matching <paramref name="predicate"/> into type <typeparamref name="T"/>.
    /// </summary>
    internal static IEnumerable<T> LoadDirectory<T>(
        string directory,
        JsonSerializerOptions opts,
        Func<string, bool>? predicate = null)
    {
        if (!Directory.Exists(directory))
            yield break;

        foreach (var file in Directory.EnumerateFiles(directory, "*.json", SearchOption.TopDirectoryOnly))
        {
            if (predicate is not null && !predicate(file))
                continue;

            var json = File.ReadAllText(file);
            var item = JsonSerializer.Deserialize<T>(json, opts)
                ?? throw new InvalidDataException($"Null result deserializing {file}");
            yield return item;
        }
    }

    /// <summary>
    /// Deserializes all *.json files in all immediate sub-directories of <paramref name="root"/>
    /// matching <paramref name="predicate"/>, yielding the sub-directory name alongside each item.
    /// </summary>
    internal static IEnumerable<(string SubDir, T Item)> LoadSubDirectories<T>(
        string root,
        JsonSerializerOptions opts,
        Func<string, bool>? predicate = null)
    {
        if (!Directory.Exists(root))
            yield break;

        foreach (var dir in Directory.EnumerateDirectories(root))
        {
            var subDir = Path.GetFileName(dir);
            foreach (var item in LoadDirectory<T>(dir, opts, predicate))
                yield return (subDir, item);
        }
    }
}
