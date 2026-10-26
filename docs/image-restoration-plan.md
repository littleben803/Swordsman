# 静态资源高清修复技术方案

- 调研日期：2026-10-26
- Unity 基线：`2021.3.16f1c1`
- 目标平台：macOS
- 状态：技术选型与第 0 幕最小实验方案；尚未修改或生成正式资源

## 结论

将静态画面提升到 `1280 × 800` 可行，但不能把它理解成一次图片批量放大。建议保留 `320 × 200` 逻辑坐标，把当前约 `2 px / 逻辑单位` 的素材密度提高到 `4 px / 逻辑单位`，并同步处理 Sprite 切片、PPU、UI 原生尺寸、纹理导入上限、透明边缘、动画帧一致性和构建体积。

技术路线采用“确定性本地超分为主，生成式编辑为少量升级通道”的混合方案：

1. 场景底图以 Real-ESRGAN `x2plus` 为第一基线，SwinIR real-world 模型作为对照。
2. 角色、怪物和二维动画以 Real-CUGAN `2x` 保守修复为第一基线，Real-ESRGAN anime 模型作为对照。
3. Alpha 不交给生成模型自由重画。RGB 与 Alpha 分离处理，Alpha 使用确定性缩放，合成后检查轮廓、黑边和光效。
4. GPT Image 2.5 Sunburst 只用于确定性超分仍无法修复的少量高价值、不透明静态背景区域。它不进入批量默认链，不用于战斗 Sprite、连续动画、碰撞相关轮廓或必须逐像素保持的中文题字。
5. 视频不在 MVP 范围内；玩法、数值、剧情、场景布局和角色设计均不改变。

Real-CUGAN、Real-ESRGAN 和 SwinIR 都可以本地离线执行，适合可重复的批处理。GPT Image 2.5 Sunburst 的定位是精确编辑，但输出仍需人工复核，不能把“高输入保真”理解成逐像素无变化保证。

## 已从工程确认的约束

- `GameDefine.SCENE_WIDTH/HEIGHT` 为 `320 × 200`。
- 战斗/RPG/营地主要 UGUI 参考分辨率为 `640 × 400`。
- 当前绝大多数纹理和独立 Sprite 使用 PPU `2`；`GameAnimation` 的 UGUI 路径还用固定 `0.5` 把 Sprite 像素换算为逻辑尺寸。
- 现有第 0 幕地图 `map00.png` 已是 `2048 × 1024`，导入器 `maxTextureSize` 为 `2048`。若直接替换成 `4096 × 2048` 而不改导入上限，Unity 会在导入时缩回去。
- `Assets/Sprite/` 中的大量 Sprite 是独立序列化切片，记录固定像素 Rect、Offset、UV 和 PPU。只把底层 PNG 放大，会导致切片区域和显示尺寸错误。
- Stage 动画中存在多种 `textureType` 与透明混合语义；部分光效或黑底资源不能按普通透明 PNG 统一处理。

因此生产方案必须引入受控的资源转换工具，而不是手工改 `.asset`、Prefab 或 `.meta` YAML。

## 目标渲染模型

| 项目 | 当前 | 高清目标 | 处理原则 |
| --- | ---: | ---: | --- |
| 逻辑视口 | `320 × 200` | `320 × 200` | 保持不变 |
| 冒烟/正式窗口 | 历史基线曾为 `1280 × 720` | `1280 × 800` | 固定 `16:10` |
| 常规素材密度 | 约 `2 px/单位` | `4 px/单位` | 图片线性尺寸放大 2 倍 |
| World Sprite PPU | 通常为 `2` | 通常为 `4` | 保持世界尺寸不变 |
| UGUI Sprite 尺寸 | `rect × 0.5` | `rect / sprite.pixelsPerUnit` | 去掉硬编码像素倍率 |
| 动画逻辑偏移 | 旧 `textureX/Y` | 保持原值 | 不随图片像素翻倍 |

这里的“提升到 1280”是提高输出和素材采样密度，不是把所有游戏坐标乘以 4。

