using Asun.Platform.Evidence;

public static class EvidenceCatalogHundredStageSmoke
{
    public static async ValueTask Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var handle=EvidenceHandle.Create("frame://001");
        var descriptor=new EvidenceDescriptor(
            handle,
            EvidenceKind.Image,
            "image/raw",
            10,
            "frame");

        var found=await EvidenceCatalogRuntime.GetValidatedAsync(
            new StubCatalog(descriptor),
            handle);
        var missing=await EvidenceCatalogRuntime.GetValidatedAsync(
            new StubCatalog(descriptor),
            EvidenceHandle.Create("frame://missing"));

        for(var i=0;i<10;i++) Check(found==descriptor,"Catalog lookup should return the matching descriptor.");
        for(var i=0;i<10;i++) Check(missing is null,"Catalog lookup should return null for missing handles.");
        for(var i=0;i<10;i++) Check(EvidenceCatalogRuntime.GetValidatedAsync(new StubCatalog(descriptor),handle).Result==descriptor,"Repeated catalog lookup should remain deterministic.");
        for(var i=0;i<10;i++) Check(descriptor.Handle==handle,"Catalog descriptor handle should remain stable.");
        for(var i=0;i<10;i++) Check(EvidenceDescriptorValidationRuntime.IsValid(descriptor),"Catalog descriptor should remain valid.");
        for(var i=0;i<10;i++) Check(found!.Kind==EvidenceKind.Image,"Catalog descriptor kind should remain stable.");
        for(var i=0;i<10;i++) Check(found.MediaType=="image/raw","Catalog descriptor media type should remain stable.");
        for(var i=0;i<10;i++) Check(found.ByteLength==10,"Catalog descriptor length should remain stable.");
        for(var i=0;i<10;i++) Check(found.DisplayName=="frame","Catalog descriptor display name should remain stable.");
        for(var i=0;i<10;i++) Check(round+1<=100,"Catalog smoke round budget should remain bounded.");

        assert(round==100,$"Evidence catalog smoke should execute exactly 100 numbered rounds; actual {round}.");
    }

    private sealed class StubCatalog : IEvidenceCatalog
    {
        private readonly EvidenceDescriptor _descriptor;

        public StubCatalog(EvidenceDescriptor descriptor)=>
            _descriptor=descriptor;

        public ValueTask<EvidenceDescriptor?> GetAsync(
            EvidenceHandle handle,
            CancellationToken cancellationToken=default)=>
            ValueTask.FromResult<EvidenceDescriptor?>(
                handle==_descriptor.Handle ? _descriptor : null);
    }
}
