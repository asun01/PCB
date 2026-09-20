# Smoke Registry

| Smoke | Owner | Scope | Execution evidence |
|---|---|---|---|
| ProductionSessionProgressAcceptanceSmoke.Run100Stages | Asun.Platform.ClientIntegration | Ready → Running progress → Completed; Cancelled; Failed; Reset; target count; sequence; frame metadata; adapter routing | Source registered; execution not claimed |
| ClientProductionProgressPresentationSmoke.Run100Stages | Asun.Platform.ClientIntegration | Client-visible Production status/session/frame progress projection | Source registered; execution not claimed |
| ClientInspectionExecutionPresentationSmoke.Run100Stages | Asun.Platform.ClientIntegration | Program/status/progress/ROI/Acquisition/result presentation boundary | Source registered; execution not claimed |
| ClientInspectionExecutionSurfaceSmoke.Run100Stages | Asun.Platform.ClientIntegration | Unified Inspection execution surface; result/ROI/Acquisition availability projection | Source registered; execution not claimed |

## Structural contract

- 10 loop groups
- 100 rounds per group
- explicit round==100
- 10 actual Check(...) call sites
- non-tautological assertions
- no TODO
- no NotImplementedException
