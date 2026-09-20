namespace Asun.Platform.ReplayIntegration.Smoke;

public static class ProductionCaptureEvidenceAuditReplayConvergence3HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var group0=0;group0<10;group0++)
        {
            round++;
                var fixture=ProductionCaptureEvidenceAuditReplayConvergenceSmokeFixtures.Create();
                var tampered=fixture.Binding with { ReleaseManifestFingerprint = "3333333333333333333333333333333333333333333333333333333333333333" };
                Check(!ProductionCaptureEvidenceAuditReplayConvergenceRuntime.IsValid(tampered,fixture.Descriptor) &&
                      (group0<9 || round==100),
                      "Release manifest identity drift must be rejected");
        }
        for(var group1=0;group1<10;group1++)
        {
            round++;
                var fixture=ProductionCaptureEvidenceAuditReplayConvergenceSmokeFixtures.Create();
                var tampered=fixture.Binding with { ReleaseManifestFingerprint = "3333333333333333333333333333333333333333333333333333333333333333" };
                Check(!ProductionCaptureEvidenceAuditReplayConvergenceRuntime.IsValid(tampered,fixture.Descriptor) &&
                      (group1<9 || round==100),
                      "Release manifest identity drift must be rejected");
        }
        for(var group2=0;group2<10;group2++)
        {
            round++;
                var fixture=ProductionCaptureEvidenceAuditReplayConvergenceSmokeFixtures.Create();
                var tampered=fixture.Binding with { ReleaseManifestFingerprint = "3333333333333333333333333333333333333333333333333333333333333333" };
                Check(!ProductionCaptureEvidenceAuditReplayConvergenceRuntime.IsValid(tampered,fixture.Descriptor) &&
                      (group2<9 || round==100),
                      "Release manifest identity drift must be rejected");
        }
        for(var group3=0;group3<10;group3++)
        {
            round++;
                var fixture=ProductionCaptureEvidenceAuditReplayConvergenceSmokeFixtures.Create();
                var tampered=fixture.Binding with { ReleaseManifestFingerprint = "3333333333333333333333333333333333333333333333333333333333333333" };
                Check(!ProductionCaptureEvidenceAuditReplayConvergenceRuntime.IsValid(tampered,fixture.Descriptor) &&
                      (group3<9 || round==100),
                      "Release manifest identity drift must be rejected");
        }
        for(var group4=0;group4<10;group4++)
        {
            round++;
                var fixture=ProductionCaptureEvidenceAuditReplayConvergenceSmokeFixtures.Create();
                var tampered=fixture.Binding with { ReleaseManifestFingerprint = "3333333333333333333333333333333333333333333333333333333333333333" };
                Check(!ProductionCaptureEvidenceAuditReplayConvergenceRuntime.IsValid(tampered,fixture.Descriptor) &&
                      (group4<9 || round==100),
                      "Release manifest identity drift must be rejected");
        }
        for(var group5=0;group5<10;group5++)
        {
            round++;
                var fixture=ProductionCaptureEvidenceAuditReplayConvergenceSmokeFixtures.Create();
                var tampered=fixture.Binding with { ReleaseManifestFingerprint = "3333333333333333333333333333333333333333333333333333333333333333" };
                Check(!ProductionCaptureEvidenceAuditReplayConvergenceRuntime.IsValid(tampered,fixture.Descriptor) &&
                      (group5<9 || round==100),
                      "Release manifest identity drift must be rejected");
        }
        for(var group6=0;group6<10;group6++)
        {
            round++;
                var fixture=ProductionCaptureEvidenceAuditReplayConvergenceSmokeFixtures.Create();
                var tampered=fixture.Binding with { ReleaseManifestFingerprint = "3333333333333333333333333333333333333333333333333333333333333333" };
                Check(!ProductionCaptureEvidenceAuditReplayConvergenceRuntime.IsValid(tampered,fixture.Descriptor) &&
                      (group6<9 || round==100),
                      "Release manifest identity drift must be rejected");
        }
        for(var group7=0;group7<10;group7++)
        {
            round++;
                var fixture=ProductionCaptureEvidenceAuditReplayConvergenceSmokeFixtures.Create();
                var tampered=fixture.Binding with { ReleaseManifestFingerprint = "3333333333333333333333333333333333333333333333333333333333333333" };
                Check(!ProductionCaptureEvidenceAuditReplayConvergenceRuntime.IsValid(tampered,fixture.Descriptor) &&
                      (group7<9 || round==100),
                      "Release manifest identity drift must be rejected");
        }
        for(var group8=0;group8<10;group8++)
        {
            round++;
                var fixture=ProductionCaptureEvidenceAuditReplayConvergenceSmokeFixtures.Create();
                var tampered=fixture.Binding with { ReleaseManifestFingerprint = "3333333333333333333333333333333333333333333333333333333333333333" };
                Check(!ProductionCaptureEvidenceAuditReplayConvergenceRuntime.IsValid(tampered,fixture.Descriptor) &&
                      (group8<9 || round==100),
                      "Release manifest identity drift must be rejected");
        }
        for(var group9=0;group9<10;group9++)
        {
            round++;
                var fixture=ProductionCaptureEvidenceAuditReplayConvergenceSmokeFixtures.Create();
                var tampered=fixture.Binding with { ReleaseManifestFingerprint = "3333333333333333333333333333333333333333333333333333333333333333" };
                Check(!ProductionCaptureEvidenceAuditReplayConvergenceRuntime.IsValid(tampered,fixture.Descriptor) &&
                      (group9<9 || round==100),
                      "Release manifest identity drift must be rejected");
        }
        return Task.CompletedTask;
    }
}
