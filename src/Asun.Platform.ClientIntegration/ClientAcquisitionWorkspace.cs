using Asun.Device.Contracts;

namespace Asun.Platform.ClientIntegration;

public enum ClientAcquisitionState
{
    Unbound,
    Ready,
    Faulted
}

public sealed record ClientAcquisitionDescriptor(
    string SourceId,
    string DisplayName,
    bool IsSimulation);

public sealed record ClientAcquisitionWorkspaceSnapshot(
    ClientAcquisitionState State,
    ClientAcquisitionDescriptor? Descriptor,
    string? LastError,
    bool CanCapture);

public sealed class ClientAcquisitionWorkspace
{
    private IFrameSource? _source;
    private ClientAcquisitionDescriptor? _descriptor;
    private string? _lastError;

    public ClientAcquisitionWorkspaceSnapshot Snapshot =>
        new(
            _source is null
                ? ClientAcquisitionState.Unbound
                : ClientAcquisitionState.Ready,
            _descriptor,
            _lastError,
            _source is not null);

    public event Action<ClientAcquisitionWorkspaceSnapshot>? Changed;

    public void Bind(
        IFrameSource source,
        ClientAcquisitionDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(descriptor);

        if(string.IsNullOrWhiteSpace(descriptor.SourceId))
            throw new ArgumentException("Acquisition source id cannot be blank.",nameof(descriptor));
        if(string.IsNullOrWhiteSpace(descriptor.DisplayName))
            throw new ArgumentException("Acquisition source display name cannot be blank.",nameof(descriptor));

        _source=source;
        _descriptor=descriptor;
        _lastError=null;
        Publish();
    }

    public bool TryGetSource(out IFrameSource source)
    {
        if(_source is null)
        {
            source=null!;
            return false;
        }

        source=_source;
        return true;
    }

    public void SetFault(string error)
    {
        if(string.IsNullOrWhiteSpace(error))
            throw new ArgumentException("Acquisition fault message cannot be blank.",nameof(error));

        _lastError=error.Trim();
        Publish();
    }

    public void Unbind()
    {
        _source=null;
        _descriptor=null;
        _lastError=null;
        Publish();
    }

    private void Publish() =>
        Changed?.Invoke(Snapshot);
}
