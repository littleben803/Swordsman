# 章节测试台

## 当前安装包规则（2026-11-12，覆盖下文历史产物说明）

用户明确只保留 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`。历史、实验、普通/高清预览、独立测试及CacheBenchmark应用已全部退役并清理，不保留整包回退副本。唯一构建入口为 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`，Unity直接输出到该路径；不得先生成另一份应用再复制覆盖，不再创建其他 `.app` 名称。旧入口及 `-swordsman-cache-benchmark` 参数已移除/拒绝。

资源Bundle、稳定导入缓存、原版资源、批准稿/原始生成输入、评审和最小验证证据继续保留。下文旧应用名、独立输出及保留说明仅是历史记录，不是当前保留清单或后续打包指令。本轮清理未改变资源、美术或用户实机验收状态。


日期：2026-11-03。适用 Unity 2021.3.16f1c1 / macOS Development 独立测试包。

## 使用

打开统一验收包 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`，先按原流程播放启动动画并进入游戏主菜单。此时右上角显示 **章节测试台** 按钮，点击才打开测试台；启动时默认不弹出。该入口仅在含章节测试宏的 Development/Editor 包中存在。测试台左上角 **关闭 [Esc]** 或 Esc/F9 可关闭，返回背后的主菜单；关闭不启动章节、不重载场景。选择章节后（采用该幕推荐等级）：

- **剧情开场 / Enter**：从该幕的原版开场进入。
- **战斗起点 / B**：已有起点快照时直接恢复；首次进入以 4 倍速播放开场、推进普通对话，直到首个可操作的玩家回合后保存独立快照。遇到剧情选择时交由人工选择，不自动决定分支。
- **资源查看 / V**：载入地图和单位，暂停剧情及回合；方向键移动镜头。第一幕额外显示该幕静态桌面道具，供资源验收。
- **测试中的 F9 / 左上角返回按钮**：结束当前测试，返回选幕。当前未保存操作不保留。主菜单的 F9 可开关测试台；启动动画及读档/帮助子界面期间不显示测试台入口。

方向键也可以在测试台选择章节；鼠标支持选幕，三列完整显示36幕。正式包和原有高清预览包不包含此入口。测试台功能本身沿用既有高清资源；第十六～二十幕固定验收副本为 `Builds/macOS/Swordsman-ChapterTests-16-20.app`，共233静态纹理；共享AllChapters当前携带386张静态纹理，覆盖索引0至35，包含108项隐藏剧情及界面切片和24处整图归档修订。逐幕实机与正常剧情待用户确认。首版19项记录保留其历史阶段含义。

### 2026-11-11 右栏三行说明与固定入口

测试中的悬浮栏不再显示进入场景的过程提示（例如“已从独立快照进入战斗起点。”）；选幕页底部仍保留原状态信息。当前 Mock 摘要移到返回按钮正下方，逻辑位置为 `(16, 60)`，尺寸与左上返回按钮一致为 `290 × 36`，采用深青绿色底、白字，长文本在固定框内换行并适配字号。仅调整显示，辅助开关与场景进入流程不变；本次未打包，新布局尚待运行画面验证。

顶部固定三行：`第x幕：yyy 角色等级 z`、`预设初始装备+确定成长，非真实通关存档`、`RPG 和客栈休息入口使用预设进度`。原入口下方提示移至第三行，camp34原资源缺失警告仍单独保留。全部关闭按钮采用红底白字；底部说明为“选择开关后，重新进入剧情生效”。这只是文案调整，开关仍按既有逻辑在各测试入口重新进入时应用。

RPG、客栈和RPG页码固定显示；没有对应入口时使用灰色禁用态，鼠标和R/I快捷键均按原可用条件拦截。RPG无目标显示0/0，单目标显示灰色1/1，多目标才允许红色页码切换。仅修改测试台显示，技能、存档、Mock效果、资源、场景和工程配置不变。

验证记录：Development构建成功，0错误/9警告，日志 `Logs/test-desk-ui-three-lines-20261111-build.log`；`codesign --verify --deep --strict` 和 `git diff --check` 通过。文案、固定占位、R/I快捷键和鼠标可用条件静态核对通过；重新启动后的测试台显式检查通过36关资源入口、普通存档回读及隔离检查，报告 `Builds/ChapterTests/reports/test-desk-ui-three-lines-20261111.json`。当前旧进程继续运行，新版窗口显示待退出重开后由用户验收；没有把旧窗口作为本轮视觉通过证据。构建覆盖运行中的二进制导致首次启动被系统缓存拒绝，已对产物可执行文件做字节不变的原子替换，签名校验仍通过；未改签名配置。

### 2026-11-11 三列选幕与 Mock 辅助（已接入测试包）

左侧改为三列共12行，右侧窄栏保留五种测试入口，辅助按生存、角色能力与装备、地图分组。七个开关默认为关闭：无限血量、无限气量、全技能术法、顶配武器、顶配防具、顶配饰品、显示隐藏宝物；“全部关闭”只更改下次进入的选择。选择后重新进入测试生效，测试中显示当前辅助状态。

血量和气量在我方可控战斗单位上显示9999并阻断对应扣减，包含可控召唤单位；剧情直接退场/击杀状态不改，敌方及不可控NPC不受保护。全技能仅解锁角色自身初始及升级表技能，五内设为原上限100，等级不改。封寒月和紫枫各有11项原生技能，战斗技能菜单使用“技能页 / Tab”按钮翻页。用户随后明确要求正式游戏同步修复：角色技能数组满槽后可扩展，额外技能通过兼容扩展持久化；主存档长度和原10槽核心布局不变，具体格式与向旧程序回退限制见[架构](architecture.md)。

顶配装备使用项目现有物品ID并验证角色可装备限制，直接提供最终成品，不消耗剧情炼化材料。武器按0..9角色依次为古剑‧龙皇、梦想无双、紫电金戟、七色彩绢、古剑‧天罡、无相法杵、回光银环、千幻流光、搜魂攫魄爪、酒仙葫芦；防具为紫云战衣、七色蝶裳、血凛咒甲、玄罗仙袖、血凛咒甲、雷光天铠、玄乌羽衣、舞风霓裳、黄泉祭衣、散仙道袍。饰品是明确测试推荐：封寒月、紫枫、韩千秀、燕明蓉使用七色璎珞，其余使用鬼神金冠，不宣称存在适用于所有场景的唯一最强饰品。敌我复用主角基础数据时，敌方使用未应用Mock的独立副本。

开启任一辅助的进度位于 `ChapterTests/v2/Mock/<开关位掩码>/stageXX/levelYY/`；自动验证继续另设Automation子目录。首次可从包内原始战斗起点复制到对应Mock目录，完整加载后再应用辅助，避免读取快照时覆盖Mock装备。关闭全部后回到原测试目录，从原起点重进恢复；原始冻结快照及正式存档目录不写入。原始预设与场景不变；正式技能扩展的兼容存储见[架构](architecture.md)，与Mock目录隔离分别验证。

宝物开关依据原数据 `MapShow`：关闭时只显示普通宝物闪光；开启时额外显示隐藏奖励位置并用冷青色区分。每次刷新先清除旧标记再按当前剩余事件重建，防止每回合重复生成；四格共享奖励仍按一个事件领取，读取存档后按剩余事件恢复，不补造攻略中独有的奖励。

本轮已获用户美术GO的TREASURE 12张重绘切片已归档并接入白名单，保留原浮点几何、15步SAF时序与透明规则。旧Type2材质由RGB亮度决定透明度，导入这些切片时关闭透明RGB边缘扩散，避免透明背景产生亮框。原图集、Sprite、Prefab和Shader未修改。最终Development构建成功，0错误/9警告；12项Mock专项检查、36幕包内冻结快照回归、既有测试台检查、472资源探针/650图集探针、四场景Missing Script=0及签名校验通过。首幕五入口、三列布局、分组开关和键盘选幕已观察；鼠标交互、最终美术和完整剧情待用户实机验收。证据 `Builds/HD4X/static-mvp/work/test-desk-mocks-20261111/delivery-verification.json`。

### 2026-11-11 测试台界面（前版记录；当前尺寸以上述三列版为准）

删除顶部说明与等级滑条，内容区上移55逻辑像素，章节列表可见高度由565扩大到620。右侧以两行显示章节/等级和“预设初始装备+确定成长，非真实通关存档”。左侧章节采用低饱和蓝灰底白字，选中幕使用更鲜明的蓝色；右侧操作及幕末 RPG 入口使用系统蓝底白字，场景切换页码保留红色。右侧操作按钮统一48逻辑像素高、12像素间距，长文案允许换行。按钮常态、悬停、按下均直接绘制纯色底，悬停提亮、按下加深，文字保持白色。底部提示改为“RPG 和客栈休息入口使用预设进度”，与顶部副标题统一15字号。列表滚动、可见选中行及按钮点击范围同步调整；Camp34 缺失警告仍保留。关卡进入、合成预设和隔离存档逻辑不变。

前版构建及实际窗口记录：AllChapters Development 构建成功，0 错误、9 项既有警告（`Logs/test-desk-ui-20261111-build.log`）；两行摘要及键盘选幕通过。前版按钮高亮仍继承旧图样，本次已替换。当前修订 AllChapters Development 构建成功，0 错误、9 项既有警告（`Logs/test-desk-ui-20261111-v2-build.log`）。新进程实际窗口确认内容区上移、列表扩大、蓝灰章节按钮与纯色悬停高亮、第十一幕蓝色RPG入口、五个右侧操作按钮等高等距、顶部/底部提示同字号，键盘选幕正常。全部操作按钮绘制及点击范围静态检查通过；未重新执行全剧情或全章节战斗验收，最终视觉及鼠标操作仍待用户实机确认。

### 2026-11-11 隐藏宝物分类核对（只读阶段，尚未接入开关）

范围：读取 `Assets/Scenes/level0.unity` 中 `GameBattleData` 与 `GameItemData` 的现行序列化数据，关联每幕 DTL 格子与 MEVT 事件；攻略仅用于独立交叉核对，不修改场景、原始数据或运行逻辑。证据与可重复脚本位于 `Builds/HD4X/static-mvp/evidence/hidden-treasure-audit-20261111/`，`map-events.json` 保存各关卡、奖励、事件 ID、MapShow 及零基坐标，`summary.json` 保存检查、源文件 SHA256 和限制。

**静态数据已确认：**

- 36个原始关卡索引中，实际被地图格引用的直接奖励事件共214个；全部为 `EventType=Item`，其 `ItemGetType` 区分物品或金钱。普通奖励130份（`MapShow=0`），隐藏奖励84份（`MapShow=-1`，分布29关），无其他显示字段值。隐藏触发格共87个，奖励按事件 ID 计数，不能按格子数计数。
- 第四幕事件8是翡翠碧沙，共享零基格子 `(20,22)/(21,22)/(20,23)/(21,23)`。原拾取代码按事件 ID 删除剩余记录，并删除同名的全部显示对象；未来标记应复用该归组，领取一次后整组消失。
- 另有9条直接奖励记录没有地图格引用，不纳入可领取统计或显示。`updateTreasures()` 当前也会移除未被格子引用的记录。不得因静态表有条目就补造位置。
- 原始索引0、18、19、31、34、35没有地图格引用的直接奖励；索引24（另一条第二十三幕路线）有7份普通奖励而没有隐藏奖励。没有直接奖励不等于没有脚本、敌人掉落、城镇或过关奖励。
- 第一、二、三、四、六、十七、二十二幕的隐藏坐标集合与攻略吻合；第零、十九、三十三幕及番外无宝箱样本吻合；第一幕普通/隐藏显示值和第四幕共享事件关系吻合，共14项检查通过。选定样本坐标直接对应项目零基格子，无需加减1；其余地图仍以项目数据为准。

**代码已确认与实现约束：**

当前 `GameBattleManager.updateTreasures()` 只排除剧情事件，没有检查 `MapShow`，会为普通和隐藏的直接奖励都创建同一闪光对象。这是代码路径判断，尚未用本轮运行截图证明。下一阶段建议仅在测试宏范围内：开关关闭时按 `MapShow` 保留普通闪光、隐藏奖励不显示；开启时为隐藏奖励增加可区分的标记。标记必须取当前剩余 `MapEvent`，不得每帧从初始表重建已领取奖励。剧情事件、敌人掉落、城镇和过关奖励均排除，不更改领取、分支及胜负判定。普通正式包的既有显示逻辑本阶段不修改。

原存档已保存剩余宝物事件 ID，读取后调用 `addTreasure()` 与 `updateTreasures()`；下一阶段可从该状态重建标记，无需变更存档格式。本轮只做代码阅读，未实际验证领取、开关切换、回合刷新或存档重载。

**待确认差异：**攻略第三十幕列有宝物，项目原始索引31却只有剧情事件，无地图直接奖励；保留差异，不补造。攻略没有单独覆盖原始索引24分支，且第十三幕标题与项目不同，不能把文章幕号直接当原始索引。其余角色技能、顶配装备、血气及三列布局仍处于方案阶段，尚未实现。

交叉核对来源：[攻略第1页](https://www.3dmgame.com/gl/1358.html)、[第2页](https://www.3dmgame.com/gl/1358_2.html)、[第3页](https://www.3dmgame.com/gl/1358_3.html)。本阶段未运行 Unity 编译或 Development 构建，因为仅修改说明文档并生成离线证据；下一阶段需要验证开/关显示、普通宝物不受影响、多格共享领取、无宝物地图、回合刷新、存档重载及普通构建隔离。

### 2026-11-10 RPG 与客栈入口

现行入口测试版为 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`，默认窗口为 `2560 × 1600`。选择第零幕后，右侧的 **幕末 RPG 场景 00 / R** 直达城镇，可以按原版流程进店。若本幕有多个可达 RPG 场景，点右侧页码切换目标。**客栈休息 / I** 直达原营地界面，显示条件为本幕存在对应的 RPG／营地事件且原 Camp 数据有效。无对应入口的幕隐藏该按钮。

