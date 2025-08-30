namespace AutoLaunch;

public enum WindowsEngine
{
    Registry,
    StartupFolder,
    TaskScheduler
}

public enum LinuxEngine
{
    Freedesktop
}

public enum MacOSEngine
{
    LaunchAgent,
    AppleScript
}

public enum WorkScope
{
    CurrentUser,
    AllUser
}
