# Phase 1 Integration Checkpoint — 37001–37500

Date: 2026-09-20
Branch: codex/phase1-nonblocked-automation-20260919

## Closed chain
Unified execution → Simulation replay.

## Executable result
Asun.Platform.PcbSimulationIntegration now aggregates a real SimulationSessionRuntime observation set with a real ProductionSessionRuntime-derived replay binding and an execution snapshot identity. The resulting projection remains factual and replay-oriented; it does not equate simulated observation content with physical captured pixels.

## Real corrections
- Replaced fabricated Production identity in the Smoke with a real ProductionSessionRuntime + SimulatedFrameSource path.
- Reduced the Smoke from 11 to the exact 10 loop/check groups required by the project gate.

## Acceptance
- Production fingerprint must match the simulation binding.
- Frame and observation counts must align.
- Simulation sequences must remain contiguous and unique.
- Observation fingerprints must retain valid SHA-256 shape.
- Aggregate fingerprint is recomputed.
- Smoke executes exactly 100 numbered checks through 10 loop groups.

## Verification boundary
Static source/structure verification only; authoritative build/test/CI evidence remains external.

## Ledgers
- PHASE1_37001-37100_STAGE_LEDGER_20260920.md
- PHASE1_37101-37200_STAGE_LEDGER_20260920.md
- PHASE1_37201-37300_STAGE_LEDGER_20260920.md
- PHASE1_37301-37400_STAGE_LEDGER_20260920.md
- PHASE1_37401-37500_STAGE_LEDGER_20260920.md

## Next live boundary
Stage 37501: unified execution → Render/Quality/Evidence multi-fact bundle.
