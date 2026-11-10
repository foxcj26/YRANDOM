#!/usr/bin/env python3
"""从编译产物组装 YRAND .cipx 插件包（zip 格式）。

用法：
    python3 scripts/pack.py                 # 输出到 dist/
    python3 scripts/pack.py --output out   # 指定输出目录
    python3 scripts/pack.py --version 2.0.1.0  # 覆盖版本号

前置：已执行 `dotnet build src/ClassIsland.YRAND/ClassIsland.YRAND.csproj -c Release`
"""
import argparse
import json
import re
import shutil
import zipfile
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
SRC = ROOT / "src" / "ClassIsland.YRAND"
BUILD = SRC / "bin" / "Release" / "net8.0"
PLUGIN = ROOT / "plugin"

# 随包携带的依赖：插件自身 + NAudio 六件套。
# Avalonia / ClassIsland.* / FluentAvalonia 由宿主提供，不随包分发。
KEEP = {
    "ClassIsland.YRAND",
    "NAudio", "NAudio.Core", "NAudio.WinMM",
    "NAudio.Wasapi", "NAudio.Asio", "NAudio.Midi",
}

# 包内文件：编译产物 + 元数据
BUILD_FILES = [
    "ClassIsland.YRAND.dll",
    "ClassIsland.YRAND.deps.json",
    "ClassIsland.YRAND.runtimeconfig.json",
    "NAudio.dll", "NAudio.Core.dll", "NAudio.WinMM.dll",
    "NAudio.Wasapi.dll", "NAudio.Asio.dll", "NAudio.Midi.dll",
]
META_FILES = ["manifest.yml", "README.md", "icon.png"]


def read_version() -> str:
    text = (PLUGIN / "manifest.yml").read_text(encoding="utf-8")
    m = re.search(r"^version:\s*(\S+)", text, re.M)
    if not m:
        raise SystemExit("manifest.yml 中没有 version 字段")
    return m.group(1)


def trim_deps(src: Path, dst: Path) -> None:
    """精简 deps.json：只保留插件自身与 NAudio 依赖条目。"""
    d = json.loads(src.read_text(encoding="utf-8"))
    out = {"runtimeTarget": d.get("runtimeTarget"), "targets": {}, "libraries": {}}
    for target, libs in d["targets"].items():
        out["targets"][target] = {
            name: info for name, info in libs.items()
            if name.split("/")[0] in KEEP
        }
    out["libraries"] = {
        name: info for name, info in d.get("libraries", {}).items()
        if name.split("/")[0] in KEEP
    }
    dst.write_text(json.dumps(out, indent=2, ensure_ascii=False), encoding="utf-8")


def main() -> None:
    ap = argparse.ArgumentParser()
    ap.add_argument("--output", default="dist", help="输出目录（默认 dist/）")
    ap.add_argument("--version", default=None, help="覆盖 manifest 中的版本号")
    args = ap.parse_args()

    version = args.version or read_version()
    outdir = (ROOT / args.output) if not Path(args.output).is_absolute() else Path(args.output)
    outdir.mkdir(parents=True, exist_ok=True)
    stage = outdir / f"_stage_{version}"
    shutil.rmtree(stage, ignore_errors=True)
    stage.mkdir()

    for f in BUILD_FILES:
        src = BUILD / f
        if not src.exists():
            raise SystemExit(f"缺少编译产物：{src}\n请先执行 dotnet build -c Release")
        shutil.copy(src, stage / f)
    for f in META_FILES:
        shutil.copy(PLUGIN / f, stage / f)

    trim_deps(stage / "ClassIsland.YRAND.deps.json", stage / "ClassIsland.YRAND.deps.json")

    pkg = outdir / f"YRAND-{version}.cipx"
    if pkg.exists():
        pkg.unlink()
    with zipfile.ZipFile(pkg, "w", zipfile.ZIP_DEFLATED) as z:
        for f in sorted(stage.iterdir()):
            z.write(f, f.name)
    shutil.rmtree(stage)
    print(f"已生成 {pkg}（{pkg.stat().st_size} bytes，{version}）")


if __name__ == "__main__":
    main()
