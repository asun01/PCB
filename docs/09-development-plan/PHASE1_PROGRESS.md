# Phase 1 Progress

- Repository baseline: complete.
- Solution/project blueprint: scaffolded.
- VS2026/.NET10/WPF shell boundary: scaffolded.
- Foundational project dependency references: wired for Platform/Core, Evidence, Vision/HALCON adapter boundary, Metrology, and Device implementation-to-contract.
- Deterministic repository tooling: active.
- FunctionSpec work queue: active and document-derived.
- FunctionSpec binding validator: active.
- PageContract structural validator: active.
- Project boundary validator: active.
- ContractCandidate C-01..C-08 structural validator: active.
- Planning cross-link validator for DEV-PLAN-008/011/012/014: active.
- Repository authority resolver/fingerprint report: active and non-mutating.
- Automatic task-admission report: active; non-authoritative preparation continues while production gates remain open.
- Runnable WPF bootstrap shell: implemented with vendor-neutral workspace host and structural smoke validator.
- Repository CI now includes all deterministic validators and open-gate reporting.
- DevExpress 25.2.3: local assembly/package resolution remains an environment gate.
- HALCON 25.11: target version fixed by repository baseline; actual local operator/API verification remains an environment gate.
- AsunImage: excluded.
- Hardware SDKs: adapter/port boundary only.
- Renderer boundary: deterministic vendor-neutral render replay sink added; Skia/WPF host integration remains outside the core contract.
- Presentation chain: input submission → composite generation → Tile/ROI visibility → render command delivery → latest-wins queue → double buffer → surface commit is covered by a new end-to-end smoke.
- FS-001: production implementation remains blocked until GATE-001 and approved C-01 schema authority are concrete.
- Test framework: still unverified; test-project selection is not invented.
- GitHub Actions runner execution is currently not providing actionable job-step evidence; repository CI failures are not treated as business-code failures without logs.

## Automation rule

Continue all non-blocked repository, tooling, dependency-boundary, simulation-boundary, documentation, validator, replay-harness preparation, and audit work. Never invent production schema/state/owner/threshold/API to bypass an open gate.

### 100-round non-blocked continuation — 2026-09-19

- Completed 100 tracked engineering work units across render evidence, structural invariants, navigation, workflow replay, Scene/Visibility, input/backpressure, render planning/budgeting, and Platform.Core regressions.
- Added deterministic Replay/Render evidence fingerprints without introducing a pixel-golden authority or vendor-specific rendering dependency.
- Added centralized invariant validation and dedicated smoke suites, all registered through the existing repository smoke entry.
- Corrected concrete source defects in AsyncPipeline and AsyncSignal discovered during repository-grounded inspection.
- Maintained the repository rule that HALCON/DevExpress/hardware authority gates remain recorded external gates and are not guessed around.

Verification note:
- No local build/test execution result is asserted.
- Latest branch workflow/status lookup returned no associated GitHub Actions run/status; authoritative build/test verification remains pending external execution.

### 300-round continuous continuation — 2026-09-19

- Added a deterministic 300-round verification matrix and registered it in the main viewport smoke entry.
- Added Render Evidence Manifest, Presentation Audit Trace, deterministic Input Replay Runtime, and Replay Session JSON evidence bundle.
- Centralized structural invariants now validate the new evidence/replay state in addition to render, presentation, input, scene, workflow, and metric state.
- Concrete platform defects previously found in AsyncPipeline and AsyncSignal remain fixed.
- The implementation remains vendor-neutral; Skia/WPF/DevExpress/HALCON/hardware-specific authority is still isolated behind the existing boundaries.

Verification note:
- No local C# build/test result is asserted.
- No GitHub Actions success is inferred without an associated workflow run/status.

### Post-300-round hardening — 2026-09-19

