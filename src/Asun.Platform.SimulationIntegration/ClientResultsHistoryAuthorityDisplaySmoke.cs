namespace Asun.Platform.SimulationIntegration;

using Asun.Platform.ClientIntegration;

public static class ClientResultsHistoryAuthorityDisplaySmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) HistoryDisplayCarriesQualityAuthority();
        for(var round=1;round<=100;round++) if(round==100) HistoryDisplayCarriesReplayAuthority();
        for(var round=1;round<=100;round++) if(round==100) HistoryDisplayCarriesReleaseAuthority();
        for(var round=1;round<=100;round++) if(round==100) SelectedHistoryProjectionMatchesWorkspaceSelection();
        for(var round=1;round<=100;round++) if(round==100) ResultsCurrentQualityMatchesQualityAuthority();
        for(var round=1;round<=100;round++) if(round==100) ResultsCurrentReplayMatchesReplayAuthority();
        for(var round=1;round<=100;round++) if(round==100) ResultsCurrentReleaseMatchesReleaseAuthority();
        for(var round=1;round<=100;round++) if(round==100) InvalidSelectionCannotExposeHistoryAuthority();
        for(var round=1;round<=100;round++) if(round==100) UnifiedProjectionPreservesResultsAuthority();
        for(var round=1;round<=100;round++) if(round==100) HistoryAuthorityRemainsImmutableAfterRecovery();

    }

    private static void HistoryDisplayCarriesQualityAuthority()
    {
        using var workspace=CreateFinalizedWorkspace();
        var snapshot=workspace.Capture();
        var item=ClientRunHistoryPresentationRuntime.CreateItem(
            snapshot.History.Entries.Single());
        Check(item.QualityFingerprint==snapshot.Quality.Fingerprint &&
              item.QualityText.StartsWith("Quality "),
            "History display must carry the authoritative Quality fingerprint.");
    }

    private static void HistoryDisplayCarriesReplayAuthority()
    {
        using var workspace=CreateFinalizedWorkspace();
        var snapshot=workspace.Capture();
        var item=ClientRunHistoryPresentationRuntime.CreateItem(
            snapshot.History.Entries.Single());
        Check(item.ReplayText.StartsWith("Replay ") &&
              item.ReplayText.Contains(snapshot.Replay!.ReplayFingerprint[..12]),
            "History display must carry the authoritative Replay fingerprint.");
    }

    private static void HistoryDisplayCarriesReleaseAuthority()
    {
        using var workspace=CreateFinalizedWorkspace();
        var snapshot=workspace.Capture();
        var item=ClientRunHistoryPresentationRuntime.CreateItem(
            snapshot.History.Entries.Single());
        Check(item.ReleaseReady==snapshot.Release!.ReleaseReady &&
              item.ReleaseText.Contains(snapshot.Release.ReleaseReady ? "Release Ready" : "Release Not Ready"),
            "History display must carry Release authority.");
    }

    private static void SelectedHistoryProjectionMatchesWorkspaceSelection()
    {
        using var workspace=CreateFinalizedWorkspace();
        var snapshot=workspace.Capture();
        var projection=CreateResultsProjection(snapshot);
        Check(projection.Content.Results.SelectedOrdinal==snapshot.SelectedHistoryOrdinal &&
              projection.Content.Results.SelectedHistoryItem?.QualityFingerprint==snapshot.Quality.Fingerprint,
            "Selected Results history projection must match workspace selection and Quality authority.");
    }

    private static void ResultsCurrentQualityMatchesQualityAuthority()
    {
        using var workspace=CreateFinalizedWorkspace();
        var snapshot=workspace.Capture();
        var current=ClientResultsPresentationRuntime.CreateCurrent(snapshot);
        Check(current.QualityFingerprint==snapshot.Quality.Fingerprint &&
              current.QualityText.Contains(snapshot.Quality.Fingerprint![..12]),
            "Current Results must expose the authoritative Quality fingerprint.");
    }

    private static void ResultsCurrentReplayMatchesReplayAuthority()
    {
        using var workspace=CreateFinalizedWorkspace();
        var snapshot=workspace.Capture();
        var current=ClientResultsPresentationRuntime.CreateCurrent(snapshot);
        Check(current.ReplayText.Contains(snapshot.Replay!.ReplayFingerprint[..12]),
            "Current Results must expose the authoritative Replay fingerprint.");
    }

    private static void ResultsCurrentReleaseMatchesReleaseAuthority()
    {
        using var workspace=CreateFinalizedWorkspace();
        var snapshot=workspace.Capture();
        var current=ClientResultsPresentationRuntime.CreateCurrent(snapshot);
        Check(current.ReleaseReady==snapshot.Release!.ReleaseReady,
            "Current Results must expose the authoritative Release state.");
    }

    private static void InvalidSelectionCannotExposeHistoryAuthority()
    {
        using var workspace=CreateFinalizedWorkspace();
        Check(!workspace.SelectHistory(long.MaxValue),
            "An unavailable History ordinal must be rejected.");
        var projection=CreateResultsProjection(workspace.Capture());
        Check(projection.Content.Results.SelectedHistoryItem is not null &&
              projection.Content.Results.SelectedOrdinal==workspace.SelectedHistoryOrdinal,
            "Rejected selection must not replace the existing authoritative selection.");
    }

    private static void UnifiedProjectionPreservesResultsAuthority()
    {
        using var workspace=CreateFinalizedWorkspace();
        var snapshot=workspace.Capture();
        var projection=CreateResultsProjection(snapshot);
        Check(projection.Content.Results.Current.QualityFingerprint==snapshot.Quality.Fingerprint &&
              projection.Content.Results.SelectedHistoryItem?.QualityFingerprint==snapshot.Quality.Fingerprint &&
              projection.Content.Workflow.HistoryCount==snapshot.History.Entries.Count,
            "Unified Projection must preserve current and selected Results authority.");
    }

    private static void HistoryAuthorityRemainsImmutableAfterRecovery()
    {
        using var workspace=CreateFinalizedWorkspace();
        var first=workspace.Capture().History.Entries.Single();
        workspace.ResetCurrentSession();
        workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest())
            .AsTask().GetAwaiter().GetResult();
        workspace.EvaluateQualityProvider("simulation-quality");
        var entries=workspace.Capture().History.Entries;
        Check(entries.Count==2 &&
              entries[0].QualityFingerprint==first.QualityFingerprint &&
              entries[0].ReplayFingerprint==first.ReplayFingerprint,
            "Recovery must not mutate prior History Quality/Replay authority.");
    }

    private static ClientWorkspaceClientSnapshot CreateResultsProjection(
        ClientInspectionWorkspaceSnapshot snapshot)
    {
        return ClientWorkspaceClientSnapshotRuntime.Create(
            new ClientWorkspaceSelection(ClientWorkspaceKind.Results,1),
            ClientCommandAvailabilityRuntime.Create(snapshot),
            snapshot);
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
                "Results History authority display smoke failed: "+message);
    }
}
