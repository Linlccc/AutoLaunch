using System.Diagnostics.CodeAnalysis;
using AutoLaunch.Decorators;

namespace AutoLaunch;

/// <summary>
/// Builder for creating <see cref="AutoLauncher"/> instances.
/// </summary>
public sealed class AutoLaunchBuilder
{
    #region field
    private string? _appName;
    private string? _appPath;
    private List<string>? _args;
    private WorkScope _workScope = WorkScope.CurrentUser;

    private WindowsEngine _windowsEngine = WindowsEngine.Registry;

    private LinuxEngine _linuxEngine = LinuxEngine.Freedesktop;

    private MacOSEngine _macOSEngine = MacOSEngine.LaunchAgent;
    private List<string>? _identifiers;
    private string? _extraConfig;
    #endregion

    /// <summary>
    /// Automatically configure the builder using the current application's settings.
    /// </summary>
    /// <returns>Returns the current <see cref="AutoLaunchBuilder"/> instance.</returns>
    public AutoLaunchBuilder Automatic()
    {
        _appPath ??= EnvironmentEx.ProcessPath;
        _appName ??= Path.GetFileNameWithoutExtension(_appPath);

        // macOS AppleScript processed into .app
        if (!OperatingSystemEx.IsMacOS() || _macOSEngine != MacOSEngine.AppleScript || _appPath is null) return this;
        int appIdx = _appPath.IndexOf(".app/", StringComparison.OrdinalIgnoreCase);
        if (appIdx != -1) _appPath = _appPath.Substring(0, appIdx + 4);
        return this;
    }

    /// <summary>
    /// Sets the name of the auto-launch item.
    /// </summary>
    /// <param name="appName">The name to set.</param>
    /// <returns>Returns the current <see cref="AutoLaunchBuilder"/> instance.</returns>
    public AutoLaunchBuilder SetAppName(string appName)
    {
        _appName = appName;
        return this;
    }

    /// <summary>
    /// Sets the path of the auto-launch item.
    /// </summary>
    /// <param name="appPath">The path to set.</param>
    /// <returns>Returns the current <see cref="AutoLaunchBuilder"/> instance.</returns>
    public AutoLaunchBuilder SetAppPath(string appPath)
    {
        _appPath = appPath;
        return this;
    }

    /// <summary>
    /// Sets the arguments for the auto-launch item.
    /// </summary>
    /// <param name="args">The arguments to set.</param>
    /// <returns>Returns the current <see cref="AutoLaunchBuilder"/> instance.</returns>
    /// <remarks>
    /// Only --hidden and --minimized arguments are effective in the <see cref="MacOSEngine.AppleScript">AppleScript</see> engine.
    /// </remarks>
    public AutoLaunchBuilder SetArgs(params IEnumerable<string> args)
    {
        _args = args.ToList();
        return this;
    }

    /// <summary>
    /// Adds arguments for the auto-launch item.
    /// </summary>
    /// <param name="args">The arguments to add.</param>
    /// <returns>Returns the current <see cref="AutoLaunchBuilder"/> instance.</returns>
    /// <remarks>
    /// Only --hidden and --minimized arguments are effective in the <see cref="MacOSEngine.AppleScript">AppleScript</see> engine.
    /// </remarks>
    public AutoLaunchBuilder AddArgs(params IEnumerable<string> args)
    {
        _args ??= [];
        _args.AddRange(args);
        return this;
    }

    /// <summary>
    /// Sets the work scope of the auto-launch item. The default value is <see cref="WorkScope.CurrentUser">CurrentUser</see>.
    /// </summary>
    /// <param name="workScope">The work scope of the auto-launch item.</param>
    /// <returns>Returns the current <see cref="AutoLaunchBuilder"/> instance.</returns>
    /// <remarks>
    /// Only effective when using the following engines:
    /// <list type="table">
    ///     <item><see cref="WindowsEngine.Registry"/></item>
    ///     <item><see cref="WindowsEngine.StartupFolder"/></item>
    ///     <item><see cref="LinuxEngine.Freedesktop"/></item>
    ///     <item><see cref="MacOSEngine.LaunchAgent"/></item>
    /// </list>
    /// Using <see cref="WorkScope.AllUser"/> may require elevated system privileges.
    /// </remarks>
    public AutoLaunchBuilder SetWorkScope(WorkScope workScope)
    {
        _workScope = workScope;
        return this;
    }

    /// <summary>
    /// Sets the Windows engine type for the auto-launch item. The default value is <see cref="WindowsEngine.Registry">Registry</see>.
    /// </summary>
    /// <param name="engine">The Windows engine type to set.</param>
    /// <returns>Returns the current <see cref="AutoLaunchBuilder"/> instance.</returns>
    public AutoLaunchBuilder SetWindowsEngine(WindowsEngine engine)
    {
        _windowsEngine = engine;
        return this;
    }

    /// <summary>
    /// Sets the Linux engine type for the auto-launch item. The default value is <see cref="LinuxEngine.Freedesktop">Freedesktop</see>.
    /// </summary>
    /// <param name="engine">The Linux engine type to set.</param>
    /// <returns>Returns the current <see cref="AutoLaunchBuilder"/> instance.</returns>
    public AutoLaunchBuilder SetLinuxEngine(LinuxEngine engine)
    {
        _linuxEngine = engine;
        return this;
    }

    /// <summary>
    /// Sets the macOS engine type for the auto-launch item. The default value is <see cref="MacOSEngine.LaunchAgent">LaunchAgent</see>.
    /// </summary>
    /// <param name="engine">The macOS engine type to set.</param>
    /// <returns>Returns the current <see cref="AutoLaunchBuilder"/> instance.</returns>
    public AutoLaunchBuilder SetMacOSEngine(MacOSEngine engine)
    {
        _macOSEngine = engine;
        return this;
    }

