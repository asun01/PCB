using Asun.Platform.Evidence;

public static class EvidenceCatalogQueryRuntimeHundredStageSmoke
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
            new EvidenceDescriptor(EvidenceHandle.Create("frame://002"),EvidenceKind.Image,"image/raw",20,"two"),
            new EvidenceDescriptor(EvidenceHandle.Create("frame://001"),EvidenceKind.Image,"image/raw",10,"one"),
            new EvidenceDescriptor(EvidenceHandle.Create("text://001"),EvidenceKind.Text,"text/plain",5,"text")
        });
        var imageQuery=new EvidenceDescriptorQuery(EvidenceKind.Image,"image/raw");
        var textQuery=new EvidenceDescriptorQuery(EvidenceKind.Text,null);

        var images=EvidenceCatalogQueryRuntime.Find(snapshot,imageQuery);
        var texts=EvidenceCatalogQueryRuntime.Find(snapshot,textQuery);

        for(var i=0;i<10;i++) Check(images.Count==2,"Image query should return two descriptors.");
        for(var i=0;i<10;i++) Check(images[0].Handle.Value=="frame://001","Query results should be sorted by opaque handle.");
        for(var i=0;i<10;i++) Check(images[1].Handle.Value=="frame://002","Query results should retain canonical ordering.");
        for(var i=0;i<10;i++) Check(texts.Count==1,"Text query should return one descriptor.");
        for(var i=0;i<10;i++) Check(texts[0].Kind==EvidenceKind.Text,"Text query should return a text descriptor.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryRuntime.Find(snapshot,imageQuery).SequenceEqual(images),"Query results should be deterministic.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryRuntime.Find(snapshot,new EvidenceDescriptorQuery(null,"image/raw")).Count==2,"Media-only image query should return two descriptors.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogQueryRuntime.Find(snapshot,new EvidenceDescriptorQuery(EvidenceKind.Image,"text/plain")).Count==0,"Non-matching compound query should be empty.");
        for(var i=0;i<10;i++) Check(images.All(descriptor=>descriptor.Kind==EvidenceKind.Image),"Image query results should respect kind.");
        for(var i=0;i<10;i++) Check(images.All(descriptor=>descriptor.MediaType=="image/raw"),"Image query results should respect media type.");

        assert(round==100,$"Evidence catalog query runtime smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
