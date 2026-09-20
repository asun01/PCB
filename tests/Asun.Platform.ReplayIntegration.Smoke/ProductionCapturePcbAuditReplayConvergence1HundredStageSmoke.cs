namespace Asun.Platform.ReplayIntegration.Smoke;

public static class ProductionCapturePcbAuditReplayConvergence1HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var group0=0;group0<10;group0++)
        {
            round++;
                var fixture=ProductionCapturePcbAuditReplayConvergenceSmokeFixtures.Create();
                var result=ProductionCapturePcbAuditReplayConvergenceRuntime.Create(fixture.Capture,fixture.Pcb);
                var repeat=ProductionCapturePcbAuditReplayConvergenceRuntime.Create(fixture.Capture,fixture.Pcb);
                Check(ProductionCapturePcbAuditReplayConvergenceRuntime.IsValid(fixture.Capture,fixture.Pcb) &&
                      ProductionCapturePcbAuditReplayConvergenceRuntime.IsEquivalent(result,repeat) &&
                      result.ProductionSessionId==fixture.Capture.ProductionSessionId &&
                      result.QualityRunId==fixture.Capture.QualityRunId &&
                      result.ReleaseManifestFingerprint==fixture.Pcb.ReleaseManifestFingerprint &&
                      result.Fingerprint.Length==64 &&
                      (group0<9 || round==100),
                      "clean capture/PCB audit replay convergence must be deterministic");
        }
        for(var group1=0;group1<10;group1++)
        {
            round++;
                var fixture=ProductionCapturePcbAuditReplayConvergenceSmokeFixtures.Create();
                var result=ProductionCapturePcbAuditReplayConvergenceRuntime.Create(fixture.Capture,fixture.Pcb);
                var repeat=ProductionCapturePcbAuditReplayConvergenceRuntime.Create(fixture.Capture,fixture.Pcb);
                Check(ProductionCapturePcbAuditReplayConvergenceRuntime.IsValid(fixture.Capture,fixture.Pcb) &&
                      ProductionCapturePcbAuditReplayConvergenceRuntime.IsEquivalent(result,repeat) &&
                      result.ProductionSessionId==fixture.Capture.ProductionSessionId &&
                      result.QualityRunId==fixture.Capture.QualityRunId &&
                      result.ReleaseManifestFingerprint==fixture.Pcb.ReleaseManifestFingerprint &&
                      result.Fingerprint.Length==64 &&
                      (group1<9 || round==100),
                      "clean capture/PCB audit replay convergence must be deterministic");
        }
        for(var group2=0;group2<10;group2++)
        {
            round++;
                var fixture=ProductionCapturePcbAuditReplayConvergenceSmokeFixtures.Create();
                var result=ProductionCapturePcbAuditReplayConvergenceRuntime.Create(fixture.Capture,fixture.Pcb);
                var repeat=ProductionCapturePcbAuditReplayConvergenceRuntime.Create(fixture.Capture,fixture.Pcb);
                Check(ProductionCapturePcbAuditReplayConvergenceRuntime.IsValid(fixture.Capture,fixture.Pcb) &&
                      ProductionCapturePcbAuditReplayConvergenceRuntime.IsEquivalent(result,repeat) &&
                      result.ProductionSessionId==fixture.Capture.ProductionSessionId &&
                      result.QualityRunId==fixture.Capture.QualityRunId &&
                      result.ReleaseManifestFingerprint==fixture.Pcb.ReleaseManifestFingerprint &&
                      result.Fingerprint.Length==64 &&
                      (group2<9 || round==100),
                      "clean capture/PCB audit replay convergence must be deterministic");
        }
        for(var group3=0;group3<10;group3++)
        {
            round++;
                var fixture=ProductionCapturePcbAuditReplayConvergenceSmokeFixtures.Create();
                var result=ProductionCapturePcbAuditReplayConvergenceRuntime.Create(fixture.Capture,fixture.Pcb);
                var repeat=ProductionCapturePcbAuditReplayConvergenceRuntime.Create(fixture.Capture,fixture.Pcb);
                Check(ProductionCapturePcbAuditReplayConvergenceRuntime.IsValid(fixture.Capture,fixture.Pcb) &&
                      ProductionCapturePcbAuditReplayConvergenceRuntime.IsEquivalent(result,repeat) &&
                      result.ProductionSessionId==fixture.Capture.ProductionSessionId &&
                      result.QualityRunId==fixture.Capture.QualityRunId &&
                      result.ReleaseManifestFingerprint==fixture.Pcb.ReleaseManifestFingerprint &&
                      result.Fingerprint.Length==64 &&
                      (group3<9 || round==100),
                      "clean capture/PCB audit replay convergence must be deterministic");
        }
        for(var group4=0;group4<10;group4++)
        {
            round++;
                var fixture=ProductionCapturePcbAuditReplayConvergenceSmokeFixtures.Create();
                var result=ProductionCapturePcbAuditReplayConvergenceRuntime.Create(fixture.Capture,fixture.Pcb);
                var repeat=ProductionCapturePcbAuditReplayConvergenceRuntime.Create(fixture.Capture,fixture.Pcb);
                Check(ProductionCapturePcbAuditReplayConvergenceRuntime.IsValid(fixture.Capture,fixture.Pcb) &&
                      ProductionCapturePcbAuditReplayConvergenceRuntime.IsEquivalent(result,repeat) &&
                      result.ProductionSessionId==fixture.Capture.ProductionSessionId &&
                      result.QualityRunId==fixture.Capture.QualityRunId &&
                      result.ReleaseManifestFingerprint==fixture.Pcb.ReleaseManifestFingerprint &&
                      result.Fingerprint.Length==64 &&
                      (group4<9 || round==100),
                      "clean capture/PCB audit replay convergence must be deterministic");
        }
        for(var group5=0;group5<10;group5++)
        {
            round++;
                var fixture=ProductionCapturePcbAuditReplayConvergenceSmokeFixtures.Create();
                var result=ProductionCapturePcbAuditReplayConvergenceRuntime.Create(fixture.Capture,fixture.Pcb);
                var repeat=ProductionCapturePcbAuditReplayConvergenceRuntime.Create(fixture.Capture,fixture.Pcb);
                Check(ProductionCapturePcbAuditReplayConvergenceRuntime.IsValid(fixture.Capture,fixture.Pcb) &&
                      ProductionCapturePcbAuditReplayConvergenceRuntime.IsEquivalent(result,repeat) &&
                      result.ProductionSessionId==fixture.Capture.ProductionSessionId &&
                      result.QualityRunId==fixture.Capture.QualityRunId &&
                      result.ReleaseManifestFingerprint==fixture.Pcb.ReleaseManifestFingerprint &&
                      result.Fingerprint.Length==64 &&
                      (group5<9 || round==100),
                      "clean capture/PCB audit replay convergence must be deterministic");
        }
        for(var group6=0;group6<10;group6++)
        {
            round++;
                var fixture=ProductionCapturePcbAuditReplayConvergenceSmokeFixtures.Create();
                var result=ProductionCapturePcbAuditReplayConvergenceRuntime.Create(fixture.Capture,fixture.Pcb);
                var repeat=ProductionCapturePcbAuditReplayConvergenceRuntime.Create(fixture.Capture,fixture.Pcb);
                Check(ProductionCapturePcbAuditReplayConvergenceRuntime.IsValid(fixture.Capture,fixture.Pcb) &&
                      ProductionCapturePcbAuditReplayConvergenceRuntime.IsEquivalent(result,repeat) &&
                      result.ProductionSessionId==fixture.Capture.ProductionSessionId &&
                      result.QualityRunId==fixture.Capture.QualityRunId &&
                      result.ReleaseManifestFingerprint==fixture.Pcb.ReleaseManifestFingerprint &&
                      result.Fingerprint.Length==64 &&
                      (group6<9 || round==100),
                      "clean capture/PCB audit replay convergence must be deterministic");
        }
        for(var group7=0;group7<10;group7++)
        {
            round++;
                var fixture=ProductionCapturePcbAuditReplayConvergenceSmokeFixtures.Create();
                var result=ProductionCapturePcbAuditReplayConvergenceRuntime.Create(fixture.Capture,fixture.Pcb);
                var repeat=ProductionCapturePcbAuditReplayConvergenceRuntime.Create(fixture.Capture,fixture.Pcb);
                Check(ProductionCapturePcbAuditReplayConvergenceRuntime.IsValid(fixture.Capture,fixture.Pcb) &&
                      ProductionCapturePcbAuditReplayConvergenceRuntime.IsEquivalent(result,repeat) &&
                      result.ProductionSessionId==fixture.Capture.ProductionSessionId &&
                      result.QualityRunId==fixture.Capture.QualityRunId &&
                      result.ReleaseManifestFingerprint==fixture.Pcb.ReleaseManifestFingerprint &&
                      result.Fingerprint.Length==64 &&
                      (group7<9 || round==100),
                      "clean capture/PCB audit replay convergence must be deterministic");
        }
        for(var group8=0;group8<10;group8++)
        {
            round++;
                var fixture=ProductionCapturePcbAuditReplayConvergenceSmokeFixtures.Create();
                var result=ProductionCapturePcbAuditReplayConvergenceRuntime.Create(fixture.Capture,fixture.Pcb);
                var repeat=ProductionCapturePcbAuditReplayConvergenceRuntime.Create(fixture.Capture,fixture.Pcb);
                Check(ProductionCapturePcbAuditReplayConvergenceRuntime.IsValid(fixture.Capture,fixture.Pcb) &&
                      ProductionCapturePcbAuditReplayConvergenceRuntime.IsEquivalent(result,repeat) &&
                      result.ProductionSessionId==fixture.Capture.ProductionSessionId &&
                      result.QualityRunId==fixture.Capture.QualityRunId &&
                      result.ReleaseManifestFingerprint==fixture.Pcb.ReleaseManifestFingerprint &&
                      result.Fingerprint.Length==64 &&
                      (group8<9 || round==100),
                      "clean capture/PCB audit replay convergence must be deterministic");
        }
        for(var group9=0;group9<10;group9++)
        {
            round++;
                var fixture=ProductionCapturePcbAuditReplayConvergenceSmokeFixtures.Create();
                var result=ProductionCapturePcbAuditReplayConvergenceRuntime.Create(fixture.Capture,fixture.Pcb);
                var repeat=ProductionCapturePcbAuditReplayConvergenceRuntime.Create(fixture.Capture,fixture.Pcb);
                Check(ProductionCapturePcbAuditReplayConvergenceRuntime.IsValid(fixture.Capture,fixture.Pcb) &&
                      ProductionCapturePcbAuditReplayConvergenceRuntime.IsEquivalent(result,repeat) &&
                      result.ProductionSessionId==fixture.Capture.ProductionSessionId &&
                      result.QualityRunId==fixture.Capture.QualityRunId &&
                      result.ReleaseManifestFingerprint==fixture.Pcb.ReleaseManifestFingerprint &&
                      result.Fingerprint.Length==64 &&
                      (group9<9 || round==100),
                      "clean capture/PCB audit replay convergence must be deterministic");
        }
        return Task.CompletedTask;
    }
}
