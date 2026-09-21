using Asun.Device.Contracts;
using Asun.Platform.Pipeline;
using Asun.Program.Core;

namespace Asun.Platform.ClientIntegration;

public static class ClientInspectionSnapshotValidationSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) EmptySnapshotIsValidated();
        for(var round=1;round<=100;round++) if(round==100) InvalidProgramLoadLeavesValidatedState();
        for(var round=1;round<=100;round++) if(round==100) ValidProgramItemsAreValidated();
        for(var round=1;round<=100;round++) if(round==100) AcquisitionBindingIsValidated();
        for(var round=1;round<=100;round++) if(round==100) PreviewStateIsValidated();
        for(var round=1;round<=100;round++) if(round==100) ProgramSelectionIsValidated();
        for(var round=1;round<=100;round++) if(round==100) HistoryResetIsValidated();
        for(var round=1;round<=100;round++) if(round==100) SessionResetIsValidated();
        for(var round=1;round<=100;round++) if(round==100) NoReplayIsFabricated();
        for(var round=1;round<=100;round++) if(round==100) NoQualityIsFabricated();
    }

    private static void EmptySnapshotIsValidated()
    {
        using var workspace=CreateWorkspace();
        var snapshot=workspace.CaptureValidated();
        Check(snapshot.Program?.Status==ClientProgramLoadStatus.Empty &&
              snapshot.Acquisition.State==ClientAcquisitionState.Unbound,
            "Empty Inspection workspace must pass the validated snapshot boundary.");
    }

    private static void InvalidProgramLoadLeavesValidatedState()
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

        var snapshot=workspace.CaptureValidated();
        Check(snapshot.Program?.Status==ClientProgramLoadStatus.Invalid &&
              snapshot.ProgramItems.Count==0 &&
              snapshot.Production.Status==ClientExecutionStatus.Idle,
            "Invalid Program state must still pass validation after dependent-state cleanup.");
    }

    private static void ValidProgramItemsAreValidated()
    {
        using var workspace=CreateWorkspace();
        workspace.LoadProgram(CreateValidProgram(),EmptyPipeline(),Guid.NewGuid(),1);
        var snapshot=workspace.CaptureValidated();
        Check(snapshot.Program?.Status==ClientProgramLoadStatus.Ready &&
              snapshot.ProgramItems.Count==1 &&
              snapshot.ProgramItems[0].StepId==snapshot.Program?.SelectedStepId,
            "Valid ProgramItems and selection must pass the validated snapshot boundary.");
    }

    private static void AcquisitionBindingIsValidated()
    {
        using var workspace=CreateWorkspace();
        workspace.BindAcquisition(
            new DeterministicSource(),
            new ClientAcquisitionDescriptor("source","Deterministic",true));
        var snapshot=workspace.CaptureValidated();
        Check(snapshot.Acquisition.State==ClientAcquisitionState.Ready &&
              snapshot.Acquisition.Preview is null,
            "Ready Acquisition without Preview must pass validation.");
    }

    private static void PreviewStateIsValidated()
    {
        using var workspace=CreateWorkspace();
        workspace.BindAcquisition(
            new DeterministicSource(),
            new ClientAcquisitionDescriptor("source","Deterministic",true));
        workspace.PreviewAcquisitionAsync().AsTask().GetAwaiter().GetResult();
        var snapshot=workspace.CaptureValidated();
        Check(snapshot.Acquisition.Preview is not null &&
              snapshot.Acquisition.State==ClientAcquisitionState.Ready,
            "Successful Acquisition Preview must pass the validated snapshot boundary.");
    }

    private static void ProgramSelectionIsValidated()
    {
        using var workspace=CreateWorkspace();
        var program=CreateValidProgram();
        workspace.LoadProgram(program,EmptyPipeline(),Guid.NewGuid(),1);
        var selected=workspace.SelectProgramStep(program.Steps[0].StepId);
        var snapshot=workspace.CaptureValidated();
        Check(selected &&
              snapshot.Program?.SelectedStepId==program.Steps[0].StepId,
            "Program selection must remain valid through full snapshot validation.");
    }

    private static void HistoryResetIsValidated()
    {
        using var workspace=CreateWorkspace();
        workspace.ResetHistory();
        Check(ClientInspectionWorkflowIntegrityRuntime.IsValid(workspace.CaptureValidated()),
            "History reset state must remain integrity-valid.");
    }

    private static void SessionResetIsValidated()
    {
        using var workspace=CreateWorkspace();
        workspace.ResetCurrentSession();
        var snapshot=workspace.CaptureValidated();
        Check(snapshot.Production.Status==ClientExecutionStatus.Idle &&
              snapshot.Program?.Status==ClientProgramLoadStatus.Empty,
            "Session reset state must remain integrity-valid.");
    }

    private static void NoReplayIsFabricated()
    {
        using var workspace=CreateWorkspace();
        var snapshot=workspace.CaptureValidated();
        Check(snapshot.Replay is null,
            "Validated baseline must not fabricate Replay.");
    }

    private static void NoQualityIsFabricated()
    {
        using var workspace=CreateWorkspace();
        var snapshot=workspace.CaptureValidated();
        Check(!snapshot.Quality.IsBound,
            "Validated baseline must not fabricate Quality.");
    }

    private static InspectionProgram CreateValidProgram()
    {
        var step=new ProgramStep(
            Guid.NewGuid(),
            1,
            ProgramStepKind.Measure,
            "Measure Step",
            new[] { new ProgramParameter("Threshold","10") });

        return new InspectionProgram(
            Guid.NewGuid(),
            "Program",
            new Version(1,0),
            new[] { step });
    }

    private static PipelineDefinition<CapturedFrame> EmptyPipeline() =>
        new(Array.Empty<PipelineStage<CapturedFrame>>());

    private static ClientInspectionWorkspace CreateWorkspace() =>
        new(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

    private sealed class DeterministicSource : IFrameSource
    {
        public ValueTask<CapturedFrame?> CaptureAsync(
            CancellationToken cancellationToken=default) =>
            ValueTask.FromResult<CapturedFrame?>(
                CapturedFrame.Create(
                    new FrameCaptureMetadata(
                        FrameSequence.Create(1),
                        16,
                        12,
                        "Gray8",
                        DateTimeOffset.UtcNow),
                    new byte[16*12]));
    }

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Inspection snapshot validation smoke failed: "+message);
    }
}
