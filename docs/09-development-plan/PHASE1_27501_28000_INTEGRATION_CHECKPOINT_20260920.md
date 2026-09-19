# Phase 1 Integration Checkpoint — 27501–28000

Date: 2026-09-20
Branch: codex/phase1-nonblocked-automation-20260919

## Closed chain
Metrology Calibration ↔ PCB Placement Observation.

## Executable result
CalibratedPcbPlacementObservationRuntime now applies a validated AffineCalibrationResult2D to a source measurement point, then executes the existing PcbPlacementObservationRuntime in calibrated board coordinates. The resulting observation is tied to the source point and calibration fingerprint and receives an independent SHA-256 fingerprint.

## Acceptance
- Calibration correspondences and result must validate before transformation.
- Calibrated point must be finite and bound to the source observation.
- Component identity must remain bound.
- Calibration fingerprint tampering is rejected.
- Placement observation is computed from the transformed board-space point.
- Smoke executes exactly 100 numbered checks through 10 loop groups.

## Verification boundary
Static source/structure verification only; authoritative build/test/CI evidence remains external.

## Ledgers
- PHASE1_27501-27600_STAGE_LEDGER_20260920.md
- PHASE1_27601-27700_STAGE_LEDGER_20260920.md
- PHASE1_27701-27800_STAGE_LEDGER_20260920.md
- PHASE1_27801-27900_STAGE_LEDGER_20260920.md
- PHASE1_27901-28000_STAGE_LEDGER_20260920.md

## Next live boundary
Stage 28001: PCB placement observation → Quality finding projection.
