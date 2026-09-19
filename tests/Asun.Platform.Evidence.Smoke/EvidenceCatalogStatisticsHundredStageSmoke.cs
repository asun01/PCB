using Asun.Platform.Evidence;

public static class EvidenceCatalogStatisticsHundredStageSmoke
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
            new EvidenceDescriptor(EvidenceHandle.Create("frame://001"),EvidenceKind.Image,"image/raw",10,"one"),
            new EvidenceDescriptor(EvidenceHandle.Create("frame://002"),EvidenceKind.Image,"image/raw",20,"two"),
            new EvidenceDescriptor(EvidenceHandle.Create("text://001"),EvidenceKind.Text,"text/plain",5,"text")
        });
        var statistics=EvidenceCatalogStatisticsRuntime.Create(snapshot);

        for(var i=0;i<10;i++) Check(statistics.DescriptorCount==3,"Statistics should preserve descriptor count.");
        for(var i=0;i<10;i++) Check(statistics.KindCounts.Count==2,"Statistics should contain two kinds.");
        for(var i=0;i<10;i++) Check(statistics.MediaTypeCounts.Count==2,"Statistics should contain two media types.");
        for(var i=0;i<10;i++) Check(statistics.KindCounts.First(item=>item.Kind==EvidenceKind.Image).Count==2,"Image kind count should be two.");
        for(var i=0;i<10;i++) Check(statistics.KindCounts.First(item=>item.Kind==EvidenceKind.Text).Count==1,"Text kind count should be one.");
        for(var i=0;i<10;i++) Check(statistics.KnownByteLengthTotal==35,"Known byte length total should be 35.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogStatisticsValidationRuntime.IsValid(snapshot,statistics),"Statistics should validate.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogStatisticsRuntime.Create(snapshot)==statistics,"Statistics calculation should be deterministic.");
        for(var i=0;i<10;i++) Check(statistics.MediaTypeCounts.First(item=>item.MediaType=="image/raw").Count==2,"image/raw count should be two.");
        for(var i=0;i<10;i++) Check(statistics.MediaTypeCounts.First(item=>item.MediaType=="text/plain").Count==1,"text/plain count should be one.");

        assert(round==100,$"Evidence catalog statistics smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
