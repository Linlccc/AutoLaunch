# AutoLaunch

<img alt="AutoLaunch" src="https://raw.githubusercontent.com/Linlccc/AutoLaunch/master/docs/icon/icon.png" width="128">

[English](README.md) | 简体中文

[![NuGet Version](https://img.shields.io/nuget/v/AutoLaunch?label=AutoLaunch&logo=dotnet)](https://www.nuget.org/packages/AutoLaunch)
[![NuGet Downloads](https://img.shields.io/nuget/dt/AutoLaunch?label=AutoLaunch)](https://www.nuget.org/packages/AutoLaunch)
[![GitHub License](https://img.shields.io/github/license/Linlccc/AutoLaunch)](https://github.com/Linlccc/AutoLaunch/blob/master/LICENSE)

[AutoLaunch](https://github.com/Linlccc/AutoLaunch) 是一个跨平台的 .NET 库，提供了在 Windows、Linux 和 macOS 系统上实现应用程序和可执行文件自动启动的统一 API。

## ✨ 特性

- 🌍 **跨平台支持**：Windows、Linux、macOS
- 🔧 **多种引擎**：每个平台支持多种实现方式
- 🎯 **易用性**：所有平台使用统一 API
- 🛠 **AOT支持**：完全支持 AOT 与裁剪
- 📦 **零依赖**：不依赖任何第三方库

## 🚚 支持的引擎

### Windows

| 引擎                | 描述        | 权限要求     | 备注            |
|-------------------|-----------|----------|---------------|
| **Registry**      | 通过注册表实现   | 普通用户/管理员 |               |
| **StartupFolder** | 通过启动文件夹实现 | 普通用户/管理员 |               |
| **TaskScheduler** | 通过任务计划实现  | 管理员      | 可启动需要管理员权限的程序 |

### Linux

| 引擎              | 描述                     | 权限要求     | 备注                     |
|-----------------|------------------------|----------|------------------------|
| **Freedesktop** | 通过 FreeDesktop 规范启动项实现 | 普通用户/管理员 | 需要支持 FreeDesktop 的桌面环境 |

### macOS

| 引擎              | 描述                    | 权限要求     | 备注                            |
|-----------------|-----------------------|----------|-------------------------------|
| **LaunchAgent** | 通过 Launch Agent 启动项实现 | 普通用户/管理员 |                               |
| **AppleScript** | 通过登录项启动项实现            | 自动化权限    | 参数只支持 `--hidden`/`--minimize` |

## 📦 安装

dotnet CLI:

```bash
dotnet add package AutoLaunch
```

Package Manager Console:

```powershell
Install-Package AutoLaunch
```

## 🚀 快速开始

所有平台使用统一配置 API

### 基本用法

```csharp
using AutoLaunch;

// 根据当前程序自动配置
var autoLauncher = new AutoLaunchBuilder().Automatic().Build();

// 同步启动
autoLauncher.Enable();
// 同步禁用
autoLauncher.Disable();
// 同步检查是否启用
bool isEnabled = autoLauncher.IsEnabled();

// 异步启动
await autoLauncher.EnableAsync();
// 异步禁用
await autoLauncher.DisableAsync();
// 异步检查是否启用
bool isEnabledAsync = await autoLauncher.IsEnabledAsync();
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
    .SetMacOSEngine(MacOSEngine.LaunchAgent) // macOS 下使用 AppleScript 方式，在其他平台该配置无效
    .SetIdentifiers("com.example.myapp") // macOS 添加 Bundle Identifier
    .SetExtraConfigIf(OperatingSystem.IsLinux(), "X-GNOME-Autostart-enabled=true") // 仅在 Linux 下添加额外配置，需要符合 Freedesktop 标准
    .SetExtraConfigIf(OperatingSystem.IsMacOS(), "<key>KeepAlive</key><true/>") // 仅在 macOS 下添加额外配置，需要符合 LaunchAgent 标准
    .Build();

autoLauncher.Enable();
```

### 安全模式

在安全模式下不会主动抛出异常

```csharp
// 构建安全模式实例
var autoLauncher = new AutoLaunchBuilder().Automatic().BuildSafe();

// 尝试启用，返回 true/false 表示成功/失败
bool success = autoLauncher.TryEnable();

if(!success)
{
   // 获取最后一次操作的异常信息
    Exception? lastException = autoLauncher.TakeLastException();
    if (lastException is PermissionDeniedException) Console.WriteLine("权限被拒绝。");
    else Console.WriteLine($"无法启用自动启动: {lastException?.Message}");
}
```

## API 文档

### AutoLaunchBuilder

| 方法                                | 描述                         |
|-----------------------------------|----------------------------|
| `Automatic()`                     | 自动配置应用名称和路径                |
| `SetAppName(string)`              | 设置应用名称                     |
| `SetAppPath(string)`              | 设置应用路径                     |
| `SetArgs(params string[])`        | 设置启动参数                     |
| `AddArgs(params string[])`        | 添加启动参数                     |
| `SetWorkScope(WorkScope)`         | 设置作用域（当前用户/所有用户）           |
| `SetWindowsEngine(WindowsEngine)` | 设置 Windows 引擎              |
| `SetLinuxEngine(LinuxEngine)`     | 设置 Linux 引擎                |
| `SetMacOSEngine(MacOSEngine)`     | 设置 macOS 引擎                |
| `SetIdentifiers(params string[])` | 设置标识符（仅 macOS LaunchAgent） |
| `AddIdentifiers(params string[])` | 添加标识符（仅 macOS LaunchAgent） |
| `SetExtraConfig(string)`          | 设置额外配置                     |
| `SetExtraConfigIf(bool, string)`  | 条件设置额外配置                   |
| `Build()`                         | 构建 AutoLauncher 实例         |
| `BuildSafe()`                     | 构建 SafeAutoLauncher 实例     |

### AutoLauncher 接口

| 方法                 | 描述       |
|--------------------|----------|
| `Enable()`         | 启用自动启动   |
| `Disable()`        | 禁用自动启动   |
| `IsEnabled()`      | 检查是否启用   |
| `EnableAsync()`    | 异步启用自动启动 |
| `DisableAsync()`   | 异步禁用自动启动 |
| `IsEnabledAsync()` | 异步检查是否启用 |

### SafeAutoLauncher 额外方法

| 方法                    | 描述                   |
|-----------------------|----------------------|
| `TryEnable()`         | 尝试启用，返回成功/失败         |
| `TryDisable()`        | 尝试禁用，返回成功/失败         |
| `TryIsEnabled()`      | 尝试检查状态，返回 (成功, 启用状态) |
| `TryEnableAsync()`    | 异步尝试启用               |
| `TryDisableAsync()`   | 异步尝试禁用               |
| `TryIsEnabledAsync()` | 异步尝试检查状态             |
| `TakeLastException()` | 获取最后一次操作的异常          |

## ⚠️ 异常类型

| 异常                           | 描述       |
|------------------------------|----------|
| `AutoLaunchException`        | 基础异常类    |
| `AutoLaunchBuilderException` | 构建器配置错误  |
| `UnsupportedOSException`     | 不支持的操作系统 |
| `PermissionDeniedException`  | 权限被拒绝    |
| `ExecuteCommandException`    | 命令执行失败   |

## 📜 许可证

根据 [MIT](LICENSE) 许可证的条款。
