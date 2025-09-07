# AutoLaunch

![AutoLaunch](https://raw.githubusercontent.com/Linlccc/AutoLaunch/master/docs/icon/icon-128x128.png)

[![GitHub repo size](https://img.shields.io/github/repo-size/Linlccc/AutoLaunch)](https://github.com/Linlccc/AutoLaunch)
[![GitHub License](https://img.shields.io/github/license/Linlccc/AutoLaunch)](https://github.com/Linlccc/AutoLaunch/blob/master/LICENSE)
[![NuGet Version](https://img.shields.io/nuget/v/AutoLaunch?label=AutoLaunch&logo=dotnet)](https://www.nuget.org/packages/AutoLaunch)
[![NuGet Downloads](https://img.shields.io/nuget/dt/AutoLaunch?label=AutoLaunch)](https://www.nuget.org/packages/AutoLaunch)
[![AutoLaunchTestTool](https://img.shields.io/badge/AutoLaunchTestTool-0D6EFD)](https://github.com/Linlccc/AutoLaunchTestTool)

English | [简体中文](https://github.com/Linlccc/AutoLaunch/blob/master/README-ZH_CN.md)

[AutoLaunch](https://github.com/Linlccc/AutoLaunch) provides the ability to automatically run any application or executable at startup or login, supporting Windows, Linux, and macOS systems.

[AutoLaunchTestTool](https://github.com/Linlccc/AutoLaunchTestTool) is a graphical test tool for this project based on [Avalonia](https://avaloniaui.net/), helping you quickly verify and experiment with auto-launch functionality.

## ✨ Features

- 🌍 **Cross-Platform Support**: Windows, Linux, macOS
- 🔧 **Multiple Engines**: Multiple implementations for each platform
- 🎯 **Ease of Use**: Unified API for all platforms
- 🧱 **AOT Support**: Fully supports AOT and trimming
- 📦 **Zero Dependencies**: No third-party library dependencies

## 📦 Installation

### .NET CLI

```bash
dotnet add package AutoLaunch
```

### Package Manager Console

```powershell
Install-Package AutoLaunch
```

## 🚀 Quick Start

`AutoLaunchBuilder` provides a unified configuration API for all platforms, eliminating constructor differences and enabling multi-platform configuration.

### Basic Usage

```csharp
using AutoLaunch;

// Auto configuration for current program
var autoLauncher = new AutoLaunchBuilder().Automatic().Build();

// Synchronous enable
autoLauncher.Enable();
// Synchronous disable
autoLauncher.Disable();
// Synchronous status check
bool enabled = autoLauncher.GetStatus();

// Asynchronous enable
await autoLauncher.EnableAsync();
// Asynchronous disable
await autoLauncher.DisableAsync();
// Asynchronous status check
bool enabled = await autoLauncher.GetStatusAsync();
```

### Custom Configuration

```csharp
var autoLauncher = new AutoLaunchBuilder()
    .SetAppName("MyApp")
    .SetAppPath("/path/to/myapp")
    .SetArgs("arg1", "arg2")
    .AddArgs("arg3")
    .SetWorkScope(WorkScope.CurrentUser) // Set work scope for auto-launch
    .SetWindowsEngine(WindowsEngine.Registry) // Use Registry on Windows, ignored on other platforms
    .SetLinuxEngine(LinuxEngine.Freedesktop) // Use Freedesktop on Linux, ignored on other platforms
    .SetMacOSEngine(MacOSEngine.AppleScript) // Use AppleScript on macOS, ignored on other platforms
    .SetIdentifiers("com.example.myapp") // Add Bundle Identifier for macOS
    .SetExtraConfigIf(OperatingSystem.IsLinux(), "X-GNOME-Autostart-enabled=true") // Add extra config for Linux (Freedesktop standard)
    .SetExtraConfigIf(OperatingSystem.IsMacOS(), "<key>KeepAlive</key><true/>") // Add extra config for macOS (LaunchAgent standard)
    .Build();

autoLauncher.Enable();
```

### Safe Mode

Exceptions will not be thrown actively in safe mode.

```csharp
// Build a safe mode instance
var autoLauncher = new AutoLaunchBuilder().Automatic().BuildSafe();

// Try to enable, returns true/false for success/failure
bool success = autoLauncher.TryEnable();

if (!success)
{
    // Get the last exception
    Exception? lastException = autoLauncher.TakeLastException();
    if (lastException is PermissionDeniedException) 
        Console.WriteLine("Permission denied.");
    else 
        Console.WriteLine($"Unable to enable auto launch: {lastException?.Message}");
}
```

## 💡 Platform & Engine Details

### Windows

#### Registry

Implements startup via registry entries.

- `WorkScope.CurrentUser`: Creates registry entry under `HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run`.
- `WorkScope.AllUser`: Creates registry entry under `HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run` (requires admin rights).

#### StartupFolder

Implements startup by adding `.bat` files to the Startup folder.

- `WorkScope.CurrentUser`: Creates file in `C:\Users\[user]\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup`.
- `WorkScope.AllUser`: Creates file in `C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup` (requires admin rights).

#### TaskScheduler

Implements startup via Task Scheduler, can start programs requiring admin rights (requires admin rights).

### Linux

#### Freedesktop

Creates [Desktop entries (.desktop)](https://specifications.freedesktop.org/desktop-entry-spec/latest/) following the [FreeDesktop.org](https://www.freedesktop.org/wiki/) standard.

- `WorkScope.CurrentUser`: Creates file under `~/.config/autostart/`.
- `WorkScope.AllUser`: Creates file under `/etc/xdg/autostart/` (requires admin rights).

### macOS

#### LaunchAgent

Creates `.plist` files following [Launch Agents](https://developer.apple.com/library/archive/documentation/MacOSX/Conceptual/BPSystemStartup/Chapters/CreatingLaunchdJobs.html).

- `WorkScope.CurrentUser`: Creates file in `~/Library/LaunchAgents/`.
- `WorkScope.AllUser`: Creates file in `/Library/LaunchAgents/` (requires admin rights).

#### AppleScript

Uses AppleScript via `System Events` to manage login items (needs automation permission).

This engine only supports `--hidden` and `--minimized` arguments, and seems to be unsupported since macOS 13 (Ventura).

## 📝 API Documentation

### AutoLaunchBuilder

This type is used to build `AutoLauncher` or
`SafeAutoLauncher` instances with chainable configuration methods. The configured engine can be set for all platforms, but only takes effect on the corresponding platform.

- **Automatic()**: Automatically configures app name and path for the current program
- **SetAppName(string)**: Set application name
- **SetAppPath(string)**: Set application path
- **SetArgs(params string[])**: Set startup arguments, overwriting previous args
- **AddArgs(params string[])**: Add startup arguments
- **SetWorkScope(WorkScope)**: Set scope (`WorkScope.CurrentUser`/`WorkScope.AllUser`)
- **SetWindowsEngine(WindowsEngine)**: Set Windows engine, see [Windows engines](#windows) for details
- **SetLinuxEngine(LinuxEngine)**: Set Linux engine, see [Linux engines](#linux) for details
- **SetMacOSEngine(MacOSEngine)**: Set macOS engine, see [macOS engines](#macos) for details
- **SetIdentifiers(params string[])**: Set identifiers, for macOS `LaunchAgent` only
- **AddIdentifiers(params string[])**: Add identifiers
- **SetExtraConfig(string)**: Set extra config, must match corresponding engine's format. Only for Linux `Freedesktop` and macOS `LaunchAgent`
- **SetExtraConfigIf(bool, string)**: Conditionally set extra config
- **Build()**: Build an `AutoLauncher` instance
- **BuildSafe()**: Build a `SafeAutoLauncher` instance

### AutoLauncher

- **Enable()**: Enable auto launch
- **Disable()**: Disable auto launch
- **GetStatus()**: Get auto launch status
- **EnableAsync()**: Enable auto launch asynchronously
- **DisableAsync()**: Disable auto launch asynchronously
- **GetStatusAsync()**: Get auto launch status asynchronously
- **IsSupported()**: Check if the current OS supports auto launch

### SafeAutoLauncher

Inherited from `AutoLauncher`, does not throw exceptions on failure, instead uses return values and last exception info.

- **TryEnable()**: Try to enable, returns success/failure
- **TryDisable()**: Try to disable, returns success/failure
- **TryGetStatus()**: Try to check status, returns (success, enabled)
- **TryEnableAsync()**: Try to enable async
- **TryDisableAsync()**: Try to disable async
- **TryGetStatusAsync()**: Try to check status async, returns (success, enabled)
- **TakeLastException()**: Get last exception

## ⚠️ Exceptions

| Exception                    | Description                         |
|------------------------------|-------------------------------------|
| `AutoLaunchException`        | Base exception for AutoLaunch       |
| `AutoLaunchBuilderException` | Builder config error                |
| `UnsupportedOSException`     | Unsupported OS                      |
| `PermissionDeniedException`  | Permission denied                   |
| `ExecuteCommandException`    | Thrown when command execution fails |

## 📜 License

Licensed under the [MIT](LICENSE) License.