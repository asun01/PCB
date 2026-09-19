using Asun.Domain.Quality;

public static class QualityFindingEvidenceCrossReferenceValidationHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){ round++; assert(condition,$"Round {round}: {message}"); }

        var finding1=new QualityFinding(
            QualityFindingId.Create("F-001"),
            "AOI.CLEARANCE",
            QualityOutcome.Fail,
            QualitySeverity.Warning,
            "Clearance finding.");
        var finding2=new QualityFinding(
            QualityFindingId.Create("F-002"),
            "SPI.VOLUME",
            QualityOutcome.Review,
            QualitySeverity.Information,
            "Volume requires review.");
        var findings=new QualityFindingSet(new[]{finding1,finding2});
        var links=new QualityFindingEvidenceSet(new[]{
            new QualityFindingEvidenceLink(finding1.Id,QualityEvidenceKey.Create("frame://001")),
            new QualityFindingEvidenceLink(finding2.Id,QualityEvidenceKey.Create("trace://002"))
        });
        var orphanLinks=new QualityFindingEvidenceSet(new[]{
            new QualityFindingEvidenceLink(QualityFindingId.Create("F-999"),QualityEvidenceKey.Create("frame://999"))
        });

        for(var i=0;i<10;i++) Check(QualityFindingEvidenceValidationRuntime.IsValid(findings,links),$"cross-reference validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(!QualityFindingEvidenceValidationRuntime.IsValid(findings,orphanLinks),$"orphan link validation round {i+1} should fail.");
        for(var i=0;i<10;i++) Check(QualityFindingEvidenceValidationRuntime.Validate(findings,orphanLinks).Count>0,$"orphan evidence diagnostics round {i+1} should be non-empty.");
        for(var i=0;i<10;i++) Check(links.ForFinding(finding1.Id).Count==1,$"finding one coverage round {i+1} should contain one link.");
        for(var i=0;i<10;i++) Check(links.ForFinding(finding2.Id).Count==1,$"finding two coverage round {i+1} should contain one link.");
        for(var i=0;i<10;i++) Check(links.ForFinding(finding1.Id).Single().Value=="frame://001",$"finding one evidence round {i+1} should remain opaque.");
        for(var i=0;i<10;i++) Check(links.ForFinding(finding2.Id).Single().Value=="trace://002",$"finding two evidence round {i+1} should remain opaque.");
        for(var i=0;i<10;i++) Check(findings.Find(finding1.Id)==finding1 && findings.Find(finding2.Id)==finding2,$"finding lookup round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(findings.Find(QualityFindingId.Create("F-999")) is null,$"missing finding round {i+1} should remain absent.");
        for(var i=0;i<10;i++) Check(findings.Findings.Select(item=>item.Id).Distinct().Count()==findings.Count,$"finding identity round {i+1} should remain unique.");

        assert(round==100,$"Quality finding-evidence cross-reference smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
