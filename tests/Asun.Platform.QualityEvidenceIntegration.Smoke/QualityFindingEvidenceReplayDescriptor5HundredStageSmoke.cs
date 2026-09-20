using Asun.Domain.Quality;
using Asun.Platform.Evidence;
using Asun.Platform.QualityEvidenceIntegration;

public static class QualityFindingEvidenceReplayDescriptor5HundredStageSmoke
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
        var changedBindings=new[]{
new QualityEvidenceHandleBinding(findingId,firstKey,EvidenceHandle.Create("evidence/changed/component")),
new QualityEvidenceHandleBinding(secondFindingId,secondKey,EvidenceHandle.Create("evidence/frame/10/pad"))};
var changedResolution=QualityFindingEvidenceResolutionRuntime.Create(run,changedBindings);
var changedDescriptors=QualityFindingEvidenceReplayDescriptorRuntime.Create(run,changedBindings,changedResolution);
for(var i=0;i<10;i++) Check(changedDescriptors[0].DescriptorFingerprint!=descriptors[0].DescriptorFingerprint,"Changed opaque Evidence identity should alter descriptor identity.");
for(var i=0;i<10;i++) Check(!QualityFindingEvidenceReplayDescriptorRuntime.IsEquivalent(descriptors,changedDescriptors),"Changed Evidence identity should break replay equivalence.");
for(var i=0;i<10;i++) Check(QualityFindingEvidenceReplayDescriptorRuntime.IsValid(run,changedBindings,changedResolution,changedDescriptors),"Changed evidence replay descriptors should validate against changed bindings.");
for(var i=0;i<10;i++) Check(QualityFindingEvidenceReplayDescriptorRuntime.IsValid(run,bindings,resolutions,descriptors),"Original descriptors should remain valid.");
for(var i=0;i<10;i++) Check(changedDescriptors[0].ResolutionFingerprint!=descriptors[0].ResolutionFingerprint,"Changed Evidence identity should alter resolution fingerprint.");
for(var i=0;i<10;i++) Check(changedDescriptors[0].EvidenceHandles[0].Value=="evidence/changed/component","Changed Evidence identity should be explicit.");
for(var i=0;i<10;i++) Check(descriptors[0].EvidenceHandles[0].Value=="evidence/frame/10/component","Original Evidence identity should remain unchanged.");
for(var i=0;i<10;i++) Check(descriptors[1].EvidenceHandles[0].Value=="evidence/frame/10/pad","Second Evidence identity should remain unchanged.");
for(var i=0;i<10;i++) Check(QualityFindingEvidenceReplayDescriptorRuntime.Create(run,bindings,resolutions)[0].FindingId==findingId,"Original descriptor creation should remain deterministic.");
for(var i=0;i<10;i++) Check(QualityFindingEvidenceReplayDescriptorRuntime.IsValid(run,bindings,resolutions,descriptors),"Final canonical replay descriptor validation should remain clean.");
        assert(round==100,$"QualityFindingEvidenceReplayDescriptor5HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
