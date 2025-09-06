namespace AutoLaunch;

/// <summary>
/// Windows auto-launch engine types.
/// </summary>
public enum WindowsEngine
{
    /// <summary>
    /// Use Windows Registry for auto-launch.
    /// </summary>
    Registry,
    /// <summary>
    /// Use Startup Folder for auto-launch.
    /// </summary>
    StartupFolder,
    /// <summary>
    /// Use Task Scheduler for auto-launch.
    /// </summary>
    TaskScheduler
}

/// <summary>
/// Linux auto-launch engine types.
/// </summary>
public enum LinuxEngine
{
    /// <summary>
    /// Use Freedesktop standard (.desktop file) for auto-launch.
    /// </summary>
    Freedesktop
}

/// <summary>
/// MacOS auto-launch engine types.
/// </summary>
public enum MacOSEngine
{
    /// <summary>
    /// Use LaunchAgent (launchd) for auto-launch.
    /// </summary>
    LaunchAgent,
    /// <summary>
    /// Use AppleScript for auto-launch.
    /// </summary>
    AppleScript
}

/// <summary>
/// The scope of the auto-launch operation.
/// </summary>
public enum WorkScope
{
    /// <summary>
    /// Current user only.
    /// </summary>
    CurrentUser,
    /// <summary>
    /// All users.
    /// </summary>
    AllUser
}
