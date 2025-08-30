namespace AutoLaunch.Platforms;

internal interface IAutoLauncher
{
    void Enable();
    void Disable();
    bool IsEnabled();
}

internal interface IAsyncAutoLauncher
{
    Task EnableAsync();
    Task DisableAsync();
    Task<bool> IsEnabledAsync();
}
