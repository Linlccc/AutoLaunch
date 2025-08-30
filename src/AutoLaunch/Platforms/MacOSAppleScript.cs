using System.Collections.ObjectModel;
using AutoLaunch.Exceptions;
using AutoLaunch.Extensions;

namespace AutoLaunch.Platforms;

#if NET5_0_OR_GREATER
[SupportedOSPlatform("macos")]
#endif
internal sealed partial class MacOSAppleScript(string appName, string appPath, ReadOnlyCollection<string> args) : AutoLauncher
{
    public override void Enable() => Exec(GetEnableScript());
    public override void Disable() => Exec(GetDisableScript());
    public override bool IsEnabled() => string.Equals("true", Exec(GetIsEnabledScript()), StringComparison.OrdinalIgnoreCase);

    #region private method
    private string GetEnableScript()
    {
        bool hidden = args.Any(arg => arg is "--hidden" or "--minimized");
        return SystemEventsTo($$"""make login item at end with properties {name:"{{appName}}",path:"{{appPath}}",hidden:{{hidden.ToString().ToLower()}}}""");
    }
    private string GetDisableScript() => SystemEventsTo($"delete login item \"{appName}\"");
    private string GetIsEnabledScript() => SystemEventsTo($"return exists login item \"{appName}\"");

    private static string SystemEventsTo(string scriptSuffix) => $"""tell application "System Events" to {scriptSuffix}""";

    private static string Exec(string script)
    {
        try
        {
            ProcessResult execRes = ProcessEx.Start("osascript", "-e", script);
            return execRes.ExitCode == 0 ? execRes.Output.Trim() : throw new ExecuteCommandException(script, execRes.ExitCode, execRes.Error);
        }
        catch (Exception ex) when (ex is not ExecuteCommandException)
        {
            throw new ExecuteCommandException(ex);
        }
    }
    private static async Task<string> ExecAsync(string script)
    {
        try
        {
            ProcessResult execRes = await ProcessEx.StartAsync("osascript", "-e", script);
            return execRes.ExitCode == 0 ? execRes.Output.Trim() : throw new ExecuteCommandException(script, execRes.ExitCode, execRes.Error);
        }
        catch (Exception ex) when (ex is not ExecuteCommandException)
        {
            throw new ExecuteCommandException(ex);
        }
    }
    #endregion
}

internal sealed partial class MacOSAppleScript
{
    public override Task EnableAsync() => ExecAsync(GetEnableScript());
    public override Task DisableAsync() => ExecAsync(GetDisableScript());
    public override async Task<bool> IsEnabledAsync() => string.Equals("true", await ExecAsync(GetIsEnabledScript()), StringComparison.OrdinalIgnoreCase);
}
