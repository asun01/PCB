# Development Status

## Phase 0 — Repository baseline
Status: COMPLETED.

## Phase 1 — Automated implementation
Status: ACTIVE.

Completed:
- VS2026 / .NET 10 / WPF solution architecture scaffolded.
- SDK-style projects created according to the repository project/namespace blueprint.
- WPF application boundary created as a bootstrap shell.
- Open external/contract gates recorded in docs/00-baseline/OPEN-GATES.md and GitHub issue #4.
- No AsunImage dependency introduced.
- Hardware-vendor SDK implementation intentionally deferred behind ports/adapters.
- Deterministic repository policy validation added.
- FunctionSpec-derived work-queue reporting restored.
- Open-gate reporting automation added.
- Repository entrypoint baseline wording synchronized with CURRENT_BASELINE.md and AGENTS.md.

Current gate:
- FS-001 production implementation is waiting for concrete GATE-001 and approved C-01 schema/validator artifacts.
- This gate does not block repository/tooling work, simulation boundaries, or other non-authoritative preparation.

Current automated work:
1. Maintain deterministic repository and document integrity gates.
2. Maintain the document-derived 69-FunctionSpec work queue and gate reporting.
3. Continue all non-blocked platform/tooling/simulation preparation.
4. Resolve or continue around Contract/Environment gates as evidence becomes available.
5. Start production FunctionSpec implementation only when its required authority chain is verified.

Status policy:
- Repository progress and production qualification are tracked separately.
- An external CI execution failure is recorded as an execution-environment issue unless its logs demonstrate a repository defect.


### Latest non-blocked viewport/runtime slice — 2026-09-19
Implemented in the framework-neutral presentation/rendering preparation path:
- Deferred render work is paged by concrete remaining WorkItems rather than re-planning the completed page.
- Full-surface clear is a one-time page action; later deferred pages continue with fine-grained work.
- Deferred delivery is explicit; unavailable tile work is surfaced for retry instead of being reported as success.
- Delivery retry requeues the failed page ahead of later deferred pages.
- Composite snapshots are reused across pure budget-pagination pages; delivery retries force a fresh Composite refresh so recovered tiles can be loaded.
- Reusable render cache excludes deferred/partial frames.
- Superseded frame generations are rejected before presentation.
- Presentation supports awaitable stop and async disposal; resource release is deferred until an active render loop exits.
- Smoke coverage was extended for deferred paging, tile recovery, reuse safety, lifecycle disposal, and delivery telemetry.

Verification note:
- These changes are repository edits and smoke-test wiring. No local build/test execution result is being asserted from this environment.
- GitHub Actions status for the latest branch commits remains unverified when no workflow run/status is associated with the commit.


### Latest viewport presentation/runtime continuation — 2026-09-19
- Deferred pages are discarded immediately when a newer composite generation is invalidated.
- Delivery retry keeps failed/deferred work at the front of the pending render sequence.
- Render invalidation regions are merged at the adapter boundary; corner-only contacts are intentionally not merged.
- EndFrame finalization remains guaranteed on render failure/cancellation paths.
- Presentation snapshots now retry sampling and expose `IsGenerationStable` instead of claiming atomicity they do not have.
- Smoke coverage now includes touching-region merge behavior, corner-only rejection, failure-path EndFrame finalization, and stable snapshot sampling.

Verification note:
- No local build/test execution result is asserted from this environment.
- Latest commit has no associated GitHub Actions run or commit status at the time of this update.


### Latest viewport runtime hardening — 2026-09-19
- Begin-frame failures now still enter the adapter finalization path so EndFrame is guaranteed by the render contract.
- Deferred delivery keeps the concrete unavailable-work exception for diagnostics instead of returning a reasonless deferred result.
- Explicit frame requeue now deduplicates WorkItems before rebuilding the deferred queue.
- Smoke coverage extends to begin-frame failure finalization, deferred error preservation, and repeated explicit requeue.

