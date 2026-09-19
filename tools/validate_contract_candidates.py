from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
CONTRACTS = ROOT / "docs" / "13-contract-candidates" / "CONTRACT-CANDIDATES.md"

REQUIRED_SECTIONS = tuple(f"C-{i:02d}" for i in range(1, 9))


def main() -> int:
    if not CONTRACTS.exists():
        print(f"ERROR: missing ContractCandidate register: {CONTRACTS.relative_to(ROOT)}")
        return 1

    text = CONTRACTS.read_text(encoding="utf-8")
    errors = []

    for section in REQUIRED_SECTIONS:
        marker = f"## {section} "
        if marker not in text:
            errors.append(f"missing required ContractCandidate section: {section}")

    if "formal code must" not in text.lower():
        errors.append("ContractCandidate register does not state its implementation binding rule")

    if "not a second schema" not in text.lower():
        errors.append("ContractCandidate register must explicitly prohibit a second schema")

    if errors:
        for error in errors:
            print(f"ERROR: {error}")
        return 1

    print("OK: C-01..C-08 ContractCandidate register is structurally complete")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
