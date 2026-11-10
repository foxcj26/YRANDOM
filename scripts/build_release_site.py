#!/usr/bin/env python3
# 生成 YRAND 版本发布站：汇总页 + 4 个版本子页（含 cipx/zip/说明下载）
import os, shutil, markdown

WS = "/workspace"
SITE = "/workspace/yrand-versions"
shutil.rmtree(SITE, ignore_errors=True)
os.makedirs(SITE, exist_ok=True)

VERSIONS = [
    {
        "ver": "2.0.0.9",
        "title": "通知时机重写",
        "tagline": "整套抽选结束才通知，滚动与定格全程静默",
        "date": "2026-11-05",
        "size": "32 KB",
        "way": "ILSpy 反编译 + Mono.Cecil IL 精准补丁",
        "highlights": [
            "只在整套抽选结束（含手动定格）时推送一次「抽选结果」提醒并播报",
            "滚动期间提醒条完全不下落；第 1/2 段中间定格不发通知、不消耗名额",
            "名单为空的错误提示保留；状态机、反作弊等其余逻辑字节级不变",
        ],
        "color": "#4FC3F7",
    },
    {
        "ver": "2.0.0.10",
        "title": "公平算法 v2 + 麦克风声浪",
        "tagline": "债务校正 + 冷落兜底，一次过程只消耗 1 个名额",
        "date": "2026-11-05",
        "size": "254 KB",
        "way": "完整重建源码工程，真实编译 0 错误",
        "highlights": [
            "公平算法：债务表 due + Gumbel 加权抽样 + 冷落滑窗兜底，实测 N=10 抽 1000 次每人偏差 ≤1",
            "点击瞬间锁定唯一结果，滚动/定格/连滑全是零消耗表演；种子指纹公示、settings.json 可复算",
            "麦克风声浪加成（NAudio 实时 dBFS）：喊得越响连滑概率越高，滚动也越快；无麦克风静默降级",
            "滚动 ease-out 减速（150→400ms），补上 README 一直宣称却没实现的「逐渐放缓」",
        ],
        "color": "#7C4DFF",
    },
    {
        "ver": "2.0.0.11",
        "title": "Fluent 图标标识",
        "tagline": "抽签图标统一覆盖设置导航 / 提醒 / 结果条",
        "date": "2026-11-05",
        "size": "254 KB",
        "way": "从宿主内嵌 FluentSystemIcons 字体精确选码位",
        "highlights": [
            "ic_fluent_lottery 抽签图标（U+EAFC 线性 / U+EAFB 实心）覆盖 4 处：设置导航、提醒提供方、结果提醒条、manifest",
            "顺手修正原版设置页特性图标位被误填中文描述的问题，设置页 id 恢复 ima.yrand",
            "图标来自宿主自带字体，不新增任何依赖或资源文件",
        ],
        "preview": True,
        "color": "#FFB74D",
    },
    {
        "ver": "2.0.0.12",
        "title": "麦克风加成入设置页",
        "tagline": "总开关 · 随机/固定模式 · 数值滑块，即改即存",
        "date": "2026-11-05",
        "size": "256 KB",
        "way": "手工注入设置页编译码 + 运行时绑定",
        "highlights": [
            "设置页新增「麦克风声浪加成」卡片：总开关、随声浪（随机）/固定两种加速率模式",
            "固定加速率（默认 +25%）与声浪最大加速率（默认 +45%）两个 0-100% 滑块，即改即存",
            "固定模式：音量过阈值即给固定加成；随机模式：按音量线性爬升；中签者始终与音量无关",
        ],
        "color": "#81C784",
    },
    {
        "ver": "2.0.0.13",
        "title": "滚动效果回归 · 两段式修复",
        "tagline": "恢复滚动换名展示与两段式连抽节奏，结果通知移除指纹",
        "date": "2026-11-06",
        "size": "583 KB",
        "way": "源码工程上继续迭代，真实编译 0 错误",
        "highlights": [
            "滚动期间恢复「同一条提醒原地实时换名」（不下落新条）",
            "中间定格改显示同源取样过渡名（避开锁定名防剧透）：每一段都有定格与停顿，两段式连抽重新可见；静默不播报、不消耗名额",
            "「抽选结果」通知不再附带指纹，指纹仅保留在悬浮窗 Tooltip 供复核",
        ],
        "color": "#F06292",
    },
    {
        "ver": "2.0.0.14",
        "title": "状态统一 · 抽取中",
        "tagline": "过程状态只显示「抽取中」，不再出现「正在抽选/已定格」",
        "date": "2026-11-06",
        "size": "597 KB",
        "way": "源码工程上继续迭代，真实编译 0 错误",
        "highlights": [
            "抽选过程期间通知条状态统一为「抽取中」：滚动换名与中间定格共用同一词，观众无需理解内部阶段",
            "滚动换名仍是同一条提醒原地实时换名；中间定格静默、不播报、不消耗名额",
            "最终结果仍为「抽选结果」+ 播报；本版起新包自动上传发布台",
        ],
        "color": "#4DD0E1",
    },
    {
        "ver": "2.0.0.15",
        "title": "修复快速滚动重复弹窗",
        "tagline": "整套抽选只注册一条过程通知，换名永不重新注册",
        "date": "2026-11-09",
        "size": "597 KB",
        "way": "源码工程上继续迭代，真实编译 0 错误",
        "highlights": [
            "根因：通知存活判断依赖 CompletedToken 清空引用，弹窗一被关闭，下次换名（最快 97ms 后）就走重新注册，新条顶掉未渲染完的旧条",
            "修复：过程开始注册唯一一条「抽取中」通知；滚动/定格换名只改控件属性，通知已结束则静默跳过、绝不补弹",
            "全程最多 2 次弹窗（过程开始 + 抽选结果）；抽选状态机、公平算法、麦克风加成全部未动",
        ],
        "color": "#AED581",
    },
]

