using Asun.Device.Contracts;

namespace Asun.Platform.ClientIntegration;

public static class ClientInspectionAcquisitionPreviewSurfaceSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) SurfaceCarriesPreview();
        for(var round=1;round<=100;round++) if(round==100) SurfaceCarriesPayload();
        for(var round=1;round<=100;round++) if(round==100) SurfaceCarriesWidth();
        for(var round=1;round<=100;round++) if(round==100) SurfaceCarriesHeight();
        for(var round=1;round<=100;round++) if(round==100) SurfaceCarriesPixelFormat();
        for(var round=1;round<=100;round++) if(round==100) SurfaceCarriesSequence();
        for(var round=1;round<=100;round++) if(round==100) SurfaceCarriesFingerprint();
        for(var round=1;round<=100;round++) if(round==100) SurfaceMarksPreviewAvailable();
        for(var round=1;round<=100;round++) if(round==100) SurfacePreviewMatchesWorkspace();
        for(var round=1;round<=100;round++) if(round==100) FaultRemovesSurfacePreview();
    }

    private static void SurfaceCarriesPreview()
    {
        var surface=CreateSurface();
        Check(surface.AcquisitionPreview is not null,
            "Inspection surface must expose the captured Acquisition preview.");
    }

    private static void SurfaceCarriesPayload()
    {
        var surface=CreateSurface();
        Check(surface.AcquisitionPreview?.Payload.Length==192,
            "Inspection surface must expose the captured payload.");
    }

    private static void SurfaceCarriesWidth()
    {
        var surface=CreateSurface();
        Check(surface.AcquisitionPreview?.Width==16,
            "Inspection surface must expose preview width.");
    }

    private static void SurfaceCarriesHeight()
    {
        var surface=CreateSurface();
        Check(surface.AcquisitionPreview?.Height==12,
            "Inspection surface must expose preview height.");
    }

    private static void SurfaceCarriesPixelFormat()
    {
        var surface=CreateSurface();
        Check(surface.AcquisitionPreview?.PixelFormat=="Gray8",
            "Inspection surface must expose preview pixel format.");
    }

    private static void SurfaceCarriesSequence()
    {
        var surface=CreateSurface();
        Check(surface.AcquisitionPreview?.Sequence.Value==1,
            "Inspection surface must expose preview sequence.");
    }

    private static void SurfaceCarriesFingerprint()
    {
        var surface=CreateSurface();
        Check(!string.IsNullOrWhiteSpace(surface.AcquisitionPreview?.PayloadFingerprint),
            "Inspection surface must expose preview payload fingerprint.");
    }

    private static void SurfaceMarksPreviewAvailable()
    {
        var surface=CreateSurface();
        Check(surface.HasAcquisitionPreview &&
              surface.AcquisitionPreviewText.Contains("16×12",StringComparison.Ordinal),
            "Inspection surface must expose coherent preview availability metadata.");
    }

    private static void SurfacePreviewMatchesWorkspace()
    {
        using var workspace=CreateWorkspace();
        workspace.BindAcquisition(new DeterministicSource(),Descriptor());
        workspace.PreviewAcquisitionAsync().AsTask().GetAwaiter().GetResult();
        var snapshot=workspace.Capture();
        var surface=ClientInspectionExecutionSurfaceRuntime.Create(snapshot);
        Check(surface.AcquisitionPreview?.PayloadFingerprint==
              snapshot.Acquisition.Preview?.PayloadFingerprint,
            "Inspection surface preview must reuse the workspace preview identity.");
    }

    private static void FaultRemovesSurfacePreview()
    {
        using var workspace=CreateWorkspace();
        workspace.BindAcquisition(new DeterministicSource(),Descriptor());
        workspace.PreviewAcquisitionAsync().AsTask().GetAwaiter().GetResult();
        workspace.Acquisition.SetFault("camera fault");
        var surface=ClientInspectionExecutionSurfaceRuntime.Create(workspace.Capture());
        Check(!surface.HasAcquisitionPreview &&
              surface.AcquisitionPreview is null,
            "Faulted Acquisition must not expose stale Preview through the Inspection surface.");
    }

    private static ClientInspectionExecutionSurface CreateSurface()
    {
        using var workspace=CreateWorkspace();
        workspace.BindAcquisition(new DeterministicSource(),Descriptor());
        workspace.PreviewAcquisitionAsync().AsTask().GetAwaiter().GetResult();
        return ClientInspectionExecutionSurfaceRuntime.Create(workspace.Capture());
    }

    private static ClientInspectionWorkspace CreateWorkspace() =>
        new(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

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
                "Inspection Acquisition preview surface smoke failed: "+message);
    }
}
