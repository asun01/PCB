namespace Asun.Platform.SimulationIntegration;

using Asun.Platform.ClientIntegration;

public static class ClientResultsSelectionRecoverySmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) SelectionProjectionBindsSelectedHistory();
        for(var round=1;round<=100;round++) if(round==100) SelectionCarriesQualityAuthority();
        for(var round=1;round<=100;round++) if(round==100) SelectionCarriesReplayAuthority();
        for(var round=1;round<=100;round++) if(round==100) SelectionCarriesReleaseAuthority();
        for(var round=1;round<=100;round++) if(round==100) ResetClearsSelectedHistory();
        for(var round=1;round<=100;round++) if(round==100) ResetClearsWpfFacingSelectionProjection();
        for(var round=1;round<=100;round++) if(round==100) ResetLeavesCurrentResultsNonFinalized();
        for(var round=1;round<=100;round++) if(round==100) RerunCreatesFreshCurrentAuthority();
        for(var round=1;round<=100;round++) if(round==100) HistoricalAuthorityRemainsDistinctFromCurrent();
        for(var round=1;round<=100;round++) if(round==100) UnifiedProjectionRemainsConsistentAfterRerun();
    }

    private static void SelectionProjectionBindsSelectedHistory()
    {
        using var workspace=CreateFinalizedWorkspace();
        var snapshot=workspace.Capture();
        var projection=CreateResultsProjection(snapshot);
        Check(projection.Content.Results.SelectedOrdinal==snapshot.SelectedHistoryOrdinal &&
              projection.Content.Results.SelectedHistoryItem?.Ordinal==snapshot.SelectedHistoryOrdinal &&
              projection.Content.Results.SelectedHistoryIsCurrent &&
              projection.Content.Results.SelectionAuthorityText=="Selected history: current authority.",
            "Selected Results projection must bind the workspace selected History ordinal.");
    }

    private static void SelectionCarriesQualityAuthority()
    {
        using var workspace=CreateFinalizedWorkspace();
        var projection=CreateResultsProjection(workspace.Capture());
        Check(projection.Content.Results.SelectedHistoryItem?.QualityFingerprint==
              workspace.Capture().Quality.Fingerprint,
            "Selected History projection must carry the authoritative Quality fingerprint.");
    }

    private static void SelectionCarriesReplayAuthority()
    {
        using var workspace=CreateFinalizedWorkspace();
        var snapshot=workspace.Capture();
        var projection=CreateResultsProjection(snapshot);
        Check(projection.Content.Results.SelectedHistoryItem?.ReplayText.Contains(
                  snapshot.Replay!.ReplayFingerprint[..12])==true,
            "Selected History projection must carry the authoritative Replay fingerprint.");
    }

    private static void SelectionCarriesReleaseAuthority()
    {
        using var workspace=CreateFinalizedWorkspace();
        var snapshot=workspace.Capture();
        var projection=CreateResultsProjection(snapshot);
        Check(projection.Content.Results.SelectedHistoryItem?.ReleaseReady==
              snapshot.Release!.ReleaseReady,
            "Selected History projection must carry the authoritative Release state.");
    }

    private static void ResetClearsSelectedHistory()
    {
        using var workspace=CreateFinalizedWorkspace();
        var hadSelection=workspace.SelectedHistory is not null;
        workspace.ResetCurrentSession();
        Check(hadSelection &&
              workspace.SelectedHistory is null &&
              workspace.Capture().SelectedHistoryOrdinal is null,
            "Reset must clear the selected History authority.");
    }

    private static void ResetClearsWpfFacingSelectionProjection()
    {
        using var workspace=CreateFinalizedWorkspace();
        workspace.ResetCurrentSession();
        var projection=CreateResultsProjection(workspace.Capture());
        Check(projection.Content.Results.SelectedOrdinal is null &&
              projection.Content.Results.SelectedHistoryItem is null &&
              projection.Content.Results.SelectionText=="No historical run selected." &&
              !projection.Content.Results.SelectedHistoryIsCurrent &&
              projection.Content.Results.SelectionAuthorityText=="No historical run selected.",
            "Reset must project an empty History selection to the WPF-facing Results surface.");
    }

    private static void ResetLeavesCurrentResultsNonFinalized()
    {
        using var workspace=CreateFinalizedWorkspace();
        workspace.ResetCurrentSession();
        var snapshot=workspace.Capture();
        var current=ClientResultsPresentationRuntime.CreateCurrent(snapshot);
        Check(current.QualityText=="Quality pending" &&
              current.ReplayText=="Replay not available" &&
              current.ReleaseText=="Release not evaluated" &&
              !current.ReleaseReady,
            "Reset must clear finalized current Results authority.");
    }

    private static void RerunCreatesFreshCurrentAuthority()
    {
        using var workspace=CreateFinalizedWorkspace();
        var first=workspace.Capture();
        workspace.ResetCurrentSession();
        workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest())
            .AsTask().GetAwaiter().GetResult();
        workspace.EvaluateQualityProvider("simulation-quality");
        var second=workspace.Capture();
        Check(second.Production.ActiveSessionId!=first.Production.ActiveSessionId &&
              second.Quality.Fingerprint!=first.Quality.Fingerprint &&
              second.Replay!.ReplayFingerprint!=first.Replay!.ReplayFingerprint,
            "Recovery re-execution must create a fresh current authority chain.");
    }

    private static void HistoricalAuthorityRemainsDistinctFromCurrent()
    {
        using var workspace=CreateFinalizedWorkspace();
        var first=workspace.Capture();
        workspace.ResetCurrentSession();
        workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest())
            .AsTask().GetAwaiter().GetResult();
        workspace.EvaluateQualityProvider("simulation-quality");
        var second=workspace.Capture();
        workspace.SelectHistory(first.History.Entries[0].Ordinal);
        var historicalProjection=CreateResultsProjection(workspace.Capture());
        Check(second.History.Entries.Count==2 &&
              second.History.Entries[0].QualityFingerprint==first.Quality.Fingerprint &&
              second.History.Entries[0].ReplayFingerprint==first.Replay!.ReplayFingerprint &&
              second.Quality.Fingerprint!=second.History.Entries[0].QualityFingerprint &&
              !historicalProjection.Content.Results.SelectedHistoryIsCurrent &&
              historicalProjection.Content.Results.SelectionAuthorityText=="Selected history: historical authority.",
            "Current Results must remain distinct from immutable historical authority.");
    }

    private static void UnifiedProjectionRemainsConsistentAfterRerun()
    {
        using var workspace=CreateFinalizedWorkspace();
        workspace.ResetCurrentSession();
        workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest())
            .AsTask().GetAwaiter().GetResult();
        workspace.EvaluateQualityProvider("simulation-quality");
        var snapshot=workspace.Capture();
        var projection=CreateResultsProjection(snapshot);
        Check(projection.Content.Results.Current.QualityFingerprint==snapshot.Quality.Fingerprint &&
              projection.Content.Results.SelectedHistoryItem?.QualityFingerprint==snapshot.Quality.Fingerprint &&
              projection.Content.Workflow.HistoryCount==snapshot.History.Entries.Count,
            "Unified Projection must remain internally consistent after recovery and re-execution.");
    }

    private static ClientWorkspaceClientSnapshot CreateResultsProjection(
        ClientInspectionWorkspaceSnapshot snapshot) =>
        ClientWorkspaceClientSnapshotRuntime.Create(
            new ClientWorkspaceSelection(ClientWorkspaceKind.Results,1),
            ClientCommandAvailabilityRuntime.Create(snapshot),
            snapshot);

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
            new ClientAcquisitionDescriptor(
                "simulation",
                "Deterministic Simulation Source",
                true));
        workspace.ExecuteAsync(
            ClientSimulationSessionFactory.CreateReleaseManifest())
            .AsTask().GetAwaiter().GetResult();
        workspace.EvaluateQualityProvider("simulation-quality");
        return workspace;
    }

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Results selection recovery smoke failed: "+message);
    }
}
