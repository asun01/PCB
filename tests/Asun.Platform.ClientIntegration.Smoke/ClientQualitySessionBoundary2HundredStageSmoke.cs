using System.Numerics;
using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientQualitySessionBoundary2HundredStageSmoke
{
    private static Asun.Domain.Quality.QualityInspectionRun QualityRun() =>
        new(
            Guid.Parse("85000000-0000-0000-0000-000000000001"),
            new[]
            {
                new Asun.Domain.Quality.QualityInspectionResult(
                    Guid.Parse("85000000-0000-0000-0000-000000000101"),
                    new Asun.Domain.Quality.QualityInspectionSnapshot(
                        Guid.Parse("85000000-0000-0000-0000-000000000201"),
                        1,
                        new Asun.Domain.Quality.QualityFindingSet(Array.Empty<Asun.Domain.Quality.QualityFinding>()),
                        new Asun.Domain.Quality.QualityFindingEvidenceSet(Array.Empty<Asun.Domain.Quality.QualityFindingEvidenceLink>()))
                )
            });


    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(
                new Vector2(100,100),
                new Vector2(100,100));
            workspace.BindQualityRun(QualityRun());
            workspace.Load(
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateDefinition(
                    Guid.Parse("85000000-0000-0000-0000-000000000302"),3));
            Check(!workspace.Quality.IsBound,
                  "loading a new Production definition must clear stale Quality state");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(
                new Vector2(100,100),
                new Vector2(100,100));
            workspace.BindQualityRun(QualityRun());
            workspace.Load(
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateDefinition(
                    Guid.Parse("85000000-0000-0000-0000-000000000302"),3));
            Check(!workspace.Quality.IsBound,
                  "loading a new Production definition must clear stale Quality state");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(
                new Vector2(100,100),
                new Vector2(100,100));
            workspace.BindQualityRun(QualityRun());
            workspace.Load(
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateDefinition(
                    Guid.Parse("85000000-0000-0000-0000-000000000302"),3));
            Check(!workspace.Quality.IsBound,
                  "loading a new Production definition must clear stale Quality state");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(
                new Vector2(100,100),
                new Vector2(100,100));
            workspace.BindQualityRun(QualityRun());
            workspace.Load(
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateDefinition(
                    Guid.Parse("85000000-0000-0000-0000-000000000302"),3));
            Check(!workspace.Quality.IsBound,
                  "loading a new Production definition must clear stale Quality state");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(
                new Vector2(100,100),
                new Vector2(100,100));
            workspace.BindQualityRun(QualityRun());
            workspace.Load(
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateDefinition(
                    Guid.Parse("85000000-0000-0000-0000-000000000302"),3));
            Check(!workspace.Quality.IsBound,
                  "loading a new Production definition must clear stale Quality state");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(
                new Vector2(100,100),
                new Vector2(100,100));
            workspace.BindQualityRun(QualityRun());
            workspace.Load(
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateDefinition(
                    Guid.Parse("85000000-0000-0000-0000-000000000302"),3));
            Check(!workspace.Quality.IsBound,
                  "loading a new Production definition must clear stale Quality state");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(
                new Vector2(100,100),
                new Vector2(100,100));
            workspace.BindQualityRun(QualityRun());
            workspace.Load(
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateDefinition(
                    Guid.Parse("85000000-0000-0000-0000-000000000302"),3));
            Check(!workspace.Quality.IsBound,
                  "loading a new Production definition must clear stale Quality state");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(
                new Vector2(100,100),
                new Vector2(100,100));
            workspace.BindQualityRun(QualityRun());
            workspace.Load(
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateDefinition(
                    Guid.Parse("85000000-0000-0000-0000-000000000302"),3));
            Check(!workspace.Quality.IsBound,
                  "loading a new Production definition must clear stale Quality state");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(
                new Vector2(100,100),
                new Vector2(100,100));
            workspace.BindQualityRun(QualityRun());
            workspace.Load(
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateDefinition(
                    Guid.Parse("85000000-0000-0000-0000-000000000302"),3));
            Check(!workspace.Quality.IsBound,
                  "loading a new Production definition must clear stale Quality state");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(
                new Vector2(100,100),
                new Vector2(100,100));
            workspace.BindQualityRun(QualityRun());
            workspace.Load(
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateDefinition(
                    Guid.Parse("85000000-0000-0000-0000-000000000302"),3));
            Check(!workspace.Quality.IsBound,
                  "loading a new Production definition must clear stale Quality state");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
