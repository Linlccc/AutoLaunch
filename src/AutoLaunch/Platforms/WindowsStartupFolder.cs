namespace AutoLaunch.Platforms;

#if NET5_0_OR_GREATER
[SupportedOSPlatform("windows")]
#endif
internal sealed partial class WindowsStartupFolder(string appName, string appPath, ReadOnlyCollection<string> args, WorkScope workScope) : AutoLauncher
{
    private readonly string _useStartupFolder = workScope == WorkScope.CurrentUser ? Environment.GetFolderPath(Environment.SpecialFolder.Startup) : Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);
    private string BatchFilePath => Path.Combine(_useStartupFolder, $"{appName}.bat");

    public override void Enable() => PermissionDeniedException.ThrowIfIOPermissionDenied(() =>
    {
        if (!Directory.Exists(_useStartupFolder)) Directory.CreateDirectory(_useStartupFolder);
        File.WriteAllText(BatchFilePath, GetBatchFileContent());
    });
    public override void Disable() => PermissionDeniedException.ThrowIfIOPermissionDenied(() =>
    {
        if (File.Exists(BatchFilePath)) File.Delete(BatchFilePath);
    });
    public override bool IsEnabled() => PermissionDeniedException.ThrowIfIOPermissionDenied(() => File.Exists(BatchFilePath));

    #region private method
    private string GetBatchFileContent() => $"""
                                             @echo off
                                             REM Auto-generated batch file for auto-launch
                                             cd /d "{Path.GetDirectoryName(appPath)}"
                                             REM start : start "title" /d "exeFolder" "exePath" "arg1" arg2 ...
                                             start "{appName}" /d "{Path.GetDirectoryName(appPath)}" "{appPath}" {ArgumentEx.EscapeArguments(args)}
                                             exit
                                             """;
    #endregion
}

internal sealed partial class WindowsStartupFolder
{
    public override Task EnableAsync() => PermissionDeniedException.ThrowIfIOPermissionDeniedAsync(() =>
    {
        if (!Directory.Exists(_useStartupFolder)) Directory.CreateDirectory(_useStartupFolder);
        return FileEx.WriteAllTextAsync(BatchFilePath, GetBatchFileContent());
    });
    public override Task DisableAsync() => PermissionDeniedException.ThrowIfIOPermissionDeniedAsync(() =>
    {
        if (File.Exists(BatchFilePath)) File.Delete(BatchFilePath);
        return Task.CompletedTask;
    });
    public override Task<bool> IsEnabledAsync() => PermissionDeniedException.ThrowIfIOPermissionDeniedAsync(() => Task.FromResult(File.Exists(BatchFilePath)));
}
