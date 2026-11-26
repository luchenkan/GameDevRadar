#!/usr/bin/env python3
"""
Dump tool for DifferentialHybridAssemblyOptions (dhao) binary files.

Format (little-endian):
  - signature: uint32 (0xABCDABCD)
  - oldDllMD5: 32 bytes UTF-8 text
  - newDllMD5: 32 bytes UTF-8 text
  - forceAllChanged: bool (1 byte)
  - changedMethodCount: uint32
  - changedMethodTokens[changedMethodCount]: uint32
  - changedStructCount: uint32
  - changedStructTokens[changedStructCount]: uint32
"""

from __future__ import annotations

import argparse
import struct
import sys
from dataclasses import dataclass
from pathlib import Path


SIGNATURE = 0xABCDABCD
U32_LE = "<I"


@dataclass
class DifferentialHybridAssemblyOptions:
    old_dll_md5: str
    new_dll_md5: str
    force_all_changed: bool
    changed_method_tokens: list[int]
    changed_struct_tokens: list[int]


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

    def read_u32(self) -> int:
        return struct.unpack(U32_LE, self.read_bytes(4))[0]

    def read_bool(self) -> bool:
        value = self.read_bytes(1)[0]
        return value != 0

    def ensure_consumed(self) -> None:
        if self._offset != len(self._data):
            remain = len(self._data) - self._offset
            raise ParseError(
                f"remain unread bytes: {remain} bytes at offset {self._offset}"
            )


def _decode_md5(raw: bytes, field_name: str) -> str:
    try:
        text = raw.decode("utf-8")
    except UnicodeDecodeError as exc:
        raise ParseError(f"{field_name} is not valid UTF-8: {exc}") from exc
    if len(text) != 32:
        raise ParseError(f"{field_name} length is {len(text)}, expected 32")
    return text


def parse_dhao(data: bytes) -> DifferentialHybridAssemblyOptions:
    r = Reader(data)

    signature = r.read_u32()
    if signature != SIGNATURE:
        raise ParseError(
            f"bad signature: expected 0x{SIGNATURE:08X}, got 0x{signature:08X}"
        )

    old_dll_md5 = _decode_md5(r.read_bytes(32), "OldDllMD5")
    new_dll_md5 = _decode_md5(r.read_bytes(32), "NewDllMD5")
    force_all_changed = r.read_bool()

    changed_method_count = r.read_u32()
    changed_method_tokens: list[int] = []
    for _ in range(changed_method_count):
        changed_method_tokens.append(r.read_u32())

    changed_struct_count = r.read_u32()
    changed_struct_tokens: list[int] = []
    for _ in range(changed_struct_count):
        changed_struct_tokens.append(r.read_u32())

    r.ensure_consumed()

    return DifferentialHybridAssemblyOptions(
        old_dll_md5=old_dll_md5,
        new_dll_md5=new_dll_md5,
        force_all_changed=force_all_changed,
        changed_method_tokens=changed_method_tokens,
        changed_struct_tokens=changed_struct_tokens,
    )


def dump(options: DifferentialHybridAssemblyOptions, file_size: int, path: Path) -> None:
    print("DifferentialHybridAssemblyOptions Dump")
    print("=" * 60)
    print(f"file: {path}")
    print(f"size: {file_size} bytes")
    print(f"signature: 0x{SIGNATURE:08X} ({SIGNATURE})")
    print(f"oldDllMD5: {options.old_dll_md5}")
    print(f"newDllMD5: {options.new_dll_md5}")
    print(f"forceAllChanged: {options.force_all_changed}")
    print(f"changedMethodCount: {len(options.changed_method_tokens)}")
    print(f"changedStructCount: {len(options.changed_struct_tokens)}")
    print()

    print("ChangedMethodTokens")
    print("-" * 60)
    if not options.changed_method_tokens:
        print("(empty)")
    else:
        for i, token in enumerate(options.changed_method_tokens, start=1):
            print(f"[{i:6d}] token=0x{token:08X} ({token:10d})")
    print()

    print("ChangedStructTokens")
    print("-" * 60)
    if not options.changed_struct_tokens:
        print("(empty)")
    else:
        for i, token in enumerate(options.changed_struct_tokens, start=1):
            print(f"[{i:6d}] token=0x{token:08X} ({token:10d})")


def build_arg_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(
        description="Dump all information from a DifferentialHybridAssemblyOptions file."
    )
    parser.add_argument("file", type=Path, help="Path to .dhao binary file")
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
        options = parse_dhao(data)
    except (OSError, ParseError) as exc:
        print(f"error: {exc}", file=sys.stderr)
        return 1

    dump(options, len(data), file_path)
    return 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv[1:]))
