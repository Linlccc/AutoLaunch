using System.Runtime.CompilerServices;

namespace AutoLaunch.Exceptions;

public class AutoLaunchException(string message, Exception? ex = null) : Exception(message, ex);

public class AutoLaunchBuilderException(string message) : AutoLaunchException(message);

public class UnsupportedOSException() : AutoLaunchException("Unsupported target os");

public class ExecuteCommandException : AutoLaunchException
{
    public ExecuteCommandException(Exception ex) : base("Error to execute command.", ex) { }
    public ExecuteCommandException(string command, int exitCode, string error) : base($"Failed to execute command: {command}, exit code: {exitCode}, error: {error}") => ExitCode = exitCode;

    public int? ExitCode { get; }
}

public class PermissionDeniedException(string message, Exception? ex = null) : AutoLaunchException(message, ex)
{
    public PermissionDeniedException(Exception? ex = null) : this("Permission denied.", ex) { }

    internal static void ThrowIfIOPermissionDenied(Action action)
    {
        try { action(); }
        catch (UnauthorizedAccessException ex) { throw new PermissionDeniedException(ex); }
    }
    internal static T ThrowIfIOPermissionDenied<T>(Func<T> func)
    {
        try { return func(); }
        catch (UnauthorizedAccessException ex) { throw new PermissionDeniedException(ex); }
    }

    internal static async Task ThrowIfIOPermissionDeniedAsync(Func<Task> func)
    {
        try { await func(); }
        catch (UnauthorizedAccessException ex) { throw new PermissionDeniedException(ex); }
    }
    internal static async Task<T> ThrowIfIOPermissionDeniedAsync<T>(Func<Task<T>> func)
    {
        try { return await func(); }
        catch (UnauthorizedAccessException ex) { throw new PermissionDeniedException(ex); }
    }
}
