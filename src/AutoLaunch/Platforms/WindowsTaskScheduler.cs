using System.Runtime.CompilerServices;

namespace AutoLaunch.Platforms;

#if NET5_0_OR_GREATER
[SupportedOSPlatform("windows")]
#endif
internal sealed partial class WindowsTaskScheduler(string appName, string appPath, ReadOnlyCollection<string> args) : AutoLauncher
{
    private const string _taskName = "AutoLaunch for $env:USERNAME";
    private static readonly string[] _defaultArgs = ["-NonInteractive", "-NoProfile", "-ExecutionPolicy", "Bypass", "-Command"];
    private readonly string _taskBaseInfo = $"""-TaskPath "\{appName}\" -TaskName "{_taskName}" """;

    public override void Enable() => Exec(GetEnableCmd());
    public override void Disable() => Exec(GetDisableCmd());
    public override bool IsEnabled() => string.Equals("true", Exec(GetIsEnabledCmd()), StringComparison.OrdinalIgnoreCase);


    #region private method
    private static string Exec(string cmd) => ProcessResult(ProcessEx.Start("powershell.exe", [.._defaultArgs, cmd]), cmd);
    private static async Task<string> ExecAsync(string cmd) => ProcessResult(await ProcessEx.StartAsync("powershell.exe", [.._defaultArgs, cmd]), cmd);

    private string GetEnableCmd()
    {
        string argument = args.Count == 0 ? string.Empty : $"""-Argument "{string.Join(" ", args)}" """;
        return $"""Register-ScheduledTask -Force -RunLevel Highest -Trigger (New-ScheduledTaskTrigger -AtLogOn -User $env:USERNAME) {_taskBaseInfo} -Action (New-ScheduledTaskAction -Execute "{appPath}" {argument}) -Description "Auto launch {appName} at user logon" """;
    }
    private string GetDisableCmd() => $$"""if({{GetIsEnabledCmd()}}) {{{GetUnregister()}}}""";
    private string GetIsEnabledCmd() => $"(Get-ScheduledTask {_taskBaseInfo} -ErrorAction SilentlyContinue) -ne $null";
    private string GetUnregister() => $"Unregister-ScheduledTask {_taskBaseInfo} -Confirm:$false";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ProcessResult(ProcessResult pres, string cmd)
    {
        if (pres.ExitCode == 0) return pres.Output.Trim();
        if (pres.Error.Contains("0x80070005")) throw new PermissionDeniedException("Permission denied. (0x80070005)");
        throw new ExecuteCommandException(cmd, pres.ExitCode, pres.Error);
    }
    #endregion
}

internal sealed partial class WindowsTaskScheduler
{
    public override Task EnableAsync() => ExecAsync(GetEnableCmd());
    public override Task DisableAsync() => ExecAsync(GetDisableCmd());
    public override async Task<bool> IsEnabledAsync() => string.Equals("true", await ExecAsync(GetIsEnabledCmd()), StringComparison.OrdinalIgnoreCase);
}
