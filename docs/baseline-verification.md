# macOS 开发基线验证

- 验证日期：2026-10-26（Asia/Shanghai）
- Unity：`2021.3.16f1c1 (56dbfdd6697f)`
- 目标：记录文档体系建立后的当前可构建、可启动基线
- 结论：通过

## 验证环境

- Unity Editor：`/Users/ideabox/Applications/Unity/Hub/Editor/2021.3.16f1/Unity.app/Contents/MacOS/Unity`
- 工程目录：`/Users/ideabox/repo/Swordsman`
- 目标平台：macOS Development Player
- 本地资源：`Assets/Resources/`、`Assets/Sprite/`、`Assets/Texture2D/` 均存在
- 验证前可用磁盘空间：约 496 GiB

以上绝对路径只描述本次本机验证环境，不属于可移植工程配置。

## 场景验证

执行：

```bash
/Users/ideabox/Applications/Unity/Hub/Editor/2021.3.16f1/Unity.app/Contents/MacOS/Unity \
  -batchmode -nographics -quit \
  -projectPath /Users/ideabox/repo/Swordsman \
  -executeMethod SwordsmanSceneValidation.ValidateScenes \
  -logFile /Users/ideabox/repo/Swordsman/Logs/stage3-scene-validation.log
```

结果：

| 场景 | 根对象数量 | Missing Script |
| --- | ---: | ---: |
| `level0.unity` | 8 | 0 |
| `level1.unity` | 4 | 0 |
| `level2.unity` | 4 | 0 |
| `level3.unity` | 4 | 0 |

- Unity 退出码：0
- 汇总：`scenes=4`、`missingScripts=0`
- 未发现 C# 编译错误

## macOS Development Build

执行：

```bash
/Users/ideabox/Applications/Unity/Hub/Editor/2021.3.16f1/Unity.app/Contents/MacOS/Unity \
  -batchmode -nographics -quit \
  -projectPath /Users/ideabox/repo/Swordsman \
  -executeMethod SwordsmanMacBuild.BuildDevelopmentPlayer \
  -logFile /Users/ideabox/repo/Swordsman/Logs/stage3-macos-development-build.log
```

结果：

- Unity 退出码：0
- Build result：`Succeeded`
- Errors：0
- Warnings：0
- Unity 报告大小：6,282,215,644 bytes
- Finder/磁盘占用显示约 5.9 GiB
- 输出：`Builds/macOS/Swordsman.app`

## 启动验证

通过 macOS 启动本轮 Development Build，并进行只读观察：

- 应用进程成功启动。
- Unity 引擎版本为 `2021.3.16f1c1`。
- 渲染设备识别为 Apple M1 Pro / Metal。
- 窗口尺寸为 1280 × 720。
- 开场画面和视频能够显示。
- 视频结束后能够进入标题菜单，显示“初章初始、前历再续、战阵重现、设定帮助”等入口。
- 验证后正常退出应用。
- Player 日志中未发现托管异常、崩溃或资源缺失错误。

本次只验证启动到标题菜单，没有进入新游戏、RPG、营地、战斗、存档或任何历史网络功能，因此不能据此证明完整玩法流程通过。

> 后续目标变更：自 2026-10-26 起，正式构建和 Debug/Development 冒烟窗口统一改以 `1280 × 800`、`16:10` 为验收基线。上面的 `1280 × 720` 只保留为当次历史实测结果，不代表当前目标；新基线尚待工程配置实施并重新验证。

## 开发构建网络行为

Player 日志显示 Development Build 会：

- 启动 Mono 托管调试监听端口。
- 发送 Unity Player discovery 组播。

这是 `BuildOptions.Development | BuildOptions.AllowDebugging` 对应的开发态行为。开发构建只用于本地调试，不得作为发布包。后续发布构建必须确认未启用 Development Build 和 Script Debugging，并重新审查网络行为。

为避免暴露本机网络信息，本文不记录运行日志中的本地地址和动态端口。

## 基线适用范围

该基线可用于后续判断变更是否破坏：

- Unity 脚本编译
- 四个主场景加载
- 场景脚本引用
- macOS Development Build
- 应用启动、开场资源和标题菜单显示

涉及具体玩法、输入、存档、音频完整性、场景切换、战斗逻辑、性能和发布合规时，仍需执行专项验证。
