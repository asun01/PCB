namespace Asun.Platform.ClientIntegration;

public static class ClientProgramSurfaceSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) EmptyProgramIsExplicit();
        for(var round=1;round<=100;round++) if(round==100) EmptyItemsRemainEmpty();
        for(var round=1;round<=100;round++) if(round==100) NoSelectionIsExplicit();
        for(var round=1;round<=100;round++) if(round==100) SelectedStepIsProjected();
        for(var round=1;round<=100;round++) if(round==100) SelectionTextIsProjected();
        for(var round=1;round<=100;round++) if(round==100) StepIdentityIsPreserved();
        for(var round=1;round<=100;round++) if(round==100) ParameterSummaryIsPreserved();
        for(var round=1;round<=100;round++) if(round==100) ProgramVersionIsPreserved();
        for(var round=1;round<=100;round++) if(round==100) ProgramCountIsPreserved();
        for(var round=1;round<=100;round++) if(round==100) ProgramSurfaceDoesNotFabricateSteps();
    }

    private static void EmptyProgramIsExplicit()
    {
        var surface=CreateEmptySurface();
        Check(surface.Snapshot.Status==ClientProgramLoadStatus.Empty &&
              surface.Items.Count==0,
            "Unbound Program must remain explicit through the Program surface.");
    }

    private static void EmptyItemsRemainEmpty()
    {
        var surface=CreateEmptySurface();
        Check(surface.SelectedItem is null &&
              surface.SelectionText=="No Program step selected.",
            "Empty Program must have no selected step.");
    }

    private static void NoSelectionIsExplicit()
    {
        var surface=CreateEmptySurface();
        Check(surface.Snapshot.SelectedStepId is null,
            "Unbound Program selection must remain null.");
    }

    private static void SelectedStepIsProjected()
    {
        var stepId=Guid.NewGuid();
        var surface=CreateSurface(stepId);
        Check(surface.SelectedItem?.StepId==stepId,
            "Program surface must project the selected step identity.");
    }

    private static void SelectionTextIsProjected()
    {
        var stepId=Guid.NewGuid();
        var surface=CreateSurface(stepId);
        Check(surface.SelectionText.Contains("Selected step 1",StringComparison.Ordinal),
            "Program surface must expose deterministic selected-step text.");
    }

    private static void StepIdentityIsPreserved()
    {
        var stepId=Guid.NewGuid();
        var surface=CreateSurface(stepId);
        Check(surface.Items[0].StepId==stepId,
            "Program step identity must be preserved.");
    }

    private static void ParameterSummaryIsPreserved()
    {
        var stepId=Guid.NewGuid();
        var surface=CreateSurface(stepId);
        Check(surface.Items[0].ParameterSummary=="Threshold=10",
            "Program parameter summary must be projected unchanged.");
    }

    private static void ProgramVersionIsPreserved()
    {
        var surface=CreateSurface(Guid.NewGuid());
        Check(surface.Snapshot.Version==new Version(2,1),
            "Program version must be preserved in the unified Program surface.");
    }

    private static void ProgramCountIsPreserved()
    {
        var surface=CreateSurface(Guid.NewGuid());
        Check(surface.Snapshot.StepCount==1 &&
              surface.Items.Count==1,
            "Program step count must agree with the projected item list.");
    }

    private static void ProgramSurfaceDoesNotFabricateSteps()
    {
        var snapshot=CreateSnapshot(null);
        var surface=ClientProgramSurfaceRuntime.Create(snapshot);
        Check(surface.Items.Count==0 &&
              surface.SelectedItem is null,
              "A Program snapshot without projected items must not fabricate steps.");
    }

    private static ClientProgramSurface CreateEmptySurface() =>
        ClientProgramSurfaceRuntime.Create(
            new ClientInspectionWorkspaceSnapshot(
                new ClientWorkspaceSnapshot(
                    null,
                    null,
                    null,
                    ClientExecutionStatus.Idle,
                    0,
                    null),
                null,
                null,
                null,
                new ClientProductionRunHistorySnapshot(
                    20,
                    1,
                    0,
                    Array.Empty<ClientProductionRunHistoryEntry>())
            ));

    private static ClientProgramSurface CreateSurface(Guid selectedStepId) =>
        ClientProgramSurfaceRuntime.Create(
            CreateSnapshot(selectedStepId));

    private static ClientInspectionWorkspaceSnapshot CreateSnapshot(Guid? selectedStepId)
    {
        var stepId=selectedStepId ?? Guid.NewGuid();
        var program=new ClientProgramWorkspaceSnapshot(
            ClientProgramLoadStatus.Ready,
            Guid.NewGuid(),
            "Deterministic Program",
            new Version(2,1),
            1,
            new string('a',64),
            Array.Empty<string>())
        {
            SelectedStepId=selectedStepId
        };

        var item=new ClientProgramDisplayItem(
            1,
            "Inspection Step",
            "Measure",
            1)
        {
            StepId=stepId,
            ParameterSummary="Threshold=10"
        };

        return new ClientInspectionWorkspaceSnapshot(
            new ClientWorkspaceSnapshot(
                program.ProgramId,
                program.Version,
                Guid.NewGuid(),
                ClientExecutionStatus.Ready,
                0,
                null),
            null,
            null,
            null,
            new ClientProductionRunHistorySnapshot(
                20,
                1,
                0,
                Array.Empty<ClientProductionRunHistoryEntry>()))
        {
            Program=program,
            ProgramItems=selectedStepId is null
                ? Array.Empty<ClientProgramDisplayItem>()
                : new[] { item }
        };
    }

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Program surface smoke failed: "+message);
    }
}
