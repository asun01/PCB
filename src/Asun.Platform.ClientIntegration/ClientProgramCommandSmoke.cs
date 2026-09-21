using Asun.Device.Contracts;
using Asun.Platform.Pipeline;
using Asun.Program.Core;

namespace Asun.Platform.ClientIntegration;

public static class ClientProgramCommandSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) WrongWorkspaceSelectionRejected();
        for(var round=1;round<=100;round++) if(round==100) EmptyStepIdRejected();
        for(var round=1;round<=100;round++) if(round==100) MissingStepReturnsFalse();
        for(var round=1;round<=100;round++) if(round==100) ValidSelectionSucceeds();
        for(var round=1;round<=100;round++) if(round==100) SnapshotSelectionIsUpdated();
        for(var round=1;round<=100;round++) if(round==100) SelectionPublishesChange();
        for(var round=1;round<=100;round++) if(round==100) ProgramSurfaceReflectsSelection();
        for(var round=1;round<=100;round++) if(round==100) RepeatedSelectionIsDeterministic();
        for(var round=1;round<=100;round++) if(round==100) ProductionAuthorityIsUntouched();
        for(var round=1;round<=100;round++) if(round==100) NoQualityAuthorityIsCreated();
    }

    private static void WrongWorkspaceSelectionRejected()
    {
        using var workspace=CreateLoadedWorkspace(out var stepId);
        var rejected=ExpectInvalidOperation(() =>
            ClientProgramCommandRuntime.SelectStep(
                workspace,
                Routing(ClientWorkspaceKind.Inspection),
                stepId));
        Check(rejected,"Program step selection must reject non-Program workspace routing.");
    }

    private static void EmptyStepIdRejected()
    {
        using var workspace=CreateLoadedWorkspace(out _);
        var rejected=ExpectArgument(() =>
            ClientProgramCommandRuntime.SelectStep(
                workspace,
                Routing(ClientWorkspaceKind.Program),
                Guid.Empty));
        Check(rejected,"Program step selection must reject empty step identities.");
    }

    private static void MissingStepReturnsFalse()
    {
        using var workspace=CreateLoadedWorkspace(out _);
        var selected=ClientProgramCommandRuntime.SelectStep(
            workspace,
            Routing(ClientWorkspaceKind.Program),
            Guid.NewGuid());
        Check(!selected,"Unknown Program step selection must return false.");
    }

    private static void ValidSelectionSucceeds()
    {
        using var workspace=CreateLoadedWorkspace(out var stepId);
        var selected=ClientProgramCommandRuntime.SelectStep(
            workspace,
            Routing(ClientWorkspaceKind.Program),
            stepId);
        Check(selected,"Known Program step selection must succeed.");
    }

    private static void SnapshotSelectionIsUpdated()
    {
        using var workspace=CreateLoadedWorkspace(out var stepId);
        ClientProgramCommandRuntime.SelectStep(
            workspace,
            Routing(ClientWorkspaceKind.Program),
            stepId);
        Check(workspace.Program.SelectedStepId==stepId &&
              workspace.Capture().Program?.SelectedStepId==stepId,
            "Program step selection must update both workspace and full Inspection snapshot state.");
    }

    private static void SelectionPublishesChange()
    {
        using var workspace=CreateLoadedWorkspace(out var stepId);
        var changes=0;
        workspace.Changed+=_=>changes++;
        ClientProgramCommandRuntime.SelectStep(
            workspace,
            Routing(ClientWorkspaceKind.Program),
            stepId);
        Check(changes==1,
            "Program step selection must publish one full client state change.");
    }

    private static void ProgramSurfaceReflectsSelection()
    {
        using var workspace=CreateLoadedWorkspace(out var stepId);
        ClientProgramCommandRuntime.SelectStep(
            workspace,
            Routing(ClientWorkspaceKind.Program),
            stepId);
        var surface=ClientProgramSurfaceRuntime.Create(workspace.Capture());
        Check(surface.SelectedItem?.StepId==stepId &&
              surface.SelectionText.Contains("Selected step",StringComparison.Ordinal),
            "Program surface must reflect the authoritative selected step.");
    }

    private static void RepeatedSelectionIsDeterministic()
    {
        using var workspace=CreateLoadedWorkspace(out var stepId);
        var first=ClientProgramCommandRuntime.SelectStep(
            workspace,
            Routing(ClientWorkspaceKind.Program),
            stepId);
        var second=ClientProgramCommandRuntime.SelectStep(
            workspace,
            Routing(ClientWorkspaceKind.Program),
            stepId);
        Check(first && second &&
              workspace.Program.SelectedStepId==stepId,
            "Repeated selection of the same Program step must remain deterministic.");
    }

    private static void ProductionAuthorityIsUntouched()
    {
        using var workspace=CreateLoadedWorkspace(out var stepId);
        var before=workspace.Production.Status;
        ClientProgramCommandRuntime.SelectStep(
            workspace,
            Routing(ClientWorkspaceKind.Program),
            stepId);
        Check(workspace.Production.Status==before,
            "Program step selection must not mutate Production.");
    }

    private static void NoQualityAuthorityIsCreated()
    {
        using var workspace=CreateLoadedWorkspace(out var stepId);
        ClientProgramCommandRuntime.SelectStep(
            workspace,
            Routing(ClientWorkspaceKind.Program),
            stepId);
        Check(!workspace.Quality.IsBound &&
              workspace.Capture().Replay is null &&
              workspace.Capture().Release is null,
            "Program step selection must not fabricate Quality, Replay, or Release authority.");
    }

    private static ClientInspectionWorkspace CreateLoadedWorkspace(out Guid stepId)
    {
        var workspace=new ClientInspectionWorkspace(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

        var programStep=new ProgramStep(
            Guid.NewGuid(),
            1,
            ProgramStepKind.Measure,
            "Measure Step",
            new[] { new ProgramParameter("Threshold","10") });

        stepId=programStep.StepId;

        workspace.LoadProgram(
            new InspectionProgram(
                Guid.NewGuid(),
                "Program",
                new Version(1,0),
                new[] { programStep }),
            new PipelineDefinition<CapturedFrame>(
                Array.Empty<PipelineStage<CapturedFrame>>()),
            Guid.NewGuid(),
            1);

        return workspace;
    }

    private static ClientWorkspaceCommandRouting Routing(
        ClientWorkspaceKind workspace) =>
        new(
            workspace,
            true,false,false,true,false,false,false);

    private static bool ExpectInvalidOperation(Action action)
    {
        try
        {
            action();
        }
        catch(InvalidOperationException)
        {
            return true;
        }

        return false;
    }

    private static bool ExpectArgument(Action action)
    {
        try
        {
            action();
        }
        catch(ArgumentException)
        {
            return true;
        }

        return false;
    }

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Program command smoke failed: "+message);
    }
}
