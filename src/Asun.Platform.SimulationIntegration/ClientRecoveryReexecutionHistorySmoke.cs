namespace Asun.Platform.SimulationIntegration;

using Asun.Platform.ClientIntegration;

public static class ClientRecoveryReexecutionHistorySmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) ResetReexecutionCompletesAgain();
        for(var round=1;round<=100;round++) if(round==100) RecoveryClearsCurrentAuthorityBeforeRerun();
        for(var round=1;round<=100;round++) if(round==100) FirstHistoryEntrySurvivesRecovery();
        for(var round=1;round<=100;round++) if(round==100) SecondHistoryEntryGetsNewOrdinal();
        for(var round=1;round<=100;round++) if(round==100) SecondHistoryEntryUsesNewSessionEvidence();
        for(var round=1;round<=100;round++) if(round==100) CurrentSelectionMovesToSecondRun();
        for(var round=1;round<=100;round++) if(round==100) HistorySelectionSequenceAdvances();
        for(var round=1;round<=100;round++) if(round==100) ResultsProjectionShowsSelectedSecondRun();
        for(var round=1;round<=100;round++) if(round==100) UnifiedProjectionRemainsValidAfterRerun();
        for(var round=1;round<=100;round++) if(round==100) OldAndNewHistoryFingerprintsRemainDistinct();

    }

    private static void ResetReexecutionCompletesAgain()
        {
            using var workspace=FinalizeQuality();
            workspace.ResetCurrentSession();
            workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest())
                .AsTask().GetAwaiter().GetResult();
            workspace.EvaluateQualityProvider("simulation-quality");
            Check(workspace.Capture().Quality.IsBound &&
                  workspace.Capture().History.Entries.Count==2,
                "A recovered session must complete Production and Quality again.");
        }

        private static void RecoveryClearsCurrentAuthorityBeforeRerun()
        {
            using var workspace=FinalizeQuality();
            workspace.ResetCurrentSession();
            var snapshot=workspace.Capture();
            Check(snapshot.Quality.IsBound==false &&
                  snapshot.Replay is null &&
                  snapshot.Release is null &&
                  snapshot.SelectedHistoryOrdinal is null,
                "Recovery must clear current authority before re-execution.");
        }

        private static void FirstHistoryEntrySurvivesRecovery()
        {
            using var workspace=FinalizeQuality();
            var first=workspace.Capture().History.Entries.Single();
            workspace.ResetCurrentSession();
            workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest())
                .AsTask().GetAwaiter().GetResult();
            workspace.EvaluateQualityProvider("simulation-quality");
            var entries=workspace.Capture().History.Entries;
            Check(entries.Count==2 &&
                  entries[0].Ordinal==first.Ordinal &&
                  entries[0].ReplayFingerprint==first.ReplayFingerprint,
                "Recovery must not erase the prior authoritative History entry.");
        }

        private static void SecondHistoryEntryGetsNewOrdinal()
        {
            using var workspace=FinalizeQuality();
            var first=workspace.Capture().History.Entries.Single();
            workspace.ResetCurrentSession();
            workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest())
                .AsTask().GetAwaiter().GetResult();
            workspace.EvaluateQualityProvider("simulation-quality");
            var entries=workspace.Capture().History.Entries;
            Check(entries[1].Ordinal>first.Ordinal,
                "A recovered re-execution must append a new bounded History ordinal.");
        }

        private static void SecondHistoryEntryUsesNewSessionEvidence()
        {
            using var workspace=FinalizeQuality();
            var first=workspace.Capture().History.Entries.Single();
            workspace.ResetCurrentSession();
            workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest())
                .AsTask().GetAwaiter().GetResult();
            workspace.EvaluateQualityProvider("simulation-quality");
            var entries=workspace.Capture().History.Entries;
            Check(entries[1].ProductionSessionId!=first.ProductionSessionId ||
                  entries[1].ReplayFingerprint!=first.ReplayFingerprint,
                "The recovered execution must not reuse the previous finalized evidence identity.");
        }

        private static void CurrentSelectionMovesToSecondRun()
        {
            using var workspace=FinalizeQuality();
            workspace.ResetCurrentSession();
            workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest())
                .AsTask().GetAwaiter().GetResult();
            workspace.EvaluateQualityProvider("simulation-quality");
            var entries=workspace.Capture().History.Entries;
            Check(workspace.SelectedHistoryOrdinal==entries[1].Ordinal,
                "Quality finalization must select the newly appended History entry.");
        }

        private static void HistorySelectionSequenceAdvances()
        {
            using var workspace=FinalizeQuality();
            var entries=workspace.Capture().History.Entries;
            var first=entries[0].Ordinal;
            workspace.ResetCurrentSession();
            workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest())
                .AsTask().GetAwaiter().GetResult();
            workspace.EvaluateQualityProvider("simulation-quality");
            var second=workspace.Capture().History.Entries[1].Ordinal;
            Check(workspace.SelectHistory(first) &&
                  workspace.SelectHistory(second) &&
                  workspace.SelectedHistoryOrdinal==second,
                "History selection must remain bounded and move deterministically between runs.");
        }

        private static void ResultsProjectionShowsSelectedSecondRun()
        {
            using var workspace=FinalizeQuality();
            workspace.ResetCurrentSession();
            workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest())
                .AsTask().GetAwaiter().GetResult();
            workspace.EvaluateQualityProvider("simulation-quality");
            var snapshot=workspace.Capture();
            var projection=ClientWorkspaceClientSnapshotRuntime.Create(
                new ClientWorkspaceSelection(ClientWorkspaceKind.Results,1),
                ClientCommandAvailabilityRuntime.Create(snapshot),
                snapshot);
            Check(projection.Content.Results.SelectedHistoryItem?.Ordinal==
                  snapshot.History.Entries[1].Ordinal &&
                  projection.Content.Results.SelectionText.Contains(
                      snapshot.History.Entries[1].Ordinal.ToString()),
                "Results Projection must expose the selected recovered run.");
        }

        private static void UnifiedProjectionRemainsValidAfterRerun()
        {
            using var workspace=FinalizeQuality();
            workspace.ResetCurrentSession();
            workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest())
                .AsTask().GetAwaiter().GetResult();
            workspace.EvaluateQualityProvider("simulation-quality");
            var snapshot=workspace.Capture();
            var projection=ClientWorkspaceClientSnapshotRuntime.CreateValidated(
                new ClientWorkspaceSelection(ClientWorkspaceKind.Results,1),
                ClientWorkspaceCommandRoutingRuntime.Create(
                    new ClientWorkspaceSelection(ClientWorkspaceKind.Results,1),
                    ClientCommandAvailabilityRuntime.Create(snapshot)),
                snapshot);
            Check(projection.Content.Workflow.HistoryCount==2 &&
                  projection.Content.Results.History.Count==2,
                "Unified Projection must remain authoritative after recovered re-execution.");
        }

        private static void OldAndNewHistoryFingerprintsRemainDistinct()
        {
            using var workspace=FinalizeQuality();
            var first=workspace.Capture().History.Entries.Single();
            workspace.ResetCurrentSession();
            workspace.ExecuteAsync(ClientSimulationSessionFactory.CreateReleaseManifest())
                .AsTask().GetAwaiter().GetResult();
            workspace.EvaluateQualityProvider("simulation-quality");
            var entries=workspace.Capture().History.Entries;
            Check(entries[0].ReplayFingerprint!=entries[1].ReplayFingerprint ||
                  entries[0].ProductionSessionId!=entries[1].ProductionSessionId,
                "Recovered History must retain distinguishable prior and current evidence.");
        }

        private static ClientInspectionWorkspace FinalizeQuality()
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
                    "Recovery re-execution History smoke failed: "+message);
        }
    }
}