- Continuous presentation runtime now retains bounded Render Evidence Manifest history and ordered Presentation Audit Trace for every executed frame outcome.
- Presentation facade exposes EvidenceHistory and AuditTrace for engineering diagnostics and future UI tooling.
- Added bounded EvidenceStore and bounded AuditTrace semantics to avoid unbounded long-running diagnostic memory.
- Added dependency-free C# source-structure validator plus regression fixtures; repository CI invokes the validator before the deterministic repository/document gates.
- Validator specifically guards against delimiter imbalance and high-confidence class-scope executable statements, complementing—not replacing—the C# compiler.
- Added a deterministic Render Evidence Comparator that reports field-level logical differences between expected and actual manifests.
- Added replay-session integrity validation covering input ordering, evidence monotonicity, audit ordering, counters, and SHA-256 evidence structure.
- Evidence history now rejects duplicate StableKey entries and validates duplicate-key absence.
- Replay-session smoke coverage now includes successful validation, malformed input sequence rejection, JSON evidence-layer presence, reset behavior, and deterministic hashes.
- Registered the comparator smoke in the primary viewport smoke entry.

Verification note:
- The current environment cannot resolve github.com for a local checkout, so no fresh local dotnet build/test result is asserted.
- The latest GitHub branch commit currently has no associated workflow run or status result, so CI success is not inferred.
- Current branch is continuously ahead of main; authoritative merge/CI handling remains with the existing open PR and repository governance.

### 400-stage continuous evidence/replay continuation — 2026-09-19

- Completed stages 401–500 and recorded them in `PHASE1_400_STAGE_LEDGER_20260919.md`.
- Added immutable `ViewportReplaySessionBundle` capture at the runtime boundary.
- Added bundle-level manifest reconstruction, integrity validation, deterministic comparison, and evidence-key linkage validation.
- Added logical render replay snapshot validator covering sequence, frame lifecycle, generation, lifecycle counters, commit/discard status, and rendered-unit totals.
- Added a deterministic 100-round replay bundle smoke and registered it in the primary viewport smoke entry.
- Added continuous presentation outcome evidence smoke covering Failed, Deferred→Presented recovery, and Superseded→Presented recovery.
- Kept all evidence/replay facilities independent of Skia, WPF, DevExpress, HALCON, and hardware SDK types.

Verification note:
- No local C# build/test result is asserted.
- Latest GitHub workflow/status lookup is still required before claiming CI success.
- The branch remains in the non-blocked automation lane; unresolved vendor/authoritative Contract/Schema/Owner/State gates remain unchanged.

### 500→600 bounded replay integration — 2026-09-19

- Completed stages 501–600 and recorded them in `PHASE1_500_600_STAGE_LEDGER_20260919.md`.
- Added bounded input replay history with capacity and dropped-event diagnostics.
- Continuous Presentation Runtime now exposes a validated diagnostic Replay Bundle assembled from bounded input, Evidence, and Audit windows.
- Presentation facade exposes ReplayBundle and ReplayBundleValidation.
- Added validated Replay Bundle JSON serialize/deserialize roundtrip.
- Added 100-round Replay Integration smoke and registered it in the main viewport smoke entry.
- The integration smoke exercises reset/recovery, bounded input history, JSON roundtrip, evidence/audit linkage, repeated mutation comparisons, and requires exactly 100 numbered rounds.
- Existing vendor-neutral replay/evidence boundaries remain unchanged.

Verification note:
- Static source-structure checks were performed on the newly touched files; no local compiler/test success is asserted.
- Latest branch workflow/status lookup must remain the authority for CI; no success is inferred without an associated run/status.
- HALCON/DevExpress/hardware and unresolved authoritative Contract/Schema/Owner/State gates remain unchanged.

### 600→700 replay diagnostic windows — 2026-09-19

