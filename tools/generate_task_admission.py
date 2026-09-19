from __future__ import annotations

import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
OPEN_GATES = ROOT / "docs" / "00-baseline" / "OPEN-GATES.md"
FS_DIR = ROOT / "docs" / "12-function-specs"


def open_gate_ids(text: str) -> set[str]:
    result: set[str] = set()
    for line in text.splitlines():
        if not line.startswith("|"):
            continue
        cells = [cell.strip() for cell in line.strip("|").split("|")]
        if not cells or cells[0] in {"Gate", "---"}:
            continue
        if cells[0].startswith("GATE-"):
            result.add(cells[0])
        elif cells[0].startswith("C-01..C-08"):
            result.add("C-01..C-08")
    return result


def metadata(text: str, label: str) -> str:
    prefix = f"- {label}："
    for line in text.splitlines():
        if line.startswith(prefix):
            value = line[len(prefix):].strip()
            marker = chr(96)
            if value.startswith(marker) and value.endswith(marker):
                return value[1:-1]
            return value or "UNVERIFIED"
    return "UNVERIFIED"


def main() -> int:
    if not OPEN_GATES.exists():
        print("ERROR: missing OPEN-GATES.md")
        return 1

    gate_text = OPEN_GATES.read_text(encoding="utf-8")
    gates = sorted(open_gate_ids(gate_text))

    global_authority_block = "GATE-001" in gates or "C-01..C-08" in gates

    items = []
    for path in sorted(FS_DIR.glob("FS-*.md")):
        text = path.read_text(encoding="utf-8")
        fs_id = metadata(text, "文档 ID")
        wp = metadata(text, "Primary Work Package")
        project = metadata(text, "Project")

        if fs_id == "UNVERIFIED":
            admission = "BLOCKED_MISSING_IDENTITY"
            reason = ["FunctionSpec document ID is missing"]
        elif global_authority_block:
            admission = "PREPARATION_ALLOWED_PRODUCTION_BLOCKED"
            reason = [
                "global repository authority/schema gates remain open; "
                "only non-authoritative preparation may continue"
            ]
        else:
            admission = "PRODUCTION_REVIEW_REQUIRED"
            reason = []

        items.append(
            {
                "function_spec": fs_id,
                "work_package": wp,
                "project": project,
                "admission": admission,
                "reasons": reason,
                "source": path.relative_to(ROOT).as_posix(),
            }
        )

    payload = {
        "repository": "asun01/PCB",
        "open_gates": gates,
        "production_code_policy": (
            "Never invent missing contracts, schema, state, owner, API, "
            "thresholds, acceptance or hardware behavior."
        ),
        "automation_policy": (
            "Continue every non-authoritative preparation task automatically; "
            "do not wait for a human when a task is not blocked by an authority/environment gate."
        ),
        "items": items,
    }

    print(json.dumps(payload, ensure_ascii=False, indent=2))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
