# Swordsman

《天地劫：神魔至尊传》2026 macOS 重制版 Unity 工程。

## 开发环境

- Unity：`2021.3.16f1c1`
- 主要目标平台：macOS
- UI：Unity UI (`com.unity.ugui`)
- 项目入口目录：仓库根目录

建议使用与项目记录完全一致的 Unity 编辑器版本，避免资源重新序列化或导入结果发生变化。

## 打开项目

1. 在 Unity Hub 中选择 **Open / Add project from disk**。
2. 选择本仓库根目录 `Swordsman`，不要选择 `Assets` 子目录。
3. 首次打开时等待 Unity 完成资源导入和脚本编译。
4. 打开 `Assets/Scenes/level0.unity`，从该场景进入 Play Mode。

项目包含以下主场景：

1. `Assets/Scenes/level0.unity`
2. `Assets/Scenes/level1.unity`
3. `Assets/Scenes/level2.unity`
4. `Assets/Scenes/level3.unity`

## Debug 构建

在 Unity 的 **File > Build Settings** 中选择 macOS，并按上述顺序加入四个主场景。开发调试构建应启用：

- Development Build
- Script Debugging（需要脚本调试时）

构建产物统一输出到 `Builds/`。该目录已被 Git 忽略，不应提交到仓库。

## 目录结构

```text
Swordsman/
├── Assets/            # 游戏代码、场景、资源、插件和编辑器工具
├── Packages/          # Unity Package Manager 配置及锁定版本
├── ProjectSettings/   # Unity 项目设置
├── .gitignore         # Unity 缓存、构建产物和本地配置忽略规则
├── LICENSE
└── README.md
```

主要资源目录：

- `Assets/Scenes/`：主场景
- `Assets/Scripts/`：运行时代码及项目编辑器脚本
- `Assets/Resources/`：运行时加载资源
- `Assets/Plugins/`：平台及第三方插件
- `Assets/Shader/`：Shader 资源
- `Assets/Sprite/`、`Assets/Texture2D/`：图像资源

## Git 约定

以下内容由本地生成，不纳入版本控制：

- `Library/`
- `Temp/`
- `Obj/`
- `Build/`、`Builds/`
- `Logs/`
- `UserSettings/`
- IDE 自动生成的工程文件和用户配置

提交资源时必须同时提交对应的 `.meta` 文件。不要提交本地缓存、构建产物、日志、签名文件、证书或其他敏感信息。

仓库包含较多图像、音频和视频资源。新增大文件前应先确认仓库策略，不要直接提交超过远端限制的单文件。

## 当前说明

- Unity `EditorBuildSettings` 当前未预置场景列表，执行手动构建时需按本文顺序添加主场景。
- `companyName`、应用标识符和发布签名尚未作为正式发布配置确定。
- `LICENSE` 文件不替代第三方代码、游戏内容、商标及素材各自适用的授权或权利声明。
