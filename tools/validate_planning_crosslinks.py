from __future__ import annotations

import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
DOCS = ROOT / "docs"
REGISTRY = DOCS / "09-development-plan" / "DEV-PLAN-008_全量功能代码落点注册表.md"
DAG = DOCS / "09-development-plan" / "DEV-PLAN-014_全量研发工作包与并行依赖DAG.md"
MATRIX = DOCS / "09-development-plan" / "DEV-PLAN-011_全量功能确定性实施矩阵.md"
PERFORMANCE = DOCS / "09-development-plan" / "DEV-PLAN-012_全量PerformanceProfile绑定矩阵.md"

EXPECTED_FS = {f"FS-{i:03d}" for i in range(1, 70)}


def clean(value: str) -> str:
    return value.replace(chr(96), "").strip()


def table_map(text: str, header_keys: tuple[str, str]) -> dict[str, str]:
    first_key, value_key = header_keys
    lines = text.splitlines()
    for index, line in enumerate(lines):
        if not line.startswith("|"):
            continue
        headers = [cell.strip() for cell in line.strip("|").split("|")]
        if first_key not in headers or value_key not in headers:
            continue

        key_index = headers.index(first_key)
        value_index = headers.index(value_key)
        result: dict[str, str] = {}

        for row in lines[index + 1:]:
            if not row.startswith("|"):
                if result:
                    break
                continue
            cells = [cell.strip() for cell in row.strip("|").split("|")]
            if len(cells) <= max(key_index, value_index):
                continue
            if set(row.replace("|", "").strip()) <= {"-", ":"}:
                continue

            key = clean(cells[key_index])
            value = clean(cells[value_index])
            if re.fullmatch(r"FS-\d{3}", key):
                result[key] = value

        return result

    return {}


def compare_set(label: str, actual: set[str], errors: list[str]) -> None:
    missing = sorted(EXPECTED_FS - actual)
    extra = sorted(actual - EXPECTED_FS)
    if missing:
        errors.append(f"{label}: missing FS IDs: {', '.join(missing)}")
    if extra:
        errors.append(f"{label}: unexpected FS IDs: {', '.join(extra)}")


def main() -> int:
    errors: list[str] = []

    for path in (REGISTRY, DAG, MATRIX, PERFORMANCE):
        if not path.exists():
            errors.append(f"missing required planning document: {path.relative_to(ROOT)}")

    if errors:
        for error in errors:
            print(f"ERROR: {error}")
        return 1

    registry_wp = table_map(
        REGISTRY.read_text(encoding="utf-8"),
        ("FS", "WorkPackage"),
    )
    dag_wp = table_map(
        DAG.read_text(encoding="utf-8"),
        ("FS", "Primary Work Package"),
    )
    matrix_capability = table_map(
        MATRIX.read_text(encoding="utf-8"),
        ("FS", "Capability"),
    )
    performance_profile = table_map(
        PERFORMANCE.read_text(encoding="utf-8"),
        ("FS", "PerformanceProfileId"),
    )

    sources = {
        "DEV-PLAN-008": registry_wp,
        "DEV-PLAN-014": dag_wp,
        "DEV-PLAN-011": matrix_capability,
        "DEV-PLAN-012": performance_profile,
    }

    for label, mapping in sources.items():
        compare_set(label, set(mapping), errors)

    if set(registry_wp) == EXPECTED_FS and set(dag_wp) == EXPECTED_FS:
        mismatches = [
            fs_id
            for fs_id in sorted(EXPECTED_FS)
            if clean(registry_wp[fs_id]) != clean(dag_wp[fs_id])
        ]
        if mismatches:
            details = ", ".join(
                f"{fs_id}={registry_wp[fs_id]!r}/{dag_wp[fs_id]!r}"
                for fs_id in mismatches
            )
            errors.append(
                "WorkPackage mismatch between DEV-PLAN-008 and DEV-PLAN-014: "
                + details
            )

    defined_wps = set()
    for line in DAG.read_text(encoding="utf-8").splitlines():
        if not line.startswith("|"):
            continue
        cells = [clean(cell) for cell in line.strip("|").split("|")]
        if cells and re.fullmatch(r"WP-\d{2}", cells[0]):
            defined_wps.add(cells[0])

    referenced_wps = {
        clean(value)
        for value in registry_wp.values()
        if clean(value).startswith("WP-")
    } | {
        clean(value)
        for value in dag_wp.values()
        if clean(value).startswith("WP-")
    }

    invalid_wps = sorted(referenced_wps - defined_wps)
    if invalid_wps:
        errors.append(
            "Planning documents reference undefined WorkPackages: "
            + ", ".join(invalid_wps)
        )

    if errors:
        for error in errors:
            print(f"ERROR: {error}")
        return 1

    print(
        "OK: 69 FS entries are cross-consistent across "
        "DEV-PLAN-008/011/012/014; WorkPackage bindings match."
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
