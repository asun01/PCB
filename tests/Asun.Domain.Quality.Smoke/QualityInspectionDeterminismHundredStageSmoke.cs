using Asun.Domain.Quality;

public static class QualityInspectionDeterminismHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        QualityFinding Make(string id,string message) =>
            new(QualityFindingId.Create(id),"RULE",QualityOutcome.Review,QualitySeverity.Information,message);

        var a=Make("F-002","b");
        var b=Make("F-001","a");

        QualityInspectionSnapshot MakeSnapshot(
            long sequence,
            bool reverseOrder,
            string evidenceForA="z") =>
            new(
                Guid.NewGuid(),
                sequence,
                new QualityFindingSet(
                    reverseOrder ? new[]{b,a} : new[]{a,b}),
                new QualityFindingEvidenceSet(
                    reverseOrder
                        ? new[]{
                            new QualityFindingEvidenceLink(b.Id,QualityEvidenceKey.Create("a")),
                            new QualityFindingEvidenceLink(a.Id,QualityEvidenceKey.Create(evidenceForA))
                        }
                        : new[]{
                            new QualityFindingEvidenceLink(a.Id,QualityEvidenceKey.Create(evidenceForA)),
                            new QualityFindingEvidenceLink(b.Id,QualityEvidenceKey.Create("a"))
                        }));

        var first=MakeSnapshot(1,false);
        var second=MakeSnapshot(2,true);
        var changedFinding=new QualityInspectionSnapshot(
            Guid.NewGuid(),
            3,
            new QualityFindingSet(new[]{a,b with {Message="changed"}}),
            first.Evidence);
        var changedEvidence=MakeSnapshot(4,false,"changed");
        var firstFingerprint=QualityInspectionDeterminismRuntime.CreateContentFingerprint(first);
        var secondFingerprint=QualityInspectionDeterminismRuntime.CreateContentFingerprint(second);
        var changedFindingFingerprint=QualityInspectionDeterminismRuntime.CreateContentFingerprint(changedFinding);
        var changedEvidenceFingerprint=QualityInspectionDeterminismRuntime.CreateContentFingerprint(changedEvidence);

        for(var i=0;i<10;i++) Check(firstFingerprint.Length==64,$"first fingerprint length round {i+1} should be SHA-256 sized.");
        for(var i=0;i<10;i++) Check(secondFingerprint.Length==64,$"second fingerprint length round {i+1} should be SHA-256 sized.");
        for(var i=0;i<10;i++) Check(firstFingerprint==secondFingerprint,$"canonical content round {i+1} should ignore insertion order, identity and sequence.");
        for(var i=0;i<10;i++) Check(QualityInspectionDeterminismRuntime.AreContentEquivalent(first,second),$"content equivalence round {i+1} should be deterministic.");
        for(var i=0;i<10;i++) Check(QualityInspectionDeterminismValidationRuntime.IsValidFingerprint(firstFingerprint),$"fingerprint validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(firstFingerprint.All(character=>Uri.IsHexDigit(character) && char.ToLowerInvariant(character)==character),$"fingerprint casing round {i+1} should remain normalized lowercase hex.");
        for(var i=0;i<10;i++) Check(firstFingerprint!=changedFindingFingerprint,$"finding mutation round {i+1} should change content fingerprint.");
        for(var i=0;i<10;i++) Check(firstFingerprint!=changedEvidenceFingerprint,$"evidence mutation round {i+1} should change content fingerprint.");
        for(var i=0;i<10;i++) Check(QualityInspectionDeterminismValidationRuntime.IsValidSnapshot(first) && QualityInspectionDeterminismValidationRuntime.IsValidSnapshot(second),$"snapshot fingerprint validation round {i+1} should pass.");
        for(var i=0;i<10;i++) Check(QualityInspectionDiffRuntime.Diff(first,first).IsEmpty,$"deterministic self-diff round {i+1} should remain empty.");

        assert(round==100,$"Quality inspection determinism smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
