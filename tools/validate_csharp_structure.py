#!/usr/bin/env python3
from __future__ import annotations

import argparse
import sys
from pathlib import Path


IGNORED_PARTS = {".git", "bin", "obj"}


def _is_quote_start(text: str, index: int) -> tuple[str, int] | None:
    """Return string state and consumed prefix length for a C# string start."""
    remaining = text[index:]

    if remaining.startswith('"""'):
        return "raw", 3

    for prefix in ('$@"', '@$"'):
        if remaining.startswith(prefix):
            return "verbatim_string", 3

    if remaining.startswith('@\"'):
        return "verbatim_string", 2

    if remaining.startswith('@"'):
        return "verbatim_string", 2

    if remaining.startswith('$"'):
        return "normal_string", 2

    if remaining.startswith('"'):
        return "normal_string", 1

    if remaining.startswith("'"):
        return "char", 1

    return None


def validate_file(path: Path, root: Path) -> list[str]:
    text = path.read_text(encoding="utf-8")
    errors: list[str] = []

    braces = 0
    parens = 0
    brackets = 0
    line = 1
    state = "code"
    raw_delimiter = 0
    line_code: list[str] = []

    def report_negative(kind: str) -> None:
        errors.append(
            f"{path.relative_to(root)}:{line}: negative {kind} depth"
        )

    def finish_line() -> None:
        nonlocal line_code

        cleaned = "".join(line_code).strip()

        if braces == 1:
            suspicious = (
                cleaned.startswith("return ")
                or cleaned.startswith("return;")
                or cleaned.startswith("ArgumentException.")
                or cleaned.startswith("ArgumentNullException.")
                or cleaned.startswith("ObjectDisposedException.")
            )

            if suspicious:
                errors.append(
                    f"{path.relative_to(root)}:{line}: "
                    "suspicious executable statement at class scope"
                )

        line_code = []

    i = 0
    while i < len(text):
        ch = text[i]
        nxt = text[i + 1] if i + 1 < len(text) else ""
        nxt2 = text[i + 2] if i + 2 < len(text) else ""

        if state == "line_comment":
            if ch == "\n":
                finish_line()
                line += 1
                state = "code"
            i += 1
            continue

        if state == "block_comment":
            if ch == "*" and nxt == "/":
                state = "code"
                i += 2
                continue
            if ch == "\n":
                finish_line()
                line += 1
            i += 1
            continue

        if state == "normal_string":
            if ch == "\\":
                i += 2
                continue
            if ch == '"':
                state = "code"
            if ch == "\n":
                line += 1
            i += 1
            continue

        if state == "verbatim_string":
            if ch == '"' and nxt == '"':
                i += 2
                continue
            if ch == '"':
                state = "code"
            if ch == "\n":
                line += 1
            i += 1
            continue

        if state == "raw":
            if raw_delimiter > 0 and text.startswith(
                '"' * raw_delimiter,
                i,
            ):
                i += raw_delimiter
                state = "code"
                raw_delimiter = 0
                continue
            if ch == "\n":
                line += 1
            i += 1
            continue

        if state == "char":
            if ch == "\\":
                i += 2
                continue
            if ch == "'":
                state = "code"
            i += 1
            continue

        # code state -----------------------------------------------------
        if ch == "/" and nxt == "/":
            state = "line_comment"
            i += 2
            continue

        if ch == "/" and nxt == "*":
            state = "block_comment"
            i += 2
            continue

        quote = _is_quote_start(text, i)
        if quote is not None:
            state, consumed = quote
            if state == "raw":
                delimiter = 3
                while i + delimiter < len(text) and text[i + delimiter] == '"':
                    delimiter += 1
                raw_delimiter = delimiter
            i += consumed
            continue

        if ch == "\n":
            finish_line()
            line += 1
            i += 1
            continue

        line_code.append(ch)

        if ch == "{":
            braces += 1
        elif ch == "}":
            braces -= 1
            if braces < 0:
                report_negative("brace")
                braces = 0
        elif ch == "(":
            parens += 1
        elif ch == ")":
            parens -= 1
            if parens < 0:
                report_negative("parenthesis")
                parens = 0
        elif ch == "[":
            brackets += 1
        elif ch == "]":
            brackets -= 1
            if brackets < 0:
                report_negative("bracket")
                brackets = 0

        i += 1

    if line_code:
        finish_line()

    if braces != 0 or parens != 0 or brackets != 0:
        errors.append(
            f"{path.relative_to(root)}: unbalanced delimiters "
            f"brace={braces} paren={parens} bracket={brackets}"
        )

    if state in {"normal_string", "verbatim_string", "raw", "char"}:
        errors.append(
            f"{path.relative_to(root)}: unterminated C# string/char literal"
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
