using Asun.Device.Contracts;

namespace Asun.Platform.ClientIntegration;

public static class ClientInspectionWorkspaceChangedSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) BindPublishesFullSnapshot();
        for(var round=1;round<=100;round++) if(round==100) BindSnapshotContainsAcquisition();
        for(var round=1;round<=100;round++) if(round==100) BindSnapshotContainsProduction();
        for(var round=1;round<=100;round++) if(round==100) PreviewPublishesFullSnapshot();
        for(var round=1;round<=100;round++) if(round==100) PreviewSnapshotContainsPreview();
        for(var round=1;round<=100;round++) if(round==100) QualityClearPublishesFullSnapshot();
        for(var round=1;round<=100;round++) if(round==100) HistoryResetPublishesFullSnapshot();
        for(var round=1;round<=100;round++) if(round==100) SessionResetPublishesFullSnapshot();
        for(var round=1;round<=100;round++) if(round==100) RebindingPublishesFullSnapshot();
        for(var round=1;round<=100;round++) if(round==100) ChangeStreamDoesNotCreateAuthorities();
    }

    private static void BindPublishesFullSnapshot()
    {
        using var workspace=CreateWorkspace();
        var changes=0;
        workspace.Changed+=_=>changes++;
        workspace.BindAcquisition(new DeterministicSource(),Descriptor());
        Check(changes==1,"Acquisition binding must publish one full Inspection snapshot change.");
    }

    private static void BindSnapshotContainsAcquisition()
    {
        using var workspace=CreateWorkspace();
        ClientInspectionWorkspaceSnapshot? snapshot=null;
        workspace.Changed+=value=>snapshot=value;
        workspace.BindAcquisition(new DeterministicSource(),Descriptor());
        Check(snapshot?.Acquisition.State==ClientAcquisitionState.Ready,
            "Full Inspection snapshot must contain the current Acquisition state.");
    }

    private static void BindSnapshotContainsProduction()
    {
        using var workspace=CreateWorkspace();
        ClientInspectionWorkspaceSnapshot? snapshot=null;
        workspace.Changed+=value=>snapshot=value;
        workspace.BindAcquisition(new DeterministicSource(),Descriptor());
        Check(snapshot?.Production.Status==ClientExecutionStatus.Idle,
            "Full Inspection snapshot must retain Production state.");
    }

    private static void PreviewPublishesFullSnapshot()
    {
        using var workspace=CreateWorkspace();
        var changes=0;
        workspace.Changed+=_=>changes++;
        workspace.BindAcquisition(new DeterministicSource(),Descriptor());
        workspace.PreviewAcquisitionAsync().AsTask().GetAwaiter().GetResult();
        Check(changes==2,"Preview must publish a full Inspection snapshot after binding.");
    }

    private static void PreviewSnapshotContainsPreview()
    {
        using var workspace=CreateWorkspace();
        ClientInspectionWorkspaceSnapshot? snapshot=null;
        workspace.Changed+=value=>snapshot=value;
        workspace.BindAcquisition(new DeterministicSource(),Descriptor());
        workspace.PreviewAcquisitionAsync().AsTask().GetAwaiter().GetResult();
        Check(snapshot?.Acquisition.Preview is not null,
            "Full Inspection snapshot must carry the latest Acquisition preview.");
    }

    private static void QualityClearPublishesFullSnapshot()
    {
        using var workspace=CreateWorkspace();
        var changes=0;
        workspace.Changed+=_=>changes++;
        workspace.ClearQualityRun();
        Check(changes==1,"Quality clear must publish a full Inspection snapshot.");
    }

    private static void HistoryResetPublishesFullSnapshot()
    {
        using var workspace=CreateWorkspace();
        var changes=0;
        workspace.Changed+=_=>changes++;
        workspace.ResetHistory();
        Check(changes==1,"History reset must publish a full Inspection snapshot.");
    }

    private static void SessionResetPublishesFullSnapshot()
    {
        using var workspace=CreateWorkspace();
        var changes=0;
        workspace.Changed+=_=>changes++;
        workspace.ResetCurrentSession();
        Check(changes>=1,"Session reset must publish a full Inspection snapshot.");
    }

    private static void RebindingPublishesFullSnapshot()
    {
        using var workspace=CreateWorkspace();
        var changes=0;
        workspace.Changed+=_=>changes++;
        workspace.BindAcquisition(new DeterministicSource(),Descriptor("source-a"));
        workspace.BindAcquisition(new DeterministicSource(),Descriptor("source-b"));
        Check(changes==2 && workspace.Acquisition.Descriptor?.SourceId=="source-b",
            "Rebinding must publish each new Inspection snapshot.");
    }

    private static void ChangeStreamDoesNotCreateAuthorities()
    {
        using var workspace=CreateWorkspace();
        ClientInspectionWorkspaceSnapshot? snapshot=null;
        workspace.Changed+=value=>snapshot=value;
        workspace.BindAcquisition(new DeterministicSource(),Descriptor());
        Check(snapshot?.Replay is null &&
              snapshot?.Release is null &&
              snapshot?.Quality.IsBound==false,
            "Full change stream must project existing state without creating Replay, Release, or Quality authority.");
    }

    private static ClientInspectionWorkspace CreateWorkspace() =>
        new(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

    private static ClientAcquisitionDescriptor Descriptor(
        string id="source") =>
        new(id,"Deterministic Source",true);

    private sealed class DeterministicSource : IFrameSource
    {
        public ValueTask<CapturedFrame?> CaptureAsync(
            CancellationToken cancellationToken=default) =>
            ValueTask.FromResult<CapturedFrame?>(CreateFrame());

        private static CapturedFrame CreateFrame() =>
            CapturedFrame.Create(
                new FrameCaptureMetadata(
                    FrameSequence.Create(1),
                    16,
                    12,
                    "Gray8",
                    DateTimeOffset.UtcNow),
                new byte[16*12]);
    }

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Inspection workspace changed smoke failed: "+message);
    }
}
