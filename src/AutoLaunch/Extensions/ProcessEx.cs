using System.Diagnostics;
using AutoLaunch.Compatibility;

namespace AutoLaunch.Extensions;

internal readonly struct ProcessResult(int exitCode, string output, string error)
{
    public int ExitCode => exitCode;
    public string Output => output;
    public string Error => error;
}

internal static class ProcessEx
{
    public static ProcessResult Start(string fileName, Action<ProcessStartInfo>? action, params IEnumerable<string> arguments)
    {
        ProcessStartInfo startInfo = ProcessStartInfoEx.New(fileName, arguments);
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        action?.Invoke(startInfo);

        using Process process = new();
        process.StartInfo = startInfo;

        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();
        return new ProcessResult(process.ExitCode, output, error);
    }
    public static ProcessResult Start(string fileName, params IEnumerable<string> arguments) => Start(fileName, null, arguments);

    public static async Task<ProcessResult> StartAsync(string fileName, Action<ProcessStartInfo>? action, params IEnumerable<string> arguments)
    {
        ProcessStartInfo startInfo = ProcessStartInfoEx.New(fileName, arguments);
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;
        action?.Invoke(startInfo);

        using Process process = new();
        process.StartInfo = startInfo;

        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();
#if NETSTANDARD2_0_OR_GREATER
        process.WaitForExit();
#else
        await process.WaitForExitAsync();
#endif

        return new ProcessResult(process.ExitCode, output, error);
    }
    public static Task<ProcessResult> StartAsync(string fileName, params IEnumerable<string> arguments) => StartAsync(fileName, null, arguments);
}
