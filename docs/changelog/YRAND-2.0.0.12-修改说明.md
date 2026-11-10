# YRAND v2.0.0.12 修改说明 —— 麦克风声浪加成入设置页

> 基线：v2.0.0.11（Fluent 图标标识）。本版把麦克风声浪加成从「只能改 settings.json」升级为设置页正式选项，并新增加速率模式（随机/固定）。

## 一、用户可见的新选项（设置页「麦克风声浪加成」卡片）

卡片图标沿用 Fluent `mic_sparkle`（U+EB90），位于「两轮停顿」与「发送 ClassIsland 提醒」之间：

| 控件 | 绑定设置项 | 说明 |
|---|---|---|
| **开关**（卡片 Footer） | `MicBoostEnabled` | 总开关，默认关 |
| **加速率模式** ToggleSwitch | `MicBonusRandomized`（新） | 开 = **随声浪（随机）**：加成随教室音量实时爬升，0 ~ 最大加速率；关 = **固定**：音量一过阈值立即给固定加成 |
| **固定加速率** 滑块 0–100%（5% 步进） | `MicFixedBonus`（新，默认 25%） | 固定模式下生效 |
| **声浪最大加速率** 滑块 0–100%（5% 步进） | `MicMaxBonus`（原有，默认 45%） | 随机模式下生效 |

滑块旁实时显示 `+25%` 样式的数值；所有改动**即改即存**（写回 settings.json 并触发 INPC 刷新）。加成区间阈值（-35 dBFS 起加 / -12 dBFS 加满）保持 settings.json 高级配置，卡片描述中有说明。

## 二、逻辑变化（`YrandHostService.EffectiveContinueChance`）

```
随声浪（随机，默认）： p = 基础滑动概率 + MicMaxBonus × clamp((dB − 阈值)/(饱和点 − 阈值), 0, 1)
固定模式：           若 dB ≥ 阈值：p = 基础滑动概率 + MicFixedBonus（封顶 100%）
```

- 与 v2.0.0.10 起的公平设计一致：**该概率只影响表演时长（连不连滑、等多久），最终中签者由公平算法在点击瞬间锁定，与音量、模式、滑块完全无关**。
- 无麦克风 / 拒绝授权 / 非 Windows 依旧静默降级。

## 三、实现说明

- 设置页 UI 是 Avalonia XAML 的编译期生成码（无 .axaml 源），本次直接在构建方法里手工注入卡片代码：`SettingsExpander` + `ToggleSwitch`（Footer 总开关）+ 展开内容三行（模式开关、固定滑块、上限滑块），绑定用运行时 `new Binding(path, TwoWay){Source = context.RootObject}` 指到设置页新增的 6 个包装属性（`MicBoostEnabled` / `MicBonusRandomized` / `MicBonusModeText` / `MicFixedBonus(+Text)` / `MicMaxBonus(+Text)`），沿用现有 `Save()` 即改即存模式。
- `YrandSettings` 新增 `MicBonusRandomized`（默认 true）与 `MicFixedBonus`（默认 25）两个持久化字段，旧 settings.json 无需迁移。

## 四、验证结果

| 验证 | 结果 |
|---|---|
| `dotnet build -c Release` | **0 错误** |
| 反编译设置页 | 卡片、`mic_sparkle` 图标、「随声浪（随机）/固定」On/Off 文案、两个滑块与绑定全部落库 ✓ |
| 反编译 `EffectiveContinueChance` | 随机/固定双分支正确 ✓ |
| deps.json | 仍为插件自身 + NAudio 六件套，未新增依赖 ✓ |
| 包内容 | 12 个文件约 582 KB，manifest v2.0.0.12 ✓ |

## 五、装上后建议这样测

1. 设置页找到「麦克风声浪加成」卡片，打开总开关。
2. 模式开关拨到「固定」，固定加速率调到 +50%：对麦克风说话（音量过 -35 dBFS），悬浮窗 Tooltip 里「连滑概率」应立刻跳到「基础值 + 50%」。
3. 拨回「随声浪（随机）」：安静时概率≈基础值，逐渐大声概率线性爬升，到 -12 dBFS 加满上限滑块值。
4. 关闭总开关后概率恒为基础滑动概率；拔掉麦克风插件应无报错、点名照常。
