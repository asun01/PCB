from pathlib import Path
import re
import sys
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]
SOLUTION = ROOT / "AsunVision.slnx"
PROPS = ROOT / "Directory.Build.props"
TARGET_PROJECT_DIR = ROOT / "src"

FORBIDDEN_IMPLEMENTATION_TERMS = (
    "AsunImage",
)

ALLOWED_TARGETS = {"net10.0", "net10.0-windows"}

def fail(errors, message):
    errors.append(message)

def read_text(path: Path) -> str:
    return path.read_text(encoding="utf-8")

def validate_solution(errors):
    if not SOLUTION.exists():
        fail(errors, f"missing solution: {SOLUTION.relative_to(ROOT)}")
        return

    try:
        root = ET.parse(SOLUTION).getroot()
    except ET.ParseError as exc:
        fail(errors, f"invalid solution XML: {exc}")
        return

    projects = [
        node.attrib.get("Path", "").replace("\\", "/")
        for node in root.findall("Project")
    ]

    if not projects:
        fail(errors, "solution contains no Project entries")
        return

    seen = set()
    for project_path in projects:
        if not project_path:
            fail(errors, "solution contains a Project entry without Path")
            continue
        if project_path in seen:
            fail(errors, f"duplicate solution project: {project_path}")
        seen.add(project_path)

        full_path = ROOT / project_path
        if not full_path.is_file():
            fail(errors, f"solution project missing: {project_path}")
            continue

        try:
            project_xml = ET.parse(full_path).getroot()
        except ET.ParseError as exc:
            fail(errors, f"invalid project XML: {project_path}: {exc}")
            continue

        if project_xml.tag != "Project" or project_xml.attrib.get("Sdk") != "Microsoft.NET.Sdk":
            fail(errors, f"unexpected project SDK: {project_path}")

        project_name = full_path.stem
        parent_name = full_path.parent.name
        if project_name != parent_name:
            fail(
                errors,
                f"project/namespace directory mismatch: {project_path} "
                f"(project={project_name}, directory={parent_name})",
            )

        properties = {}
        for group in project_xml.findall("PropertyGroup"):
            for child in list(group):
                properties[child.tag] = (child.text or "").strip()

        target = properties.get("TargetFramework")
        if target not in ALLOWED_TARGETS:
            fail(errors, f"unexpected TargetFramework in {project_path}: {target!r}")

        use_wpf = properties.get("UseWPF", "").lower() == "true"
        is_app = properties.get("OutputType") == "WinExe"
        if parent_name == "Asun.App.Shell":
            if target != "net10.0-windows" or not use_wpf or not is_app:
                fail(errors, f"Asun.App.Shell must remain net10.0-windows/WPF/WinExe: {project_path}")
        elif use_wpf or is_app:
            fail(errors, f"non-shell project must remain a library project: {project_path}")

        project_text = read_text(full_path)
        for term in ("AsunImage", "HALCON", "Halcon", "DevExpress"):
            if term in project_text:
                fail(errors, f"vendor dependency text is not allowed in scaffold project file {project_path}: {term}")

def validate_shared_build_policy(errors):
    if not PROPS.exists():
        fail(errors, "missing Directory.Build.props")
        return

    text = read_text(PROPS)
    required = {
        "<Nullable>enable</Nullable>": "nullable",
        "<ImplicitUsings>enable</ImplicitUsings>": "implicit usings",
        "<Deterministic>true</Deterministic>": "deterministic build",
        "<TreatWarningsAsErrors>true</TreatWarningsAsErrors>": "warnings as errors",
        "<GenerateDocumentationFile>true</GenerateDocumentationFile>": "documentation file generation",
    }
    for marker, label in required.items():
        if marker not in text:
            fail(errors, f"Directory.Build.props missing required policy: {label}")

def validate_source_exclusions(errors):
    excluded_roots = {
        ".git",
        "bin",
        "obj",
        ".vs",
    }
    extensions = {".cs", ".xaml", ".csproj", ".props", ".targets", ".json"}

    for path in ROOT.rglob("*"):
        if not path.is_file() or path.suffix.lower() not in extensions:
            continue
        relative_parts = path.relative_to(ROOT).parts
        if any(part in excluded_roots for part in relative_parts):
            continue
        if "src" not in relative_parts:
            continue

        text = read_text(path)
        for term in FORBIDDEN_IMPLEMENTATION_TERMS:
            if term in text:
                fail(errors, f"forbidden implementation dependency {term!r}: {path.relative_to(ROOT)}")

def validate_no_latest_package_versions(errors):
    for path in ROOT.rglob("*.csproj"):
        if any(part in {"bin", "obj", ".git"} for part in path.relative_to(ROOT).parts):
            continue
        text = read_text(path)
        if re.search(r'(?i)\\b(?:Version|PackageReferenceVersion)\\s*=\\s*["\\']latest["\\']', text):
            fail(errors, f"floating package version 'latest' is forbidden: {path.relative_to(ROOT)}")

def main():
    errors = []
    validate_solution(errors)
    validate_shared_build_policy(errors)
    validate_source_exclusions(errors)
    validate_no_latest_package_versions(errors)

    if errors:
        for error in errors:
            print(f"ERROR: {error}")
        return 1

    project_count = sum(1 for _ in SOLUTION.read_text(encoding="utf-8").split('<Project Path="')) - 1
    print(f"OK: repository policy gate passed ({project_count} solution projects)")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
