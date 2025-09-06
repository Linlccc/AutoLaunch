namespace AutoLaunch.Compatibility;

internal static class FileEx
{
    public static async Task WriteAllTextAsync(string path, string? contents, CancellationToken cancellationToken = default)
    {
#if NETSTANDARD2_0
        await Task.Run(() => File.WriteAllText(path, contents), cancellationToken);
#else
        await File.WriteAllTextAsync(path, contents, cancellationToken);
#endif
    }
}
