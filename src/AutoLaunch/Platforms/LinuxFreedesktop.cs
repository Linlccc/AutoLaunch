using System.Collections.ObjectModel;
using AutoLaunch.Compatibility;

namespace AutoLaunch.Platforms;

#if NET5_0_OR_GREATER
[SupportedOSPlatform("linux")]
#endif
internal sealed partial class LinuxFreedesktop(string appName, string appPath, ReadOnlyCollection<string> args, WorkScope workScope, string? extraConfig) : AutoLauncher
{
    private static readonly string _curUserAutoStartDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "autostart");
    private static readonly string _allUserAutoStartDir = Path.Combine("etc", "xdg", "autostart");

    private readonly string _useAutoStartDir = workScope == WorkScope.CurrentUser ? _curUserAutoStartDir : _allUserAutoStartDir;
    private string AutoStartFile => Path.Combine(_useAutoStartDir, $"{appName}.desktop");

    public override void Enable()
    {
        if (!Directory.Exists(_useAutoStartDir)) Directory.CreateDirectory(_useAutoStartDir);
        File.WriteAllText(AutoStartFile, GetDesktopFileContent());
    }
    public override void Disable()
    {
        if (File.Exists(AutoStartFile)) File.Delete(AutoStartFile);
    }
    public override bool IsEnabled() => File.Exists(AutoStartFile);

    #region private method
    private string GetDesktopFileContent() => $"""
                                               [Desktop Entry]
                                               Type=Application
                                               Name={appName}
                                               Exec={appPath} {string.Join(" ", args)}
                                               StartupNotify=false
                                               Terminal=false
                                               Comment={appName} startup
                                               {extraConfig}
                                               """;
    #endregion
}

internal sealed partial class LinuxFreedesktop
{
    public override Task EnableAsync()
    {
        if (!Directory.Exists(_useAutoStartDir)) Directory.CreateDirectory(_useAutoStartDir);
        return FileEx.WriteAllTextAsync(AutoStartFile, GetDesktopFileContent());
    }
    public override Task DisableAsync()
    {
        if (File.Exists(AutoStartFile)) File.Delete(AutoStartFile);
        return Task.CompletedTask;
    }
    public override Task<bool> IsEnabledAsync() => Task.FromResult(File.Exists(AutoStartFile));
}
