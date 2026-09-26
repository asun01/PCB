# Device Inspection Specifications

This directory defines the device-oriented specification layer for the PCB/PCBA inspection platform.

Authoritative chain: Device Type → Device Capability → Inspection Feature → Algorithm/Measurement → Result → Quality → Evidence → Replay → Release.

Every device family has its own subdirectory. Device documents specialize generic contracts in docs/12-function-specs and never invent vendor SDK behavior, hardware timing, metrology tolerances, defect thresholds, or production acceptance criteria.

Initial families: 2D AOI, 3D AOI, 3D SPI, 2D X-Ray, 3D AXI/CT, Optical/Laser Metrology, ICT, FCT, Bare-Board PCB Inspection.

Capability documentation is not production qualification. Model-specific behavior, accuracy, repeatability, throughput, safety, algorithm performance, and acceptance thresholds require authoritative evidence.

Each device directory grows toward README.md, DEVICE_SPEC.md, CAPABILITIES.md, FEATURES.md, MEASUREMENTS.md, QUALITY.md, UI_WORKFLOW.md, VENDOR_ADAPTER.md, and qualification/evidence records.