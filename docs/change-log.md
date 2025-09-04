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

### 变更

### 优化

### 修复

### 移除

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

