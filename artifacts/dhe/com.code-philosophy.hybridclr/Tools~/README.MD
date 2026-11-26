# MetaVersionDump Tools / MetaVersionDump 工具说明

本目录包含两个二进制 dump 工具，用于解析并打印 `MetaVersion` 与 `DHAO` 文件的全部信息。  
This directory contains two binary dump tools for printing all information from `MetaVersion` and `DHAO` files.

---

## 1) 工具列表 / Tool List

- `dump_metaversion.py`
  - 解析 `MetaVersionFile` 格式（签名 `CPMV`，schema version = `1`）。
  - Parses `MetaVersionFile` format (signature `CPMV`, schema version = `1`).
- `dump_dhao.py`
  - 解析 `DifferentialHybridAssemblyOptions` 格式（签名 `0xABCDABCD`）。
  - Parses `DifferentialHybridAssemblyOptions` format (signature `0xABCDABCD`).

---

## 2) 环境要求 / Requirements

- Python 3.9+（推荐）
- 仅使用 Python 标准库，无第三方依赖

- Python 3.9+ (recommended)
- Standard library only, no third-party dependencies

---

## 3) 用法 / Usage

在当前目录执行（Windows / PowerShell 示例）：  
Run in this directory (Windows / PowerShell examples):

```bash
python .\dump_metaversion.py .\DHE.mv.bytes
python .\dump_dhao.py .\DHE.dhao.bytes
```

也可传绝对路径：  
You can also use absolute paths:

```bash
python d:/workspace/business-editions/tools/MetaVersionDump/dump_metaversion.py d:/workspace/business-editions/tools/MetaVersionDump/DHE.mv.bytes
python d:/workspace/business-editions/tools/MetaVersionDump/dump_dhao.py d:/workspace/business-editions/tools/MetaVersionDump/DHE.dhao.bytes
```

---

## 4) 输出说明 / Output Notes

### `dump_metaversion.py`

- 打印文件头信息：`signature`、`schemaVersion`、`fileVersion`、计数信息。
- 打印 TypeDef 与 MethodDef 条目。
- 每条打印 CLI token（而非 rid）：
  - TypeDef token = `0x02000000 | rid`
  - MethodDef token = `0x06000000 | rid`
- 同时打印十六进制和十进制 token。

- Prints header fields: `signature`, `schemaVersion`, `fileVersion`, and counts.
- Prints TypeDef and MethodDef entries.
- Prints CLI token (not rid):
  - TypeDef token = `0x02000000 | rid`
  - MethodDef token = `0x06000000 | rid`
- Shows both hex and decimal token values.

### `dump_dhao.py`

- 打印文件头信息：`signature`、`oldDllMD5`、`newDllMD5`、`forceAllChanged`。
- 打印 `ChangedMethodTokens` 与 `ChangedStructTokens` 全部条目。
- token 同时显示十六进制和十进制。

- Prints header fields: `signature`, `oldDllMD5`, `newDllMD5`, `forceAllChanged`.
- Prints all `ChangedMethodTokens` and `ChangedStructTokens`.
- Tokens are shown in both hex and decimal.

---

## 5) 校验与错误处理 / Validation & Errors

- 两个工具都会校验文件签名（signature）。
- `dump_metaversion.py` 校验 schema version 必须为 `1`。
- 两个工具都会检查是否存在“未读尾部字节”（trailing/remain unread bytes）。
- 文件不存在、格式错误、读取越界等情况会输出错误并返回非 0 退出码。

- Both tools validate file signatures.
- `dump_metaversion.py` validates schema version must be `1`.
- Both tools ensure no unread trailing bytes remain.
- On missing file or invalid format, they print errors and return non-zero exit code.

---

## 6) 参考 / Reference

- `DHE.mv.bytes`: MetaVersion 样例文件 / MetaVersion sample
- `DHE.dhao.bytes`: DHAO 样例文件 / DHAO sample
- `DHE.mv.spec`: 可读对照信息（用于校验 token/version 映射）/ human-readable spec for cross-checking token/version mapping
