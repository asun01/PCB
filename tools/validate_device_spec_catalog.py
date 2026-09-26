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

root_matrix = ROOT / "CAPABILITY_MATRIX.md"
if not root_matrix.is_file():
    fail("missing CAPABILITY_MATRIX.md")

template = ROOT / "DEVICE_SPEC_TEMPLATE.md"
if not template.is_file():
    fail("missing DEVICE_SPEC_TEMPLATE.md")

print(f"[device-spec] PASS: {len(DEVICE_DIRS)} device families; required surfaces present.")
