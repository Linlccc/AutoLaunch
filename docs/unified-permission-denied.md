# 统一权限被拒绝异常抛出

本文用于记录不同平台、不同引擎无权限时的状态  
方便统一抛出 `AutoLaunch.Exceptions.PermissionDeniedException` 异常

## 总结

- Windows
    - Registry
        - 抛出 `System.UnauthorizedAccessException`
        - 抛出 `System.Security.SecurityException` see:[source](https://github.com/dotnet/runtime/blob/30700fb260399015cb12bbea53869b621995b83a/src/libraries/Microsoft.Win32.Registry/src/Microsoft/Win32/RegistryKey.cs#L525-L530)
    - StartupFolder - 抛出 `System.UnauthorizedAccessException`
    - TaskScheduler - 错误消息包含 HRESULT：`0x80070005`
- Linux
    - Freedesktop - 抛出 `System.UnauthorizedAccessException`
- macOS
    - LaunchAgent - 抛出 `System.UnauthorizedAccessException`
    - AppleScript - 错误消息包含错误码：`-1743`

## 特殊错误信息来源

记录特殊的错误信息怎么查看或者来源

### ScheduledTask(任务计划)

> 转发的 Windows 错误码，see: [HRESULT](https://learn.microsoft.com/zh-cn/windows/win32/seccrypto/common-hresult-values)

### AppleScript osascript

> 没找到相关的文档  
> 按照以下步骤来获取错误码信息

1. 将以下脚本保存成 `errs.sh`
2. 添加执行权限 `chmod +x errs.sh`
3. 查看某个错误码 `./errs.sh 1743`，查看某个范围 `./errs.sh 1700 1800`

```bash
#!/bin/bash

function run_osascript() {
    osascript -s o -e "Error number -$1" | grep -Ev 'execution error: (An error of type -|发生"-[0-9]+"类型错误)'
}

if [ $# -eq 1 ]; then
    run_osascript "$1"
elif [ $# -eq 2 ]; then
    for ((i=$1; i<=$2; i++)); do
        run_osascript "$i"
    done
else
    echo "用法: $0 <number> 或 $0 <start> <end>"
    exit 1
fi
```

## 权限被拒绝的情况

### Windows

> `Registry` + `AllUser`

- 非管理员权限执行启用
- 非管理员权限执行禁用

> `StartupFolder` + `AllUser`  
> 文件有更细粒度的权限控制

- 在没有特殊权限的情况下
    - 非管理员权限执行启用
    - 非管理员权限执行禁用
- 其他情况
    - 对 `CommonStartup` 目录没有写入权限时执行启用
    - 对 `CommonStartup` 目录内容没有删除权限时执行禁用
    - 对 `CommonStartup` 目录内容没有读取权限时执行状态检查

> `TaskScheduler`  
> 任务计划中，如果没有读取权限，不管任务是否存在，状态检查动作永远返回 `false`，禁用动作永远成功（但是不会有任何实际动作）

- 非管理员权限执行启用
- 非管理员权限执行禁用

### Linux

> `Freedesktop` + `AllUser`  
> 在此不对细粒度权限控制做说明

- 没有管理员权限时执行启用
- 没有管理员权限时执行禁用

### macOS

> `LaunchAgent` + `AllUser`  
> 在此不对细粒度权限控制做说明

- 没有管理员权限时执行启用
- 没有管理员权限时执行禁用

> `AppleScript`

- 没有自动化权限时执行启用
- 没有自动化权限时执行禁用
- 没有自动化权限时执行状态检查