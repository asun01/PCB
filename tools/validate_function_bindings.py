from __future__ import annotations

import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
DOCS = ROOT / "docs"
FS_DIR = DOCS / "12-function-specs"
REGISTRY = DOCS / "09-development-plan" / "DEV-PLAN-008_全量功能代码落点注册表.md"
PERFORMANCE = DOCS / "09-development-plan" / "DEV-PLAN-012_全量PerformanceProfile绑定矩阵.md"
PAGE_DIR = DOCS / "14-page-contracts"

EXPECTED_FS_COUNT = 69
TICK = chr(96)


def clean(value: str) -> str:
    return value.replace(TICK, "").strip()


def fs_id_from_name(path: Path) -> str | None:
    match = re.match(r"FS-(\d{3})_", path.name)
    return f"FS-{match.group(1)}" if match else None


def metadata(text: str, label: str) -> str:
    prefix = f"- {label}："
    for line in text.splitlines():
        if line.startswith(prefix):
            value = line[len(prefix):].strip()
            if value.startswith(TICK) and value.endswith(TICK):
                return value[1:-1]
            return value or "UNVERIFIED"
    return "UNVERIFIED"


def table_map(text: str, first_key: str, value_key: str) -> dict[str, str]:
    result: dict[str, str] = {}
    lines = text.splitlines()
    header_index = None
    headers: list[str] = []

    for index, line in enumerate(lines):
        if not line.startswith("|"):
            continue
        cells = [cell.strip() for cell in line.strip("|").split("|")]
        if first_key in cells and value_key in cells:
            headers = cells
            header_index = index
            break

    if header_index is None:
        return result

    key_index = headers.index(first_key)
    value_index = headers.index(value_key)

    for line in lines[header_index + 1:]:
        if not line.startswith("|") or set(line.replace("|", "").strip()) <= {"-", ":"}:
            continue
        cells = [cell.strip() for cell in line.strip("|").split("|")]
        if len(cells) <= max(key_index, value_index):
            continue
        key = clean(cells[key_index])
        value = clean(cells[value_index])
        if key.startswith("FS-"):
            result[key] = value

    return result


def page_contract_ids() -> set[str]:
    ids = set()
    for path in PAGE_DIR.glob("UI-PCS-*.md"):
        match = re.match(r"(UI-PCS-\d{3})_", path.name)
        if match:
            ids.add(match.group(1))
    return ids


def main() -> int:
    errors: list[str] = []

    registry_text = REGISTRY.read_text(encoding="utf-8")
    performance_text = PERFORMANCE.read_text(encoding="utf-8")

    registry_project = table_map(registry_text, "FS", "Project")
    registry_port = table_map(registry_text, "FS", "Canonical Port")
    registry_page = table_map(registry_text, "FS", "Page")
    registry_wp = table_map(registry_text, "FS", "WorkPackage")
    registry_perf = table_map(registry_text, "FS", "Performance")
    performance_map = table_map(performance_text, "FS", "PerformanceProfileId")

    fs_files = sorted(FS_DIR.glob("FS-*.md"))
    fs_ids = {fs_id_from_name(path) for path in fs_files}
    fs_ids.discard(None)

    if len(fs_files) != EXPECTED_FS_COUNT:
        errors.append(
            f"expected {EXPECTED_FS_COUNT} FunctionSpec files, found {len(fs_files)}"
        )

    if len(fs_ids) != EXPECTED_FS_COUNT:
        errors.append(
            f"expected {EXPECTED_FS_COUNT} unique FunctionSpec IDs, found {len(fs_ids)}"
        )

    all_ids = sorted(fs_ids)
    page_ids = page_contract_ids()

    for fs_id in all_ids:
        path = next((p for p in fs_files if fs_id_from_name(p) == fs_id), None)
        if path is None:
            errors.append(f"{fs_id}: source file not found")
            continue

        text = path.read_text(encoding="utf-8")
        actual_id = metadata(text, "文档 ID")
        if actual_id != fs_id:
            errors.append(f"{fs_id}: metadata ID is {actual_id!r}")

        project = metadata(text, "Project")
        port = metadata(text, "Canonical port")
        page = metadata(text, "UI PageContract")
        work_package = metadata(text, "Primary Work Package")
        perf_gate = metadata(text, "Performance gate")

        for name, value in (
            ("Project", project),
            ("Canonical port", port),
            ("UI PageContract", page),
            ("Primary Work Package", work_package),
            ("Performance gate", perf_gate),
        ):
            if value == "UNVERIFIED":
                errors.append(f"{fs_id}: missing {name}")

        expected_project = registry_project.get(fs_id, "UNVERIFIED")
        expected_port = registry_port.get(fs_id, "UNVERIFIED")
        expected_page = registry_page.get(fs_id, "UNVERIFIED")
        expected_wp = registry_wp.get(fs_id, "UNVERIFIED")
        expected_perf = registry_perf.get(fs_id, "UNVERIFIED")
        explicit_perf = performance_map.get(fs_id, "UNVERIFIED")

        if project != expected_project:
            errors.append(
                f"{fs_id}: project mismatch FS={project!r} registry={expected_project!r}"
            )
        if port != expected_port:
            errors.append(
                f"{fs_id}: port mismatch FS={port!r} registry={expected_port!r}"
            )
        if page != expected_page:
            errors.append(
                f"{fs_id}: page mismatch FS={page!r} registry={expected_page!r}"
            )
        if work_package != expected_wp:
            errors.append(
                f"{fs_id}: work package mismatch FS={work_package!r} registry={expected_wp!r}"
            )

        perf_profile_match = re.search(r"(PF-FS-\d{3})", perf_gate)
        if not perf_profile_match:
            errors.append(f"{fs_id}: Performance gate does not contain PF-FS-xxx")
        else:
            actual_perf = perf_profile_match.group(1)
            if actual_perf != clean(expected_perf):
                errors.append(
                    f"{fs_id}: PerformanceProfile mismatch FS={actual_perf!r} registry={expected_perf!r}"
                )
            if clean(explicit_perf) != actual_perf:
                errors.append(
                    f"{fs_id}: PerformanceProfile mismatch matrix={explicit_perf!r} FS={actual_perf!r}"
                )

        page_id_match = re.fullmatch(r"UI-PCS-\d{3}", clean(page))
        if page_id_match and page_id_match.group(0) not in page_ids:
            errors.append(f"{fs_id}: PageContract file missing for {page}")

    if set(registry_project) != set(all_ids):
        errors.append("DEV-PLAN-008 FS set does not exactly match FunctionSpec file set")
    if set(performance_map) != set(all_ids):
        errors.append("DEV-PLAN-012 FS set does not exactly match FunctionSpec file set")

    if errors:
        for error in errors:
            print(f"ERROR: {error}")
        return 1

    print(
        f"OK: {EXPECTED_FS_COUNT} FunctionSpecs have consistent "
        "Project/Port/Page/WorkPackage/Performance bindings"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