入口沿用原事件的落点及后续关卡，并使用独立的合成测试进度。开发构建 0 错误、9 警告，包内 36 份战斗快照齐全。包内检查通过第零幕 RPG00、索引 3 的 RPG02、索引 7 的 RPG26、无路线拦截、第零幕客栈及离开客栈回城，以及第零幕战斗快照回归。

现行包包含已确认的 12 张固定店景与 47 张买卖页切片。第零幕自动触发三个原版进店事件，检查高清背景、左右面板、价格图标、选中框及确认／取消弹窗。15 张新增图集的可见 RGB 和全域 Alpha 与归档一致，用户实机验收待确认。验收记录位于 `Builds/HD4X/static-mvp/evidence/rpg-shop-20261110/package-verification.json`。

第零幕 **I** 进入 `Camp00` 营地。现存旧版播放表只有人物与道具、阴影和光晕三层。修复版在运行时以第十幕 `Camp10_t0` 恢复固定房间底图，沿用原层坐标 (3, 0)，人物和动态层仍用 Camp00。查看城镇和商店使用 **R**。本幕合成进度可以没有可炼化的物品，空列表应清空说明和选中状态，确认键保持在当前页。

现行全章节测试包已接入 20 张已获美术 GO 的休息底图，覆盖 33 个 Sprite 引用。第零幕使用高清 `Camp10_t0`，并通过底图坐标、炼化及三家商店检查。商品行价格图标保留原色，淡化状态只调透明度。验收记录见 `Builds/HD4X/static-mvp/evidence/camp-hd-20261110/package-verification.json`。

