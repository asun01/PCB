from pathlib import Path
import re
import sys

ROOT=Path(__file__).resolve().parents[1]
XAML=ROOT/"src/Asun.App.Shell/Bootstrap/MainWindow.xaml"
CODE=ROOT/"src/Asun.App.Shell/Bootstrap/MainWindow.xaml.cs"

EVENT_PATTERN=re.compile(
    r'(?:Click|Closed|SelectionChanged|SizeChanged|MouseDown|MouseMove|MouseUp|MouseWheel|KeyDown)="([A-Za-z0-9_]+)"'
)
METHOD_PATTERN=re.compile(r'private (?:async )?void ([A-Za-z0-9_]+)\s*\(')

REQUIRED_PROJECTION_SUBSCRIPTIONS=(
    "_clientProjection.Changed+=",
    "_clientProjection.ExecutionChanged+=",
    "_clientProjection.RoiPulseChanged+=",
)

FORBIDDEN_LEGACY_SUBSCRIPTIONS=(
    "_client.ProductionChanged+=",
    "_workspaceRuntime.Changed+=",
)

REQUIRED_COMMAND_FACADES=(
    "ClientInspectionExecutionCommandRuntime.",
    "ClientProgramCommandRuntime.",
    "ClientQualityCommandRuntime.",
    "ClientResultsCommandRuntime.",
)


def main() -> int:
    errors: list[str] = []

    if not XAML.exists():
        errors.append(f"missing XAML: {XAML}")
    if not CODE.exists():
        errors.append(f"missing code-behind: {CODE}")

    if errors:
        for error in errors:
            print("ERROR:", error)
        return 1

    xaml=XAML.read_text(encoding="utf-8")
    code=CODE.read_text(encoding="utf-8")

    handlers=EVENT_PATTERN.findall(xaml)
    methods=METHOD_PATTERN.findall(code)

    method_counts={name: methods.count(name) for name in set(methods)}
    for handler in sorted(set(handlers)):
        count=method_counts.get(handler,0)
        if count!=1:
            errors.append(
                f"XAML handler '{handler}' must map to exactly one private void method; found {count}"
            )

    for subscription in REQUIRED_PROJECTION_SUBSCRIPTIONS:
        if subscription not in code:
            errors.append(f"missing required projection subscription: {subscription}")

    for subscription in FORBIDDEN_LEGACY_SUBSCRIPTIONS:
        if subscription in code:
            errors.append(f"legacy direct subscription must not exist: {subscription}")

    for facade in REQUIRED_COMMAND_FACADES:
        if facade not in code:
            errors.append(f"missing command facade usage: {facade}")

    open_braces=code.count("{")
    close_braces=code.count("}")
    if open_braces!=close_braces:
        errors.append(
            f"code-behind braces are unbalanced: open={open_braces} close={close_braces}"
        )

    bad_tokens=("TODO","NotImplementedException")
    for token in bad_tokens:
        if token in code:
            errors.append(f"forbidden token present in code-behind: {token}")

    print(
        f"handlers={len(set(handlers))} "
        f"projection_subscriptions={sum(item in code for item in REQUIRED_PROJECTION_SUBSCRIPTIONS)} "
        f"legacy_subscriptions={sum(item in code for item in FORBIDDEN_LEGACY_SUBSCRIPTIONS)} "
        f"commands={sum(item in code for item in REQUIRED_COMMAND_FACADES)}"
    )

    if errors:
        for error in errors:
            print("ERROR:",error)
        return 1

    print("Client shell binding contract: OK")
    return 0


if __name__=="__main__":
    raise SystemExit(main())
