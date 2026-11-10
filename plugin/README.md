# YRAND（ClassIsland.YRAND）

适用于 **ClassIsland 2.x（Avalonia）** 的课堂随机点名插件。项目结构与写法参考 [Gordonynh/RandomPicker](https://github.com/Gordonynh/RandomPicker)，滑动点名的交互参考 ICC-CE（InkCanvasForClass 社区版，1.7.18015）内置随机点名的「名字滚动 → 停止 → 抽出」玩法。

> 本插件以 net8.0 + ClassIsland.PluginSdk 2.0.0.2 编译，可被 ClassIsland 2.0（net8.0 宿主）与 2.1+（net10.0 宿主）加载——net8.0 程序集对更高版本宿主向前兼容。

## 功能特性

- **点名悬浮窗**：置顶小窗，点「开始点名」抽取；按住空白处拖动位置，位置自动记住；不抢焦点、不进任务栏，并有置顶保持。
- **滑动抽取**：名字快速滚动，随时间逐渐放缓，时间到减速定格——谁也猜不到这轮转多久。
- **随机滑动时长**：可开启「每轮在最小~最大之间随机取时长」，也可关闭改用固定时长。
- **抽出选项后继续滑动到下一人（连抽）**：抽出一人后，按「滑动概率」掷骰子决定要不要继续滑到下一位；「滑动连续最高次数」到数强制停下。连续滑动期间随时可点「停止」认怂。该选项的备注为——**你的心脏有多大**。
- **大窗口**：抽中后在屏幕中央弹出大字卡片，停留时长可调，自动淡出；连抽时复用同一个窗口，不会叠出一摞。可在设置页开关。
- **ClassIsland 提醒（通知）**：内置提醒提供方（显示在【应用设置 → 提醒 → YRAND】）。**整套抽选只注册一条过程通知**（v2.0.0.15 修复：快速滚动时不再重复触发弹窗）——过程开始注册一次，滚动换名与中间定格均为原地属性更新（状态统一「抽取中」、静默不播报、不消耗名额），通知意外关闭也不补弹；过程结束才关闭它并新开「抽选结果」提醒播报最终锁定结果。
- **反作弊：同名合并**：名单内相同名字自动合并为一个——往名单里把同一个名字抄几遍是刷不出概率的。
- **本轮内不重复**：抽到的人本轮不再出现，抽完一轮自动开新一轮；关闭则是纯随机（只回避上一次同一个人）。
- **公平算法 v2（v2.0.0.10）**：抛弃「每格现抽」，改为**债务校正 + 冷落兜底**——每人维护债务 `due = t/N − 被抽次数`，用 Gumbel 加权抽样；任何人连续 `⌈γ·N⌉` 次未被抽中则强制进入必抽池。实测 N=10 抽 1000 次每人次数与均分偏差 ≤1。**整个抽选过程只消耗 1 个名额**：点击瞬间锁定唯一结果，滚动/定格/连滑全部是零消耗表演，最终中签与滑动概率、连滑次数、点击时机完全无关。 **v2.0.0.13 修复**：中间定格改为显示同源取样的过渡名（避开锁定名防剧透），恢复两段式连抽的可见节奏——每一段都有定格与停顿，最终结果在最后一段揭晓。
- **可验证随机**：每次抽选开局生成种子 S 并公示指纹 SHA256(S) 前 8 位；结果公布后种子写入 `settings.json` 的 `LastSeedHex`，任何人可用本插件同算法复算，自证没有针对谁。
- **麦克风声浪加成（v2.0.0.10，v2.0.0.12 入设置页）**：开启「麦克风声浪加成」后，插件实时采集麦克风响度（dBFS）。设置页「麦克风声浪加成」卡片可配置：**总开关**；**加速率模式**——「随声浪（随机）」= 加成随教室音量实时爬升（-35 dBFS 起加、-12 dBFS 加满），「固定」= 音量一过阈值就给固定加成；**固定加速率**与**声浪最大加速率**两个滑块（0~100%，5% 步进）。声浪越大滚动也越快；无麦克风/拒绝授权时自动静默降级，不影响点名。阈值（-35/-12 dBFS）仍走 settings.json。
- **Fluent 图标标识（v2.0.0.11）**：设置窗口左侧导航、【设置 → 提醒】提供方列表、主界面「抽选结果」提醒条统一使用 ClassIsland 内置 Fluent System Icons 的「抽签 lottery」图标（`\uEAFC` 线性 / `\uEAFB` 实心，分别对应导航未选中/选中两态）；同时把原设置页特性里误填进图标位的中文描述清掉。插件清单 `manifest.yml` 显式声明 `icon: icon.png`（ClassIsland 默认值），插件管理页显示 256×256 彩色图标。图标来自宿主自带字体 `FluentSystemIcons-Resizable`，**不新增任何依赖或资源文件**。
- **公平账本**：`PickedCounts`/`LastPickedAt`/`TotalDraws` 随 settings.json 持久化，名单增删时新成员按平均水位入场、老成员账本继承。

## 项目结构

```
YRAND/                                   # 本目录即 ClassIsland 的插件目录（data/Plugins/YRAND）
├── ClassIsland.YRAND.csproj             # net8.0 + Avalonia 11.3 + ClassIsland.PluginSdk 2.0.0.2
├── Directory.Build.props                # 编译产物统一放 build\，避免 bin/obj 混进插件目录
├── NuGet.config                         # NuGet 源（含微软中国 CDN nuget.azure.cn，国内还原更快）
├── manifest.yml                         # 插件清单（id: ima.yrand，apiVersion 2.0.0.0）
├── icon.png                             # 插件图标（ClassIsland 按约定读取 manifest 旁的 icon.png）
├── YrandPlugin.cs                       # 插件入口（PluginBase + AssemblyLoadContext 挂钩）
├── Models/
│   └── YrandSettings.cs                 # 设置 POCO + JSON 读写
├── Services/
│   ├── YrandRosterService.cs            # 名单解析（去空行/#注释/同名合并）与抽取
│   ├── YrandHostService.cs              # 插件主体：悬浮窗管理、抽取、连抽判定、提醒发送
│   └── YrandNotificationProvider.cs     # ClassIsland 提醒提供方
└── Views/
    ├── YrandWindow.cs                   # 悬浮窗（滚动状态机 / 拖动 / 位置记忆）
    ├── TopmostKeeper.cs                 # 置顶保持
    └── SettingsPages/
        └── YrandSettingsPage.axaml(.cs) # 设置页（名单 / 滑动 / 连抽 / 反作弊）
```

编译产物在 `build/bin/Release/net8.0/`，打包版（.cipx）在 `build/dist/`；编译后 `SyncPluginPayload` 目标会自动把运行所需文件同步到本插件根目录。

## 环境要求

- Windows / Linux，.NET 8 SDK 及以上
- ClassIsland 2.0 及以上（Avalonia 版；2.1.x 宿主同样适用）

## 编译

```bash
cd ClassIsland.YRAND
dotnet build -c Release
```

编译产物位于 `bin/Release/net8.0/`。插件依赖（Avalonia、FluentAvalonia、ClassIsland.Core）运行时全部使用宿主已加载的那份，不会打进插件包。

## 安装到 ClassIsland

方式一：**直接使用编译产物**（`bin/Release/net8.0/`）：

1. 将目录中的**全部文件**复制为一个 zip 包（必须包含 `ClassIsland.YRAND.dll`、`ClassIsland.YRAND.deps.json`、`ClassIsland.YRAND.runtimeconfig.json`、`manifest.yml`），把扩展名改为 `.cipx`；
2. 打开 ClassIsland →【应用设置】→【插件】→「安装插件」，选择该 `.cipx` 文件；或将解压后的文件夹直接放到 ClassIsland 数据目录的 `Plugins` 文件夹；
3. 重启 ClassIsland 完成加载。

> ⚠️ `.deps.json` 必须随插件一起分发：宿主的 `PluginLoadContext` 用 `AssemblyDependencyResolver` 解析插件，缺少该文件会报
> `Dependency resolution failed ... Failed to locate managed application`（本插件在 2.0.0.0 版踩过这个坑，2.0.0.1 起已修复）。
> `runtimes/` 目录可以不带——里面的 SkiaSharp native 库由宿主提供。

方式二：源码编译后按同样方式打包。

## 使用说明

1. **配置名单**：ClassIsland →【应用设置】→ 随机点名 页面，在「学生名单」中每行填一个名字。空行和以 `#` 开头的行会被忽略；页面顶部会显示「共 X 人可参与抽取」。
2. **打开悬浮窗**：设置页点「打开悬浮窗」，或开启「启动自动显示」；悬浮窗按住空白处可拖动，位置自动记住。
3. **开始点名**：点「开始点名」，名字开始滑动，到时自动减速定格。
4. **连抽**：打开悬浮窗右下角的「连抽」开关，或在设置页开启「抽出选项后继续滑动到下一人」（你的心脏有多大）。每次抽出后按「滑动概率」决定是否继续，连续次数到达「滑动连续最高次数」就停。
5. **结果展示**：抽中后按设置弹出屏幕中央大窗口、推送 ClassIsland 提醒，两者可独立开关。

## 设置项一览

| 设置 | 说明 |
| ---- | ---- |
| 学生名单 | 每行一个名字，空行与 `#` 注释忽略，即改即生效 |
| 同名合并（反作弊） | 名单内相同名字自动合并为一个 |
| 本轮内不重复 | 抽过的本轮不再出现，抽完自动开新一轮 |
| 是否随机滑动时长 | 开=每轮时长随机（最小~最大）；关=固定时长 |
| 抽出选项后继续滑动到下一人 | 连抽总开关（备注：你的心脏有多大） |
| 滑动概率 | 每次抽出后继续滑动的概率（0~100%） |
| 滑动连续最高次数 | 最多连续滑动几轮（1~20） |
| 抽中后显示大窗口 | 屏幕中央大字卡片，可设停留时长 |
| 发送 ClassIsland 提醒 | 抽中结果推送到主界面提醒条 |
| 语音播报 | 抽中定格后按播报词模板念出结果，`{name}` 占位，滚动阶段不出声 |
| 悬浮窗启动自动显示 | ClassIsland 启动后自动打开悬浮窗 |

## 常见问题

- **加载失败：`Dependency resolution failed ... Failed to locate managed application`**：插件目录缺 `.deps.json` 或整个编译产物没就位（2.0.0.0/2.0.0.2 两次踩坑）。插件目录里必须同时有 `ClassIsland.YRAND.dll` + `ClassIsland.YRAND.deps.json` + `manifest.yml`。项目已内置 `SyncPluginPayload` 编译目标：编译后自动把运行所需文件同步到插件目录；在别的机器上编译时用 `dotnet build -c Release -p:ClassIslandPluginDir="<宿主插件目录绝对路径>"` 一键部署。
- **编译机要求**：需要 .NET 8 SDK（只装了 .NET 运行时是编不了的，`dotnet --list-sdks` 为空说明没装 SDK）。
- **日志刷 `AsyncImageLoader Failed to resolve image ... icon.png`**：插件目录缺 `icon.png`（ClassIsland 按约定读 manifest 旁边的 icon.png）。2.0.0.3 起随包分发，编译后自动同步。
- **日志出现 `[Control] PlatformImpl is null, couldn't handle input (PopupRoot)`**：Avalonia 内部的无害警告（弹层在窗口关闭瞬间收到输入事件），与插件功能无关，可忽略。
- **装上后设置页白屏/空白**：本插件通过 `AssemblyLoadContext.Default.Resolving` 挂了程序集解析回调，若你自行裁剪了入口代码会无法加载 axaml，请保留 `EnsureAssemblyResolvable()`。
- **提醒不弹**：检查 ClassIsland 的总提醒开关，以及【应用设置】→【提醒】→【随机点名】是否被单独关闭。
- **悬浮窗不见了**：设置页点「打开悬浮窗」即可找回；位置若被拖到屏幕外，会在窗口打开时自动夹回屏幕内。
- **NuGet 还原慢**：项目已附带 `NuGet.config`，优先走微软中国 CDN（nuget.azure.cn）。

## 开发说明（API 依据）

- 插件入口：`PluginBase` + `Initialize(HostBuilderContext, IServiceCollection)`；
- 提醒：`NotificationProviderBase` + `[NotificationProviderInfo]`，经 `services.AddNotificationProvider<T>()` 注册，`ShowNotification(new NotificationRequest { MaskContent = ..., OverlayContent = ... })` 发送（`INotificationHostService.ShowNotification` 为 internal，插件不可直达）；
- 设置页：`SettingsPageBase` + `[SettingsPageInfo]`，经 `services.AddSettingsPage<T>()` 注册；
- 服务：`AddHostedService` 注册插件主体，设置页经 `IAppHost.Host.Services` 取回；
- 插件运行于独立 `PluginLoadContext`：必须生成 `.deps.json`（`EnableDynamicLoading`），且 Avalonia 资源（axaml）需挂接默认 `AssemblyLoadContext.Resolving`。
