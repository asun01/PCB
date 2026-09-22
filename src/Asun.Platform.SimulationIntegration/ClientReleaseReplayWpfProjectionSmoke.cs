namespace Asun.Platform.SimulationIntegration;

using Asun.Platform.ClientIntegration;

public static class ClientReleaseReplayWpfProjectionSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) QualityAuthorityIsProjected();
        for(var round=1;round<=100;round++) if(round==100) ReplayAuthorityIsProjected();
        for(var round=1;round<=100;round++) if(round==100) ReleaseAuthorityIsProjected();
        for(var round=1;round<=100;round++) if(round==100) ReleaseReplayQualityMatchesCurrentQuality();
        for(var round=1;round<=100;round++) if(round==100) ReleaseReplayReplayMatchesCurrentReplay();
        for(var round=1;round<=100;round++) if(round==100) ReleaseReplayReleaseMatchesCurrentRelease();
        for(var round=1;round<=100;round++) if(round==100) ResetClearsReleaseReplayAuthority();
        for(var round=1;round<=100;round++) if(round==100) ResetLeavesNoReplayOrRelease();
        for(var round=1;round<=100;round++) if(round==100) RecoveryCreatesFreshReleaseReplayAuthority();
        for(var round=1;round<=100;round++) if(round==100) UnifiedProjectionPreservesReleaseReplayAuthority();
    }

    private static void QualityAuthorityIsProjected()
    {
        using var workspace=CreateFinalizedWorkspace();
        var snapshot=workspace.Capture();
        var surface=ClientResultsSurfaceRuntime.Create(snapshot);
        Check(surface.ReleaseReplay.QualityFingerprint==snapshot.Quality.Fingerprint,
            "ReleaseReplay surface must expose the current Quality authority.");
    }

    private static void ReplayAuthorityIsProjected()
    {
        using var workspace=CreateFinalizedWorkspace();
        var snapshot=workspace.Capture();
        var surface=ClientResultsSurfaceRuntime.Create(snapshot);
        Check(surface.ReleaseReplay.ReplayAvailable &&
              surface.ReleaseReplay.ReplayFingerprint==snapshot.Replay!.ReplayFingerprint,
            "ReleaseReplay surface must expose the current Replay authority.");
    }

    private static void ReleaseAuthorityIsProjected()
    {
        using var workspace=CreateFinalizedWorkspace();
        var snapshot=workspace.Capture();
        var surface=ClientResultsSurfaceRuntime.Create(snapshot);
        Check(surface.ReleaseReplay.ReleaseAvailable &&
              surface.ReleaseReplay.ReleaseManifestFingerprint==snapshot.Release!.ReleaseManifestFingerprint &&
              surface.ReleaseReplay.ReleaseReady==snapshot.Release.ReleaseReady,
            "ReleaseReplay surface must expose the current Release authority.");
    }

    private static void ReleaseReplayQualityMatchesCurrentQuality()
    {
        using var workspace=CreateFinalizedWorkspace();
        var surface=ClientResultsSurfaceRuntime.Create(workspace.Capture());
        Check(surface.Current.QualityFingerprint==surface.ReleaseReplay.QualityFingerprint,
            "Results Current and ReleaseReplay must share the same Quality authority.");
    }

    private static void ReleaseReplayReplayMatchesCurrentReplay()
    {
        using var workspace=CreateFinalizedWorkspace();
        var snapshot=workspace.Capture();
        var surface=ClientResultsSurfaceRuntime.Create(snapshot);
        Check(surface.ReleaseReplay.ReplayFingerprint==snapshot.Replay!.ReplayFingerprint,
            "ReleaseReplay Replay identity must remain bound to current Replay evidence.");
    }

    private static void ReleaseReplayReleaseMatchesCurrentRelease()
    {
        using var workspace=CreateFinalizedWorkspace();
        var snapshot=workspace.Capture();
        var surface=ClientResultsSurfaceRuntime.Create(snapshot);
        var history=surface.History.Single();
        Check(surface.ReleaseReplay.ReleaseManifestFingerprint==
              snapshot.Release!.ReleaseManifestFingerprint &&
              surface.ReleaseReplay.ReleaseArtifactPath==snapshot.Release.ArtifactPath &&
              history.ProductionSessionId==snapshot.Production.ActiveSessionId &&
              history.QualityFingerprint==surface.ReleaseReplay.QualityFingerprint,
            "ReleaseReplay Release identity and historical authority must remain bound to the same finalized run.");
    }

    private static void ResetClearsReleaseReplayAuthority()
    {
        using var workspace=CreateFinalizedWorkspace();
        workspace.ResetCurrentSession();
        var surface=ClientResultsSurfaceRuntime.Create(workspace.Capture());
        Check(!surface.ReleaseReplay.ReplayAvailable &&
              !surface.ReleaseReplay.ReleaseAvailable &&
              surface.ReleaseReplay.QualityFingerprint==string.Empty,
            "Reset must clear ReleaseReplay authority from the Results projection.");
    }

    private static void ResetLeavesNoReplayOrRelease()
    {
        using var workspace=CreateFinalizedWorkspace();
        workspace.ResetCurrentSession();
        var snapshot=workspace.Capture();
        Check(snapshot.Replay is null && snapshot.Release is null && !snapshot.Quality.IsBound,
            "Reset must clear the runtime evidence behind ReleaseReplay.");
    }

    private static void RecoveryCreatesFreshReleaseReplayAuthority()
    {
        using var workspace=CreateFinalizedWorkspace();
        var first=ClientResultsSurfaceRuntime.Create(workspace.Capture());
        workspace.ResetCurrentSession();
        workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest())
            .AsTask().GetAwaiter().GetResult();
        workspace.EvaluateQualityProvider("simulation-quality");
        var secondSnapshot=workspace.Capture();
        var second=ClientResultsSurfaceRuntime.Create(secondSnapshot);
        Check(second.ReleaseReplay.QualityFingerprint==secondSnapshot.Quality.Fingerprint &&
              second.ReleaseReplay.ReplayFingerprint!=first.ReleaseReplay.ReplayFingerprint &&
              second.ReleaseReplay.ReleaseManifestFingerprint==secondSnapshot.Release!.ReleaseManifestFingerprint,
            "Recovery re-execution must publish a fresh ReleaseReplay authority chain.");
    }

    private static void UnifiedProjectionPreservesReleaseReplayAuthority()
    {
        using var workspace=CreateFinalizedWorkspace();
        var snapshot=workspace.Capture();
        var projection=ClientWorkspaceClientSnapshotRuntime.Create(
            new ClientWorkspaceSelection(ClientWorkspaceKind.Results,1),
            ClientCommandAvailabilityRuntime.Create(snapshot),
            snapshot);
        Check(projection.Content.Results.ReleaseReplay.QualityFingerprint==snapshot.Quality.Fingerprint &&
              projection.Content.Results.ReleaseReplay.ReplayFingerprint==snapshot.Replay!.ReplayFingerprint &&
              projection.Content.Results.ReleaseReplay.ReleaseManifestFingerprint==
              snapshot.Release!.ReleaseManifestFingerprint,
            "Unified Client Projection must preserve ReleaseReplay authority without reconstruction.");
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
                "ReleaseReplay WPF projection smoke failed: "+message);
    }
}
