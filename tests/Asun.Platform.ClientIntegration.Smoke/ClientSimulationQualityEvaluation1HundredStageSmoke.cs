using System.Numerics;
using Asun.Platform.ClientIntegration;
using Asun.Platform.SimulationIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientSimulationQualityEvaluation1HundredStageSmoke
{
    public static async Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(
                new Vector2(100,100),
                new Vector2(100,100));
            workspace.LoadProgram(
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("89000000-0000-0000-0000-000000000101"),
                3);
            workspace.BindAcquisitionSource("simulation");
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            var quality=workspace.EvaluateQuality(
                ClientSimulationQualityRunProvider.Instance);
            Check(quality.IsBound &&
                  quality.Provider?.IsSimulation==true &&
                  quality.ResultCount==3 &&
                  quality.FindingCount==3,
                  "Simulation Quality provider must consume the completed Production report and create a validated run");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(
                new Vector2(100,100),
                new Vector2(100,100));
            workspace.LoadProgram(
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("89000000-0000-0000-0000-000000000101"),
                3);
            workspace.BindAcquisitionSource("simulation");
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            var quality=workspace.EvaluateQuality(
                ClientSimulationQualityRunProvider.Instance);
            Check(quality.IsBound &&
                  quality.Provider?.IsSimulation==true &&
                  quality.ResultCount==3 &&
                  quality.FindingCount==3,
                  "Simulation Quality provider must consume the completed Production report and create a validated run");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(
                new Vector2(100,100),
                new Vector2(100,100));
            workspace.LoadProgram(
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("89000000-0000-0000-0000-000000000101"),
                3);
            workspace.BindAcquisitionSource("simulation");
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            var quality=workspace.EvaluateQuality(
                ClientSimulationQualityRunProvider.Instance);
            Check(quality.IsBound &&
                  quality.Provider?.IsSimulation==true &&
                  quality.ResultCount==3 &&
                  quality.FindingCount==3,
                  "Simulation Quality provider must consume the completed Production report and create a validated run");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(
                new Vector2(100,100),
                new Vector2(100,100));
            workspace.LoadProgram(
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("89000000-0000-0000-0000-000000000101"),
                3);
            workspace.BindAcquisitionSource("simulation");
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            var quality=workspace.EvaluateQuality(
                ClientSimulationQualityRunProvider.Instance);
            Check(quality.IsBound &&
                  quality.Provider?.IsSimulation==true &&
                  quality.ResultCount==3 &&
                  quality.FindingCount==3,
                  "Simulation Quality provider must consume the completed Production report and create a validated run");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(
                new Vector2(100,100),
                new Vector2(100,100));
            workspace.LoadProgram(
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("89000000-0000-0000-0000-000000000101"),
                3);
            workspace.BindAcquisitionSource("simulation");
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            var quality=workspace.EvaluateQuality(
                ClientSimulationQualityRunProvider.Instance);
            Check(quality.IsBound &&
                  quality.Provider?.IsSimulation==true &&
                  quality.ResultCount==3 &&
                  quality.FindingCount==3,
                  "Simulation Quality provider must consume the completed Production report and create a validated run");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(
                new Vector2(100,100),
                new Vector2(100,100));
            workspace.LoadProgram(
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("89000000-0000-0000-0000-000000000101"),
                3);
            workspace.BindAcquisitionSource("simulation");
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            var quality=workspace.EvaluateQuality(
                ClientSimulationQualityRunProvider.Instance);
            Check(quality.IsBound &&
                  quality.Provider?.IsSimulation==true &&
                  quality.ResultCount==3 &&
                  quality.FindingCount==3,
                  "Simulation Quality provider must consume the completed Production report and create a validated run");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(
                new Vector2(100,100),
                new Vector2(100,100));
            workspace.LoadProgram(
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("89000000-0000-0000-0000-000000000101"),
                3);
            workspace.BindAcquisitionSource("simulation");
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            var quality=workspace.EvaluateQuality(
                ClientSimulationQualityRunProvider.Instance);
            Check(quality.IsBound &&
                  quality.Provider?.IsSimulation==true &&
                  quality.ResultCount==3 &&
                  quality.FindingCount==3,
                  "Simulation Quality provider must consume the completed Production report and create a validated run");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(
                new Vector2(100,100),
                new Vector2(100,100));
            workspace.LoadProgram(
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("89000000-0000-0000-0000-000000000101"),
                3);
            workspace.BindAcquisitionSource("simulation");
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            var quality=workspace.EvaluateQuality(
                ClientSimulationQualityRunProvider.Instance);
            Check(quality.IsBound &&
                  quality.Provider?.IsSimulation==true &&
                  quality.ResultCount==3 &&
                  quality.FindingCount==3,
                  "Simulation Quality provider must consume the completed Production report and create a validated run");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(
                new Vector2(100,100),
                new Vector2(100,100));
            workspace.LoadProgram(
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("89000000-0000-0000-0000-000000000101"),
                3);
            workspace.BindAcquisitionSource("simulation");
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            var quality=workspace.EvaluateQuality(
                ClientSimulationQualityRunProvider.Instance);
            Check(quality.IsBound &&
                  quality.Provider?.IsSimulation==true &&
                  quality.ResultCount==3 &&
                  quality.FindingCount==3,
                  "Simulation Quality provider must consume the completed Production report and create a validated run");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(
                new Vector2(100,100),
                new Vector2(100,100));
            workspace.LoadProgram(
                ClientSimulationSessionFactory.CreateProgram(),
                ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("89000000-0000-0000-0000-000000000101"),
                3);
            workspace.BindAcquisitionSource("simulation");
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            var quality=workspace.EvaluateQuality(
                ClientSimulationQualityRunProvider.Instance);
            Check(quality.IsBound &&
                  quality.Provider?.IsSimulation==true &&
                  quality.ResultCount==3 &&
                  quality.FindingCount==3,
                  "Simulation Quality provider must consume the completed Production report and create a validated run");
        }
        if(round==100)
            return;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