- Completed stages 601–700 and recorded them in `PHASE1_600_700_STAGE_LEDGER_20260919.md`.
- Added `ViewportReplaySessionBundleWindowRuntime` for bounded diagnostic tail projections.
- Window projection preserves audit→evidence reference closure and rejects impossible evidence limits.
- Added deterministic window descriptors for input/evidence/audit sequence boundaries.
- Added exact 100-round replay-window smoke covering bounded tails, empty windows, negative-limit guards, nested containment, JSON roundtrip, tamper detection, and full-capacity equivalence.
- Registered the new smoke in the primary viewport smoke entry.
- Corrected the matrix to exactly ten loop groups with one Check per iteration, yielding exactly 100 numbered rounds.
- No pixel-golden authority or vendor-specific renderer semantics were introduced.

Verification note:
- Static source checks confirm balanced C# delimiters in the newly touched runtime/smoke files.
- The exact replay-window smoke structure is 10 loops × 10 iterations × 1 Check = 100 numbered rounds, with an explicit `round == 100` assertion.
- No local build/test/CI success is asserted without authoritative execution evidence.

### 700→800 unified replay execution — 2026-09-19

- Completed stages 701–800 and recorded them in `PHASE1_700_800_STAGE_LEDGER_20260919.md`.
- Added `ViewportReplayExecutionRuntime` as the single deterministic input-replay execution path.
- Execution reports capture initial/final generation, deterministic input/result hashes, transform/document/selection changes, dirty-event count, and ordered results.
- `ViewportInputReplayRuntime.Replay` now delegates to the unified execution runtime, and `ReplayReport` exposes the full execution report.
- Added exact 100-round replay execution smoke covering deterministic repeatability, validation guards, empty replay, valid bundle replay, bounded counters, finite result coordinates, reversed ordering rejection, and cross-runtime consistency.
- Registered the execution smoke in the primary viewport smoke entry.
- Static review keeps the new runtime backend-neutral and free of HALCON/DevExpress/hardware-specific contracts.

Verification note:
- Exact replay execution matrix is 10 loop groups × 10 iterations × 1 Check = 100 numbered rounds; final round-count assertion is outside the Check counter.
- No local build/test/CI success is asserted without authoritative execution evidence.

### 800→900 deterministic replay state fingerprint — 2026-09-19

- Completed stages 801–900 and recorded them in `PHASE1_800_900_STAGE_LEDGER_20260919.md`.
- Added `ViewportReplayStateFingerprintRuntime` for deterministic viewport/ROI/generation state capture and structured comparison.
- Added `ViewportReplayExecutionStateRuntime` to pair the existing replay execution report with initial/final state fingerprints without duplicating input execution logic.
- State comparison reports field paths for transform, generation, selection, ROI count, geometry, and state hash differences.
- Added exact 100-round smoke coverage for deterministic replay state, mutation detection, empty replay preservation, stable ROI identity, direct-vs-bundle convergence, and final-state consistency.
- Registered the state fingerprint smoke in the primary viewport smoke entry.
- Deterministic tests use an explicit stable ROI Guid because auto-generated identities are session-specific and cannot form a reproducible cross-session fingerprint.

Verification note:
- State fingerprint smoke is 10 loop groups × 10 iterations × 1 Check = 100 numbered rounds, with the final `round == 100` assertion outside the Check counter.
- New C# sources have balanced delimiters and no placeholder implementation.
- No local build/test/CI success is asserted without authoritative execution evidence.

### 900→1000 deterministic replay verification gate — 2026-09-19

- Completed stages 901–1000 and recorded them in `PHASE1_900_1000_STAGE_LEDGER_20260919.md`.
- Added `ViewportReplayVerificationRuntime` for full replay verification across input hash, execution result hash, and final state fingerprint.
- Added an explicitly scoped `VerifyInputAgainstBundle` gate so Bundle verification does not claim result/state equivalence when the Bundle only provides input evidence.
- Added exact 100-round verification smoke covering clean gates, input mutation, final-state mutation, bundle input matching/mismatch, malformed bundles, empty replay, structured state comparison, selection mismatch, and repeated clean verification.
- Repaired `ViewportInputReplayRuntime.EvidenceHash()` so its deterministic newline separator is valid C# string syntax.
- Registered the verification smoke in the primary viewport smoke entry.
- No vendor-specific renderer, HALCON, DevExpress, or hardware contracts were introduced.

