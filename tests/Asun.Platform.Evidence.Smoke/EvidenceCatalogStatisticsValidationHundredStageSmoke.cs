using Asun.Platform.Evidence;

public static class EvidenceCatalogStatisticsValidationHundredStageSmoke
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
        var invalid=statistics with {DescriptorCount=2};

        for(var i=0;i<10;i++) Check(EvidenceCatalogStatisticsValidationRuntime.IsValid(snapshot,statistics),"Valid statistics should pass.");
        for(var i=0;i<10;i++) Check(!EvidenceCatalogStatisticsValidationRuntime.IsValid(snapshot,invalid),"Mismatched descriptor count should be rejected.");
        for(var i=0;i<10;i++) Check(statistics.KindCounts.Sum(item=>item.Count)==snapshot.Count,"Kind counts should cover every descriptor.");
        for(var i=0;i<10;i++) Check(statistics.MediaTypeCounts.Sum(item=>item.Count)==snapshot.Count,"Media type counts should cover every descriptor.");
        for(var i=0;i<10;i++) Check(statistics.KnownByteLengthTotal==10,"Known byte length total should remain ten.");
        for(var i=0;i<10;i++) Check(statistics.KindCounts.SequenceEqual(statistics.KindCounts.OrderBy(item=>item.Kind)),"Kind counts should be canonically ordered.");
        for(var i=0;i<10;i++) Check(statistics.MediaTypeCounts.SequenceEqual(statistics.MediaTypeCounts.OrderBy(item=>item.MediaType,StringComparer.Ordinal)),"Media type counts should be canonically ordered.");
        for(var i=0;i<10;i++) Check(statistics.KindCounts.Select(item=>item.Kind).Distinct().Count()==statistics.KindCounts.Count,"Kind count keys should be unique.");
        for(var i=0;i<10;i++) Check(statistics.MediaTypeCounts.Select(item=>item.MediaType).Distinct(StringComparer.Ordinal).Count()==statistics.MediaTypeCounts.Count,"Media type count keys should be unique.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogSnapshotValidationRuntime.IsValid(snapshot),"Source snapshot should remain valid.");

        assert(round==100,$"Evidence catalog statistics validation smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
