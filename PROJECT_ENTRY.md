# PCB Project Entry Point

> **唯一项目入口文件。新会话开始后，只需要先读取本文件，再读取其指向的项目进度文件，即可恢复项目上下文。**

## Repository

- Current repository: `asun01/PCB`
- Migration target: `16318948605231/Asun-PCB`
- Development branch retained for historical continuity: `codex/phase1-nonblocked-automation-20260919`
- Canonical delivery branch: `main`
- **Current canonical project state is main.**

## Mandatory continuation order

1. Read this file: `PROJECT_ENTRY.md`
2. Read progress: `PROJECT_PROGRESS.md`
3. Read `docs/09-development-plan/PHASE1_PROGRESS.md`
4. Inspect the current `main` tree before making changes.
5. Continue implementation from the existing state; do not restart or create a replacement repository.

## Product chain

`Inspection → Production → Result → Quality → Replay → Release → Reset/Recovery → Unified Projection → WPF Client`

Current major specification track:

`Device → Capability → Inspection Feature → Algorithm → Measurement → Finding → Result → Quality → Evidence → Replay → Release → Client`

## Device specification policy

Device-specific semantics belong under `docs/14-device-specs/<device>/`. Truly cross-device contracts belong in shared/platform documentation. Production parameters, accuracy, thresholds, hardware behavior, vendor protocols and qualification claims require authoritative sources and must not be invented.

The current golden device specification is:

`docs/14-device-specs/03d-spi/`

## Source-of-truth rule

The repository files and Git history are authoritative. Chat summaries are not authoritative project state. Never claim Build/Test/CI success unless an actual execution result exists.

## Branch rule

All future delivery work should be committed to the long-lived development branch when development isolation is required, then merged into `main`. `main` is the canonical delivery state and the branch to migrate/use for the replacement repository.

## Migration

The full canonical project state is intended to move from `asun01/PCB` to `16318948605231/Asun-PCB`. After migration, this entry file remains the starting point for new sessions, with the repository identity updated only after the target repository is confirmed complete.
