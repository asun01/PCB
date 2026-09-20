using Asun.Platform.Evidence;
using Asun.Platform.PcbExecutionIntegration;
using Asun.Platform.PcbEvidenceResolutionIntegration;
using Asun.Platform.PcbEvidenceReleaseIntegration;
using Asun.Release.Core;

public static class PcbEvidenceReleaseFactProjectionHundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var snapshot=new PcbExecutionSnapshot(
            new string('a',64),
            Guid.Parse("BA000000-0000-0000-0000-000000000001"),
            new string('b',64),
            new string('c',64),
            2,
            2,
            Guid.Parse("BB000000-0000-0000-0000-000000000001"),
            new string('d',64),
            new string('e',64));
        var evidenceProjection=new ProductionEvidenceReferenceProjection(
            snapshot.ProductionSessionId,
            snapshot.ProductionFingerprint,
            new[]
            {
                new ProductionEvidenceFrameReference(1,new[]{EvidenceHandle.Create("rel/a")}),
                new ProductionEvidenceFrameReference(2,new[]{EvidenceHandle.Create("rel/b")})
            },
            new string('f',64));
        var resolution=new PcbExecutionEvidenceResolution(
            snapshot.Fingerprint,
            evidenceProjection.Fingerprint,
            new string('c',64),
            new[]{EvidenceHandle.Create("rel/a"),EvidenceHandle.Create("rel/b")},
            new[]{EvidenceHandle.Create("rel/a"),EvidenceHandle.Create("rel/b")},
            Array.Empty<EvidenceHandle>(),
            new string('d',64));
        var manifest=ReleaseManifestRuntime.Create(
            new ReleaseIdentity("Asun PCB",new Version(1,0,0),"stable"),
            new[]{new ReleaseArtifact("pcb/evidence.logical",new string('1',64),10)});
        var projection=PcbEvidenceReleaseFactProjectionRuntime.Create(snapshot,resolution,manifest);
        var tampered=projection with {FoundCount=0};
        var tamperedManifest=projection with {ReleaseManifestFingerprint=new string('2',64)};

        for(var i=0;i<10;i++) Check(projection.RequestedCount==2,"Projection should report two requested handles.");
        for(var i=0;i<10;i++) Check(projection.FoundCount==2,"Projection should report two found handles.");
        for(var i=0;i<10;i++) Check(projection.MissingCount==0,"Projection should report zero missing handles.");
        for(var i=0;i<10;i++) Check(projection.AllRequestedResolved,"Projection should mark all requested handles as resolved factually.");
        for(var i=0;i<10;i++) Check(projection.ExecutionSnapshotFingerprint==snapshot.Fingerprint,"Projection should bind execution snapshot identity.");
        for(var i=0;i<10;i++) Check(projection.EvidenceProjectionFingerprint==evidenceProjection.Fingerprint,"Projection should bind evidence projection identity.");
        for(var i=0;i<10;i++) Check(projection.ReleaseManifestFingerprint==manifest.Fingerprint,"Projection should bind release manifest identity.");
        for(var i=0;i<10;i++) Check(PcbEvidenceReleaseFactProjectionValidationRuntime.IsValid(snapshot,resolution,manifest,projection),"Evidence release facts should validate.");
        for(var i=0;i<10;i++) Check(!PcbEvidenceReleaseFactProjectionValidationRuntime.IsValid(snapshot,resolution,manifest,tampered),"Found-count tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!PcbEvidenceReleaseFactProjectionValidationRuntime.IsValid(snapshot,resolution,manifest,tamperedManifest),"Manifest identity tampering should be rejected.");

        assert(round==100,$"PCB evidence release fact smoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
