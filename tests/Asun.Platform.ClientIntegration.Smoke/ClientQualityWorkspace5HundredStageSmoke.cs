using Asun.Domain.Quality;
using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientQualityWorkspace5HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var invalid=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000104"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000204"),
                            2,
                            new QualityFindingSet(Array.Empty<QualityFinding>()),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())),
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000105"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000205"),
                            1,
                            new QualityFindingSet(Array.Empty<QualityFinding>()),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                });
            var workspace=new ClientQualityWorkspace();
            var failed=false;
            try
            {
                workspace.Bind(invalid);
            }
            catch(ArgumentException)
            {
                failed=true;
            }
            Check(failed,
                  "invalid Quality Run ordering must be rejected by the client boundary");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var invalid=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000104"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000204"),
                            2,
                            new QualityFindingSet(Array.Empty<QualityFinding>()),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())),
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000105"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000205"),
                            1,
                            new QualityFindingSet(Array.Empty<QualityFinding>()),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                });
            var workspace=new ClientQualityWorkspace();
            var failed=false;
            try
            {
                workspace.Bind(invalid);
            }
            catch(ArgumentException)
            {
                failed=true;
            }
            Check(failed,
                  "invalid Quality Run ordering must be rejected by the client boundary");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var invalid=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000104"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000204"),
                            2,
                            new QualityFindingSet(Array.Empty<QualityFinding>()),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())),
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000105"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000205"),
                            1,
                            new QualityFindingSet(Array.Empty<QualityFinding>()),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                });
            var workspace=new ClientQualityWorkspace();
            var failed=false;
            try
            {
                workspace.Bind(invalid);
            }
            catch(ArgumentException)
            {
                failed=true;
            }
            Check(failed,
                  "invalid Quality Run ordering must be rejected by the client boundary");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var invalid=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000104"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000204"),
                            2,
                            new QualityFindingSet(Array.Empty<QualityFinding>()),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())),
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000105"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000205"),
                            1,
                            new QualityFindingSet(Array.Empty<QualityFinding>()),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                });
            var workspace=new ClientQualityWorkspace();
            var failed=false;
            try
            {
                workspace.Bind(invalid);
            }
            catch(ArgumentException)
            {
                failed=true;
            }
            Check(failed,
                  "invalid Quality Run ordering must be rejected by the client boundary");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var invalid=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000104"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000204"),
                            2,
                            new QualityFindingSet(Array.Empty<QualityFinding>()),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())),
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000105"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000205"),
                            1,
                            new QualityFindingSet(Array.Empty<QualityFinding>()),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                });
            var workspace=new ClientQualityWorkspace();
            var failed=false;
            try
            {
                workspace.Bind(invalid);
            }
            catch(ArgumentException)
            {
                failed=true;
            }
            Check(failed,
                  "invalid Quality Run ordering must be rejected by the client boundary");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var invalid=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000104"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000204"),
                            2,
                            new QualityFindingSet(Array.Empty<QualityFinding>()),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())),
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000105"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000205"),
                            1,
                            new QualityFindingSet(Array.Empty<QualityFinding>()),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                });
            var workspace=new ClientQualityWorkspace();
            var failed=false;
            try
            {
                workspace.Bind(invalid);
            }
            catch(ArgumentException)
            {
                failed=true;
            }
            Check(failed,
                  "invalid Quality Run ordering must be rejected by the client boundary");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var invalid=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000104"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000204"),
                            2,
                            new QualityFindingSet(Array.Empty<QualityFinding>()),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())),
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000105"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000205"),
                            1,
                            new QualityFindingSet(Array.Empty<QualityFinding>()),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                });
            var workspace=new ClientQualityWorkspace();
            var failed=false;
            try
            {
                workspace.Bind(invalid);
            }
            catch(ArgumentException)
            {
                failed=true;
            }
            Check(failed,
                  "invalid Quality Run ordering must be rejected by the client boundary");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var invalid=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000104"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000204"),
                            2,
                            new QualityFindingSet(Array.Empty<QualityFinding>()),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())),
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000105"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000205"),
                            1,
                            new QualityFindingSet(Array.Empty<QualityFinding>()),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                });
            var workspace=new ClientQualityWorkspace();
            var failed=false;
            try
            {
                workspace.Bind(invalid);
            }
            catch(ArgumentException)
            {
                failed=true;
            }
            Check(failed,
                  "invalid Quality Run ordering must be rejected by the client boundary");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var invalid=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000104"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000204"),
                            2,
                            new QualityFindingSet(Array.Empty<QualityFinding>()),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())),
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000105"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000205"),
                            1,
                            new QualityFindingSet(Array.Empty<QualityFinding>()),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                });
            var workspace=new ClientQualityWorkspace();
            var failed=false;
            try
            {
                workspace.Bind(invalid);
            }
            catch(ArgumentException)
            {
                failed=true;
            }
            Check(failed,
                  "invalid Quality Run ordering must be rejected by the client boundary");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var invalid=new QualityInspectionRun(
                Guid.Parse("84000000-0000-0000-0000-000000000001"),
                new[]
                {
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000104"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000204"),
                            2,
                            new QualityFindingSet(Array.Empty<QualityFinding>()),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())),
                    new QualityInspectionResult(
                        Guid.Parse("84000000-0000-0000-0000-000000000105"),
                        new QualityInspectionSnapshot(
                            Guid.Parse("84000000-0000-0000-0000-000000000205"),
                            1,
                            new QualityFindingSet(Array.Empty<QualityFinding>()),
                            new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))
                });
            var workspace=new ClientQualityWorkspace();
            var failed=false;
            try
            {
                workspace.Bind(invalid);
            }
            catch(ArgumentException)
            {
                failed=true;
            }
            Check(failed,
                  "invalid Quality Run ordering must be rejected by the client boundary");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
