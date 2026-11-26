#!/usr/bin/env python3
"""
Dump tool for MetaVersion binary files.

Format (little-endian):
  - signature: 4 bytes ("CPMV")
  - schemaVersion: int32 (must be 1)
  - fileVersion: int32
  - typeDefCount: int32
  - typeDefVersions[typeDefCount]:
      - version: int32
      - nameId: int32
  - methodDefCount: int32
  - methodDefVersions[methodDefCount]:
      - version: int32
      - signatureId: int32
"""

from __future__ import annotations

import argparse
import struct
import sys
from dataclasses import dataclass
from pathlib import Path


FILE_SIGNATURE = b"CPMV"
SCHEMA_VERSION = 1
INT32_LE = "<i"
TYPE_DEF_TABLE = 0x02000000
METHOD_DEF_TABLE = 0x06000000


@dataclass
class TypeDefVersion:
    version: int
    name_id: int


@dataclass
class MethodDefVersion:
    version: int
    signature_id: int


@dataclass
class MetaVersionFile:
    file_version: int
    type_def_versions: list[TypeDefVersion]
    method_def_versions: list[MethodDefVersion]


class ParseError(Exception):
    pass


class Reader:
    def __init__(self, data: bytes) -> None:
        self._data = data
        self._offset = 0

    @property
    def offset(self) -> int:
        return self._offset

    def read_bytes(self, size: int) -> bytes:
        end = self._offset + size
        if end > len(self._data):
            raise ParseError(
                f"unexpected EOF: need {size} bytes at offset {self._offset}, "
                f"but file size is {len(self._data)}"
            )
        chunk = self._data[self._offset:end]
        self._offset = end
        return chunk

    def read_i32(self) -> int:
        return struct.unpack(INT32_LE, self.read_bytes(4))[0]

    def ensure_consumed(self) -> None:
        if self._offset != len(self._data):
            remaining = len(self._data) - self._offset
            raise ParseError(
                f"trailing bytes detected: {remaining} bytes left at offset {self._offset}"
            )


def parse_metaversion(data: bytes) -> MetaVersionFile:
    r = Reader(data)

    signature = r.read_bytes(4)
    if signature != FILE_SIGNATURE:
        raise ParseError(f"bad signature: expected {FILE_SIGNATURE!r}, got {signature!r}")

    schema_version = r.read_i32()
    if schema_version != SCHEMA_VERSION:
        raise ParseError(
            f"bad schema version: expected {SCHEMA_VERSION}, got {schema_version}"
        )

    file_version = r.read_i32()

    type_def_count = r.read_i32()
    if type_def_count < 0:
        raise ParseError(f"invalid typeDefCount: {type_def_count}")

    type_defs: list[TypeDefVersion] = []
    for _ in range(type_def_count):
        version = r.read_i32()
        name_id = r.read_i32()
        type_defs.append(TypeDefVersion(version=version, name_id=name_id))

    method_def_count = r.read_i32()
    if method_def_count < 0:
        raise ParseError(f"invalid methodDefCount: {method_def_count}")

    method_defs: list[MethodDefVersion] = []
    for _ in range(method_def_count):
        version = r.read_i32()
        signature_id = r.read_i32()
        method_defs.append(MethodDefVersion(version=version, signature_id=signature_id))

    r.ensure_consumed()

    return MetaVersionFile(
        file_version=file_version,
        type_def_versions=type_defs,
        method_def_versions=method_defs,
    )


def dump(meta: MetaVersionFile, file_size: int, path: Path) -> None:
    print("MetaVersion Dump")
    print("=" * 60)
    print(f"file: {path}")
    print(f"size: {file_size} bytes")
    print(f"signature: {FILE_SIGNATURE.decode('ascii')}")
    print(f"schemaVersion: {SCHEMA_VERSION}")
    print(f"fileVersion: {meta.file_version}")
    print(f"typeDefCount: {len(meta.type_def_versions)}")
    print(f"methodDefCount: {len(meta.method_def_versions)}")
    print()

    print("TypeDefVersions")
    print("-" * 60)
    if not meta.type_def_versions:
        print("(empty)")
    else:
        for idx, td in enumerate(meta.type_def_versions, start=1):
            token = TYPE_DEF_TABLE | idx
            print(
                f"[{idx:6d}] token=0x{token:08X}({token:10d}) "
                f"version={td.version:11d} nameId={td.name_id:11d}"
            )
    print()

    print("MethodDefVersions")
    print("-" * 60)
    if not meta.method_def_versions:
        print("(empty)")
    else:
        for idx, md in enumerate(meta.method_def_versions, start=1):
            token = METHOD_DEF_TABLE | idx
            print(
                f"[{idx:6d}] token=0x{token:08X}({token:10d}) "
                f"version={md.version:11d} signatureId={md.signature_id:11d}"
            )


def build_arg_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(
        description="Dump all information from a MetaVersion binary file."
    )
    parser.add_argument("file", type=Path, help="Path to MetaVersion file")
    return parser


def main(argv: list[str]) -> int:
    parser = build_arg_parser()
    args = parser.parse_args(argv)
    file_path: Path = args.file

    if not file_path.exists():
        print(f"error: file not found: {file_path}", file=sys.stderr)
        return 2
    if not file_path.is_file():
        print(f"error: not a file: {file_path}", file=sys.stderr)
        return 2

    try:
        data = file_path.read_bytes()
        meta = parse_metaversion(data)
    except (OSError, ParseError) as exc:
        print(f"error: {exc}", file=sys.stderr)
        return 1

    dump(meta, len(data), file_path)
    return 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv[1:]))
