#!/usr/bin/env python3
"""Validate the required device inspection specification hierarchy.

This is a structural/documentation validator. It does not qualify hardware,
algorithms, measurement accuracy, or vendor SDK behavior.
"""
from pathlib import Path
import re
import sys

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
        for number in ids:
            feature_id = f"03D-SPI-F{number}"
            matches = [
                path for path in detail_dir.glob(f"F{number}-*.md")
                if feature_id in path.read_text(encoding="utf-8")
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

root_matrix = ROOT / "CAPABILITY_MATRIX.md"
if not root_matrix.is_file():
    fail("missing CAPABILITY_MATRIX.md")

template = ROOT / "DEVICE_SPEC_TEMPLATE.md"
if not template.is_file():
    fail("missing DEVICE_SPEC_TEMPLATE.md")

print(f"[device-spec] PASS: {len(DEVICE_DIRS)} device families; required surfaces present.")
