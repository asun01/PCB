from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
OPEN_GATES = ROOT / "docs" / "00-baseline" / "OPEN-GATES.md"


def main() -> int:
    if not OPEN_GATES.exists():
        print(f"ERROR: missing gate register: {OPEN_GATES.relative_to(ROOT)}")
        return 1

    text = OPEN_GATES.read_text(encoding="utf-8")
    rows = []
    for line in text.splitlines():
        if not line.startswith("| "):
            continue
        if line.startswith("| Gate "):
            continue
        if set(line.replace("|", "").strip()) <= {"-", ":"}:
            continue
        rows.append(line)

    print("Recorded open gates:")
    for row in rows:
        print(row)

    print(
        "Policy: open gates are reported as facts; this tool never closes, "
        "downgrades, or infers gate status."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
