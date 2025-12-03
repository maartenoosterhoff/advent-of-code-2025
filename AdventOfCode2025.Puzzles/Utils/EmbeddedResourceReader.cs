using System.Reflection;

namespace AdventOfCode2025.Puzzles.Utils;

internal class EmbeddedResourceReader
{
    public static string[] Read<T>(string filename)
    {
        var folder = typeof(T).Namespace!.Replace("AdventOfCode2025.Puzzles.", string.Empty);

        var lines = Read(folder, filename);

        return lines
            .Replace(Environment.NewLine, "\n")
            .Replace("\r", "\n")
            .Split("\n")
            .ToArray();
    }

    private static string Read(string folder, string filename)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = $"{typeof(EmbeddedResourceReader).Namespace?.Replace(".Utils", "")}.{folder}.{filename}.txt";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream!);
        var result = reader.ReadToEnd();
        return result;
    }
}