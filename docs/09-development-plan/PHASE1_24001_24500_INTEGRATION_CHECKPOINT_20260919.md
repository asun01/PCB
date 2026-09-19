# PHASE1 24001–24500 Integration Checkpoint — 2026-09-19

## Product chain
Metrology Calibration: Correspondence → Affine Least Squares → Transform → Residual Metrics → Fingerprint → Independent Validation.

## Implemented
- CalibrationCorrespondence2D.
- AffineCalibrationRuntime with 6-parameter affine least-squares fitting and pivoted 6×6 solve.
- RMS and maximum geometric residual calculation.
- Deterministic calibration fingerprint.
- Independent validation that re-fits and compares all six affine coefficients plus error metrics.
- Dedicated calibration Smoke registered in the Metrology Smoke project.

## Product relevance
This is a vendor-neutral mathematical calibration layer that can later support pixel↔metric mapping, FOV calibration, coordinate-system establishment and multi-FOV stitching without pretending to be a HALCON calibration operator.

## Real corrections during block
- Repaired a malformed determinant-threshold identifier before checkpointing.
- Tightened transform validation to compare all six affine coefficients instead of only one coefficient.

## Verification boundary
- Static audits confirm balanced delimiters and no TODO/`NotImplementedException`.
- Calibration Smoke uses 10 loop groups and explicit `round == 100`.
- No compiler/test/CI success is claimed without authoritative workflow evidence.
