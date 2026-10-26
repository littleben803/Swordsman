# Shader 恢复记录

- 更新日期：2026-10-26
- Unity：`2021.3.16f1c1`
- 状态：动画 Shader 已通过场景、构建和运行画面验证；地图 Shader 已通过场景与构建验证，运行画面待人工复测

## 问题表现

迁移后的 macOS Development Build 中，战斗角色、灯光、部分动画和部分地图叠加层出现矩形、圆形或不规则的大面积黑底。原始 PNG 并非缺少透明数据，但透明区域仍被作为不透明像素绘制。

## 根因

迁移后的 `Assets/Shader/Game_Type0.shader` 至 `Game_Type7.shader` 带有 `DummyShaderTextExporter` 标记，并被统一替换为 Opaque Shader：

- `RenderType` 为 `Opaque`
- 没有透明队列
- 没有 Alpha Blend
- 没有关闭深度写入
- 没有读取 Sprite/UI 顶点颜色
- 丢失了不同 `textureType` 的 Alpha 计算

`GameAnimation` 会根据动画帧中的 `textureType` 为 `Image` 或 `SpriteRenderer` 指定 `Type0～Type7` 材质，因此占位 Shader 直接影响角色和特效。

`Assets/Shader/Game_Map.shader` 同样被替换为带有 `DummyShaderTextExporter` 标记的 Opaque Shader。战斗地图静态层通过 `Prefab/Map` 和 `Map.mat` 使用该 Shader。第 0 幕的 `map00-2.png` 约 55.75% 像素为透明，但 Opaque Shader 忽略 Alpha 混合，把透明像素的黑色 RGB 直接写入画面。

## 参考来源

从当前工程同级的迁移前工程恢复：

```text
../Sword/Man/Client/Assets/Shader/GameType0.shader
../Sword/Man/Client/Assets/Shader/GameType1.shader
../Sword/Man/Client/Assets/Shader/GameType2.shader
../Sword/Man/Client/Assets/Shader/GameType3.shader
../Sword/Man/Client/Assets/Shader/GameType4.shader
../Sword/Man/Client/Assets/Shader/GameType5.shader
../Sword/Man/Client/Assets/Shader/GameType6.shader
../Sword/Man/Client/Assets/Shader/GameType7.shader
../Sword/Man/Client/Assets/Shader/GameMap.shader
```

恢复时保留当前工程原有文件名和 `.meta` GUID，只替换 Shader 源码。除移除两处行尾空白外，逻辑与迁移前源码一致。

## 恢复内容

所有 Type Shader 恢复了：

- Transparent Queue 与 RenderType
- `Blend SrcAlpha OneMinusSrcAlpha`
- `ZWrite Off`
- `Lighting Off`
- Unity 顶点变换
- 顶点颜色和 Alpha 传递

各类型继续采用迁移前的差异化处理：

- Type0：使用纹理 Alpha，并处理顶点颜色和特殊增亮逻辑。
- Type1：使用纹理 Alpha与顶点 Alpha。
- Type2～Type6：从纹理 RGB 最大值生成 Alpha，以兼容无 Alpha 的黑底光效/动画纹理。
- Type7：保留原工程的特殊 Alpha 处理和纯黑透明逻辑。

地图 Shader 恢复了：

- Transparent Queue 与 RenderType
- `Blend SrcAlpha OneMinusSrcAlpha`
- `ZWrite Off`
- `Lighting Off`
- 使用贴图原始 Alpha 合成静态地图叠加层

没有修改任何 PNG、贴图 `.meta`、材质、Prefab、场景或业务代码。

## 验证结果

### 场景验证

- 四个主场景全部加载成功。
- `scenes=4`
- `missingScripts=0`
- 无 Shader 编译错误。

动画 Shader 验证日志：`Logs/shader-restoration-scene-validation.log`（本地文件，不提交）。

地图 Shader 本轮验证日志：`Logs/map-shader-restoration-scene-validation.log`（本地文件，不提交）。

### macOS Development Build

- Build result：Succeeded
- Errors：0
- Warnings：0
- 地图 Shader 本轮构建大小：6,282,218,552 bytes

动画 Shader 验证日志：`Logs/shader-restoration-macos-build.log`（本地文件，不提交）。

地图 Shader 本轮构建日志：`Logs/map-shader-restoration-macos-build.log`（本地文件，不提交）。

### 运行画面

从标题菜单进入“战阵重现”并观察实际战斗场景：

- 角色周围的矩形黑底已消失。
- 场景光晕能够与地图背景正常叠加。
- 未再出现原截图中的大面积黑色遮挡。
- Player 日志未发现 Shader 错误、托管异常或崩溃。

本次动画验证覆盖启动、标题菜单和一个战斗场景画面，不代表所有 Type 在全部技能、剧情动画和 UI 中都已逐项验证。地图 Shader 已通过四个主场景加载和 macOS Development Build；第 0 幕末尾 `map00-2` 森林叠加层仍需人工运行复测。

后续遇到特定动画或地图层颜色、透明度异常时，应先记录关卡、图层、动画名、`textureType` 和截图，再对照迁移前工程处理；不要批量修改原始贴图。
