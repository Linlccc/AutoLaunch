using System.Runtime.CompilerServices;

namespace AutoLaunch.Platforms;

#if NET5_0_OR_GREATER
[SupportedOSPlatform("macos")]
#endif
internal sealed partial class MacOSAppleScript(string appName, string appPath, ReadOnlyCollection<string> args) : AutoLauncher
{
    public override void Enable() => Exec(GetEnableScript());
    public override void Disable() => Exec(GetDisableScript());
    public override bool IsEnabled() => string.Equals("true", Exec(GetIsEnabledScript()), StringComparison.OrdinalIgnoreCase);


    public override Task EnableAsync() => ExecAsync(GetEnableScript());
    public override Task DisableAsync() => ExecAsync(GetDisableScript());
    public override async Task<bool> IsEnabledAsync() => string.Equals("true", await ExecAsync(GetIsEnabledScript()), StringComparison.OrdinalIgnoreCase);
}

internal sealed partial class MacOSAppleScript
{
    private string GetEnableScript() => SystemEventsTo($$"""make login item at end with properties {name:"{{appName}}",path:"{{appPath}}",hidden:{{args.Any(arg => arg is "--hidden" or "--minimized").ToString().ToLower()}}}""");
    private string GetDisableScript() => SystemEventsTo($"delete login item \"{appName}\"");
    private string GetIsEnabledScript() => SystemEventsTo($"return exists login item \"{appName}\"");
    private static string SystemEventsTo(string scriptSuffix) => $"""tell application "System Events" to {scriptSuffix}""";

    private static string Exec(string script) => ProcessResult(ProcessEx.Start("osascript", "-e", script), script);
    private static async Task<string> ExecAsync(string script) => ProcessResult(await ProcessEx.StartAsync("osascript", "-e", script), script);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ProcessResult(ProcessResult pres, string script)
    {
        if (pres.ExitCode == 0) return pres.Output.Trim();
        if (pres.Error.Contains("-1743")) throw new PermissionDeniedException("Permission denied. (-1743)");
        throw new ExecuteCommandException(script, pres.ExitCode, pres.Error);
    }
}
