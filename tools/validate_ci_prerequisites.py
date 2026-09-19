from __future__ import annotations

from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
TOOLS = ROOT / "tools"

REQUIRED_SCRIPTS = (
    "validate_docs.py",
    "validate_solution_layout.py",
    "validate_wpf_shell.py",
    "validate_repository_policy.py",
    "validate_function_bindings.py",
    "validate_page_contracts.py",
    "validate_project_boundaries.py",
    "validate_contract_candidates.py",
    "validate_planning_crosslinks.py",
    "task_gate_check.py",
    "generate_task_admission.py",
    "validate_repository_authorities.py",
    "build_master.py",
    "version_baseline_audit.py",
)


def main() -> int:
    errors: list[str] = []
    for name in REQUIRED_SCRIPTS:
        path = TOOLS / name
        if not path.is_file():
            errors.append(f"missing CI script: {path.relative_to(ROOT)}")

    if errors:
        for error in errors:
            print(f"ERROR: {error}")
        return 1

    print(f"OK: all {len(REQUIRED_SCRIPTS)} CI scripts are present")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