### Latest viewport runtime closure — 2026-09-19
- Render delivery now supports partial success: unavailable tile work is collected instead of aborting the whole frame at the first missing tile.
- Deferred delivery preserves the concrete deferred WorkItems and the number of units already rendered in the same attempt.
- Continuous retry requeues only the deferred WorkItems; generic hard failures still retain the full-frame retry path.
- Deferred retry therefore avoids replaying already-rendered tiles while keeping the recovered tile in the same generation.
- Input submission activity is edge-triggered rather than one signal per queued event, reducing stale wakeups after batched input draining.
- The scheduler exposes awaitable render activity; the continuous presentation loop now waits on either input activity or render activity instead of polling during idle periods.
- Added smoke coverage for partial tile delivery/recovery and idle-to-active input wakeup through the Presentation Runtime.

Verification note:
- Repository edits and smoke wiring were completed through the GitHub repository workflow; no local build/test execution result is asserted from this environment.
- The latest branch commits still require external CI/build execution for authoritative compile/test verification.

### Latest viewport presentation/visibility closure — 2026-09-19
- Render batches now expose executable counts for Tile, ROI, Overlay, invalidation, and FullSurface work.
- Delivery results now publish a unified ViewportRenderFrameState containing status, generation, planned/rendered/deferred units, region count, deferred WorkItems, and error state.
- Continuous Runtime retains the last delivery; Presentation Runtime exposes both LastDelivery and LastFrameState.
- Presentation diagnostics snapshots now include LastFrameState so engineering/UI layers do not need to reconstruct frame completeness from lower-level statistics.
- Delivery telemetry now distinguishes partial deferred delivery from ordinary deferred attempts.
- Tile/ROI joint visibility is now consumed by the Render Adapter for Tile delivery instead of being calculated and discarded at the sink boundary.
- Joint visibility provides constant-time Tile lookup and smoke coverage verifies a cross-boundary ROI is associated with every intersecting visible tile.
- End-to-end smoke now covers Batch metrics, partial-frame state, exact deferred retry, Presentation frame-state publication, and Tile/ROI joint visibility.

Verification note:
- Repository implementation and smoke wiring are completed through the GitHub repository workflow.
- No local build/test execution result is asserted from this environment; external CI/build execution remains the authoritative compile/test verification path.

### Latest presentation transaction / reuse closure — 2026-09-19
- Render sinks now have optional transactional Commit/Discard callbacks after EndFrame, keeping the framework-neutral contract compatible with existing sinks through default no-op methods.
- Added ViewportRenderSurfaceRuntime with explicit Idle/Rendering/Presented/Discarded/Disposed state and generation-aware commit/discard semantics.
- Continuous Presentation Runtime owns the render surface and commits it only after successful delivery; failed, cancelled, and deferred frames are discarded instead of being presented.
- Presentation diagnostics now expose the last surface snapshot alongside the last frame state.
- Rendered frames enter reusable-frame cache only after explicit presentation (MarkPresented); RefreshAsync no longer caches frames that have not reached the surface commit stage.
- Reuse smoke coverage now proves an unpresented pipeline frame is not reusable and becomes reusable only after MarkPresented.
- Added surface transaction smoke coverage for successful commit, render failure discard, deferred discard, presentation reset, and best-effort discard cleanup.

Verification note:
- Implementation, integration, and smoke wiring were completed through the repository workflow.
- No local build/test execution result is asserted from this environment.
- The latest branch commits still require actual local/CI build and test execution for authoritative verification.

### Latest viewport surface transaction closure — 2026-09-19
- Render delivery now owns an explicit surface transaction: Begin → Render/EndFrame → Commit on complete success, or Discard on failure/cancellation/deferred delivery.
- Surface snapshots retain the exact Presented Regions for the last successful presentation, so incremental presentation can expose the concrete committed area rather than only a count.
- Commit context now carries exact batch layer counts for Tile, ROI, Overlay, Invalidation, and FullSurface work, allowing framework-specific sinks to consume layer facts without reparsing the WorkPlan.
- Surface discard diagnostics now retain the discarded Generation and delivery status while preserving the last successfully Presented Generation.
- Continuous Frame Runtime connects delivery to the surface transaction and only marks the render frame reusable after successful presentation.
- Smoke coverage now exercises FullSurface initial presentation, ROI incremental presentation, Overlay-only presentation, commit ordering, exact presented regions, commit-layer metrics, and commit failure/discard diagnostics.

Verification note:
- Repository edits and smoke wiring are completed.
- No local build/test execution result is asserted from this environment.
- The current GitHub branch state must still be validated by an actual external build/test run when CI execution is available.

