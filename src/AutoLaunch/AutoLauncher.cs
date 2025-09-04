namespace AutoLaunch;

public interface IAutoLauncher
{
    void Enable();
    void Disable();
    bool IsEnabled();
}

public interface IAsyncAutoLauncher
{
    Task EnableAsync();
    Task DisableAsync();
    Task<bool> IsEnabledAsync();
}

public abstract class AutoLauncher : IAutoLauncher, IAsyncAutoLauncher
{
    public abstract void Enable();
    public abstract void Disable();
    public abstract bool IsEnabled();

    public abstract Task EnableAsync();
    public abstract Task DisableAsync();
    public abstract Task<bool> IsEnabledAsync();
}

public abstract class SafeAutoLauncher : AutoLauncher
{
    public abstract bool TryEnable();
    public abstract bool TryDisable();
    public abstract (bool success, bool enabled) TryIsEnabled();

    public abstract Task<bool> TryEnableAsync();
    public abstract Task<bool> TryDisableAsync();
    public abstract Task<(bool success, bool enabled)> TryIsEnabledAsync();

    public abstract Exception? TakeLastException();
}
