#!/usr/bin/env python3
from __future__ import annotations

import argparse
import sys
from pathlib import Path


IGNORED_PARTS = {".git", "bin", "obj"}


def strip_csharp_line(line: str) -> str:
    result: list[str] = []
    state = "code"
    i = 0

    while i < len(line):
        ch = line[i]
        nxt = line[i + 1] if i + 1 < len(line) else ""

        if state == "line":
            break

        if state == "block":
            if ch == "*" and nxt == "/":
                state = "code"
                i += 2
                continue
            i += 1
            continue

        if state == "normal_string":
            if ch == "\":
                i += 2
                continue
            if ch == '"':
                state = "code"
            i += 1
            continue

        if state == "verbatim_string":
            if ch == '"' and nxt == '"':
                i += 2
                continue
            if ch == '"':
                state = "code"
            i += 1
            continue

        if state == "char":
            if ch == "\":
                i += 2
                continue
            if ch == "'":
                state = "code"
            i += 1
            continue

        if ch == "/" and nxt == "/":
            break

        if ch == "/" and nxt == "*":
            state = "block"
            i += 2
            continue

        if ch == "@" and nxt == '"':
            state = "verbatim_string"
            i += 2
            continue

        if ch == '"':
            state = "normal_string"
            i += 1
            continue

        if ch == "'":
            state = "char"
            i += 1
            continue

        result.append(ch)
        i += 1

    return "".join(result)


def validate_file(path: Path, root: Path) -> list[str]:
    text = path.read_text(encoding="utf-8")
    lines = text.splitlines()

    braces = 0
    parens = 0
    brackets = 0
    errors: list[str] = []
    block_state = False

    for number, raw in enumerate(lines, 1):
        line = raw
        # Keep block-comment state across lines while stripping the rest.
        if block_state:
            end = line.find("*/")
            if end < 0:
                continue
            block_state = False
            line = line[end + 2 :]

        cleaned_parts: list[str] = []
        cursor = 0

        while cursor < len(line):
            start = line.find("/*", cursor)
            if start < 0:
                cleaned_parts.append(line[cursor:])
                break

            cleaned_parts.append(line[cursor:start])
            end = line.find("*/", start + 2)
            if end < 0:
                block_state = True
                break

            cursor = end + 2

        cleaned = "".join(cleaned_parts)
        cleaned = strip_csharp_line(cleaned).strip()

        for ch in cleaned:
            if ch == "{":
                braces += 1
            elif ch == "}":
                braces -= 1
                if braces < 0:
                    errors.append(
                        f"{path.relative_to(root)}:{number}: negative brace depth"
                    )
                    braces = 0
            elif ch == "(":
                parens += 1
            elif ch == ")":
                parens -= 1
                if parens < 0:
                    errors.append(
                        f"{path.relative_to(root)}:{number}: negative parenthesis depth"
                    )
                    parens = 0
            elif ch == "[":
                brackets += 1
            elif ch == "]":
                brackets -= 1
                if brackets < 0:
                    errors.append(
                        f"{path.relative_to(root)}:{number}: negative bracket depth"
                    )
                    brackets = 0

        # A normal namespace/class body has brace depth 1 at class scope.
        # Standalone executable statements at that depth are almost always
        # malformed source accidentally left outside a method.
        if braces == 1:
            suspicious = (
                cleaned.startswith("return ")
                or cleaned.startswith("ArgumentException.")
                or cleaned.startswith("ArgumentNullException.")
                or cleaned.startswith("ObjectDisposedException.")
            )
            if suspicious:
                errors.append(
                    f"{path.relative_to(root)}:{number}: suspicious executable statement at class scope"
                )

    if braces != 0 or parens != 0 or brackets != 0:
        errors.append(
            f"{path.relative_to(root)}: unbalanced delimiters "
            f"brace={braces} paren={parens} bracket={brackets}"
        )

    return errors


def collect_files(root: Path) -> list[Path]:
    return sorted(
        path
        for path in root.rglob("*.cs")
        if not any(part in IGNORED_PARTS for part in path.parts)
    )


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("root", nargs="?", default=".")
    args = parser.parse_args()

    root = Path(args.root).resolve()
    files = collect_files(root)
    errors: list[str] = []

    for path in files:
        errors.extend(validate_file(path, root))

    print(f"Validated {len(files)} C# source files.")

    if errors:
        for error in errors:
            print(f"ERROR: {error}", file=sys.stderr)
        return 1

    print("C# source structure validation passed.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
