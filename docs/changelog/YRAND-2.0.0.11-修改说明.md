# YRAND v2.0.0.11 修改说明 —— Fluent 图标标识

> 基线：v2.0.0.10（完整重建源码工程，真实编译）。本版只加图标标识，**抽选/通知/麦克风/公平算法逻辑全部未改动**。

## 一、图标选型

从宿主内嵌的 Fluent 图标字体 `FluentSystemIcons-Resizable`（`avares://ClassIsland.Core/Assets/Fonts/#FluentSystemIcons-Resizable`）中解析出 cmap，共 5332 个字形、覆盖 U+E000–U+F4D3，选定：

| 用途 | 字形名 | 码位 |
|---|---|---|
| 线性（未选中态 / 提醒列表） | `ic_fluent_lottery_20_regular` | **U+EAFC** |
| 实心（选中态 / 通知条标识） | `ic_fluent_lottery_20_filled` | **U+EAFB** |

- 语义是「抽签 / 随机抽取」，与 YRAND 课堂随机点名高度贴合，比通用的人头/骰子图标更有辨识度。
- 码位已从字体内逐一验证存在，字体由 ClassIsland 宿主提供，**不新增任何依赖、不随包携带字体文件**。
- 预览见 `YRAND-fluent-icon-preview.png`（左：regular，右：filled）。

## 二、四处落点

| # | 位置 | 改动 | 用户可见效果 |
|---|---|---|---|
| 1 | 设置页注册特性 `SettingsPageInfo` | 由 3 参数无图标版改为 5 参数版：`("ima.yrand", "YRAND", "\uEAFC", "\uEAFB", External)` | 设置窗口左侧「YRAND」导航项显示抽签图标；选中该项时切换为实心版 |
| 2 | 提醒提供方特性 `NotificationProviderInfo` | 第 3 参数 `IconGlyph` 由空串 `""` 改为 `"\uEAFC"` | 【应用设置 → 提醒】的 YRAND 提供方显示抽签图标 |
| 3 | 主界面通知控件 `YrandNotificationControl` | `StackPanel` 首位插入 `new FluentIcon("\uEAFB")`（字号 24、透明度 0.9、右间距 2） | 每次「抽选结果」提醒条最左侧出现实心抽签标识，原有的 `Prefix ｜ Name` 绑定与顺序不变 |
| 4 | `manifest.yml` | 新增 `icon: icon.png` | 插件管理/市场页显示 256×256 彩色图标 |

> 关于第 4 点：`ClassIsland.Core.Models.Plugin.PluginManifest.Icon` 的默认值本身就是 `icon.png`，所以这是**显式规范化声明**，不是修 bug；原包里的 `icon.png` 本来就生效。

## 三、顺手修正的两处历史问题

1. **原版特性图标位被误填中文描述**：v2.0.0.8 原 DLL 中 `SettingsPageInfo` 的第 4 参数（实为 `SelectedIconGlyph`）是一段 75 字节中文「课堂随机点名：名单、滑动抽取、自动滑动与反作弊设置」，会被当作图标字形交给 `FluentIcon` 渲染。本版已替换为真实的 `U+EAFB`。
2. **设置页 id 恢复**：上一版重建工程时因反编译器丢失特性参数，简化成了 `"yrand"`；本版恢复为 `"ima.yrand"`，与 `manifest.yml` 的 `id` 及线上 2.0.0.8 一致。

## 四、验证结果（沙箱真实执行）

| 验证 | 结果 |
|---|---|
| `dotnet build -c Release` | **0 错误**（仅反编译生成码自带 1 条无害 CS0219 警告） |
| Cecil 读新 DLL 特性 blob | `SettingsPageInfo` 参数 3/4 = `EE-AB-BC` / `EE-AB-BB`（即 U+EAFC / U+EAFB）；`NotificationProviderInfo` 参数 3 = `EE-AB-BC` ✓ |
| ILSpy 反编译通知控件 | `StackPanel` 首子元素确为 `new FluentIcon("\ueafb")`，后面依次是 Prefix、分隔线、Name ✓ |
| 依赖检查 | `deps.json` 仍只有插件自身 + NAudio 六件套，未新增依赖 ✓ |
| 包内容 | 12 个文件，约 593 KB；`runtimeconfig.json` 为 net8.0；`manifest.yml` 版本 2.0.0.11 ✓ |

## 五、装上后请这样看

1. 打开 ClassIsland 设置窗口 → 左侧「YRAND」项应出现抽签图标，点中后变实心。
2. 【设置 → 提醒】→ 提供方列表里的 YRAND 应带抽签图标。
3. 点一次圆钮完成抽选 → 主界面「抽选结果」提醒条最左侧应出现实心抽签图标。
4. 插件管理页应显示彩色 256×256 图标。

若某处显示成空白或方块，说明宿主字体版本不含该码位，把现象发我即可换成 `ic_fluent_person_question_mark`（U+ED31/ED30）等备选码位，替换成本很低。

## 六、本版未改动

- 悬浮窗 40×40 圆钮：它要显示滚动的名字，塞图标会挤压文字，故保持原样。
- 设置页内部标题栏：设置页 UI 是编译期 XAML 生成码，本期未注入图标，可排下一版。
- 抽选状态机、通知时机（过程结束才通知）、麦克风分贝加成、债务校正公平算法：全部沿用 v2.0.0.10，未动一行。
