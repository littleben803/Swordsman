# macOS LZFSE 高清资源分块读取验证

## 2026-11-17：正式macOS构建接入

已把系统LZFSE分块流接入 `SwordsmanHd4xSpriteResolver` 的macOS Bundle加载路径。Player从包内 `SWLZF001` 容器按需读取，16 MiB解码缓存、4路预取及1 MiB Unity读取缓冲；Editor原始UnityFS Bundle仍可直接读取。流随Bundle保持存活；检测失败的Bundle会释放流。构建入口仍为唯一 `SwordsmanMacBuild.BuildDevelopmentPlayerChapterTestsAllChapters`：共享像素存储、Splash后处理及LZ4HC标准化完成后，对包内高清Bundle编码LZFSE并全量回读核验，再替换该包内文件、重新签名。`data.unity3d`仍使用LZ4HC。脚本只允许显式`--production`写入唯一应用的固定Bundle位置。

本轮输入LZ4HC高清Bundle为3,300,624,150字节，最终容器2,936,657,162字节，减少363,966,988字节（11.027%）。完整解码9,498,636,360字节的SHA256为 `d6886fd33bb10e457a1d6e3b3d181c76d6985b1fb1d200ed1c3af48e41c1304a`；包内文件SHA256与构建报告一致。最终应用磁盘占用以 `du -sk` 为4,233,812 KiB（约4.34 GB）。唯一Development构建Succeeded，0错误、9条既有警告；构建入口计时287.57秒，含HD Bundle生成及多段后处理。`codesign --verify --deep --strict`通过。

实际arm64 Player运行 `-swordsman-static-mvp-check` 通过1819项高清资源及地图/图集几何检查；章节测试台 `-swordsman-chapter-tests-check` 扫过36幕，战斗检查点、普通存档和隔离路径检查通过。通用Player含arm64及x86_64切片，Rosetta x86_64 Player也通过同一高清检查。实际Intel Mac、交互画质与章节操作仍需用户真机验收；自动资源几何检查不替代人工画面判断。

隔离工程 `Builds/CompressionValidation/` 下约12 GiB实验Bundle、候选容器与最小工程已在验收后删除，日志保存在 `Logs/lzfse-validation-performance-20261116/` 和 `Logs/lzfse-production-20261117/`。音视频、原版角色动画/播放表、48张混合图集、Unity版本及工程配置未因本轮改变。完整性保护仍为构建时全量SHA，生产读取器没有逐块校验；若未来需要抵御包内单块损坏，应专项补充校验/失败处理。

## 2026-11-17：隔离读取性能优化复测（正式接入前记录）

本阶段完成。允许修改范围仅为已有Editor验证器及本文；未修改离线编码脚本、生产解析器或任何正式默认入口。没有升级Unity、构建Player或裁剪Intel架构，音视频、原版动画/播放表、48张混合图集未动。下方2026-11-16内容保留为首轮历史，不代表本轮性能结论。

### 最新输入与公平基线

锁定本轮开始时最新高清Bundle快照，源SHA256为 `c2c1ce22a7fa529116dc29e7a09cba556569d83f7100fe34b9a176225029abfe`，3,460,090,358字节；快照前后核对缓存记录及SHA。其他任务正在主工程构建，因此全部测试使用隔离工程与冻结副本，不混入后续版本。

最新源包尚含重复像素存储，故只在实验副本上复用既有 `share_texture_storage.py` 与正式构建入口的LZ4HC压缩实现，得到和正式方案同口径的基线。共享存储及LZ4HC共减少159,466,208字节，这部分不计入LZFSE收益。正式脚本、源Bundle、应用均未被本任务改写。

| 最新数据 | 字节 |
|---|---:|
| 已共享存储的LZ4HC基线 | 3,300,624,150 |
| LZFSE实验容器（含全部索引与前缀） | 2,936,657,162 |
| 额外减少 | **363,966,988（363.97 MB，11.027%）** |

基线SHA256 `dbd3d7dfff723aa5fdb142e05861f0288e927deea22480ec4fc31dfae437041b`；容器SHA256 `a2a007699a678c0f64a6e101ac13906460280b49d04eed99b11460914d2741dc`。72,774块中70,766块LZFSE、1,984块回退原LZ4、24块原始数据。完整9,498,636,360字节解码SHA256为 `d6886fd33bb10e457a1d6e3b3d181c76d6985b1fb1d200ed1c3af48e41c1304a`，与LZ4HC基线一致。编码调用57.32秒；离线编码、完整回读和初始随机验证89.40秒。