CSS = """
:root{--bg:#141418;--card:#1E1E26;--card2:#262630;--line:#3A3A48;--fg:#EDEDF5;--mut:#9A9AAE;--acc:#7C4DFF}
*{box-sizing:border-box;margin:0;padding:0}
body{background:var(--bg);color:var(--fg);font-family:'Segoe UI','Microsoft YaHei',system-ui,sans-serif;line-height:1.65}
.wrap{max-width:860px;margin:0 auto;padding:48px 24px 80px}
header{text-align:center;margin-bottom:40px}
h1{font-size:2.1rem;letter-spacing:1px}
h1 .accent{color:var(--acc)}
.sub{color:var(--mut);margin-top:10px;font-size:.95rem}
.timeline{position:relative;padding-left:26px}
.timeline::before{content:'';position:absolute;left:7px;top:8px;bottom:8px;width:2px;background:var(--line)}
.card{position:relative;background:var(--card);border:1px solid var(--line);border-radius:18px;padding:24px 26px;margin-bottom:18px}
.card::before{content:'';position:absolute;left:-26px;top:28px;width:12px;height:12px;border-radius:50%;background:var(--dot,#7C4DFF);box-shadow:0 0 0 4px rgba(124,77,255,.15)}
.badge{display:inline-block;font-size:.72rem;padding:2px 10px;border-radius:99px;border:1px solid var(--line);color:var(--mut);margin-right:6px;vertical-align:middle}
.badge.latest{background:var(--acc);color:#fff;border-color:var(--acc)}
.vnum{font-size:1.5rem;font-weight:700}
.vnum a{color:inherit;text-decoration:none}
.vnum a:hover{color:var(--acc)}
.tmeta{color:var(--mut);font-size:.85rem;margin:2px 0 10px}
.tagline{font-size:.98rem;margin-bottom:12px}
ul.hl{margin:0 0 16px 18px;color:#C9C9D6;font-size:.92rem}
ul.hl li{margin-bottom:4px}
.btns{display:flex;gap:10px;flex-wrap:wrap}
.btn{display:inline-block;padding:8px 18px;border-radius:10px;text-decoration:none;font-size:.9rem;border:1px solid var(--line);color:var(--fg);transition:.15s}
.btn:hover{border-color:var(--acc);color:#fff}
.btn.primary{background:var(--acc);border-color:var(--acc);color:#fff}
.btn.primary:hover{filter:brightness(1.15)}
.note{background:var(--card);border:1px dashed var(--line);border-radius:14px;padding:16px 20px;color:var(--mut);font-size:.88rem;margin-top:28px}
.story{display:flex;align-items:center;justify-content:space-between;gap:16px;background:linear-gradient(120deg,#2A2140,#1E1E26);border:1px solid var(--acc);border-radius:16px;padding:18px 22px;margin-bottom:26px;text-decoration:none;color:var(--fg)}
.story:hover{filter:brightness(1.18)}
.story b{display:block;font-size:1rem}
.story span{display:block;color:var(--mut);font-size:.86rem;margin-top:4px}
.story em{font-style:normal;color:var(--acc2);white-space:nowrap;font-size:.9rem}
.preview{max-width:100%;border-radius:14px;border:1px solid var(--line);margin:12px 0 16px}
.back{display:inline-block;color:var(--mut);text-decoration:none;font-size:.9rem;margin-bottom:22px}
.back:hover{color:var(--acc)}
.doc{background:var(--card);border:1px solid var(--line);border-radius:16px;padding:26px 30px;margin-top:26px;font-size:.93rem}
.doc h1{font-size:1.3rem;margin-bottom:14px}
.doc h2{font-size:1.1rem;margin:20px 0 8px;border-bottom:1px solid var(--line);padding-bottom:6px}
.doc h3{font-size:1rem;margin:14px 0 6px}
.doc table{border-collapse:collapse;width:100%;margin:10px 0;font-size:.86rem}
.doc th,.doc td{border:1px solid var(--line);padding:6px 10px;text-align:left}
.doc th{background:var(--card2)}
.doc code{background:var(--card2);border-radius:6px;padding:1px 6px;font-size:.85em;color:#C9B8FF}
.doc pre code{display:block;padding:12px;overflow-x:auto}
.doc blockquote{border-left:3px solid var(--acc);padding-left:12px;color:var(--mut);margin:10px 0}
.doc ul,.doc ol{margin:8px 0 8px 20px}
footer{text-align:center;color:var(--mut);font-size:.8rem;margin-top:40px}
"""

