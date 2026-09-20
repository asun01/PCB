using Asun.Domain.Quality;
using Asun.Platform.QualityReleaseIntegration;
using Asun.Release.Core;

public static class QualityReleaseReplayDescriptor4HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var run=QualityInspectionRunRuntime.Create(
            Guid.Parse("91000000-0000-0000-0000-000000000001"),
            new[]
            {
                new QualityInspectionResult(
                    Guid.Parse("92000000-0000-0000-0000-000000000001"),
                    new QualityInspectionSnapshot(
                        Guid.Parse("93000000-0000-0000-0000-000000000001"),1,
                        new QualityFindingSet(new[]
                        {
                            new QualityFinding(QualityFindingId.Create("QUALITY.FAIL"),QualityOutcome.Fail,QualitySeverity.Critical,"Demonstration failure")
                        }),
                        new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))),
                new QualityInspectionResult(
                    Guid.Parse("92000000-0000-0000-0000-000000000002"),
                    new QualityInspectionSnapshot(
                        Guid.Parse("93000000-0000-0000-0000-000000000002"),2,
                        new QualityFindingSet(new[]
                        {
                            new QualityFinding(QualityFindingId.Create("QUALITY.REVIEW"),QualityOutcome.Review,QualitySeverity.Warning,"Demonstration review")
                        }),
                        new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())))
            });
        var manifest=ReleaseManifestRuntime.Create(
            new ReleaseIdentity("Asun PCB",new Version(1,0,0),"stable"),
            new[]{new ReleaseArtifact("quality/results.logical",new string('a',64),32)});
        var descriptor=QualityReleaseReplayDescriptorRuntime.Create(run,manifest);
        var malformed=descriptor with {DescriptorFingerprint="bad"};
        var upper=descriptor with {DescriptorFingerprint=new string('F',64)};
        for(var i=0;i<10;i++) Check(!QualityReleaseReplayDescriptorRuntime.IsValid(run,manifest,malformed),"Malformed descriptor fingerprint should be rejected.");
        for(var i=0;i<10;i++) Check(!QualityReleaseReplayDescriptorRuntime.IsValid(run,manifest,upper),"Uppercase descriptor fingerprint should be rejected by canonical comparison.");
        for(var i=0;i<10;i++) Check(QualityReleaseReplayDescriptorRuntime.Validate(run,manifest,malformed).Count>0,"Malformed fingerprint should produce diagnostics.");
        for(var i=0;i<10;i++) Check(QualityReleaseReplayDescriptorRuntime.Validate(run,manifest,upper).Count>0,"Uppercase fingerprint should produce diagnostics.");
        for(var i=0;i<10;i++) Check(descriptor.DescriptorFingerprint.All(Uri.IsHexDigit),"Baseline descriptor fingerprint should be hexadecimal.");
        for(var i=0;i<10;i++) Check(descriptor.QualityRunId!=Guid.Empty,"Baseline run identity should be valid.");
        for(var i=0;i<10;i++) Check(descriptor.ReleaseReady,"Release readiness should remain true for the valid manifest.");
        for(var i=0;i<10;i++) Check(QualityReleaseReplayDescriptorRuntime.IsValid(run,manifest,descriptor),"Baseline descriptor should remain valid.");
        assert(round==100,$"QualityReleaseReplayDescriptor4HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