### 优化方式与测量结果

只在显式实验参数下启用：原生 `pread` 读取、固定地址并复用输入/输出缓冲、每个解码工作线程复用系统scratch、单次Read内最多4 MiB的有界并行分块解码。Read/Seek串行化，所有分块完成后才返回；不做后台无界预读。缓存仍为16 MiB，不新增依赖。选择4路解码、1 MiB Unity托管读取缓冲；保留旧64 KiB串行实现作为同包对照。默认参数仍是旧实验设置，没有接入产品路径。

同机同Unity 2021.3.16f1c1，4种配置各3个全新隔离Editor进程，交错执行；OS文件缓存温热，后台任务未被中断。32组资源加载包含3个最大纹理；渲染读回时间另记，不计入加载时间。以下不是冷启动、章节耗时或正式Player结果。

| 指标，中位数 | LZ4HC基线 | 上版LZFSE，同一最新数据 | 优化LZFSE |
|---|---:|---:|---:|
| 32组资源加载 | 507.83 ms | 1,633.37 ms | **601.67 ms** |
| 加载范围 | 492.06–516.66 ms | 1,625.50–1,675.41 ms | 592.71–679.37 ms |
| Stream初始化 | 原生内部 | 24.99 ms | 26.33 ms |
| Bundle打开API | 11.69 ms | 13.10 ms | 20.26 ms |
| 初始化+打开+加载，各轮相加后中位数 | 519.51 ms | 见JSON | 649.23 ms |
| 进程峰值RSS | 1,737.64 MiB | 1,743.17 MiB | **1,751.19 MiB** |

资源加载相对上版缩短63.16%（约2.71倍速度），相对LZ4HC仍多93.84 ms / 18.48%。计入Stream初始化和打开后，整段多129.71 ms / 24.97%；不能仅用打开API时间代表总打开成本。进程峰值RSS中位数增量13.55 MiB；getrusage与 `/usr/bin/time -l` 复核，包含相同GPU读回工作负载，不是完整游戏内存上限。

4 MiB缓冲三轮加载中位数625.81 ms，解码947.94 MB；1 MiB方案601.67 ms、解码854.59 MB、读取压缩数据242.68 MB，相比上版解码816.86 MB有约4.62%的读取放大。4 MiB未获得稳定收益，故选择1 MiB。性能改善来自组合优化；阶段筛选还测了单路和2路，不能把全部收益归因于增加线程。

### 正确性、Intel及范围核验

- 最新数据完整解码SHA一致；初始1,158组随机读取通过。
- 新增81组1–20 MiB独立LZ4基线oracle读取，覆盖跨块、4 GiB边界、缓存淘汰及尾部；合计1,239组，C#原生1/2/4路均通过。
- 12轮加载对照：1,819资产名称，32组Texture2D、36个Sprite几何、8张原尺寸GPU读回哈希逐项相同。不是全游戏场景验收。
- Rosetta x86_64系统解码器完整9.499 GB及1,158组读取通过，另通过扩展1,239组读取；未移除Intel支持。但现有隔离Unity运行于ARM64，**Intel真机Unity/Mono、GPU与性能尚未验证**，不能以Rosetta系统解码代替。
- 本阶段重新散列194,629个受保护文件，全部与阶段起点相同；已有大量Git修改来自先前任务，未覆盖或回滚；状态列表中临时StreamingAssets条目的消失对应并行标题构建的既有清理流程（`SwordsmanHd4xBuild.cs`），不是本任务删除。实验最小工程源码与主工程验证器逐字节一致。

当前建议：性能差距已明显缩小，约364 MB收益保持，可讨论下一阶段实际章节加载和Intel真机验证；本阶段不进入默认路径。生产接入仍需Stream与Bundle同寿命、异步取消/卸载压力、损坏数据与失败处理；当前没有生产级逐块完整性机制。

### 本轮命令与证据

完整参数、轮次及环境位于 `Logs/lzfse-validation-performance-20261116/`（目录日期为启动日，完成日为11月17日）：

