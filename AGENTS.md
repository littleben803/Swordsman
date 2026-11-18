# Swordsman 项目协作规范

本文件是项目对 AI 开发环境和维护人员的最高优先级仓库级协作入口。项目事实、约束或工作流变化时，应同步更新本文件或 `docs/` 对应文档，避免关键上下文只存在于聊天记录中。

## 沟通与工作方式

- 默认使用中文。用户是资深 iOS/macOS 工程师，但不熟悉 Unity、游戏引擎和前端体系。
- 先给专业结论，再用客户端开发类比解释必要的 Unity 概念；不重复解释基础 iOS/macOS 概念。
- 结论必须区分：已从代码或配置确认、仅由旧日志证明、仍需人工确认。
- 不要一上来写代码。每项开发任务开始前必须明确：产品目标、技术方案、任务边界、允许修改的文件、不允许修改的文件、验收标准。
- 按阶段小步执行。每完成一个阶段，先汇报结果并等待用户确认，再进入下一阶段。
- 修改应聚焦、可审查、可回滚；不做顺手重构。

## 产品定位

- 项目名称：Swordsman，《天地劫：神魔至尊传》2026 macOS 重制版。
- 当前主要目标平台是 macOS。
- 当前目标是保持原有玩法、数据和资源表现，在可验证的前提下逐步恢复、适配和维护；不得自行重新设计玩法或数值。
- 代码中仍保留旧 iOS、Android、云存档、激活和支付相关实现。它们属于历史能力，不代表当前 macOS 版本的正式产品承诺。

项目现状与已知问题见 [docs/current-status.md](docs/current-status.md)。

## 技术基线

- Unity：`2021.3.16f1c1`，优先使用完全一致的版本。
- UI：Unity UI（UGUI）。
- 输入：工程同时保留旧 `Input` API、EasyTouch 和本地 DLL 形式的 Input System 资产；不得默认升级或替换输入方案。
- 主场景顺序必须保持：
  1. `Assets/Scenes/level0.unity`：Title
  2. `Assets/Scenes/level1.unity`：RPG
  3. `Assets/Scenes/level2.unity`：Camp
  4. `Assets/Scenes/level3.unity`：Battle
- `GameSceneType` 枚举值、场景索引和构建脚本场景数组相互耦合，未经专项批准不得调整顺序。
- `ProjectSettings/EditorBuildSettings.asset` 当前没有预置场景；命令行构建由 `Assets/Editor/SwordsmanMacBuild.cs` 显式提供场景。
- macOS 正式构建与 Debug/Development 冒烟测试统一使用 `1280 × 800` 窗口，保持原生 `16:10` 画幅。游戏逻辑视口仍为 `320 × 200`；未经专项批准不得改成 `16:9`、拉伸画面、裁切上下内容或扩大逻辑视口。

详细架构见 [docs/architecture.md](docs/architecture.md)。

## 仓库与资源策略

