using Asun.Platform.Evidence;

public static class EvidenceCatalogQueryResultFingerprintHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var snapshot=EvidenceCatalogSnapshotRuntime.Create(new[]{
            new EvidenceDescriptor(EvidenceHandle.Create("frame://001"),EvidenceKind.Image,"image/raw",10,"one")
        });
        var result=EvidenceCatalogQueryResultRuntime.Execute(
            snapshot,
            new EvidenceDescriptorQuery(EvidenceKind.Image,null));
        var fingerprint=EvidenceCatalogQueryResultFingerprintRuntime.CreateFingerprint(result);
        var tampered=result with {MatchCount=2};

        for(var i=0;i<10;i++) Check(fingerprint.Length==64,"Query result fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(fingerprint.All(Uri.IsHexDigit),"Query result fingerprint should be hexadecimal.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryResultFingerprintRuntime.CreateFingerprint(result)==fingerprint,"Query result fingerprint should be deterministic.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryResultFingerprintValidationRuntime.IsValid(snapshot,result,fingerprint),"Query result fingerprint should validate.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogQueryResultFingerprintValidationRuntime.IsValid(snapshot,tampered,fingerprint),"Query result mutation should invalidate the original fingerprint.");
        for(var i=0;i<10;i++) Check(fingerprint.All(character=>char.ToLowerInvariant(character)==character),"Query result fingerprint should remain lowercase.");
        for(var i=0;i<10;i++) Check(result.Handles.Count==1,"Query result should retain one handle.");
        for(var i=0;i<10;i++) Check(result.Query.Kind==EvidenceKind.Image,"Fingerprint input should retain query kind.");
        for(var i=0;i<10;i++) Check(result.SnapshotFingerprint.Length==64,"Fingerprint input should retain snapshot fingerprint.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryResultValidationRuntime.IsValid(snapshot,result),"Fingerprint source result should remain valid.");

        assert(round==100,$"Evidence query result fingerprint smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
