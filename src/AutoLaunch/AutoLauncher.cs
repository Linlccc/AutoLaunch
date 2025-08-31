namespace AutoLaunch;

public abstract class AutoLauncher : IAutoLauncher, IAsyncAutoLauncher
{
    public abstract void Enable();
    public abstract void Disable();
    public abstract bool IsEnabled();

    public abstract Task EnableAsync();
    public abstract Task DisableAsync();
    public abstract Task<bool> IsEnabledAsync();
}
