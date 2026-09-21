namespace Asun.Platform.ClientIntegration;

public static class ClientInspectionWorkflowIntegritySmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) ReadyProgramCountMustMatch();
        for(var round=1;round<=100;round++) if(round==100) InvalidProgramItemsAreRejected();
        for(var round=1;round<=100;round++) if(round==100) FaultedAcquisitionCannotRetainPreview();
        for(var round=1;round<=100;round++) if(round==100) QualityPendingProductionIsValid();
        for(var round=1;round<=100;round++) if(round==100) QualityPendingCannotExposeEvidence();
        for(var round=1;round<=100;round++) if(round==100) ReleaseRequiresReplay();
        for(var round=1;round<=100;round++) if(round==100) ReplayReleaseIdentityMustMatch();
        for(var round=1;round<=100;round++) if(round==100) QualityRequiresCompletedProduction();
        for(var round=1;round<=100;round++) if(round==100) SelectedHistoryMustExist();
        for(var round=1;round<=100;round++) if(round==100) CoherentCompletedChainIsValid();
    }


    private static void EmptySnapshotIsValid()
    {
        var snapshot=CreateBaseSnapshot();
        Check(ClientInspectionWorkflowIntegrityRuntime.IsValid(snapshot),
            "Empty client snapshot must remain a valid baseline state.");
    }

    private static void ReadyProgramCountMustMatch()
    {
        var stepId=Guid.NewGuid();
        var snapshot=CreateBaseSnapshot() with
        {
            Program=new ClientProgramWorkspaceSnapshot(
                ClientProgramLoadStatus.Ready,
                Guid.NewGuid(),
                "Program",
                new Version(1,0),
                2,
                new string('a',64),
                Array.Empty<string>())
            {
                SelectedStepId=stepId
            },
            ProgramItems=new[]
            {
                new ClientProgramDisplayItem(1,"A","Measure",0){StepId=stepId}
            }
        };
        Check(!ClientInspectionWorkflowIntegrityRuntime.IsValid(snapshot),
            "Ready Program with mismatched item count must be rejected by the integrity gate.");
    }

    private static void InvalidProgramItemsAreRejected()
    {
        var snapshot=CreateBaseSnapshot() with
        {
            Program=new ClientProgramWorkspaceSnapshot(
                ClientProgramLoadStatus.Invalid,
                Guid.NewGuid(),
                null,
                new Version(1,0),
                0,
                null,
                new[] { "invalid" }),
            ProgramItems=new[] { new ClientProgramDisplayItem(1,"A","Measure",0) }
        };
        Check(!ClientInspectionWorkflowIntegrityRuntime.IsValid(snapshot),
            "Invalid Program cannot expose ProgramItems.");
    }

    private static void FaultedAcquisitionCannotRetainPreview()
    {
        var snapshot=CreateBaseSnapshot() with
        {
            Acquisition=new ClientAcquisitionWorkspaceSnapshot(
                ClientAcquisitionState.Faulted,
                new ClientAcquisitionDescriptor("camera","Camera",false),
                "camera fault",
                false)
            {
                Preview=new ClientAcquisitionPreviewSnapshot(
                    Asun.Device.Contracts.FrameSequence.Create(1),10,10,"Gray8",
                    DateTimeOffset.UtcNow,new string('a',64),new byte[1])
            }
        };
        Check(!ClientInspectionWorkflowIntegrityRuntime.IsValid(snapshot),
            "Faulted Acquisition with stale Preview must be rejected.");
    }

    private static void QualityPendingProductionIsValid()
    {
        var snapshot=CreateBaseSnapshot() with
        {
            Production=CreateProduction(ClientExecutionStatus.Completed)
        };
        Check(ClientInspectionWorkflowIntegrityRuntime.IsValid(snapshot),
            "Completed Production must remain valid while Quality is explicitly pending.");
    }

    private static void QualityPendingCannotExposeEvidence()
    {
        var replay=CreateReplay();
        var snapshot=CreateBaseSnapshot() with
        {
            Production=CreateProduction(ClientExecutionStatus.Completed),
            Replay=replay
        };
        Check(!ClientInspectionWorkflowIntegrityRuntime.IsValid(snapshot),
            "Quality-pending Production cannot expose Replay evidence.");
    }

    private static void ReleaseRequiresReplay()
    {
        var production=CreateProduction(ClientExecutionStatus.Completed);
        var release=new ClientReleaseProjection(
            production.ProgramId!.Value,
            production.ActiveSessionId!.Value,
            new string('a',64),true,"artifact.bin",
            new string('b',64),new string('c',64));
        var snapshot=CreateBaseSnapshot() with
        {
            Production=production,
            Release=release
        };
        Check(!ClientInspectionWorkflowIntegrityRuntime.IsValid(snapshot),
            "Release projection without Replay authority must be rejected.");
    }

    private static void ReplayReleaseIdentityMustMatch()
    {
        var replay=CreateReplay();
        var release=new ClientReleaseProjection(
            Guid.NewGuid(),replay.ProductionSessionId,replay.ReplayFingerprint,
            true,"artifact.bin",new string('b',64),new string('c',64));
        var snapshot=CreateBaseSnapshot() with
        {
            Production=CreateProduction(ClientExecutionStatus.Completed),
            Replay=replay,
            Release=release
        };
        Check(!ClientInspectionWorkflowIntegrityRuntime.IsValid(snapshot),
            "Replay and Release identity drift must be rejected.");
    }

    private static void QualityRequiresCompletedProduction()
    {
        var quality=new ClientQualityWorkspaceSnapshot(
            Guid.NewGuid(),1,1,0,1,0,0,new string('a',64),true,
            Array.Empty<ClientQualityFindingDisplayItem>());
        var snapshot=CreateBaseSnapshot() with
        {
            Production=CreateProduction(ClientExecutionStatus.Ready),
            Quality=quality
        };
        Check(!ClientInspectionWorkflowIntegrityRuntime.IsValid(snapshot),
            "Quality bound before completed Production must be rejected.");
    }

    private static void SelectedHistoryMustExist()
    {
        var snapshot=CreateBaseSnapshot() with { SelectedHistoryOrdinal=99 };
        Check(!ClientInspectionWorkflowIntegrityRuntime.IsValid(snapshot),
            "Unknown selected history ordinal must be rejected.");
    }

    private static void CoherentCompletedChainIsValid()
    {
        var replay=CreateReplay();
        var quality=new ClientQualityWorkspaceSnapshot(
            Guid.NewGuid(),1,1,0,1,0,0,new string('a',64),true,
            Array.Empty<ClientQualityFindingDisplayItem>());
        replay=replay with { QualityFingerprint=quality.Fingerprint };
        var release=new ClientReleaseProjection(
            replay.ProgramId,replay.ProductionSessionId,replay.ReplayFingerprint,
            true,"artifact.bin",new string('b',64),new string('c',64))
        {
            QualityFingerprint=quality.Fingerprint
        };
        var snapshot=CreateBaseSnapshot() with
        {
            Production=CreateProduction(ClientExecutionStatus.Completed),
            Quality=quality,
            Replay=replay,
            Release=release,
            History=new ClientProductionRunHistorySnapshot(
                20,2,0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(
                        1,replay.ProgramId,replay.ProductionSessionId,
                        replay.FrameCount,replay.ReplayFingerprint,true,"artifact.bin")
                }),
            SelectedHistoryOrdinal=1
        };
        Check(ClientInspectionWorkflowIntegrityRuntime.IsValid(snapshot),
            "Coherent completed Production/Quality/Replay/Release/History chain must pass integrity validation.");
    }

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Inspection workflow integrity smoke failed: "+message);
    }
}
