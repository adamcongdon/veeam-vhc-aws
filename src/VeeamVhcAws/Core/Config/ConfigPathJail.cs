namespace VeeamVhcAws.Core.Config;

public static class ConfigPathJail
{
    public const string OutsideMessage = "Path is outside the config directory.";

    public static string Resolve(string configFilePath, string candidate)
    {
        var full = Lexical(configFilePath, candidate);
        var root = Path.GetDirectoryName(Path.GetFullPath(configFilePath));
        if (string.IsNullOrEmpty(root) || !IsInside(RealPath(root), RealPath(full)))
            throw new InvalidOperationException(OutsideMessage);
        return full;
    }

    private static string Lexical(string configPath, string path)
    {
        if (Path.IsPathRooted(path))
            return Path.GetFullPath(path);
        var dir = Path.GetDirectoryName(Path.GetFullPath(configPath));
        if (string.IsNullOrEmpty(dir))
            return Path.GetFullPath(path);
        return Path.GetFullPath(Path.Combine(dir, path));
    }

    private static bool IsInside(string root, string candidate)
    {
        var comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
        root = Path.TrimEndingDirectorySeparator(Path.GetFullPath(root));
        candidate = Path.TrimEndingDirectorySeparator(Path.GetFullPath(candidate));
        if (string.Equals(root, candidate, comparison))
            return true;
        return candidate.StartsWith(root + Path.DirectorySeparatorChar, comparison);
    }

    private static string RealPath(string path)
    {
        var full = Path.GetFullPath(path);
        var missing = new Stack<string>();
        var current = full;
        while (!Exists(current))
        {
            var name = Path.GetFileName(current);
            if (string.IsNullOrEmpty(name))
                break;
            missing.Push(name);
            var parent = Path.GetDirectoryName(current);
            if (string.IsNullOrEmpty(parent) || parent == current)
                break;
            current = parent;
        }

        current = ResolveExisting(current);
        while (missing.Count > 0)
            current = Path.Combine(current, missing.Pop());
        return Path.GetFullPath(current);
    }

    private static bool Exists(string path) => File.Exists(path) || Directory.Exists(path);

    private static string ResolveExisting(string path)
    {
        var seen = new HashSet<string>(OperatingSystem.IsWindows()
            ? StringComparer.OrdinalIgnoreCase
            : StringComparer.Ordinal);
        var current = Path.GetFullPath(path);
        while (seen.Add(current))
        {
            string? link = null;
            if (Directory.Exists(current))
                link = new DirectoryInfo(current).LinkTarget;
            else if (File.Exists(current))
                link = new FileInfo(current).LinkTarget;
            if (string.IsNullOrEmpty(link))
                return current;
            var next = Path.IsPathRooted(link)
                ? link
                : Path.Combine(Path.GetDirectoryName(current)!, link);
            current = Path.GetFullPath(next);
        }

        throw new InvalidOperationException(OutsideMessage);
    }
}
