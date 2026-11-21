# YRAND 发布包 MD5 校验信息

依据：<https://docs.classisland.tech/dev/plugins/publishing.html> 「上架到插件市场」一节。
索引生成器从 Release 发行日志中提取 `CLASSISLAND_PKG_MD5` 注释；请把下面**第 1 行的注释**原样追加到目标 Release 备注的末尾（HTML 注释，页面渲染时不可见）。

<!-- CLASSISLAND_PKG_MD5 {"YRAND-2.0.0.15.cipx": "9d13d4dcb1c3734f97582643e4143306"} -->

> 只复制上面那一行。若同一 Release 挂了多个附件，把字典补全即可（键为附件文件名，值为该文件的 MD5 大写/小写十六进制均可）。

## 全部 7 个发布包（备用）

| 文件 | MD5 |
|---|---|
| YRAND-2.0.0.9.cipx | `4823bab7b5acdbf6c4b94f6b661019eb` |
| YRAND-2.0.0.10.cipx | `822da4ea330c368cd804df92810b29d7` |
| YRAND-2.0.0.11.cipx | `a656d6be5b98f9325ace0875f212962d` |
| YRAND-2.0.0.12.cipx | `2418c81e31c4beb6978923da644f17a0` |
| YRAND-2.0.0.13.cipx | `54c0d2e1ea039f59de8f6791d847a8d3` |
| YRAND-2.0.0.14.cipx | `44ef5fe23ce82ea9f8000cef693f8f50` |
| YRAND-2.0.0.15.cipx | `9d13d4dcb1c3734f97582643e4143306` |

## 全量字典（多个附件时可用）

```text
<!-- CLASSISLAND_PKG_MD5 {"YRAND-2.0.0.9.cipx": "4823bab7b5acdbf6c4b94f6b661019eb", "YRAND-2.0.0.10.cipx": "822da4ea330c368cd804df92810b29d7", "YRAND-2.0.0.11.cipx": "a656d6be5b98f9325ace0875f212962d", "YRAND-2.0.0.12.cipx": "2418c81e31c4beb6978923da644f17a0", "YRAND-2.0.0.13.cipx": "54c0d2e1ea039f59de8f6791d847a8d3", "YRAND-2.0.0.14.cipx": "44ef5fe23ce82ea9f8000cef693f8f50", "YRAND-2.0.0.15.cipx": "9d13d4dcb1c3734f97582643e4143306"} -->
```

> 提醒：若之后重新打包（例如同步作者字段），包字节变化会改变 MD5，需重新计算并更新发行日志。本文件由 `releases/` 下当前包计算得出。

## 新增 4 个发布包（v2.0.0.16–v2.0.0.19，foxcj26/YRANDOM Release 备注均已内嵌 MD5 注释）

| 文件 | MD5 |
|---|---|
| YRAND-2.0.0.16.cipx | `915a1318ba2942dcd777760e573ea1de` |
| YRAND-2.0.0.17.cipx | `26acd5562f8231ac40596c311102fd40` |
| YRANDOM-2.0.0.18.cipx | `89994c2760cb2b6996a5f913ebffee51` |
| YRANDOM-2.0.0.19.cipx | `832f4654f0dc5e0f2be642d94bc6d4b4` |