## 合成进度边界

唯一预设源是 `Assets/ChapterTests/presets.json`。36 个条目按原始关卡索引排列，标题直接取主场景的原始数据，不用显示幕号推算索引（含序幕与番外）。

默认队伍来自前一关原始单位表中的玩家角色；建议等级来自本关敌方有效等级中位数，序幕取初始等级。角色使用原始初始装备及确定性成长值，保留原始升级数据不改动。预设现使用 schemaVersion=2、revision=2，显式包含跨幕剧情 flags 及必要测试物品；调整后需重新验证快照。

这些是合成测试起点，不是真实通关档。本轮进一步生成所有关卡的战斗起点并进行跨进程核验；这不代表逐幕完整战斗通关或所有分支验收，测试装备也不用于战斗平衡验收。

首次快进 90 秒仍未到达玩家回合时恢复正常速度，允许人工继续；只在满足回合、光标和对话等条件后捕获起点，不把过场误当战斗档。

## 存档兼容与隔离

测试功能仅在 `SWORDSMAN_CHAPTER_TESTS` 且 Development/Editor 时编译。测试包初始化即把现有本地存档入口指向独立目录：

```text
Application.persistentDataPath/ChapterTests/v2/stageXX/levelYY/
```

普通五槽与自动战斗存档均在该目录，原始存档目录不写入。测试包阻断旧 Debug 快捷键入口，不调用旧网络调试加载。网络、账号、支付和原版业务流程均未新增。

2026-11-11起，原10槽二进制核心布局不变，增加可选技能扩展读取与写入，见[架构](architecture.md)：普通槽位仍为 3000 字节，战斗档仍为 102400 字节。测试台只增加外部 `BattleStart.json` 元数据，以关卡、等级、预设 SHA256、二进制 SHA256、长度和头部校验 `BattleStart.dat`。失效快照重新建立；普通自动保存写 `SaveBattle.dat`，不覆盖冻结的起点。

测试检查使用额外的 `Automation/` 子目录，只清理该子目录。旧正式战斗档兼容测试先复制到这里，再调用原版读取器，源文件只读。

## 构建与自动验证

```sh
"/Users/ideabox/Applications/Unity/Hub/Editor/2021.3.16f1/Unity.app/Contents/MacOS/Unity" -batchmode -nographics -quit -projectPath /Users/ideabox/repo/Swordsman -executeMethod SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters -logFile /Users/ideabox/repo/Swordsman/Logs/allchapters-development-build.log
```

构建仅临时暂存预设与现有高清 Bundle，完成后清除暂存，恢复工程设置；输出为独立测试应用，不覆盖普通包或原有高清预览包。

```sh
Builds/macOS/Swordsman-ChapterTests-AllChapters.app/Contents/MacOS/Swordsman -batchmode -swordsman-chapter-tests-check -swordsman-chapter-tests-report /Users/ideabox/repo/Swordsman/Builds/HD4X/static-mvp/evidence/chapter-test-desk-runtime-final.json -logFile /Users/ideabox/repo/Swordsman/Logs/chapter-test-desk-runtime-final.log
```

命令行也可传 `-swordsman-test-stage 1 -swordsman-test-mode opening`，mode 支持 `opening`、`battle`、`inspect`。

