using System.Diagnostics.CodeAnalysis;

namespace AutoLaunch;

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

    public AutoLaunchBuilder SetAppName(string appName)
    {
        _appName = appName;
        return this;
    }

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

    public AutoLaunchBuilder SetWindowsEngine(WindowsEngine engine)
    {
        _windowsEngine = engine;
        return this;
    }

    public AutoLaunchBuilder SetLinuxEngine(LinuxEngine engine)
    {
        _linuxEngine = engine;
        return this;
    }

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

    public AutoLaunchBuilder SetExtraConfigIf(bool condition, string extraConfig)
    {
        if (condition) _extraConfig = extraConfig;
        return this;
    }


    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Handled internally")]
    public AutoLauncher Build()
    {
        if (string.IsNullOrWhiteSpace(_appName)) throw new AutoLaunchBuilderException("AppName is required.");
        if (string.IsNullOrWhiteSpace(_appPath)) throw new AutoLaunchBuilderException("AppPath is required.");
        if (!Path.IsPathRooted(_appPath)) throw new AutoLaunchException($"AppPath '{_appPath}' is not absolute.");
        string appName = _appName!, appPath = _appPath!;
        _args ??= [];

        if (OperatingSystemEx.IsWindows())
        {
            return _windowsEngine switch
            {
                WindowsEngine.Registry => new WindowsRegistry(appName, appPath, _args.AsReadOnly(), _workScope),
                WindowsEngine.StartupFolder => new WindowsStartupFolder(appName, appPath, _args.AsReadOnly(), _workScope),
                WindowsEngine.TaskScheduler => new WindowsTaskScheduler(appName, appPath, _args.AsReadOnly()),
                _ => throw new AutoLaunchBuilderException("Invalid Windows engine.")
            };
        }
        if (OperatingSystemEx.IsLinux())
        {
            return _linuxEngine switch
            {
                LinuxEngine.Freedesktop => new LinuxFreedesktop(appName, appPath, _args.AsReadOnly(), _workScope, _extraConfig),
                _ => throw new AutoLaunchBuilderException("Invalid Linux engine.")
            };
        }
        if (OperatingSystemEx.IsMacOS())
        {
            return _macOSEngine switch
            {
                MacOSEngine.AppleScript => new MacOSAppleScript(Path.GetFileNameWithoutExtension(appPath), appPath, _args.AsReadOnly()),
                MacOSEngine.LaunchAgent => new MacOSLaunchAgent(appName, appPath, _args.AsReadOnly(), _workScope, _identifiers?.AsReadOnly(), _extraConfig),
                _ => throw new AutoLaunchBuilderException("Invalid Windows engine.")
            };
        }

        throw new UnsupportedOSException();
    }
}
