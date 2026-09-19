using Asun.Domain.Quality;

public static class QualityInspectionAuditHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var finding=new QualityFinding(
            QualityFindingId.Create("F-001"),
            "RULE",
            QualityOutcome.Review,
            QualitySeverity.Information,
            "audit finding");
        var findings=new QualityFindingSet(new[]{finding});
        var evidence=new QualityFindingEvidenceSet(new[]{
            new QualityFindingEvidenceLink(
                finding.Id,
                QualityEvidenceKey.Create("frame://001"))
        });
        var snapshot=new QualityInspectionSnapshot(
            Guid.NewGuid(),
            12,
            findings,
            evidence);
        var result=new QualityInspectionResult(
            Guid.NewGuid(),
            snapshot);
        var record=QualityInspectionAuditRuntime.Create(result);
        var invalid=new QualityInspectionAuditRecord(
            record.ResultId,
            record.SnapshotId,
            -1,
            record.FindingCount,
            record.EvidenceLinkCount,
            record.ContentFingerprint);

        for(var i=0;i<10;i++) Check(QualityInspectionAuditValidationRuntime.IsValid(record),$"audit validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(record.ResultId==result.ResultId,$"audit result identity round {i+1} should match.");
        for(var i=0;i<10;i++) Check(record.SnapshotId==snapshot.SnapshotId,$"audit snapshot identity round {i+1} should match.");
        for(var i=0;i<10;i++) Check(record.Sequence==snapshot.Sequence,$"audit sequence round {i+1} should match.");
        for(var i=0;i<10;i++) Check(record.FindingCount==1 && record.EvidenceLinkCount==1,$"audit content counts round {i+1} should match.");
        for(var i=0;i<10;i++) Check(record.ContentFingerprint.Length==64,$"audit fingerprint round {i+1} should remain SHA-256 sized.");
        for(var i=0;i<10;i++) Check(record.ContentFingerprint==QualityInspectionResultDeterminismRuntime.CreateContentFingerprint(result),$"audit fingerprint determinism round {i+1} should be stable.");
        for(var i=0;i<10;i++) Check(!QualityInspectionAuditValidationRuntime.IsValid(invalid),$"negative audit sequence round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(QualityInspectionAuditValidationRuntime.Validate(record).Count==0,$"audit diagnostics round {i+1} should remain empty for valid data.");
        for(var i=0;i<10;i++) Check(record.ContentFingerprint.All(character=>Uri.IsHexDigit(character) && char.ToLowerInvariant(character)==character),$"audit fingerprint normalization round {i+1} should remain lowercase hexadecimal.");

        assert(round==100,$"Quality inspection audit smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
