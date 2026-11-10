# YRAND

ClassIsland 课堂随机点名插件。一个圆形悬浮小钮，点一下开始抽选：名字在**同一条 ClassIsland 提醒上原地滚动换名**，时间到定下一位；再按「滑动概率」决定是否连滑并逐个定格；整套过程结束后才推送一次「抽选结果」提醒并语音播报。

> 当前版本 **v2.0.0.15**（2026-11-09）。基于 ClassIsland 2.0 / net8.0 + PluginSdk 2.0.0.2，兼容 2.1+ 的 net10 宿主。

## 功能

| 特性 | 说明 |
|---|---|
| **两段式抽选** | 匀速滚动（ease-out 减速）→ 定格 → 按滑动概率连滑 → 最终揭晓；中间定格静默、不播报、不消耗名额 |
| **公平算法 v2** | 债务校正 `due = t/N − c` + Gumbel 加权抽样 + 冷落滑窗兜底；实测 N=10 抽 1000 次每人偏差 ≤1 |
| **一次过程一个名额** | 点击瞬间由 256-bit 种子锁定唯一结果，滚动/连滑/音量/点击时机都改变不了中签者 |
| **可验证随机** | 开局公示指纹 `SHA256(S)[0..8]`，结束后公布种子 `S`（`settings.json: LastSeedHex`），可复算自证 |
| **麦克风声浪加成** | NAudio 实时 dBFS：随声浪线性爬升或固定加成，影响连滑概率与滚动速度；可在设置页配置开关/模式/数值，无设备静默降级 |
| **Fluent 图标** | 宿主内置 `FluentSystemIcons-Resizable` 的抽签图标（U+EAFC/EAFB），覆盖设置导航、提醒提供方、结果提醒条 |
| **通知时机** | 整套抽选只注册一条过程通知（原地换名），结束才新开「抽选结果」——滚动再快也不会重复弹窗 |

设置项、账本字段与高级参数见 [`plugin/README.md`](plugin/README.md)。

## 安装

ClassIsland → 插件管理 → 本地安装，选择 [`releases/`](releases/) 下最新的 `.cipx`（覆盖安装前建议先卸载旧版）。

各版本完整修改说明见 [`docs/changelog/`](docs/changelog/)，在线版本发布台：<https://ade236ccde4e195c4.app.workbuddy.host/?sharecode=UV5IFW4S3E>

## 从源码构建

需要 **.NET 8 SDK**（`dotnet --version` ≥ 8.0）。

```bash
dotnet restore src/ClassIsland.YRAND/ClassIsland.YRAND.csproj
dotnet build   src/ClassIsland.YRAND/ClassIsland.YRAND.csproj -c Release
python3 scripts/pack.py            # 产出 dist/YRAND-<version>.cipx
```

`pack.py` 会：收集编译产物（dll / deps.json / runtimeconfig.json / NAudio 六件套）、精简 `deps.json`（只保留插件自身与 NAudio，宿主提供的 Avalonia / ClassIsland / FluentAvalonia 不随包）、合并 [`plugin/`](plugin/) 下的 manifest / README / icon，打包为 `.cipx`（zip 格式）。

**依赖说明**：`ClassIsland.PluginSdk 2.0.0.2`、`Avalonia 11.3.6` 由 ClassIsland 宿主提供，编译期通过 NuGet 引用、不随包分发；`NAudio 2.2.1` 随包携带。设置页使用的 `SettingsExpander` 由 `ClassIsland.Core` 提供，无需额外引用 FluentAvalonia NuGet 包。

GitHub Actions 会在 push / PR 时自动执行构建并上传 `.cipx` 构建产物（见 [`.github/workflows/build.yml`](.github/workflows/build.yml)）。

## 仓库结构

```
src/ClassIsland.YRAND/   可编译的插件源码工程（Models / Services / Views / CompiledAvaloniaXaml）
plugin/                  插件包元数据：manifest.yml、README.md、icon.png
releases/                各版本 .cipx 发布包（v2.0.0.9 ~ v2.0.0.15）
docs/changelog/          逐版本修改说明
docs/algorithm/          点名算法示意图（3 张 SVG：流程 / 公平内核 / 麦克风曲线）
scripts/                 打包脚本、版本发布站生成脚本
```

## 源码来源说明

本仓库源码由 **v2.0.0.8 发布包反编译还原并重建为可编译工程**后持续迭代而来：`CompiledAvaloniaXaml/` 是 Avalonia XAML 编译期生成码（原设置页 XAML 已编译成 IL，此目录为等价生成码），`AvaloniaResources.bin` 为内嵌资源 blob。业务代码（`Models/` `Services/` `Views/`）在此基线上重写，新增 `FairDrawService`、`MicLevelService` 与通知编排逻辑。

## 版本历史

| 版本 | 主题 |
|---|---|
| v2.0.0.9 | 通知时机重写（IL 补丁） |
| v2.0.0.10 | 公平算法 v2 + 麦克风声浪加成（重建源码工程） |
| v2.0.0.11 | Fluent 抽签图标标识 |
| v2.0.0.12 | 麦克风加成入设置页 |
| v2.0.0.13 | 滚动效果回归 · 两段式修复 · 结果通知去指纹 |
| v2.0.0.14 | 过程状态统一「抽取中」 |
| **v2.0.0.15** | **修复快速滚动重复弹窗（过程通知只注册一次）** |

完整开发历程（15 轮对话、6 幕、算法示意图）见在线全记录页：<https://ade236ccde4e195c4.app.workbuddy.host/story/?sharecode=UV5IFW4S3E>

## 上传到 GitHub

本仓库已完成初始化与首次提交（分支 `main`）。推送前需准备：

1. 在 CodeBuddy 设置页的「连接器」处授权 **GitHub**（或自行配置本机 git 凭据）；
2. 创建远程仓库（网页新建，或 `gh repo create <owner>/<repo>`）；
3. 关联并推送：

```bash
git remote add origin https://github.com/<owner>/<repo>.git
git push -u origin main
```

`releases/` 下的 `.cipx` 已随仓库提交，也可另行作为 GitHub Releases 附件上传；打 `v2.0.0.15` 之类的 tag 可触发版本归档。CI 会自动构建并产出最新 `.cipx` 构建产物。

## 许可

[MIT](LICENSE)
