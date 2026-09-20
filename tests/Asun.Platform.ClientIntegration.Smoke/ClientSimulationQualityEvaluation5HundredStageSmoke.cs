using System.Numerics;
using Asun.Platform.ClientIntegration;
using Asun.Platform.SimulationIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientSimulationQualityEvaluation5HundredStageSmoke
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
                Guid.Parse("89000000-0000-0000-0000-000000000105"),
                3);
            workspace.BindAcquisitionSource("simulation");
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            workspace.EvaluateQuality(ClientSimulationQualityRunProvider.Instance);
            var workflow=ClientInspectionWorkflowRuntime.Evaluate(workspace.Capture());
            Check(workflow.Step==ClientWorkflowStep.ReviewResults &&
                  workflow.HistoryCount==1 &&
                  workflow.QualityBound,
                  "completed simulated Production plus Quality must advance the unified client workflow to Results");
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
                Guid.Parse("89000000-0000-0000-0000-000000000105"),
                3);
            workspace.BindAcquisitionSource("simulation");
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            workspace.EvaluateQuality(ClientSimulationQualityRunProvider.Instance);
            var workflow=ClientInspectionWorkflowRuntime.Evaluate(workspace.Capture());
            Check(workflow.Step==ClientWorkflowStep.ReviewResults &&
                  workflow.HistoryCount==1 &&
                  workflow.QualityBound,
                  "completed simulated Production plus Quality must advance the unified client workflow to Results");
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
                Guid.Parse("89000000-0000-0000-0000-000000000105"),
                3);
            workspace.BindAcquisitionSource("simulation");
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            workspace.EvaluateQuality(ClientSimulationQualityRunProvider.Instance);
            var workflow=ClientInspectionWorkflowRuntime.Evaluate(workspace.Capture());
            Check(workflow.Step==ClientWorkflowStep.ReviewResults &&
                  workflow.HistoryCount==1 &&
                  workflow.QualityBound,
                  "completed simulated Production plus Quality must advance the unified client workflow to Results");
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
                Guid.Parse("89000000-0000-0000-0000-000000000105"),
                3);
            workspace.BindAcquisitionSource("simulation");
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            workspace.EvaluateQuality(ClientSimulationQualityRunProvider.Instance);
            var workflow=ClientInspectionWorkflowRuntime.Evaluate(workspace.Capture());
            Check(workflow.Step==ClientWorkflowStep.ReviewResults &&
                  workflow.HistoryCount==1 &&
                  workflow.QualityBound,
                  "completed simulated Production plus Quality must advance the unified client workflow to Results");
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
                Guid.Parse("89000000-0000-0000-0000-000000000105"),
                3);
            workspace.BindAcquisitionSource("simulation");
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            workspace.EvaluateQuality(ClientSimulationQualityRunProvider.Instance);
            var workflow=ClientInspectionWorkflowRuntime.Evaluate(workspace.Capture());
            Check(workflow.Step==ClientWorkflowStep.ReviewResults &&
                  workflow.HistoryCount==1 &&
                  workflow.QualityBound,
                  "completed simulated Production plus Quality must advance the unified client workflow to Results");
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
                Guid.Parse("89000000-0000-0000-0000-000000000105"),
                3);
            workspace.BindAcquisitionSource("simulation");
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            workspace.EvaluateQuality(ClientSimulationQualityRunProvider.Instance);
            var workflow=ClientInspectionWorkflowRuntime.Evaluate(workspace.Capture());
            Check(workflow.Step==ClientWorkflowStep.ReviewResults &&
                  workflow.HistoryCount==1 &&
                  workflow.QualityBound,
                  "completed simulated Production plus Quality must advance the unified client workflow to Results");
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
                Guid.Parse("89000000-0000-0000-0000-000000000105"),
                3);
            workspace.BindAcquisitionSource("simulation");
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            workspace.EvaluateQuality(ClientSimulationQualityRunProvider.Instance);
            var workflow=ClientInspectionWorkflowRuntime.Evaluate(workspace.Capture());
            Check(workflow.Step==ClientWorkflowStep.ReviewResults &&
                  workflow.HistoryCount==1 &&
                  workflow.QualityBound,
                  "completed simulated Production plus Quality must advance the unified client workflow to Results");
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
                Guid.Parse("89000000-0000-0000-0000-000000000105"),
                3);
            workspace.BindAcquisitionSource("simulation");
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            workspace.EvaluateQuality(ClientSimulationQualityRunProvider.Instance);
            var workflow=ClientInspectionWorkflowRuntime.Evaluate(workspace.Capture());
            Check(workflow.Step==ClientWorkflowStep.ReviewResults &&
                  workflow.HistoryCount==1 &&
                  workflow.QualityBound,
                  "completed simulated Production plus Quality must advance the unified client workflow to Results");
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
                Guid.Parse("89000000-0000-0000-0000-000000000105"),
                3);
            workspace.BindAcquisitionSource("simulation");
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            workspace.EvaluateQuality(ClientSimulationQualityRunProvider.Instance);
            var workflow=ClientInspectionWorkflowRuntime.Evaluate(workspace.Capture());
            Check(workflow.Step==ClientWorkflowStep.ReviewResults &&
                  workflow.HistoryCount==1 &&
                  workflow.QualityBound,
                  "completed simulated Production plus Quality must advance the unified client workflow to Results");
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
                Guid.Parse("89000000-0000-0000-0000-000000000105"),
                3);
            workspace.BindAcquisitionSource("simulation");
            await workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest());
            workspace.EvaluateQuality(ClientSimulationQualityRunProvider.Instance);
            var workflow=ClientInspectionWorkflowRuntime.Evaluate(workspace.Capture());
            Check(workflow.Step==ClientWorkflowStep.ReviewResults &&
                  workflow.HistoryCount==1 &&
                  workflow.QualityBound,
                  "completed simulated Production plus Quality must advance the unified client workflow to Results");
        }
        if(round==100)
            return;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
