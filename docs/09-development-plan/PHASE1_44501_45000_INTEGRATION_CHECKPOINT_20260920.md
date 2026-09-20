# Phase 1 — Stages 44501–45000 Integration Checkpoint — 2026-09-20

## Boundary

Closed the Acquisition/Capture → Evidence canonical projection hardening cell.

## Product-chain result

Added `ProductionCaptureEvidenceCanonicalRuntime`.

The runtime derives deterministic per-frame and projection-level SHA-256 fingerprints from capture sequence, payload identity, dimensions, pixel format, and opaque Evidence handles. It validates the existing Production → Capture provenance → Evidence projection and rejects count mismatch, duplicate/missing frames, malformed payload identity, invalid handles, and blank/invalid capture metadata.

Canonicalization is independent of physical Evidence storage and keeps Evidence handles opaque.

## Acceptance evidence

Five 100-stage ledgers and five dedicated 100-round Smokes are registered in `tests/Asun.Platform.CaptureEvidenceIntegration.Smoke/Program.cs`.

Static audit: each new Smoke has 10 loop groups, 100 meaningful Check calls, explicit `round==100`, balanced delimiters, and no TODO/NotImplementedException.

No authoritative build/test/CI result is claimed.