## 资源转换流水线

### 1. 清点与分类

为每个输入记录路径、GUID、尺寸、Alpha、引用的 Sprite 切片、Shader/`textureType`、是否连续帧和 SHA-256。分类至少包括：不透明地图、透明地图层、独立 Sprite、图集 Sprite、连续动画、光效、中文题字和 UI。

### 2. 生成候选图

- 所有候选输出到实验目录，不覆盖原图。
- 图集按 Sprite Rect 裁块、加边缘 padding、分别修复，再按放大后的 Rect 回填；禁止把相邻帧当成一张插画共同重绘。
- 透明资源先做去预乘/边缘扩展，RGB 超分，Alpha 单独确定性放大，再重新合成。
- 同一动画序列固定模型、倍率和参数；禁止逐帧人工改提示词。
- 中文题字优先做确定性缩放和人工描边清理。若字形发生变化，直接判定失败。

### 3. Unity 资源接入

实验通过后再开发 Editor 转换工具，按同一倍率生成或更新 Sprite 切片及导入设置。工具必须支持 dry-run、变更清单、备份/回滚和引用验证；不得调用现有 `GameToolsMenu` 批处理命令代替专项工具。

预期需要处理：

- Texture Importer 的 `maxTextureSize`、压缩和 Alpha 设置；
- Sprite Rect、像素 Offset、纹理坐标与 PPU；
- `GameAnimation` 中 UGUI 尺寸从固定 `0.5` 改为基于 `sprite.pixelsPerUnit`；
- 现有 Prefab/GUID 引用保持不变或提供显式映射；
- 原始资源一键回退。

### 4. 运行时验收

同一存档、同一镜头、同一帧分别截取原版与高清版本，至少检查：

- 构图、角色比例、碰撞/站位和可见逻辑范围不变；
- Alpha 轮廓、黑底光效、遮挡顺序和 Shader 混合不变；
- 连续帧无呼吸、闪烁、五官漂移或线条跳动；
- 中文字形和 UI 九宫格不变形；
- 内存峰值、加载时间、包体积和帧率在预算内。

## 第 0 幕最小实验集合

第一轮只做 5 组、10 张源图，不直接接入正式资源：

| 组 | 源资源 | 数量 | 验证风险 | 首选候选 |
| --- | --- | ---: | --- | --- |
| A 场景底图 | `Assets/Resources/texture/map/stage00/map00.png` | 1 | 大尺寸写实绘景、细节真实性、4K 导入上限 | Real-ESRGAN x2plus；SwinIR 对照 |
| B 透明前景层 | `Assets/Resources/texture/map/stage00/map00-2.png` | 1 | 大面积透明、树叶/岩石边缘、叠层接缝 | RGB: Real-ESRGAN；Alpha: Lanczos/双三次 |
| C 幕标题 | `Assets/Resources/texture/map/stage00/Tanm_00_t0.png` | 1 | 中文字形、纹理、黑底/透明语义 | Real-CUGAN 保守 2x；禁止生成式改字 |
| D 连续地面帧 | `Assets/Resources/texture/stage/stage00/floor_00/FLOOR_00_t0.png` 至 `FLOOR_00_t5.png` | 6 | 帧间一致性、循环闪烁、贴图接缝 | Real-CUGAN 保守 2x |
| E 角色/怪物图集 | `Assets/Texture2D/Eanm_11_5.png` | 1 图集、4 个代表切片 | 小尺寸人脸、老鼠轮廓、图集切片、Alpha | Real-CUGAN 保守 2x；Real-ESRGAN anime 对照 |

说明：E 组源图是 `128 × 256` 图集，包含人物和怪物帧。首轮只选择 4 个差异明显的切片生成候选，但必须按未来图集回填规则验证坐标，不能只看单张裁图是否好看。

为了把变量分开，实验分两步：

