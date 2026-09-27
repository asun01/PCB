# PCB Project Progress

**Canonical branch:** `main`  
**Canonical repository:** `asun01/PCB`  
**Development branch:** `codex/phase1-nonblocked-automation-20260919`  
**Current main HEAD:** `824d1bc69116059c442e5672877dffb37d35794f`  
**Migration target:** `16318948605231/Asun-PCB`

## Current state

### 1. Client implementation chain

The repository contains the continuing real-client chain:

`Inspection → Production → Result → Quality → Replay → Release → Reset/Recovery → Unified Projection → WPF Client`

Recent work closed additional Quality/Results/WPF projection seams and retained the authority distinction between current execution, selected history, Quality, Replay and Release.

### 2. Device specification system

`docs/14-device-specs/` is now the device specification layer.

Current device families include:
- 2D AOI
- 3D AOI
- 3D SPI
- 2D X-Ray
- 3D AXI/CT
- Optical/Laser Metrology
- ICT
- FCT
- Bare-Board PCB Inspection

Shared capability and device-template surfaces are kept separate from device-specific semantics.

### 3. Golden device: 3D SPI

Location:

`docs/14-device-specs/03d-spi/`

Eight detailed feature specifications exist:

- 03D-SPI-F001 PastePresence
- 03D-SPI-F002 PasteArea
- 03D-SPI-F003 PasteHeight
- 03D-SPI-F004 PasteVolume
- 03D-SPI-F005 PasteOffset
- 03D-SPI-F006 PasteShape
- 03D-SPI-F007 PasteCoplanarity
- 03D-SPI-F008 BridgingRisk

Cross-cutting specification surfaces include Feature Execution Contract, Feature Traceability, Parameter Authority and Qualification Gates.

### 4. Validation

`tools/validate_device_spec_catalog.py` validates the required device specification hierarchy and, for 3D SPI, requires one detailed specification per feature plus the required contract/authority sections.

A repository-grounded structural audit after the latest 3D SPI expansion found:
- 8/8 feature specifications present
- 8/8 required feature sections present
- 4/4 golden cross-cutting surfaces present

This is a structural/documentation audit, not hardware qualification.

### 5. Authority gates

The following remain explicitly non-guessed and require authoritative sources where applicable:

- hardware behavior and timing
- vendor SDK/protocol semantics
- calibration/metrology qualification
- accuracy/repeatability/uncertainty
- production thresholds
- defect acceptance rules
- safety/interlock semantics
- production Quality/Release gates

### 6. Current continuation direction

Continue from the canonical `main` state. The next work should deepen the golden 3D SPI specification and its mapping into Feature Contract/Domain/Runtime/Program-Recipe/Execution/Result/Quality/Evidence/Replay/WPF, while using the validated structure to expand other device families.

Do not restart the project. Do not create another PCB repository. Do not confuse this project with AsunMeasure.

## Migration status

The source repository has an existing target repository:

`https://github.com/16318948605231/Asun-PCB`

The target repository currently exists but the connected GitHub account has read-only access to it. A full push/copy therefore requires write permission to the target repository. Do not claim the migration is complete until the target `main` has been verified against the source canonical state.
