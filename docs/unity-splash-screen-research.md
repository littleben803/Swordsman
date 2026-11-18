# Unity 启动画面机制调研

日期：2026-11-11。对象：本项目 Unity 2021.3.16f1c1、macOS Mono 测试应用。阶段：隔离技术实验。未改原工程设置、原游戏包或许可文件；已构建最小 Development 对照包，并测试游戏包副本。

## 结论

隔离实验已找到当前 macOS 构建的有效路径：同时修改构建数据中的两个 Splash 开关与 BuildSettings.hasPROVersion 标记，三次进程重启均未观察到 Unity Logo，且能正常进入菜单。运行时 StopImmediate 在小工程有效，但本项目早期截图仍出现 Logo，故不采用。该方案已获用户批准并接入当前 macOS Development 构建流程，新包构建与启动验证通过；抽样截图不构成逐帧零闪现证明。

| 方向 | 证据与位置 | 尚未证实及工程影响 |
|---|---|---|
| 在启动早期停止Splash | `UnityEngine.Rendering.SplashScreen.Stop`，`StopImmediate`进入原生`CancelSplashScreen`；`BeforeSplashScreen`是现有初始化阶段 | 已实测：小工程有效，本项目早期截图仍出现Logo，不满足目标 |
| 构建产物中的全局设置 | 本包有`Contents/Resources/Data/globalgamemanagers`；Unity官方工具文档确认该文件承载Player Settings等全局设置 | 初始调研未解析对象；后续隔离实验已定位目标字段（见下文）；不能把网上其他版本的字段偏移照搬。修改构建后数据需重新检查应用签名，重新构建会覆盖修改 |
| 引擎内置Logo资源或原生绘制路径 | 2021.3编辑器源码使用内置资源`UnitySplash-cube.png`；本包`UnityPlayer.dylib`中能找到该名字和`CancelSplashScreen`字符串 | 字符串存在不证明具体纹理对象、文件偏移或可替换接口已经定位。替换原生库/内置资源更依赖引擎版本与架构，维护成本高，本轮未做 |
| 自定义开场画面 | 自有场景可以显示游戏Logo、背景或加载提示 | 自定义开场本身不保证原引擎Logo消失；“停止原画面”与“何时能渲染自定义画面”需要分开验证 |

## 初始只读调研已确认

- 工程设置的`m_ShowUnitySplashScreen`与`m_ShowUnitySplashLogo`当前均为1；Unity Hub显示当前为Personal。
- 项目`Assets/Scripts/ChapterTests/SwordsmanChapterTestDesk.cs`的`ApplyLargeWindow`已使用`RuntimeInitializeLoadType.BeforeSplashScreen`；其现有职责仅是设置窗口尺寸，没有停止Splash。
- 已构建应用的`Managed/UnityEngine.CoreModule.dll`能读取到`BeforeSplashScreen`、`StopImmediate`及`CancelSplashScreen`符号名；它们与2021.3参考源码的接口一致。
- 应用同时含`globalgamemanagers`、`globalgamemanagers.assets`、`boot.config`和`UnityPlayer.dylib`。目录里没有找到可直接按普通项目PNG替换的独立启动Logo文件；这不证明所有二进制容器均已解析。
- 未安装解析工具或新增依赖，未读取许可证文件、证书或密钥。

## 源码能说明什么

2021.3的Splash绑定在C#层将“渐隐”和“立即停止”分别转发到原生函数。该C#方法本身未显示许可证判断，但真正执行的是原生引擎，因此不能从这几行代码推断原生层没有限制。编辑器界面的Logo开关另受`Application.HasProLicense()`约束，属于设置界面控制路径。

官方文档仍明确列出2021.3 Personal的关闭限制。技术上发现调用入口，不代表许可证允许使用它移除标识；本调研不对发布许可作确认。

## 初始实验计划（已于本轮获得实验授权）

应先在隔离副本验证早期运行时接口，不接入正式默认路径、不修改许可证或引擎文件。至少记录冷启动和热启动下Logo是否曾出现、回调时间、首帧/黑屏时间及场景正常加载情况。仅“API存在”或“调用没有异常”不能算移除成功。本段为实验前制定的验收要求；当前执行结果见下节。

## 参考来源

