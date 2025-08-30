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
