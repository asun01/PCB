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
    public ClientProgramWorkspaceSnapshot? Program { get; init; }
    public ClientQualityWorkspaceSnapshot Quality { get; init; }=new(
        null,0,0,0,0,0,0,null,false,
        Array.Empty<ClientQualityFindingDisplayItem>());
    public ClientAcquisitionWorkspaceSnapshot Acquisition { get; init; }=new(
        ClientAcquisitionState.Unbound,
        null,
        null,
        false);
}

public sealed class ClientInspectionWorkspace : IDisposable
{
    private readonly ClientProgramWorkspace _program;
    private readonly ClientProductionWorkspace _production;
    private readonly ClientProductionRunHistory _history;
    private readonly ClientRoiInteractionWorkspace _roi;
    private readonly ClientQualityWorkspace _quality;
    private readonly ClientAcquisitionWorkspace _acquisition;
    private readonly ClientAcquisitionCatalog _acquisitionCatalog;
    private readonly ClientQualityProviderCatalog _qualityProviderCatalog;

    private ProductionSessionReport? _lastProductionReport;
    private ClientProductionReplaySnapshot? _replay;
    private ClientReleaseProjection? _release;
    private long? _selectedHistoryOrdinal;
    private Action<ClientInspectionWorkspaceSnapshot>? _changed;
    private int _disposed;

    public ClientInspectionWorkspace(
        Vector2 imageSize,
        Vector2 viewportSize,
        int historyCapacity=20,
        IProductionSessionRunner? productionRunner=null)
    {
        _program=new ClientProgramWorkspace();
        _production=new ClientProductionWorkspace(productionRunner);
        _history=new ClientProductionRunHistory(historyCapacity);
        _roi=new ClientRoiInteractionWorkspace(imageSize,viewportSize);
        _quality=new ClientQualityWorkspace();
        _acquisition=new ClientAcquisitionWorkspace();
        _acquisitionCatalog=new ClientAcquisitionCatalog();
        _qualityProviderCatalog=new ClientQualityProviderCatalog();
        _production.Changed+=OnProductionChanged;
    }

    public void CancelExecution()
    {
        ThrowIfDisposed();
        _production.Cancel();
    }

    public event Action<ClientWorkspaceSnapshot>? ProductionChanged
    {
        add
        {
            ThrowIfDisposed();
            _production.Changed+=value;
        }
        remove
        {
            if(Volatile.Read(ref _disposed)!=0)
                return;
            _production.Changed-=value;
        }
    }

    public event Action<ClientInspectionWorkspaceSnapshot>? Changed
    {
        add
        {
            ThrowIfDisposed();
            _changed+=value;
        }
        remove
        {
            if(Volatile.Read(ref _disposed)!=0)
                return;
            _changed-=value;
        }
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

    public long? SelectedHistoryOrdinal
    {
        get
        {
            ThrowIfDisposed();
            return _selectedHistoryOrdinal;
        }
    }

    public ClientProductionRunHistoryEntry? SelectedHistory
    {
        get
        {
            ThrowIfDisposed();
            var ordinal=_selectedHistoryOrdinal;
            return ordinal is null
                ? null
                : _history.Capture().Entries.FirstOrDefault(entry=>entry.Ordinal==ordinal.Value);
        }
    }

    public bool SelectHistory(long ordinal)
    {
        ThrowIfDisposed();
        var entry=_history.Capture().Entries.FirstOrDefault(item=>item.Ordinal==ordinal);
        if(entry is null)
            return false;

        _selectedHistoryOrdinal=ordinal;
        PublishChanged();
        return true;
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
            CanRedoRoi=_roi.Document.CanRedo,
            Program=_program.Snapshot,
            Quality=_quality.Capture(),
            Acquisition=_acquisition.Snapshot
        };
    }

    public ClientProgramWorkspaceSnapshot LoadProgram(
        Asun.Program.Core.InspectionProgram program,
        Asun.Platform.Pipeline.PipelineDefinition<Asun.Device.Contracts.CapturedFrame> pipeline,
        Guid sessionId,
        int frameCount)
    {
        ThrowIfDisposed();
        var snapshot=_program.Load(program);
        if(snapshot.Status==ClientProgramLoadStatus.Ready)
        {
            var definition=_program.CreateSessionDefinition(sessionId,pipeline,frameCount);
            _production.Load(definition);
            _quality.Clear();
            _replay=null;
            _release=null;
        }

        return snapshot;
    }

    public ClientProgramWorkspaceSnapshot Program
    {
        get
        {
            ThrowIfDisposed();
            return _program.Snapshot;
        }
    }

    public Asun.Program.Core.InspectionProgram? CurrentProgram
    {
        get
        {
            ThrowIfDisposed();
            return _program.CurrentProgram;
        }
    }

    public ClientQualityWorkspaceSnapshot Quality
    {
        get
        {
            ThrowIfDisposed();
            return _quality.Capture();
        }
    }

    public ClientAcquisitionWorkspaceSnapshot Acquisition
    {
        get
        {
            ThrowIfDisposed();
            return _acquisition.Snapshot;
        }
    }

    public ClientQualityProviderCatalog QualityProviderCatalog
    {
        get
        {
            ThrowIfDisposed();
            return _qualityProviderCatalog;
        }
    }

    public ClientAcquisitionCatalog AcquisitionCatalog
    {
        get
        {
            ThrowIfDisposed();
            return _acquisitionCatalog;
        }
    }

