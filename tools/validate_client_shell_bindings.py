from pathlib import Path
import re
import sys

ROOT = Path(__file__).resolve().parents[1]
XAML = ROOT / "src/Asun.App.Shell/Bootstrap/MainWindow.xaml"
CODE = ROOT / "src/Asun.App.Shell/Bootstrap/MainWindow.xaml.cs"

# Keep this list explicit and framework-neutral. The validator is intended to
# catch accidental event-handler drift in the shell without trying to model
# WPF's full XAML compiler.
EVENT_PATTERN = re.compile(
    r'\b(?:Click|Closed|SelectionChanged|SizeChanged|MouseDown|MouseMove|MouseUp|'
    r'MouseWheel|KeyDown|KeyUp|Loaded|Unloaded|TextChanged|Checked|Unchecked|'
    r'PreviewMouseDown|PreviewMouseMove|PreviewMouseUp|PreviewMouseWheel|'
    r'Drop|DragOver)\s*=\s*"([A-Za-z0-9_]+)"'
)
NAME_PATTERN = re.compile(r'\bx:Name\s*=\s*"([A-Za-z0-9_]+)"')
METHOD_PATTERN = re.compile(
    r'\bprivate\s+(?:static\s+)?(?:async\s+)?void\s+([A-Za-z0-9_]+)\s*\('
)

REQUIRED_PROJECTION_SUBSCRIPTIONS = (
    re.compile(r"_clientProjection\.Changed\s*\+="),
    re.compile(r"_clientProjection\.ExecutionChanged\s*\+="),
    re.compile(r"_clientProjection\.RoiPulseChanged\s*\+="),
)

FORBIDDEN_LEGACY_SUBSCRIPTIONS = (
    re.compile(r"_client\.ProductionChanged\s*\+="),
    re.compile(r"_workspaceRuntime\.Changed\s*\+="),
)

REQUIRED_SHELL_ROUTING_GATES = (
    "BindAcquisitionButton.IsEnabled=routing.CanBindAcquisition",
    "if(!routing.CanBindAcquisition)",
    "if(!routing.CanPreviewAcquisition)",
)

REQUIRED_INSPECTION_PROJECTION_BINDINGS = (
    "RunReadinessStatus.Text=inspection.RunReadinessText;",
    "RenderPreview(inspection.AcquisitionPreview);",
)

REQUIRED_SHELL_NAMES = (
    'x:Name="RunReadinessStatus"',
)

FORBIDDEN_LOCAL_PREVIEW_RENDERING = (
    "RenderPreview(preview);",
)

REQUIRED_COMMAND_FACADES = (
    "ClientInspectionExecutionCommandRuntime.",
    "ClientInspectionAcquisitionCommandRuntime.",
    "ClientProgramCommandRuntime.",
    "ClientQualityCommandRuntime.",
    "ClientResultsCommandRuntime.",
)

FORBIDDEN_DIRECT_WORKSPACE_READS = (
    re.compile(r"_client\\.Production\\b"),
    re.compile(r"_client\\.Quality\\b(?!ProviderCatalog)"),
    re.compile(r"_client\\.History\\b"),
    re.compile(r"_client\\.SelectedHistoryOrdinal\\b"),
    re.compile(r"_client\\.Acquisition\\b(?!Catalog)"),
    re.compile(r"_client\\.Program\\b(?!CommandRuntime)"),
    re.compile(r"_client\\.CurrentProgram\\b"),
)

FORBIDDEN_DIRECT_WORKSPACE_MUTATIONS = (
    "_client.BindAcquisitionSource(",
    "_client.LoadProgram(",
    "_client.ExecuteAsync(",
    "_client.CancelExecution(",
    "_client.ResetCurrentSession(",
    "_client.SetRoiMode(",
    "_client.UndoRoi(",
    "_client.RedoRoi(",
    "_client.SubmitRoiInput(",
    "_client.SelectProgramStep(",
    "_client.SelectQualityFinding(",
    "_client.SelectHistory(",
)


def _duplicates(values: list[str]) -> list[str]:
    counts: dict[str, int] = {}
    for value in values:
        counts[value] = counts.get(value, 0) + 1
    return sorted(name for name, count in counts.items() if count > 1)


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

    xaml = XAML.read_text(encoding="utf-8")
    code = CODE.read_text(encoding="utf-8")

    handlers = EVENT_PATTERN.findall(xaml)
    methods = METHOD_PATTERN.findall(code)
    names = NAME_PATTERN.findall(xaml)

    for duplicate in _duplicates(names):
        errors.append(f"duplicate x:Name in XAML: {duplicate}")

    method_counts = {name: methods.count(name) for name in set(methods)}
    for handler in sorted(set(handlers)):
        count = method_counts.get(handler, 0)
        if count != 1:
            errors.append(
                f"XAML handler '{handler}' must map to exactly one private void method; found {count}"
            )

    for subscription in REQUIRED_PROJECTION_SUBSCRIPTIONS:
        if not subscription.search(code):
            errors.append(f"missing required projection subscription: {subscription.pattern}")

    for subscription in FORBIDDEN_LEGACY_SUBSCRIPTIONS:
        if subscription.search(code):
            errors.append(f"legacy direct subscription must not exist: {subscription.pattern}")

    for gate in REQUIRED_SHELL_ROUTING_GATES:
        if gate not in code:
            errors.append(f"missing required shell routing gate: {gate}")

    for binding in REQUIRED_INSPECTION_PROJECTION_BINDINGS:
        if binding not in code:
            errors.append(f"missing Inspection projection binding: {binding}")

    for shell_name in REQUIRED_SHELL_NAMES:
        if shell_name not in xaml:
            errors.append(f"missing required WPF shell control: {shell_name}")

    for local_render in FORBIDDEN_LOCAL_PREVIEW_RENDERING:
        if local_render in code:
            errors.append(f"preview image must be rendered from the unified Inspection projection: {local_render}")

    for facade in REQUIRED_COMMAND_FACADES:
        if facade not in code:
            errors.append(f"missing command facade usage: {facade}")

    for read in FORBIDDEN_DIRECT_WORKSPACE_READS:
        if read.search(code):
            errors.append(f"direct workspace read must not exist in shell: {read.pattern}")

    for mutation in FORBIDDEN_DIRECT_WORKSPACE_MUTATIONS:
        if mutation in code:
            errors.append(f"direct workspace mutation must not exist in shell: {mutation}")

    open_braces = code.count("{")
    close_braces = code.count("}")
    if open_braces != close_braces:
        errors.append(
            f"code-behind braces are unbalanced: open={open_braces} close={close_braces}"
        )

    bad_tokens = ("TODO", "NotImplementedException")
    for token in bad_tokens:
        if token in code:
            errors.append(f"forbidden token present in code-behind: {token}")

    print(
        f"names={len(names)} "
        f"handlers={len(set(handlers))} "
        f"projection_subscriptions={sum(item.search(code) is not None for item in REQUIRED_PROJECTION_SUBSCRIPTIONS)} "
        f"legacy_subscriptions={sum(item.search(code) is not None for item in FORBIDDEN_LEGACY_SUBSCRIPTIONS)} "
        f"routing_gates={sum(item in code for item in REQUIRED_SHELL_ROUTING_GATES)} "
        f"projection_bindings={sum(item in code for item in REQUIRED_INSPECTION_PROJECTION_BINDINGS)} "
        f"commands={sum(item in code for item in REQUIRED_COMMAND_FACADES)}"
    )

    if errors:
        for error in errors:
            print("ERROR:", error)
        return 1

    print("Client shell binding contract: OK")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
