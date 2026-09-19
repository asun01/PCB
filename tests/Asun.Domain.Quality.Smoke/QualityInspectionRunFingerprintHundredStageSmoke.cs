using Asun.Domain.Quality;

public static class QualityInspectionRunFingerprintHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        static QualityInspectionResult Result(
            string findingId,
            long sequence,
            QualityOutcome outcome)
        {
            var finding=new QualityFinding(
                QualityFindingId.Create(findingId),
                "RULE",
                outcome,
                QualitySeverity.Warning,
                "Observation.");
            var snapshot=new QualityInspectionSnapshot(
                Guid.NewGuid(),
                sequence,
                new QualityFindingSet(new[]{finding}),
                new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()));
            return new QualityInspectionResult(Guid.NewGuid(),snapshot);
        }

        var run=QualityInspectionRunRuntime.Create(
            Guid.NewGuid(),
            new[]{Result("F-001",1,QualityOutcome.Pass),Result("F-002",2,QualityOutcome.Fail)});
        var fingerprint=QualityInspectionRunFingerprintRuntime.CreateFingerprint(run);
        var duplicateFingerprint=QualityInspectionRunFingerprintRuntime.CreateFingerprint(
            QualityInspectionRunRuntime.Create(
                run.RunId,
                run.Results.ToArray()));
        var changedRun=QualityInspectionRunRuntime.Create(
            run.RunId,
            new[]{
                run.Results[0],
                Result("F-003",2,QualityOutcome.Fail)
            });

        for(var i=0;i<10;i++) Check(fingerprint.Length==64,"Run fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(fingerprint.All(Uri.IsHexDigit),"Run fingerprint should be hexadecimal.");
        for(var i=0;i<10;i++) Check(QualityInspectionRunFingerprintValidationRuntime.IsValid(run,fingerprint),"Run fingerprint should validate.");
        for(var i=0;i<10;i++) Check(fingerprint==duplicateFingerprint,"Run fingerprint should be deterministic.");
        for(var i=0;i<10;i++) Check(!QualityInspectionRunFingerprintValidationRuntime.IsValid(changedRun,fingerprint),"Run mutation should invalidate the original fingerprint.");
        for(var i=0;i<10;i++) Check(fingerprint.All(character=>char.ToLowerInvariant(character)==character),"Run fingerprint should remain lowercase.");
        for(var i=0;i<10;i++) Check(run.ResultCount==2,"Run source should retain two results.");
        for(var i=0;i<10;i++) Check(run.Results.Select(result=>result.SnapshotId).Distinct().Count()==2,"Run snapshots should remain unique.");
        for(var i=0;i<10;i++) Check(run.Results.Select(result=>result.ResultId).Distinct().Count()==2,"Run result ids should remain unique.");
        for(var i=0;i<10;i++) Check(QualityInspectionRunValidationRuntime.IsValid(run),"Fingerprint source run should remain valid.");

        assert(round==100,$"Quality inspection run fingerprint smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