本轮 Development 构建成功，0 错误、9 项既有警告；四场景检查 `missingScripts=0`。基础回归覆盖 36 关资源入口、第一幕快照校验与恢复、普通存档往返、旧战斗档副本读取、损坏/错关/预设变更拒绝及存档路径隔离。具体结果以最终运行报告为准。自动检查不替代逐幕剧情与美术人工验收。

## 测试台首版记录（前阶段）

新增文件：

- `Assets/ChapterTests/presets.json` 及 Unity 生成的资源/目录 `.meta`。
- `Assets/Scripts/ChapterTests/SwordsmanChapterTestSession.cs`、`SwordsmanChapterTestDesk.cs`、`SwordsmanChapterTestChecks.cs` 及 Unity 生成的 `.meta`。
- 本文档 `docs/chapter-test-desk.md`。

修改文件：`Assets/Editor/SwordsmanMacBuild.cs`（新增专用构建入口）、`Assets/Scripts/Base/GameSetting.cs`（测试宏下存档路由）、`Assets/Scripts/Manager/GameInputManager.cs`（测试菜单拦截与旧 Debug 键屏蔽）、`docs/current-status.md`、`docs/verification.md`。

原始资源、场景、Prefab、工程配置、Packages、签名与正式存档未修改；本次开始前已有的其他工作区修改原样保留。

前阶段已通过实际窗口验证：方向键选幕、V 进入第一幕资源查看（含静态桌面道具）、F9 返回、B 从已建立的独立快照直达第一幕战斗。已把自动检查生成且校验通过的第一幕 level03 起点复制到本机手动测试目录；不覆盖已有手动快照。

用户已确认原测试台交互无问题。新版沿用同一交互，仅扩充章节预设、快照交付和验证。

本阶段进行逐幕起点生成和重载验证；完整战斗通关与全部剧情分支不在起点验证结论之内。

## 全章节起点补齐（2026-11-01）

目标为每个原始 stage 索引建立一个可复现的合成开场及首个玩家回合快照，逐幕核对原版读写器往返。不是绕过剧情直接摆放单位，也不是逐关自动获胜。

本轮修改限于 `Assets/ChapterTests/`、`Assets/Scripts/ChapterTests/`、`Assets/Editor/SwordsmanMacBuild.cs`、`scripts/verify-chapter-tests.py` 及对应文档。原始数据、剧情事件、战斗 AI/规则、`GameUserData` 读写器及正式存档格式不改。独立输出 AllChapters 应用，保留用户正在运行的旧测试包。

### 分支预设与依据

为了让后期角色及关卡可测，选取一个明确的合成分支，非所有分支的组合覆盖：

| 预设 | 应用范围（原始索引） | 原始事件依据 |
| --- | --- | --- |
| flag12=1 | 9–34 | stage08 Event2 step71 写入，后期 stage29 开场引用 |
| flag13=1 | 11–34 | stage10 Event4 step0 写入；stage15 开场/后续事件引用 |
| flag14=1 | 16–34 | stage15 Event2 step90 的夏侯仪留队路径写入；stage16 起开场引用 |
| 物品216（七色璎珞） | 29–34 | stage29 Event1 step1 通过 opcode0x53 检查持有并写 flag10 |
| flag16=1 | 30–34 | stage29 Event1 step0–3 在 flag12 与物品条件满足后写入，后期分支引用 |

预设只补这一个测试分支的必要前提；不宣称装备、支线、隐藏收集及结局均为真实通关所得。原始初始装备和确定性等级成长继续保留。番外独立使用原始番外预设。

### 快照交付与保护

schemaVersion=2 使用独立 `ChapterTests/v2`；v1 手动测试进度原样保留。逐幕生成检查写 `v2/Population/`，基础回归写 `v2/Automation/`，都不清理手动测试目录。

通过核验的快照归档到 `Assets/ChapterTests/Checkpoints/stageXX/levelYY/BattleStart.dat` 与同名 JSON 元数据。专用构建把它们复制到 StreamingAssets。首次选幕时，仅在当前等级的两个本地起点文件均不存在且包内校验通过时安装；已有手动起点不覆盖，损坏档仍被拒绝。普通自动存档与冻结起点继续分离。

### 逐幕验证命令

```sh
python3 /Users/ideabox/repo/Swordsman/scripts/verify-chapter-tests.py --stages 0-35 --output /Users/ideabox/repo/Swordsman/Builds/ChapterTests/reports/creation
```

每幕使用独立进程；可加 `--jobs 3` 同时检查不同幕（目录独立、Unity 构建仍串行）。300 秒外部超时只结束自己启动的进程；原版开场内最多等待 210 秒。到达真实可操作玩家回合后：校验快照及隔离路径、保存/读取普通槽、恢复战斗快照，比较关卡、回合、单位身份/位置/显示/死亡/行动状态、等级、当前 HP/MP、256 个剧情变量、十名角色基础数据及关键物品持有状态。运行错误单独记录，不当作通过。

全部生成检查通过后，使用 `--export-only` 将同一报告目录对应的快照归档；它会先校验所有所选报告、源预设与已测应用一致性以及快照元数据/头部/长度/SHA256，再写入资产目录。最终包的跨进程读取另用 `--package` 执行并保存到独立报告目录；该参数隐含 `--reuse`，使用全新的 `v2/PackageValidation/` 目录，缺少有效快照直接失败，不允许回退播放开场来掩盖打包缺失。若需要重跑部分关卡，`--stages 2,15-17` 可指定原始索引。逐幕报告和汇总位于 `Builds/ChapterTests/reports/`；日志在 `Logs/chapter-tests-<报告目录名>-stageXX.log`，均不提交 Git。

### 原始索引34的营地读取限制

原始索引34（第三十三幕「冰岳绝剑」）已从原版开场到达首个可操作回合，冻结快照校验、战斗重载及状态比较通过。但初次检查额外执行普通槽位读取时，发现原始 `GameCampData` 指向 Camp34，而本地没有 `Prefab/Camp/Image/camp34`，`GameCampUI.show` 抛出 Instantiate(null)。原始失败报告保留于 `Builds/ChapterTests/reports/discovery/stage34-original-camp-failure.json`，原始日志为 `Logs/chapter-tests-creation-stage34.log`。

该失败发生在冻结起点已生成之后，未影响冻结文件或战斗重载。因此只归档已核验的战斗快照，不将普通营地读取计为通过。归档此次发现需显式加 `--allow-known-camp34`；该例外只识别已知舞台、异常及 Camp 调用栈，不接受其他运行错误。

