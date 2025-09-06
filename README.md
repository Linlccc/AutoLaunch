# AutoLaunch

![AutoLaunch](https://raw.githubusercontent.com/Linlccc/AutoLaunch/master/docs/icon/icon-128x128.png)

English | [简体中文](https://github.com/Linlccc/AutoLaunch/blob/master/README-ZH_CN.md)

[![GitHub repo size](https://img.shields.io/github/repo-size/Linlccc/AutoLaunch)](https://github.com/Linlccc/AutoLaunch)
[![GitHub License](https://img.shields.io/github/license/Linlccc/AutoLaunch)](https://github.com/Linlccc/AutoLaunch/blob/master/LICENSE)
[![NuGet Version](https://img.shields.io/nuget/v/AutoLaunch?label=AutoLaunch&logo=dotnet)](https://www.nuget.org/packages/AutoLaunch)
[![NuGet Downloads](https://img.shields.io/nuget/dt/AutoLaunch?label=AutoLaunch)](https://www.nuget.org/packages/AutoLaunch)
[![AutoLaunchTestTool](https://img.shields.io/badge/AutoLaunchTestTool-0D6EFD)](https://github.com/Linlccc/AutoLaunchTestTool)

[AutoLaunch](https://github.com/Linlccc/AutoLaunch) is a cross-platform .NET library that provides a unified API for configuring application / executable auto-start behavior on Windows, Linux, and macOS.

A graphical test utility built with [Avalonia](https://avaloniaui.net/), [AutoLaunchTestTool](https://github.com/Linlccc/AutoLaunchTestTool), is available to help you quickly verify and experiment with auto-start functionality.

## ✨ Features

- 🌍 Cross-platform: Windows, Linux, macOS
- 🔧 Multiple engines: Several mechanisms per platform
- 🎯 Unified API: Same usage pattern across all platforms
- 🛠 AOT friendly: Fully supports trimming and native AOT
- 📦 Zero dependencies: No third-party runtime requirements

## 🚚 Supported Engines

### Windows

| Engine            | Description                         | Permission | Notes                                  |
|-------------------|-------------------------------------|------------|----------------------------------------|
| **Registry**      | Uses Run / RunOnce registry entries | User/Admin |                                        |
| **StartupFolder** | Uses Startup folder shortcut        | User/Admin |                                        |
| **TaskScheduler** | Uses Windows Task Scheduler         | Admin      | Can start programs requiring elevation |

### Linux

| Engine          | Description                                          | Permission | Notes                                            |
|-----------------|------------------------------------------------------|------------|--------------------------------------------------|
| **Freedesktop** | Creates a .desktop autostart entry (XDG/Freedesktop) | User/Admin | Requires a Freedesktop-compliant desktop session |

### macOS

| Engine          | Description                       | Permission         | Notes                                                     |
|-----------------|-----------------------------------|--------------------|-----------------------------------------------------------|
| **LaunchAgent** | Uses Launch Agent plist entry     | User/Admin         |                                                           |
| **AppleScript** | Adds a login item via AppleScript | Automation consent | Only supports `--hidden` / `--minimize` argument patterns |

## 📦 Installation

dotnet CLI:

```bash
dotnet add package AutoLaunch
```

Package Manager Console:

```powershell
Install-Package AutoLaunch
```

## 🚀 Quick Start

Unified configuration API across all platforms.

### Basic Usage

```csharp
using AutoLaunch;

// Automatically infer application name and path from current process
var autoLauncher = new AutoLaunchBuilder().Automatic().Build();

// Enable (sync)
autoLauncher.Enable();
// Disable (sync)
autoLauncher.Disable();
// Check if enabled (sync)
bool isEnabled = autoLauncher.IsEnabled();

// Enable (async)
await autoLauncher.EnableAsync();
// Disable (async)
await autoLauncher.DisableAsync();
// Check if enabled (async)
bool isEnabledAsync = await autoLauncher.IsEnabledAsync();
```

### Custom Configuration

```csharp
var autoLauncher = new AutoLaunchBuilder()
    .SetAppName("MyApp")
    .SetAppPath("/path/to/myapp")
    .SetArgs("arg1", "arg2")
    .AddArgs("arg3")
    .SetWorkScope(WorkScope.CurrentUser) // Choose scope for auto-start
    .SetWindowsEngine(WindowsEngine.Registry) // Applies only on Windows
    .SetLinuxEngine(LinuxEngine.Freedesktop) // Applies only on Linux
    .SetMacOSEngine(MacOSEngine.LaunchAgent) // Applies only on macOS
    .SetIdentifiers("com.example.myapp") // macOS bundle identifiers (LaunchAgent)
    .SetExtraConfigIf(OperatingSystem.IsLinux(), "X-GNOME-Autostart-enabled=true") // Linux-only extra config (Freedesktop spec compliant)
    .SetExtraConfigIf(OperatingSystem.IsMacOS(), "<key>KeepAlive</key><true/>") // macOS LaunchAgent-only extra plist fragment
    .Build();

autoLauncher.Enable();
```

### Safe Mode

In safe mode, no exceptions are thrown directly; you query the result state.

```csharp
// Build a safe-mode launcher
var autoLauncher = new AutoLaunchBuilder().Automatic().BuildSafe();

// Try to enable; returns true/false
bool success = autoLauncher.TryEnable();

if (!success)
{
    // Retrieve the last exception
    Exception? lastException = autoLauncher.TakeLastException();
    if (lastException is PermissionDeniedException) Console.WriteLine("Permission denied.");
    else Console.WriteLine($"Failed to enable auto-launch: {lastException?.Message}");
}
```

## API Reference

### AutoLaunchBuilder

| Method                            | Description                               |
|-----------------------------------|-------------------------------------------|
| `Automatic()`                     | Automatically sets app name & path        |
| `SetAppName(string)`              | Sets the application name                 |
| `SetAppPath(string)`              | Sets the executable path                  |
| `SetArgs(params string[])`        | Replaces all startup arguments            |
| `AddArgs(params string[])`        | Appends additional startup arguments      |
| `SetWorkScope(WorkScope)`         | Sets scope (current user / all users)     |
| `SetWindowsEngine(WindowsEngine)` | Selects engine for Windows                |
| `SetLinuxEngine(LinuxEngine)`     | Selects engine for Linux                  |
| `SetMacOSEngine(MacOSEngine)`     | Selects engine for macOS                  |
| `SetIdentifiers(params string[])` | Sets identifiers (macOS LaunchAgent only) |
| `AddIdentifiers(params string[])` | Adds identifiers (macOS LaunchAgent only) |
| `SetExtraConfig(string)`          | Sets extra raw config block               |
| `SetExtraConfigIf(bool, string)`  | Conditionally sets an extra config block  |
| `Build()`                         | Builds an `AutoLauncher`                  |
| `BuildSafe()`                     | Builds a `SafeAutoLauncher`               |

### AutoLauncher Interface

| Method             | Description                  |
|--------------------|------------------------------|
| `Enable()`         | Enables auto-start           |
| `Disable()`        | Disables auto-start          |
| `IsEnabled()`      | Checks if enabled            |
| `EnableAsync()`    | Enables auto-start (async)   |
| `DisableAsync()`   | Disables auto-start (async)  |
| `IsEnabledAsync()` | Checks enabled state (async) |

### SafeAutoLauncher Extra Methods

| Method                | Description                                       |
|-----------------------|---------------------------------------------------|
| `TryEnable()`         | Attempts enable; returns success/failure          |
| `TryDisable()`        | Attempts disable; returns success/failure         |
| `TryIsEnabled()`      | Attempts status check; returns (success, enabled) |
| `TryEnableAsync()`    | Async variant                                     |
| `TryDisableAsync()`   | Async variant                                     |
| `TryIsEnabledAsync()` | Async variant                                     |
| `TakeLastException()` | Retrieves the last thrown exception (if any)      |

## ⚠️ Exception Types

| Exception                    | Description                       |
|------------------------------|-----------------------------------|
| `AutoLaunchException`        | Base exception type               |
| `AutoLaunchBuilderException` | Builder configuration error       |
| `UnsupportedOSException`     | Unsupported operating system      |
| `PermissionDeniedException`  | Permission denied                 |
| `ExecuteCommandException`    | External command execution failed |

## 📜 License

Released under the terms of the [MIT](LICENSE) License.