1. **离线质量门**：只生成候选和对照图，评审清晰度、保真、Alpha、字形和帧一致性。
2. **Unity 接入门**：离线质量通过后，复制到隔离实验路径，验证 PPU `4`、切片翻倍、UI 尺寸换算、`4096` 导入和第 0 幕实际合成。没有通过第一步，不进入 Unity 接入。

## Go / No-Go 标准

Go 必须同时满足：

- 盲评相对原图和普通 Lanczos 2x 有明确可见提升；
- 构图、角色身份、服装、武器、动作和中文字形没有语义变化；
- Alpha 边缘没有新增黑边、白边、孔洞或半透明噪点；
- 连续帧以原帧率循环 10 次无明显闪烁或形变；
- 原图/高清图切换后世界尺寸和 UI 尺寸误差不超过 0.5 逻辑单位；
- 第 0 幕 1280 × 800 冒烟无裁切、拉伸、错层、丢图或明显性能回退；
- 原始资源可在一次操作内恢复。

任一关键人物、中文题字、透明轮廓或动画一致性失败，则对应类别 No-Go；不能以“整体更清晰”为理由接受内容变化。

## 成本预估

### 实验阶段

- 本地超分模型没有按张 API 费用；成本主要是工具封装、候选比较和人工 QA。
- 预计工程投入为 2～4 个工程日：约 0.5～1 日完成离线脚本与清单，0.5～1 日完成候选/评审，1～2 日完成隔离 Unity 接入与回归。
- GPT Image 2.5 Sunburst 首轮不是必跑项。若 A 组确定性方案失败，只选 1～2 个不透明裁块，每块生成 2～3 个候选，并设置不超过 10 美元的实验硬上限。实际费用按输入/输出 token 由 API usage 记录，不用静态单价反推整项目成本。

### 全量阶段

本地快照约有 9,937 张 PNG，且大量 Sprite 由 1,325 张源图集切片而来。全量成本不能简单按 PNG 张数计算，主要取决于：

- 需要修复的唯一源图集数量和面积；
- 需要人工审核的连续动画组数；
- 2048/4096 大图是否拆图；
- 压缩格式、包体和显存预算；
- 多少资源需要生成式升级或人工修边。

所有图片线性放大 2 倍会使未压缩像素量约增至 4 倍。当前约 6.28 GB 的 Development Build 不能据此直接推算最终包体，但应预期资源包体、导入时间和运行时纹理内存显著增长。只有第 0 幕实验拿到真实压缩率和峰值内存后，才能给出可信的全量工期和磁盘预算。

## 安全、授权与回退

- 原游戏美术的发行授权仍待独立核查；高清修复不会自动产生新的发行权。
- 本地离线模型优先，避免把整套原始资源上传到外部服务。
- 使用 GPT Image 前必须单独确认允许上传的资源范围、数据处理约束和预算；未经确认不上传。
- 不覆盖源图，不改原 GUID，不把模型权重、生成缓存、大图候选或构建产物提交到 Git。
- 每个输出保存源文件哈希、模型/权重版本、参数、处理时间和人工结论，确保可复现和可回滚。

## 下一阶段建议

下一阶段仅执行“离线质量门”：搭建可复现的本地 2x 对照脚本，按上述 5 组生成候选和对比图，不改 Unity 工程资源。候选评审通过后，再单独确认 Unity 接入阶段允许修改的 Editor 工具、`GameAnimation.cs`、实验资源目录和 importer 设置。

## 参考资料

- [OpenAI Image generation](https://developers.openai.com/api/docs/guides/image-generation)
- [OpenAI Image prompting](https://developers.openai.com/api/docs/guides/image-prompting)
- [Real-CUGAN 官方说明](https://github.com/bilibili/ailab/blob/main/Real-CUGAN/README_EN.md)
- [Real-ESRGAN 官方仓库](https://github.com/xinntao/Real-ESRGAN)
- [SwinIR 官方仓库](https://github.com/JingyunLiang/SwinIR)
- [Unity TextureImporter.maxTextureSize](https://docs.unity3d.com/2021.3/Documentation/ScriptReference/TextureImporter-maxTextureSize.html)