    public async ValueTask<ClientAcquisitionPreviewSnapshot> PreviewAcquisitionAsync(
        CancellationToken cancellationToken=default)
    {
        ThrowIfDisposed();
        var preview=await _acquisition.PreviewAsync(cancellationToken);
        PublishChanged();
        return preview;
    }

    public ProductionSessionReport? LastProductionReport
    {
        get
        {
            ThrowIfDisposed();
            return _lastProductionReport;
        }
    }

    public bool BindAcquisitionSource(string sourceId)
    {
        ThrowIfDisposed();
        if(!_acquisitionCatalog.TryCreate(
            sourceId,
            out var source,
            out var descriptor) ||
           descriptor is null)
            return false;

        _acquisition.Bind(source,descriptor);
        PublishChanged();
        return true;
    }

    public void BindAcquisition(
        IFrameSource source,
        ClientAcquisitionDescriptor descriptor)
    {
        ThrowIfDisposed();
        _acquisition.Bind(source,descriptor);
        PublishChanged();
    }

    public void BindQualityRun(Asun.Domain.Quality.QualityInspectionRun run)
    {
        ThrowIfDisposed();
        _quality.Bind(run);
        PublishChanged();
    }

    public ClientQualityWorkspaceSnapshot EvaluateQualityProvider(
        string providerId)
    {
        ThrowIfDisposed();
        if(!_qualityProviderCatalog.TryCreate(providerId,out var provider))
            throw new ArgumentException(
                $"Quality provider '{providerId}' is not registered.",
                nameof(providerId));

        return EvaluateQuality(provider);
    }

    public ClientQualityWorkspaceSnapshot EvaluateQuality(
        IClientQualityRunProvider provider)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(provider);

        var report=_lastProductionReport
            ?? throw new InvalidOperationException(
                "A completed Production report is required before Quality evaluation.");

        if(_production.Snapshot.Status!=ClientExecutionStatus.Completed)
            throw new InvalidOperationException(
                "Quality evaluation requires a completed Production session.");

        var run=provider.Create(report);
        _quality.Bind(run,provider.Descriptor);
        PublishChanged();
        return _quality.Capture();
    }

    public bool SelectQualityFinding(string findingId)
    {
        ThrowIfDisposed();
        var selected=_quality.SelectFinding(findingId);
        if(selected)
            PublishChanged();

        return selected;
    }

    public void ClearQualityRun()
    {
        ThrowIfDisposed();
        _quality.Clear();
        PublishChanged();
    }

    public void Load(ProductionSessionDefinition definition)
    {
        ThrowIfDisposed();
        _production.Load(definition);
        _lastProductionReport=null;
        _quality.Clear();
        _acquisition.Unbind();
        _replay=null;
        _release=null;
    }

    public async ValueTask<ProductionSessionReport> ExecuteAsync(
        ReleaseManifest releaseManifest,
        CancellationToken cancellationToken=default)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(releaseManifest);

        if(!_acquisition.TryGetSource(out var source))
            throw new InvalidOperationException("A Ready Acquisition source must be bound before Production execution.");

        _replay=null;
        _release=null;
        _quality.Clear();
        _selectedHistoryOrdinal=null;

        var report=await _production.StartAsync(source,cancellationToken);

        _lastProductionReport=report;
        _roi.BindProductionReport(report);

        var production=_production.Snapshot;
        if(production.LastFrameWidth>0 &&
           production.LastFrameHeight>0)
        {
            _roi.SetImageSize(new Vector2(
                (float)production.LastFrameWidth,
                (float)production.LastFrameHeight));
        }
        _replay=ClientProductionReplaySnapshotRuntime.Create(
            _production.Snapshot,
            report);
        _release=ClientReleaseProjectionRuntime.Create(
            _replay,
            releaseManifest);

        var historyEntry=_history.Append(_replay,_release);
        _selectedHistoryOrdinal=historyEntry.Ordinal;
        PublishChanged();
        return report;
    }

    public ValueTask<ProductionSessionReport> ExecuteAsync(
        IFrameSource source,
        ReleaseManifest releaseManifest,
        CancellationToken cancellationToken=default)
    {
        ThrowIfDisposed();
        _acquisition.Bind(
            source,
            new ClientAcquisitionDescriptor(
                "runtime-source",
                "Bound runtime source",
                false));

        return ExecuteAsync(releaseManifest,cancellationToken);
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
        _program.Reset();
        _production.Reset();
        _lastProductionReport=null;
        _roi.Reset();
        _quality.Clear();
        _acquisition.Unbind();
        _replay=null;
        _release=null;
        _selectedHistoryOrdinal=null;
    }

    public void ResetHistory()
    {
        ThrowIfDisposed();
        _history.Reset();
        _selectedHistoryOrdinal=null;
        PublishChanged();
    }

    public void Dispose()
    {
        if(Interlocked.Exchange(ref _disposed,1)!=0)
            return;

        _production.Changed-=OnProductionChanged;
        _changed=null;
        _roi.Dispose();
    }

    private void OnProductionChanged(ClientWorkspaceSnapshot snapshot)
    {
        ProductionChanged?.Invoke(snapshot);
        _changed?.Invoke(Capture());
    }

    private void PublishChanged()
    {
        ProductionChanged?.Invoke(_production.Snapshot);
        Changed?.Invoke(Capture());
    }

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_disposed!=0,this);
}