SHARE_JS = """
(function(){
  var q = location.search;
  if (!q) return;
  document.querySelectorAll('a[href]').forEach(function(a){
    var h = a.getAttribute('href');
    if (h && !h.startsWith('http') && !h.startsWith('#') && !h.startsWith('mailto')) a.setAttribute('href', h + q);
  });
})();
"""

def page(title, body):
    return f"""<!DOCTYPE html>
<html lang="zh-CN"><head><meta charset="utf-8">
<meta name="viewport" content="width=device-width,initial-scale=1">
<title>{title}</title><style>{CSS}</style></head>
<body><div class="wrap">{body}</div>
<script>{SHARE_JS}</script></body></html>"""

def render_md(path):
    text = open(path, encoding="utf-8").read()
    return markdown.markdown(text, extensions=["tables", "fenced_code"])

# ---------- 版本子页 ----------
for v in VERSIONS:
    ver = v["ver"]
    d = f"{SITE}/v{ver}"
    os.makedirs(d, exist_ok=True)
    for ext in ("cipx", "zip", ""):
        src = f"{WS}/YRAND-{ver}.cipx" if ext == "cipx" else (f"{WS}/YRAND-{ver}.zip" if ext == "zip" else f"{WS}/YRAND-{ver}-修改说明.md")
        shutil.copy(src, d)
    if v.get("preview"):
        shutil.copy(f"{WS}/YRAND-fluent-icon-preview.png", d)

    preview_html = '<img class="preview" src="YRAND-fluent-icon-preview.png" alt="Fluent 图标预览">' if v.get("preview") else ""
    hl = "".join(f"<li>{h}</li>" for h in v["highlights"])
    body = f"""
<a class="back" href="../">← 返回版本汇总</a>
<header style="text-align:left;margin-bottom:22px">
  <div><span class="badge">{v['date']}</span><span class="badge">{v['size']}</span><span class="badge">{v['way']}</span></div>
  <h1 style="margin-top:10px">YRAND <span class="accent">v{ver}</span> · {v['title']}</h1>
  <p class="sub">{v['tagline']}</p>
</header>
{preview_html}
<ul class="hl">{hl}</ul>
<div class="btns">
  <a class="btn primary" href="YRAND-{ver}.cipx">⬇ 下载插件包 .cipx（推荐）</a>
  <a class="btn" href="YRAND-{ver}.zip">⬇ .zip 副本</a>
  <a class="btn" href="YRAND-{ver}-修改说明.md">📄 修改说明 .md</a>
</div>
<div class="doc"><h2>完整修改说明</h2>{render_md(f'{WS}/YRAND-{ver}-修改说明.md')}</div>
<a class="back" href="../" style="margin-top:26px">← 返回版本汇总</a>"""
    open(f"{d}/index.html", "w", encoding="utf-8").write(page(f"YRAND v{ver}", body))