    /// <summary>
    /// Sets the identifiers for the auto-launch item.
    /// </summary>
    /// <param name="identifiers">The identifiers to set.</param>
    /// <returns>Returns the current <see cref="AutoLaunchBuilder"/> instance.</returns>
    /// <remarks>
    /// Only effective when using the following engines:
    /// <list type="table">
    ///     <item><see cref="MacOSEngine.LaunchAgent"/></item>
    /// </list>
    /// </remarks>
    public AutoLaunchBuilder SetIdentifiers(params IEnumerable<string> identifiers)
    {
        _identifiers = identifiers.ToList();
        return this;
    }

    /// <summary>
    /// Adds identifiers for the auto-launch item.
    /// </summary>
    /// <param name="identifiers">The identifiers to add.</param>
    /// <returns>Returns the current <see cref="AutoLaunchBuilder"/> instance.</returns>
    /// <remarks>
    /// Only effective when using the following engines:
    /// <list type="table">
    ///     <item><see cref="MacOSEngine.LaunchAgent"/></item>
    /// </list>
    /// </remarks>
    public AutoLaunchBuilder AddIdentifiers(params IEnumerable<string> identifiers)
    {
        _identifiers ??= [];
        _identifiers.AddRange(identifiers);
        return this;
    }

    /// <summary>
    /// Sets extra configuration for the auto-launch item.
    /// </summary>
    /// <param name="extraConfig">extra configuration</param>
    /// <returns>Returns the current <see cref="AutoLaunchBuilder"/> instance.</returns>
    /// <remarks>
    /// Only effective when using the following engines:
    /// <list type="table">
    ///     <item><see cref="MacOSEngine.LaunchAgent"/></item>
    ///     <item><see cref="LinuxEngine.Freedesktop"/></item>
    /// </list>
    /// The format of the extra configuration should comply with the specifications of the respective engine.
    /// </remarks>
    public AutoLaunchBuilder SetExtraConfig(string extraConfig)
    {
        _extraConfig = extraConfig;
        return this;
    }

    /// <summary>
    /// Conditionally sets extra configuration for the auto-launch item.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="extraConfig">extra configuration</param>
    /// <returns>Returns the current <see cref="AutoLaunchBuilder"/> instance.</returns>
    /// <remarks>
    /// Only effective when using the following engines:
    /// <list type="table">
    ///     <item><see cref="MacOSEngine.LaunchAgent"/></item>
    ///     <item><see cref="LinuxEngine.Freedesktop"/></item>
    /// </list>
    /// The format of the extra configuration should comply with the specifications of the respective engine.
    /// </remarks>
    public AutoLaunchBuilder SetExtraConfigIf(bool condition, string extraConfig)
    {
        if (condition) _extraConfig = extraConfig;
        return this;
    }


    /// <summary>
    /// Builds and returns an instance of <see cref="AutoLauncher"/> based on the configured settings.
    /// </summary>
    /// <returns>An instance of <see cref="AutoLauncher"/>.</returns>
    /// <exception cref="AutoLaunchBuilderException">Thrown when required parameters are missing or invalid.</exception>
    /// <exception cref="UnsupportedOSException">Thrown when the operating system is unsupported.</exception>
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Handled internally")]
    public AutoLauncher Build()
    {
        if (string.IsNullOrWhiteSpace(_appName)) throw new AutoLaunchBuilderException("AppName is required.");
        if (string.IsNullOrWhiteSpace(_appPath)) throw new AutoLaunchBuilderException("AppPath is required.");
        if (!Path.IsPathRooted(_appPath)) throw new AutoLaunchBuilderException($"AppPath '{_appPath}' is not absolute.");
        string appName = _appName!, appPath = _appPath!;
        _args ??= [];

        AutoLauncher platformLauncher;
        if (OperatingSystemEx.IsWindows())
        {
            platformLauncher = _windowsEngine switch
            {
                WindowsEngine.Registry => new WindowsRegistry(appName, appPath, _args.AsReadOnly(), _workScope),
                WindowsEngine.StartupFolder => new WindowsStartupFolder(appName, appPath, _args.AsReadOnly(), _workScope),
                WindowsEngine.TaskScheduler => new WindowsTaskScheduler(appName, appPath, _args.AsReadOnly()),
                _ => throw new AutoLaunchBuilderException("Invalid Windows engine.")
            };
        }
        else if (OperatingSystemEx.IsLinux())
        {
            platformLauncher = _linuxEngine switch
            {
                LinuxEngine.Freedesktop => new LinuxFreedesktop(appName, appPath, _args.AsReadOnly(), _workScope, _extraConfig),
                _ => throw new AutoLaunchBuilderException("Invalid Linux engine.")
            };
        }
        else if (OperatingSystemEx.IsMacOS())
        {
            platformLauncher = _macOSEngine switch
            {
                MacOSEngine.AppleScript => new MacOSAppleScript(Path.GetFileNameWithoutExtension(appPath), appPath, _args.AsReadOnly()),
                MacOSEngine.LaunchAgent => new MacOSLaunchAgent(appName, appPath, _args.AsReadOnly(), _workScope, _identifiers?.AsReadOnly(), _extraConfig),
                _ => throw new AutoLaunchBuilderException("Invalid Windows engine.")
            };
        }
        else throw new UnsupportedOSException();
        return new ExceptionalUnifiedDecorator(platformLauncher);
    }

    /// <summary>
    /// Builds and returns a <see cref="SafeAutoLauncher"/> instance that does not throw exceptions.
    /// </summary>
    /// <returns>A <see cref="SafeAutoLauncher"/> instance.</returns>
    public SafeAutoLauncher BuildSafe() => new SafeDecorator(Build());
}
