# 统一权限被拒绝异常抛出

本文用于记录不同平台、不同引擎无权限时的状态
方便统一抛出 `AutoLaunch.Exceptions.PermissionDeniedException` 异常

## 总结

- Windows
  - Registry - 抛出 `System.UnauthorizedAccessException`
  - StartupFolder - 抛出 `System.UnauthorizedAccessException`
  - TaskScheduler - 错误消息包含HRESULT： `0x80070005`
- Linux
  - Freedesktop - 抛出 `System.UnauthorizedAccessException`
- macOS
  - LaunchAgent - 抛出 `System.UnauthorizedAccessException`
  - AppleScript - 错误消息包含错误码：`-1743`

## 特殊错误信息来源

记录特殊的错误信息怎么查看或者来源

### ScheduledTask(任务计划)

> 转发的 windows 错误码，see: [hresult](https://learn.microsoft.com/zh-cn/windows/win32/seccrypto/common-hresult-values)

### AppleScript osascript

> 没找到相关的文档
> 按照以下步骤来获取错误码信息

1. 将以下脚本保存成 `errs.sh`
2. 添加执行权限 `chmod +x errs.sh`
3. 看某个错误码 `./errs.sh 1743`，看某个范围 `./errs.sh 1700 1800`

```bash
#!/bin/bash

function run_osascript() {
    osascript -s o -e "Error number -$1" | grep -Ev 'execution error: (An error of type -|发生“-[0-9]+”类型错误)'
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
