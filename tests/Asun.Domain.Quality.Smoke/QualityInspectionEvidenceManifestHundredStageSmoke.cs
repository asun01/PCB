using Asun.Domain.Quality;

public static class QualityInspectionEvidenceManifestHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var f1=new QualityFinding(
            QualityFindingId.Create("F-002"),
            "RULE",
            QualityOutcome.Review,
            QualitySeverity.Information,
            "second");
        var f2=new QualityFinding(
            QualityFindingId.Create("F-001"),
            "RULE",
            QualityOutcome.Review,
            QualitySeverity.Information,
            "first");
        var snapshot=new QualityInspectionSnapshot(
            Guid.NewGuid(),
            30,
            new QualityFindingSet(new[]{f1,f2}),
            new QualityFindingEvidenceSet(new[]{
                new QualityFindingEvidenceLink(f1.Id,QualityEvidenceKey.Create("z")),
                new QualityFindingEvidenceLink(f2.Id,QualityEvidenceKey.Create("a"))
            }));
        var manifest=QualityInspectionEvidenceManifestRuntime.Create(snapshot);
        var invalid=new QualityInspectionEvidenceManifest(new[]{
            new QualityFindingEvidenceLink(
                QualityFindingId.Create("F-999"),
                QualityEvidenceKey.Create("frame://999"))
        });

        for(var i=0;i<10;i++) Check(manifest.Count==2,$"manifest count round {i+1} should match snapshot evidence.");
        for(var i=0;i<10;i++) Check(manifest.Links[0].FindingId.Value=="F-001",$"manifest finding ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(manifest.Links[0].EvidenceKey.Value=="a",$"manifest evidence ordering round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(QualityInspectionEvidenceManifestValidationRuntime.IsValid(snapshot,manifest),$"manifest validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(QualityInspectionEvidenceManifestValidationRuntime.Validate(snapshot,manifest).Count==0,$"valid manifest diagnostics round {i+1} should remain empty.");
        for(var i=0;i<10;i++) Check(!QualityInspectionEvidenceManifestValidationRuntime.IsValid(snapshot,invalid),$"orphan manifest round {i+1} should be rejected.");
        for(var i=0;i<10;i++) Check(manifest.Links.SequenceEqual(manifest.Links.OrderBy(link=>link.FindingId.Value,StringComparer.Ordinal).ThenBy(link=>link.EvidenceKey.Value,StringComparer.Ordinal)),$"manifest ordering round {i+1} should be canonical.");
        for(var i=0;i<10;i++) Check(manifest.Links.Select(link=>link.EvidenceKey.Value).Distinct(StringComparer.Ordinal).Count()==2,$"manifest evidence uniqueness round {i+1} should remain explicit.");
        for(var i=0;i<10;i++) Check(snapshot.Evidence.Links.Count==2,$"snapshot evidence source round {i+1} should remain unchanged.");
        for(var i=0;i<10;i++) Check(manifest.Links[1].FindingId==f1.Id && manifest.Links[1].EvidenceKey.Value=="z",$"manifest second relationship round {i+1} should remain deterministic.");

        assert(round==100,$"Quality inspection evidence manifest smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