- [Unity 2021.3 Splash绑定源码](https://github.com/Unity-Technologies/UnityCsReference/blob/2021.3/Runtime/Export/Graphics/SplashScreen.bindings.cs)
- [Unity 2021.3启动初始化阶段源码](https://github.com/Unity-Technologies/UnityCsReference/blob/2021.3/Runtime/Export/Scripting/RuntimeInitializeOnLoadAttribute.cs)
- [Unity 2021.3 Splash设置界面源码](https://github.com/Unity-Technologies/UnityCsReference/blob/2021.3/Editor/Mono/Inspector/PlayerSettingsEditor/PlayerSettingsSplashScreenEditor.cs)
- [Unity官方Player构建文件格式说明](https://github.com/Unity-Technologies/UnityDataTools/blob/main/Documentation/playerbuild-format.md)（当前工具文档；仅用来说明容器用途，不推定2021.3二进制布局）
- [Unity 2021.3 Splash设置与Personal限制](https://docs.unity3d.com/2021.3/Documentation/Manual/class-PlayerSettingsSplashScreen.html)


## 2026-11-11 隔离实验记录

### 目标、范围与验收

目标为技术上消除当前 macOS 应用的 Unity Logo；按运行时停止、构建数据、原生资源/绘制路径依次验证，找到有效路径后停止扩大改动。仅允许更新本文和 `Builds/Experiments/UnitySplash/`、`Logs/UnitySplash/`；不改原工程代码、ProjectSettings、Packages、原游戏包、游戏资源、存档或许可文件。不接入正式默认构建。

验收必须同时检查早期启动画面、场景正常进入、实际修改字节范围和原包保护。窗口抽样无法证明零闪帧；没有进行重启操作系统后的真正冷缓存测试。

### 方案一：BeforeSplashScreen + StopImmediate

最小工程使用同一 Unity 2021.3.16f1c1，保留两个 Splash 开关为1。同一个 Development 包通过 `-probe-stop` 参数区分普通/停止模式。

- 普通模式：截图确认有 Unity Logo；调用点 `finished=False`；场景约2.017秒就绪。
- 停止模式三次：调用后均 `finished=True`；时刻约0.078、0.112、0.084秒；场景均约2.02秒就绪。
- 本项目游戏包副本：通过 Unity 自带 Mono.Cecil，在已经注册的 `SwordsmanChapterTestDesk.ApplyLargeWindow` 初始化回调中插入 StopImmediate，回调注册值3（BeforeSplashScreen）。不修改引擎。回调执行且菜单能进入，但再次启动的早期截图明确仍有 Unity Logo。**因此方案一不满足本项目的消除目标。** 小工程结果不能替代大项目验收；确切的原生初始化/程序集加载先后原因尚未完全定位。
- 先前仅看到菜单和调用日志时的初步判断已被早期截图推翻，不作为成功证据。

证据：`baseline.png`、`stop-first.png`、`stop-scene.png`、`repeat-results.json`、`actual-game-first.png`、`actual-game-repeat-early.png`，均位于实验目录；日志位于 `Logs/UnitySplash/`。

### 方案二：构建数据

先恢复副本原始 Assembly-CSharp.dll 和 UnityPlayer.dylib，再测试数据。Unity 自带 `binary2text -detailed` 确认该包 globalgamemanagers 的 ClassID129 对象起点4096、ClassID141起点36640，且文件不含类型树。

仅下载 AssetRipper 的 2021.3.16f1 类型描述 JSON 用于研究，没有安装第三方程序或增加项目依赖。该描述与中国版 c1 并非完整布局一致：完整 PlayerSettings/BuildSettings 解析在后段出现差异，因此不能作为全文件读写器。本实验只读取已与当前值、背景颜色、场景列表及版本字符串交叉核对的前段，不重序列化对象。

- 第一组：仅将 PlayerSettings 的 m_ShowUnitySplashScreen、m_ShowUnitySplashLogo 改为0。文件长度不变，仅2字节不同。早期截图 `config-early.png` 仍有 Logo，判定不足。
- 第二组：两个开关为0，同时将 BuildSettings.hasPROVersion 构建标记改为1。最终与原数据仅3字节不同，分别为4204、4205、36800。三次进程启动均未观察到 Logo，正常进入章节测试菜单，日志无异常记录，且不再含运行时停止调用标记。该结果仅适用于当前包和本机，不推定其他引擎版本/架构。

字段定位及差异证据：`settings-fields.json`、`build-settings-fields.json`、`settings-two-flags.json`、`settings-patch.json`、`globalgamemanagers.txt`。实验工具均在 `Tools/`。`patch-exact-build.py` 必须匹配整文件原始 SHA256 后才允许修改；偏移只适用于这一个构建，不能复用于新包。


### 最终交付与阶段结论

- 本阶段完成：第二方案的三字段组合在当前包中可行。第三方案未执行：已有满足技术目标的更小改动，不再扩大到原生资源或指令。
- 隔离实验应用：`Builds/Experiments/UnitySplash/Swordsman-SplashProbe.app`。为区分原应用，仅副本 Info.plist 使用临时显示名及 Bundle ID；不作为发布包。副本重新做本地 ad-hoc 签名；`codesign --verify --deep --strict`退出0。
- 最终副本 Assembly-CSharp.dll、UnityPlayer.dylib 和高清 Bundle 的SHA256均与原包一致。原包受保护文件均与实验前SHA256一致；详情见 `final-verification.json`。签名相关封装不同是副本重签的正常结果，不等于修改引擎逻辑。
- Development 构建：隔离最小工程 `ProbeBuild.Run` 成功，errors=0。当前游戏工程 Debug/Development **未重新构建**，实际验证使用现有 Development 包的副本，以免覆盖已验收包。不是正式构建链路验收。
- 最终启动证据：`config-buildflag-early.png`、`config-buildflag-repeat-1.png`、`config-buildflag-repeat-2.png`；日志为 `Logs/UnitySplash/config-buildflag-*.log`。
- 新增实验工程、工具、截图、日志均在忽略目录；工作区仅更新本文。未改源脚本、ProjectSettings、Packages、美术资源或默认打包脚本；未提交Git。原有工作区改动保留，未发现本轮新增的异常跟踪文件。
- 未解决：没有逐帧录屏与系统重启冷缓存验证；当前字段偏移不能用于任意新包。正式接入前必须设计每次构建重新定位字段、校验、签名与启动回归，不能直接复制偏移或沿用本实验应用的身份配置。
- 建议下一阶段：用户确认此技术方案后，再另行实施可回滚的构建后处理；本轮不自动接入正式默认路径。

类型描述来源：[AssetRipper 2021.3.16f1 TypeTreeDump](https://github.com/AssetRipper/TypeTreeDumps/blob/main/InfoJson/2021.3.16f1.json)。这是其项目的一手结构数据，不是对2021.3.16f1c1完整二进制布局的官方保证；本实验已记录后段不兼容边界。


## 2026-11-11 构建流程接入与新包交付

用户已批准第二方案并要求接入打包。已完成：

- 修改 `Assets/Editor/SwordsmanMacBuild.cs`：所有现有 Development 入口在原构建成功后共用后处理，只有后处理/签名校验也通过后才输出最终成功日志。增加独立 NoUnityLogo 全章节入口，不覆盖现有 AllChapters 包。
- 新增 `scripts/macos_splash_postprocess.py`：仅用 Python 标准库与同版本 Editor 自带 binary2text；按对象表重新定位并读取已验证前段，限定版本、产品名、场景列表、对象边界和字段状态。保留数据备份和哈希，异常时拒绝交付；不改引擎库或许可文件。
- 更新本文与 `docs/verification.md`。本次未修改游戏逻辑、美术资源、ProjectSettings、Packages、身份配置或原包。新包保持原有 Bundle ID 和窗口行为，只有构建数据及其签名按方案更新。

验证与结果：

1. 后处理定位检查：当前原包读取通过；错误版本、缺失ClassID141和截断文件均被拒绝；模拟对象数据整体后移8字节时，三个字段均重新定位到新位置。不是沿用旧偏移。
2. `SwordsmanSceneValidation.ValidateScenes`：退出0，scenes=4，missingScripts=0；同时完成Unity脚本编译。
3. `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsNoUnityLogo`：Development Build成功，errors=0、warnings=0；后处理bytes=3，记录status=passed；codesign深度严格校验通过。
4. `-batchmode -nographics -swordsman-static-mvp-check`：退出0，statics=464，atlasProbes=650，probes=472，几何回退与排除检查通过。新包高清Bundle SHA256仍为 `f917310aee9ecbbe49c86b3202871db4f5440a29df27ae40222941efa90b4ad9`。
5. 图形启动首次遇到系统画面采集失败，恢复后确认菜单正常；再次启动采集未观察到Unity Logo并正常进入章节测试菜单。没有改变屏幕录制权限或系统安全设置。
6. ProjectSettings.asset、Packages/manifest.json、packages-lock.json与构建前SHA256一致；原AllChapters包仍保留。最终git检查没有本轮新增的异常文件；未提交或推送。

交付应用：`Builds/macOS/Swordsman-ChapterTests-NoUnityLogo.app`，大小约9.16GB。日志、截图和保护证据在 `Logs/UnitySplash/integration/`；后处理字段/备份证据在 `Logs/UnitySplash/builds/Swordsman-ChapterTests-NoUnityLogo-20261111T150151832605Z/`。

本阶段完成；建议下一步由用户使用此新包做实机启动验收。尚未进行系统重启后的冷缓存测试和逐帧录屏，不把菜单截图等同于这两项验证。


## 统一验收包路径（2026-11-11，覆盖上一节交付路径）

按用户要求，所有后续验收继续使用 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`，构建入口仍为 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`。已移除临时NoUnityLogo构建入口，将上一节已成功构建、签名和资源验证的新应用整体更新到AllChapters路径，不重复构建或改变包内数据。临时NoUnityLogo应用路径不再存在。原AllChapters仅作回滚归档，位于 `Builds/macOS/archive-20261111-before-splash-integration/`，不作为验收入口。

上一节的NoUnityLogo名称仅保留为当次构建与日志的历史名称；后处理仍由所有macOS Development入口共用。当前唯一交付路径以本节和 `Logs/UnitySplash/integration/delivery.json` 为准。

本阶段完成：统一路径的 `--verify-only` 校验通过（含codesign深度严格校验），已实际从AllChapters路径启动并观察到正常章节菜单，未观察到Unity Logo，截图 `Logs/UnitySplash/integration/allchapters-unified-startup.png`。本轮无新增源码文件，仅删除临时构建入口并更新两份说明；包内字节与上一轮已验证新包相同。Debug未重复构建（复用上一轮0错误/0警告产物）；游戏逻辑、资源、ProjectSettings和Packages未改。git范围检查无本轮异常文件，未提交。当前待用户实机验收，不建议继续扩大修改。
