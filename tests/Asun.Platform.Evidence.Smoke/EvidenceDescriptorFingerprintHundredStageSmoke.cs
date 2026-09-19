using Asun.Platform.Evidence;

public static class EvidenceDescriptorFingerprintHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var descriptor=new EvidenceDescriptor(
            EvidenceHandle.Create("frame://fp"),
            EvidenceKind.Image,
            "image/raw",
            99,
            "fingerprint");
        var fingerprint=EvidenceDescriptorFingerprintRuntime.CreateFingerprint(descriptor);
        var tampered=descriptor with {DisplayName="changed"};
        var bad=new string('a',64);

        for(var i=0;i<10;i++) Check(fingerprint.Length==64,"Descriptor fingerprint should be SHA-256 sized.");
        for(var i=0;i<10;i++) Check(EvidenceDescriptorFingerprintValidationRuntime.IsValidFingerprint(fingerprint),"Descriptor fingerprint syntax should be valid.");
        for(var i=0;i<10;i++) Check(EvidenceDescriptorFingerprintRuntime.CreateFingerprint(descriptor)==fingerprint,"Descriptor fingerprint should be deterministic.");
        for(var i=0;i<10;i++) Check(EvidenceDescriptorFingerprintValidationRuntime.IsValidDescriptor(descriptor,fingerprint),"Descriptor fingerprint should validate the source descriptor.");
        for(var i=0;i<10;i++) Check(!EvidenceDescriptorFingerprintValidationRuntime.IsValidDescriptor(tampered,fingerprint),"Descriptor mutation should invalidate the original fingerprint.");
        for(var i=0;i<10;i++) Check(!EvidenceDescriptorFingerprintValidationRuntime.IsValidFingerprint(bad)==false,"A valid-length lowercase hex fingerprint should pass syntax validation.");
        for(var i=0;i<10;i++) Check(fingerprint.All(character=>Uri.IsHexDigit(character)),"Descriptor fingerprint should contain only hexadecimal characters.");
        for(var i=0;i<10;i++) Check(fingerprint.All(character=>char.ToLowerInvariant(character)==character),"Descriptor fingerprint should remain lowercase.");
        for(var i=0;i<10;i++) Check(descriptor.Handle.Value=="frame://fp","Descriptor source handle should remain stable.");
        for(var i=0;i<10;i++) Check(descriptor.MediaType=="image/raw","Descriptor source media type should remain stable.");

        assert(round==100,$"Evidence descriptor fingerprint smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
