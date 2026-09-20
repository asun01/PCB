using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientQualityFindingSelection5HundredStageSmoke
{
    private static Asun.Domain.Quality.QualityInspectionRun Run() =>
        new(
            Guid.Parse("87000000-0000-0000-0000-000000000001"),
            new[]
            {
                new Asun.Domain.Quality.QualityInspectionResult(
                    Guid.Parse("87000000-0000-0000-0000-000000000101"),
                    new Asun.Domain.Quality.QualityInspectionSnapshot(
                        Guid.Parse("87000000-0000-0000-0000-000000000201"),
                        1,
                        new Asun.Domain.Quality.QualityFindingSet(new[]
                        {
                            new Asun.Domain.Quality.QualityFinding(
                                Asun.Domain.Quality.QualityFindingId.Create("F-001"),
                                "WIDTH",
                                Asun.Domain.Quality.QualityOutcome.Pass,
                                Asun.Domain.Quality.QualitySeverity.None,
                                "Pass"),
                            new Asun.Domain.Quality.QualityFinding(
                                Asun.Domain.Quality.QualityFindingId.Create("F-002"),
                                "OFFSET",
                                Asun.Domain.Quality.QualityOutcome.Review,
                                Asun.Domain.Quality.QualitySeverity.Warning,
                                "Review")
                        }),
                        new Asun.Domain.Quality.QualityFindingEvidenceSet(Array.Empty<Asun.Domain.Quality.QualityFindingEvidenceLink>()))
                )
            });

    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(Run());
            var first=workspace.Capture().Fingerprint;
            workspace.SelectFinding("F-002");
            var second=workspace.Capture().Fingerprint;
            Check(first==second &&
                  workspace.Capture().SelectedFindingId=="F-002",
                  "changing Quality presentation selection must not mutate the underlying Quality fingerprint");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(Run());
            var first=workspace.Capture().Fingerprint;
            workspace.SelectFinding("F-002");
            var second=workspace.Capture().Fingerprint;
            Check(first==second &&
                  workspace.Capture().SelectedFindingId=="F-002",
                  "changing Quality presentation selection must not mutate the underlying Quality fingerprint");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(Run());
            var first=workspace.Capture().Fingerprint;
            workspace.SelectFinding("F-002");
            var second=workspace.Capture().Fingerprint;
            Check(first==second &&
                  workspace.Capture().SelectedFindingId=="F-002",
                  "changing Quality presentation selection must not mutate the underlying Quality fingerprint");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(Run());
            var first=workspace.Capture().Fingerprint;
            workspace.SelectFinding("F-002");
            var second=workspace.Capture().Fingerprint;
            Check(first==second &&
                  workspace.Capture().SelectedFindingId=="F-002",
                  "changing Quality presentation selection must not mutate the underlying Quality fingerprint");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(Run());
            var first=workspace.Capture().Fingerprint;
            workspace.SelectFinding("F-002");
            var second=workspace.Capture().Fingerprint;
            Check(first==second &&
                  workspace.Capture().SelectedFindingId=="F-002",
                  "changing Quality presentation selection must not mutate the underlying Quality fingerprint");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(Run());
            var first=workspace.Capture().Fingerprint;
            workspace.SelectFinding("F-002");
            var second=workspace.Capture().Fingerprint;
            Check(first==second &&
                  workspace.Capture().SelectedFindingId=="F-002",
                  "changing Quality presentation selection must not mutate the underlying Quality fingerprint");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(Run());
            var first=workspace.Capture().Fingerprint;
            workspace.SelectFinding("F-002");
            var second=workspace.Capture().Fingerprint;
            Check(first==second &&
                  workspace.Capture().SelectedFindingId=="F-002",
                  "changing Quality presentation selection must not mutate the underlying Quality fingerprint");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(Run());
            var first=workspace.Capture().Fingerprint;
            workspace.SelectFinding("F-002");
            var second=workspace.Capture().Fingerprint;
            Check(first==second &&
                  workspace.Capture().SelectedFindingId=="F-002",
                  "changing Quality presentation selection must not mutate the underlying Quality fingerprint");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(Run());
            var first=workspace.Capture().Fingerprint;
            workspace.SelectFinding("F-002");
            var second=workspace.Capture().Fingerprint;
            Check(first==second &&
                  workspace.Capture().SelectedFindingId=="F-002",
                  "changing Quality presentation selection must not mutate the underlying Quality fingerprint");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var workspace=new ClientQualityWorkspace();
            workspace.Bind(Run());
            var first=workspace.Capture().Fingerprint;
            workspace.SelectFinding("F-002");
            var second=workspace.Capture().Fingerprint;
            Check(first==second &&
                  workspace.Capture().SelectedFindingId=="F-002",
                  "changing Quality presentation selection must not mutate the underlying Quality fingerprint");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
