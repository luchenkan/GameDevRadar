from pathlib import Path
import dnfile
from dncil.cil.body import CilMethodBody
from dncil.cil.body.reader import CilMethodBodyReaderBytes
from dncil.clr.token import Token, StringToken

root = Path(__file__).resolve().parents[1]
path = root / "artifacts" / "Unity.IL2CPP.dll.1"
pe = dnfile.dnPE(str(path))
raw = path.read_bytes()
md = pe.net.mdtables
method_rid = {id(row): rid for rid, row in enumerate(md.MethodDef.rows, 1)}
method_owner = {}
for t in md.TypeDef.rows:
    owner = f"{t.TypeNamespace}.{t.TypeName}".strip(".")
    for item in t.MethodList:
        if item.row is not None:
            method_owner[method_rid[id(item.row)]] = owner


def table_row(table, rid):
    if not table or rid < 1 or rid > len(table.rows):
        return None
    return table.rows[rid - 1]


def token_name(value):
    table_id, rid = value >> 24, value & 0xFFFFFF
    if table_id == 0x70:
        item = pe.net.user_strings.get(rid)
        return repr(str(item.value if hasattr(item, "value") else item))
    row = None
    if table_id == 0x06:
        row = table_row(md.MethodDef, rid)
        if row:
            return f"{method_owner.get(rid, '?')}::{row.Name}"
    elif table_id == 0x0A:
        row = table_row(md.MemberRef, rid)
        if row:
            parent = row.Class.row if row.Class else None
            parent_name = f"{parent.TypeNamespace}.{parent.TypeName}".strip(".") if parent is not None and hasattr(parent, "TypeName") else "?"
            return f"{parent_name}::{row.Name}"
    elif table_id == 0x04:
        row = table_row(md.Field, rid)
        if row:
            return f"field {row.Name}"
    elif table_id in (0x01, 0x02):
        table = md.TypeRef if table_id == 0x01 else md.TypeDef
        row = table_row(table, rid)
        if row:
            return f"{row.TypeNamespace}.{row.TypeName}".strip(".")
    return f"token(0x{value:08X})"


def parse_method(row):
    if not row.Rva:
        return None
    try:
        offset = pe.get_offset_from_rva(row.Rva)
        return CilMethodBody(CilMethodBodyReaderBytes(raw[offset:]))
    except Exception:
        return None


bodies = {method_rid[id(row)]: parse_method(row) for row in md.MethodDef.rows}
target_tokens = set()
for row in md.MethodDef.rows:
    rid = method_rid[id(row)]
    if method_owner.get(rid) == "HybridCLRConfig" and str(row.Name) in ("IsDifferentialHybridAssembly", "NeedInject"):
        target_tokens.add(0x06000000 | rid)

caller_rids = set()
for rid, body in bodies.items():
    if body is None:
        continue
    for insn in body.instructions:
        if isinstance(insn.operand, Token) and not isinstance(insn.operand, StringToken) and insn.operand.value in target_tokens:
            caller_rids.add(rid)

selected = set(caller_rids)
for row in md.MethodDef.rows:
    rid = method_rid[id(row)]
    if method_owner.get(rid) == "HybridCLRConfig":
        selected.add(rid)

out = []
out.append(f"Callers of HybridCLRConfig hooks: {len(caller_rids)}")
for rid in sorted(selected):
    row = table_row(md.MethodDef, rid)
    body = bodies[rid]
    out.append(f"\n=== {method_owner.get(rid, '?')}::{row.Name} [0x{0x06000000 | rid:08X}] ===")
    if body is None:
        out.append("<no body>")
        continue
    for insn in body.instructions:
        operand = ""
        if isinstance(insn.operand, Token):
            operand = token_name(insn.operand.value)
        elif insn.operand is not None:
            operand = str(insn.operand)
        out.append(f"IL_{insn.offset:04X}: {insn.mnemonic:<14} {operand}")

output = root / "analysis" / "Unity.IL2CPP.key-methods.il.txt"
output.parent.mkdir(parents=True, exist_ok=True)
output.write_text("\n".join(out), encoding="utf-8")
print("\n".join(out[:8]))
print(f"Wrote {output} with {len(out)} lines")
