using Asun.Device.Contracts;
using Asun.Platform.Pipeline;
using Asun.Program.Core;

namespace Asun.Platform.ClientIntegration;

public static class ClientProgramReloadBoundarySmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) ReloadUnbindsAcquisition();
        for(var round=1;round<=100;round++) if(round==100) ReloadClearsQuality();
        for(var round=1;round<=100;round++) if(round==100) ReloadClearsReplay();
        for(var round=1;round<=100;round++) if(round==100) ReloadClearsRelease();
        for(var round=1;round<=100;round++) if(round==100) ReloadClearsSelectedHistory();
        for(var round=1;round<=100;round++) if(round==100) ReloadClearsLastProductionReport();
        for(var round=1;round<=100;round++) if(round==100) ReloadResetsRoi();
        for(var round=1;round<=100;round++) if(round==100) ReloadCreatesFreshProductionSession();
        for(var round=1;round<=100;round++) if(round==100) ReloadPublishesNewProgramIdentity();
        for(var round=1;round<=100;round++) if(round==100) ReloadLeavesClientWorkflowValid();
    }

    private static void ReloadUnbindsAcquisition()
    {
        using var workspace=CreateReloadedWorkspace();
        Check(workspace.Acquisition.State==ClientAcquisitionState.Unbound &&
              !workspace.Acquisition.CanCapture,
            "Loading a new Program must invalidate the previous Acquisition binding.");
    }

    private static void ReloadClearsQuality()
    {
        using var workspace=CreateReloadedWorkspace();
        Check(!workspace.Quality.IsBound &&
              workspace.Quality.FindingCount==0,
            "Loading a new Program must clear the previous Quality projection.");
    }

    private static void ReloadClearsReplay()
    {
        using var workspace=CreateReloadedWorkspace();
        Check(workspace.Capture().Replay is null,
            "Loading a new Program must clear the previous Replay projection.");
    }

    private static void ReloadClearsRelease()
    {
        using var workspace=CreateReloadedWorkspace();
        Check(workspace.Capture().Release is null,
            "Loading a new Program must clear the previous Release projection.");
    }

    private static void ReloadClearsSelectedHistory()
    {
        using var workspace=CreateReloadedWorkspace();
        Check(workspace.SelectedHistoryOrdinal is null,
            "Loading a new Program must clear the previous selected history entry.");
    }

    private static void ReloadClearsLastProductionReport()
    {
        using var workspace=CreateReloadedWorkspace();
        Check(workspace.LastProductionReport is null,
            "Loading a new Program must clear the previous Production report reference.");
    }

    private static void ReloadResetsRoi()
    {
        using var workspace=CreateReloadedWorkspace();
        Check(!workspace.CanUndoRoi &&
              !workspace.CanRedoRoi,
            "Loading a new Program must reset the previous ROI editing history.");
    }

    private static void ReloadCreatesFreshProductionSession()
    {
        using var workspace=CreateReloadedWorkspace();
        Check(workspace.Production.Status==ClientExecutionStatus.Ready &&
              workspace.Production.ActiveSessionId is Guid,
            "Loading a new Program must establish a fresh Ready Production session.");
    }

    private static void ReloadPublishesNewProgramIdentity()
    {
        using var workspace=CreateReloadedWorkspace();
        Check(workspace.Program.ProgramId==SecondProgramId &&
              workspace.Program.Version==new Version(2,0),
            "Loading a new Program must make the new Program identity authoritative.");
    }

    private static void ReloadLeavesClientWorkflowValid()
    {
        using var workspace=CreateReloadedWorkspace();
        Check(ClientInspectionWorkflowIntegrityRuntime.IsValid(workspace.Capture()),
            "The client workflow integrity gate must remain valid after Program reload.");
    }

    private static readonly Guid SecondProgramId=
        Guid.Parse("20000000-0000-0000-0000-000000000002");

    private static ClientInspectionWorkspace CreateReloadedWorkspace()
    {
        var workspace=new ClientInspectionWorkspace(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

        workspace.LoadProgram(
            CreateProgram(
                Guid.Parse("10000000-0000-0000-0000-000000000001"),
                new Version(1,0),
                "Program One"),
            new PipelineDefinition<CapturedFrame>(Array.Empty<PipelineStage<CapturedFrame>>()),
            Guid.Parse("30000000-0000-0000-0000-000000000001"),
            1);

        workspace.BindAcquisition(
            new NullFrameSource(),
            new ClientAcquisitionDescriptor(
                "old-source",
                "Old source",
                true));

        workspace.LoadProgram(
            CreateProgram(
                SecondProgramId,
                new Version(2,0),
                "Program Two"),
            new PipelineDefinition<CapturedFrame>(Array.Empty<PipelineStage<CapturedFrame>>()),
            Guid.Parse("30000000-0000-0000-0000-000000000002"),
            1);

        return workspace;
    }

    private static InspectionProgram CreateProgram(
        Guid programId,
        Version version,
        string name)=>
        new(
            programId,
            name,
            version,
            new[]
            {
                new ProgramStep(
                    Guid.NewGuid(),
                    1,
                    ProgramStepKind.Inspect,
                    "Inspect",
                    Array.Empty<ProgramParameter>())
            });

    private sealed class NullFrameSource : IFrameSource
    {
        public ValueTask<CapturedFrame?> CaptureAsync(
            CancellationToken cancellationToken=default)=>
            ValueTask.FromResult<CapturedFrame?>(null);
    }

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Program reload boundary smoke failed: "+message);
    }
}
