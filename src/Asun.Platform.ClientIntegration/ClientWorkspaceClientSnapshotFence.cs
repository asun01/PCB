namespace Asun.Platform.ClientIntegration;

public sealed class ClientWorkspaceClientSnapshotFence
{
    private ClientWorkspaceClientSnapshot? _current;

    public ClientWorkspaceClientSnapshot? Current => _current;

    public bool TryApply(ClientWorkspaceClientSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if(_current is not null &&
           snapshot.ProjectionSequence<=_current.ProjectionSequence)
            return false;

        _current=snapshot;
        return true;
    }

    public void Reset() => _current=null;
}