后续检查在该幕先检查原始营地 Prefab；若仍缺失，记录 `ordinarySaveBlocker` 并继续独立的战斗快照检查。其他幕缺营地资源仍直接失败。测试台为该幕显示明确提示，请使用 B 战斗起点。原版资源及业务代码保持原样；正常游戏是否需要提供该营地路径，待专项确认。

### 本机运行命令

```sh
open -n "/Users/ideabox/repo/Swordsman/Builds/macOS/Swordsman-ChapterTests-AllChapters.app"
```

同机仍开着旧测试包时，`-n` 确保启动新包实例。默认推荐等级提供包内起点，其他等级仍按原版开场建立独立快照。

### 逐幕结果

下表的“起点”指原版开场到首个可操作玩家回合；“包内重载”包含隔离目录安装、再次启动进程读取、存在可操作角色及创建/重载前后状态一致。共36个推荐等级起点，不代表所有等级或整幕通关。普通营地读取35项通过，索引34单列阻塞。

| 原始索引 | 原始标题 | 推荐等级 | 开场起点 | 包内重载与状态比对 | 普通营地读取 |
| --- | --- | --- | --- | --- | --- |
| 0 | 第零幕  独行 | 1 | 通过 | 通过 | 通过 |
| 1 | 第一幕  因缘 | 3 | 通过 | 通过 | 通过 |
| 2 | 第二幕 紫金王鼎 | 4 | 通过 | 通过 | 通过 |
| 3 | 第三幕  狂剑客 | 11 | 通过 | 通过 | 通过 |
| 4 | 第四幕  隐者 | 9 | 通过 | 通过 | 通过 |
| 5 | 第五幕  十方鬼众 | 10 | 通过 | 通过 | 通过 |
| 6 | 第六幕  亘古魂幡 | 12 | 通过 | 通过 | 通过 |
| 7 | 第七幕  幽谷寒琴 | 16 | 通过 | 通过 | 通过 |
| 8 | 第八幕  杀阵 | 16 | 通过 | 通过 | 通过 |
| 9 | 第九幕  玄魔刻印 | 18 | 通过 | 通过 | 通过 |
| 10 | 第十幕  悬天古径 | 22 | 通过 | 通过 | 通过 |
| 11 | 第十一幕  故人 | 26 | 通过 | 通过 | 通过 |
| 12 | 第十二幕  密道 | 28 | 通过 | 通过 | 通过 |
| 13 | 第十三幕  九曜之厅 | 29 | 通过 | 通过 | 通过 |
| 14 | 第十四幕  萤火 | 35 | 通过 | 通过 | 通过 |
| 15 | 第十五幕  魔骸 | 35 | 通过 | 通过 | 通过 |
| 16 | 第十六幕  战阵 | 39 | 通过 | 通过 | 通过 |
| 17 | 第十七幕  祭天塔 | 42 | 通过 | 通过 | 通过 |
| 18 | 第十八幕  千魔啸夜 | 44 | 通过 | 通过 | 通过 |
| 19 | 第十九幕  飞天 | 46 | 通过 | 通过 | 通过 |
| 20 | 第二十幕  埋伏 | 48 | 通过 | 通过 | 通过 |
| 21 | 第二十一幕  神兵 | 52 | 通过 | 通过 | 通过 |
| 22 | 第二十二幕  突围 | 53 | 通过 | 通过 | 通过 |
| 23 | 第二十三幕  焚炎劫火 | 58 | 通过 | 通过 | 通过 |
| 24 | 第二十三幕  黄泉幽径 | 56 | 通过 | 通过 | 通过 |
| 25 | 第二十四幕  无还之道 | 55 | 通过 | 通过 | 通过 |
| 26 | 第二十五幕  宿命 | 65 | 通过 | 通过 | 通过 |
| 27 | 第二十六幕  剑与非剑 | 71 | 通过 | 通过 | 通过 |
| 28 | 第二十七幕  古月 | 68 | 通过 | 通过 | 通过 |
| 29 | 第二十八幕  天琰神煌 | 67 | 通过 | 通过 | 通过 |
| 30 | 第二十九幕  三界之门 | 70 | 通过 | 通过 | 通过 |
| 31 | 第三十幕  无幻刹那 | 77 | 通过 | 通过 | 通过 |
| 32 | 第三十一幕  天之颠地之 | 81 | 通过 | 通过 | 通过 |
| 33 | 第三十二幕  千幻华舞 | 81 | 通过 | 通过 | 通过 |
| 34 | 第三十三幕  冰岳绝剑 | 83 | 通过 | 通过 | 阻塞：缺 camp34 |
| 35 | 番外篇  两百块帝国 | 91 | 通过 | 通过 | 通过 |

原始创建报告：`Builds/ChapterTests/reports/creation/stageXX.json`；最终包读取报告：`Builds/ChapterTests/reports/packaged/stageXX.json`。初次营地失败与工具负例构造修正的证据保存在 `reports/discovery/`；不覆盖它们来表示通过。

### 本轮文件与验证清单

新增：`Assets/ChapterTests/Checkpoints/` 内的36套冻结档案（DAT、JSON及 Unity 自动生成的 Meta）、`scripts/verify-chapter-tests.py`。

修改：`Assets/ChapterTests/presets.json`、三个 `Assets/Scripts/ChapterTests/` 脚本、`Assets/Editor/SwordsmanMacBuild.cs`、`docs/chapter-test-desk.md`、`docs/current-status.md`、`docs/verification.md`。原有存档路由和输入钩子本轮未再改动。

执行：`BuildDevelopmentPlayerChapterTestsAllChapters`、逐幕创建/归档/`--package` 检查、`SwordsmanSceneValidation.ValidateScenes`、`-swordsman-chapter-tests-check`、保护文件 SHA256 比较、`git diff --check`。最终机器汇总为 `Builds/ChapterTests/validation.json`。

Debug/Development 构建0错误、9项既有警告；四场景 missingScripts=0。原版资源、场景、工程设置、Packages及正式存档未修改，v1目录保留。工作区原有改动保留，不提交或推送。

建议先用新包抽查不同幕的推荐等级起点，再进入后续资源盘点；Camp34 营地路径作为独立待确认问题保留。


## 第二幕美术接入后的当前测试包（2026-11-01）

