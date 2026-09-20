using System.Numerics;
using Asun.Platform.RoiProductionIntegration;
using Asun.Production.Runtime;
using Asun.UI.Viewports;

namespace Asun.Platform.ClientIntegration;

public sealed record ClientRoiInteractionSnapshot(
    Guid ProductionSessionId,
    int RoiCount,
    Guid? SelectedRoiId,
    long ProcessedEvents,
    long RejectedEvents,
    string RoiFingerprint,
    string InteractionFingerprint);

public sealed class ClientRoiInteractionWorkspace : IDisposable
{
    private readonly ViewportRoiInputRecoveryRuntime _runtime;
    private ProductionSessionReport? _productionReport;
    private int _disposed;

    public ClientRoiInteractionWorkspace(
        Vector2 imageSize,
        Vector2 viewportSize)
    {
        _runtime=new ViewportRoiInputRecoveryRuntime(imageSize,viewportSize);
    }

    public void BindProductionReport(ProductionSessionReport productionReport)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(productionReport);

        if(productionReport.SessionId==Guid.Empty)
            throw new ArgumentException("Production session identity cannot be empty.",nameof(productionReport));

        StopAndResetInteraction();
        _productionReport=productionReport;

        if(!_runtime.TryStart(out _))
            throw new InvalidOperationException("ROI input recovery session could not be started.");
    }

    public ViewportPresentationState State
    {
        get
        {
            ThrowIfDisposed();
            return _runtime.InputRecovery.State;
        }
    }

    public RoiDocumentRuntime Document
    {
        get
        {
            ThrowIfDisposed();
            return _runtime.RoiViewport.Document;
        }
    }

    public RoiEditorMode Mode
    {
        get
        {
            ThrowIfDisposed();
            return _runtime.RoiViewport.Mode;
        }
        set
        {
            ThrowIfDisposed();
            _runtime.RoiViewport.Mode=value;
        }
    }

    public void SetImageSize(Vector2 imageSize)
    {
        ThrowIfDisposed();
        _runtime.RoiViewport.SetImageSize(imageSize);
    }

    public void ResizeViewport(Vector2 viewportSize)
    {
        ThrowIfDisposed();
        _runtime.RoiViewport.ResizeViewport(viewportSize);
    }

    public RoiViewportSnapshot CaptureViewportSnapshot()
    {
        ThrowIfDisposed();
        return _runtime.RoiViewport.CreateSnapshot();
    }

    public bool Submit(
        ViewportInputEventKind kind,
        Vector2 viewportPoint,
        int wheelDelta=0,
        ViewportMouseButton button=ViewportMouseButton.Left)
    {
        ThrowIfDisposed();
        return _runtime.TrySubmitAndProcess(kind,viewportPoint,wheelDelta,button);
    }

    public Guid AddRectangle(
        Vector2 imageCenter,
        Vector2 imageSize,
        Guid? id=null)
    {
        ThrowIfDisposed();
        return Document.Add(RoiGeometry.CreateRectangle(imageCenter,imageSize),id);
    }

    public ClientRoiInteractionSnapshot Capture()
    {
        ThrowIfDisposed();
        var report=_productionReport
            ?? throw new InvalidOperationException("A Production report must be bound before ROI capture.");

        var roiSnapshot=_runtime.Capture();
        var context=ProductionRoiInteractionContextRuntime.Create(report,roiSnapshot);

        return new ClientRoiInteractionSnapshot(
            report.SessionId,
            context.RoiCount,
            context.SelectedRoiId,
            roiSnapshot.ProcessedEvents,
            roiSnapshot.RejectedEvents,
            context.RoiFingerprint,
            context.BindingFingerprint);
    }

    public ProductionRoiInteractionContext CaptureProductionContext()
    {
        ThrowIfDisposed();
        var report=_productionReport
            ?? throw new InvalidOperationException("A Production report must be bound before ROI capture.");

        return ProductionRoiInteractionContextRuntime.Create(report,_runtime.Capture());
    }

    public void StopInteraction()
    {
        ThrowIfDisposed();
        StopAndResetInteraction();
    }

    public void Reset()
    {
        ThrowIfDisposed();
        StopAndResetInteraction();
        _productionReport=null;
    }

    public void Dispose()
    {
        if(Interlocked.Exchange(ref _disposed,1)!=0)
            return;

        _runtime.Dispose();
        _productionReport=null;
    }

    private void StopAndResetInteraction()
    {
        if(_runtime.InputRecovery.State is ViewportPresentationState.Running or ViewportPresentationState.Stopping)
            _runtime.Cancel();

        _runtime.Reset();
    }

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_disposed!=0,this);
}
