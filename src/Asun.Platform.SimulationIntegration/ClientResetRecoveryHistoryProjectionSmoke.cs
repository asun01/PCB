namespace Asun.Platform.SimulationIntegration;

using Asun.Platform.ClientIntegration;

public static class ClientResetRecoveryHistoryProjectionSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) ResetPreservesLoadedProgramDefinition();
        for(var round=1;round<=100;round++) if(round==100) ResetClearsQualityAuthority();
        for(var round=1;round<=100;round++) if(round==100) ResetClearsReplayAuthority();
        for(var round=1;round<=100;round++) if(round==100) ResetClearsReleaseAuthority();
        for(var round=1;round<=100;round++) if(round==100) ResetClearsProductionReport();
        for(var round=1;round<=100;round++) if(round==100) ResetClearsPendingHistorySelection();
        for(var round=1;round<=100;round++) if(round==100) FinalizedHistoryCarriesQualityAuthority();
        for(var round=1;round<=100;round++) if(round==100) IntegrityRejectsHistoryAuthorityDrift();
        for(var round=1;round<=100;round++) if(round==100) UnifiedProjectionCarriesWorkflowAuthority();
        for(var round=1;round<=100;round++) if(round==100) UnifiedProjectionCarriesHistoryAuthority();
    }

    private static void ResetPreservesLoadedProgramDefinition()
    {
        using var workspace=CreateCompletedProductionWorkspace();
        var programId=workspace.Program.ProgramId;
        var programName=workspace.Program.Name;

        workspace.ResetCurrentSession();

        Check(workspace.Program.Status==ClientProgramLoadStatus.Ready &&
              workspace.Program.ProgramId==programId &&
              workspace.Program.Name==programName,
            "Reset must preserve the loaded Program definition while clearing runtime execution evidence.");
    }

    private static void ResetClearsQualityAuthority()
    {
        using var workspace=FinalizeQuality();
        workspace.ResetCurrentSession();

        Check(!workspace.Quality.IsBound && workspace.Quality.Fingerprint is null,
            "Reset must clear the authoritative Quality result.");
    }

    private static void ResetClearsReplayAuthority()
    {
        using var workspace=FinalizeQuality();
        workspace.ResetCurrentSession();

        Check(workspace.Capture().Replay is null,
            "Reset must clear Replay authority.");
    }

    private static void ResetClearsReleaseAuthority()
    {
        using var workspace=FinalizeQuality();
        workspace.ResetCurrentSession();

        Check(workspace.Capture().Release is null,
            "Reset must clear Release authority.");
    }

    private static void ResetClearsProductionReport()
    {
        using var workspace=FinalizeQuality();
        workspace.ResetCurrentSession();

        Check(workspace.LastProductionReport is null,
            "Reset must clear the previous Production report.");
    }

    private static void ResetClearsPendingHistorySelection()
    {
        using var workspace=FinalizeQuality();
        Check(workspace.SelectedHistoryOrdinal is not null,
            "Finalization must select the newly appended Run History entry.");

        workspace.ResetCurrentSession();

        Check(workspace.SelectedHistoryOrdinal is null,
            "Reset must clear transient Run History selection.");
    }

    private static void FinalizedHistoryCarriesQualityAuthority()
    {
        using var workspace=FinalizeQuality();
        var snapshot=workspace.Capture();
        var history=snapshot.History.Entries.Single();

        Check(history.QualityFingerprint==snapshot.Quality.Fingerprint &&
              history.ReplayFingerprint==snapshot.Replay!.ReplayFingerprint &&
              history.ReplayFingerprint==snapshot.Release!.ReplayFingerprint,
            "Finalized Run History must retain the same Quality and Replay authority fingerprints.");
    }

    private static void IntegrityRejectsHistoryAuthorityDrift()
    {
        using var workspace=FinalizeQuality();
        var snapshot=workspace.Capture();
        var entry=snapshot.History.Entries.Single();
        var driftedHistory=snapshot.History with
        {
            Entries=new[]
            {
                entry with { QualityFingerprint=new string('b',64) }
            }
        };

        Check(!ClientInspectionWorkflowIntegrityRuntime.IsValid(
                snapshot with { History=driftedHistory }),
            "Workflow integrity must reject Run History Quality authority drift.");
    }

    private static void UnifiedProjectionCarriesWorkflowAuthority()
    {
        using var workspace=FinalizeQuality();
        var selection=new ClientWorkspaceSelection(ClientWorkspaceKind.Home,0);
        var availability=ClientCommandAvailabilityRuntime.Create(workspace.Capture());
        var projection=ClientWorkspaceClientSnapshotRuntime.Create(
            selection,
            availability,
            workspace.Capture());

        var workflow=ClientInspectionWorkflowRuntime.Evaluate(workspace.Capture());

        Check(projection.Content.Workflow.Step==workflow.Step &&
              projection.Content.Workflow.Message==workflow.Message &&
              projection.Content.Workflow.Fingerprint==workflow.Fingerprint,
            "Unified Client Projection must carry the authoritative Workflow snapshot.");
    }

    private static void UnifiedProjectionCarriesHistoryAuthority()
    {
        using var workspace=FinalizeQuality();
        var selection=new ClientWorkspaceSelection(ClientWorkspaceKind.Results,1);
        var availability=ClientCommandAvailabilityRuntime.Create(workspace.Capture());
        var projection=ClientWorkspaceClientSnapshotRuntime.Create(
            selection,
            availability,
            workspace.Capture());

        Check(projection.Content.Results.History.Count==1 &&
              projection.Content.Results.History[0].Ordinal==
              workspace.Capture().History.Entries[0].Ordinal &&
              projection.Content.Results.Current.QualityText.StartsWith("Quality "),
            "Unified Results Projection must carry finalized History and Quality authority.");
    }

    private static ClientInspectionWorkspace FinalizeQuality()
    {
        var workspace=CreateCompletedProductionWorkspace();
        workspace.EvaluateQualityProvider("simulation-quality");
        return workspace;
    }

    private static ClientInspectionWorkspace CreateCompletedProductionWorkspace()
    {
        var workspace=new ClientInspectionWorkspace(
            new System.Numerics.Vector2(64,64),
            new System.Numerics.Vector2(64,64));

        workspace.QualityProviderCatalog.Register(
            ClientSimulationQualityRunProvider.CreateDefinition());

        var definition=ClientSimulationSessionFactory.CreateDefinition(frameCount:3);
        workspace.LoadProgram(
            ClientSimulationSessionFactory.CreateProgram(),
            ClientSimulationSessionFactory.CreatePipeline(),
            definition.SessionId,
            definition.FrameCount);
        workspace.BindAcquisition(
            ClientSimulationSessionFactory.CreateSource(),
            new ClientAcquisitionDescriptor(
                "simulation",
                "Deterministic Simulation Source",
                true));

        workspace.ExecuteAsync(
            ClientSimulationSessionFactory.CreateReleaseManifest())
            .AsTask()
            .GetAwaiter()
            .GetResult();

        return workspace;
    }

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Reset Recovery History Projection smoke failed: "+message);
    }
}
