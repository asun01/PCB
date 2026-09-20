# Phase 1 — Stages 41001–41500 Integration Checkpoint — 2026-09-20

## Boundary

Closed the viewport CoalesceMoves / input lifecycle cell.

## Product-chain result

The bounded viewport input path now has explicit acceptance coverage for:

- capacity-one pointer-move coalescing;
- preservation of the latest pointer position;
- accepted/coalesced accounting;
- propagation of coalescing evidence from submission runtime;
- finite-coordinate rejection;
- terminal cancellation state;
- deterministic queue draining.

The dedicated coalescing Smoke is registered in `Asun.UI.Viewports.Smoke`.

## Acceptance evidence

Five 100-stage ledgers cover 41001–41500.

The new Smoke contains exactly 10 `for` loop groups and 10 meaningful `Check(...)` call sites, with explicit `round==100`, balanced delimiters, and no TODO/NotImplementedException.

No authoritative build/test/CI result is claimed.
