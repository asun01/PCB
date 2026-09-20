# PHASE1 74501-75000 INTEGRATION CHECKPOINT — 2026-09-20

**WPF ROI Surface**
→ **WPF Input Adapter**
→ **Client ROI Workspace**
→ **ROI Snapshot**
→ **Visual Rectangle Projection**

Added `WpfRoiInputAdapter` and connected it to the WPF shell. The client now exposes explicit Select/Create ROI modes, pointer input translation, Escape cancellation, viewport resize propagation, and visual rectangle projection from the UI-neutral ROI snapshot.

The host is explicitly a development/vendor-neutral surface. It does not claim final DevExpress/Skia implementation or HALCON execution.

Static source/XAML audit only; no authoritative build/test/CI success is claimed.

Completed boundary: **75,000**
Next executable stage: **75,001**
