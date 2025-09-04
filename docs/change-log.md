# 变更日志

## Template

### 新增

### 变更

### 优化

### 修复

### 移除

---

## Next

### 新增

- 添加 `ExceptionalUnifiedDecorator` 异常装饰器，统一处理异常消息
- 添加 `SafeAutoLauncher` 安全启动器，不会抛出异常
- 添加 `SafeDecorator` 安全装饰器，实现 `SafeAutoLauncher`
- 添加 `AutoLaunchBuilder.BuildSafe` 方法，构建 `SafeAutoLauncher`

### 变更

- `AutoLaunch` 中抛出所有的异常继承自 `AutoLaunch.AutoLaunchException`
- 开放 `IAutoLauncher` 接口
- 开放 `IAsyncAutoLauncher` 接口

### 优化

### 修复

### 移除

___

## v1.0.0-preview.4

该版本用于测试工作流创建 Release。

___

## v1.0.0-preview.3

该版本用于测试工作流创建 Release。

### 新增

- 添加项目文档文件
- 工作流自动发布一个默认 GitHub Release

---

## v1.0.0-preview.2

### 新增

- 添加 GitHub Issue 模板

### 变更

- 修改 icon 图标
- `AutoLaunchBuilder.Build` 检查 `AppPath` 是否是绝对路径

### 优化

- 无权限时统一抛出 `AutoLaunch.Exceptions.PermissionDeniedException` 异常。 #1

---

## v1.0.0-preview.1

这是 AutoLaunch 的首个预览版本，已实现项目的核心功能。  
**重要提示**：预览版可能包含不稳定 API 或行为变更，不建议直接用于生产环境。

### 新增功能

- Windows - 注册表自启动
- Windows - 开始文件夹自启动
- Windows - 任务计划自启动
- Linux - Freedesktop 自启动
- MacOS - LaunchAgent 自启动
- MacOS - AppleScript 自启动
- 多平台统一 API 调用
- 完全支持 AOT 与裁剪

