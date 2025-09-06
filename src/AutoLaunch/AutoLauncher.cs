namespace AutoLaunch;

/// <summary>
/// Interface for providing auto-launch functionality.
/// </summary>
public interface IAutoLauncher
{
    /// <summary>
    /// Enables the auto-launch feature.
    /// </summary>
    void Enable();

    /// <summary>
    /// Disables the auto-launch feature.
    /// </summary>
    void Disable();

    /// <summary>
    /// Checks whether the auto-launch feature is enabled.
    /// </summary>
    /// <returns>True if enabled; otherwise, false.</returns>
    bool IsEnabled();
}

/// <summary>
/// Interface for providing asynchronous auto-launch functionality.
/// </summary>
public interface IAsyncAutoLauncher
{
    /// <summary>
    /// Enables the auto-launch feature asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task EnableAsync();

    /// <summary>
    /// Disables the auto-launch feature asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DisableAsync();

    /// <summary>
    /// Checks whether the auto-launch feature is enabled asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The result indicates whether it is enabled.</returns>
    Task<bool> IsEnabledAsync();
}

/// <summary>
/// Abstract base class for auto-launch functionality.
/// </summary>
public abstract class AutoLauncher : IAutoLauncher, IAsyncAutoLauncher
{
    /// <summary>
    /// Enables the auto-launch feature.
    /// </summary>
    public abstract void Enable();

    /// <summary>
    /// Disables the auto-launch feature.
    /// </summary>
    public abstract void Disable();

    /// <summary>
    /// Checks whether the auto-launch feature is enabled.
    /// </summary>
    /// <returns>True if enabled; otherwise, false.</returns>
    public abstract bool IsEnabled();

    /// <summary>
    /// Enables the auto-launch feature asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public abstract Task EnableAsync();

    /// <summary>
    /// Disables the auto-launch feature asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public abstract Task DisableAsync();

    /// <summary>
    /// Checks whether the auto-launch feature is enabled asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The result indicates whether it is enabled.</returns>
    public abstract Task<bool> IsEnabledAsync();

    /// <summary>
    /// Checks whether the current operating system is supported for auto-launch functionality.
    /// </summary>
    /// <returns>True if supported; otherwise, false.</returns>
    public static bool IsSupported() => OperatingSystemEx.IsWindows() || OperatingSystemEx.IsLinux() || OperatingSystemEx.IsMacOS();
}

/// <summary>
/// Abstract base class for safe auto-launch.
/// None of the methods should throw exceptions; exception information is provided via <see cref="SafeAutoLauncher.TakeLastException"/>.
/// </summary>
public abstract class SafeAutoLauncher : AutoLauncher
{
    /// <summary>
    /// Attempts to enable auto-launch.
    /// </summary>
    /// <returns>Returns true if enabling was successful; otherwise, false.</returns>
    public abstract bool TryEnable();

    /// <summary>
    /// Attempts to disable auto-launch.
    /// </summary>
    /// <returns>Returns true if disabling was successful; otherwise, false.</returns>
    public abstract bool TryDisable();

    /// <summary>
    /// Attempts to check whether auto-launch is enabled.
    /// </summary>
    /// <returns>
    /// success indicates whether the operation was successful;
    /// enabled indicates whether auto-launch is currently enabled.
    /// </returns>
    public abstract (bool success, bool enabled) TryIsEnabled();

    /// <summary>
    /// Asynchronously attempts to enable auto-launch.
    /// </summary>
    /// <returns>A task whose result is whether enabling was successful.</returns>
    public abstract Task<bool> TryEnableAsync();

    /// <summary>
    /// Asynchronously attempts to disable auto-launch.
    /// </summary>
    /// <returns>A task whose result is whether disabling was successful.</returns>
    public abstract Task<bool> TryDisableAsync();

    /// <summary>
    /// Asynchronously attempts to check whether auto-launch is enabled.
    /// </summary>
    /// <returns>
    /// A task whose result is a tuple:
    /// success indicates whether the operation was successful;
    /// enabled indicates whether auto-launch is currently enabled.
    /// </returns>
    public abstract Task<(bool success, bool enabled)> TryIsEnabledAsync();

    /// <summary>
    /// Gets the exception from the most recent operation, or null if there was no exception.
    /// </summary>
    /// <returns>The most recent exception object, or null if there was no exception.</returns>
    public abstract Exception? TakeLastException();
}
