from __future__ import annotations

import xml.etree.ElementTree as ET
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
SRC = ROOT / "src"

ALLOWED_REFERENCES: dict[str, set[str]] = {
    "Asun.Platform.Contracts": set(),
    "Asun.Platform.Core": {"Asun.Platform.Contracts"},
    "Asun.Platform.Evidence": {"Asun.Platform.Contracts"},
    "Asun.Vision.Contracts": set(),
    "Asun.Vision.Halcon": {"Asun.Vision.Contracts"},
    "Asun.Metrology.Core": {"Asun.Platform.Contracts"},
    "Asun.Device.Contracts": set(),
    "Asun.Device.Impl": {"Asun.Device.Contracts"},
    "Asun.UI.DesignSystem": set(),
    "Asun.UI.Viewports": set(),
    "Asun.Domain.Pcb": set(),
    "Asun.Domain.Hdi": set(),
    "Asun.Domain.Fpc": set(),
    "Asun.Domain.RigidFlex": set(),
    "Asun.Domain.Stencil": set(),
    "Asun.Domain.Spi": set(),
    "Asun.Domain.Aoi": set(),
    "Asun.Domain.Quality": set(),
    "Asun.App.Shell": set(),
}


def project_name(path: Path) -> str:
    return path.stem


def referenced_projects(path: Path) -> set[str]:
    root = ET.parse(path).getroot()
    refs = set()
    for node in root.iter("ProjectReference"):
        include = node.attrib.get("Include", "")
        target = Path(include).as_posix()
        refs.add(Path(target).stem)
    return refs


def main() -> int:
    errors = []

    for project_file in sorted(SRC.glob("*/Asun.*.csproj")):
        name = project_name(project_file)
        if name not in ALLOWED_REFERENCES:
            continue

        actual = referenced_projects(project_file)
        allowed = ALLOWED_REFERENCES[name]
        forbidden = sorted(actual - allowed)
        if forbidden:
            errors.append(
                f"{name}: forbidden ProjectReference(s): {', '.join(forbidden)}"
            )

        missing = sorted(allowed - actual)
        if missing:
            errors.append(
                f"{name}: required ProjectReference(s) missing: {', '.join(missing)}"
            )

        if name == "Asun.App.Shell":
            for forbidden_root in ("Asun.Vision.Halcon", "Asun.Device.Impl"):
                if forbidden_root in actual:
                    errors.append(
                        f"{name}: shell must not reference implementation adapter {forbidden_root}"
                    )

    if errors:
        for error in errors:
            print(f"ERROR: {error}")
        return 1

    print("OK: project dependency direction matches the current scaffold boundary")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
