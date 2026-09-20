using System.Numerics;
using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientResultHistorySelection3HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(100,100));
            workspace.LoadProgram(
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateProgram(),
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("86000000-0000-0000-0000-000000000301"),
                3);
            workspace.BindAcquisitionSource("missing");
            Check(!workspace.Acquisition.CanCapture &&
                  workspace.SelectedHistoryOrdinal is null,
                  "Result selection must remain independent of an unavailable Acquisition source");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(100,100));
            workspace.LoadProgram(
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateProgram(),
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("86000000-0000-0000-0000-000000000301"),
                3);
            workspace.BindAcquisitionSource("missing");
            Check(!workspace.Acquisition.CanCapture &&
                  workspace.SelectedHistoryOrdinal is null,
                  "Result selection must remain independent of an unavailable Acquisition source");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(100,100));
            workspace.LoadProgram(
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateProgram(),
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("86000000-0000-0000-0000-000000000301"),
                3);
            workspace.BindAcquisitionSource("missing");
            Check(!workspace.Acquisition.CanCapture &&
                  workspace.SelectedHistoryOrdinal is null,
                  "Result selection must remain independent of an unavailable Acquisition source");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(100,100));
            workspace.LoadProgram(
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateProgram(),
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("86000000-0000-0000-0000-000000000301"),
                3);
            workspace.BindAcquisitionSource("missing");
            Check(!workspace.Acquisition.CanCapture &&
                  workspace.SelectedHistoryOrdinal is null,
                  "Result selection must remain independent of an unavailable Acquisition source");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(100,100));
            workspace.LoadProgram(
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateProgram(),
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("86000000-0000-0000-0000-000000000301"),
                3);
            workspace.BindAcquisitionSource("missing");
            Check(!workspace.Acquisition.CanCapture &&
                  workspace.SelectedHistoryOrdinal is null,
                  "Result selection must remain independent of an unavailable Acquisition source");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(100,100));
            workspace.LoadProgram(
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateProgram(),
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("86000000-0000-0000-0000-000000000301"),
                3);
            workspace.BindAcquisitionSource("missing");
            Check(!workspace.Acquisition.CanCapture &&
                  workspace.SelectedHistoryOrdinal is null,
                  "Result selection must remain independent of an unavailable Acquisition source");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(100,100));
            workspace.LoadProgram(
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateProgram(),
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("86000000-0000-0000-0000-000000000301"),
                3);
            workspace.BindAcquisitionSource("missing");
            Check(!workspace.Acquisition.CanCapture &&
                  workspace.SelectedHistoryOrdinal is null,
                  "Result selection must remain independent of an unavailable Acquisition source");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(100,100));
            workspace.LoadProgram(
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateProgram(),
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("86000000-0000-0000-0000-000000000301"),
                3);
            workspace.BindAcquisitionSource("missing");
            Check(!workspace.Acquisition.CanCapture &&
                  workspace.SelectedHistoryOrdinal is null,
                  "Result selection must remain independent of an unavailable Acquisition source");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(100,100));
            workspace.LoadProgram(
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateProgram(),
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("86000000-0000-0000-0000-000000000301"),
                3);
            workspace.BindAcquisitionSource("missing");
            Check(!workspace.Acquisition.CanCapture &&
                  workspace.SelectedHistoryOrdinal is null,
                  "Result selection must remain independent of an unavailable Acquisition source");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var workspace=new ClientInspectionWorkspace(new Vector2(100,100),new Vector2(100,100));
            workspace.LoadProgram(
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateProgram(),
                Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreatePipeline(),
                Guid.Parse("86000000-0000-0000-0000-000000000301"),
                3);
            workspace.BindAcquisitionSource("missing");
            Check(!workspace.Acquisition.CanCapture &&
                  workspace.SelectedHistoryOrdinal is null,
                  "Result selection must remain independent of an unavailable Acquisition source");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
