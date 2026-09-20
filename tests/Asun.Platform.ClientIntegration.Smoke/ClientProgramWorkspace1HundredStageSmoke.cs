using Asun.Device.Contracts;
using Asun.Platform.Pipeline;
using Asun.Production.Runtime;
using Asun.Program.Core;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientProgramWorkspace1HundredStageSmoke
{
    private static InspectionProgram Program(bool valid=true)
    {
        var id=Guid.Parse("81000000-0000-0000-0000-000000000001");
        var stepId=Guid.Parse("81000000-0000-0000-0000-000000000101");
        return new InspectionProgram(
            id,
            valid ? "Client Program" : "",
            new Version(1,0,0),
            new[]
            {
                new ProgramStep(
                    stepId,
                    1,
                    ProgramStepKind.Acquire,
                    valid ? "Acquire" : "",
                    new[]{new ProgramParameter("mode","client")})
            });
    }

    private static PipelineDefinition<CapturedFrame> Pipeline() =>
        new(new[]
        {
            new PipelineStage<CapturedFrame>(
                1,
                "Acquire",
                frame=>frame)
        });

    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var workspace=new ClientProgramWorkspace();
            var snapshot=workspace.Load(Program());
            Check(snapshot.Status==ClientProgramLoadStatus.Ready &&
                  snapshot.ProgramId==Guid.Parse("81000000-0000-0000-0000-000000000001") &&
                  snapshot.StepCount==1 &&
                  snapshot.ExecutionPlanFingerprint is { Length:64 },
                  "valid program must enter Ready state with an execution plan");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var workspace=new ClientProgramWorkspace();
            var snapshot=workspace.Load(Program());
            Check(snapshot.Status==ClientProgramLoadStatus.Ready &&
                  snapshot.ProgramId==Guid.Parse("81000000-0000-0000-0000-000000000001") &&
                  snapshot.StepCount==1 &&
                  snapshot.ExecutionPlanFingerprint is { Length:64 },
                  "valid program must enter Ready state with an execution plan");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var workspace=new ClientProgramWorkspace();
            var snapshot=workspace.Load(Program());
            Check(snapshot.Status==ClientProgramLoadStatus.Ready &&
                  snapshot.ProgramId==Guid.Parse("81000000-0000-0000-0000-000000000001") &&
                  snapshot.StepCount==1 &&
                  snapshot.ExecutionPlanFingerprint is { Length:64 },
                  "valid program must enter Ready state with an execution plan");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var workspace=new ClientProgramWorkspace();
            var snapshot=workspace.Load(Program());
            Check(snapshot.Status==ClientProgramLoadStatus.Ready &&
                  snapshot.ProgramId==Guid.Parse("81000000-0000-0000-0000-000000000001") &&
                  snapshot.StepCount==1 &&
                  snapshot.ExecutionPlanFingerprint is { Length:64 },
                  "valid program must enter Ready state with an execution plan");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var workspace=new ClientProgramWorkspace();
            var snapshot=workspace.Load(Program());
            Check(snapshot.Status==ClientProgramLoadStatus.Ready &&
                  snapshot.ProgramId==Guid.Parse("81000000-0000-0000-0000-000000000001") &&
                  snapshot.StepCount==1 &&
                  snapshot.ExecutionPlanFingerprint is { Length:64 },
                  "valid program must enter Ready state with an execution plan");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var workspace=new ClientProgramWorkspace();
            var snapshot=workspace.Load(Program());
            Check(snapshot.Status==ClientProgramLoadStatus.Ready &&
                  snapshot.ProgramId==Guid.Parse("81000000-0000-0000-0000-000000000001") &&
                  snapshot.StepCount==1 &&
                  snapshot.ExecutionPlanFingerprint is { Length:64 },
                  "valid program must enter Ready state with an execution plan");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var workspace=new ClientProgramWorkspace();
            var snapshot=workspace.Load(Program());
            Check(snapshot.Status==ClientProgramLoadStatus.Ready &&
                  snapshot.ProgramId==Guid.Parse("81000000-0000-0000-0000-000000000001") &&
                  snapshot.StepCount==1 &&
                  snapshot.ExecutionPlanFingerprint is { Length:64 },
                  "valid program must enter Ready state with an execution plan");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var workspace=new ClientProgramWorkspace();
            var snapshot=workspace.Load(Program());
            Check(snapshot.Status==ClientProgramLoadStatus.Ready &&
                  snapshot.ProgramId==Guid.Parse("81000000-0000-0000-0000-000000000001") &&
                  snapshot.StepCount==1 &&
                  snapshot.ExecutionPlanFingerprint is { Length:64 },
                  "valid program must enter Ready state with an execution plan");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var workspace=new ClientProgramWorkspace();
            var snapshot=workspace.Load(Program());
            Check(snapshot.Status==ClientProgramLoadStatus.Ready &&
                  snapshot.ProgramId==Guid.Parse("81000000-0000-0000-0000-000000000001") &&
                  snapshot.StepCount==1 &&
                  snapshot.ExecutionPlanFingerprint is { Length:64 },
                  "valid program must enter Ready state with an execution plan");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var workspace=new ClientProgramWorkspace();
            var snapshot=workspace.Load(Program());
            Check(snapshot.Status==ClientProgramLoadStatus.Ready &&
                  snapshot.ProgramId==Guid.Parse("81000000-0000-0000-0000-000000000001") &&
                  snapshot.StepCount==1 &&
                  snapshot.ExecutionPlanFingerprint is { Length:64 },
                  "valid program must enter Ready state with an execution plan");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
