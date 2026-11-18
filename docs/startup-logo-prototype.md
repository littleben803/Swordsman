# 启动计时与最小 Logo 原型

## 2026-11-17 设计师v2片头资源交接（已构建，待用户实机验收）

设计师v2的`Movie/Title`仍为1280×800、24fps、130帧，首帧已有星光。`GameMovieManager`沿用片头专用1280×800 RenderTexture，在第0帧完成一次视频渲染后撤公司Logo；剧情视频继续使用原共享320×200目标。唯一AllChapters已更新并通过实际Player自动启动检查，证明Logo到视频首帧再到标题场景的顺序；高清视觉、音效听感和末帧衔接仍待用户实机验收。v2来源、SHA、压缩与图集见 [title-animation-hd.md](title-animation-hd.md) 顶部记录；下文原片第0帧全黑的结论只适用于旧视频。

## 当前安装包规则（2026-11-12，覆盖下文历史产物说明）

用户明确只保留 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`。历史、实验、普通/高清预览、独立测试及CacheBenchmark应用已全部退役并清理，不保留整包回退副本。唯一构建入口为 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`，Unity直接输出到该路径；不得先生成另一份应用再复制覆盖，不再创建其他 `.app` 名称。旧入口及 `-swordsman-cache-benchmark` 参数已移除/拒绝。

资源Bundle、稳定导入缓存、原版资源、批准稿/原始生成输入、评审和最小验证证据继续保留。下文旧应用名、独立输出及保留说明仅是历史记录，不是当前保留清单或后续打包指令。本轮清理未改变资源、美术或用户实机验收状态。


## 公司 Logo 启动页（2026-11-13，当前方案）

用户确认 `Builds/design/company-logo-20261113/ideabox-company-startup-v1.png` 为终稿并授权接入、验证和更新唯一AllChapters应用。原文件2560×1600 RGBA逐字节复制到 `Assets/Resources/StartupPrototype/PortraitStill.png`，沿用原GUID。Unity单图导入为RGBA32未压缩、NPOT None、无mipmap、Alpha FromInput/Transparency、Bilinear/Clamp，Standalone max4096，避免缩小批准稿。

完整PNG画布在1280×800逻辑区域居中等比显示，沿用PNG自带留白、英文粗体及较小中文。启动层仅绘制深色背景和这一张公司PNG；删除原副标题、正在载入及循环点号，不再依赖GUI字体。窗口尺寸和游戏320×200逻辑视口不变。

原 `Movie/Title` 文件、音轨、准备和播放逻辑不变，无固定展示等待；其第1内容帧经过一次EndOfFrame渲染后撤掉公司Logo。现有跳过、错误/缺片回调保持。不修改Unity原生Splash、构建后处理、场景、Prefab、ProjectSettings或游戏玩法/存档。

本轮验证与交付证据统一放在 `Logs/startup-company-logo-20261113/`。ConfigureAndValidate脚本编译、批准PNG哈希/GUID/实际2560×1600 RGBA32/16:10几何和四主场景Missing Script=0通过，Editor首内容帧/撤层/跳过/缺片回调通过。首包实际运行暴露右上角全局HD标记仍覆盖启动页，因此仅给HD调试覆盖层增加启动页可见性判断；公司页隐藏，撤层后恢复原有行为，不写PlayerPrefs。修正后Editor再次通过，唯一应用已直接重新构建成功，0错误/9个既有警告，1819项HD缓存全复用、imported=0。最终独立Player退出0、报告passed=true，实际2560×1600启动截图确认HD标记已隐藏，原片头结束进入菜单淡入。严格签名检查、2044项保护文件（其余代码/场景/Prefab/Shader/配置/视频）哈希和唯一应用检查通过。Git状态路径与本轮开始一致，无本轮新增异常；未提交或推送，2026-11-13用户视觉验收通过。Unity Development Build自带右下角水印保留，不修改引擎或构建类型。首包机器报告不作为最终纯Logo画面验收结果。

### 用户验收与正式包收尾（2026-11-13）

用户明确反馈“验收没问题，就用这版”。当前唯一应用中的公司Logo启动页及原片头衔接已获实机验收，本项完成并沿用本版；不扩大为全游戏或正式发行验收。清单 `startupStaticLogo` 已标记已完成/done=true。

