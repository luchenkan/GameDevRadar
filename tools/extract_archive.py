from pathlib import Path
import py7zr

root = Path(__file__).resolve().parents[1]
archive = root / "artifacts" / "dhe-2022-tuanjie-8.15.0.7z"
out = root / "artifacts" / "dhe"
out.mkdir(parents=True, exist_ok=True)
with py7zr.SevenZipFile(archive, mode="r") as z:
    for entry in z.list():
        print(entry.filename)
    z.extractall(path=out)
