using Asun.Domain.Quality;
using Asun.Platform.Evidence;
using Asun.Platform.QualityEvidenceIntegration;

public static class QualityFindingEvidenceResolutionHundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var findingId=QualityFindingId.Create("AOI.COMPONENT.MISSING");
        var secondFindingId=QualityFindingId.Create("AOI.PAD.MISSING");
        var firstKey=QualityEvidenceKey.Create("evidence/frame/10/component");
        var secondKey=QualityEvidenceKey.Create("evidence/frame/10/pad");
        var run=QualityInspectionRunRuntime.Create(
            Guid.Parse("B1000000-0000-0000-0000-000000000001"),
            new[]
            {
                new QualityInspectionResult(
                    Guid.Parse("B2000000-0000-0000-0000-000000000001"),
                    new QualityInspectionSnapshot(
                        Guid.Parse("B3000000-0000-0000-0000-000000000001"),
                        10,
                        new QualityFindingSet(new[]
                        {
                            new QualityFinding(findingId,"AOI.COMPONENT.MISSING",QualityOutcome.Fail,QualitySeverity.Major,"Component missing"),
                            new QualityFinding(secondFindingId,"AOI.PAD.MISSING",QualityOutcome.Fail,QualitySeverity.Minor,"Pad missing")
                        }),
                        new QualityFindingEvidenceSet(new[]
                        {
                            new QualityFindingEvidenceLink(findingId,firstKey),
                            new QualityFindingEvidenceLink(secondFindingId,secondKey)
                        })))
            });
        var bindings=new[]
        {
            new QualityEvidenceHandleBinding(findingId,firstKey,EvidenceHandle.Create("evidence/frame/10/component")),
            new QualityEvidenceHandleBinding(secondFindingId,secondKey,EvidenceHandle.Create("evidence/frame/10/pad"))
        };
        var resolutions=QualityFindingEvidenceResolutionRuntime.Create(run,bindings);
        var reordered=resolutions.Reverse().ToArray();
        var tampered=resolutions
            .Select(resolution=>resolution.FindingId==findingId
                ? resolution with { EvidenceHandles=new[]{EvidenceHandle.Create("evidence/frame/10/tampered")} }
                : resolution)
            .ToArray();
        var missing=resolutions.Skip(1).ToArray();

        for(var i=0;i<10;i++) Check(run.ResultCount==1,"Quality run should contain one inspection result.");
        for(var i=0;i<10;i++) Check(run.Results[0].Findings.Count==2,"Quality run should contain two findings.");
        for(var i=0;i<10;i++) Check(run.Results[0].Evidence.Count==2,"Quality run should contain two opaque evidence links.");
        for(var i=0;i<10;i++) Check(resolutions.Count==2,"Resolution should contain one entry per finding.");
        for(var i=0;i<10;i++) Check(resolutions[0].FindingId==findingId,"Resolution should preserve first finding identity.");
        for(var i=0;i<10;i++) Check(resolutions[0].EvidenceHandles[0]==EvidenceHandle.Create("evidence/frame/10/component"),"Resolution should preserve the opaque handle.");
        for(var i=0;i<10;i++) Check(QualityFindingEvidenceResolutionRuntime.IsValid(run,bindings,resolutions),"Canonical finding evidence resolution should validate.");
        for(var i=0;i<10;i++) Check(QualityFindingEvidenceResolutionRuntime.IsValid(run,bindings,reordered),"Resolution ordering should not affect validity.");
        for(var i=0;i<10;i++) Check(!QualityFindingEvidenceResolutionRuntime.IsValid(run,bindings,tampered),"Opaque handle tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!QualityFindingEvidenceResolutionRuntime.IsValid(run,bindings,missing),"Missing finding resolution should be rejected.");

        assert(round==100,$"Finding evidence resolution smoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
