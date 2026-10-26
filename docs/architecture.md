# 项目架构

- 更新日期：2026-10-26
- 适用基线：Unity `2021.3.16f1c1`

## 总览

项目是场景驱动的单机 Unity 架构。可类比为四个大型 ViewController：每个场景承载一类主流程，多个全局单例共享状态、资源、音频、输入和跳转能力。

```text
level0 Title ───────┐
                    ├── GameSceneManager ── level1 RPG
                    ├── GameSceneManager ── level2 Camp
                    └── GameSceneManager ── level3 Battle

全局服务：GameManager / GameUserData / GameSetting / 音频 / UI / 输入
数据来源：本地 Resources + 场景/Prefab 序列化数据 + 本地存档
历史能力：网络接口 / 云存档 / 激活 / iOS 支付
```

`GameSceneManager` 直接使用 `GameSceneType` 枚举值作为 build index。场景调整必须同时审查枚举、场景管理器、`SwordsmanMacBuild`、所有依赖 active scene index 的代码和发布构建配置。

## 代码模块

- `Base`：基础类型、缓存、定义、设置、日志、历史网络层和单例实现。`GameSetting` 与 `GamePHP` 属于高风险边界。
- `Config`、`Data`：配置及角色、技能、物品、文本、经验、战斗和关卡数据；改动可能影响数值、解析或存档兼容。
- `Manager`：游戏、场景、音乐、电影、UI、相机、输入、事件和触摸管理器。`GameManager.Start` 是主要初始化入口之一。
- `Title`：标题、开始、普通读档、战斗续关和历史云存档入口。
- `RPG`：非战斗地图移动、寻路、事件和场景状态。
- `Camp`：营地、整备、炼化、商店及相关 UI/输入。
- `Battle`：回合、单位、AI、寻路、攻击、技能、地图、事件、UI 和结算，是最大的业务模块。
- `UI`：共享 UI 基类和各场景界面，多数与场景对象或 Prefab 引用紧密绑定。
- `Save`：`GameUserData` 管理玩家状态、本地档、战斗临时档和历史云存档数据，是兼容关键区。
- `Assets/Editor`：macOS 构建和场景验证工具。
- `Assets/Scripts/Editor`：旧资源处理工具；可能重写 importer 或 Prefab，不能作为无副作用诊断运行。

## 资源系统

运行资源主要通过 `Resources.Load` 加载，常见类别为 Material、Movie、Music、Prefab、Sound 和 Texture。`Assets/Sprite/` 与 `Assets/Texture2D/` 还存放大量本地资源。

资源路径相当于公开 API：改变大小写、目录或文件名之前，必须搜索代码引用并在 Unity 中验证。上述大资源目录按项目策略不由 Git 管理。

## 状态与持久化

- `GameSetting` 使用 `PlayerPrefs` 保存触摸缩放、语言、模式、音乐、音效和画质。修改键名会使现有设置丢失。
- `GameUserData` 在 `Application.persistentDataPath` 下保存进度和战斗状态，具有固定容量、槽位和历史格式假设。
- 全局单例和 `GameUserData` 承担场景间状态；`GameSceneLoadMode` 区分新游戏、读档、开始战斗、战斗结束和返回营地等流程。

存档修改前必须记录旧格式、准备旧档样本、明确迁移策略，并验证失败时不会覆盖有效存档。

## 输入与平台层

工程并存键盘、旧 Unity Input、EasyTouch、Input System 相关资产和移动端条件代码。不能推断某个输入体系可直接替换其他体系。

代码包含 `UNITY_IPHONE`、`UNITY_ANDROID` 分支和 `DllImport("__Internal")`。删除前必须确认历史平台是否仍需保持可编译。

## 高风险区域

- 四个主场景及索引
- `GameSceneType`、`GameSceneLoadMode`
- `GameUserData` 与存档格式
- `GamePHP` 及云存档、激活、日志调用
- `Resources.Load` 路径和本地大资源
- 序列化字段、脚本类名、`.meta` GUID
- `ProjectSettings`、插件 DLL 和原生平台代码
- 战斗回合、AI、数值和剧情事件

这些区域的修改必须单独设定边界与专项验收标准。
