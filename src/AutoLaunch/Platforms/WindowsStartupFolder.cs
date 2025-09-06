namespace AutoLaunch.Platforms;

#if NET5_0_OR_GREATER
[SupportedOSPlatform("windows")]
#endif
internal sealed partial class WindowsStartupFolder(string appName, string appPath, ReadOnlyCollection<string> args, WorkScope workScope) : AutoLauncher
{
    public override void Enable()
    {
        if (!Directory.Exists(_useStartupFolder)) Directory.CreateDirectory(_useStartupFolder);
        File.WriteAllText(BatchFilePath, GetBatchFileContent());
    }
    public override void Disable()
    {
        if (File.Exists(BatchFilePath)) File.Delete(BatchFilePath);
    }
    public override bool GetStatus() => File.Exists(BatchFilePath);


    public override Task EnableAsync()
    {
        if (!Directory.Exists(_useStartupFolder)) Directory.CreateDirectory(_useStartupFolder);
        return FileEx.WriteAllTextAsync(BatchFilePath, GetBatchFileContent());
    }
    public override Task DisableAsync()
    {
        if (File.Exists(BatchFilePath)) File.Delete(BatchFilePath);
        return Task.CompletedTask;
    }
    public override Task<bool> GetStatusAsync() => Task.FromResult(File.Exists(BatchFilePath));
}

internal sealed partial class WindowsStartupFolder
{
    private readonly string _useStartupFolder = workScope == WorkScope.CurrentUser ? Environment.GetFolderPath(Environment.SpecialFolder.Startup) : Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);

    private string BatchFilePath => Path.Combine(_useStartupFolder, $"{appName}.bat");

    private string GetBatchFileContent() =>
        $"""
         @echo off
         REM Auto-generated batch file for auto-launch
         cd /d "{Path.GetDirectoryName(appPath)}"
         REM start : start "title" /d "exeFolder" "exePath" "arg1" arg2 ...
         start "{appName}" /d "{Path.GetDirectoryName(appPath)}" {ArgumentEx.EscapeArguments([appPath, ..args])}
         exit
         """;
}
