using Asun.Platform.Evidence;

public static class EvidenceWindowReferenceResolutionFingerprintHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        static EvidenceCatalogSnapshotEnvelope Envelope(string handle)=>
            EvidenceCatalogSnapshotEnvelopeRuntime.Create(
                EvidenceCatalogSnapshotRuntime.Create(new[]{
                    new EvidenceDescriptor(EvidenceHandle.Create(handle),EvidenceKind.Image,"image/raw",10,handle)
                }));

        var window=EvidenceCatalogSnapshotWindowRuntime.Create(new[]{
            new EvidenceCatalogSnapshotWindowEntry(1,Envelope("frame://001"))
        });
        var referenceSet=new EvidenceReferenceSet(new[]{
            EvidenceHandle.Create("frame://001"),
            EvidenceHandle.Create("missing://001")
        });
        var result=EvidenceCatalogSnapshotWindowReferenceResolutionRuntime.Resolve(window,referenceSet);
        var fingerprint=EvidenceCatalogSnapshotWindowReferenceResolutionFingerprintRuntime.CreateFingerprint(result);
        var invalid=result with {ReferenceSet=new EvidenceReferenceSet(new[]{EvidenceHandle.Create("frame://002")})};

        for(var i=0;i<10;i++) Check(fingerprint.Length==64,"Window reference fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(fingerprint.All(Uri.IsHexDigit),"Window reference fingerprint should be hexadecimal.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowReferenceResolutionFingerprintRuntime.CreateFingerprint(result)==fingerprint,"Window reference fingerprint should be deterministic.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowReferenceResolutionFingerprintValidationRuntime.IsValid(window,result,fingerprint),"Window reference fingerprint should validate.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogSnapshotWindowReferenceResolutionFingerprintValidationRuntime.IsValid(window,invalid,fingerprint),"Window reference mutation should invalidate the original fingerprint.");
        for(var i=0;i<10;i++) Check(fingerprint.All(character=>char.ToLowerInvariant(character)==character),"Window reference fingerprint should remain lowercase.");
        for(var i=0;i<10;i++) Check(result.Entries.Count==1,"Window reference result should retain one entry.");
        for(var i=0;i<10;i++) Check(result.ReferenceSet.Handles.Count==2,"Window reference result should retain two handles.");
        for(var i=0;i<10;i++) Check(result.WindowFingerprint.Length==64,"Window reference source fingerprint should remain fixed width.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowReferenceResolutionValidationRuntime.IsValid(window,result),"Fingerprint source result should remain valid.");

        assert(round==100,$"Evidence window reference fingerprint smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
