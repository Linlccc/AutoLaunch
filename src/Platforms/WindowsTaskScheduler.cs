using System.Runtime.CompilerServices;

namespace AutoLaunch.Platforms;

#if NET5_0_OR_GREATER
[SupportedOSPlatform("windows")]
#endif
internal sealed partial class WindowsTaskScheduler(string appName, string appPath, ReadOnlyCollection<string> args) : AutoLauncher
{
    public override void Enable() => Exec(GetEnableCmd());
    public override void Disable() => Exec(GetDisableCmd());
    public override bool GetStatus() => string.Equals("true", Exec(GetIsEnabledCmd()), StringComparison.OrdinalIgnoreCase);


    public override Task EnableAsync() => ExecAsync(GetEnableCmd());
    public override Task DisableAsync() => ExecAsync(GetDisableCmd());
    public override async Task<bool> GetStatusAsync() => string.Equals("true", await ExecAsync(GetIsEnabledCmd()), StringComparison.OrdinalIgnoreCase);
}

internal sealed partial class WindowsTaskScheduler
{
    private const string _taskName = "AutoLaunch for $env:USERNAME";
    private static readonly string[] _defaultArgs = ["-NonInteractive", "-NoProfile", "-ExecutionPolicy", "Bypass", "-Command"];
    private readonly string _taskBaseInfo = $"""-TaskPath "\{appName}\" -TaskName "{_taskName}" """;

    private string GetEnableCmd()
    {
        string argument = args.Count == 0 ? string.Empty : $"""-Argument "{string.Join(" ", args)}" """;
        return $"""Register-ScheduledTask -Force -RunLevel Highest -Trigger (New-ScheduledTaskTrigger -AtLogOn -User $env:USERNAME) {_taskBaseInfo} -Action (New-ScheduledTaskAction -Execute "{appPath}" {argument}) -Description "Auto launch {appName} at user logon" """;
    }
    private string GetDisableCmd() => $$"""if({{GetIsEnabledCmd()}}) {Unregister-ScheduledTask {{_taskBaseInfo}} -Confirm:$false}""";
    private string GetIsEnabledCmd() => $"(Get-ScheduledTask {_taskBaseInfo} -ErrorAction SilentlyContinue) -ne $null";

    private static string Exec(string cmd) => ProcessResult(ProcessEx.Start("powershell.exe", [.._defaultArgs, cmd]), cmd);
    private static async Task<string> ExecAsync(string cmd) => ProcessResult(await ProcessEx.StartAsync("powershell.exe", [.._defaultArgs, cmd]), cmd);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ProcessResult(ProcessResult pres, string cmd)
    {
        if (pres.ExitCode == 0) return pres.Output.Trim();
        if (pres.Error.Contains("0x80070005")) throw new PermissionDeniedException("Permission denied. (0x80070005)");
        throw new ExecuteCommandException(cmd, pres.ExitCode, pres.Error);
    }
}
