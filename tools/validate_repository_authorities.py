from __future__ import annotations

import hashlib
import re
import subprocess
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
AUTHORITY_DOC = ROOT / "docs" / "00-baseline" / "REQUIRED-REPOSITORY-AUTHORITIES.md"

EXACT_PATH_RE = re.compile(r"[A-Za-z0-9_./\\-]+\.(?:md|json)$")


def git_head() -> str:
    try:
        return subprocess.check_output(
            ["git", "rev-parse", "HEAD"],
            cwd=ROOT,
            text=True,
            stderr=subprocess.DEVNULL,
        ).strip()
    except (OSError, subprocess.CalledProcessError):
        return "UNAVAILABLE"


def git_blob_sha(path: Path) -> str:
    relative = path.relative_to(ROOT).as_posix()
    try:
        output = subprocess.check_output(
            ["git", "ls-tree", "HEAD", "--", relative],
            cwd=ROOT,
            text=True,
            stderr=subprocess.DEVNULL,
        ).strip()
    except (OSError, subprocess.CalledProcessError):
        return "UNAVAILABLE"

    parts = output.split()
    return parts[2] if len(parts) >= 3 else "UNAVAILABLE"


def source_candidates(source: str) -> list[str]:
    raw = source.strip().replace("\\", "/")
    candidates: list[str] = []

    for token in re.findall(r"`([^`]+)`", raw):
        if EXACT_PATH_RE.fullmatch(token.strip()):
            candidates.append(token.strip())

    if EXACT_PATH_RE.fullmatch(raw):
        candidates.append(raw)

    # Keep only repository-relative paths. This avoids treating prose such as
    # "actual repository path as authoritative" as a fake file location.
    unique: list[str] = []
    for candidate in candidates:
        if candidate not in unique:
            unique.append(candidate)
    return unique


def parse_authorities(text: str) -> list[tuple[str, str]]:
    authorities: list[tuple[str, str]] = []

    for line in text.splitlines():
        if not line.startswith("|"):
            continue
        cells = [cell.strip() for cell in line.strip("|").split("|")]
        if len(cells) < 2 or cells[0] in {"标识", "Identifier"}:
            continue
        if set(line.replace("|", "").strip()) <= {"-", ":"}:
            continue
        if re.fullmatch(r"(DEV-[A-Z]+-\d{3}|contracts/.+)", cells[0]):
            authorities.append((cells[0], cells[1]))

    return authorities


def main() -> int:
    if not AUTHORITY_DOC.exists():
        print(f"ERROR: missing authority register: {AUTHORITY_DOC.relative_to(ROOT)}")
        return 1

    text = AUTHORITY_DOC.read_text(encoding="utf-8")
    authorities = parse_authorities(text)

    if not authorities:
        print("ERROR: authority register contains no parseable authority rows")
        return 1

    print(f"Authority register: {AUTHORITY_DOC.relative_to(ROOT)}")
    print(f"Repository HEAD: {git_head()}")
    print(f"Registered authority rows: {len(authorities)}")

    unresolved = 0
    resolved = 0
    prose_only = 0

    for authority_id, source in authorities:
        candidates = source_candidates(source)
        if not candidates:
            prose_only += 1
            print(f"- {authority_id}: SOURCE_REQUIRES_PATH_RESOLUTION | {source}")
            continue

        found = []
        for candidate in candidates:
            candidate_path = ROOT / candidate
            if candidate_path.is_file():
                blob_sha = git_blob_sha(candidate_path)
                sha256 = hashlib.sha256(candidate_path.read_bytes()).hexdigest()
                found.append((candidate, blob_sha, sha256))

        if not found:
            unresolved += 1
            print(
                f"- {authority_id}: MISSING | expected={', '.join(candidates)}"
            )
            continue

        resolved += 1
        for candidate, blob_sha, sha256 in found:
            print(
                f"- {authority_id}: RESOLVED | path={candidate} "
                f"git_blob={blob_sha} sha256={sha256}"
            )

    print(
        "Summary: "
        f"resolved={resolved}, unresolved={unresolved}, "
        f"path_resolution_required={prose_only}"
    )
    print(
        "Policy: this tool resolves and fingerprints existing repository "
        "authorities only. It never creates, substitutes, or downgrades "
        "authority, schema, state, owner, or acceptance semantics."
    )

    # Missing authorities are an expected governance gate, not a tooling
    # defect. The gate itself remains authoritative in REQUIRED-REPOSITORY-AUTHORITIES.md.
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
