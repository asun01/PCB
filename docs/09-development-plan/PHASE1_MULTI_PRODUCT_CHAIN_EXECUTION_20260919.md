# Phase 1 — Multi-Product-Chain Continuous Execution Model

## Execution branch

codex/phase1-nonblocked-automation-20260919

## Objective

The 1,000,000-stage program remains active, but completed work is now rotated across concrete product chains so the repository continuously converges toward an actual industrial PCB/PCBA product rather than a single subsystem.

## Product chains

### Chain A — PCB Domain
Board → Layer Stack → Component → Pad/Via/Hole → Net → Region → Fiducial → Coordinate Reference → Assembly/Inspection Target.

### Chain B — Vision / Metrology
Measurement Point → Units → Coordinate System → Transform → Calibration Data → Feature Extraction Boundary → Measurement Result → Tolerance/Traceability Boundary.

### Chain C — Acquisition / Device
Frame Source → Capture Metadata → Trigger/Sequence → Buffer → Simulation → Device State → Motion Command Boundary → Acquisition Session.

### Chain D — Render / Presentation
Render Work Item → Render Batch → Command Stream → Frame Summary → Surface/Delivery → Queue → Recovery → UI Adapter boundary.

### Chain E — Inspection / Quality
Inspection Target → Rule Invocation Boundary → Observation → Finding → Severity/Category → Evidence Link → Inspection Result → Quality Summary.

### Chain F — Program / Recipe
Program Definition → Step → Parameter Set → Version → Validation → Deterministic Execution Plan → Replay Descriptor.

### Chain G — Pipeline / Orchestration
Input → Normalize → Acquire → Transform → Inspect → Measure → Aggregate → Evidence → Result → Commit/Present.

### Chain H — Evidence / Audit
Evidence Descriptor → Catalog → Snapshot → Query → Reference Closure → Diagnostic Bundle → Audit Projection → Replay.

### Chain I — Simulation / Digital Twin
Simulated Board → Simulated Frames → Deterministic Defects → Measurement/Inspection Playback → Fault Injection → Recovery.

### Chain J — Release / Compliance
Configuration Integrity → Version Identity → Audit Event → Artifact Manifest → Migration Boundary → Release Readiness.

### Chain K — Persistence
Repository-neutral persistence boundary → Snapshot store contract → transaction boundary → schema version → migration → recovery. Implement only when authority is available; do not invent storage semantics early.

### Chain L — Production Runtime
Recipe Load → Device Bring-up → Acquisition Session → Inspection Run → Measurement → Result → Evidence → Operator Review → Release/Export.

## Rotation rule

Completed 500-stage batches rotate across multiple chains. The next primary chains are:

1. 18001–18500 — PCB Domain / Component chain
2. 18501–19000 — Vision / Metrology foundation
3. 19001–19500 — Render / Presentation runtime
4. 19501–20000 — Acquisition / Device simulation boundary
5. 20001–20500 — Inspection / Quality integration
6. 20501–21000 — Program / Recipe
7. 21001–21500 — Pipeline / Orchestration
8. 21501–22000 — Simulation / Digital Twin
9. 22001–22500 — Release / Compliance
10. 22501–23000 — Cross-chain Production Runtime

The sequence then repeats with deeper integration rather than merely recreating the same contracts.

## Product-chain completion rule

A chain block is not complete because a type exists. It requires:

- real executable domain/runtime logic;
- validation of invalid states;
- at least one end-to-end integration Smoke;
- deterministic behavior where practical;
- a concrete cross-module handoff;
- stage ledger;
- integration checkpoint;
- progress synchronization;
- honest build/CI boundary.

## Anti-skeleton rule

The following alone never qualifies as stage completion:

- empty interfaces;
- DTO-only additions without runtime behavior;
- placeholder methods;
- NotImplementedException;
- TODO-only scaffolding;
- documentation-only claims of implementation;
- history commits that are not present on the active execution branch.

Historical commits are evidence of history only; active completion is determined from the current branch.

## Current live boundary

- Completed: 18,000
- Next: 18,001
- Global horizon: 2,501–1,002,500
- Active non-blocked execution window: 4,501–104,500

## 100,000-stage macro-batch execution rule — effective 2026-09-20

The previous 500-stage grouping is retained as an **internal quality-control cell**, not as the maximum autonomous execution size.

The execution hierarchy is now:

- **100,000 stages = one macro-batch**: the primary planning and continuous-execution unit.
- **500 stages = one integration checkpoint cell**: used for concrete cross-chain closure, five 100-stage ledgers, and a checkpoint document inside the macro-batch.
- **100 stages = one acceptance cell**: requires real executable work, invalid-state handling, an exact 100-round Smoke, registration/static audit, and a ledger.
- **10-round loops = Smoke implementation discipline**: ten meaningful Check call sites, explicit round == 100, balanced delimiters, no TODO/NotImplementedException, and no tautological assertions.

The macro-batch therefore does **not** mean that 100,000 files/tasks are fabricated in advance or that one tool call must contain 100,000 mutations. It means the autonomous execution horizon is planned and rotated as a 100,000-stage product program while implementation is continuously committed through the internal 500/100 acceptance cells.

Within each 100,000-stage macro-batch, product chains continue to rotate dynamically. A chain may be revisited whenever a real cross-module dependency or integration bottleneck appears; the macro-batch is a horizon, not a promise to execute one subsystem for 100,000 consecutive stages.

The current global horizon remains 2501–1,002,500, so it is naturally partitioned into ten 100,000-stage macro-batches (with the final residual 2,500 stages handled as the closing macro-batch).

Historical 500-stage checkpoints remain valid evidence and are not rewritten. This rule changes the **future execution cadence**, not the historical stage accounting.

## Live execution synchronization — 2026-09-20

- Active branch: `codex/phase1-nonblocked-automation-20260919`
- Completed boundary: **42,000**
- Next executable stage: **42,001**
- Current macro horizon: **41,501–141,500**
- Macro plan: `PHASE1_41501_141500_100000_STAGE_MACROBATCH_PLAN_20260920.md`

The current rotation has moved through Production→PCB, Metrology→PCB, Viewport backpressure/coalescing, and Quality→Evidence. The next cells continue dynamic rotation rather than locking the repository to a single subsystem.

## Live execution synchronization — Stage 42500

- Completed boundary: **42,500**
- Next executable stage: **42,501**
- Active macro horizon: **41,501–141,500**
- Latest rotation: Pipeline replay → Release logical handoff.
