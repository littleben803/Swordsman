# 验证指南

- 更新日期：2026-10-26
- Unity 基线：`2021.3.16f1c1`

## 环境前提

当前本机 Unity Editor：

```text
/Users/ideabox/Applications/Unity/Hub/Editor/2021.3.16f1/Unity.app/Contents/MacOS/Unity
```

这是当前机器路径，不写入工程配置。若不存在，应通过 Unity Hub 确认相同版本安装位置，不自动换用其他版本。

完整运行还要求本地存在未由 Git 管理的 `Assets/Resources/`、`Assets/Sprite/`、`Assets/Texture2D/`。

## macOS 窗口与画幅基线

- 正式构建和 Debug/Development 冒烟测试都使用 `1280 × 800` 窗口，画幅为 `16:10`。
- 游戏逻辑视口保持 `320 × 200`。验证目标是按整数倍显示原画幅，不扩大可见逻辑范围，不拉伸，也不裁切上下内容。
- 冒烟启动时显式传入窗口参数，避免本机历史 `PlayerPrefs` 或上一次退出时的窗口状态干扰结果：

```bash
open "Builds/macOS/Swordsman.app" --args \
  -screen-width 1280 \
  -screen-height 800 \
  -screen-fullscreen 0
```

- 正式构建还必须把相同尺寸写入正式构建默认配置，使用户首次启动且没有命令行参数时也得到 `1280 × 800`。当前工程仍是 `1920 × 1080`、非 Windowed 配置，这一项尚未实施，不能仅凭冒烟参数认定正式默认配置已完成。
- 验证时记录实际渲染区域和原生窗口内容区；macOS 标题栏不计入 `1280 × 800` 游戏内容尺寸。

## 一级：文本和仓库检查

适用于纯文档或明确不影响 Unity 的修改：

```bash
git diff --check
git diff --stat
git status --short --branch
```

检查无关变化、本地资源或构建产物、`.meta` 异常、大量序列化变化和敏感信息。

## 二级：脚本编译和场景验证

关闭正在占用项目的 Unity Editor 后执行：

```bash
UNITY_EDITOR="/Users/ideabox/Applications/Unity/Hub/Editor/2021.3.16f1/Unity.app/Contents/MacOS/Unity"
"$UNITY_EDITOR" \
  -batchmode -nographics -quit \
  -projectPath "/Users/ideabox/repo/Swordsman" \
  -executeMethod SwordsmanSceneValidation.ValidateScenes \
  -logFile "/Users/ideabox/repo/Swordsman/Logs/swordsman-scene-validation.log"
```

成功标准：退出码为 0；日志包含 `SWORDSMAN_SCENE_VALIDATION_COMPLETE`、`scenes=4`、`missingScripts=0`；没有 C# 编译错误。该工具只验证场景可加载和 Missing Script，不等于玩法测试。

## 三级：macOS Development Build

```bash
UNITY_EDITOR="/Users/ideabox/Applications/Unity/Hub/Editor/2021.3.16f1/Unity.app/Contents/MacOS/Unity"
"$UNITY_EDITOR" \
  -batchmode -nographics -quit \
  -projectPath "/Users/ideabox/repo/Swordsman" \
  -executeMethod SwordsmanMacBuild.BuildDevelopmentPlayer \
  -logFile "/Users/ideabox/repo/Swordsman/Logs/swordsman-macos-development-build.log"
```

输出为 `Builds/macOS/Swordsman.app`。成功标准：退出码为 0；日志包含 `SWORDSMAN_MAC_BUILD result=Succeeded` 和 `errors=0`；应用存在且可启动。此脚本启用 Development 和 AllowDebugging，不是发布构建。

## 人工冒烟测试

按变更范围选择，基础路径建议覆盖：

1. 启动并进入标题界面。
2. 验证键盘方向/WASD、Return 和 Escape 的相关交互。
3. 开始新游戏或读取本地存档。
4. 在 RPG 场景完成移动或事件。
5. 进入营地并打开相关界面。
6. 进入战斗，完成至少一回合或目标操作。
7. 正常离开战斗并返回预期场景。
8. 涉及存档时完成保存、退出、重启和读取。
9. 确认音乐、音效、视频和主要贴图没有缺失。
10. 分别对正式构建和 Debug/Development 构建确认游戏内容区为 `1280 × 800`、画幅为 `16:10`，不存在横向拉伸、上下裁切或额外逻辑视野。

默认测试不使用真实云账号、支付账号或线上服务。

## 验证矩阵

| 修改类型 | 最低验证 |
| --- | --- |
| Markdown | diff check、链接、Git 状态 |
| 独立 C# 工具 | Unity 编译、工具专项测试 |
| `MonoBehaviour`/序列化字段 | 编译、场景验证、相关冒烟 |
| 场景或 Prefab | 场景验证、Development Build、相关流程 |
| 资源路径或资源 | Development Build、引用路径冒烟 |
| 场景顺序或切换 | 四场景验证、构建、完整切换链路 |
| 存档 | 旧档读取、新档写入、重启读取、失败测试 |
| 网络/云/激活/支付 | 专项方案、明确授权、隔离环境、合规检查 |
| ProjectSettings/插件/Unity | 全量导入、场景验证、构建、平台专项测试 |

## 失败与日志

- `Logs/` 不提交 Git；历史日志只能证明过去状态。
- 汇报时注明“本轮运行”或“既有日志”。
- 警告不得自动批量修复，应判断是否由本次修改引入。
- 失败时保留关键错误，判断是本次修改、资源缺失、版本不匹配还是已有问题，只修复已批准范围。
- 不默认删除 `Library/`、重导全资源或升级 Unity；这些操作必须先说明并获得确认。
