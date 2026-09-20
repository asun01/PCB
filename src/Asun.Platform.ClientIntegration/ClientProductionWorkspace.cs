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

public sealed class ProductionSessionRuntimeAdapter : IProductionSessionRunner
{
    public ValueTask<ProductionSessionReport> RunAsync(
        ProductionSessionDefinition definition,
        IFrameSource source,
        CancellationToken cancellationToken=default)=>
        ProductionSessionRuntime.RunAsync(definition,source,cancellationToken);
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
    string? LastError);

public sealed class ClientProductionWorkspace
{
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
            null);
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
        _snapshot=_snapshot with { Status=ClientExecutionStatus.Running,LastError=null };

        try
        {
            var report=await _runner.RunAsync(_definition,source,linked.Token);
            _snapshot=_snapshot with
            {
                Status=ClientExecutionStatus.Completed,
                LastFrameCount=report.FrameCount,
                LastReportFingerprint=report.Fingerprint,
                LastError=null
            };
            return report;
        }
        catch(OperationCanceledException)
        {
            _snapshot=_snapshot with { Status=ClientExecutionStatus.Cancelled };
            throw;
        }
        catch(Exception exception)
        {
            _snapshot=_snapshot with
            {
                Status=ClientExecutionStatus.Failed,
                LastError=exception.Message
            };
            throw;
        }
        finally
        {
            _activeCancellation=null;
        }
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
            null);
    }
}
