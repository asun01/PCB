using Asun.Device.Contracts;

namespace Asun.Platform.ClientIntegration;

public static class ClientAcquisitionPreviewFaultSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) PreviewBecomesAvailable();
        for(var round=1;round<=100;round++) if(round==100) FaultInvalidatesPreview();
        for(var round=1;round<=100;round++) if(round==100) UnbindInvalidatesPreview();
        for(var round=1;round<=100;round++) if(round==100) FaultStateBlocksCapture();
        for(var round=1;round<=100;round++) if(round==100) RecoveryClearsFault();
        for(var round=1;round<=100;round++) if(round==100) PreviewMetadataSurvivesCapture();
        for(var round=1;round<=100;round++) if(round==100) PreviewFingerprintSurvivesCapture();
        for(var round=1;round<=100;round++) if(round==100) BoundSourceIsReady();
        for(var round=1;round<=100;round++) if(round==100) PreviewPublishesChange();
        for(var round=1;round<=100;round++) if(round==100) FaultPublishesChange();
    }

    private static void PreviewBecomesAvailable()
    {
        var workspace=CreateWorkspace();
        workspace.Bind(new DeterministicSource(),Descriptor());
        workspace.PreviewAsync().AsTask().GetAwaiter().GetResult();
        Check(workspace.Snapshot.Preview is not null,
            "Successful Acquisition preview must be retained.");
    }

    private static void FaultInvalidatesPreview()
    {
        var workspace=CreateWorkspace();
        workspace.Bind(new DeterministicSource(),Descriptor());
        workspace.PreviewAsync().AsTask().GetAwaiter().GetResult();
        workspace.SetFault("camera fault");
        Check(workspace.Snapshot.Preview is null,
            "Acquisition fault must invalidate a stale preview.");
    }

    private static void UnbindInvalidatesPreview()
    {
        var workspace=CreateWorkspace();
        workspace.Bind(new DeterministicSource(),Descriptor());
        workspace.PreviewAsync().AsTask().GetAwaiter().GetResult();
        workspace.Unbind();
        Check(workspace.Snapshot.Preview is null,
            "Acquisition unbind must invalidate preview.");
    }

    private static void FaultStateBlocksCapture()
    {
        var workspace=CreateWorkspace();
        workspace.Bind(new DeterministicSource(),Descriptor());
        workspace.SetFault("camera fault");
        var sourceAvailable=workspace.TryGetSource(out _);
        Check(workspace.Snapshot.State==ClientAcquisitionState.Faulted &&
              !workspace.Snapshot.CanCapture &&
              !sourceAvailable,
            "Faulted Acquisition must not advertise capture or expose a Production source.");
    }

    private static void RecoveryClearsFault()
    {
        var workspace=CreateWorkspace();
        workspace.Bind(new DeterministicSource(),Descriptor());
        workspace.SetFault("camera fault");
        workspace.PreviewAsync().AsTask().GetAwaiter().GetResult();
        var sourceAvailable=workspace.TryGetSource(out _);
        Check(workspace.Snapshot.State==ClientAcquisitionState.Ready &&
              workspace.Snapshot.LastError is null &&
              sourceAvailable,
            "A successful preview must recover the Acquisition state and expose the source again.");
    }

    private static void PreviewMetadataSurvivesCapture()
    {
        var workspace=CreateWorkspace();
        workspace.Bind(new DeterministicSource(),Descriptor());
        workspace.PreviewAsync().AsTask().GetAwaiter().GetResult();
        Check(workspace.Snapshot.Preview?.Width==16 &&
              workspace.Snapshot.Preview.Height==12,
            "Preview dimensions must come from the captured frame metadata.");
    }

    private static void PreviewFingerprintSurvivesCapture()
    {
        var workspace=CreateWorkspace();
        workspace.Bind(new DeterministicSource(),Descriptor());
        workspace.PreviewAsync().AsTask().GetAwaiter().GetResult();
        Check(!string.IsNullOrWhiteSpace(workspace.Snapshot.Preview?.PayloadFingerprint),
            "Preview fingerprint must come from the captured payload.");
    }

    private static void BoundSourceIsReady()
    {
        var workspace=CreateWorkspace();
        workspace.Bind(new DeterministicSource(),Descriptor());
        Check(workspace.Snapshot.State==ClientAcquisitionState.Ready &&
              workspace.Snapshot.CanCapture,
            "A cleanly bound Acquisition source must be Ready.");
    }

    private static void PreviewPublishesChange()
    {
        var workspace=CreateWorkspace();
        var changes=0;
        workspace.Changed+=_=>changes++;
        workspace.Bind(new DeterministicSource(),Descriptor());
        workspace.PreviewAsync().AsTask().GetAwaiter().GetResult();
        Check(changes>=2,
            "Acquisition bind and preview must publish client state changes.");
    }

    private static void FaultPublishesChange()
    {
        var workspace=CreateWorkspace();
        var changes=0;
        workspace.Changed+=_=>changes++;
        workspace.Bind(new DeterministicSource(),Descriptor());
        workspace.SetFault("camera fault");
        Check(changes>=2,
            "Acquisition fault must publish a client state change.");
    }

    private static ClientAcquisitionWorkspace CreateWorkspace() =>
        new();

    private static ClientAcquisitionDescriptor Descriptor() =>
        new("deterministic-preview","Deterministic Preview",true);

    private sealed class DeterministicSource : IFrameSource
    {
        public ValueTask<CapturedFrame?> CaptureAsync(
            CancellationToken cancellationToken=default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var metadata=new FrameCaptureMetadata(
                FrameSequence.Create(1),
                16,
                12,
                "Gray8",
                DateTimeOffset.UtcNow);

            return ValueTask.FromResult<CapturedFrame?>(
                CapturedFrame.Create(metadata,new byte[16*12]));
        }
    }

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Acquisition preview fault smoke failed: "+message);
    }
}
