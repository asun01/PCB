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

public sealed class ClientWorkspaceRuntime
{
    private long _transitionSequence;
    private ClientWorkspaceKind _workspace=ClientWorkspaceKind.Home;

    public ClientWorkspaceSelection Current =>
        new(_workspace,_transitionSequence);

    public event Action<ClientWorkspaceSelection>? Changed;

    public bool TryNavigate(ClientWorkspaceKind workspace)
    {
        if(!Enum.IsDefined(workspace) || workspace==_workspace)
            return false;

        _workspace=workspace;
        var selection=new ClientWorkspaceSelection(
            _workspace,
            Interlocked.Increment(ref _transitionSequence));

        Changed?.Invoke(selection);
        return true;
    }

    public void Reset()
    {
        _workspace=ClientWorkspaceKind.Home;
        Changed?.Invoke(new ClientWorkspaceSelection(
            _workspace,
            Interlocked.Increment(ref _transitionSequence)));
    }
}
