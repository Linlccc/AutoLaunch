# 发布步骤

1. 在 change-log.md 中记录本次发布的变更内容。【可选】
2. 修改 `Directory.Build.props` 文件中的 `version` 属性。【可选，发布时不依赖】
3. 在要发布的提交上创建 Git 标签（Tag），格式为 `v{version}`，例如 `v1.0.0` 或 `v1.0.0-preview.1`。
4. 推送 Git 标签到 GitHub。
5. 在 GitHub 上发布 Release。
