using System.Collections.ObjectModel;
using AutoLaunch.Exceptions;
using AutoLaunch.Extensions;

namespace AutoLaunch.Platforms;

#if NET5_0_OR_GREATER
[SupportedOSPlatform("windows")]
#endif
internal sealed partial class WindowsTaskScheduler(string appName, string appPath, ReadOnlyCollection<string> args) : AutoLauncher
{
    private const string _taskName = "AutoLaunch for $env:USERNAME";

    private readonly string _taskBaseInfo = $"""-TaskPath "\{appName}\" -TaskName "{_taskName}" """;
    private readonly string _taskExecInfo = $"""-Execute "{appPath}" -Argument "{string.Join(" ", args)}" """;

    public override void Enable() => Exec(GetEnableCmd());
    public override void Disable() => Exec(GetDisableCmd());
    public override bool IsEnabled() => string.Equals("true", Exec(GetIsEnabledCmd()), StringComparison.OrdinalIgnoreCase);


    #region private method
    private string GetEnableCmd() => $"Register-ScheduledTask -Force -RunLevel Highest -Trigger (New-ScheduledTaskTrigger -AtLogOn -User $env:USERNAME) {_taskBaseInfo} -Action (New-ScheduledTaskAction {_taskExecInfo})";
    private string GetDisableCmd() => $"Unregister-ScheduledTask {_taskBaseInfo} -Confirm:$false";
    private string GetIsEnabledCmd() => $"(Get-ScheduledTask {_taskBaseInfo} -ErrorAction SilentlyContinue) -ne $null";

    private static string Exec(string cmd)
    {
        try
        {
            ProcessResult execRes = ProcessEx.Start("powershell.exe", "-NonInteractive", "-NoProfile", "-ExecutionPolicy", "Bypass", "-Command", cmd);
            return execRes.ExitCode == 0 ? execRes.Output.Trim() : throw new ExecuteCommandException(cmd, execRes.ExitCode, execRes.Error);
        }
        catch (Exception ex) when (ex is not ExecuteCommandException)
        {
            throw new ExecuteCommandException(ex);
        }
    }

    private static async Task<string> ExecAsync(string cmd)
    {
        try
        {
            ProcessResult execRes = await ProcessEx.StartAsync("powershell.exe", "-NonInteractive", "-NoProfile", "-ExecutionPolicy", "Bypass", "-Command", cmd);
            return execRes.ExitCode == 0 ? execRes.Output.Trim() : throw new ExecuteCommandException(cmd, execRes.ExitCode, execRes.Error);
        }
        catch (Exception ex) when (ex is not ExecuteCommandException)
        {
            throw new ExecuteCommandException(ex);
        }
    }
    #endregion
}

internal sealed partial class WindowsTaskScheduler
{
    public override Task EnableAsync() => ExecAsync(GetEnableCmd());
    public override Task DisableAsync() => ExecAsync(GetDisableCmd());
    public override async Task<bool> IsEnabledAsync() => string.Equals("true", await ExecAsync(GetIsEnabledCmd()), StringComparison.OrdinalIgnoreCase);
}
