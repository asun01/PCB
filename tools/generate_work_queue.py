from __future__ import annotations

import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
DOCS = ROOT / "docs"
FS_DIR = DOCS / "12-function-specs"


def extract_inline_field(text: str, key: str) -> str:
    key_lower = key.lower()
    for line in text.splitlines():
        stripped = line.strip()
        if not stripped.startswith("|"):
            continue
        parts = [part.strip() for part in stripped.strip("|").split("|")]
        if len(parts) >= 2 and parts[0].lower() == key_lower:
            return parts[1]
    return "UNVERIFIED"


def extract_metadata(text: str, label: str) -> str:
    prefix = f"- {label}："
    for line in text.splitlines():
        if line.startswith(prefix):
            value = line[len(prefix):].strip()
            marker = chr(96)
            if value.startswith(marker) and value.endswith(marker):
                return value[1:-1]
            return value or "UNVERIFIED"
    return "UNVERIFIED"


def parse_function_spec(path: Path) -> dict[str, str]:
    text = path.read_text(encoding="utf-8")
    return {
        "id": extract_metadata(text, "文档 ID"),
        "name": path.stem,
        "status": extract_metadata(text, "状态"),
        "project": extract_inline_field(text, "Project"),
        "canonical_port": extract_inline_field(text, "Canonical port"),
        "page_contract": extract_inline_field(text, "UI PageContract"),
        "primary_work_package": extract_inline_field(text, "Primary Work Package"),
        "algorithm_baseline": extract_inline_field(text, "Algorithm baseline"),
        "performance_gate": extract_inline_field(text, "Performance gate"),
        "source": path.relative_to(ROOT).as_posix(),
    }


def main() -> int:
    items = [parse_function_spec(path) for path in sorted(FS_DIR.glob("FS-*.md"))]
    missing_identity = [item for item in items if item["id"] == "UNVERIFIED"]
    payload = {
        "repository": "asun01/PCB",
        "function_spec_count": len(items),
        "items": items,
        "missing_identity": missing_identity,
        "policy": (
            "Document-derived reporting only. This tool does not invent "
            "contracts, APIs, owners, thresholds, states, or acceptance values."
        ),
    }
    print(json.dumps(payload, ensure_ascii=False, indent=2))
    return 1 if missing_identity else 0


if __name__ == "__main__":
    raise SystemExit(main())
