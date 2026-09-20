using Asun.Domain.Quality;
using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientQualityWorkspace4HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var run=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000103"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000203"),
                            1,
                            new QualityFindingSet(new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("F-003"),
                                    "PLACEMENT",
                                    QualityOutcome.Review,
                                    QualitySeverity.Warning,
                                    "Manual review required.")
                            }),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                    )
                });
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(run);
            var first=workspace.Capture();
            var second=workspace.Capture();
            Check(first.Fingerprint==second.Fingerprint &&
                  first.FindingCount==1 &&
                  first.ReviewCount==1,
                  "Quality presentation fingerprint must be deterministic across repeated capture");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var run=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000103"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000203"),
                            1,
                            new QualityFindingSet(new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("F-003"),
                                    "PLACEMENT",
                                    QualityOutcome.Review,
                                    QualitySeverity.Warning,
                                    "Manual review required.")
                            }),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                    )
                });
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(run);
            var first=workspace.Capture();
            var second=workspace.Capture();
            Check(first.Fingerprint==second.Fingerprint &&
                  first.FindingCount==1 &&
                  first.ReviewCount==1,
                  "Quality presentation fingerprint must be deterministic across repeated capture");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var run=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000103"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000203"),
                            1,
                            new QualityFindingSet(new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("F-003"),
                                    "PLACEMENT",
                                    QualityOutcome.Review,
                                    QualitySeverity.Warning,
                                    "Manual review required.")
                            }),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                    )
                });
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(run);
            var first=workspace.Capture();
            var second=workspace.Capture();
            Check(first.Fingerprint==second.Fingerprint &&
                  first.FindingCount==1 &&
                  first.ReviewCount==1,
                  "Quality presentation fingerprint must be deterministic across repeated capture");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var run=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000103"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000203"),
                            1,
                            new QualityFindingSet(new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("F-003"),
                                    "PLACEMENT",
                                    QualityOutcome.Review,
                                    QualitySeverity.Warning,
                                    "Manual review required.")
                            }),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                    )
                });
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(run);
            var first=workspace.Capture();
            var second=workspace.Capture();
            Check(first.Fingerprint==second.Fingerprint &&
                  first.FindingCount==1 &&
                  first.ReviewCount==1,
                  "Quality presentation fingerprint must be deterministic across repeated capture");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var run=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000103"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000203"),
                            1,
                            new QualityFindingSet(new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("F-003"),
                                    "PLACEMENT",
                                    QualityOutcome.Review,
                                    QualitySeverity.Warning,
                                    "Manual review required.")
                            }),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                    )
                });
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(run);
            var first=workspace.Capture();
            var second=workspace.Capture();
            Check(first.Fingerprint==second.Fingerprint &&
                  first.FindingCount==1 &&
                  first.ReviewCount==1,
                  "Quality presentation fingerprint must be deterministic across repeated capture");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var run=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000103"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000203"),
                            1,
                            new QualityFindingSet(new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("F-003"),
                                    "PLACEMENT",
                                    QualityOutcome.Review,
                                    QualitySeverity.Warning,
                                    "Manual review required.")
                            }),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                    )
                });
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(run);
            var first=workspace.Capture();
            var second=workspace.Capture();
            Check(first.Fingerprint==second.Fingerprint &&
                  first.FindingCount==1 &&
                  first.ReviewCount==1,
                  "Quality presentation fingerprint must be deterministic across repeated capture");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var run=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000103"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000203"),
                            1,
                            new QualityFindingSet(new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("F-003"),
                                    "PLACEMENT",
                                    QualityOutcome.Review,
                                    QualitySeverity.Warning,
                                    "Manual review required.")
                            }),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                    )
                });
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(run);
            var first=workspace.Capture();
            var second=workspace.Capture();
            Check(first.Fingerprint==second.Fingerprint &&
                  first.FindingCount==1 &&
                  first.ReviewCount==1,
                  "Quality presentation fingerprint must be deterministic across repeated capture");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var run=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000103"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000203"),
                            1,
                            new QualityFindingSet(new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("F-003"),
                                    "PLACEMENT",
                                    QualityOutcome.Review,
                                    QualitySeverity.Warning,
                                    "Manual review required.")
                            }),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                    )
                });
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(run);
            var first=workspace.Capture();
            var second=workspace.Capture();
            Check(first.Fingerprint==second.Fingerprint &&
                  first.FindingCount==1 &&
                  first.ReviewCount==1,
                  "Quality presentation fingerprint must be deterministic across repeated capture");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var run=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000103"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000203"),
                            1,
                            new QualityFindingSet(new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("F-003"),
                                    "PLACEMENT",
                                    QualityOutcome.Review,
                                    QualitySeverity.Warning,
                                    "Manual review required.")
                            }),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                    )
                });
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(run);
            var first=workspace.Capture();
            var second=workspace.Capture();
            Check(first.Fingerprint==second.Fingerprint &&
                  first.FindingCount==1 &&
                  first.ReviewCount==1,
                  "Quality presentation fingerprint must be deterministic across repeated capture");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var run=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000103"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000203"),
                            1,
                            new QualityFindingSet(new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("F-003"),
                                    "PLACEMENT",
                                    QualityOutcome.Review,
                                    QualitySeverity.Warning,
                                    "Manual review required.")
                            }),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                    )
                });
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(run);
            var first=workspace.Capture();
            var second=workspace.Capture();
            Check(first.Fingerprint==second.Fingerprint &&
                  first.FindingCount==1 &&
                  first.ReviewCount==1,
                  "Quality presentation fingerprint must be deterministic across repeated capture");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
