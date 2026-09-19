# PHASE1 21501–22000 Integration Checkpoint — 2026-09-19

## Product chain
Simulation / Digital Twin: PCB Assembly → Seeded Scenario → Simulated Defect → Metrology Coordinate → Frame Sequence → Session Replay.

## Implemented
- `Asun.Simulation.Core` added as a vendor-neutral deterministic simulation boundary.
- Scenarios bind to the real `PcbAssemblySnapshot`.
- Generated defects target actual component designators and finite Metrology coordinates.
- Observation fingerprints are derived from sequence, assembly fingerprint, seed, and defect content.
- Simulation sessions enforce contiguous frame sequences and validate every observation.
- Dedicated Simulation Smoke project registered in `AsunVision.slnx`.

## Boundary / hardening note
The current simulator uses seeded `Random` and is deterministic for identical inputs within the current runtime contract. Cross-runtime byte-for-byte reproducibility is not yet treated as a release-level guarantee because the current seed derivation uses runtime `HashCode.Combine`; a later hardening block should replace that with a stable explicit integer hash before claiming cross-process golden determinism.

## Verification boundary
- Static audits confirm balanced delimiters and no TODO/`NotImplementedException`.
- Both new 100-round Smokes use 10 loop groups and explicit `round == 100`.
- No compiler/test/CI success is claimed without authoritative workflow evidence.
