# AutoLaunch

![AutoLaunch](https://raw.githubusercontent.com/Linlccc/AutoLaunch/master/docs/icon/icon-128x128.png)

[![GitHub repo size](https://img.shields.io/github/repo-size/Linlccc/AutoLaunch)](https://github.com/Linlccc/AutoLaunch)
[![GitHub License](https://img.shields.io/github/license/Linlccc/AutoLaunch)](https://github.com/Linlccc/AutoLaunch/blob/master/LICENSE)
[![NuGet Version](https://img.shields.io/nuget/v/AutoLaunch?label=AutoLaunch&logo=dotnet)](https://www.nuget.org/packages/AutoLaunch)
[![NuGet Downloads](https://img.shields.io/nuget/dt/AutoLaunch?label=AutoLaunch)](https://www.nuget.org/packages/AutoLaunch)
[![AutoLaunchTestTool](https://img.shields.io/badge/AutoLaunchTestTool-0D6EFD)](https://github.com/Linlccc/AutoLaunchTestTool)

[English](https://github.com/Linlccc/AutoLaunch/blob/master/README.md) | 简体中文

[AutoLaunch](https://github.com/Linlccc/AutoLaunch) 提供自动在启动、登录时运行任意应用程序或可执行文件的功能。支持 Windows、Linux 和 macOS 系统。

[AutoLaunchTestTool](https://github.com/Linlccc/AutoLaunchTestTool) 为该项目提供了一个基于 [Avalonia](https://avaloniaui.net/) 的图形界面测试工具，可以帮助你快速验证和实验自动启动功能。

## ✨ 特性

- 🌍 **跨平台支持**：Windows、Linux、macOS
- 🔧 **多引擎**：每个平台支持多种实现方式
- 🎯 **易用性**：所有平台使用统一 API
- 🧱 **AOT 支持**：完全支持 AOT 与裁剪
- 📦 **零依赖**：不依赖任何第三方库

## 📦 安装

### .NET CLI

```bash
dotnet add package AutoLaunch
```

### Package Manager Console

```powershell
Install-Package AutoLaunch
```

## 🚀 快速开始

`AutoLaunchBuilder` 在所有平台使用统一配置 API，消除不同平台下构造参数的差异，实现一套配置多平台运行。

### 基本用法

```csharp
using AutoLaunch;

// 根据当前程序自动配置
var autoLauncher = new AutoLaunchBuilder().Automatic().Build();

// 同步启用
autoLauncher.Enable();
// 同步禁用
autoLauncher.Disable();
// 同步获取状态
bool enabled = autoLauncher.GetStatus();

// 异步启用
await autoLauncher.EnableAsync();
// 异步禁用
await autoLauncher.DisableAsync();
// 异步获取状态
bool enabled = await autoLauncher.GetStatusAsync();
```

### 自定义配置

```csharp
var autoLauncher = new AutoLaunchBuilder()
    .SetAppName("MyApp")
    .SetAppPath("/path/to/myapp")
    .SetArgs("arg1", "arg2")
    .AddArgs("arg3")
    .SetWorkScope(WorkScope.CurrentUser) // 配置自启动的工作范围
    .SetWindowsEngine(WindowsEngine.Registry) // Windows 下使用注册表方式，在其他平台该配置无效
    .SetLinuxEngine(LinuxEngine.Freedesktop) // Linux 下使用 Freedesktop 标准方式，在其他平台该配置无效
    .SetMacOSEngine(MacOSEngine.AppleScript) // macOS 下使用 AppleScript 方式，在其他平台该配置无效
    .SetIdentifiers("com.example.myapp") // macOS 添加 Bundle Identifier
    .SetExtraConfigIf(OperatingSystem.IsLinux(), "X-GNOME-Autostart-enabled=true") // 仅在 Linux 下添加额外配置，需要符合 Freedesktop 标准
    .SetExtraConfigIf(OperatingSystem.IsMacOS(), "<key>KeepAlive</key><true/>") // 仅在 macOS 下添加额外配置，需要符合 LaunchAgent 标准
    .Build();

autoLauncher.Enable();
```

### 安全模式

在安全模式下不会主动抛出异常：

```csharp
// 构建安全模式实例
var autoLauncher = new AutoLaunchBuilder().Automatic().BuildSafe();

// 尝试启用，返回 true/false 表示成功/失败
bool success = autoLauncher.TryEnable();

if (!success)
{
    // 获取最后一次操作的异常信息
    Exception? lastException = autoLauncher.TakeLastException();
    if (lastException is PermissionDeniedException) 
        Console.WriteLine("权限被拒绝。");
    else 
        Console.WriteLine($"无法启用自动启动：{lastException?.Message}");
}
```

## 💡 平台与引擎细节

### Windows

#### Registry

通过注册表实现启动项。

- `WorkScope.CurrentUser`：在 `HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run` 下创建注册表项
- `WorkScope.AllUser`：在 `HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run` 下创建注册表项（需要管理员权限）

#### StartupFolder

通过启动文件夹添加启动程序 `.bat` 文件实现启动项。

- `WorkScope.CurrentUser`：在 `C:\Users\[user]\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Startup` 下创建文件
- `WorkScope.AllUser`：在 `C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup` 下创建文件（需要管理员权限）

#### TaskScheduler

通过任务计划程序实现启动项，可以启动需要管理员权限的程序（需要管理员权限）。

### Linux

#### Freedesktop

创建符合 [FreeDesktop.org](https://www.freedesktop.org/wiki/) 规范的 [Desktop entries (.desktop)](https://specifications.freedesktop.org/desktop-entry-spec/latest/) 来实现启动项。

- `WorkScope.CurrentUser`：在 `~/.config/autostart/` 下创建文件
- `WorkScope.AllUser`：在 `/etc/xdg/autostart/` 下创建文件（需要管理员权限）

### macOS

#### LaunchAgent

创建符合 [Launch Agents](https://developer.apple.com/library/archive/documentation/MacOSX/Conceptual/BPSystemStartup/Chapters/CreatingLaunchdJobs.html) 的 `.plist` 文件来实现启动项。

- `WorkScope.CurrentUser`：在 `~/Library/LaunchAgents/` 下创建文件
- `WorkScope.AllUser`：在 `/Library/LaunchAgents/` 下创建文件（需要管理员权限）

#### AppleScript

使用 AppleScript 通过 `System Events` 管理登录项来实现启动项（需要自动化权限）。

该引擎只支持 `--hidden` 和 `--minimized` 两个参数，并且似乎在 macOS 13 (Ventura) 之后也不再支持了（没有相关文档）。

## 📝 API 文档

### AutoLaunchBuilder

该类型用于构建 `AutoLauncher` 或 `SafeAutoLauncher` 实例，支持使用链式调用配置参数。配置中的引擎可在所有平台设置，但仅在对应平台生效。

- **Automatic()**：自动配置应用名称和路径，适用于当前运行的程序
- **SetAppName(string)**：设置应用名称
- **SetAppPath(string)**：设置应用路径
- **SetArgs(params string[])**：设置启动参数，覆盖之前的参数
- **AddArgs(params string[])**：添加启动参数
- **SetWorkScope(WorkScope)**：设置作用域（`WorkScope.CurrentUser`/`WorkScope.AllUser`）
- **SetWindowsEngine(WindowsEngine)**：设置 Windows 引擎，更多信息参阅 [Windows 引擎](#windows)
- **SetLinuxEngine(LinuxEngine)**：设置 Linux 引擎，更多信息参阅 [Linux 引擎](#linux)
- **SetMacOSEngine(MacOSEngine)**：设置 macOS 引擎，更多信息参阅 [macOS 引擎](#macos)
- **SetIdentifiers(params string[])**：设置标识符，仅适用于 macOS 下 `LaunchAgent` 引擎
- **AddIdentifiers(params string[])**：添加标识符
- **SetExtraConfig(string)**：设置额外配置，内容需符合对应引擎的规范，仅适用于 Linux 下 `Freedesktop` 和 macOS 下 `LaunchAgent` 引擎
- **SetExtraConfigIf(bool, string)**：条件设置额外配置
- **Build()**：构建 `AutoLauncher` 实例
- **BuildSafe()**：构建 `SafeAutoLauncher` 实例

### AutoLauncher

- **Enable()**：启用自动启动
- **Disable()**：禁用自动启动
- **GetStatus()**：获取自动启动状态
- **EnableAsync()**：异步启用自动启动
- **DisableAsync()**：异步禁用自动启动
- **GetStatusAsync()**：异步获取自动启动状态
- **IsSupported()**：检查当前操作系统是否支持自动启动功能

### SafeAutoLauncher

继承自 `AutoLauncher`，在操作失败时不会抛出异常，而是通过返回值和最后一次异常信息进行反馈。

- **TryEnable()**：尝试启用，返回成功/失败
- **TryDisable()**：尝试禁用，返回成功/失败
- **TryGetStatus()**：尝试检查状态，返回 (成功, 启用状态)
- **TryEnableAsync()**：异步尝试启用
- **TryDisableAsync()**：异步尝试禁用
- **TryGetStatusAsync()**：异步尝试检查状态，返回 (成功, 启用状态)
- **TakeLastException()**：获取最后一次操作的异常

## ⚠️ 异常

| 异常                           | 描述               |
|------------------------------|------------------|
| `AutoLaunchException`        | AutoLaunch 库异常基类 |
| `AutoLaunchBuilderException` | 构建器配置错误          |
| `UnsupportedOSException`     | 不支持的操作系统         |
| `PermissionDeniedException`  | 权限被拒绝            |
| `ExecuteCommandException`    | 命令行执行失败时抛出       |

## 📜 许可证

根据 [MIT](LICENSE) 许可证的条款分发。
