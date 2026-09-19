var failures = new List<string>();

void Assert(bool condition, string message)
{
    if (!condition)
        failures.Add(message);
}

QualityOutcomeValidationHundredStageSmoke.Run(Assert);
QualitySeverityValidationHundredStageSmoke.Run(Assert);
QualityFindingIdValidationHundredStageSmoke.Run(Assert);
QualityFindingValidationHundredStageSmoke.Run(Assert);
QualityFindingSetValidationHundredStageSmoke.Run(Assert);
QualityEvidenceKeyValidationHundredStageSmoke.Run(Assert);
QualityFindingEvidenceLinkValidationHundredStageSmoke.Run(Assert);
QualityFindingEvidenceSetValidationHundredStageSmoke.Run(Assert);
QualityFindingEvidenceIndexValidationHundredStageSmoke.Run(Assert);
QualityFindingEvidenceCrossReferenceValidationHundredStageSmoke.Run(Assert);
QualityInspectionSnapshotValidationHundredStageSmoke.Run(Assert);
QualityInspectionDiffValidationHundredStageSmoke.Run(Assert);
QualityInspectionDiffRuntimeHundredStageSmoke.Run(Assert);
QualityInspectionSequenceValidationHundredStageSmoke.Run(Assert);
QualityInspectionDeterminismHundredStageSmoke.Run(Assert);
QualityInspectionResultValidationHundredStageSmoke.Run(Assert);
QualityInspectionEvidenceDiffHundredStageSmoke.Run(Assert);
QualityInspectionAuditHundredStageSmoke.Run(Assert);
QualityInspectionChainValidationHundredStageSmoke.Run(Assert);
QualityInspectionAuditDiffHundredStageSmoke.Run(Assert);
QualityInspectionEvidenceManifestHundredStageSmoke.Run(Assert);
QualityInspectionReplayProjectionHundredStageSmoke.Run(Assert);
QualityInspectionReplayProjectionDiffHundredStageSmoke.Run(Assert);
QualityInspectionReplayBundleHundredStageSmoke.Run(Assert);
QualityInspectionReplayBundleFingerprintHundredStageSmoke.Run(Assert);
QualityInspectionReplayEnvelopeHundredStageSmoke.Run(Assert);
QualityInspectionReplayWindowHundredStageSmoke.Run(Assert);
QualityInspectionReplayWindowDiffHundredStageSmoke.Run(Assert);
QualityInspectionOutcomeSummaryHundredStageSmoke.Run(Assert);
QualityInspectionSeveritySummaryHundredStageSmoke.Run(Assert);
QualityInspectionEvidenceSummaryHundredStageSmoke.Run(Assert);
QualityInspectionSummaryHundredStageSmoke.Run(Assert);
QualityInspectionSummaryDiffHundredStageSmoke.Run(Assert);
QualityInspectionRuleSummaryHundredStageSmoke.Run(Assert);
QualityInspectionRuleSummaryDiffHundredStageSmoke.Run(Assert);
QualityInspectionRuleFindingIndexHundredStageSmoke.Run(Assert);
QualityInspectionRuleEvidenceIndexHundredStageSmoke.Run(Assert);
QualityInspectionRuleAuditProjectionHundredStageSmoke.Run(Assert);
QualityInspectionRuleAuditProjectionFingerprintHundredStageSmoke.Run(Assert);
QualityInspectionFindingAuditRecordHundredStageSmoke.Run(Assert);
QualityInspectionFindingAuditIndexHundredStageSmoke.Run(Assert);
QualityInspectionFindingAuditIndexFingerprintHundredStageSmoke.Run(Assert);
QualityInspectionFindingAuditProjectionHundredStageSmoke.Run(Assert);
QualityInspectionFindingAuditProjectionDiffHundredStageSmoke.Run(Assert);
QualityInspectionFindingAuditEnvelopeHundredStageSmoke.Run(Assert);
QualityInspectionFindingAuditWindowHundredStageSmoke.Run(Assert);
QualityInspectionFindingAuditWindowDiffHundredStageSmoke.Run(Assert);
QualityInspectionFindingAuditReplayBundleHundredStageSmoke.Run(Assert);
QualityInspectionAuditProjectionHundredStageSmoke.Run(Assert);
QualityInspectionAuditProjectionFingerprintHundredStageSmoke.Run(Assert);
QualityInspectionAuditEnvelopeHundredStageSmoke.Run(Assert);

if (failures.Count > 0)
{
    foreach (var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure}");

    return 1;
}

Console.WriteLine("Asun.Domain.Quality smoke tests passed.");
return 0;
