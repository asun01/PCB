using Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowQueryFingerprintHundredStageSmoke
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
            new EvidenceCatalogSnapshotWindowEntry(1,Envelope("frame://001")),
            new EvidenceCatalogSnapshotWindowEntry(2,Envelope("frame://002"))
        });
        var resultSet=EvidenceCatalogSnapshotWindowQueryRuntime.Execute(
            window,
            new EvidenceDescriptorQuery(EvidenceKind.Image,null));
        var fingerprint=EvidenceCatalogSnapshotWindowQueryFingerprintRuntime.CreateFingerprint(resultSet);
        var invalid=resultSet with {EntryCount=3};

        for(var i=0;i<10;i++) Check(fingerprint.Length==64,"Window query fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(fingerprint.All(Uri.IsHexDigit),"Window query fingerprint should be hexadecimal.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowQueryFingerprintRuntime.CreateFingerprint(resultSet)==fingerprint,"Window query fingerprint should be deterministic.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowQueryFingerprintValidationRuntime.IsValid(window,resultSet,fingerprint),"Window query fingerprint should validate.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogSnapshotWindowQueryFingerprintValidationRuntime.IsValid(window,invalid,fingerprint),"Window query mutation should invalidate the original fingerprint.");
        for(var i=0;i<10;i++) Check(fingerprint.All(character=>char.ToLowerInvariant(character)==character),"Window query fingerprint should remain lowercase.");
        for(var i=0;i<10;i++) Check(resultSet.Results.Count==2,"Window query result count should remain two.");
        for(var i=0;i<10;i++) Check(resultSet.Results.All(item=>item.Result.Query.Kind==EvidenceKind.Image),"All window query results should retain image kind.");
        for(var i=0;i<10;i++) Check(resultSet.WindowFingerprint.Length==64,"Window query source fingerprint should remain fixed width.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotWindowQueryValidationRuntime.IsValid(window,resultSet),"Fingerprint source result set should remain valid.");

        assert(round==100,$"Evidence window query fingerprint smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
