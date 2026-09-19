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
