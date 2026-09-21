namespace Asun.Platform.ClientIntegration;

public sealed record ClientWorkspaceClientExecutionPulse(
    long Sequence,
    ClientWorkspaceSelection Selection,
    ClientWorkspaceCommandRouting Routing,
    ClientExecutionStatus Status,
    ClientProductionProgressPresentation Progress);

public sealed class ClientWorkspaceClientProjection : IDisposable
{
    private readonly ClientWorkspaceRuntime _navigation;
    private readonly ClientInspectionWorkspace _inspection;
    private readonly Func<ClientWorkspaceSelection,ClientWorkspaceCommandRouting> _routingFactory;
    private ClientWorkspaceClientSnapshot? _snapshot;
    private long _projectionSequence;
    private long _executionPulseSequence;
    private int _disposed;

    public ClientWorkspaceClientProjection(
        ClientWorkspaceRuntime navigation,
        ClientInspectionWorkspace inspection,
        Func<ClientWorkspaceSelection,ClientWorkspaceCommandRouting> routingFactory)
    {
        ArgumentNullException.ThrowIfNull(navigation);
        ArgumentNullException.ThrowIfNull(inspection);
        ArgumentNullException.ThrowIfNull(routingFactory);

        _navigation=navigation;
        _inspection=inspection;
        _routingFactory=routingFactory;

        _navigation.Changed+=OnNavigationChanged;
        _inspection.Changed+=OnInspectionChanged;
        _inspection.ProductionChanged+=OnProductionChanged;
        _inspection.RoiChanged+=OnRoiChanged;
        _inspection.RoiPulseChanged+=OnRoiPulseChanged;

        _snapshot=CreateSnapshot(++_projectionSequence);
    }

    public event Action<ClientWorkspaceClientSnapshot>? Changed;

    public event Action<ClientWorkspaceSnapshot>? ProductionChanged;

    public event Action<ClientWorkspaceClientExecutionPulse>? ExecutionChanged;

    public event Action<RoiViewportSnapshot>? RoiChanged;

    public event Action<ClientInspectionRoiPulse>? RoiPulseChanged;

    public ClientWorkspaceClientSnapshot Snapshot
    {
        get
        {
            ThrowIfDisposed();
            return _snapshot!;
        }
    }

    public ClientWorkspaceClientSnapshot Refresh()
    {
        ThrowIfDisposed();
        Publish();
        return _snapshot!;
    }

    public void Dispose()
    {
        if(Interlocked.Exchange(ref _disposed,1)!=0)
            return;

        _navigation.Changed-=OnNavigationChanged;
        _inspection.Changed-=OnInspectionChanged;
        _inspection.ProductionChanged-=OnProductionChanged;
        _inspection.RoiChanged-=OnRoiChanged;
        _inspection.RoiPulseChanged-=OnRoiPulseChanged;
        Changed=null;
        ProductionChanged=null;
        ExecutionChanged=null;
        RoiChanged=null;
        RoiPulseChanged=null;
    }

    private ClientWorkspaceClientSnapshot CreateSnapshot(long sequence)
    {
        var selection=_navigation.Current;
        var routing=_routingFactory(selection);
        return ClientWorkspaceClientSnapshotRuntime.Create(
            selection,
            routing,
            _inspection.Capture()) with
        {
            ProjectionSequence=sequence
        };
    }

    private void OnNavigationChanged(ClientWorkspaceSelection selection) =>
        Publish();

    private void OnInspectionChanged(ClientInspectionWorkspaceSnapshot snapshot) =>
        Publish();

    private void OnProductionChanged(ClientWorkspaceSnapshot snapshot)
    {
        ProductionChanged?.Invoke(snapshot);

        var selection=_navigation.Current;
        var routing=_routingFactory(selection);
        var pulse=new ClientWorkspaceClientExecutionPulse(
            Interlocked.Increment(ref _executionPulseSequence),
            selection,
            routing,
            snapshot.Status,
            ClientProductionProgressPresentationRuntime.Create(snapshot));
        ExecutionChanged?.Invoke(pulse);
    }

    private void OnRoiChanged(RoiViewportSnapshot snapshot) =>
        RoiChanged?.Invoke(snapshot);

    private void OnRoiPulseChanged(ClientInspectionRoiPulse pulse) =>
        RoiPulseChanged?.Invoke(pulse);

    private void Publish()
    {
        if(Volatile.Read(ref _disposed)!=0)
            return;

        _snapshot=CreateSnapshot(++_projectionSequence);
        Changed?.Invoke(_snapshot);
    }

    private void ThrowIfDisposed() =>
        ObjectDisposedException.ThrowIf(_disposed!=0,this);
}
