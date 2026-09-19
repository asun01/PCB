using Asun.Domain.Quality;

public static class QualityFindingEvidenceLinkValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round=0;
        void Check(bool condition,string message){ round++; assert(condition,$"Round {round}: {message}"); }

        var link = new QualityFindingEvidenceLink(
            QualityFindingId.Create("F-001"),
            QualityEvidenceKey.Create("frame://001"));
        var invalidFinding = link with { FindingId = new QualityFindingId(" ") };
        var invalidEvidence = link with { EvidenceKey = new QualityEvidenceKey(" ") };

        for(var i=0;i<10;i++) Check(link.IsValid,$"valid link round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(QualityFindingEvidenceValidationRuntime.IsValid(link),$"link validator round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(link.FindingId.Value=="F-001",$"finding reference round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(link.EvidenceKey.Value=="frame://001",$"evidence reference round {i+1} should remain stable.");
        for(var i=0;i<10;i++) Check(!invalidFinding.IsValid,$"invalid finding link round {i+1} should fail.");
        for(var i=0;i<10;i++) Check(!invalidEvidence.IsValid,$"invalid evidence link round {i+1} should fail.");
        for(var i=0;i<10;i++) Check(QualityFindingEvidenceValidationRuntime.Validate(invalidFinding).Count>0,$"invalid finding evidence validation round {i+1} should produce evidence.");
        for(var i=0;i<10;i++) Check(QualityFindingEvidenceValidationRuntime.Validate(invalidEvidence).Count>0,$"invalid evidence key validation round {i+1} should produce evidence.");
        for(var i=0;i<10;i++) Check(link==new QualityFindingEvidenceLink(QualityFindingId.Create("F-001"),QualityEvidenceKey.Create("frame://001")),$"link equality round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(link!=invalidFinding && link!=invalidEvidence,$"invalid link variants round {i+1} should remain distinct.");

        assert(round==100,$"Quality finding-evidence link smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
