# AutoLaunch

<img alt="AutoLaunch" src="https://raw.githubusercontent.com/Linlccc/AutoLaunch/master/docs/icon/icon.png" width="128">

English | [简体中文](README-ZH_CN.md)

[![NuGet Version](https://img.shields.io/nuget/v/AutoLaunch?label=AutoLaunch&logo=dotnet)](https://www.nuget.org/packages/AutoLaunch)
[![NuGet Downloads](https://img.shields.io/nuget/dt/AutoLaunch?label=AutoLaunch)](https://www.nuget.org/packages/AutoLaunch)
[![GitHub License](https://img.shields.io/github/license/Linlccc/AutoLaunch)](https://github.com/Linlccc/AutoLaunch/blob/master/LICENSE)

[AutoLaunch](https://github.com/Linlccc/AutoLaunch) is a cross-platform .NET library that provides a unified API for enabling auto-start for applications and executables on Windows, Linux, and macOS systems.

## ✨ Features

- 🌍 **Cross-Platform Support:** Windows, Linux, macOS
- 🔧 **Multiple Engines:** Various implementations for each platform
- 🎯 **Ease of Use:** Unified API across all platforms
- 🛠 **AOT Support:** Fully supports AOT and trimming
- 📦 **Zero Dependency:** No third-party library required

## 🚚 Supported Engines

### Windows

| Engine            | Description                       | Permission | Note                                       |
|-------------------|-----------------------------------|------------|--------------------------------------------|
| **Registry**      | Manage startup via registry       | User/Admin |                                            |
| **StartupFolder** | Manage startup via startup folder | User/Admin |                                            |
| **TaskScheduler** | Manage startup via Task Scheduler | Admin      | Can launch programs requiring admin rights |

### Linux

| Engine          | Description                             | Permission | Note                                                  |
|-----------------|-----------------------------------------|------------|-------------------------------------------------------|
| **Freedesktop** | Manage startup via Freedesktop standard | User/Admin | Requires a desktop environment supporting Freedesktop |

### macOS

| Engine          | Description                          | Permission            | Note                                            |
|-----------------|--------------------------------------|-----------------------|-------------------------------------------------|
| **LaunchAgent** | Manage startup via Launch Agent      | User/Admin            |                                                 |
| **AppleScript** | Manage startup items via login items | Automation permission | Only supports `--hidden`/`--minimize` arguments |

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

Unified configuration API for all platforms

### Basic Usage

```csharp
using AutoLaunch;

// Automatically configure for current program
var autoLauncher = new AutoLaunchBuilder().Automatic().Build();

// Enable auto-launch synchronously
autoLauncher.Enable();
// Disable auto-launch synchronously
autoLauncher.Disable();
// Check if enabled synchronously
bool isEnabled = autoLauncher.IsEnabled();

// Enable auto-launch asynchronously
await autoLauncher.EnableAsync();
// Disable auto-launch asynchronously
await autoLauncher.DisableAsync();
// Check if enabled asynchronously
bool isEnabledAsync = await autoLauncher.IsEnabledAsync();
```

### Custom Configuration

```csharp
var autoLauncher = new AutoLaunchBuilder()
    .SetAppName("MyApp")
    .SetAppPath("/path/to/myapp")
    .SetArgs("arg1", "arg2")
    .AddArgs("arg3")
    .SetWorkScope(WorkScope.CurrentUser) // Set work scope for auto-launch
    .SetWindowsEngine(WindowsEngine.Registry) // Use Registry engine on Windows, ignored on other platforms
    .SetLinuxEngine(LinuxEngine.Freedesktop) // Use Freedesktop engine on Linux, ignored on other platforms
    .SetMacOSEngine(MacOSEngine.LaunchAgent) // Use LaunchAgent engine on macOS, ignored on other platforms
    .SetIdentifiers("com.example.myapp") // Add Bundle Identifier for macOS
    .SetExtraConfigIf(OperatingSystem.IsLinux(), "X-GNOME-Autostart-enabled=true") // Add extra config for Linux, must conform to Freedesktop standard
    .SetExtraConfigIf(OperatingSystem.IsMacOS(), "<key>KeepAlive</key><true/>") // Add extra config for macOS, must conform to LaunchAgent standard
    .Build();

autoLauncher.Enable();
```

### Safe Mode

No exceptions will be thrown in safe mode

```csharp
// Build an instance in safe mode
var autoLauncher = new AutoLaunchBuilder().Automatic().BuildSafe();

// Try to enable, returns true/false for success/failure
bool success = autoLauncher.TryEnable();

if(!success)
{
   // Get the last exception
    Exception? lastException = autoLauncher.TakeLastException();
    if (lastException is PermissionDeniedException) Console.WriteLine("Permission denied.");
    else Console.WriteLine($"Failed to enable auto-launch: {lastException?.Message}");
}
```

## API Documentation

### AutoLaunchBuilder

| Method                            | Description                               |
|-----------------------------------|-------------------------------------------|
| `Automatic()`                     | Automatically configure app name and path |
| `SetAppName(string)`              | Set app name                              |
| `SetAppPath(string)`              | Set app path                              |
| `SetArgs(params string[])`        | Set startup arguments                     |
| `AddArgs(params string[])`        | Add startup arguments                     |
| `SetWorkScope(WorkScope)`         | Set work scope (current user/all users)   |
| `SetWindowsEngine(WindowsEngine)` | Set Windows engine                        |
| `SetLinuxEngine(LinuxEngine)`     | Set Linux engine                          |
| `SetMacOSEngine(MacOSEngine)`     | Set macOS engine                          |
| `SetIdentifiers(params string[])` | Set identifiers (macOS LaunchAgent only)  |
| `AddIdentifiers(params string[])` | Add identifiers (macOS LaunchAgent only)  |
| `SetExtraConfig(string)`          | Set extra configuration                   |
| `SetExtraConfigIf(bool, string)`  | Conditionally set extra configuration     |
| `Build()`                         | Build AutoLauncher instance               |
| `BuildSafe()`                     | Build SafeAutoLauncher instance           |

### AutoLauncher Interface

| Method             | Description                        |
|--------------------|------------------------------------|
| `Enable()`         | Enable auto-launch                 |
| `Disable()`        | Disable auto-launch                |
| `IsEnabled()`      | Check if enabled                   |
| `EnableAsync()`    | Enable auto-launch asynchronously  |
| `DisableAsync()`   | Disable auto-launch asynchronously |
| `IsEnabledAsync()` | Check if enabled asynchronously    |

### SafeAutoLauncher Extra Methods

| Method                | Description                                     |
|-----------------------|-------------------------------------------------|
| `TryEnable()`         | Try to enable, returns success/failure          |
| `TryDisable()`        | Try to disable, returns success/failure         |
| `TryIsEnabled()`      | Try to check status, returns (success, enabled) |
| `TryEnableAsync()`    | Try to enable asynchronously                    |
| `TryDisableAsync()`   | Try to disable asynchronously                   |
| `TryIsEnabledAsync()` | Try to check status asynchronously              |
| `TakeLastException()` | Get last exception                              |

## ⚠️ Exception Types

| Exception                    | Description                  |
|------------------------------|------------------------------|
| `AutoLaunchException`        | Base exception class         |
| `AutoLaunchBuilderException` | Builder configuration error  |
| `UnsupportedOSException`     | Unsupported operating system |
| `PermissionDeniedException`  | Permission denied            |
| `ExecuteCommandException`    | Command execution failed     |

## 📜 License

Licensed under the terms of the [MIT License](LICENSE).