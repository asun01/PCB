using Asun.Device.Contracts;
using Asun.Production.Runtime;

namespace Asun.Platform.ClientIntegration;

public interface IProductionSessionRunner
{
    ValueTask<ProductionSessionReport> RunAsync(
        ProductionSessionDefinition definition,
        IFrameSource source,
        CancellationToken cancellationToken=default);
}

public interface IProductionSessionProgressRunner
{
    ValueTask<ProductionSessionReport> RunAsync(
        ProductionSessionDefinition definition,
        IFrameSource source,
        IProgress<ProductionSessionProgress> progress,
        CancellationToken cancellationToken=default);
}

public sealed class ProductionSessionRuntimeAdapter :
    IProductionSessionRunner,
    IProductionSessionProgressRunner
{
    public ValueTask<ProductionSessionReport> RunAsync(
        ProductionSessionDefinition definition,
        IFrameSource source,
        CancellationToken cancellationToken=default)=>
        ProductionSessionRuntime.RunAsync(definition,source,cancellationToken);

    public ValueTask<ProductionSessionReport> RunAsync(
        ProductionSessionDefinition definition,
        IFrameSource source,
        IProgress<ProductionSessionProgress> progress,
        CancellationToken cancellationToken=default)=>
        ProductionSessionRuntime.RunAsync(definition,source,cancellationToken,progress);
}

public enum ClientExecutionStatus
{
    Idle,
    Ready,
    Running,
    Completed,
    Cancelled,
    Failed
}

public sealed record ClientWorkspaceSnapshot(
    Guid? ProgramId,
    Version? ProgramVersion,
    Guid? ActiveSessionId,
    ClientExecutionStatus Status,
    int LastFrameCount,
    string? LastReportFingerprint,
    string? LastError)
{
    public int TargetFrameCount { get; init; }
    public int FramesProcessed { get; init; }
    public FrameSequence? LastSequence { get; init; }
    public long LastFrameWidth { get; init; }
    public long LastFrameHeight { get; init; }
    public string LastPixelFormat { get; init; }="";
};

public sealed class ClientProductionWorkspace
{
    private sealed class InlineProgress<T> : IProgress<T>
    {
        private readonly Action<T> _handler;

        public InlineProgress(Action<T> handler)
        {
            _handler=handler;
        }

        public void Report(T value) => _handler(value);
    }
    private readonly IProductionSessionRunner _runner;
    private ProductionSessionDefinition? _definition;
    private CancellationTokenSource? _activeCancellation;
    private ClientWorkspaceSnapshot _snapshot=new(
        null,
        null,
        null,
        ClientExecutionStatus.Idle,
        0,
        null,
        null);

    public ClientProductionWorkspace(IProductionSessionRunner? runner=null)
    {
        _runner=runner ?? new ProductionSessionRuntimeAdapter();
    }

    public ClientWorkspaceSnapshot Snapshot=>_snapshot;

    public event Action<ClientWorkspaceSnapshot>? Changed;

    public event Action<ClientWorkspaceSnapshot>? Changed;

    private void Publish() => Changed?.Invoke(_snapshot);

    public void Load(ProductionSessionDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        if(definition.SessionId==Guid.Empty)
            throw new ArgumentException("Client session identity cannot be empty.",nameof(definition));

        _definition=definition;
        _snapshot=new ClientWorkspaceSnapshot(
            definition.ProgramPlan.ProgramId,
            definition.ProgramPlan.Version,
            definition.SessionId,
            ClientExecutionStatus.Ready,
            0,
            null,
            null)
        {
            TargetFrameCount=definition.FrameCount,
            FramesProcessed=0,
            LastSequence=null
        };
        PublishChanged();
        Publish();
    }

    public async ValueTask<ProductionSessionReport> StartAsync(
        IFrameSource source,
        CancellationToken cancellationToken=default)
    {
        ArgumentNullException.ThrowIfNull(source);
        if(_definition is null)
            throw new InvalidOperationException("A production session must be loaded before start.");
        if(_snapshot.Status==ClientExecutionStatus.Running)
            throw new InvalidOperationException("A client production session is already running.");

        using var linked=CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _activeCancellation=linked;
        _snapshot=_snapshot with
        {
            Status=ClientExecutionStatus.Running,
            LastFrameCount=0,
            LastReportFingerprint=null,
            LastError=null,
            FramesProcessed=0,
            LastSequence=null
        };
        Publish();

        try
        {
            var progress=new InlineProgress<ProductionSessionProgress>(UpdateProgress);

            var report= _runner is IProductionSessionProgressRunner progressRunner
                ? await progressRunner.RunAsync(_definition,source,progress,linked.Token)
                : await _runner.RunAsync(_definition,source,linked.Token);
            _snapshot=_snapshot with
            {
                Status=ClientExecutionStatus.Completed,
                LastFrameCount=report.FrameCount,
                LastReportFingerprint=report.Fingerprint,
                LastError=null,
                TargetFrameCount=report.FrameCount,
                FramesProcessed=report.FrameCount,
                LastSequence=report.Frames.Count==0
                    ? null
                    : report.Frames.MaxBy(frame=>frame.Sequence.Value)!.Sequence
            };
            Publish();
            return report;
        }
        catch(OperationCanceledException)
        {
            _snapshot=_snapshot with { Status=ClientExecutionStatus.Cancelled };
            Publish();
            throw;
        }
        catch(Exception exception)
        {
            _snapshot=_snapshot with
            {
                Status=ClientExecutionStatus.Failed,
                LastError=exception.Message
            };
            Publish();
            throw;
        }
        finally
        {
            _activeCancellation=null;
        }
    }

    private void UpdateProgress(ProductionSessionProgress progress)
    {
        if(progress.SessionId!=_snapshot.ActiveSessionId)
            return;

        _snapshot=_snapshot with
        {
            FramesProcessed=progress.CompletedFrames,
            TargetFrameCount=progress.TotalFrames,
            LastSequence=progress.LastSequence
        };
        PublishChanged();
        Publish();
    }

    public void Cancel()=>
        _activeCancellation?.Cancel();

    public void Reset()
    {
        _activeCancellation?.Cancel();
        _activeCancellation=null;
        _definition=null;
        _snapshot=new ClientWorkspaceSnapshot(
            null,
            null,
            null,
            ClientExecutionStatus.Idle,
            0,
            null,
            null)
        {
            TargetFrameCount=0,
            FramesProcessed=0,
            LastSequence=null,
            LastFrameWidth=0,
            LastFrameHeight=0,
            LastPixelFormat=""
        };
        Publish();
    }
}
