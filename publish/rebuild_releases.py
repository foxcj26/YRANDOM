#!/usr/bin/env python3
"""GitHub 账号解封后：一键重建 / 校验 YRAND 的 7 个纯数字 tag Release（幂等）。

用法（解封后执行，Token 需具备 repo 权限）：
    export GH_TOKEN=<你的 token>
    python3 publish/rebuild_releases.py            # 重建（幂等）并校验
    python3 publish/rebuild_releases.py --check    # 仅校验，不做任何写操作

规则（对应 ClassIsland 2.x 插件市场上架要求）：
  - tag 严格为 a.b.c.d：2.0.0.9 ~ 2.0.0.15，全部指向提交
    a4243e2f8976730ff35827a702c74e0f47004e5c（与原 v-tag 相同提交）
  - Release 名称：2.0.0.x · 主题
  - 备注 = docs/changelog 对应修改说明原文 + 末尾官方 MD5 注释
    <!-- CLASSISLAND_PKG_MD5 {"YRAND-<v>.cipx": "<md5>"} -->
  - 附件 = releases/YRAND-<v>.cipx；2.0.0.15 设为 latest
"""
import hashlib
import json
import sys
from pathlib import Path

import requests

ROOT = Path(__file__).resolve().parent.parent
OWNER, REPO = "foxcj26", "YRAND"
API = "https://api.github.com"
UPLOAD = "https://uploads.github.com"
SHA = "a4243e2f8976730ff35827a702c74e0f47004e5c"
VERSIONS = ["2.0.0.9", "2.0.0.10", "2.0.0.11", "2.0.0.12", "2.0.0.13", "2.0.0.14", "2.0.0.15"]
THEMES = {
    "2.0.0.9": "通知逻辑重写",
    "2.0.0.10": "公平算法 v2 + 麦克风声浪加成",
    "2.0.0.11": "Fluent 抽签图标标识",
    "2.0.0.12": "麦克风声浪加成入设置页",
    "2.0.0.13": "滚动效果回归 · 两段式可见 · 结果通知去指纹",
    "2.0.0.14": "过程状态统一「抽取中」· 自动上传",
    "2.0.0.15": "修复快速滚动重复弹窗",
}

TOKEN = __import__("os").environ.get("GH_TOKEN", "")
if not TOKEN:
    sys.exit("请先 export GH_TOKEN=<token>")
H = {"Authorization": f"token {TOKEN}", "Accept": "application/vnd.github+json"}
UH = {**H, "Content-Type": "application/octet-stream"}
CHECK_ONLY = "--check" in sys.argv


def rel(v: str) -> Path:
    return ROOT / "releases" / f"YRAND-{v}.cipx"


def body_for(v: str) -> str:
    md = (ROOT / "docs" / "changelog" / f"YRAND-{v}-修改说明.md").read_text(encoding="utf-8").strip()
    md5 = hashlib.md5(rel(v).read_bytes()).hexdigest()
    return md + f'\n\n<!-- CLASSISLAND_PKG_MD5 {{"YRAND-{v}.cipx": "{md5}"}} -->\n'


def main() -> None:
    r = requests.get(f"{API}/repos/{OWNER}/{REPO}", headers=H, timeout=30)
    if r.status_code != 200:
        sys.exit(f"仓库不可达：HTTP {r.status_code} {r.json().get('message', '')}（账号是否已解封？）")
    releases = {x["tag_name"]: x for x in
                requests.get(f"{API}/repos/{OWNER}/{REPO}/releases?per_page=100", headers=H, timeout=30).json()}
    tags = {t["name"] for t in
            requests.get(f"{API}/repos/{OWNER}/{REPO}/tags?per_page=100", headers=H, timeout=30).json()}

    for v in VERSIONS:
        name = f"{v} · {THEMES[v]}"
        data = rel(v).read_bytes()
        digest = "sha256:" + hashlib.sha256(data).hexdigest()
        md5 = hashlib.md5(data).hexdigest()
        body = body_for(v)

        if CHECK_ONLY:
            x = releases.get(v)
            ok = bool(x) and x["body"].rstrip().endswith(f'{{"YRAND-{v}.cipx": "{md5}"}} -->')
            print(f"  {v:10} Release {'✓' if x else '✗ 缺失'} | MD5 注释 {'✓' if ok else '✗'} | "
                  f"附件 {[a['name'] for a in (x['assets'] if x else [])]}")
            continue

        if v not in releases:
            c = requests.post(f"{API}/repos/{OWNER}/{REPO}/releases", headers=H, timeout=30,
                              json={"tag_name": v, "target_commitish": SHA, "name": name,
                                    "body": body, "draft": False, "prerelease": False})
            assert c.status_code == 201, (c.status_code, c.text[:300])
            releases[v] = c.json()
            print(f"  创建 Release {v} (tag {'复用' if v in tags else '新建'})")
        else:
            p = requests.patch(f"{API}/repos/{OWNER}/{REPO}/releases/{releases[v]['id']}",
                               headers=H, timeout=30, json={"name": name, "body": body})
            assert p.status_code == 200, (p.status_code, p.text[:300])
            print(f"  更新 Release {v}（备注=修改说明+MD5）")

        rel_id = releases[v]["id"]
        assets = {a["name"]: a for a in releases[v]["assets"]}
        aname = f"YRAND-{v}.cipx"
        if aname not in assets:
            with rel(v).open("rb") as f:
                u = requests.post(f"{UPLOAD}/repos/{OWNER}/{REPO}/releases/{rel_id}/assets",
                                  params={"name": aname}, headers=UH, data=f, timeout=180)
            assert u.status_code in (200, 201), (u.status_code, u.text[:300])
            a = u.json()
        else:
            a = assets[aname]
        same = a["size"] == len(data) and a.get("digest") == digest
        print(f"    附件 {aname} {a['size']}B digest {'一致 ✓' if same else '不一致 ✗'}")
        assert same

    if not CHECK_ONLY:
        lid = releases["2.0.0.15"]["id"]
        p = requests.patch(f"{API}/repos/{OWNER}/{REPO}/releases/{lid}",
                           headers=H, timeout=30, json={"make_latest": "true"})
        print("  latest -> 2.0.0.15:", p.status_code)

    latest = requests.get(f"{API}/repos/{OWNER}/{REPO}/releases/latest", headers=H, timeout=30).json()
    print(f"  远端 latest = {latest.get('tag_name')}")
    print("完成 ✓")


if __name__ == "__main__":
    main()
