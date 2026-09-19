using Asun.Domain.Quality;

public static class QualityEvidenceKeyValidationHundredStageSmoke
{
    public static void Run(Action<bool, string> assert)
    {
        var round = 0;
        void Check(bool condition, string message)
        {
            round++;
            assert(condition, $"Round {round}: {message}");
        }

        var key = QualityEvidenceKey.Create(" image://FRAME-001 ");
        var blank = new QualityEvidenceKey(" ");
        var rejected = false;

        try { _ = QualityEvidenceKey.Create(" "); }
        catch (ArgumentException) { rejected = true; }

        for (var i=0;i<10;i++) Check(key.IsValid && key.Value=="image://FRAME-001",$"normalized evidence key round {i+1} should be stable.");
        for (var i=0;i<10;i++) Check(key.ToString()=="image://FRAME-001",$"evidence key string round {i+1} should be deterministic.");
        for (var i=0;i<10;i++) Check(!blank.IsValid,$"blank evidence key round {i+1} should be invalid.");
        for (var i=0;i<10;i++) Check(rejected,$"blank key factory round {i+1} should reject invalid input.");
        for (var i=0;i<10;i++) Check(QualityFindingEvidenceValidationRuntime.IsValid(new QualityFindingEvidenceLink(QualityFindingId.Create("F-1"),key)),$"key-link validation round {i+1} should pass.");
        for (var i=0;i<10;i++) Check(!QualityFindingEvidenceValidationRuntime.IsValid(new QualityFindingEvidenceLink(QualityFindingId.Create("F-1"),blank)),$"blank key link round {i+1} should fail.");
        for (var i=0;i<10;i++) Check(QualityEvidenceKey.Create(key.Value)==key,$"recreated key round {i+1} should preserve equality.");
        for (var i=0;i<10;i++) Check(key!=new QualityEvidenceKey("image://FRAME-002"),$"distinct evidence keys round {i+1} should remain distinct.");
        for (var i=0;i<10;i++) Check(QualityEvidenceKey.Create("FRAME-001").Value=="FRAME-001",$"plain stable key round {i+1} should remain opaque.");
        for (var i=0;i<10;i++) Check(key.IsValid,$"final evidence key state round {i+1} should remain valid.");

        assert(round==100,$"Quality evidence-key smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
