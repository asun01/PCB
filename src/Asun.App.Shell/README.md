# Asun.App.Shell

Bootstrap-only WPF application boundary for Visual Studio 2026 / .NET 10 validation. DevExpress controls are intentionally not referenced until the actual 25.2.3 local assembly/package mechanism is verified; see docs/00-baseline/OPEN-GATES.md.


## Current runnable slice

The shell now launches a vendor-neutral WPF `Bootstrap/MainWindow.xaml` workspace host. It intentionally contains no production commands, fact writes, HALCON objects, or DevExpress references. The host is a reversible bootstrap boundary until the relevant Application Port/PageContract and DevExpress environment gate are resolved.