### Latest viewport presentation transaction hardening — 2026-09-19
- Surface presentation now carries exact committed regions and explicit layer counts across Tile, ROI, Overlay, Invalidation, and FullSurface work.
- FullSurface, ROI-incremental, and Overlay-only commits are exercised as separate presentation paths on the same surface generation sequence.
- Surface failures retain the discarded generation and delivery status while the last successful presented generation remains diagnosable.
- Surface reset now clears the complete presentation session history, including generations, sequence, units, regions, and discard diagnostics.
- Fixed an incremental planning bug where an Overlay-only invalidation could replay the complete ROI scene merely because the current SceneDiff was empty.
- Smoke coverage now asserts Overlay-only batches contain no Tile or ROI work, and that surface reset removes all prior presentation state.
- Existing dedicated ViewportRenderSurfaceSmoke was reused for surface-specific scenarios to avoid duplicating ownership across smoke suites.

Verification note:
- Implementation and smoke wiring completed through the repository workflow.
- No local build/test execution result is asserted from this environment.
- GitHub commit status/workflow availability remains the authoritative external verification path.

### Latest viewport generation/reuse/lifecycle hardening — 2026-09-19
- Scheduler now rejects late submissions from older generations at the submission boundary, preventing stale DirtyFlags from contaminating a newer frame.
- Render reuse is generation-monotonic: an older presented frame cannot overwrite a newer cached frame.
- Pipeline refresh/invalidate/build entry points evict cached reuse as soon as a newer Composite generation enters the pipeline, before the newer frame is necessarily presented.
- Presentation snapshots now track model-generation stability and surface-presentation-sequence stability separately; `IsPresentationStable` is true only when both remain unchanged across sampling.
- Scheduler activity signaling is now disposed together with the render pipeline, completing the semaphore/resource lifecycle.
- Reuse smoke now uses actual navigation-generated generations for older-frame overwrite checks.
- Scheduler lifecycle smoke covers stale-generation rejection and explicit scheduler disposal.

Verification note:
- Repository implementation and smoke wiring are complete for this round.
- No local build/test execution result is asserted from this environment.
- The latest GitHub branch still requires external CI/build execution for authoritative compile/test verification.

### Latest viewport presentation fence / transaction hardening — 2026-09-19
- Surface presentation now uses an explicit `ViewportRenderSurfaceTransaction` token with an independent monotonic transaction sequence.
- A surface rejects render generations older than the last successfully presented generation, even if they arrive after Scheduler-level filtering.
- Same-generation retries are allowed, but each retry receives a new transaction sequence so an older discarded token cannot alias a later transaction.
- Delivery now carries the active surface transaction through Commit/Discard paths; stale or concurrent transaction attempts cannot commit over the current Surface state.
- Surface rollback is best-effort during concurrent disposal, preserving the original delivery exception instead of allowing cleanup to mask it.
- Presentation snapshot stability now samples Surface state, RenderingGeneration, RenderingSequence, and PresentationSequence in addition to Composite.Generation.
- Surface smoke now covers stale-generation rejection, same-generation transaction sequence monotonicity, stale token rejection, and concurrent disposal during commit.

Verification note:
- Repository implementation and smoke wiring are complete for this round.
- No local build/test execution result is asserted from this environment.
- GitHub status/workflow results remain unavailable for the current branch and are not being inferred as successful.
### Latest render command stream / presentation queue hardening — 2026-09-19
- Added framework-neutral `ViewportRenderCommandStream` so Pipeline Frames now carry an immutable ordered render command sequence derived from the already-budgeted Render Batch.
- Render Adapter now executes the command stream rather than interpreting the WorkPlan directly, establishing a stable backend-facing render command boundary without Skia/WPF/DevExpress dependencies.
- Added bounded latest-wins `ViewportPresentationQueueRuntime` with explicit pending, single-in-flight, presented, cancel, stale-reject, drop, and monotonic submission-token semantics.
- Continuous presentation now uses `Pipeline Frame -> Presentation Queue -> InFlight -> Render Sink -> Acknowledge/Cancel`, while the existing Surface Transaction remains the backend presentation transaction fence.
- Queue token sequence remains monotonic across reset so an old token cannot alias a post-reset submission.
- Queue state is exposed through `ViewportPresentationRuntime.PresentationQueue` and queue resources are disposed with the presentation runtime.
- Added `ViewportPresentationQueueSmoke` and wired it into the main smoke entry. The smoke covers command-stream/batch alignment, latest-wins coalescing, single in-flight protection, stale generation rejection, cancel, reset token monotonicity, and overlay-only command execution.
- Extended continuous presentation smoke to verify that successful frames travel through the queue to Presented state and that reset clears queue presentation state.
- Verified that the active branch contains all new command-stream/queue files. Accidental copies produced on the default branch during initial file creation were removed from `main`.

