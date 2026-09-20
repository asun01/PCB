using Asun.Domain.Quality;
using Asun.Platform.Evidence;
using Asun.Platform.QualityEvidenceIntegration;

public static class QualityFindingEvidenceReplayDescriptor3HundredStageSmoke
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
                        new QualityFindingEvidenceLink(secondFindingId,secondKey)})))}});
        var bindings=new[]{
            new QualityEvidenceHandleBinding(findingId,firstKey,EvidenceHandle.Create("evidence/frame/10/component")),
            new QualityEvidenceHandleBinding(secondFindingId,secondKey,EvidenceHandle.Create("evidence/frame/10/pad"))};
        var resolutions=QualityFindingEvidenceResolutionRuntime.Create(run,bindings);
        var descriptors=QualityFindingEvidenceReplayDescriptorRuntime.Create(run,bindings,resolutions);
        var missing=descriptors.Skip(1).ToArray();
var duplicate=descriptors.Concat(new[]{descriptors[0]}).ToArray();
var reordered=resolutions.Reverse().ToArray();
var reorderedDescriptors=QualityFindingEvidenceReplayDescriptorRuntime.Create(run,bindings,reordered);
for(var i=0;i<10;i++) Check(!QualityFindingEvidenceReplayDescriptorRuntime.IsValid(run,bindings,resolutions,missing),"Missing finding descriptor should be rejected.");
for(var i=0;i<10;i++) Check(!QualityFindingEvidenceReplayDescriptorRuntime.IsValid(run,bindings,resolutions,duplicate),"Duplicate finding descriptor should be rejected.");
for(var i=0;i<10;i++) Check(QualityFindingEvidenceReplayDescriptorRuntime.IsValid(run,bindings,reordered,reorderedDescriptors),"Resolution reordering should remain valid.");
for(var i=0;i<10;i++) Check(QualityFindingEvidenceReplayDescriptorRuntime.IsEquivalent(descriptors,reorderedDescriptors),"Resolution reordering should preserve replay identity.");
for(var i=0;i<10;i++) Check(QualityFindingEvidenceReplayDescriptorRuntime.Validate(run,bindings,resolutions,missing).Count>0,"Missing descriptor should produce diagnostics.");
for(var i=0;i<10;i++) Check(QualityFindingEvidenceReplayDescriptorRuntime.Validate(run,bindings,resolutions,duplicate).Count>0,"Duplicate descriptor should produce diagnostics.");
for(var i=0;i<10;i++) Check(descriptors[0].FindingId.Value<descriptors[1].FindingId.Value,StringComparison.Ordinal),"Canonical finding ordering should be deterministic.");
for(var i=0;i<10;i++) Check(QualityFindingEvidenceReplayDescriptorRuntime.IsValid(run,bindings,resolutions,descriptors),"Baseline descriptors should remain valid.");
for(var i=0;i<10;i++) Check(descriptors.All(item=>item.FindingId.IsValid),"Finding identities should remain valid.");
for(var i=0;i<10;i++) Check(descriptors.All(item=>item.EvidenceHandles.All(handle=>handle.IsValid)),"Opaque handles should remain valid.");
        assert(round==100,$"QualityFindingEvidenceReplayDescriptor3HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
