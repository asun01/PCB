namespace Asun.Platform.SimulationIntegration;

using Asun.Platform.ClientIntegration;

public static class ClientRecoveryReexecutionHistoryProjectionSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) RecoveryReturnsToExecutableSession();
        for(var round=1;round<=100;round++) if(round==100) RecoveryClearsCurrentAuthority();
        for(var round=1;round<=100;round++) if(round==100) ReexecutionCreatesFreshReportEvidence();
        for(var round=1;round<=100;round++) if(round==100) ReexecutionCreatesNewHistoryEntry();
        for(var round=1;round<=100;round++) if(round==100) NewHistorySelectionIsAuthoritative();
        for(var round=1;round<=100;round++) if(round==100) OldHistoryRemainsImmutable();
        for(var round=1;round<=100;round++) if(round==100) CurrentWorkflowBindsToNewReplay();
        for(var round=1;round<=100;round++) if(round==100) ResultsProjectionUsesCurrentAuthority();
        for(var round=1;round<=100;round++) if(round==100) InvalidHistorySelectionDoesNotChangeSelection();
        for(var round=1;round<=100;round++) if(round==100) UnifiedProjectionCarriesCurrentWorkflow();
    }

    private static void RecoveryReturnsToExecutableSession()
    {
        using var workspace=CreateFinalizedWorkspace();
        var session=workspace.Production.ActiveSessionId;
        workspace.ResetCurrentSession();
        Check(workspace.Production.Status==ClientExecutionStatus.Ready &&
              workspace.Production.ActiveSessionId==session,
            "Recovery must reopen the retained Production session as Ready.");
    }

    private static void RecoveryClearsCurrentAuthority()
    {
        using var workspace=CreateFinalizedWorkspace();
        workspace.ResetCurrentSession();
        var snapshot=workspace.Capture();
        Check(!snapshot.Quality.IsBound &&
              snapshot.Replay is null &&
              snapshot.Release is null &&
              snapshot.SelectedHistoryOrdinal is null,
            "Recovery must clear current Quality, Replay, Release, and selection authority.");
    }

    private static void ReexecutionCreatesFreshReportEvidence()
    {
        using var workspace=CreateFinalizedWorkspace();
        var oldFingerprint=workspace.LastProductionReport!.Fingerprint;
        workspace.ResetCurrentSession();
        Execute(workspace);
        Check(workspace.LastProductionReport is not null &&
              workspace.LastProductionReport.Fingerprint!=oldFingerprint,
            "A recovered execution must publish fresh Production report evidence.");
    }

    private static void ReexecutionCreatesNewHistoryEntry()
    {
        using var workspace=CreateFinalizedWorkspace();
        var before=workspace.Capture().History.Entries.Count;
        workspace.ResetCurrentSession();
        Execute(workspace);
        workspace.EvaluateQualityProvider("simulation-quality");
        Check(workspace.Capture().History.Entries.Count==before+1,
            "Recovered execution must append a distinct finalized History entry.");
    }

    private static void NewHistorySelectionIsAuthoritative()
    {
        using var workspace=CreateFinalizedWorkspace();
        workspace.ResetCurrentSession();
        Execute(workspace);
        workspace.EvaluateQualityProvider("simulation-quality");
        var snapshot=workspace.Capture();
        Check(snapshot.SelectedHistoryOrdinal==snapshot.History.Entries.Max(entry=>entry.Ordinal),
            "Finalization must select the newly appended authoritative History entry.");
    }

    private static void OldHistoryRemainsImmutable()
    {
        using var workspace=CreateFinalizedWorkspace();
        var old=workspace.Capture().History.Entries.Single();
        workspace.ResetCurrentSession();
        Execute(workspace);
        workspace.EvaluateQualityProvider("simulation-quality");
        var current=workspace.Capture().History.Entries.Single(entry=>entry.Ordinal==old.Ordinal);
        Check(current.ReplayFingerprint==old.ReplayFingerprint &&
              current.QualityFingerprint==old.QualityFingerprint,
            "Recovery and re-execution must not mutate prior History authority.");
    }

    private static void CurrentWorkflowBindsToNewReplay()
    {
        using var workspace=CreateFinalizedWorkspace();
        var old=workspace.Capture().Replay!.ReplayFingerprint;
        workspace.ResetCurrentSession();
        Execute(workspace);
        workspace.EvaluateQualityProvider("simulation-quality");
        var snapshot=workspace.Capture();
        Check(snapshot.Replay is not null &&
              snapshot.Replay.ReplayFingerprint!=old &&
              snapshot.History.Entries.Any(entry=>entry.ReplayFingerprint==snapshot.Replay.ReplayFingerprint),
            "Current Workflow must bind to the new Replay identity.");
    }

    private static void ResultsProjectionUsesCurrentAuthority()
    {
        using var workspace=CreateFinalizedWorkspace();
        workspace.ResetCurrentSession();
        Execute(workspace);
        workspace.EvaluateQualityProvider("simulation-quality");
        var display=ClientResultsPresentationRuntime.CreateCurrent(workspace.Capture());
        Check(display.Status=="Completed" &&
              display.QualityText.StartsWith("Quality ") &&
              display.ReplayText.StartsWith("Replay ") &&
              display.ReleaseReady,
            "Results projection must expose the current Quality, Replay, and Release authority.");
    }

    private static void InvalidHistorySelectionDoesNotChangeSelection()
    {
        using var workspace=CreateFinalizedWorkspace();
        var selected=workspace.Capture().SelectedHistoryOrdinal;
        Check(!workspace.SelectHistory(long.MaxValue) &&
              workspace.Capture().SelectedHistoryOrdinal==selected,
            "Selecting an unavailable History ordinal must not change the current selection.");
    }

    private static void UnifiedProjectionCarriesCurrentWorkflow()
    {
        using var workspace=CreateFinalizedWorkspace();
        workspace.ResetCurrentSession();
        Execute(workspace);
        workspace.EvaluateQualityProvider("simulation-quality");
        var snapshot=workspace.Capture();
        var selection=ClientWorkspaceSelectionRuntime.CreateDefault();
        var routing=ClientWorkspaceCommandRoutingRuntime.Create(snapshot);
        var projection=ClientWorkspaceContentSurfaceRuntime.CreateValidated(
            selection,
            routing,
            snapshot);
        Check(projection.Workflow.Step==ClientInspectionWorkflowRuntime.Evaluate(snapshot).Step &&
              projection.Results.Current.QualityText==ClientResultsPresentationRuntime.CreateCurrent(snapshot).QualityText,
            "Unified Projection must carry the same current Workflow and Results authority.");
    }

    private static ClientInspectionWorkspace CreateFinalizedWorkspace()
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
            new ClientAcquisitionDescriptor("simulation","Deterministic Simulation Source",true));
        Execute(workspace);
        workspace.EvaluateQualityProvider("simulation-quality");
        return workspace;
    }

    private static void Execute(ClientInspectionWorkspace workspace) =>
        workspace.ExecuteAsync(
            ClientSimulationSessionFactory.CreateReleaseManifest())
        .AsTask().GetAwaiter().GetResult();

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Recovery re-execution History projection smoke failed: "+message);
    }
}