`Development Build` 是开发包水印；正式使用非Development构建时不显示，无需修改Logo图片。目前唯一构建入口仍固定设置 `BuildOptions.Development | BuildOptions.AllowDebugging`，本轮不修改或重建现役应用。

2026-11-13后续授权已将公司Logo纳入默认macOS启动路径：`Supported` 仅判断macOS平台，`Enabled = Supported && !Application.isBatchMode`，不再依赖 `DEVELOPMENT_BUILD`、章节测试标志或 `-swordsman-startup-prototype` 参数。macOS正式包、开发包和Editor图形运行均默认显示；其他平台及无图形批处理不启用。PNG/GUID/排版、无额外文字/Loading、无固定等待、原片头首内容帧后撤层及跳过/缺片处理不变。

仅修改启动条件和对应验证工具，不改图片、视频、场景、工程设置、构建入口或后处理。当前已验收的唯一应用保持原包，未终止正在运行的游戏，未构建Development或正式Player。正式包将来仍需实际运行复验水印与完整启动；本轮源码/脚本验证不等于正式包实机验收。

新增 `SwordsmanStartupPrototypeValidation.ValidateReleaseScripts`：通过Unity内置 `PlayerBuildInterface.CompilePlayerScripts` 仅编译macOS Player脚本，`ScriptCompilationOptions.None`、额外标志为空，不构建Player或修改配置。反射检查新编译的实际正式脚本程序集：批处理supported=true/enabled=false，图形进程supported=true/enabled=true，均通过。无启用参数的Editor Play Mode回归已通过，确认Logo默认出现、原片头首内容帧后撤层、跳过和缺片单次回调；证据 `Logs/startup-company-logo-default-20261113/`。首次图形进程在引擎Metal初始化卡住（未执行脚本），停止该验证进程后仅用命令行 `-force-gfx-direct` 重跑，未改工程配置。 2356项保护文件及现有应用全部文件哈希不变，Git无本轮新增异常。完整命令、首次失败说明及验证边界见 `Logs/startup-company-logo-default-20261113/report.md`；正式包实机与水印检查仍待正式构建阶段。