Verification note:
- The verification smoke is 10 loop groups × 10 iterations × 1 Check = 100 numbered rounds, with the final `round == 100` assertion outside the Check counter.
- Static source review is used for this pass; no build/test/CI success is asserted without authoritative execution evidence.

### 1001→1500 continuous five-hundred-round replay hardening — 2026-09-19

- Completed stages 1001–1500 as five contiguous 100-stage blocks.
- Added Bundle-level replay verification with component match flags and structured differences.
- Added internal Replay Bundle format versioning and malformed-array validation.
- Added bounded Replay Checkpoint runtime/store for prefix and stage-boundary verification.
- Added Presentation Evidence/Audit verification gate with ordering checks.
- Added integrated Replay Diagnostic Gate combining execution, state, bundle, checkpoint, and presentation evidence verification.
- Corrected `ViewportInputReplayRuntime.EvidenceHash()` to use a valid deterministic `"\n"` separator.
- Registered five new 100-round smoke matrices, giving a 500-round continuous hardening sequence.
- Added `PHASE1_1001_1500_STAGE_LEDGER_20260919.md` with exactly 500 consecutive stage entries.

Verification boundary:
- Static source verification confirms the new C# files are structurally balanced and the five smoke matrices each use 10 loops × 10 Check calls with an explicit `round == 100` assertion.
- No local build/test/CI success is asserted without authoritative execution evidence.

### 1501→2000 continuous diagnostic runtime hardening — 2026-09-19

- Completed stages 1501–2000 as five contiguous 100-stage blocks.
- Added `ViewportReplayDiagnosticSnapshotRuntime` to unify Execution, Bundle, Checkpoint and cross-layer hash consistency.
- Added bounded `ViewportReplayDiagnosticSnapshotStore`.
- Added portable `ViewportReplayDiagnosticManifestRuntime` with internal format versioning and malformed JSON validation.
- Added `ViewportPresentationReplayDiagnosticRuntime` plus `ViewportPresentationRuntime.CreateReplayDiagnosticSnapshot()`.
- Added five exact 100-round smoke matrices covering snapshot, bounded history, portable manifest, presentation facade, and continuous integration.
- Registered all five new smokes in the primary viewport smoke entry.
- Added independent stage ledgers for 1501–1600, 1601–1700, 1701–1800, 1801–1900, and 1901–2000.
- Corrected malformed JSON string literals in the diagnostic manifest smoke and normalized the presentation smoke to exactly 100 rounds.
- No HALCON, DevExpress, hardware SDK, or renderer-specific contracts were introduced.

Verification note:
- Each of the five new smoke matrices is structurally 10 loops × 10 Check calls with an explicit `round == 100` assertion.
- The five ledgers contain exactly 100 consecutive stage entries each.
- Static source checks were used; no build/test/CI success is asserted without authoritative execution evidence.

- Final hardening after the 1501–2000 static audit: Diagnostic Manifest hash validation now rejects null/blank hashes cleanly; Presentation Diagnostic equivalence now includes the complete Presentation Snapshot; capture rejects Presentation/State generation drift.

### 2001→2500 checkpoint and bundle integrity hardening — 2026-09-19

- Completed stages 2001–2500 as five contiguous 100-stage blocks.
- Corrected a real semantic defect in ViewportReplayExecutionReport: the execution layer now preserves the actual last input sequence instead of deriving it from result count.
- ViewportReplayCheckpointRuntime.Capture(report) now uses the actual source sequence; non-empty checkpoints require a positive last sequence.
- ViewportReplayCheckpointStore now enforces monotonic Generation in addition to ordinal/input sequence.
- ViewportReplaySessionBundleRuntime.FromJson now normalizes malformed JSON parsing into a deterministic InvalidOperationException.
- Bundle validation now rejects non-finite input positions, undefined input kind/button values, duplicate or blank evidence StableKeys, and dangling audit evidence references.
- Repaired an existing malformed string literal in ViewportReplayBundleSchemaHundredStageSmoke.
- Added and registered five new exact 100-round Smoke matrices for checkpoint sequence semantics, bundle integrity, JSON serialization boundary, checkpoint-store generation, and integrated diagnostic integrity.
- Added independent stage ledgers for 2001–2100, 2101–2200, 2201–2300, 2301–2400, and 2401–2500.
- Static verification confirms each new 100-round smoke has exactly 10 loops × 10 Check calls and round == 100.
- No local build/test/CI success is asserted without authoritative execution evidence.

