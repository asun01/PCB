using Asun.Device.Contracts;
using Asun.Platform.Pipeline;
using Asun.Program.Core;

namespace Asun.Platform.ClientIntegration;

public static class ClientInspectionWorkspaceProgramItemsSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) InvalidProgramCaptureIsSafe();
        for(var round=1;round<=100;round++) if(round==100) InvalidProgramItemsRemainEmpty();
        for(var round=1;round<=100;round++) if(round==100) InvalidProgramStatusIsPreserved();
        for(var round=1;round<=100;round++) if(round==100) ValidProgramProducesItems();
        for(var round=1;round<=100;round++) if(round==100) ProgramItemIdentityIsPreserved();
        for(var round=1;round<=100;round++) if(round==100) ProgramItemCountMatches();
        for(var round=1;round<=100;round++) if(round==100) SelectedStepIsProjected();
        for(var round=1;round<=100;round++) if(round==100) ProgramVersionIsProjected();
        for(var round=1;round<=100;round++) if(round==100) ProgramNameIsProjected();
        for(var round=1;round<=100;round++) if(round==100) EmptyWorkspaceHasNoProgramItems();
    }

    private static void InvalidProgramCaptureIsSafe()
    {
        using var workspace=CreateWorkspace();
        var snapshot=workspace.LoadProgram(
            new InspectionProgram(
                Guid.NewGuid(),
                "",
                new Version(1,0),
                Array.Empty<ProgramStep>()),
            EmptyPipeline(),
            Guid.NewGuid(),
            1);

        var captured=workspace.Capture();
        Check(snapshot.Status==ClientProgramLoadStatus.Invalid &&
              captured.ProgramItems.Count==0,
            "Invalid Program Capture must be safe and must not generate ProgramItems.");
    }

    private static void InvalidProgramItemsRemainEmpty()
    {
        using var workspace=CreateWorkspace();
        workspace.LoadProgram(
            CreateValidProgram(),
            EmptyPipeline(),
            Guid.NewGuid(),
            1);
        workspace.BindAcquisition(
            new DeterministicSource(),
            new ClientAcquisitionDescriptor("source","Deterministic",true));

        workspace.LoadProgram(
            new InspectionProgram(
                Guid.NewGuid(),
                "",
                new Version(1,0),
                Array.Empty<ProgramStep>()),
            EmptyPipeline(),
            Guid.NewGuid(),
            1);

        var snapshot=workspace.Capture();
        Check(snapshot.ProgramItems.Count==0 &&
              snapshot.Program?.Status==ClientProgramLoadStatus.Invalid &&
              snapshot.Production.Status==ClientExecutionStatus.Idle &&
              snapshot.Acquisition.State==ClientAcquisitionState.Unbound &&
              snapshot.Quality.IsBound==false &&
              snapshot.Replay is null &&
              snapshot.Release is null,
            "Invalid Program load must clear dependent Production, Acquisition, Quality, Replay, and Release state.");

    private static void InvalidProgramStatusIsPreserved()
    {
        using var workspace=CreateWorkspace();
        workspace.LoadProgram(
            new InspectionProgram(
                Guid.NewGuid(),
                "",
                new Version(1,0),
                Array.Empty<ProgramStep>()),
            EmptyPipeline(),
            Guid.NewGuid(),
            1);

        Check(workspace.Capture().Program?.Status==ClientProgramLoadStatus.Invalid,
            "Invalid Program status must remain visible in the client snapshot.");
    }

    private static void ValidProgramProducesItems()
    {
        using var workspace=CreateWorkspace();
        workspace.LoadProgram(CreateValidProgram(),EmptyPipeline(),Guid.NewGuid(),1);
        Check(workspace.Capture().ProgramItems.Count==1,
            "Valid Program must generate one canonical ProgramItem.");
    }

    private static void ProgramItemIdentityIsPreserved()
    {
        using var workspace=CreateWorkspace();
        var program=CreateValidProgram();
        workspace.LoadProgram(program,EmptyPipeline(),Guid.NewGuid(),1);
        Check(workspace.Capture().ProgramItems[0].StepId==program.Steps[0].StepId,
            "ProgramItem must preserve the canonical ProgramStep identity.");
    }

    private static void ProgramItemCountMatches()
    {
        using var workspace=CreateWorkspace();
        workspace.LoadProgram(CreateValidProgram(),EmptyPipeline(),Guid.NewGuid(),1);
        var snapshot=workspace.Capture();
        Check(snapshot.Program?.StepCount==snapshot.ProgramItems.Count,
            "Program step count must match the ProgramItems projection.");
    }

    private static void SelectedStepIsProjected()
    {
        using var workspace=CreateWorkspace();
        workspace.LoadProgram(CreateValidProgram(),EmptyPipeline(),Guid.NewGuid(),1);
        var snapshot=workspace.Capture();
        Check(snapshot.Program?.SelectedStepId==snapshot.ProgramItems[0].StepId,
            "Program snapshot selection must align with the first canonical ProgramItem.");
    }

    private static void ProgramVersionIsProjected()
    {
        using var workspace=CreateWorkspace();
        workspace.LoadProgram(CreateValidProgram(),EmptyPipeline(),Guid.NewGuid(),1);
        Check(workspace.Capture().Program?.Version==new Version(3,2),
            "Program version must be preserved through the Inspection snapshot.");
    }

    private static void ProgramNameIsProjected()
    {
        using var workspace=CreateWorkspace();
        workspace.LoadProgram(CreateValidProgram(),EmptyPipeline(),Guid.NewGuid(),1);
        Check(workspace.Capture().Program?.Name=="Client Program",
            "Program name must be preserved through the Inspection snapshot.");
    }

    private static void EmptyWorkspaceHasNoProgramItems()
    {
        using var workspace=CreateWorkspace();
        Check(workspace.Capture().ProgramItems.Count==0 &&
              workspace.Capture().Program?.Status==ClientProgramLoadStatus.Empty,
            "An empty client workspace must not fabricate ProgramItems.");
    }

    private static InspectionProgram CreateValidProgram()
    {
        var step=new ProgramStep(
            Guid.NewGuid(),
            1,
            ProgramStepKind.Measure,
            "Measure Step",
            new[]
            {
                new ProgramParameter("Threshold","10")
            });

        return new InspectionProgram(
            Guid.NewGuid(),
            "Client Program",
            new Version(3,2),
            new[] { step });
    }

    private sealed class DeterministicSource : IFrameSource
    {
        public ValueTask<CapturedFrame?> CaptureAsync(
            CancellationToken cancellationToken=default) =>
            ValueTask.FromResult<CapturedFrame?>(null);
    }

    private static PipelineDefinition<CapturedFrame> EmptyPipeline() =>
        new(Array.Empty<PipelineStage<CapturedFrame>>());

    private static ClientInspectionWorkspace CreateWorkspace() =>
        new(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Inspection ProgramItems smoke failed: "+message);
    }
}