用户指定继续构建`Swordsman-ChapterTests-AllChapters.app`，本轮沿用上述构建入口；没有修改测试台代码、预设、冻结起点或存档路由。第二幕地图/标题/Face38图集新增到静态白名单，FACE02新增t9，当前22纹理/33头像表情；正式普通入口不启用替换，旧测试应用和HD预览应用保留。

新包构建0错误/9警告；包内预设和36个冻结起点共72文件与源哈希一致。测试台36幕资源入口、第一幕快照/普通存档/拒绝无效快照/旧档副本/路径隔离回归通过；使用`--package --stages 0-35 --jobs 3 --output Builds/ChapterTests/reports/chapter02-package-regression`重载全部包内起点通过。索引34的Camp34缺失仍记录为普通营地读取限制，不将其计为通过。当前证据见静态MVP唯一报告`evidence/chapter02-integration-validation.json`。

人工验收请打开该AllChapters包，选择“第二幕 紫金王鼎”，使用“剧情开场”检查正常剧情；“资源查看”可单独检查绘景/人物，“战斗起点”继续使用已归档的合成快照。自动资源/起点核验不等于正常剧情及全部分支已通过。


## 第三、四幕美术实机验收入口（2026-11-01）

本轮按用户指定构建同名AllChapters包，当前34项静态纹理、54头像表情，Development构建0错误/9警告；未改章节预设、冻结快照或测试台交互/存档目录规则。接入报告为`Builds/HD4X/static-mvp/evidence/chapter0304-integration-validation.json`。第三/四幕实际地图、标题、头像入口与RPG02检查及36个包内战斗起点重载回归通过，索引34营地读取限制继续保留。

正常测试台已打开并选中第三幕「狂剑客」，推荐等级11；按Enter从剧情开场开始验收。F9结束当前测试返回选幕，再选第四幕「隐者」，推荐等级9。B可进入已验证合成战斗起点，V仅资源查看；二者不代替正常剧情验收。用户手动实例不附加自动检查目录参数；普通测试存档仍与正式存档隔离。第三、四幕完整剧情用户确认尚未记录，通过后再收尾进入第五、六幕盘点。


### 2026-11-02 固定地形与石拱修复复验包

同名AllChapters已更新为46纹理/54头像表情，新增第三、四幕各6固定地形并替换map03石拱局部，构建0错误/9警告。21项接入/回退、36幕战斗起点和隔离存档、四场景检查通过；具体证据及9张真实渲染截图见`Builds/HD4X/static-mvp/evidence/chapter0304-floor-integration-validation.json`。索引34原Camp34缺失限制保留。检查入口显式触发，不影响正常测试台运行。

本轮交付为正常测试实例，无自动检查参数，第三幕「狂剑客」等级11已选中，Enter剧情开场；F9返回后选择第四幕「隐者」等级9。重点复验第三幕洞口两人入场与攻击/技能特写、第四幕对应特写。B/V为检查起点或资源查看，不代替用户正常剧情验收。完成后先补第零～第二幕18张，当前未制作；此前“直接进入第五、六幕”的历史计划由此顺序替代。


### 2026-11-02 完整石拱连续重绘替换包

AllChapters已重新构建，当前仍46纹理，仅更新map03完整石拱连续重绘（SHA256 `29664dffcc571647e2f25cb0614159232698b13ec8546612e5ef05013ff90997`）。本次0错误/0警告，资源/真实第三幕入口/缺包坏包检查、第三幕18特写UI和70原版入场帧通过。实际检查截图与完整证据见`Builds/HD4X/static-mvp/evidence/chapter03-arch-continuous-integration-validation.json`；本次未重复36幕起点检查，上一轮回归保留历史范围。

正常应用已打开，最终截图观察到第三幕剧情正在运行，保留当前状态供用户复验；重点看完整石拱下沿、两处原黑凸块及人物进场衔接；F9返回可继续第四幕「隐者」等级9。运行实例不携带自动检查参数。用户正常剧情实机确认仍等待，第零～第二幕18张未制作，先等本批GO。


### 2026-11-02 第零～第二幕18张固定地形复验包

用户已确认第三、四幕本批实机通过，现按“好了后直接替换”授权补制并接入第零～第二幕各6张固定地形。当前64静态纹理，含第零～第四幕30固定地形，其余原46项PNG/元数据不变；新18项未标用户美术/实机通过。构建0错误/9警告；30项正常与回退、90次真实攻击/技能UI选帧、四场景及36幕入口/战斗起点通过。第一幕旧检查内容通过后曾退出-11，单独重跑退出0，原因待定位，已保留记录；原Camp34普通读取限制不变。当前证据`Builds/HD4X/static-mvp/evidence/chapter0002-floor-integration-validation.json`。

正常实例已打开，最终截图观察到第零幕「独行」正常剧情正在运行，保留当前状态供用户复验。F9返回选幕后验第一幕「因缘」、第二幕「紫金王鼎」；Enter为剧情开场，B进入已有战斗起点。重点查看攻击/技能/地图技能的地形变体、原人物同屏与透明边缘；V仅资源查看，不能代替正常剧情实机验收。实例不带自动检查参数；当前只交复验，不自动进入第五、六幕。


### 2026-11-02 本批对照确认收尾

第零～第二幕18张在原版/高清对照评审后获用户确认并要求收尾，当前64项AllChapters保留，未重建或改动应用。固定地形三个工作包按“美术对照确认、归档与机器接入通过”完成，本轮未单独新增整幕剧情实机结论。前述“新18项未确认”的内容为此前阶段状态，最新以唯一清单及`Builds/HD4X/static-mvp/evidence/chapter0002-floor-closeout.json`为准。对照入口`Builds/HD4X/static-mvp/work/floor-old-hd-review/index.html`保留；归档原生图链接已核对，过程重复文件已清理。后续幕不自动启动。


### 2026-11-02 第五至第十幕统一交付，等待用户验证

