# 项目架构

## 2026-11-16 已迁移OLD资源专项退役完成

用户批准的611张完整替换旧图、1971个无引用旧Sprite及对应meta已删除；48张混合图集保持。正式运行继续按稳定ID/路径别名从现有HD Bundle加载，不依赖已退役GUID对应文件。构建几何及历史源哈希转存到SHA锁定的 `hd-retired-source-geometry.json`，Editor工具使用轻量数据记录，不再读取退役像素；混合图集继续保留活动源。默认增量构建复用1819项高清缓存、重导入0，未来可由批准HD输入和冻结几何重建，不需恢复OLD。

唯一应用已更新并完成编译、四场景和运行/画面检查；详细大小、删除范围和一次第24幕并发退出异常的复验记录见[最新交付](verification.md#2026-11-16-已迁移old资源清理与唯一应用交付)。以下“旧资源仍保留”章节仅代表历史阶段。

## 2026-11-16 高清引用迁移已进入唯一应用

第2阶段的ID绑定与高清常驻已构建进入唯一AllChapters应用，运行、36幕包内起点、31组地图/人物图检查和原生章节测试台/F9/资源查看通过。描述表修正29条原Sprite名称大小写，Development检查器补齐31/34幕后续图层前置加载，未改剧情或关卡。147张旧完整图集与1507个旧Sprite由Unity自动从主资源中剔除，本地611张旧图及48张混合图集仍保持。实际包大小、证据和验收范围以[本次交付](verification.md#2026-11-16-高清引用迁移唯一应用交付)及机器清单 `referenceMigration` 为准；以下“尚未构建”章节保留当时源码阶段含义。

## 2026-11-16 第2阶段：高清引用迁移（源码与资产已验证）

611张完整替换旧图（464独立贴图、147完整图集）的正式引用改为现有HD Bundle中的1971条轻量Sprite描述。141个Prefab、4个场景共3804字段由ID组件绑定，不再硬引用本批旧Sprite；464条原资源路径直接取高清，其他原版路径保持。场景绑定先于普通Awake，Prefab加载入口支持直接检查其数组；帧索引、时序及非引用序列化数据保持。小地图按实际PPU计算，世界尺寸不变。

48张混合图集、全部旧PNG/Sprite文件保持；本批正式引用的几何约束改由描述表提供。Resources自动打包规则仍有效，本阶段不清理旧文件、不重建应用。章节测试台/F9保留，高清缺失显式失败。机器清单见 `static-art-mvp-resources.json.referenceMigration`，验证见[本次记录](verification.md#2026-11-16-高清引用迁移第2阶段未删旧资源未重建应用)。以下第1阶段章节保留当时范围。

- 更新日期：2026-10-26
- 适用基线：Unity `2021.3.16f1c1`

## 2026-11-16 高清加载常驻（源码阶段）

高清解析器不再受 `SWORDSMAN_HD4X` 宏控制。精确白名单内的资源必须解析到获准高清Bundle；缺失、初始化失败或几何不符显式抛出 `HD_REQUIRED:`，不回退OLD。白名单外角色/动画、未选中图集切片仍走原版路径。原Sprite/GUID和旧图仍保留作为当前序列化与几何输入，引用迁移另行分阶段执行。

运行时HD标记组件、F8/鼠标标记开关及标记偏好读写已删除；章节测试台和F9保留。Development显式场景检查仅保留高清预期分支。此变更已完成脚本编译及四场景验证，尚未重建唯一应用，包内行为仍对应上次构建。

## 总览

项目是场景驱动的单机 Unity 架构。可类比为四个大型 ViewController：每个场景承载一类主流程，多个全局单例共享状态、资源、音频、输入和跳转能力。

```text
level0 Title ───────┐
                    ├── GameSceneManager ── level1 RPG
                    ├── GameSceneManager ── level2 Camp
                    └── GameSceneManager ── level3 Battle

全局服务：GameManager / GameUserData / GameSetting / 音频 / UI / 输入
数据来源：本地 Resources + 场景/Prefab 序列化数据 + 本地存档
历史能力：网络接口 / 激活 / iOS 支付（云存档已于2026-11-12移除）
```

`GameSceneManager` 直接使用 `GameSceneType` 枚举值作为 build index。场景调整必须同时审查枚举、场景管理器、`SwordsmanMacBuild`、所有依赖 active scene index 的代码和发布构建配置。

## 代码模块

- `Base`：基础类型、缓存、定义、设置、日志、历史网络层和单例实现。`GameSetting` 与 `GamePHP` 属于高风险边界。
- `Config`、`Data`：配置及角色、技能、物品、文本、经验、战斗和关卡数据；改动可能影响数值、解析或存档兼容。
- `Manager`：游戏、场景、音乐、电影、UI、相机、输入、事件和触摸管理器。`GameManager.Start` 是主要初始化入口之一。
- `Title`：标题、开始、本地普通读档和战斗续关。
- `RPG`：非战斗地图移动、寻路、事件和场景状态。
- `Camp`：营地、整备、炼化、商店及相关 UI/输入。
- `Battle`：回合、单位、AI、寻路、攻击、技能、地图、事件、UI 和结算，是最大的业务模块。
- `UI`：共享 UI 基类和各场景界面，多数与场景对象或 Prefab 引用紧密绑定。
- `Save`：`GameUserData` 管理玩家状态、本地档和战斗临时档，是兼容关键区。云同步明确不做，云存档入口、缓存和读写分支已删除。
- `Assets/Editor`：macOS 构建和场景验证工具。
- `Assets/Scripts/Editor`：旧资源处理工具；可能重写 importer 或 Prefab，不能作为无副作用诊断运行。

## 资源系统

运行资源主要通过 `Resources.Load` 加载，常见类别为 Material、Movie、Music、Prefab、Sound 和 Texture。`Assets/Sprite/` 与 `Assets/Texture2D/` 还存放大量本地资源。

资源路径相当于公开 API：改变大小写、目录或文件名之前，必须搜索代码引用并在 Unity 中验证。上述大资源目录按项目策略不由 Git 管理。

## 状态与持久化

### 2026-11-11 技能容量与旧档兼容

用户明确授权正式游戏与测试模式一起解除10项技能的学习/选择上限。`GameUnitBase.Skills` 保留原字段名及 `short[]` 类型，初始10槽，`addSkill` 在满槽时扩展数组并拒绝重复；不扩大同时用于物品栏和UI行数的 `MAX_SLOT`。战斗菜单每页仍10行，按实际技能数量分页，鼠标或Tab换页；正常等级、五内、技能内容及学习条件不改。

本地普通档仍3000字节，战斗档仍102400字节，既有字段次序及前10个技能槽完全不变。`GameSkillSaveExtension` 仅在存在额外技能时，在原缓冲尾部空闲空间写入 `SM_SKL01` 扩展：48字节尾头包含标记、实际核心长度、扩展负载长度与SHA256，负载按角色索引保存第11项起的技能。普通档现行核心2922字节，78字节预留空间足够保存封寒月和紫枫的两项额外技能。写入前检查与核心无重叠，再打开目标文件；无额外技能时不写扩展，保留旧字节布局。

旧档无扩展时按原10槽读取，并清除同一内存对象上次载入的额外技能。合法扩展先完整校验边界、摘要和记录，全部通过后一次应用；损坏扩展记录警告并保留可读取的前10槽核心，不部分恢复。旧程序仍可读新文件的前10槽，但不会保留它不认识的额外技能；因此带额外技能的新档不应再用旧程序覆盖保存。未新增或启用历史云服务，当前支持与验证目标是macOS本地存档。

相关实现为 `Assets/Scripts/Save/GameSkillSaveExtension.cs`、`GameUserData.cs`、`Assets/Scripts/Data/GameUnitInitData.cs` 和 `Assets/Scripts/Battle/GameBattleSkillUI.cs`。独立格式读写及真实游戏存读验证结果统一记入测试台专项记录。


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
- `GamePHP` 及历史调试、激活、日志调用
- `Resources.Load` 路径和本地大资源
- 序列化字段、脚本类名、`.meta` GUID
- `ProjectSettings`、插件 DLL 和原生平台代码
- 战斗回合、AI、数值和剧情事件

这些区域的修改必须单独设定边界与专项验收标准。


## 2026-11-14 设置页与虚拟键盘

设置页移除原6个帮助/攻略/QQ群外链入口与评价按钮及回调；语言表的退役位置留空，既有枚举索引保持。保留底部原有B站/业务联系展示区域。难度文案改为“简单模式/困难模式”，仍对应Normal/Hard，敌人数值公式不变。删除评价状态对Stage=31角色0地图事件门槛的减免，统一要求累计熟练度至少25；其他角色、装备、剧情标记及事件格条件不变。旧review偏好不再读取或写入，不迁移游戏存档。

`GameSetting.virtualKeyboardEnabled` 使用新增同名PlayerPrefs键；无该键时iOS/Android默认打开，macOS/Windows及编辑器默认关闭。已选开关跨重启保留。`touchScale`沿用原键和100%～150%范围，每次10%。虚拟键盘控制原有EasyTouch方向盘及A/B操作键，营地/RPG还显示C/D；不是系统文本输入键盘。

`GameTouchCenterUI`将平台编译开关改为运行时用户设置，保留各场景/剧情原有收起与显示调用。`GameInputManager`同时接受可见虚拟按键与实体键盘。当前工程activeInputHandler=0；在仅启用旧Input的编译条件下，`GameTouchCenterUI`运行时禁用历史InputSystemUIInputModule并补齐StandaloneInputModule，使既有UGUI/EasyTouch接收鼠标与触摸。关闭EventSystem默认导航事件，由原GameInputManager处理导航/确认，避免重复提交。Input System DLL、场景EventSystem序列化配置、ProjectSettings及Packages不修改。关闭、收起及窗口失焦时清除输入状态；通过EasyTouch公开activated接口复位其内部按压状态，不修改第三方插件。PlayerPrefs设置不进入普通/战斗存档，测试与正式存档目录隔离策略不变。未新增依赖、网络或权限。

本节描述实现边界；实际平台与验证结果见[验证指南](verification.md)。


### 2026-11-14 虚拟键盘单项切换

虚拟键盘行改为与语言行相同的单个当前值：打开或关闭。标签与当前值都可点击；选中该行后通过原Confirm动作（Return/Enter/Space或虚拟A）反转开关。导航从7项缩为6项；缩放、平台默认值与偏好键不变。虚拟A对应Confirm，虚拟B对应Cancel（Escape或C），不是实体键盘字母A/B。


### 2026-11-14 缩放设置禁用联动（待统一打包）

虚拟键盘关闭时，“键盘大小/缩小/放大”显示为灰色，缩放Button不可交互，原回调也拒绝修改大小；键盘导航正反向跳过两项。重新打开恢复交互并保留原touchScale。不改场景、存档、平台默认值或其他设置。此修订已做脚本编译与四场景检查，新增专项断言尚未在Player运行；按用户要求暂不重建应用，等待后续优化完成后统一打包。
## 2026-11-14 虚拟键盘古铜半透明皮肤接入

用户批准中央金属圆环与“天地劫”篆书风格浮雕印章。`GameVirtualKeyboardSkin`在左右触控UI的`initSingleton()`中只替换显示Sprite，并将A/B位置设为右下锚点的`(-144,60)`/`(-60.5,132.25)`：A左下、B右上。DPad仍为原230×230整体EasyTouch控件，buttonSizeCoef=3、中心无方向输入区及全部方向回调保持；A/B仍各100×100，确认/取消回调不换，C/D保持原样。没有改插件、场景、Prefab或工程配置。

三张本地RGBA Sprite为`Assets/Resources/VirtualKeyboard/Bronze/{DPad,A,B}.png`，Unity生成对应.meta。原版Sprite保留不覆盖，运行时改显示引用。按钮底面Alpha上限138/255，金属及字形上限243/255，外侧及间隙透明；Image为白色，避免再次叠乘旧0.5透明度。A/B按下沿用同一皮肤并变暖色，不切回原红色图；旧文字对象仅隐藏以避免双字。开关、缩放、淡入淡出及输入组件保持原实现，无新增偏好、网络或依赖。

2026-11-14已通过导入、脚本编译、四场景及47项Editor Play Mode检查，查看实际1280×800的100%/150%截图确认可透出背景。独立Player未构建、未做新皮肤原生鼠标复验；遵照用户要求，等后续指示统一构建唯一测试包。证据`Logs/virtual-keyboard-bronze-20261114/delivery.json`；资源权威记录在`static-art-mvp-resources.json`的`virtualKeyboardSkin`，不计入原1819项HD Bundle。
2026-11-14后续用户退出并授权统一构建：上述皮肤及关闭键盘后禁用尺寸行的修改已进入唯一AllChapters Development包，构建0错误/9既有警告；33项最终Player检查、实际截图、高清资源及签名检查通过，用户实机手感与美术验收待确认。当前交付以`Logs/virtual-keyboard-bronze-20261114/player/delivery.json`为准，前文未构建表述仅对应接入阶段。
## 2026-11-15 紧凑A/B布局，待统一构建

用户要求继续缩小、缩短间距并靠右下，明确选择允许点击区缩到80×80。当前`GameVirtualKeyboardSkin`将A/B设为80×80，中心分别为右下锚点`(-111,45)`、`(-45,111)`，A左下/B右上。从原始617×617 RGBA切图增加44px透明留边得到705×705，显示直径为原始约70%（比上一版80%再缩约12.5%）；无重绘、插值或Alpha变化。两中心距由约110.4降至93.34，正常倍率下右侧/底部可见像素padding约11.30/10.73。点击矩形的空白角区仍有14×14交叠，逐像素检查两键全部非零Alpha像素均不进入另一键点击矩形，避免对可见按钮的误拦截。

仅改A/B外观尺寸与位置和获准点击尺寸，确认/取消回调、EasyTouch插件、DPad、C/D、开关及缩放倍率规则保持。运行验证工具同步80×80及坐标断言。47项Editor、四场景、1280×800的100%/150%实际渲染通过，当前未构建或更新唯一应用；现有Player仍为之前80%显示尺寸及100×100点击区版本。证据`Logs/virtual-keyboard-bronze-20261115/ab-compact/delivery.json`，后续以用户构建指令为准。
2026-11-15后续用户确认退出并授权更新：紧凑A/B的70%显示、80×80点击区、中心(-111,45)/(-45,111)及右下留白已进入唯一AllChapters包，构建0错误/9既有警告、33项最终Player检查及实际100%/150%截图通过。前文“待统一构建”仅对应源码接入阶段；当前证据`Logs/virtual-keyboard-bronze-20261115/ab-compact/player/delivery.json`。用户实机视觉和鼠标手感待确认。
## 2026-11-15 左方向盘进一步缩小并匹配右侧留边，待打包

用户要求左侧再小一点且左下padding与右侧A/B一致。当前DPad透明画布1550×1550，原1254×1254核心RGBA原样居中、各侧留148px；比上一版1394画布的显示比例再缩约10%，相当于初始显示的80.90%。`GameVirtualKeyboardSkin.ApplyLeft`仅将原230×230控件位置设为(101.44,99.39)，大小、buttonSizeCoef=3、方向回调和中心无方向区不变；点击区域随图案移动，透明边缘可在Canvas外裁掉，不改变可见按钮和方向判定。可见左边缘对应B键右边缘的留白，可见底边缘对应A键底边缘，随既有touchScale同步缩放。A/B、插件、场景和工程配置保持。

Editor验证按实际PNG非零Alpha边界、真实RectTransform及Canvas坐标测量，在100%和150%分别检查左/右与底部留白相等（容差0.05 Canvas单位）及可见图案未越界，并保留方向输入检查。54项Editor及四场景Missing Script=0通过，实际截图已查看；未构建或更新唯一Player。证据`Logs/virtual-keyboard-bronze-20261115/dpad-compact-padding/delivery.json`，待用户后续统一构建指令。
## 2026-11-15 设置页工作室信息与官网入口

设置页底部三行改为：`点击访问「Biyou Village Studio」主页`、`马村长欢迎您加入「碧游村」`、`微信：27771640`，同步简繁。`GameHelpUI.StudioInformation`为运行时文案来源，`updateSettingText()`在展示和语言切换时更新；初始场景Text和原工作室文案的序列化简繁条目也同步，枚举/数组长度及其他条目保持。用户括号内的链接说明不作为产品文案。

`onClickB()`补齐现有Button绑定，使用固定`https://www.ideaboxapps.com/`调用`Application.OpenURL`。官网地址依据相邻官网仓库`COMPANY_INFO.md`的主访问地址，已核对HTTPS200。只有原`buttonB`的点击Rect改为142×10、中心(0,-31)；其Text为284×56、中心偏移(0,-9)、原0.5缩放，完整三行中心仍在原(0,-40)。第一行落在点击区内，第二三行说明文字位于点击区外且Text不接收射线。没有内嵌网页、自动跳转或新SDK；网页打开交给操作系统处理。

14项工作室入口、54项键盘回归、四场景及简繁实际画面通过；Editor真实Button回调已调起系统浏览器，观察到Chrome打开官网主页。没有构建/更新Player，其他平台尚未实机验证。证据`Logs/studio-homepage-20261115/delivery.json`。
2026-11-15用户授权合并更新后，方向盘大小/匹配padding与工作室三行文案/官网入口已同时进入唯一AllChapters Development包。构建0错误/9既有警告，49项最终Player联合检查及实际画面通过；从最终包的真实Button指针点击处理器实际调起系统浏览器并核对官网首页。以上各节的“待构建”仅对应接入阶段；当前交付证据`Logs/settings-combined-20261115/delivery.json`，用户实机验收仍待确认。


## 2026-11-15 三档难度扩展

全局mode保留Normal=0/Hard=1，新增Easy=2；界面中等/困难对应原算法。简单策略集中在GameEasyRules，战斗中禁止切换，不新增存档字段。经验临时余额在行动升级流程内结清，持久Exp仍使用原short；五蕴成长、技能门槛、剧情及31幕条件保持。具体规则和兼容证据见[difficulty-mode-proposal.md](difficulty-mode-proposal.md)。


## 2026-11-16 剧情对话速度与奖励提示计时

GameSetting.fastDialogue使用新增全局PlayerPrefs键fastDialogue（0正常/1快速，缺省0），不进入GameUserData或任何存档槽格式。设置页在难度下以运行时克隆原语言行的方式增加单项切换，7个导航位置为难度/对话/语言/虚拟键盘/缩小/放大/关闭；场景和Prefab不改。

GameMsgBoxUI.showDialogue显式取1或1.5倍率，仅由战斗事件onMoveToTalk和RPG剧情talk调用。原showText签名保留且固定倍率1，经验及系统提示继续使用该接口；GameMsgBoxTextUI将倍率传给GameImageText，在字符计时及自动翻页等待中使用，不改Time.timeScale、剧情脚本等待、走路/战斗动画或选择框。正常名义字间隔0.1秒、完整显示/分页等待5秒，快速约0.0667秒/3.333秒；实际逐字节奏受帧率量化影响。新消息会清理旧分页暂停状态，手动加快和继续操作保持。

GameBattleGetItemUI独立停留2秒后调用原unShowFade；场景原FadeOutTime=0.3不变，原Ent/Esc提前关闭不变。金币/物品发放时机及数量不改。验证与文件范围见verification.md的2026-11-16条目。

## 2026-11-17 macOS高清Bundle存储

正式macOS Player的高清Bundle由共享存储和LZ4HC基线进一步编码为系统LZFSE分块容器，仍位于原 `StreamingAssets/SwordsmanHD4X/swordsman-hd4x` 路径。运行时 `SwordsmanHd4xSpriteResolver` 检查容器头，经 `SwordsmanLzfseStream` 提供可Seek的UnityFS视图给 `AssetBundle.LoadFromStream`；Editor原始UnityFS源仍使用 `LoadFromFile`。资源ID、Sprite几何及原版资源路径不变。构建流程和验证见 [LZFSE记录](lzfse-validation.md)。
