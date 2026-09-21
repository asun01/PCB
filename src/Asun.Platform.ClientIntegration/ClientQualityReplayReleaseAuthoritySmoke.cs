using Asun.Platform.SimulationIntegration;

namespace Asun.Platform.ClientIntegration;

public static class ClientQualityReplayReleaseAuthoritySmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) ProductionCompletesBeforeQuality();
        for(var round=1;round<=100;round++) if(round==100) ReplayIsDeferredUntilQuality();
        for(var round=1;round<=100;round++) if(round==100) ReleaseIsDeferredUntilQuality();
        for(var round=1;round<=100;round++) if(round==100) QualityBecomesBoundAuthority();
        for(var round=1;round<=100;round++) if(round==100) ReplayCarriesQualityFingerprint();
        for(var round=1;round<=100;round++) if(round==100) ReleaseCarriesQualityFingerprint();
        for(var round=1;round<=100;round++) if(round==100) ReplayAndReleaseRemainLinked();
        for(var round=1;round<=100;round++) if(round==100) HistoryIsCreatedOnlyAfterFinalization();
        for(var round=1;round<=100;round++) if(round==100) FinalizedWorkflowIsValid();
        for(var round=1;round<=100;round++) if(round==100) ResetClearsAuthorityChain();
    }

    private static void ProductionCompletesBeforeQuality()
    {
        using var workspace=CreateCompletedProductionWorkspace();
        Check(workspace.Production.Status==ClientExecutionStatus.Completed &&
              !workspace.Quality.IsBound,
            "Production must complete while Quality remains an explicit pending authority.");
    }

    private static void ReplayIsDeferredUntilQuality()
    {
        using var workspace=CreateCompletedProductionWorkspace();
        Check(workspace.Capture().Replay is null,
            "Replay must not exist before authoritative Quality evaluation.");
    }

    private static void ReleaseIsDeferredUntilQuality()
    {
        using var workspace=CreateCompletedProductionWorkspace();
        Check(workspace.Capture().Release is null,
            "Release must not exist before authoritative Quality evaluation.");
    }

    private static void QualityBecomesBoundAuthority()
    {
        using var workspace=FinalizeQuality();
        Check(workspace.Quality.IsBound &&
              workspace.Quality.Fingerprint is { Length:64 },
            "Quality evaluation must publish an authoritative Quality fingerprint.");
    }

    private static void ReplayCarriesQualityFingerprint()
    {
        using var workspace=FinalizeQuality();
        Check(workspace.Capture().Replay?.QualityFingerprint==
              workspace.Quality.Fingerprint,
            "Replay must carry the exact authoritative Quality fingerprint.");
    }

    private static void ReleaseCarriesQualityFingerprint()
    {
        using var workspace=FinalizeQuality();
        Check(workspace.Capture().Release?.QualityFingerprint==
              workspace.Quality.Fingerprint,
            "Release must carry the exact authoritative Quality fingerprint.");
    }

    private static void ReplayAndReleaseRemainLinked()
    {
        using var workspace=FinalizeQuality();
        var snapshot=workspace.Capture();
        Check(snapshot.Replay is not null &&
              snapshot.Release is not null &&
              snapshot.Replay.ReplayFingerprint==snapshot.Release.ReplayFingerprint &&
              snapshot.Replay.QualityFingerprint==snapshot.Release.QualityFingerprint,
            "Replay and Release must remain bound to the same Quality-authorized execution identity.");
    }

    private static void HistoryIsCreatedOnlyAfterFinalization()
    {
        using var workspace=FinalizeQuality();
        var snapshot=workspace.Capture();
        Check(snapshot.History.Entries.Count==1 &&
              snapshot.SelectedHistoryOrdinal==snapshot.History.Entries[0].Ordinal,
            "Run History must be appended only after Quality, Replay, and Release finalization.");
    }

    private static void FinalizedWorkflowIsValid()
    {
        using var workspace=FinalizeQuality();
        Check(ClientInspectionWorkflowIntegrityRuntime.IsValid(workspace.Capture()),
            "The finalized Quality → Replay → Release workflow must satisfy the client integrity boundary.");
    }

    private static void ResetClearsAuthorityChain()
    {
        using var workspace=FinalizeQuality();
        workspace.ResetCurrentSession();
        var snapshot=workspace.Capture();
        Check(!snapshot.Quality.IsBound &&
              snapshot.Replay is null &&
              snapshot.Release is null &&
              snapshot.SelectedHistoryOrdinal is null &&
              workspace.LastProductionReport is null,
            "Reset must clear the complete Quality → Replay → Release authority chain.");
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
                "Quality Replay Release authority smoke failed: "+message);
    }
}
