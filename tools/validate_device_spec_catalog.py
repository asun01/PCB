#!/usr/bin/env python3
"""Validate the required device inspection specification hierarchy.

This is a structural/documentation validator. It does not qualify hardware,
algorithms, measurement accuracy, or vendor SDK behavior.
"""
from pathlib import Path
import re

ROOT = Path(__file__).resolve().parents[1] / "docs" / "14-device-specs"
DEVICE_DIRS = (
    "02d-aoi",
    "03d-aoi",
    "03d-spi",
    "02d-xray",
    "03d-axi-ct",
    "metrology",
    "ict",
    "fct",
    "pcb-bare-board",
)
REQUIRED = (
    "README.md",
    "DEVICE_SPEC.md",
    "CAPABILITIES.md",
    "FEATURES.md",
    "MEASUREMENTS.md",
    "QUALITY.md",
    "UI_WORKFLOW.md",
    "VENDOR_ADAPTER.md",
)
SPI_FEATURE_REQUIRED_HEADINGS = (
    "## Purpose",
    "## Contract",
    "## Decision semantics",
    "## Parameters",
    "## Boundary and failure cases",
    "## Quality / Evidence / Replay",
    "## UI / Program / Recipe",
    "## Authority gates",
)

def fail(message: str) -> None:
    print(f"[device-spec] FAIL: {message}")
    raise SystemExit(1)

if not ROOT.is_dir():
    fail(f"missing catalog root: {ROOT}")

for slug in DEVICE_DIRS:
    directory = ROOT / slug
    if not directory.is_dir():
        fail(f"missing device directory: {slug}")
    for name in REQUIRED:
        path = directory / name
        if not path.is_file() or not path.read_text(encoding="utf-8").strip():
            fail(f"missing or empty {slug}/{name}")

    features = (directory / "FEATURES.md").read_text(encoding="utf-8")
    ids = re.findall(rf"{re.escape(slug.upper())}-F(\d{{3}})", features)
    if not ids:
        fail(f"no normalized feature IDs in {slug}/FEATURES.md")
    if len(ids) != len(set(ids)):
        fail(f"duplicate normalized feature IDs in {slug}/FEATURES.md")

    if slug == "03d-spi":
        detail_dir = directory / "inspection-features"
        if not detail_dir.is_dir():
            fail("03d-spi missing inspection-features directory")
        registered_numbers = set(ids)
        detail_files = sorted(detail_dir.glob("F[0-9][0-9][0-9]-*.md"))
        for path in detail_files:
            match = re.match(r"F(\d{3})-", path.name)
            if not match:
                continue
            number = match.group(1)
            if number not in registered_numbers:
                fail(
                    f"unregistered 03D-SPI detailed specification: {path.name}"
                )

        for number in ids:
            feature_id = f"03D-SPI-F{number}"
            matches = [
                path for path in detail_files
                if path.name.startswith(f"F{number}-")
                and feature_id in path.read_text(encoding="utf-8")
            ]
            if len(matches) != 1:
                fail(
                    f"{feature_id} must have exactly one detailed specification "
                    f"under 03d-spi/inspection-features"
                )
            content = matches[0].read_text(encoding="utf-8")
            for heading in SPI_FEATURE_REQUIRED_HEADINGS:
                if heading not in content:
                    fail(f"{feature_id} missing required section: {heading}")

        for name in (
            "FEATURE_EXECUTION_CONTRACT.md",
            "FEATURE_TRACEABILITY.md",
            "PARAMETER_AUTHORITY.md",
            "QUALIFICATION_GATES.md",
        ):
            path = directory / name
            if not path.is_file() or not path.read_text(encoding="utf-8").strip():
                fail(f"03d-spi missing required golden-spec surface: {name}")

MATRIX_DEVICES = (
    "2D AOI", "3D AOI", "3D SPI", "2D X-Ray", "3D AXI/CT",
    "Metrology", "ICT", "FCT", "Bare PCB",
)

def validate_capability_matrix(path: Path) -> None:
    lines = [line.strip() for line in path.read_text(encoding="utf-8").splitlines() if line.strip()]
    header_index = next((i for i, line in enumerate(lines) if line.startswith("| Capability |")), None)
    if header_index is None or header_index + 1 >= len(lines):
        fail("CAPABILITY_MATRIX.md missing the canonical table header")
    header = [cell.strip() for cell in lines[header_index].strip("|").split("|")]
    expected = ["Capability", *MATRIX_DEVICES]
    if header != expected:
        fail("CAPABILITY_MATRIX.md has an unexpected device column set or order")
    separator = [cell.strip() for cell in lines[header_index + 1].strip("|").split("|")]
    if len(separator) != len(expected) or any(not re.fullmatch(r":?-{3,}:?", cell) for cell in separator):
        fail("CAPABILITY_MATRIX.md has an invalid Markdown separator row")
    rows = lines[header_index + 2:]
    if not rows:
        fail("CAPABILITY_MATRIX.md has no capability rows")
    for row in rows:
        cells = [cell.strip() for cell in row.strip("|").split("|")]
        if len(cells) != len(expected):
            fail("CAPABILITY_MATRIX.md contains a row with the wrong column count")
        if not cells[0]:
            fail("CAPABILITY_MATRIX.md contains an empty capability name")
        if any(value not in {"Candidate", "-"} for value in cells[1:]):
            fail("CAPABILITY_MATRIX.md contains a non-structural qualification value")

root_matrix = ROOT / "CAPABILITY_MATRIX.md"
if not root_matrix.is_file():
    fail("missing CAPABILITY_MATRIX.md")
validate_capability_matrix(root_matrix)

template = ROOT / "DEVICE_SPEC_TEMPLATE.md"
if not template.is_file():
    fail("missing DEVICE_SPEC_TEMPLATE.md")

print(f"[device-spec] PASS: {len(DEVICE_DIRS)} device families; required surfaces present.")
