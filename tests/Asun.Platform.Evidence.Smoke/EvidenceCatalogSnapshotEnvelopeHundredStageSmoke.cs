using Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotEnvelopeHundredStageSmoke
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
            new EvidenceDescriptor(EvidenceHandle.Create("frame://001"),EvidenceKind.Image,"image/raw",20,"first")
        });
        var envelope=EvidenceCatalogSnapshotEnvelopeRuntime.Create(snapshot);
        var tamperedSnapshot=EvidenceCatalogSnapshotRuntime.Create(new[]{
            new EvidenceDescriptor(EvidenceHandle.Create("frame://001"),EvidenceKind.Image,"image/raw",21,"first")
        });
        var tampered=new EvidenceCatalogSnapshotEnvelope(
            tamperedSnapshot,
            envelope.Fingerprint);
        var malformed=new EvidenceCatalogSnapshotEnvelope(
            snapshot,
            new string('a',64));

        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotEnvelopeValidationRuntime.IsValid(envelope),"Snapshot envelope should validate.");
        for(var i=0;i<10;i++) Check(envelope.Fingerprint==EvidenceCatalogSnapshotFingerprintRuntime.CreateFingerprint(snapshot),"Envelope fingerprint should match snapshot fingerprint.");
        for(var i=0;i<10;i++) Check(envelope.Snapshot==snapshot,"Envelope should retain the source snapshot.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogSnapshotEnvelopeValidationRuntime.IsValid(malformed),"Malformed envelope fingerprint should be rejected.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogSnapshotEnvelopeValidationRuntime.IsValid(tampered),"Tampered envelope snapshot should be rejected.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotEnvelopeRuntime.Create(snapshot).Fingerprint==envelope.Fingerprint,"Envelope creation should be deterministic.");
        for(var i=0;i<10;i++) Check(envelope.Fingerprint.Length==64,"Envelope fingerprint should remain fixed width.");
        for(var i=0;i<10;i++) Check(envelope.Fingerprint.All(Uri.IsHexDigit),"Envelope fingerprint should remain hexadecimal.");
        for(var i=0;i<10;i++) Check(envelope.Snapshot.Count==1,"Envelope snapshot count should remain stable.");
        for(var i=0;i<10;i++) Check(envelope.Snapshot.Find(EvidenceHandle.Create("frame://001")) is not null,"Envelope snapshot lookup should remain available.");

        assert(round==100,$"Evidence snapshot envelope smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