Verification note:
- No local build/test execution result is asserted from this environment.
- GitHub commit status and workflow runs for the active HEAD are currently empty; no CI success is inferred.
### Latest double-buffer / frame-fence / region-commit hardening — 2026-09-19
- Added framework-neutral `ViewportPresentationBufferRuntime` with two logical backbuffer slots: one Presented slot and one alternate Rendering slot.
- Backbuffer Begin/Commit/Discard is fenced by the Queue submission Generation + Sequence token; stale generation or older same-generation sequence cannot acquire or commit.
- Backbuffer reset clears presentation state but preserves a monotonic submission-sequence floor, preventing pre-reset callbacks from aliasing a new presentation session.
- Continuous rendering now performs `Queue InFlight -> Backbuffer Begin -> Surface Delivery/Commit -> Backbuffer Commit -> Queue ACK`; failure and deferred paths discard the backbuffer and release the queue InFlight token.
- Backbuffer commits retain the exact command-stream region set and expose region-commit statistics for incremental presentation diagnostics.
- `ViewportPresentationSnapshot` now includes Queue and Buffer snapshots and requires Generation, Surface, Buffer, and Queue stability before reporting `IsPresentationStable`.
- Queue statistics now expose InFlight generation/sequence to make render-thread handoff state observable.
- Added `ViewportPresentationBufferSmoke` and extended continuous/facade smoke to verify Queue, Backbuffer, and Surface reach the same presented generation/sequence and reset together.
- Hardened the Continuous ACK-rejected path so an externally invalidated presentation token cannot leave Queue InFlight permanently occupied.

Verification note:
- No local build/test execution result is asserted from this environment.
- GitHub status checks and workflow runs for the active HEAD are still empty; no CI success is inferred.
### Latest asynchronous presentation execution / stale-frame fence hardening — 2026-09-19
- Added framework-neutral `ViewportPresentationExecutionRuntime` as the logical render-worker boundary. Continuous frame production now enqueues frames while the execution worker independently consumes and presents them.
- `ViewportPresentationQueueRuntime` now exposes activity signaling, in-flight cancellation tokens, current-submission checks, and a monotonic latest submission sequence.
- A newer submission cancels an older in-flight presentation. The cancellation token is linked into Render Delivery so slow render/sink work can be interrupted instead of presenting stale content.
- Surface and Backbuffer Commit now accept the presentation fence and reject superseded submissions before publishing a new Presented state.
- Execution validates the Queue fence before Buffer commit and ACK. Queue ACK itself now rejects any token whose submission sequence is no longer the latest.
- Control-loop activity waiting now includes the presentation worker task, so an unexpected worker exit cannot leave the UI/control loop permanently idle.
- Continuous Runtime smoke now verifies input generation can advance while the render worker is blocked, proving frame production is decoupled from slow presentation execution.
- Added execution-worker smoke for Queue -> Worker wake-up -> Surface/Backbuffer commit -> Queue ACK, plus a stale in-flight supersede scenario where the older frame is cancelled and the newer generation becomes Presented.
- Main smoke entry includes the asynchronous execution coverage.

Verification note:
- No local build/test execution result is asserted from this environment.
- GitHub commit status and workflow runs for the active HEAD remain empty; no CI success is inferred.

