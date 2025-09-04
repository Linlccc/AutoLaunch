using Microsoft.Win32;

namespace AutoLaunch.Platforms;

#if NET5_0_OR_GREATER
[SupportedOSPlatform("windows")]
#endif
internal sealed partial class WindowsRegistry(string appName, string appPath, ReadOnlyCollection<string> args, WorkScope workScope) : AutoLauncher
{
    private const string _runRegPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string _startupApprovedRegPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";

    private readonly RegistryKey _useRegRoot = workScope == WorkScope.CurrentUser ? Registry.CurrentUser : Registry.LocalMachine;

    public override void Enable() => PermissionDeniedException.ThrowIfIOPermissionDenied(() =>
    {
        using RegistryKey runKey = _useRegRoot.CreateSubKey(_runRegPath, true);
        runKey.SetValue(appName, ArgumentEx.EscapeArguments([appPath, ..args]));

        using RegistryKey startupApprovedKey = _useRegRoot.CreateSubKey(_startupApprovedRegPath, true);
        startupApprovedKey.SetValue(appName, new byte[] { 2, 0, 0, 0, 0, 0, 0, 0 }, RegistryValueKind.Binary);
    });
    public override void Disable() => PermissionDeniedException.ThrowIfIOPermissionDenied(() =>
    {
        using RegistryKey? runKey = _useRegRoot.OpenSubKey(_runRegPath, true);
        runKey?.DeleteValue(appName, false);

        using RegistryKey? startupApprovedKey = _useRegRoot.OpenSubKey(_startupApprovedRegPath, true);
        startupApprovedKey?.DeleteValue(appName, false);
    });
    public override bool IsEnabled() => PermissionDeniedException.ThrowIfIOPermissionDenied(() =>
    {
        using RegistryKey? runKey = _useRegRoot.OpenSubKey(_runRegPath, false);
        if (string.IsNullOrWhiteSpace(runKey?.GetValue(appName) as string)) return false;

        using RegistryKey? startupApprovedKey = _useRegRoot.OpenSubKey(_startupApprovedRegPath, false);
        if (startupApprovedKey?.GetValue(appName) is byte[] bytes) return bytes.FirstOrDefault() == 0x02;

        // If there is no StartupApproved entry, consider it enabled
        return true;
    });
}

internal sealed partial class WindowsRegistry
{
    public override Task EnableAsync() => PermissionDeniedException.ThrowIfIOPermissionDeniedAsync(() => Task.Run(Enable));
    public override Task DisableAsync() => PermissionDeniedException.ThrowIfIOPermissionDeniedAsync(() => Task.Run(Disable));
    public override Task<bool> IsEnabledAsync() => PermissionDeniedException.ThrowIfIOPermissionDeniedAsync(() => Task.Run(IsEnabled));
}
