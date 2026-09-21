namespace Asun.Platform.ClientIntegration;

public sealed class ClientWorkspaceClientSnapshotFence
{
    private ClientWorkspaceClientSnapshot? _current;
    private long _lastAcceptedSequence;

    public ClientWorkspaceClientSnapshot? Current => _current;

    public long LastAcceptedSequence => _lastAcceptedSequence;

    public bool TryApply(ClientWorkspaceClientSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if(snapshot.ProjectionSequence<=_lastAcceptedSequence)
            return false;

        _current=snapshot;
        _lastAcceptedSequence=snapshot.ProjectionSequence;
        return true;
    }

    public void Reset() => _current=null;
}
