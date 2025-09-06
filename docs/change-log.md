# 变更日志

## Template

### 修复

### 变更

---

## 1.0.1

### 修复

- nuget 文档中的 README 图标和多语言链接错误。

### 变更

- .net8.0 以下的版本添加源链接支持。
- 添加 `AutoLauncher.IsSupported()` 静态方法，检查当前平台是否受支持。
- 将 `IsEnabled` 修改成 `GetStatus`，异步和安全版本均同步修改。

---

## v1.0.0

### 修复

- 修复使用 `WindowsEngine.Registry` 引擎时权限被拒绝抛出 `SecurityException` 而不是 `PermissionDeniedException` 的问题。

### 变更

- 添加不会抛出异常的安全启动器 `SafeAutoLauncher`，通过 `AutoLaunchBuilder.BuildSafe` 方法构建。
- `AutoLaunch` 中抛出的所有异常均继承自 `AutoLaunch.AutoLaunchException`。
- 开放 `IAutoLauncher`、`IAsyncAutoLauncher` 接口。
- 完成 README 和 README-ZH_CN 文档。
- 完成代码文档注释。

___

## v1.0.0-preview.4

该版本用于测试工作流创建 Release。

___

## v1.0.0-preview.3

### 变更

- 添加项目文档文件。

---

## v1.0.0-preview.2

### 修复

- 修复无权限时统一抛出 `AutoLaunch.Exceptions.PermissionDeniedException` 异常 (#1)。

### 变更

- 添加 GitHub Issue 模板。
- 修改图标。
- `AutoLaunchBuilder.Build` 时检查 `AppPath` 是否为绝对路径。

---

## v1.0.0-preview.1

这是 AutoLaunch 的首个预览版本，已实现项目的核心功能。  
**重要提示**：预览版可能包含不稳定 API 或行为变更，不建议直接用于生产环境。

### 变更

- 完全支持 AOT 与裁剪。
- 提供跨平台统一的启动器构建 API。
- **Windows**：实现了基于注册表、开始菜单文件夹、任务计划的自启动。
- **Linux**：实现了基于 Freedesktop 的自启动。
- **MacOS**：实现了基于 LaunchAgent 和 AppleScript 的自启动。


