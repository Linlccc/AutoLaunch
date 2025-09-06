namespace AutoLaunch.Compatibility;

internal static class EnvironmentEx
{
#if NETSTANDARD2_0_OR_GREATER
    public static string? ProcessPath => Process.GetCurrentProcess().MainModule?.FileName;
#else
    public static string? ProcessPath => Environment.ProcessPath;
#endif
}