### Latest presentation queue / execution diagnostics hardening — 2026-09-19
- Presentation queue supersede cancellation is now triggered outside the queue monitor; cancellation callbacks therefore cannot re-enter queue state mutation while the enqueue transaction is still open.
- Queue diagnostics now expose the monotonic latest submission sequence, allowing presentation stability sampling to distinguish same-generation submissions instead of relying on generation alone.
- Presentation execution now exposes framework-neutral counters for Executed, Presented, Superseded, Cancelled, Deferred, Failed, RenderedUnits, and the last generation/sequence.
- Presentation Snapshot now carries execution diagnostics and an explicit IsExecutionStable flag; IsPresentationStable requires Generation, Surface, Buffer, Queue, and Execution stability together.
- Execution diagnostics reset with the continuous presentation runtime so a new session does not inherit prior execution counters.
- Smoke coverage now verifies supersede cancellation observes the newer pending submission only after enqueue visibility, execution outcome counters, execution stability, and reset clearing of execution diagnostics.

Verification note:
- Changes are implemented and smoke wiring is updated on the active branch.
- No local build/test execution result is asserted from this environment.
- GitHub Actions status/workflow execution remains unverified unless a run is explicitly associated with the current commit.

### Latest presentation commit-window hardening — 2026-09-19
- Presentation Queue now exposes an explicit commit window: a current InFlight submission must enter TryBeginCommit before backend publication.
- Once the commit window is active, newer submissions remain visible as Pending but do not cancel the active backend commit; they are scheduled for the next presentation cycle.
- Added IsCommitCurrent, TryCompleteCommit, and TryAbortCommit so Surface/Backbuffer publication and Queue acknowledgement share the same submission token.
- Render Delivery now accepts a begin-commit callback immediately before the backend CommitFrameAsync boundary; the execution worker uses this callback to establish the commit window and then fences Surface/Backbuffer publication against the commit token.
- Existing TryAcknowledgePresented semantics remain backward compatible for callers that do not use the explicit commit window.
- Queue lifecycle cancellation during Reset/Dispose is now performed outside the Queue monitor so cancellation callbacks cannot re-enter Queue state while cleanup is still holding the monitor.
- Queue diagnostics expose commit-window generation/sequence and Presentation Snapshot stability now samples that state.
- Added queue-level and end-to-end execution smoke coverage proving that a newer frame arriving during a stalled backend commit cannot cancel the active publication; it remains Pending and is presented afterward.
- Commit-window smoke also verifies reset clears the active commit diagnostics without resetting the monotonic submission-token domain.

Verification note:
- Repository implementation and smoke wiring are updated on the active branch.
- Local repository build could not be executed because the execution environment has neither repository network access nor the dotnet CLI.
- No GitHub Actions success is inferred; external workflow/status execution remains unverified unless an associated run exists.

### Latest 100-round continuous non-blocked development — 2026-09-19
Implemented and wired into the repository's existing smoke entry:
- Deterministic render evidence fingerprints for Batch, CommandStream, PipelineFrame and ReplayOperation traces.
- Replay evidence now records Commit/Discard transaction boundaries and exposes a deterministic EvidenceHash.
- Centralized structural invariant checks cover WorkPlan, Batch, CommandStream, FrameState, Delivery, Queue, Buffer, Surface, Replay, Input, Backpressure, VisibleRegion, TileFrame, Scene, SceneDiff, Workflow, ZoomProfile, PlanMetrics and FrameMetrics.
- Added dedicated smoke coverage for navigation primitives, transactional workflow replay, Scene/Visibility behavior, input/backpressure edge cases, render planning/budgeting, and presentation evidence accounting.
- Fixed two concrete platform-source defects discovered during this round: malformed duplicate code in AsyncPipeline.ContainsNode and invalid boolean handling of Task.WaitAsync in AsyncSignal.
- Added targeted platform invariant smoke coverage for AsyncSignal, AsyncPipeline, BoundedWorkQueue, ResourceLeasePool, Percentiles, RunningStatistics and OperationTimeout.
- The 100-round execution ledger is stored under docs/09-development-plan/PHASE1_100_ROUND_LEDGER_20260919.md.

Verification note:
- These are repository edits and smoke wiring. No local C# build/test result is asserted from this environment.
- The latest branch commit checked through the available GitHub Actions/status interfaces has no associated workflow run/status, so CI/build success remains unverified.
- Existing HALCON 25.11, DevExpress 25.2.3 and hardware-vendor environment/authority gates remain recorded and were not bypassed.