请启动当前 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`，逐个选择第五至第十幕：V查看地图，Enter进入剧情开场，B进入独立战斗起点，F9回选幕。第五至第十幕完整OLD/当前高清对照位于 `Builds/HD4X/static-mvp/work/chapter-05-35-production/index.html`，按幕点击原尺寸对照，以100%检查细节；同页另列受控实际运行截图。

本阶段已统一接入并完成机器检查，具体当前数量与限制见[当前状态](current-status.md)，验证证据见[验证记录](verification.md)。测试台仍保留全部36索引入口，但本次重绘仅推进到第十幕；后续幕使用既有原资源。原Camp34普通营地读取缺失限制保留。请集中反馈需返工的幕/图片；验证完成前不继续第十一幕以后的生产。


### 2026-11-03 第九幕地图复验入口

同名 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app` 已更新第九幕地图及两张单帧剧情覆盖片。选择第九幕后按V查看，方向键巡视人物和各处地面，F9返回；Enter开场/B战斗仍沿用原入口。返工专用对照 `Builds/HD4X/static-mvp/work/chapter-09-map-redraw/index.html` 提供8个人物原尺寸OLD/当前、整图原版/当前及被退回版/当前对照，并附10个实际测试视口。用户正常剧情复验待确认；退出异常和Camp34既有限制见[验证记录](verification.md)，仍停在第十幕。


### 2026-11-03 当前8人原配色/动作版本

同名AllChapters已更新本次人物重绘，选择第九幕后按V查看、方向键巡视、F9返回。最新入口 `Builds/HD4X/static-mvp/work/chapter-09-people-original/index.html`，逐人打开原尺寸OLD/当前对照；旧map-redraw页已标历史版本。其他测试入口不变，用户正常剧情复验待确认。


### 2026-11-03 第十一至第十五幕统一交付

按用户明确授权，第十一「故人」、十二「密道」、十三「真相」、十四「萤火」、十五「魔骸」在同一任务连续完成，中间不等待确认。制作、逐幕完整对照、归档、AllChapters预览接入及机器检查完成；用户逐幕美术和最后统一正常剧情实机验收待确认，相关done及用户GO仍false，不扩到16..35。

使用当前AllChapters依次选择十一至十五幕：V资源查看、Enter剧情开场、B独立战斗起点、F9返回选幕。完整OLD/高清对照入口 `Builds/HD4X/static-mvp/work/chapter-11-15-production/index.html`，每幕查看完后可集中反馈具体图片及返工点，最后统一实机。测试台保留36幕入口，本轮生产仅到15，索引34原Camp34普通营地读取限制保留。具体实时检查范围见[验证记录](verification.md)。


### 2026-11-03 静态背景人形准则与第十五幕8处补绘

第十五幕新增8处棺内人物/遗骸补绘，当前同名AllChapters已更新。先查看 `Builds/HD4X/static-mvp/work/chapter-11-15-background-people/index.html` 的三方对照；11..15总入口和第十五幕总览已刷新。V资源查看后可用方向键移动镜头检查各棺内人形，Enter剧情开场、B独立战斗起点、F9返回选幕。本轮GPU截图仅初始墙面/对话视口，尚未覆盖8处人物；最终统一正常剧情实机由用户验收，未自动记录通过。


### 2026-11-03 第九幕8人周围地面统一（当前版本）

同名AllChapters已更新第九幕地面统一版本。选择第九幕按V、方向键巡视8人周围地面，F9返回；当前对照与实际截图入口Builds/HD4X/static-mvp/work/chapter-09-ground-unify/index.html，人物当前设计保持，用户复验待确认。




### 2026-11-03 第十六至第二十幕统一交付

第十六「战阵」、十七「祭天塔」、十八「千魔啸夜」、十九「飞天」、二十「埋伏」按本次授权连续完成，中间不等待确认。新增50纹理、扩展3个既有头像图集，共53纹理变化；新增8个战场地图/前景、2个共享RPG地图、5标题、30固定地形、11张表情及1张固定屏风/台座覆盖片。独立Sprite、动作/光影/环境循环继续复用，未启用正式默认替换。

逐幕完整OLD/当前高清对照共164项（32/28/25/30/49），总入口 `Builds/HD4X/static-mvp/work/chapter-16-20-production/index.html`；含每幕总览、完整原尺寸对照、当前PNG及原尺寸细节，高清侧逐RGBA回读并再核对当前归档/图集切片。总览只看布局。205份内置ImageGen成功原生图、实际提示词、输入/来源哈希保留；地图/地形2倍、头像4倍，不使用整体超分替代重绘。

第十六幕背景28处人形（21名背向卫兵及另外7处人物/遗骸）完成新绘，队列按原位置高低关系装配统一本人重绘，单独修正了坐倒人物被误画为持剑战斗姿势；未回填原人形RGB。专项原尺寸对照在该幕Map16/people-audit。城镇及大厅嵌字采用生成无字材质与原字形窄掩码分层，第二十幕移除新增地毯徽记/器物，固定覆盖片保持独立显隐。第十九幕独立人物Eanm_07不准入。

使用当前 `Builds/macOS/Swordsman-ChapterTests-16-20.app` 依次选择第十六至二十幕；V资源查看、Enter剧情开场、B独立战斗起点、F9返回选幕。每幕对照检查后集中反馈具体图片，最后统一正常剧情实机；测试台36幕入口保留，当前生产到索引20。


### 2026-11-03 第九幕8人与地图一体绘景（最新）

同名AllChapters已更新第九幕人物+场景一体绘景版本。第九幕按V、方向键巡视，F9返回；最新对照/原生绘景/实际截图入口Builds/HD4X/static-mvp/work/chapter-09-joint-paint/index.html。旧地面修补页和旧人物页已标历史方案，当前8人美术及整幕正常剧情复验待确认。


### 2026-11-03 第二十一至第二十五幕验收入口