# ---------- 开发全记录子页（独立分享路径 /story/） ----------
STORY_SRC = "/workspace/yrand-story"
STORY_DST = f"{SITE}/story"
os.makedirs(STORY_DST, exist_ok=True)
for f in os.listdir(STORY_SRC):
    src = os.path.join(STORY_SRC, f)
    if os.path.isfile(src):
        shutil.copy(src, STORY_DST)

# ---------- 汇总页 ----------
cards = []
for i, v in enumerate(VERSIONS):
    latest = ' <span class="badge latest">最新</span>' if i == len(VERSIONS) - 1 else ""
    hl = "".join(f"<li>{h}</li>" for h in v["highlights"][:3])
    cards.append(f"""
<div class="card" style="--dot:{v['color']}">
  <div class="vnum"><a href="v{v['ver']}/">v{v['ver']} · {v['title']}</a>{latest}</div>
  <div class="tmeta">{v['date']} · {v['size']} · {v['way']}</div>
  <div class="tagline">{v['tagline']}</div>
  <ul class="hl">{hl}</ul>
  <div class="btns">
    <a class="btn primary" href="v{v['ver']}/">进入版本页</a>
    <a class="btn" href="v{v['ver']}/YRAND-{v['ver']}.cipx">直接下载 .cipx</a>
  </div>
</div>""")

body = f"""
<header>
  <h1>YRAND <span class="accent">·</span> 版本发布台</h1>
  <p class="sub">ClassIsland 课堂随机点名插件 · 2026-11-05 一次性迭代发布的 4 个版本（自线上 v2.0.0.8 起步）</p>
</header>
<a class="story" href="story/">
  <div><b>📖 开发全记录 · 一次对话驱动的插件进化</b>
  <span>12 轮对话 · 6 幕 · 完整需求与实现脉络，附 3 张新点名算法示意图（主流程 / 公平内核 / 麦克风曲线）</span></div>
  <em>查看 →</em>
</a>
<div class="timeline">{''.join(cards)}</div>
<div class="note">
  <b>安装方式</b>：ClassIsland → 插件管理 → 本地安装，选择 .cipx 包（覆盖安装前建议先卸载旧版）。<br>
  <b>版本取舍</b>：每个版本在上一版基础上叠加，直接安装 <b>v2.0.0.15（最新）</b> 即可获得全部功能。<br>
  <b>公平性说明</b>：v2.0.0.10 起最终中签者在点击瞬间锁定，滑动概率 / 麦克风音量 / 连滑次数只影响表演时长，不影响抽到谁。
</div>
<footer>YRAND · ima.yrand · ClassIsland.PluginSdk 2.0.0.2 / net8.0</footer>"""
open(f"{SITE}/index.html", "w", encoding="utf-8").write(page("YRAND 版本发布台", body))

print("站点生成完成：")
for root, _, files in os.walk(SITE):
    for f in sorted(files):
        print(" ", os.path.relpath(os.path.join(root, f), SITE))