- 高清美术仅执行 [静态美术 MVP](docs/static-art-mvp.md) 和唯一机器清单 [语义白名单与退役登记](docs/static-art-mvp-resources.json)，使用明确语义分类，不再使用字母类别。静态地图/前景、全部Floor_x攻击与技能特写固定地形变体、对话人物图、场景标题、界面与其他非排除美术纳入高清目标，先盘点入唯一清单再按阶段制作。角色（含NPC、怪物）、法术、招式相关的Sprite动画帧原样复用。除独立角色Sprite及角色相关法术、招式、行动序列外，其他美术原则上纳入重绘目标；宝物闪光等非角色环境提示动画不得按全部动画排除。界面动画、箭头、升级提示和宝物闪光按原播放系列逐帧重绘，逐组提供OLD／高清GIF及逐帧PNG对照，运行接入单独验收。直接绘制或烘焙在获准静态背景/固定覆盖片内的人物、NPC和遗骸须随背景高清重绘，不保留原像素；保持原服饰配色、体型、朝向和大致姿态，不把人形误画为器物。
- 旧高清方案、标准、角色流程、工具/项目Skill及安装软链接、实验资源（含已批准角色重绘）和重复中间产物已于2026-10-31统一清理，不再维护或恢复旧路线；工作区外旧技能或记忆也不能覆盖新MVP。当前仅保留必要最终成果、原始生成输入和最小证据，状态以唯一机器清单为准。不得擅自删除其他原版游戏资源和当前有效静态成果。2026-11-16用户已专项批准清理完成高清替换及引用迁移的611张旧纹理、1971个无引用Sprite及2582个对应meta；精确范围见机器清单`referenceMigration.cleanup`，48张混合图集不动。退役源的无像素几何/来源哈希由`docs/hd-retired-source-geometry.json`保留并经Editor源码SHA锁定，用于高清重建，不恢复OLD回退。2026-11-12用户已明确覆盖旧应用保留规则：历史、实验、普通/静态预览及独立测试安装包全部退役，只保留 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`。
- 静态美术先补齐类型样板，再逐幕推进与验收，共用资源跨幕复用。唯一清单使用共享待办，不分配到人、不设负责人/领取/人员锁；AI开始前读最新状态，开始时标进行中，检查及对应确认通过后勾选完成并附证据，只更新相关记录。独立任务可并行，同源图集不得同时修改；Unity和Bundle构建逐次执行。具体规则见静态MVP，待办标记不代替阶段授权。
- Git 只管理代码、场景、工程配置和少量必要资产。
- `Assets/Resources/`、`Assets/Sprite/`、`Assets/Texture2D/` 体积很大，只保存在本地，按既定策略不纳入 Git。
- 不得把上述本地资源视为仓库损坏，也不得擅自修改 `.gitignore`、启用 Git LFS、批量 `git add -f` 或上传这些资源。
- 新 AI 环境或维护机器必须由项目所有者另行提供本地资源；Git 克隆本身不保证可运行。
- 不得删除、移动、批量重命名或重新导入本地资源。大量代码依赖 `Resources.Load` 字符串路径，Unity 场景和 Prefab 还依赖 GUID。
- Unity 资源修改必须连同对应 `.meta` 文件处理；不得手工生成新 GUID 覆盖现有 `.meta`。
- `Library/`、`Logs/`、`UserSettings/`、`Build/`、`Builds/` 及 IDE 生成文件不得提交。
- 不得提交模型、大型二进制、构建产物、签名文件、证书、私钥、token 或本地环境文件，除非用户明确允许。

## 代码修改纪律

- 未经明确允许，不新增第三方依赖，不升级 Unity 或 Package。
- 未经明确允许，不修改业务逻辑、战斗规则、剧情、关卡、数值、存档格式或资源路径。
- 未经明确允许，不修改 `ProjectSettings/`、`Packages/`、场景、Prefab、Shader、插件 DLL 和原生插件。
- 不修改签名、Bundle ID、Team、Version、Build、商店配置或发布能力。
- 不跨目录重构，不批量格式化旧代码，不统一命名风格，不顺手消除全部警告。
- 保持现有序列化字段名和类型。重命名或改变 `MonoBehaviour` 字段可能使场景/Prefab 引用丢失。
- 保持旧存档兼容。涉及 `GameUserData`、存档槽、二进制读写、`PlayerPrefs` 键或 `Application.persistentDataPath` 时，必须单列兼容风险并获得确认。
- 避免直接编辑 `.unity`、`.prefab`、`.asset` YAML；必须编辑时，先说明原因和回滚方案并运行场景验证。
- 编辑器工具可能批量修改 importer、Prefab 或资源；运行 `GameToolsMenu` 命令前必须获得明确授权。

## 隐私、安全、网络与合规

- 不保存、打印、提交或回显密钥、token、证书、Apple Developer 凭据、签名资产和私钥。
- 未经明确允许，不新增或启用网络请求、账号、登录、服务端、在线 AI、分析 SDK、广告追踪、StoreKit、CloudKit、iCloud 同步或敏感权限。
- 代码中存在历史网络接口、云存档、激活、日志上传和 iOS 支付逻辑。修改前必须确认服务状态、数据流向、隐私声明和目标平台行为。
- 不在文档、日志或汇报中抄录历史接口的敏感参数、用户标识或服务地址。
- 根目录 `LICENSE` 不能自动证明原游戏名称、商标、剧情、图片、音乐、视频、字体、插件和第三方内容均可发行。对外发布前必须独立核查。

详见 [docs/security-privacy-and-licensing.md](docs/security-privacy-and-licensing.md)。

## 开发和验证

- 开始前检查 `git status --short --branch`，识别用户已有修改；不覆盖或回退无关内容。
- 优先通过 Unity Editor 修改场景、Prefab 和序列化资产，通过文本工具修改 C# 与 Markdown。
- 按风险逐级验证：静态检查 → Unity 脚本编译 → 场景验证 → Development Build → 人工冒烟测试。
- 唯一应用产物为 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`，唯一Player构建入口为 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`。Unity直接构建到此路径；不得先构建临时应用再复制覆盖，不再生成独立实验/对比/测试应用，也不复制完整 `.app` 作为历史或回退包。
- 保留高清Bundle、增量导入缓存、原始输入/批准稿、评审与最小日志证据；它们不是可按旧安装包清理的文件。验证和耗时对比也复用唯一应用，不创建新应用名。已有应用正在运行时，不强行终止未保存进度；无须重建的任务保留当前包，只做脚本编译/场景验证。
- 构建产物统一放入 `Builds/`，日志放入 `Logs/`；二者均不提交。
- 当前编辑器路径为 `/Users/ideabox/Applications/Unity/Hub/Editor/2021.3.16f1/Unity.app/Contents/MacOS/Unity`。若不存在，应让用户确认新路径，不擅自升级版本。
- 标准命令和验收矩阵见 [docs/verification.md](docs/verification.md)，完整流程见 [docs/development-workflow.md](docs/development-workflow.md)。

## 文档归档和地图

- 根目录 `README.md`：面向首次打开仓库的人，保留简短项目介绍、打开方式和基本运行入口。
- 根目录 `AGENTS.md`：维护长期有效的约束、边界、验证要求和文档地图。
- `docs/`：归档开发产生的分析、架构、决策、调试、验证、迁移和发布文档；无需创建 `docs/README.md`。
- 不为同一事实建立多个副本；确定权威位置，其余只链接。
- 文档文件名使用小写英文和连字符，正文默认中文，并写明日期、适用版本或验证状态。
- 推测必须标记“待确认”。架构、数据格式、构建、安全边界或流程变化时，必须同步更新文档。

文档地图：

- [README.md](README.md)：项目简介、打开方式和基础构建说明。
- [docs/current-status.md](docs/current-status.md)：当前状态、技术债、风险和待确认事项。
- [docs/architecture.md](docs/architecture.md)：场景、模块、资源加载和持久化架构。
- [docs/development-workflow.md](docs/development-workflow.md)：阶段协作、修改纪律和交付流程。
- [docs/verification.md](docs/verification.md)：场景验证、macOS Development Build 和人工验收。
- [docs/baseline-verification.md](docs/baseline-verification.md)：2026-10-26 macOS 构建与启动基线。
- [docs/shader-restoration.md](docs/shader-restoration.md)：动画透明混合 Shader 的迁移恢复与验证记录。
- [docs/static-art-mvp.md](docs/static-art-mvp.md)：唯一有效高清美术文档；语义分类、生产边界、样板、共享待办、逐幕推进、验收和最小保留规则。
- [docs/static-art-mvp-resources.json](docs/static-art-mvp-resources.json)：唯一高清美术机器清单；语义白名单、候选、切片、引用证据、待办状态/依赖、逐幕验收与清理登记。
- 旧高清方案、标准、角色流程、生产清单和美术接入说明已统一清理，不再维护历史正文，不作为当前规范。
- [docs/security-privacy-and-licensing.md](docs/security-privacy-and-licensing.md)：历史网络能力、隐私、安全和授权边界。

## 阶段汇报格式

每阶段完成后必须汇报：

- 本阶段是否完成
- 新增了哪些文件
- 修改了哪些文件
- 明确哪些重要区域未修改
- 运行了哪些验证命令
- Debug 构建结果；未运行时写明“未运行”及原因
- 是否有未解决问题
- `git status` 是否存在异常文件
- 是否建议进入下一阶段

## 禁止的默认行为

- 不因旧代码、弃用 API 或警告就自动升级、重构或清理。
- 不自动修复全部 Missing Script、序列化引用或资源 importer。
- 不连接历史线上服务或使用真实账号测试。
- 不改变本地大资源的版本控制策略。
- 不把历史日志结果描述为当前修改后的实时验证。
- 不提交、推送、创建分支、打标签或发布构建，除非用户明确要求。
