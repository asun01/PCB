# Phase 1 — Stages 40001–40500 Integration Checkpoint — 2026-09-20

## Boundary

Closed the Metrology / Production → PCB placement provenance integration cell.

## Product-chain result

`ProductionMeasurementPcbBindingRuntime` now binds a Production measurement fact to the concrete calibrated PCB placement observation that produced it.

The binding preserves:

- Production frame sequence and input fingerprint;
- PCB component identity and designator;
- calibration fingerprint;
- calibrated observation fingerprint;
- deterministic binding fingerprint.

It also provides:

- independent validation and tamper rejection;
- deterministic canonical key;
- binding equivalence;
- persistence-neutral replay descriptor.

The existing Metrology/Production Smoke program registers `ProductionMeasurementPcbBindingHundredStageSmoke`, with exactly 10 loop groups and 10 meaningful Check call sites plus explicit `round==100`.

Five 100-stage ledgers cover 40001–40500.

No authoritative build/test/CI result is claimed.
