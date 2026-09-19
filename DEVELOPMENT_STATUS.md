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
