# PHASE1 18001–18500 Integration Checkpoint — 2026-09-19

## Product chain
PCB Domain: Board → Component → Collection → Lookup → Statistics → Assembly Snapshot.

## Implemented
- `PcbComponentReference` with placement, package, value, side, layer and rotation.
- Board-aware component validation.
- Deterministic component collection with duplicate-designator rejection and canonical ordering.
- Designator/package lookup runtime.
- Side-aware component statistics.
- `PcbAssemblySnapshot` with deterministic SHA-256 fingerprint and full validation.

## Real defect correction
The initial assembly validator incorrectly attempted to cast component references into feature references. The validator was corrected to use the component contract directly, including board-aware validation and duplicate-designator detection.

The component collection Smoke was also normalized to the required 10 loop-group / 100-round structure.

## Verification boundary
- Five 100-round Smokes registered in the PCB Smoke entry.
- Static audits report balanced delimiters, no TODO/`NotImplementedException`, and exact ten-loop structure on new Smokes.
- No local build/test/CI success is claimed without authoritative evidence.
