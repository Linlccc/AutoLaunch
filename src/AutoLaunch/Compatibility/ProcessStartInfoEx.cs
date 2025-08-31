namespace AutoLaunch.Compatibility;

internal static class ProcessStartInfoEx
{
    internal static ProcessStartInfo New(string fileName, params IEnumerable<string> arguments)
    {
#if NETSTANDARD2_0_OR_GREATER
        return new ProcessStartInfo(fileName, ArgumentEx.EscapeArguments(arguments.ToArray()));
#else
        return new ProcessStartInfo(fileName, arguments);
#endif
    }
}