### One-million-stage execution program established — 2026-09-19

- Global non-blocked execution horizon expanded from the previous 10,000-stage planning slice to exactly 1,000,000 future stages: 2501–1,002,500.
- Added PHASE1_2501_1002500_MASTER_1000000_STAGE_EXECUTION_PLAN_20260919.md.
- The master plan uses deterministic hierarchy: 2,000 batches × 500 stages, 10,000 blocks × 100 stages, 100,000 micro-blocks × 10 stages.
- The plan defines 20 engineering tracks, deterministic track rotation, acceptance requirements, stage-number continuity rules, and vendor-neutral continuation rules.
- Stages 1–2,500 remain the only completed scope currently represented as repository evidence; 2501–1,002,500 are planned only.

### 2501→3000 render and presentation invariant hardening — 2026-09-19

- Completed stages 2501–3000 as five contiguous 100-stage blocks.
- Added delivery-result validation covering generation, unit accounting, deferred/cancelled/failed state coherence, and deferred-work accounting.
- Corrected ViewportRenderDeliveryTracker so LastGeneration is monotonic under out-of-order delivery results.
- Added command-stream validation covering sequence, generation, kind mapping, finite bounds and category accounting.
- Added render-surface validation covering transaction state, generation/sequence metadata, presentation state, units and regions.
- Added presentation-buffer validation covering slot ownership, generation/sequence metadata, state transitions, units and regions.
- Added presentation-queue validation covering enqueue/dequeue/pending/dropped accounting, token pairing, commit state and latest-sequence fences.
- Registered five new exact 100-round Smoke matrices in the primary viewport smoke entry.
- Added stage ledgers for 2501–2600, 2601–2700, 2701–2800, 2801–2900 and 2901–3000.
- Static verification confirms all five new Smoke matrices use 10 loop groups × 10 Check calls with round == 100.
- No local build/test/CI success is asserted without authoritative execution evidence.

### 3001→3500 render strategy and bounded cache hardening — 2026-09-19

- Completed stages 3001–3500 as five contiguous 100-stage blocks.
- Added reusable Render Budget validation for total/category ceilings, source membership and generation preservation.
- Added Render Priority validation for work preservation, deterministic ordering, invalidation prefix and generation coherence.
- Added Render Reuse validation for presented-frame reuse, generation identity, clearing and deferred-frame exclusion.
- Added Tile Cache validation for bounded count, LRU recency, unique keys and statistics coherence.
- Added Tile Prefetch validation for maximum size, source membership, visible-first ordering, distinct indices and deterministic policy boundaries.
- Added five exact 100-round Smoke matrices and registered them in the primary viewport smoke entry.
- Completed 3001–3500 with five dedicated stage ledgers.
- No local build/test/CI success is asserted without authoritative execution evidence.


### 3501→4000 presentation lifecycle and tile request hardening — 2026-09-19

