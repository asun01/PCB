# Phase 1 — Stages 40501–41000 Integration Checkpoint — 2026-09-20

## Boundary

Closed the viewport input backpressure lifecycle cell on the active PCB branch.

## Product-chain result

The existing `ViewportInputSubmissionRuntime` now has a concrete bounded backpressure companion, `ViewportInputBackpressureRuntime`, covering:

- bounded submission capacity;
- DropNewest handling;
- accepted/dropped/coalesced accounting;
- lifecycle completion/cancellation state;
- deterministic snapshots;
- finite-position validation;
- disposal protection.

A dedicated 100-round Smoke is registered in the existing viewport Smoke program.

## Acceptance evidence

Five 100-stage ledgers cover 40501–41000.

The Smoke contains exactly 10 `for` loop groups and 10 meaningful `Check(...)` call sites, with explicit `round==100`, balanced delimiters, and no TODO/NotImplementedException.

No authoritative build/test/CI result is claimed.
