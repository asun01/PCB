using System.Numerics;
using Asun.Device.Contracts;
using Asun.Release.Core;
using Asun.Production.Runtime;

namespace Asun.Platform.ClientIntegration;

public sealed record ClientInspectionWorkspaceSnapshot(
    ClientWorkspaceSnapshot Production,
    ClientRoiInteractionSnapshot? Roi,
    ClientProductionReplaySnapshot? Replay,
    ClientReleaseProjection? Release,
    ClientProductionRunHistorySnapshot History)
{
    public bool CanUndoRoi { get; init; }
    public bool CanRedoRoi { get; init; }
}

public sealed class ClientInspectionWorkspace : IDisposable
{
    private readonly ClientProductionWorkspace _production;
    private readonly ClientProductionRunHistory _history;
    private readonly ClientRoiInteractionWorkspace _roi;

    private ClientProductionReplaySnapshot? _replay;
    private ClientReleaseProjection? _release;
    private int _disposed;

    public ClientInspectionWorkspace(
        Vector2 imageSize,
        Vector2 viewportSize,
        int historyCapacity=20,
        IProductionSessionRunner? productionRunner=null)
    {
        _production=new ClientProductionWorkspace(productionRunner);
        _history=new ClientProductionRunHistory(historyCapacity);
        _roi=new ClientRoiInteractionWorkspace(imageSize,viewportSize);
    }

    public void CancelExecution()
    {
        ThrowIfDisposed();
        _production.Cancel();
    }

    public ClientWorkspaceSnapshot Production
    {
        get
        {
            ThrowIfDisposed();
            return _production.Snapshot;
        }
    }

    public ClientProductionRunHistorySnapshot History
    {
        get
        {
            ThrowIfDisposed();
            return _history.Capture();
        }
    }

    public ClientInspectionWorkspaceSnapshot Capture()
    {
        ThrowIfDisposed();

        ClientRoiInteractionSnapshot? roiSnapshot=null;
        if(Production.Status==ClientExecutionStatus.Completed)
            roiSnapshot=_roi.Capture();

        return new ClientInspectionWorkspaceSnapshot(
            _production.Snapshot,
            roiSnapshot,
            _replay,
            _release,
            _history.Capture())
        {
            CanUndoRoi=_roi.Document.CanUndo,
            CanRedoRoi=_roi.Document.CanRedo
        };
    }

    public void Load(ProductionSessionDefinition definition)
    {
        ThrowIfDisposed();
        _production.Load(definition);
        _replay=null;
        _release=null;
    }

    public async ValueTask<ProductionSessionReport> ExecuteAsync(
        IFrameSource source,
        ReleaseManifest releaseManifest,
        CancellationToken cancellationToken=default)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(releaseManifest);

        var report=await _production.StartAsync(source,cancellationToken);

        _roi.BindProductionReport(report);
        _replay=ClientProductionReplaySnapshotRuntime.Create(
            _production.Snapshot,
            report);
        _release=ClientReleaseProjectionRuntime.Create(
            _replay,
            releaseManifest);

        _history.Append(_replay,_release);
        return report;
    }

    public ClientRoiInteractionSnapshot CaptureRoi()
    {
        ThrowIfDisposed();
        if(_production.Snapshot.Status!=ClientExecutionStatus.Completed)
            throw new InvalidOperationException("ROI capture requires a completed Production client execution.");
        return _roi.Capture();
    }

    public bool CanUndoRoi
    {
        get
        {
            ThrowIfDisposed();
            return _roi.Document.CanUndo;
        }
    }

    public bool CanRedoRoi
    {
        get
        {
            ThrowIfDisposed();
            return _roi.Document.CanRedo;
        }
    }

    public bool UndoRoi()
    {
        ThrowIfDisposed();
        return _roi.Document.Undo();
    }

    public bool RedoRoi()
    {
        ThrowIfDisposed();
        return _roi.Document.Redo();
    }

    public void SetRoiMode(RoiEditorMode mode)
    {
        ThrowIfDisposed();
        _roi.Mode=mode;
    }

    public void ResizeRoiViewport(Vector2 viewportSize)
    {
        ThrowIfDisposed();
        _roi.ResizeViewport(viewportSize);
    }

    public bool SubmitRoiInput(
        ViewportInputEventKind kind,
        Vector2 viewportPoint,
        int wheelDelta=0,
        ViewportMouseButton button=ViewportMouseButton.Left)
    {
        ThrowIfDisposed();
        return _roi.Submit(kind,viewportPoint,wheelDelta,button);
    }

    public RoiViewportSnapshot CaptureRoiViewport()
    {
        ThrowIfDisposed();
        return _roi.CaptureViewportSnapshot();
    }

    public void StopRoiInteraction()
    {
        ThrowIfDisposed();
        _roi.StopInteraction();
    }

    public void ResetCurrentSession()
    {
        ThrowIfDisposed();
        _production.Reset();
        _roi.Reset();
        _replay=null;
        _release=null;
    }

    public void ResetHistory()
    {
        ThrowIfDisposed();
        _history.Reset();
    }

    public void Dispose()
    {
        if(Interlocked.Exchange(ref _disposed,1)!=0)
            return;

        _roi.Dispose();
    }

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_disposed!=0,this);
}
