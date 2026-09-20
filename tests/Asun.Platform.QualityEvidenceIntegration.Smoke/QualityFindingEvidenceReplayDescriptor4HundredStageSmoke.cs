using Asun.Domain.Quality;
using Asun.Platform.Evidence;
using Asun.Platform.QualityEvidenceIntegration;

public static class QualityFindingEvidenceReplayDescriptor4HundredStageSmoke
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
        var malformed=descriptors.Select(item=>item.FindingId==secondFindingId?item with {DescriptorFingerprint="bad"}:item).ToArray();
for(var i=0;i<10;i++) Check(!QualityFindingEvidenceReplayDescriptorRuntime.IsValid(run,bindings,resolutions,malformed),"Malformed descriptor fingerprint should be rejected.");
for(var i=0;i<10;i++) Check(QualityFindingEvidenceReplayDescriptorRuntime.Validate(run,bindings,resolutions,malformed).Count>0,"Malformed fingerprint should produce diagnostics.");
for(var i=0;i<10;i++) Check(descriptors.All(item=>item.ResolutionFingerprint.All(Uri.IsHexDigit)),"Resolution fingerprints should remain hexadecimal.");
for(var i=0;i<10;i++) Check(descriptors.All(item=>item.DescriptorFingerprint.All(Uri.IsHexDigit)),"Descriptor fingerprints should remain hexadecimal.");
for(var i=0;i<10;i++) Check(QualityFindingEvidenceReplayDescriptorRuntime.Create(run,bindings,resolutions).Count==2,"Repeated descriptor creation should preserve count.");
for(var i=0;i<10;i++) Check(QualityFindingEvidenceReplayDescriptorRuntime.Create(run,bindings,resolutions)[1].DescriptorFingerprint==descriptors[1].DescriptorFingerprint,"Repeated creation should preserve second descriptor identity.");
for(var i=0;i<10;i++) Check(QualityFindingEvidenceReplayDescriptorRuntime.IsValid(run,bindings,resolutions,descriptors),"Baseline descriptors should remain valid.");
for(var i=0;i<10;i++) Check(descriptors[0].EvidenceHandles.Count==1 && descriptors[1].EvidenceHandles.Count==1,"Every finding should retain exactly one opaque handle.");
for(var i=0;i<10;i++) Check(descriptors[0].FindingId!=descriptors[1].FindingId,"Finding descriptor identities should be unique.");
for(var i=0;i<10;i++) Check(descriptors.Count==2,"Final descriptor cardinality should remain stable.");
        assert(round==100,$"QualityFindingEvidenceReplayDescriptor4HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
