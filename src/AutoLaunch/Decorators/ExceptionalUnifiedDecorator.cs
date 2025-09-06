using System.Runtime.CompilerServices;
using System.Security;

namespace AutoLaunch.Decorators;

internal class ExceptionalUnifiedDecorator(AutoLauncher inner) : AutoLauncher
{
    public override void Enable() => ExceptionUnified(inner.Enable);
    public override void Disable() => ExceptionUnified(inner.Disable);
    public override bool GetStatus() => ExceptionUnified(inner.GetStatus);
    public override Task EnableAsync() => ExceptionUnifiedAsync(inner.EnableAsync);
    public override Task DisableAsync() => ExceptionUnifiedAsync(inner.DisableAsync);
    public override Task<bool> GetStatusAsync() => ExceptionUnifiedAsync(inner.GetStatusAsync);

    #region private method
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T ExceptionUnified<T>(Func<T> func)
    {
        try { return func(); }
        // Wrap SecurityException into PermissionDeniedException
        catch (SecurityException secEx) { throw new PermissionDeniedException(innerException: secEx); }
        // Wrap UnauthorizedAccessException into PermissionDeniedException
        catch (UnauthorizedAccessException unAccEx) { throw new PermissionDeniedException(innerException: unAccEx); }
        // Wrap other exceptions into AutoLaunchException
        catch (Exception ex) when (ex is not AutoLaunchException) { throw new AutoLaunchException("An error occurred while executing the operation.", ex); }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static async Task<T> ExceptionUnifiedAsync<T>(Func<Task<T>> func)
    {
        try { return await func(); }
        // Wrap SecurityException into PermissionDeniedException
        catch (SecurityException secEx) { throw new PermissionDeniedException(innerException: secEx); }
        // Wrap UnauthorizedAccessException into PermissionDeniedException
        catch (UnauthorizedAccessException unAccEx) { throw new PermissionDeniedException(innerException: unAccEx); }
        // Wrap other exceptions into AutoLaunchException
        catch (Exception ex) when (ex is not AutoLaunchException) { throw new AutoLaunchException("An error occurred while executing the operation.", ex); }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ExceptionUnified(Action action) => ExceptionUnified(() =>
    {
        action();
        return true;
    });
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Task<bool> ExceptionUnifiedAsync(Func<Task> func) => ExceptionUnifiedAsync(async () =>
    {
        await func();
        return true;
    });
    #endregion
}
