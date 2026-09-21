# Smoke Registry

| Smoke | Owner | Scope | Execution evidence |
|---|---|---|---|
| ProductionSessionProgressAcceptanceSmoke.Run100Stages | Asun.Platform.ClientIntegration | Ready → Running progress → Completed; Cancelled; Failed; Reset; target count; sequence; frame metadata; adapter routing | Source registered; execution not claimed |
| ClientProductionProgressPresentationSmoke.Run100Stages | Asun.Platform.ClientIntegration | Client-visible Production status/session/frame progress projection | Source registered; execution not claimed |
| ClientInspectionExecutionPresentationSmoke.Run100Stages | Asun.Platform.ClientIntegration | Program/status/progress/ROI/Acquisition/result presentation boundary | Source registered; execution not claimed |
| ClientInspectionExecutionSurfaceSmoke.Run100Stages | Asun.Platform.ClientIntegration | Unified Inspection execution surface; result/ROI/Acquisition availability projection | Source registered; execution not claimed |
| ClientInspectionExecutionCommandSmoke.Run100Stages | Asun.Platform.ClientIntegration | Inspection preview/run/cancel/reset/ROI command facade over existing workspace authority | Source registered; execution not claimed |
| ClientAcquisitionPreviewFaultSmoke.Run100Stages | Asun.Platform.ClientIntegration | Acquisition preview lifecycle, stale-preview invalidation, fault/recovery, metadata/fingerprint | Source registered; execution not claimed |
| ClientInspectionAcquisitionBindingEventSmoke.Run100Stages | Asun.Platform.ClientIntegration | Direct Acquisition binding event propagation and authority isolation | Source registered; execution not claimed |
| ClientInspectionAcquisitionPreviewSurfaceSmoke.Run100Stages | Asun.Platform.ClientIntegration | Inspection Surface raw Acquisition preview payload/metadata projection and stale-preview protection | Source registered; execution not claimed |
| ClientInspectionWorkspaceChangedSmoke.Run100Stages | Asun.Platform.ClientIntegration | Full Inspection snapshot change stream across Acquisition/Quality/History/Session boundaries | Source registered; execution not claimed |
| ClientResultsCommandSmoke.Run100Stages | Asun.Platform.ClientIntegration | Results workspace history-selection command facade over existing bounded Run History authority | Source registered; execution not claimed |
| ClientResultsSurfaceSmoke.Run100Stages | Asun.Platform.ClientIntegration | Unified current-result/history-selection surface over existing Result/Replay/Release/Run History projections | Source registered; execution not claimed |
| ClientQualityCommandSmoke.Run100Stages | Asun.Platform.ClientIntegration | Quality finding selection/clear command facade over existing Quality workspace authority | Source registered; execution not claimed |
| ClientQualitySurfaceSmoke.Run100Stages | Asun.Platform.ClientIntegration | Unified Quality result/finding/selection projection over existing validated Quality authority | Source registered; execution not claimed |

## Structural contract

- 10 loop groups
- 100 rounds per group
- explicit round==100
- 10 actual Check(...) call sites
- non-tautological assertions
- no TODO
- no NotImplementedException