- `python3 -B scripts/share_texture_storage.py --input <冻结副本> --output <实验基线> --report <sharing.json>`；隔离Editor执行 `LzfseBaselineNormalizer.Run` 复用原HC实现。
- `python3 -B scripts/validate_lzfse_bundle.py --input <实验基线> --output <实验容器> --report <container.json>`。
- `python3 -B Logs/lzfse-validation-performance-20261116/run-confirmation.py`：12轮编译/资源加载/渲染对照；`run-profiles.py` 为前置筛选。
- `extend-random.py` 生成独立LZ4 oracle扩展用例；`run-wide-random.py` 执行1/2/4路C#随机读取。
- `arch -x86_64 /usr/bin/python3 -B scripts/validate_lzfse_bundle.py ... --verify-only --manifest <container.json>`；`verify-wide-intel.py` 补扩展读取。

汇总为 `delivery.json`、`benchmark-summary.json`；输入锁定 `input-lock.json`；全量一致性 `container.json`；随机读取 `wide-random-*.json`；Intel `intel-rosetta.json`、`intel-wide-random.json`；保护核验 `protection-result.json`。实验大文件均在 `Builds/CompressionValidation/` 并被Git忽略。本阶段没有新增根工程源码文件，仅修改验证器和本文；辅助实验驱动、向量与报告均在被忽略目录。隔离Editor脚本编译成功，**Debug/Development Player构建未运行，遵循本阶段禁止构建应用的边界**。

## 2026-11-16：首轮验证历史

日期：2026-11-16。状态：独立验证完成，未接入默认路径，未构建应用，未升级 Unity。Intel Mac 支持保持为必要约束，不裁剪 Intel 架构。

## 范围与输入版本

仅新增 `scripts/validate_lzfse_bundle.py`、`Assets/Editor/SwordsmanLzfseValidation.cs` 及 Unity 生成的对应 `.meta`、本记录。没有修改生产解析器、构建入口、音视频、原版动画/播放表或48张混合图集。

输入为本阶段开始时应用中的HD Bundle，SHA256 `2fad49c238dd7fa1b2638064debe9828dd7787d8c28766036bf606729513c706`，3300791092字节。全部加载对照完成后再次确认输入仍为该SHA。本阶段另一个任务正在主工程执行标题不透明度构建；其标题导入缓存、源Bundle、构建缓存和临时ProjectSettings变化单列在保护报告，不覆盖、不回滚、不计入本阶段收益。该并行任务后续发布的新Bundle不自动继承本报告验收。

## 实验格式与正确性

外层 `SWLZF001` 容器包含固定索引、虚拟UnityFS前缀、128KiB分块数据。70765块使用系统LZFSE；当LZFSE更大时，1991块保留原LZ4、24个小块保留原始数据。不是全文件解压到内存，也不是对既有LZ4文件再套一层压缩。

读取器呈现一个未压缩UnityFS的可Seek视图，原有节点内数据和偏移保持，Unity用 `AssetBundle.LoadFromStream` 按需读取。C#流使用16MiB LRU解码缓存、复用压缩输入缓冲区及被淘汰的解码缓冲区；读/Seek和缓存操作加锁。显式Editor命令入口之外没有任何初始化钩子，不注册到运行时解析器。

全部9498648032字节原始解压数据SHA256一致：`24cfddf41ece8f50776b6d8a30ba8d535bc5fece891dbfa604a6003d5bbeb0bb`。每个LZFSE编码块先做字节级round-trip，再从落盘容器完整解码复核SHA。Python及Unity C#分别通过1158组读取，覆盖随机位置、跨块、超过4GiB偏移、零长度、文件尾/尾后读取、Begin/Current/End seek及拒绝负位置。

Unity读取到1819个资产名称；对照加载32组跨组资源（含3个最大纹理），32条纹理/Sprite记录、36个选中Sprite的几何结果一致。8张原生纹理尺寸的GPU读回哈希逐项相同。它是数据与渲染一致性验证，不是完整游戏画面或剧情验收。

## 实际压缩结果

| 项目 | 字节 |
|---|---:|
| 原HD Bundle | 3,300,791,092 |
| 实验容器 | 2,936,780,757 |
| 实际减少 | **364,010,335（364.01 MB，11.028%）** |
| 其中索引 | 2,328,960 |
| 虚拟UnityFS前缀 | 728,007 |

收益已包含所有容器、前缀和索引开销，没有替换应用中的原Bundle。累计系统编码调用58.34秒，完整离线编码/回读/随机验证合计91.86秒；不包含Unity基准测试。

