namespace AutoLaunch;

/// <summary>
/// Base exception for AutoLaunch-related errors.
/// </summary>
public class AutoLaunchException(string message, Exception? innerException = null) : Exception(message, innerException);

/// <summary>
/// Exception thrown during the building of an AutoLaunch instance.
/// </summary>
public class AutoLaunchBuilderException(string message) : AutoLaunchException(message);

/// <summary>
/// Exception thrown when the target operating system is unsupported.
/// </summary>
public class UnsupportedOSException() : AutoLaunchException("Unsupported target os");

/// <summary>
/// Exception thrown when permission is denied to perform an operation.
/// </summary>
public class PermissionDeniedException(string message = "Permission denied.", Exception? innerException = null) : AutoLaunchException(message, innerException);

/// <summary>
/// Exception thrown when executing a command fails.
/// </summary>
public class ExecuteCommandException(string command, int exitCode, string error) : AutoLaunchException($"Failed to execute command: {command}; exit code: {exitCode}; error: {error}")
{
    /// <summary>The command that was attempted.</summary>
    public string Command { get; } = command;
    /// <summary>The exit code returned by the command.</summary>
    public int ExitCode { get; } = exitCode;
    /// <summary>The error message returned by the command.</summary>
    public string Error { get; } = error;
}
