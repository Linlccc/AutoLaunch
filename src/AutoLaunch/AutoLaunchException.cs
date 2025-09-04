namespace AutoLaunch;

public class AutoLaunchException(string message, Exception? innerException = null) : Exception(message, innerException);

public class AutoLaunchBuilderException(string message) : AutoLaunchException(message);

public class UnsupportedOSException() : AutoLaunchException("Unsupported target os");

public class ExecuteCommandException(string command, int exitCode, string error) : AutoLaunchException($"Failed to execute command: {command}; exit code: {exitCode}; error: {error}")
{
    public int ExitCode { get; } = exitCode;
}

public class PermissionDeniedException(string message = "Permission denied.", Exception? innerException = null) : AutoLaunchException(message, innerException);
