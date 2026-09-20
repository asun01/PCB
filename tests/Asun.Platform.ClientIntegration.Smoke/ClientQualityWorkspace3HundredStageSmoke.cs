using Asun.Domain.Quality;
using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientQualityWorkspace3HundredStageSmoke
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
                        Guid.Parse("84000000-0000-0000-0000-000000000102"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000202"),
                            1,
                            new QualityFindingSet(new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("F-002"),
                                    "PAD_OFFSET",
                                    QualityOutcome.Fail,
                                    QualitySeverity.Critical,
                                    "Inspection result requires review.")
                            }),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                    )
                });
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(run);
            var snapshot=workspace.Capture();
            Check(snapshot.FailCount==1 &&
                  snapshot.PassCount==0 &&
                  snapshot.ReviewCount==0 &&
                  snapshot.EvidenceLinkCount==0 &&
                  snapshot.Findings[0].Severity=="Critical",
                  "Quality outcome and severity must remain visible without fabricating evidence");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var run=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000102"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000202"),
                            1,
                            new QualityFindingSet(new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("F-002"),
                                    "PAD_OFFSET",
                                    QualityOutcome.Fail,
                                    QualitySeverity.Critical,
                                    "Inspection result requires review.")
                            }),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                    )
                });
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(run);
            var snapshot=workspace.Capture();
            Check(snapshot.FailCount==1 &&
                  snapshot.PassCount==0 &&
                  snapshot.ReviewCount==0 &&
                  snapshot.EvidenceLinkCount==0 &&
                  snapshot.Findings[0].Severity=="Critical",
                  "Quality outcome and severity must remain visible without fabricating evidence");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var run=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000102"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000202"),
                            1,
                            new QualityFindingSet(new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("F-002"),
                                    "PAD_OFFSET",
                                    QualityOutcome.Fail,
                                    QualitySeverity.Critical,
                                    "Inspection result requires review.")
                            }),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                    )
                });
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(run);
            var snapshot=workspace.Capture();
            Check(snapshot.FailCount==1 &&
                  snapshot.PassCount==0 &&
                  snapshot.ReviewCount==0 &&
                  snapshot.EvidenceLinkCount==0 &&
                  snapshot.Findings[0].Severity=="Critical",
                  "Quality outcome and severity must remain visible without fabricating evidence");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var run=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000102"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000202"),
                            1,
                            new QualityFindingSet(new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("F-002"),
                                    "PAD_OFFSET",
                                    QualityOutcome.Fail,
                                    QualitySeverity.Critical,
                                    "Inspection result requires review.")
                            }),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                    )
                });
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(run);
            var snapshot=workspace.Capture();
            Check(snapshot.FailCount==1 &&
                  snapshot.PassCount==0 &&
                  snapshot.ReviewCount==0 &&
                  snapshot.EvidenceLinkCount==0 &&
                  snapshot.Findings[0].Severity=="Critical",
                  "Quality outcome and severity must remain visible without fabricating evidence");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var run=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000102"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000202"),
                            1,
                            new QualityFindingSet(new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("F-002"),
                                    "PAD_OFFSET",
                                    QualityOutcome.Fail,
                                    QualitySeverity.Critical,
                                    "Inspection result requires review.")
                            }),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                    )
                });
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(run);
            var snapshot=workspace.Capture();
            Check(snapshot.FailCount==1 &&
                  snapshot.PassCount==0 &&
                  snapshot.ReviewCount==0 &&
                  snapshot.EvidenceLinkCount==0 &&
                  snapshot.Findings[0].Severity=="Critical",
                  "Quality outcome and severity must remain visible without fabricating evidence");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var run=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000102"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000202"),
                            1,
                            new QualityFindingSet(new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("F-002"),
                                    "PAD_OFFSET",
                                    QualityOutcome.Fail,
                                    QualitySeverity.Critical,
                                    "Inspection result requires review.")
                            }),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                    )
                });
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(run);
            var snapshot=workspace.Capture();
            Check(snapshot.FailCount==1 &&
                  snapshot.PassCount==0 &&
                  snapshot.ReviewCount==0 &&
                  snapshot.EvidenceLinkCount==0 &&
                  snapshot.Findings[0].Severity=="Critical",
                  "Quality outcome and severity must remain visible without fabricating evidence");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var run=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000102"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000202"),
                            1,
                            new QualityFindingSet(new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("F-002"),
                                    "PAD_OFFSET",
                                    QualityOutcome.Fail,
                                    QualitySeverity.Critical,
                                    "Inspection result requires review.")
                            }),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                    )
                });
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(run);
            var snapshot=workspace.Capture();
            Check(snapshot.FailCount==1 &&
                  snapshot.PassCount==0 &&
                  snapshot.ReviewCount==0 &&
                  snapshot.EvidenceLinkCount==0 &&
                  snapshot.Findings[0].Severity=="Critical",
                  "Quality outcome and severity must remain visible without fabricating evidence");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var run=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000102"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000202"),
                            1,
                            new QualityFindingSet(new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("F-002"),
                                    "PAD_OFFSET",
                                    QualityOutcome.Fail,
                                    QualitySeverity.Critical,
                                    "Inspection result requires review.")
                            }),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                    )
                });
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(run);
            var snapshot=workspace.Capture();
            Check(snapshot.FailCount==1 &&
                  snapshot.PassCount==0 &&
                  snapshot.ReviewCount==0 &&
                  snapshot.EvidenceLinkCount==0 &&
                  snapshot.Findings[0].Severity=="Critical",
                  "Quality outcome and severity must remain visible without fabricating evidence");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var run=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000102"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000202"),
                            1,
                            new QualityFindingSet(new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("F-002"),
                                    "PAD_OFFSET",
                                    QualityOutcome.Fail,
                                    QualitySeverity.Critical,
                                    "Inspection result requires review.")
                            }),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                    )
                });
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(run);
            var snapshot=workspace.Capture();
            Check(snapshot.FailCount==1 &&
                  snapshot.PassCount==0 &&
                  snapshot.ReviewCount==0 &&
                  snapshot.EvidenceLinkCount==0 &&
                  snapshot.Findings[0].Severity=="Critical",
                  "Quality outcome and severity must remain visible without fabricating evidence");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var run=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000102"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000202"),
                            1,
                            new QualityFindingSet(new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("F-002"),
                                    "PAD_OFFSET",
                                    QualityOutcome.Fail,
                                    QualitySeverity.Critical,
                                    "Inspection result requires review.")
                            }),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                    )
                });
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(run);
            var snapshot=workspace.Capture();
            Check(snapshot.FailCount==1 &&
                  snapshot.PassCount==0 &&
                  snapshot.ReviewCount==0 &&
                  snapshot.EvidenceLinkCount==0 &&
                  snapshot.Findings[0].Severity=="Critical",
                  "Quality outcome and severity must remain visible without fabricating evidence");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
