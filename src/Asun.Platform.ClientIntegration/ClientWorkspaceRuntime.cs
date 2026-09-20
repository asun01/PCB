namespace Asun.Platform.ClientIntegration;

public enum ClientWorkspaceKind
{
    Home,
    Inspection,
    Program,
    Quality,
    Results
}

public readonly record struct ClientWorkspaceSelection(
    ClientWorkspaceKind Workspace,
    long TransitionSequence);

public sealed class ClientWorkspaceRuntime : IDisposable
{
    private long _transitionSequence;
    private ClientWorkspaceKind _workspace=ClientWorkspaceKind.Home;
    private int _disposed;

    public ClientWorkspaceSelection Current =>
        new(_workspace,_transitionSequence);

    public event Action<ClientWorkspaceSelection>? Changed;

    public bool TryNavigate(ClientWorkspaceKind workspace)
    {
        if(Volatile.Read(ref _disposed)!=0)
            return false;

        if(!Enum.IsDefined(workspace) || workspace==_workspace)
            return false;

        _workspace=workspace;
        Publish();
        return true;
    }

    public void Reset()
    {
        if(Volatile.Read(ref _disposed)!=0)
            return;

        _workspace=ClientWorkspaceKind.Home;
        Publish();
    }

    public void Dispose()
    {
        if(Interlocked.Exchange(ref _disposed,1)!=0)
            return;

        Changed=null;
    }

    private void Publish()
    {
        var selection=new ClientWorkspaceSelection(
            _workspace,
            Interlocked.Increment(ref _transitionSequence));

        Changed?.Invoke(selection);
    }
}
