from pathlib import Path
import re

ROOT = Path(__file__).resolve().parents[1]
PAGE_DIR = ROOT / "docs" / "14-page-contracts"

REQUIRED_MARKERS = (
    "PageContract",
    "Commands",
    "Queries",
    "八态",
    "AutomationId",
)


def main() -> int:
    errors = []
    pages = sorted(PAGE_DIR.glob("UI-PCS-*.md"))

    for path in pages:
        text = path.read_text(encoding="utf-8")
        match = re.match(r"(UI-PCS-\d{3})_", path.name)
        if not match:
            errors.append(f"invalid PageContract filename: {path.relative_to(ROOT)}")
            continue

        page_id = match.group(1)
        if page_id not in text:
            errors.append(f"{page_id}: document does not self-identify its PageContract ID")

        missing = [marker for marker in REQUIRED_MARKERS if marker not in text]
        for marker in missing:
            errors.append(f"{page_id}: missing required PageContract marker: {marker}")

    ids = [
        re.match(r"(UI-PCS-\d{3})_", path.name).group(1)
        for path in pages
        if re.match(r"(UI-PCS-\d{3})_", path.name)
    ]
    duplicates = {page_id for page_id in ids if ids.count(page_id) > 1}

    for page_id in sorted(duplicates):
        errors.append(f"duplicate PageContract file ID: {page_id}")

    if errors:
        for error in errors:
            print(f"ERROR: {error}")
        return 1

    print(f"OK: {len(pages)} PageContract files passed structural validation")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
