using Asun.Platform.Evidence;

public static class EvidenceReferenceResolutionFingerprintHundredStageSmoke
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
        var referenceSet=new EvidenceReferenceSet(new[]{
            EvidenceHandle.Create("frame://001"),
            EvidenceHandle.Create("frame://002")
        });
        var resolution=EvidenceReferenceResolutionRuntime.Resolve(snapshot,referenceSet);
        var fingerprint=EvidenceReferenceResolutionFingerprintRuntime.CreateFingerprint(resolution);
        var invalid=resolution with {MissingHandles=Array.Empty<EvidenceHandle>()};

        for(var i=0;i<10;i++) Check(fingerprint.Length==64,"Resolution fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(fingerprint.All(Uri.IsHexDigit),"Resolution fingerprint should be hexadecimal.");
        for(var i=0;i<10;i++) Check(EvidenceReferenceResolutionFingerprintRuntime.CreateFingerprint(resolution)==fingerprint,"Resolution fingerprint should be deterministic.");
        for(var i=0;i<10;i++) Check(EvidenceReferenceResolutionFingerprintValidationRuntime.IsValid(snapshot,referenceSet,resolution,fingerprint),"Resolution fingerprint should validate.");
        for(var i=0;i<10;i++) Check(!EvidenceReferenceResolutionFingerprintValidationRuntime.IsValid(snapshot,referenceSet,invalid,fingerprint),"Resolution mutation should invalidate the original fingerprint.");
        for(var i=0;i<10;i++) Check(fingerprint.All(character=>char.ToLowerInvariant(character)==character),"Resolution fingerprint should remain lowercase.");
        for(var i=0;i<10;i++) Check(resolution.FoundHandles.Count==1,"Resolution should retain one found handle.");
        for(var i=0;i<10;i++) Check(resolution.MissingHandles.Count==1,"Resolution should retain one missing handle.");
        for(var i=0;i<10;i++) Check(resolution.SnapshotFingerprint.Length==64,"Resolution source fingerprint should remain fixed width.");
        for(var i=0;i<10;i++) Check(EvidenceReferenceResolutionValidationRuntime.IsValid(snapshot,referenceSet,resolution),"Fingerprint source resolution should remain valid.");

        assert(round==100,$"Evidence reference resolution fingerprint smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