- Completed stages 3501–4000 as five contiguous 100-stage blocks.
- Audited presentation-buffer Reset semantics and retained the monotonic submission-fence domain intentionally: reset clears presentation state/counters, but pre-reset submission sequences remain invalid.
- Corrected a real presentation-buffer contract defect: Commit now rejects rendered units greater than the planned unit budget instead of publishing impossible accounting.
- Added presentation-buffer and presentation-queue lifecycle Smoke matrices covering reset, stale fences, commit windows, cancellation, latest sequence and disposed state.
- Added TileLoadCoordinatorValidationRuntime and a 100-round concurrency Smoke covering same-tile load coalescing, shared results, caller cancellation isolation, completion cleanup and cache population.
- Added TileCacheWarmupValidationRuntime and a 100-round Smoke covering duplicate-request suppression, cache-only repeat warmup, source-call suppression and load-health accounting.
- Added TileRequestPlannerValidationRuntime and a 100-round Smoke covering visible coverage, visible-first ordering, tile uniqueness, geometry-derived distance, finite request rectangles, deterministic ordering and out-of-grid rejection.
- Registered all five new lifecycle/planner Smokes in the primary viewport smoke entry.
- Added stage ledgers for 3501–3600, 3601–3700, 3701–3800, 3801–3900 and 3901–4000.
- Static verification confirms each new 100-stage Smoke uses 10 loop groups × 10 Check calls with round == 100, and the new C# sources have balanced delimiters with no placeholder implementation.
- No local build/test/CI success is asserted without authoritative execution evidence.


### 4001→4500 input capture, routing and ROI lifecycle hardening — 2026-09-19

- Completed stages 4001–4500 as five contiguous 100-stage blocks.
- Corrected a real `ViewportInputRouterRuntime` ownership defect: failed capture acquisition can no longer leak PointerUp/Escape release into another input owner; right-button input no longer occupies ROI capture; foreign-owner pointer moves are isolated.
- Added input-capture and router validators plus exact 100-round lifecycle Smokes covering ownership, blocking, release symmetry and foreign-input isolation.
- Added `ViewportInputSubmissionValidationRuntime` and a 100-round Smoke covering move coalescing, completion, reset lifecycle, cancellation and post-reset submission.
- Corrected a real `RoiTeachingGuide.Start(false)` terminal-state defect: a completed non-repeatable guide no longer re-enters an active/null-step dead state.
- Added ROI editor validation covering committed/preview convergence, cancel restoration and create-cancel semantics.
- Added ROI teaching validation covering terminal completion, non-repeatable Start, explicit restart and step identity.
- Registered all five new Smokes in the primary viewport smoke entry.
- Added ledgers for 4001–4100, 4101–4200, 4201–4300, 4301–4400 and 4401–4500.
- Static verification confirms each new 100-stage Smoke uses 10 loop groups × 10 Check calls with `round == 100`; delimiters are balanced and no placeholder implementation was introduced in the new validation/Smoke files.
- No local build/test/CI success is asserted without authoritative execution evidence.


### 4501→5000 scene, selection, document identity and tile visibility hardening — 2026-09-19

Completed the first 500-stage batch inside the requested 100,000-stage execution window (4501–104500).

Real defects corrected:
- `RoiSelectionRuntime.SelectFromPoint` now honors the supplied handle tolerance through spatial-index hit testing instead of point-containment-only lookup.
- `RoiLayerRuntime.Ensure` now preserves existing visibility/lock state while allowing synchronized name/order updates.
- `ViewportSceneRuntime` now gives ROI body segments stable `HandleIndex` identity, enabling complete multi-edge scene diffs.
- `RoiDocumentRuntime.Cancel` preserves the identity of the ROI being cancelled across snapshot rollback.
- `RoiDocumentRuntime.Add` rejects empty and duplicate explicit ROI ids.

Validation added:
- `ViewportSceneDiffValidationRuntime`
- `RoiLayerValidationRuntime`
- `RoiSelectionValidationRuntime`
- `RoiDocumentValidationRuntime`
- `ViewportTileRoiVisibilityValidationRuntime`
- five dedicated 100-round Smoke entries, all registered in `tests/Asun.UI.Viewports.Smoke/Program.cs`.

Stage ledgers closed:
- 4501–4600
- 4601–4700
- 4701–4800
- 4801–4900
- 4901–5000

Verification status:
- Source-level/static structure checks were performed on the changed acceptance assets.
- No local build, test, GitHub Actions, hardware, HALCON, DevExpress, or vendor SDK success is claimed unless authoritative execution evidence exists.
- The 100,000-stage execution window remains active for stages 4501–104500.


