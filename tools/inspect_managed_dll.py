from pathlib import Path
import dnfile

root = Path(__file__).resolve().parents[1]
path = root / "artifacts" / "Unity.IL2CPP.dll.1"
pe = dnfile.dnPE(str(path))
md = pe.net.mdtables
print(f"Assembly: {path.name}")
print(f"TypeDef rows: {len(md.TypeDef.rows)}; MethodDef rows: {len(md.MethodDef.rows)}")
keywords = ("hybrid", "differential", "dhe", "interp", "hotupdate", "hotfix")
for t in md.TypeDef.rows:
    full_name = f"{t.TypeNamespace}.{t.TypeName}".strip(".")
    methods = []
    for item in t.MethodList:
        row = item.row
        if row is not None:
            methods.append(str(row.Name))
    matched_type = any(k in full_name.lower() for k in keywords)
    matched_methods = [m for m in methods if any(k in m.lower() for k in keywords)]
    if matched_type or matched_methods:
        print(f"\nTYPE {full_name}")
        for m in methods:
            marker = " *" if m in matched_methods else ""
            print(f"  {m}{marker}")
