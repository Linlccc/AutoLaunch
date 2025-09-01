using System.Runtime.CompilerServices;

namespace AutoLaunch.Platforms;

#if NET5_0_OR_GREATER
[SupportedOSPlatform("macos")]
#endif
internal sealed partial class MacOSLaunchAgent(string appName, string appPath, ReadOnlyCollection<string> args, WorkScope workScope, ReadOnlyCollection<string>? identifiers, string? extraConfig) : AutoLauncher
{
    private static readonly string _curUserLaunchAgentsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "LaunchAgents");
    private static readonly string _allUserLaunchAgentsDir = Path.Combine(Path.DirectorySeparatorChar.ToString(), "Library", "LaunchAgents");

    private readonly string _useLaunchAgentsDir = workScope == WorkScope.CurrentUser ? _curUserLaunchAgentsDir : _allUserLaunchAgentsDir;
    private string LaunchAgentFile => Path.Combine(_useLaunchAgentsDir, $"{identifiers?.FirstOrDefault() ?? appName}.plist");

    public override void Enable()
    {
        ThrowIfNotAbsolutePath();
        if (!Directory.Exists(_useLaunchAgentsDir)) Directory.CreateDirectory(_useLaunchAgentsDir);
        File.WriteAllText(LaunchAgentFile, GetLaunchAgentFileContent());
    }
    public override void Disable()
    {
        if (File.Exists(LaunchAgentFile)) File.Delete(LaunchAgentFile);
    }
    public override bool IsEnabled() => File.Exists(LaunchAgentFile);

    #region private method
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfNotAbsolutePath()
    {
        if (!Path.IsPathRooted(appPath)) throw new AutoLaunchException($"App path '{appPath}' is not absolute.");
    }

    private string GetLaunchAgentFileContent()
    {
        string[] programArgs = [appPath, ..args];
        string programArguments = string.Join("", programArgs.Select(arg => $"<string>{arg}</string>"));
        string bundleIdentifiers = string.Join("", identifiers?.Select(id => $"<string>{id}</string>") ?? []);
        return $"""
                <?xml version="1.0" encoding="UTF-8"?>
                <!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
                <plist version="1.0">
                <dict>
                    <key>Label</key>
                    <string>{appName}</string>
                    <key>AssociatedBundleIdentifiers</key>
                    <array>{bundleIdentifiers}</array>
                    <key>ProgramArguments</key>
                    <array>{programArguments}</array>
                    <key>RunAtLoad</key>
                    <true/>
                    {extraConfig}
                </dict>
                </plist>
                """;
    }
    #endregion
}

internal sealed partial class MacOSLaunchAgent
{
    public override Task EnableAsync()
    {
        ThrowIfNotAbsolutePath();
        if (!Directory.Exists(_useLaunchAgentsDir)) Directory.CreateDirectory(_useLaunchAgentsDir);
        return FileEx.WriteAllTextAsync(LaunchAgentFile, GetLaunchAgentFileContent());
    }
    public override Task DisableAsync()
    {
        if (File.Exists(LaunchAgentFile)) File.Delete(LaunchAgentFile);
        return Task.CompletedTask;
    }
    public override Task<bool> IsEnabledAsync() => Task.FromResult(File.Exists(LaunchAgentFile));
}
