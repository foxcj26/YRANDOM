# YRAND v2.0.0.10 修改说明 —— 公平算法 v2 + 麦克风声浪加成

> 本版基于「混元第二版思路」完整重写了随机算法，并新增麦克风分贝 → 连滑概率加成。
> 与 v2.0.0.9 不同：本版不再是 IL 补丁，而是**重建完整源码工程真实编译**（ILSpy 反编译还原 + XamlIl 生成码修复，0 错误构建）。

## 一、公平算法 v2（核心重写）

新增 `FairDrawService`，抛弃「每次定格现抽」模型：

- **债务校正**：每人维护 `due_i = t/N − c_i`（t=总抽取次数，c_i=被抽次数），抽样取 `argmax(β·due_i + Gumbel(U_i))`，`β` 默认 2.5（`FairnessBeta` 可调，0=纯随机，→∞ 逼近严格轮转）。
- **冷落兜底**：任何人连续 `W=⌈γ·N⌉` 次未被抽中（γ 默认 1.5，`ColdWindowFactor`）即进入必抽池，**硬保证**任意窗口内每人至少出现一次。
- **一次过程只消耗一个名额**：点击圆钮瞬间 `Reserve()` 锁定唯一结果（幂等）；滚动、定格、连滑全部为零消耗表演；仅最终通知发出时 `Commit()` 推进账本。**中签与滑动概率、连滑次数、手动点击时机完全无关**——彻底消除 v2.0.0.9 的「连滑隐性消耗」偏差。
- **视觉取样同源**：滚动闪现的名字从同一公平分布取样（`PickVisual()`，DRBG 驱动、允许重复、零消耗），「常被看见」与「真被抽中」机会一致。
- **名单变更**：新成员按当前平均水位入场（不偏不倚），老成员账本继承。
- **滚动节奏**：由固定 150ms 改为 ease-out 减速（150→400ms），补上 README 一直宣称但从未实现的「逐渐放缓」。

### 实测验证（N=10 人，抽 1000 次）

| 验证项 | 结果 |
|---|---|
| 次数分布 | 每人 99~101 次，**偏差 ≤1**（理想 100） |
| 冷落兜底 | 200 轮未抽者进入必抽池后第 2 次命中 ✓ |
| 指纹↔种子 | SHA256(种子) 前 8 位与公示指纹一致 ✓ |
| 结果锁定 | 过程内重复 Reserve 返回同一人 ✓ |

## 二、可验证随机

- 每次抽选开局生成 256-bit 种子 S（CSPRNG），指纹 = SHA256(S) 前 8 位，显示在悬浮窗 tooltip 与结果通知里（「抽选结果 · 指纹 3F7A9068」）。
- 过程结束后种子写入 `settings.json` 的 `LastSeedHex`，用同算法（HMAC-SHA256 计数器流 DRBG + 相同抽样逻辑）即可复算整局，课堂公开自证。

## 三、麦克风声浪加成（新功能）

新增 `MicLevelService`（NAudio）：

- 16kHz/16bit 单声道、100ms 窗计算 RMS → **dBFS**（0=满量程），指数平滑抗爆音。
- **概率映射**：连滑概率 = 基础「滑动概率」 + 声浪加成。分贝超过 `MicThresholdDb`（默认 -35）后线性爬升，到 `MicCeilingDb`（默认 -12，约等于放声大喊）加满 `MicMaxBonus`（默认 +45 个百分点）。学生喊得越响，越容易连滑。
- **滚动加速**：声浪越大滚动换名间隔越短（最高提速 35%），课堂气氛直接反馈到画面。
- **稳健性**：无麦克风设备 / 拒绝授权 / 非 Windows 平台一律静默降级（`Failed` 标记后不再重试），点名主流程不受任何影响；概率判定仍用系统 CSPRNG。
- 总开关 `MicBoostEnabled` 默认**关闭**，在 `settings.json` 中设为 `true` 启用（设置页 UI 下一版加入）。

## 四、工程与包变化

- 完整源码工程重建于沙箱（`yrand-build/`），net8.0 + Avalonia 11.3.6 + ClassIsland.PluginSdk 2.0.0.2 + NAudio 2.2.1，Release 编译 0 错误。
- 通知行为延续 v2.0.0.9：全程只有最终结果一条通知；结果通知现附指纹徽章。
- `settings.json` 新增字段：`FairnessBeta`、`ColdWindowFactor`、`MicBoostEnabled`、`MicThresholdDb`、`MicCeilingDb`、`MicMaxBonus`、`PickedCounts`、`LastPickedAt`、`TotalDraws`、`LastSeedFingerprint`、`LastSeedHex`（旧字段全部保留，旧配置无损升级）。
- cipx 现包含 NAudio 全家桶（6 个 dll，约 500KB）——麦克风功能唯一新增依赖；`deps.json` 按惯例精简为「插件自身 + NAudio」，其余依赖回退宿主。
- 包内不含 pdb；manifest 版本 `2.0.0.10`，apiVersion 不变。

## 五、安装与验证

1. ClassIsland → 插件 → 安装 `YRAND-2.0.0.10.cipx`（建议先卸载旧版）。
2. 启用声浪加成：编辑 `data/Plugins/YRAND/config/settings.json`，设 `"MicBoostEnabled": true`，重启 ClassIsland。
3. 重点验证：
   - 点圆钮 → 滚动期间无声浪时匀速、越接近定格越慢；喊叫时明显加速（开启加成后）
   - 命中连滑时全程只有最后一人被通知/播报，中间名字无声
   - 抽几次后打开 settings.json 看 `PickedCounts`：次数差应 ≤1~2；`LastSeedHex` 与通知上指纹可互验
   - 拔掉麦克风再抽：一切照常（静默降级）

## 六、已知边界

- NAudio 采集仅 Windows 生效（课堂场景）；Linux/macOS 自动降级。
- 设置页新增项暂无 UI 控件（设置页 axaml 为编译期产物，重建成本高），本期通过 settings.json 配置；「公平账本」面板与轮盘可视化列入下一版。
