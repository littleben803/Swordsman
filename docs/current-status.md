# 项目当前状态

- 更新日期：2026-10-26
- 技术基线：Unity `2021.3.16f1c1`
- 目标平台：macOS
- 状态：基于当前本地代码、配置和既有日志完成初次盘点

## 已确认事实

Swordsman 是《天地劫：神魔至尊传》的 Unity 重制工程，包含标题、RPG 探索、营地和战斗四个主要场景。当前架构以场景和多个全局单例驱动，UI 使用 UGUI，资源主要通过 `Resources.Load` 按字符串路径加载。

| 索引 | 场景 | 枚举 | 职责 |
| --- | --- | --- | --- |
| 0 | `Assets/Scenes/level0.unity` | `Title` | 标题、开始和读档入口 |
| 1 | `Assets/Scenes/level1.unity` | `Rpg` | 地图探索与剧情事件 |
| 2 | `Assets/Scenes/level2.unity` | `Camp` | 营地、整备和商店 |
| 3 | `Assets/Scenes/level3.unity` | `Battle` | 战斗、回合和结算 |

`ProjectSettings/EditorBuildSettings.asset` 当前场景列表为空。macOS 构建脚本通过固定数组传入上述场景，因此顺序不能随意调整。

玩家设置使用 `PlayerPrefs`；游戏进度由 `GameUserData` 写入 `Application.persistentDataPath`。工程保留 EasyTouch、旧 Input API、本地插件 DLL、iOS 原生桥接和历史网络实现。

## 本地资源策略

以下大体积运行资源只保留在项目所有者本地，不由 Git 管理：

- `Assets/Resources/`
- `Assets/Sprite/`
- `Assets/Texture2D/`

这是既定策略，不属于待修复问题。单独克隆 Git 仓库不能得到完整可运行工程；新机器必须由项目所有者另行提供资源。不得擅自修改 `.gitignore`、引入 Git LFS 或强制提交这些目录。

本次盘点时本地 `Assets/` 约 3.6 GB；该数字仅描述 2026-10-26 的工作副本快照。

## 已有验证证据

本地历史日志及 2026-10-26 基线验证显示：

- 四个主场景均曾成功打开，`missingScripts=0`。
- 2026-10-26 已重新验证四个场景与 macOS Development Build，结果通过；详见 `baseline-verification.md`。
- 一次成功构建产物约 6.28 GB。
- 较旧日志中存在 `UnityWebRequest.isNetworkError`、`isHttpError` 弃用警告。

基线只证明验证时的状态。修改代码、场景、资源或配置后必须重新验证，不能将旧结果当成本轮实时结果。

## 主要技术债与风险

- **资源路径耦合**：大量字符串形式的 `Resources.Load`，改名、移动或大小写变化可能只在运行时失败。
- **动画与地图 Shader 已恢复**：2026-10-26 已从同级 `Sword` 原始工程恢复 Type0～Type7 及 `Game/Map` 的透明混合逻辑；动画 Shader 已通过构建与战斗画面验证，地图 Shader 已通过场景与构建验证，第 0 幕末尾森林叠加层待人工复测。
- **Unity 序列化耦合**：场景和 Prefab 依赖脚本 GUID、类名、字段名与类型；代码能编译不等于引用完整。
- **全局状态较多**：多个单例的初始化顺序和激活状态形成隐含依赖。
- **存档兼容风险**：`GameUserData` 包含固定尺寸、槽位、战斗临时档和历史云存档逻辑。
- **历史平台代码**：仍有 iOS、Android、云存档、激活、日志上传和支付实现，不能直接认定无用并删除。
- **发布配置未确定**：当前 `companyName`、Bundle Identifier、正式版本、签名和公证流程尚未确定。
- **授权待核查**：Apache 2.0 根许可证不能自动覆盖原游戏内容和第三方素材。

## 待确认事项

- macOS 最终发行范围与渠道。
- 历史云存档、激活、日志上传和支付功能的去留。
- 旧存档需要兼容到什么版本。
- 本地大资源在新机器间的交接和版本标识方式。
- 第三方 DLL、EasyTouch 和原游戏内容授权清单。
- 正式 Bundle Identifier、版本、签名与公证方案。

以上事项在用户确认前只能调查和记录，不能由维护者自行作产品决定。
