using Asun.Domain.Quality;
using Asun.Platform.Evidence;
using Asun.Platform.QualityEvidenceIntegration;

public static class QualityFindingEvidenceReplayDescriptor1HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var findingId=QualityFindingId.Create("AOI.COMPONENT.MISSING");
        var secondFindingId=QualityFindingId.Create("AOI.PAD.MISSING");
        var firstKey=QualityEvidenceKey.Create("evidence/frame/10/component");
        var secondKey=QualityEvidenceKey.Create("evidence/frame/10/pad");
        var run=QualityInspectionRunRuntime.Create(
            Guid.Parse("B1000000-0000-0000-0000-000000000001"),
            new[]{new QualityInspectionResult(
                Guid.Parse("B2000000-0000-0000-0000-000000000001"),
                new QualityInspectionSnapshot(
                    Guid.Parse("B3000000-0000-0000-0000-000000000001"),
                    10,
                    new QualityFindingSet(new[]{
                        new QualityFinding(findingId,"AOI.COMPONENT.MISSING",QualityOutcome.Fail,QualitySeverity.Major,"Component missing"),
                        new QualityFinding(secondFindingId,"AOI.PAD.MISSING",QualityOutcome.Fail,QualitySeverity.Minor,"Pad missing")}),
                    new QualityFindingEvidenceSet(new[]{
                        new QualityFindingEvidenceLink(findingId,firstKey),
                        new QualityFindingEvidenceLink(secondFindingId,secondKey)})))});
        var bindings=new[]{
            new QualityEvidenceHandleBinding(findingId,firstKey,EvidenceHandle.Create("evidence/frame/10/component")),
            new QualityEvidenceHandleBinding(secondFindingId,secondKey,EvidenceHandle.Create("evidence/frame/10/pad"))};
        var resolutions=QualityFindingEvidenceResolutionRuntime.Create(run,bindings);
        var descriptors=QualityFindingEvidenceReplayDescriptorRuntime.Create(run,bindings,resolutions);
        for(var i=0;i<10;i++) Check(descriptors.Count==2,"Replay descriptor count should match finding count.");
for(var i=0;i<10;i++) Check(descriptors[0].FindingId==findingId,"First finding identity should be preserved.");
for(var i=0;i<10;i++) Check(descriptors[0].EvidenceHandles[0].IsValid,"First opaque Evidence handle should remain valid.");
for(var i=0;i<10;i++) Check(descriptors.All(item=>item.ResolutionFingerprint.Length==64),"Resolution fingerprints should be fixed width.");
for(var i=0;i<10;i++) Check(descriptors.All(item=>item.DescriptorFingerprint.Length==64),"Descriptor fingerprints should be fixed width.");
for(var i=0;i<10;i++) Check(QualityFindingEvidenceReplayDescriptorRuntime.IsValid(run,bindings,resolutions,descriptors),"Canonical replay descriptors should validate.");
for(var i=0;i<10;i++) Check(QualityFindingEvidenceReplayDescriptorRuntime.IsEquivalent(descriptors,descriptors.Reverse().ToArray()),"Descriptor ordering should be canonical.");
for(var i=0;i<10;i++) Check(QualityFindingEvidenceReplayDescriptorRuntime.Create(run,bindings,resolutions)[0].DescriptorFingerprint==descriptors[0].DescriptorFingerprint,"Descriptor creation should be deterministic.");
for(var i=0;i<10;i++) Check(descriptors.All(item=>item.DescriptorFingerprint.All(Uri.IsHexDigit)),"Descriptor fingerprints should be hexadecimal.");
for(var i=0;i<10;i++) Check(descriptors.All(item=>item.EvidenceHandles.Count>0),"Every finding descriptor should carry evidence.");
        assert(round==100,$"QualityFindingEvidenceReplayDescriptor1HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
