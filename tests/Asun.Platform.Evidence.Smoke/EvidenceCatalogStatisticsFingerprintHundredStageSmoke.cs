using Asun.Platform.Evidence;

public static class EvidenceCatalogStatisticsFingerprintHundredStageSmoke
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
        var statistics=EvidenceCatalogStatisticsRuntime.Create(snapshot);
        var fingerprint=EvidenceCatalogStatisticsFingerprintRuntime.CreateFingerprint(statistics);
        var invalid=statistics with {KnownByteLengthTotal=11};

        for(var i=0;i<10;i++) Check(fingerprint.Length==64,"Statistics fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(fingerprint.All(Uri.IsHexDigit),"Statistics fingerprint should be hexadecimal.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogStatisticsFingerprintRuntime.CreateFingerprint(statistics)==fingerprint,"Statistics fingerprint should be deterministic.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogStatisticsFingerprintValidationRuntime.IsValid(snapshot,statistics,fingerprint),"Statistics fingerprint should validate.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogStatisticsFingerprintValidationRuntime.IsValid(snapshot,invalid,fingerprint),"Statistics mutation should invalidate the original fingerprint.");
        for(var i=0;i<10;i++) Check(fingerprint.All(character=>char.ToLowerInvariant(character)==character),"Statistics fingerprint should remain lowercase.");
        for(var i=0;i<10;i++) Check(statistics.DescriptorCount==1,"Statistics should retain one descriptor.");
        for(var i=0;i<10;i++) Check(statistics.KnownByteLengthTotal==10,"Statistics should retain byte total.");
        for(var i=0;i<10;i++) Check(statistics.KindCounts.Count==1,"Statistics should retain one kind bucket.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogStatisticsValidationRuntime.IsValid(snapshot,statistics),"Fingerprint source statistics should remain valid.");

        assert(round==100,$"Evidence statistics fingerprint smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