参考：[Unity构建模式说明](https://docs.unity3d.com/ja/current/Manual/building-introduction.html)。此前用户验收归档仅更新文档/清单；后续默认macOS启用条件变更已按上述单项授权完成。未运行Debug/Release Player构建，保留已验收应用。

以下2026-11-12人物图尺寸与文字点态仅为历史记录，已被当前公司Logo方案替代。

## 静态启动 Logo（2026-11-12，历史方案）

用户明确批准用附件PNG替换人物动画。原图1008×1792 RGBA，仅在左右各补8、上下各补128透明像素，得到1024×2048；原图像素及Alpha逐字节不变。原始附件归档在 `Builds/HD4X/static-mvp/inputs/startup-static-logo-20261112/`，运行纹理沿用 `Assets/Resources/StartupPrototype/PortraitStill.png` 及原GUID。Unity实际导入为未压缩RGBA32，max2048、NPOT None、无mipmap、Alpha FromInput/Transparency、Bilinear/Clamp，不再得到旧的512×512纹理。纹理尺寸不限制UI显示尺寸。

在1280×800逻辑画面中，Logo原图内容区域等比显示为270×480，水平居中、顶部44；UV排除透明补边。启动背景铺满窗口，整体逻辑排版使用统一缩放，避免初始化瞬间纵横分别缩放。原副标题y572、载入提示y690和0.4秒点态保持，游戏320×200逻辑视口及16:10画幅不变。

启动层在首场景前同步加载静态PNG，从首个Unity可绘制帧保持同一图片；不创建人物VideoPlayer/RenderTexture，不加载或播放 `StartupPrototype/SwordPortrait`，不再保留人物视频展示/循环等待。2026-11-16经用户确认，已删除无用的 `SwordPortrait.mp4` 及对应 `.meta`；静态 `PortraitStill.png` 与其 `.meta` 保留，其他音视频不调整。引擎尚不可绘制的启动阶段仍不保证已有游戏Logo，未添加原生窗口或更改Splash配置。

原 `Movie/Title` 视频保持原文件和音轨。Prepare完成即Play，不设固定停留；已核对原片第0帧全黑，第1帧已有内容，所以收到第1帧后启用原RawImage，等一次EndOfFrame渲染完成再撤静态Logo。缺片、超时和跳过沿既有回调退出覆盖层；剧情内视频播放路径不变。

验证工具改为检查静态PNG的实际尺寸、格式、GUID、显示区域、原视频解码和跳过/缺片回调。快速启动不强制覆盖三个点态，不以人为延时换取测试通过。当前像素/Alpha校验、脚本编译、四场景Missing Script=0、Editor实际首内容帧/撤层/跳过/缺片检查均通过。证据统一位于 `Logs/startup-static-logo-20261112/`。

独立AllChapters已直接构建并更新，耗时341.868秒（约5分42秒），Succeeded、0错误/9个旧代码警告。1819项HD资源全部命中缓存，imported=0、removed=0；输入校验121.957秒，含Bundle校验的资源阶段153.614秒。包内HD Bundle SHA256保持 `9c70db2bdd0b5fc0c71c778b8c3b4bf3181104f928af6ae9d47e9d17185657a6`。未生成其他应用或整包备份。

实际运行唯一应用，未附加prototype/hold参数，进程退出0，`player-runtime.json` passed=true：静态Logo先绘制、无人物VideoPlayer、原片首内容帧后撤层、原开场结束回调均通过。包内 `resources.assets` 独立回读确认 `PortraitStill` 为1024×2048/RGBA32；助手已观察2560×1600启动页实拍和原开场结束后的菜单淡入截图。1538项受保护文件（工程配置、场景、Prefab、Shader、原视频、附件原图）哈希不变，原GUID和原图RGBA像素不变，严格签名检查通过。实际视觉与流畅度仍待用户复验，不宣称引擎可绘制前已有Logo。

本阶段新增仅原始PNG/来源归档及最小验证记录；修改3个既有启动/验证C#、1张Logo及其导入meta、启动说明/当前状态/验证指南/唯一清单。玩法、存档代码、场景、工程配置、原开场/人物MP4和HD运行加载逻辑未改。Git保留阶段开始已有修改，无新增异常路径，未提交或推送。


下文旧视频启动及效果包描述仅为历史记录，不能作为当前构建和运行说明。

日期：2026-11-12。Unity 2021.3.16f1c1 / macOS。状态：隔离原型；默认启动行为不变，未接入或覆盖独立验收包。

## AllChapters轻量启动层取消固定停留（2026-11-12，静态替换前历史记录）

用户明确要求现有Unity Splash后处理行为不变。本轮未修改后处理、原构建入口或ProjectSettings；也没有执行许可标记改写，因此未重新构建或覆盖AllChapters应用。磁盘上的旧效果包仍有6.4秒停留，不得描述为已更新。

源码已根据既有AllChapters编译标志SWORDSMAN_CHAPTER_TESTS_LARGE_WINDOW默认启用轻量启动层（无图形批处理除外），不需要新增场景或修改构建标志。普通其他包默认路径不变。非展示模式WaitForPresentation直接返回false，原开场Prepare完成即请求播放，不再等人物片源首帧或一圈循环；展示包原有6.4秒模式单独保留。人物动画、文字副标题及0.4秒点态沿用，原开场首内容帧到达后撤掉覆盖层。

实际Editor RunPlayMode无hold参数检查通过，包含原开场首内容帧、撤层、跳过和缺片兜底。Prepare完成至Play请求耗时见 `Logs/startup-no-hold-20261112/verification.json`；不宣称独立应用冷启动速度。只修改启动原型、Editor检查及说明；没有新增第三方依赖。独立应用更新仍未完成。

## 人物巨剑动画效果包（2026-11-12，展示版历史记录）

用户确认中央使用抠像后的人物巨剑动画，不使用金色天地劫流光Logo。运行载体采用同版深色合成MP4，保持480×624、24fps、145帧和柔和边缘；不是带透明通道的视频。背景固定RGB(14,17,21)，与合成底色相同。Grok源音轨在启动层静音，原开场动画仍保留自己的既有播放行为。局部白晕与剑气饱和度变化属于已交付抠像预览的已知限制，本轮不重做抠像。

副标题保持实时文字“神魔至尊传 · HD 重制版”。底部独立绘制固定起点的“正在载入”和点号区域，以实时钟每0.4秒循环.、..、...；按三个点的最大宽度预留空间，不随点数居中偏移。中央竖图在1280×800逻辑画面中等比显示，区域约369×480，副标题与底部提示位于其下。

人物视频和占位图异步加载；解码首帧后循环播放。预览包单独使用SWORDSMAN_STARTUP_PREVIEW编译标志，默认启用此启动层，并在人物动画首帧后保留6.4秒便于观察约一圈循环。原开场视频可提前Prepare，等待展示结束后才开始Play，不在遮挡层后偷偷播完。这个额外展示时长只用于效果包；正常编译不带该标志时仍需原型参数启用，不改变普通默认启动。

新增独立官方构建入口SwordsmanStartupPreviewBuild.Build，输出 `Builds/macOS/Swordsman-StartupPreview.app`，窗口1280×800，主场景顺序不变。复用已校验HD Bundle和隔离章节测试数据；不覆盖AllChapters。它直接调用Unity BuildPipeline，保留工程原Splash设置，不调用既有许可标记改写后处理；因此此包可能显示Unity原生启动标识，不承诺隐藏。

本轮新增预览构建工具、StartupPrototype本地MP4/占位PNG及Unity生成的meta；修改启动原型、原开场Prepare等待衔接和现有Editor检查，修改本记录、current-status与verification。无新依赖、玩法、存档或场景序号修改。

Editor实际解码检查通过人物视频首帧与循环、三个加载点态、原开场首内容帧、提前跳过、缺片单次回调。日志与报告位于 `Logs/startup-artwork-integration-20261112/`。独立Development包已构建成功，0错误/9个既有警告，日志 `Logs/startup-artwork-integration-20261112/build.log`。实际图形进程退出0，人物视频首帧/完整循环、点态覆盖7（.、..、...）、原开场首内容帧及结束回调全部通过；1280×800实际截图已观察，原开场结束后显示菜单。运行报告 `runtime.json`，交付汇总 `delivery.json`，截图 `player-captures/portrait-loading-2.png`，均位于上述证据目录。codesign深度严格验证通过；场景、ProjectSettings、原构建入口及许可后处理源码哈希与本轮开始相同，原Title.mp4与第一阶段记录哈希相同。未调用许可后处理，未覆盖AllChapters。

本轮执行 `SwordsmanStartupPrototypeValidation.RunPlayMode`、`SwordsmanStartupPreviewBuild.Build`、独立Player的 `-swordsman-startup-report` 与 `-swordsman-startup-capture`、`codesign --verify --deep --strict`、保护文件SHA256和范围内 `git diff --check`。首次独立进程在系统启动验证阶段等待较长，后来正常进入；没有把这次等待时长解释成Unity加载耗时或宣称已消除全部黑屏。当前阶段完成，待用户实机验收；本轮Git没有新增异常路径，未提交/推送。

以下“文字Logo”与“未打包”描述为第一阶段历史记录，不代表当前效果包内容。

## 目标与边界

在保留原开场动画的前提下，记录启动分段时间，并用文字 Logo 承接可渲染后的等待。仅修改标题启动调用、视频准备衔接，新增原型组件与编辑器检查。无新场景、Prefab、字体/图像/视频资源、依赖、网络或存档写入；不改变主场景索引、原游戏数据、签名和工程配置。

本轮发现现有构建后处理通过改写hasPROVersion等构建字段隐藏Unity Logo。原文件保留，本轮没有调用或扩展该路径。公开编辑器API报告hasProLicense=False；未以此绕过限制制作独立包。后续独立应用“不出现Unity Logo”必须采用许可证支持的正式配置。旧记录中的补丁和旧包结果不代表本轮验收。

## 启用方式

- `-swordsman-startup-timing`：只记录原启动路径。
- `-swordsman-startup-prototype`：启用Logo与视频Prepare衔接，并记录计时。
- 编辑器附加 `-swordsman-startup-capture`：保存实际Game View绘制截图，输出到 `Logs/startup-prototype-20261112/captures/`。

功能仅支持macOS Editor/Development；不带参数时不会创建Logo对象，原视频调用保持原路径。标志不写PlayerPrefs。截图参数仅在Editor生效。

Logo是代码绘制的最小文字样板：“天地劫 / 神魔至尊传 · HD 重制版”，不代表最终美术。引擎和首场景加载完成前仍无法保证绘制；没有原生启动窗口或前置场景。不能据此宣称消除冷启动全部黑屏。

## 视频衔接

原片 `Assets/Resources/movie/Title.mp4` 共79帧，320×200，15fps，画面约5.267秒。离线逐帧检查确认第0帧RGB全0，第1帧已有星光。因此仅在收到第0帧回调时撤Logo仍会露出原片黑帧。

原型先给Logo绘制机会，再加载原视频并Prepare，准备完成后正常播放。首帧事件继续观测到当前原片第1帧，启用原有RawImage，在一次EndOfFrame后撤Logo；不修改或转码原片、音轨、播放速度和原有后续回调。第1帧门槛只针对已核对的这个原片，不是通用视频非黑检测算法；日后替换片源必须重验。

准备及内容首帧各有15秒等待上限；错误、文件缺失或跳过时清除覆盖层并结束视频，沿原回调显示菜单。剧情内其他视频仍走原方法。事件订阅在结束及重新开始时清理，避免重复订阅。

## 计时口径与验证

日志前缀 `SWORDSMAN_STARTUP`，包含进程起点参考毫秒、Unity实时秒和帧号。阶段覆盖BeforeSplashScreen、BeforeSceneLoad、首场景加载、标题Start、视频资源加载、Prepare完成、解码首帧、首内容帧、Logo撤除。`logo_first_repaint_submitted`仅表示绘制提交；它不等于操作系统实际呈现时间。进程起点读取失败时使用托管初始化时间回退，不应解读为完整进程启动耗时。

执行入口 `SwordsmanStartupPrototypeValidation.Validate` 校验编译、原片存在/帧数、内置字体及公开许可状态；`RunPlayMode`在实际场景中进行视频解码与回调检查。基线用timing参数，原型用prototype参数；不加 `-quit`，由检查完成后自动退出。均不调用Player构建或现有后处理。

证据目录：`Logs/startup-prototype-20261112/`。批处理Play Mode检查已通过Prepare、解码首帧、揭示、提前跳过及视频缺失时的单次回调。图形Play Mode已取得Logo和撤除后的原动画星光截图。首次截图揭示了原片第0帧为黑色的问题，修订后重新验证，未把第一版黑图作为成功证据。

性能数字只作单次Editor样本，不能当作Standalone冷启动性能；原型的作用主要是遮住等待，而非证明加载更快。独立应用首个可见帧、完整黑屏时长、Unity Logo零闪现、系统重启后的冷缓存及慢盘场景均未验证。

Debug/Development Player构建：未运行。原因是当前构建入口会执行前述许可标记后处理，本轮不调用；没有替换现有AllChapters应用。场景/ProjectSettings/构建入口/后处理文件与本轮开始哈希一致，既有工作区修改保留。

### 本轮单次图形Editor样本

以标题Start为0，毫秒如下；两次独立Editor会话受编辑器窗口刷新和缓存影响，不能据此计算独立应用加速率：

| 事件 | 基线 | 原型 |
|---|---:|---:|
| Logo绘制提交 | — | 279.24 |
| 视频Prepare完成 | 530.52 | 619.92 |
| 解码首帧 | 716.74 | 719.02 |
| 首内容帧 | 974.86 | 951.76 |
| 撤除Logo | — | 958.53 |

机器汇总：`Logs/startup-prototype-20261112/verification.json`；两份 `*-visual-playmode.json` 均无检查错误。原型截图在 `captures/logo.png` 和 `captures/video-after-reveal.png`，均为实际Editor Game View输出，不是独立应用冷启动截图。

## 文件与阶段交付

新增 `Assets/Scripts/Title/SwordsmanStartupPrototype.cs`、`Assets/Editor/SwordsmanStartupPrototypeValidation.cs` 及Unity生成的对应meta，新增本文。修改 `Assets/Scripts/Title/GameSceneTitle.cs`、`Assets/Scripts/Manager/GameMovieManager.cs`、`docs/current-status.md` 和 `docs/verification.md`。现有用户修改保留，无本轮新增异常路径；未提交/推送。

实际执行Unity命令行 `-executeMethod SwordsmanStartupPrototypeValidation.Validate`，以及两种 `RunPlayMode`（timing/prototype）和图形capture模式；本轮改动文件的 `git diff --check` 通过，保护文件SHA256核对通过。原型/Editor验证阶段完成；独立包冷启动、实际首帧呈现和“不出现Unity Logo”未完成。建议在使用正式支持所需启动配置的条件后，进入独立应用逐帧计时阶段；本轮不扩大到原生启动窗口或引擎二进制。
