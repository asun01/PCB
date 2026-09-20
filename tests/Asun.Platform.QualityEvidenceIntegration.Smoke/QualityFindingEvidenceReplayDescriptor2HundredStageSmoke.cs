using Asun.Domain.Quality;
using Asun.Platform.Evidence;
using Asun.Platform.QualityEvidenceIntegration;

public static class QualityFindingEvidenceReplayDescriptor2HundredStageSmoke
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
        var tampered=descriptors.Select(item=>item.FindingId==findingId?item with {EvidenceHandles=new[]{EvidenceHandle.Create("evidence/tampered")}:item}).ToArray();
var badFingerprint=descriptors.Select(item=>item.FindingId==secondFindingId?item with {ResolutionFingerprint=new string('a',64)}:item).ToArray();
for(var i=0;i<10;i++) Check(!QualityFindingEvidenceReplayDescriptorRuntime.IsValid(run,bindings,resolutions,tampered),"Opaque handle tampering should be rejected.");
for(var i=0;i<10;i++) Check(!QualityFindingEvidenceReplayDescriptorRuntime.IsValid(run,bindings,resolutions,badFingerprint),"Resolution fingerprint tampering should be rejected.");
for(var i=0;i<10;i++) Check(QualityFindingEvidenceReplayDescriptorRuntime.Validate(run,bindings,resolutions,tampered).Count>0,"Handle tampering should produce diagnostics.");
for(var i=0;i<10;i++) Check(QualityFindingEvidenceReplayDescriptorRuntime.Validate(run,bindings,resolutions,badFingerprint).Count>0,"Fingerprint tampering should produce diagnostics.");
for(var i=0;i<10;i++) Check(descriptors[0].EvidenceHandles[0].Value=="evidence/frame/10/component","Baseline first handle should remain stable.");
for(var i=0;i<10;i++) Check(descriptors[1].EvidenceHandles[0].Value=="evidence/frame/10/pad","Baseline second handle should remain stable.");
for(var i=0;i<10;i++) Check(QualityFindingEvidenceReplayDescriptorRuntime.IsValid(run,bindings,resolutions,descriptors),"Baseline descriptors should remain valid.");
for(var i=0;i<10;i++) Check(descriptors.Count==2,"Baseline descriptor count should remain stable.");
for(var i=0;i<10;i++) Check(descriptors.All(item=>item.DescriptorFingerprint.Length==64),"Baseline fingerprints should remain fixed width.");
for(var i=0;i<10;i++) Check(QualityFindingEvidenceReplayDescriptorRuntime.Create(run,bindings,resolutions).Count==2,"Recreated descriptor set should preserve count.");
        assert(round==100,$"QualityFindingEvidenceReplayDescriptor2HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
