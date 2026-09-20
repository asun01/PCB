# Phase 1 Integration Checkpoint — 36501–37000

Date: 2026-09-20
Branch: codex/phase1-nonblocked-automation-20260919

## Closed chain
Evidence catalog resolution → Release/replay facts.

## Executable result
Asun.Platform.PcbEvidenceReleaseIntegration now projects catalog-resolution facts alongside unified execution and Release identities. Requested, found, and missing handles remain explicit factual counts; the derived all-resolved property is descriptive and not a customer acceptance verdict.

## Acceptance
- Evidence resolution and Release manifest identities must remain bound.
- Requested/found/missing counts are recomputed.
- AllRequestedResolved must equal the missing-handle count being zero.
- Aggregate fingerprint is recomputed.
- Smoke executes exactly 100 numbered checks through 10 loop groups.

## Verification boundary
Static source/structure verification only; authoritative build/test/CI evidence remains external.

## Ledgers
- PHASE1_36501-36600_STAGE_LEDGER_20260920.md
- PHASE1_36601-36700_STAGE_LEDGER_20260920.md
- PHASE1_36701-36800_STAGE_LEDGER_20260920.md
- PHASE1_36801-36900_STAGE_LEDGER_20260920.md
- PHASE1_36901-37000_STAGE_LEDGER_20260920.md

## Next live boundary
Stage 37001: unified execution → simulation replay aggregate.