### 5001→5500 platform resource, queue, pipeline and statistics hardening — 2026-09-19

Completed the next 500-stage batch inside the active 100,000-stage execution window.

Real defect corrected:
- `ResourceLeasePool.AcquireAsync(resource, timeout, cancellationToken)` now accepts `TimeSpan.Zero` as a valid non-blocking timeout, matching the synchronous acquisition semantics. Negative finite timeouts remain rejected.

Validation added:
- `ResourceLeasePoolValidationRuntime`
- `BoundedWorkQueueValidationRuntime`
- `AsyncPipelineValidationRuntime`
- `StatisticsValidationRuntime`
- `AsyncSignalValidationRuntime`
- five dedicated 100-round Platform Core Smokes, registered in `tests/Asun.Platform.Core.Smoke/Program.cs`.

Stage ledgers closed:
- 5001–5100
- 5101–5200
- 5201–5300
- 5301–5400
- 5401–5500

Verification status:
- Static structure audits passed for the five new Smokes: 10 loop groups, 100 numbered rounds, balanced delimiters, no placeholder markers.
- No local build/test/CI success is claimed; the latest checked branch commit has no workflow run/status evidence.


### 5501→6000 simulation, tile-range, frame and observability hardening — 2026-09-19

Completed the third 500-stage batch inside the active 100,000-stage execution window.

Implemented:
- reusable `SimulatedTileSource<TTile>` over the existing `ITileSource<TTile>` boundary;
- deterministic delay/failure/cancellation/concurrency simulation;
- `TileRangeValidationRuntime`;
- `ViewportTileFrameValidationRuntime`;
- `TileViewportDiagnosticsValidationRuntime`;
- `TileLoadHealthValidationRuntime`;
- `SimulatedTileSourceValidationRuntime`.

Added and registered five 100-round validation Smokes:
- tile range;
- viewport tile frame;
- tile viewport diagnostics;
- tile load health;
- simulated tile source.

Defect hardening during acceptance:
- fixed missing `System.Numerics` dependency in tile-range validator;
- removed an invalid invariant that treated cumulative cache evictions as current cache occupancy;
- tightened tile-frame coverage to use order-independent set equality;
- removed a tautological cache-health assertion.

Closed acceptance assets:
- stage ledgers 5501–5600, 5601–5700, 5701–5800, 5801–5900, 5901–6000;
- integration checkpoint `PHASE1_5501_6000_INTEGRATION_CHECKPOINT_20260919.md`.

Verification status:
- static structure audits passed for the five new 100-round Smokes: 10 loop groups, exact `round == 100`, balanced delimiters, no placeholder markers;
- no local build/test/CI success is claimed without authoritative execution evidence.


### 6001→6500 metrology/PCB geometry-contract hardening — 2026-09-19

Completed stages 6001–6500 inside the active 100,000-stage execution window.

Real defect corrected:
- `AffineTransform2D.TryInvert` now follows the public `IsInvertible` stability threshold and rejects non-finite inverse matrices, preventing contradictory inversion decisions at ill-conditioned transforms.

Validation added:
- `NumericToleranceValidationRuntime`
- `AffineTransform2DValidationRuntime`
- `ImageGeometryValidationRuntime`
- `PointSet2DValidationRuntime`
- `Polygon2DValidationRuntime`

Added dedicated `Asun.Vision.Contracts.Smoke` project, added it to `AsunVision.slnx`, and registered five exact 100-round Smokes.

Closed acceptance assets:
- stage ledgers 6001–6100, 6101–6200, 6201–6300, 6301–6400, 6401–6500;
- integration checkpoint `PHASE1_6001_6500_INTEGRATION_CHECKPOINT_20260919.md`.

Verification status:
- static structure audits passed: five Smokes × 10 loop groups, exact `round == 100`, balanced delimiters, no placeholder markers;
- no local build/test/CI success is claimed without authoritative execution evidence.