当前 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app` 已更新至288静态纹理；第二十三幕有焚炎劫火/黄泉幽径两条章节，均纳入。先打开 `Builds/HD4X/static-mvp/work/chapter-21-25-production/index.html`，按幕检查155项完整OLD/高清对照，集中反馈需返工图片；同页各幕另有4张受控运行截图。最后统一实机：选择21..25显示章节，V查看资源、Enter剧情开场、B独立战斗起点、F9返回。自动检查不替代正常剧情与用户美术验收。日志/汇总见[验证记录](verification.md)。


### 2026-11-03 第十幕岩壁重绘测试包

已按用户整图确认替换MAP10并重新构建 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`，Succeeded，0错误/0警告。日志 `Logs/chapter10-cliff-approved-build.log`。按用户要求未启动游戏或执行额外运行验证，用户自行选择第十幕实机检查。变更范围及交付证据见[静态美术记录](static-art-mvp.md#2026-11-03-第十幕整图确认后接入与打包)。


### 2026-11-03 索引27至30审图及统一实机入口

先打开 `Builds/HD4X/static-mvp/work/chapter-27-30-production/index.html`，依次检查索引27「剑与非剑」、28「古月」、29「天琰神煌」、30「三界之门」共123项原版/高清对照。每幕总览只看布局，清晰度请点完整原尺寸图并以100%查看；同页有实际1280×800受控截图。集中反馈要重绘的图片后，最后使用 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`（当前326纹理）进行正常剧情实机验证。目录显示幕数与资源索引有偏移，以幕名对应选择；V资源、Enter剧情、B独立战斗起点、F9返回。用户GO仍待确认。日志与证据见[验证记录](verification.md)。


### 2026-11-03 RPG-Map02修正版测试包

已按用户确认替换RPG-map02并重新编译AllChapters应用，Succeeded，0错误/0警告。日志 `Logs/rpg-map02-continuous-fix-build.log`。按用户要求未启动游戏、未执行额外运行验证，实机由用户稍后统一检查。固定OLD/重绘对比图路径不变；范围与交付记录见[静态美术记录](static-art-mvp.md#2026-11-03-rpg-map02确认后替换与打包)。


### 2026-11-04 全部剩余章节审图与最后实机入口

索引31「无幻刹那」、32「天之颠地之涯」、33「千幻华舞」、34「冰岳绝剑」、35「特别幕 两百块帝国」的148项OLD/高清对照入口为 `Builds/HD4X/static-mvp/work/chapter-31-35-production/index.html`。每幕总览仅看布局，清晰度请打开完整原尺寸图以100%查看。检查并集中反馈返工图后，使用 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`（376纹理）进行最后正常剧情实机验证；本轮没有启动Player代验。原有Enter剧情、B独立战斗起点、V资源查看、F9返回保持；索引34既有Camp缺失限制保持。用户GO待确认，日志和证据见[验证记录](verification.md)。


### 2026-11-04 FACE02_t0愠色确认与统一包替换

用户确认“没问题，go吧，归档并替换吧”。本轮确认范围是FACE02_t0当前归档愠色表情，原图愠色、当前收眉且收平嘴角，采用已归档版本，不新增生成或重绘其他表情。同次把此前已确认的RPG-map11「鐵／華佗藥鋪」修字版同步进包。Map11-1仍保留最初第一版高清，柔和云层候选不采用；现役云PNG及元数据SHA不变。

沿用最新已扩展到376纹理的白名单和AllChapters，不回退其他批次成果。本次376组PNG/元数据与构建前锁定值一致，FACE02_t0切片与用户确认PNG逐RGBA一致，RPG-map11与确认修字图SHA一致；190项11..15评审对照回读通过。

当前 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app` 已更新；本轮实时Development构建Succeeded、0错误/0警告，日志 `Logs/face02-t0-approved-development-build.log`，四场景Missing Script=0。9项资源/实际场景/回退检查通过，覆盖472资源探针、15排除、3错误几何回退、第十一至十五幕真实入口中的头像、共享map11 RPG入口、缺包及坏包回退；codesign深度严格校验通过。196767个原版资源/配置/其他代码/已有存档及普通与旧预览应用保护文件未变。本轮未做用户正常剧情或目标表情的GPU逐帧视觉验收，最终实机待用户；索引34原Camp34缺失限制不变。

仅补充FACE02图集元数据中的本轮确认记录，更新5份相关文档/清单与评审状态，并替换统一包；本轮图集PNG、其他图像、业务/解析/显式检查代码、工程配置、签名版本均未改。运行protect.py before/after、run-checks.py、归档哈希/切片/对照回读检查、SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters、SwordsmanSceneValidation.ValidateScenes、codesign --verify --deep --strict、git diff --check。已有工作区修改保留，无新增异常Git路径，未提交/推送。证据 `Builds/HD4X/static-mvp/evidence/face02-t0-approved-integration-20261104.json`。


### 2026-11-11 主菜单入口与可关闭蒙层

本轮仅修改 `SwordsmanChapterTestDesk.cs` 的测试入口及显示生命周期，不改场景、Prefab、原版标题逻辑、美术、存档格式或Mock效果。取消默认自动打开测试台，等待Title场景加载完毕、片头结束、标题淡入完成且无读档/帮助遮挡后显示右上按钮。测试中原返回功能保留，回到选幕后关闭可见主菜单，不会把标题片头暂停在底层。

右上按钮位于已有HD标记下方，避免重叠。测试台采用全屏15%黑色遮罩及78%不透明度的深色面板；按钮保持原有对比度。左上新增关闭按钮，标题右移避免重叠。打开时记录原时间倍率和标题暂停状态，关闭时恢复；等待关闭输入释放并屏蔽紧随的帧，避免Esc/F9或鼠标穿透到底层菜单。章节选择、快捷键、七组Mock选项及独立存档目录沿用原逻辑。

构建与交付沿用 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`，只更新 `Builds/macOS/Swordsman-ChapterTests-AllChapters.app`。验证证据位于 `Logs/chapter-desk-modal-20261111/`，结果如下。


本阶段实现和打包完成：最终Development构建Succeeded，0错误/9个既有警告；四场景Missing Script=0；去Logo字段与codesign深度严格检查通过。第一幕包内战斗快照复用、战斗重载、状态回读、普通存档回读和隔离检查通过（`stage01.json`，非完整剧情通关）。GUI实测确认片头期间不出现测试台、结束后主菜单右上按钮可见、F9打开半透明蒙层、Esc关闭、关闭后方向键可改变主菜单选择；再次打开后V进入资源查看，F9返回选幕后Esc能关闭回标题，日志时间倍率由0恢复为1。

鼠标自动化点击未能完成目标按钮命中：屏幕目标与Unity收到的指针坐标不一致，未判定为鼠标验收通过，也未为自动化工具改变用户的坐标或输入系统。两处鼠标按钮已接入既有点击分发，但仍需用户实机确认。截图为 `final-main.png`、`final-panel.png`、`final-closed-input.png`、`returned-title.png`；未把快捷键成功表述为鼠标成功。

本轮仅修改测试台脚本与本文，无新增源码/依赖；游戏正式流程、场景、Prefab、美术、存档格式、Mock效果、ProjectSettings及Packages未改。统一更新AllChapters应用，未创建新验收应用名；回滚副本在 `Builds/macOS/archive-20261111-before-desk-modal/`。Git检查保留原有修改，无本轮新增异常文件，未提交。建议下一步由用户在统一包确认鼠标开关与蒙层观感。