## 加载耗时与内存

使用完全相同版本Unity `2021.3.16f1c1` 的隔离最小工程（`Builds/CompressionValidation/UnityProbe`），只依赖安装自带的AssetBundle/ImageConversion/JsonSerialize模块；没有新增第三方依赖。验证器源码与主工程新Editor工具逐字节一致。隔离工程用于避免主工程并行构建锁，不加载主工程业务场景。

同一Apple Silicon/32GiB机器，两种模式各3个全新Editor进程交错执行，文件缓存温热。用户游戏及另一个主工程构建仍在后台；未清空系统缓存、未终止它们。以下是受控资源工作负载的结果，不是冷启动、正式Player或单幕游戏耗时。GPU读回耗时单独记录，不计入资产加载列；RSS峰值包含这个相同读回工作负载。

| 指标 | 原LoadFromFile | LZFSE流 |
|---|---:|---:|
| AssetBundle加载API调用，中位数 | 11.93 ms | 12.62 ms |
| 读取器/索引初始化（另外3轮） | 原生内部，包含在上一项 | 16.91 ms |
| 32组资源加载，中位数 | 525.70 ms | 1,639.09 ms |
| 加载范围 | 503.65–551.30 ms | 1,633.24–1,846.81 ms |
| 进程峰值RSS，中位数 | 1,737.42 MiB | 1,749.52 MiB |
| 解码缓存上限/实测峰值 | 原生实现 | 16 MiB / 16 MiB |

AssetBundle API列不包含实验Stream构造；读取器/索引初始化另测3轮，中位数16.91ms（16.60–17.94ms）。不能把12.62ms当作完整打开耗时。

本工作负载加载约3.12倍，多1113.40ms；RSS中位数增加12.09MiB。内存指标是Darwin进程最大RSS，使用getrusage并由 `/usr/bin/time -l` 复核，不代表整机内存或完整GPU预算。实验路径实际解码约817.5MB，没有在打开时物化整个9.5GB视图。

首版每块分配缓冲区额外产生明显GC/RSS压力；保留于 `initial-allocation/`。仅在实验读取器中复用缓冲区后重新运行随机检查及全部六轮对照，上表以复测为准。仍有系统LZFSE解码及托管Stream桥接代价，不能将16MiB缓存上限视作整个游戏的内存增量上限。

## Intel与未覆盖风险

通过 `arch -x86_64 /usr/bin/python3` 在Rosetta下使用x86_64系统libcompression，完整解码9.499GB并通过1158组读取，哈希相同，峰值RSS164507648字节。没有删除Intel切片或改变应用架构。此结果证明当前格式及系统解码器的x86_64字节正确性，不能替代Intel真机Unity/Mono、GPU及性能回归。

生产接入前仍需：Intel真机验收；实际章节加载/切换测量；异步并发加载、取消和卸载压力；Stream与Bundle完整同寿命管理；损坏输入注入、块级校验及失败处理。目前有离线整包SHA和全量验证，未实现生产级逐块完整性机制。更换输入Bundle版本必须重新生成并验收。

当前结论：无损存储与按需随机读取原型成立，空间收益实测约364MB；加载代价明显，暂不建议直接接入正式默认路径。下一阶段可继续在隔离入口优化性能并补Intel真机验证，保留现有LZ4HC正式包。

## 命令与证据

离线入口：`/usr/bin/python3 -B scripts/validate_lzfse_bundle.py --input <锁定HD包> --output Builds/CompressionValidation/swordsman-hd4x.swlzfse --report Logs/lzfse-validation-20261116/container.json`。

跨架构复验：相同脚本加 `--verify-only --manifest Logs/lzfse-validation-20261116/container.json`，用 `arch -x86_64 /usr/bin/python3` 执行。

Unity显式入口：`SwordsmanLzfseValidation.Run`，使用 `-swordsman-lzfse-mode random|baseline|candidate` 及报告/输入路径参数。完整顺序与参数保留于 `Logs/lzfse-validation-20261116/run-unity.py`；不调用Player构建入口。

总报告 `Logs/lzfse-validation-20261116/delivery.json`；容器 `container.json`；随机向量 `random-vectors.json`；三轮对照 `benchmark-summary.json`；x86_64验证 `intel-rosetta.json`；文件保护 `protection-result.json`。实验文件和最小工程位于 `Builds/CompressionValidation/`，全部不纳入Git。
