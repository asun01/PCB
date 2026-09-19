from pathlib import Path
import re

ROOT = Path(__file__).resolve().parents[1]
SHELL = ROOT / "src" / "Asun.App.Shell"
APP_XAML = SHELL / "App.xaml"
WINDOW_XAML = SHELL / "Bootstrap" / "MainWindow.xaml"
WINDOW_CS = SHELL / "Bootstrap" / "MainWindow.xaml.cs"

FORBIDDEN = ("AsunImage", "Halcon", "DevExpress")


def fail(message: str, errors: list[str]) -> None:
    errors.append(message)


def main() -> int:
    errors: list[str] = []

    for path in (APP_XAML, WINDOW_XAML, WINDOW_CS):
        if not path.is_file():
            fail(f"missing shell bootstrap file: {path.relative_to(ROOT)}", errors)

    if errors:
        for error in errors:
            print(f"ERROR: {error}")
        return 1

    app = APP_XAML.read_text(encoding="utf-8")
    xaml = WINDOW_XAML.read_text(encoding="utf-8")
    code = WINDOW_CS.read_text(encoding="utf-8")

    if "<Application.StartupUri>Bootstrap/MainWindow.xaml</Application.StartupUri>" not in app:
        fail("App.xaml must point to Bootstrap/MainWindow.xaml", errors)

    if 'x:Class="Asun.App.Shell.Bootstrap.MainWindow"' not in xaml:
        fail("MainWindow.xaml has unexpected x:Class", errors)

    if "InitializeComponent();" not in code:
        fail("MainWindow code-behind does not initialize the view", errors)

    automation_ids = re.findall(
        r'AutomationProperties\.AutomationId="([^"]+)"', xaml
    )
    if not automation_ids:
        fail("bootstrap window has no AutomationId markers", errors)
    if len(automation_ids) != len(set(automation_ids)):
        fail("bootstrap window contains duplicate AutomationId values", errors)

    for term in FORBIDDEN:
        if term in app or term in xaml or term in code:
            fail(f"bootstrap shell contains forbidden vendor/vision dependency term: {term}", errors)

    if errors:
        for error in errors:
            print(f"ERROR: {error}")
        return 1

    print(
        f"OK: bootstrap WPF shell is structurally runnable "
        f"({len(automation_ids)} AutomationId markers)"
    )
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
