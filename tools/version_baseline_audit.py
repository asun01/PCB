from pathlib import Path

ROOT=Path(__file__).resolve().parents[1]
DOCS=ROOT/"docs"

PATTERNS={
    "AsunImage": [],
    "HALCON 24.11": [],
}
forbidden_new_baseline=["AsunImage"]

def main():
    for p in DOCS.rglob("*.md"):
        if p.name in {"README.md","MASTER_ALL_DOCUMENTS.md","REPOSITORY-TECHNICAL-BASELINE.md"}:
            continue
        text=p.read_text(encoding="utf-8")
        for key in PATTERNS:
            if key in text:
                PATTERNS[key].append(p.as_posix())
    print("Historical/reference mentions requiring repository-baseline interpretation:")
    for key, paths in PATTERNS.items():
        print(f"- {key}: {len(paths)} documents")
        for p in paths[:20]:
            print(f"  {p}")
    print("Implementation baseline: VS2026 / .NET10 / WPF / DevExpress25.2.3 / HALCON25.11 / no AsunImage")
    return 0

if __name__=="__main__":
    raise SystemExit(main())
