namespace AutoLaunch.Decorators;

internal class SafeDecorator(AutoLauncher inner) : SafeAutoLauncher
{
    private AutoLaunchException? _lastException;

    public override void Enable() => inner.Enable();
    public override void Disable() => inner.Disable();
    public override bool IsEnabled() => inner.IsEnabled();
    public override Task EnableAsync() => inner.EnableAsync();
    public override Task DisableAsync() => inner.DisableAsync();
    public override Task<bool> IsEnabledAsync() => inner.IsEnabledAsync();


    public override bool TryEnable() => SafeExecute(Enable);
    public override bool TryDisable() => SafeExecute(Disable);
    public override (bool, bool) TryIsEnabled() => SafeExecute(IsEnabled);
    public override Task<bool> TryEnableAsync() => SafeExecuteAsync(EnableAsync);
    public override Task<bool> TryDisableAsync() => SafeExecuteAsync(DisableAsync);
    public override Task<(bool, bool)> TryIsEnabledAsync() => SafeExecuteAsync(IsEnabledAsync);

    public override Exception? TakeLastException()
    {
        (AutoLaunchException? last, _lastException) = (_lastException, null);
        return last;
    }

    #region private method
    private (bool success, T) SafeExecute<T>(Func<T> func)
    {
        try
        {
            return (true, func());
        }
        catch (AutoLaunchException ex)
        {
            _lastException = ex;
            return (false, default!);
        }
    }
    private async Task<(bool success, T)> SafeExecuteAsync<T>(Func<Task<T>> func)
    {
        try
        {
            return (true, await func());
        }
        catch (AutoLaunchException ex)
        {
            _lastException = ex;
            return (false, default!);
        }
    }

    private bool SafeExecute(Action action) => SafeExecute(() =>
    {
        action();
        return true;
    }).success;
    private async Task<bool> SafeExecuteAsync(Func<Task> func) => (await SafeExecuteAsync(async () =>
    {
        await func();
        return true;
    })).success;
    #endregion
}
