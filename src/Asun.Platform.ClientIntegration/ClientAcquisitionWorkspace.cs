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

public sealed record ClientAcquisitionPreviewSnapshot(
    FrameSequence Sequence,
    long Width,
    long Height,
    string PixelFormat,
    DateTimeOffset CapturedAtUtc,
    string PayloadFingerprint,
    byte[] Payload);

public sealed record ClientAcquisitionWorkspaceSnapshot(
    ClientAcquisitionState State,
    ClientAcquisitionDescriptor? Descriptor,
    string? LastError,
    bool CanCapture)
{
    public ClientAcquisitionPreviewSnapshot? Preview { get; init; }
};

public sealed class ClientAcquisitionWorkspace
{
    private IFrameSource? _source;
    private ClientAcquisitionDescriptor? _descriptor;
    private string? _lastError;
    private ClientAcquisitionPreviewSnapshot? _preview;

    public ClientAcquisitionWorkspaceSnapshot Snapshot =>
        new(
            _source is null
                ? ClientAcquisitionState.Unbound
                : _lastError is null
                    ? ClientAcquisitionState.Ready
                    : ClientAcquisitionState.Faulted,
            _descriptor,
            _lastError,
            _source is not null && _lastError is null)
        {
            Preview=_preview
        };

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
        _preview=null;
        Publish();
    }

    public async ValueTask<ClientAcquisitionPreviewSnapshot> PreviewAsync(
        CancellationToken cancellationToken=default)
    {
        if(_source is null)
            throw new InvalidOperationException("An Acquisition source must be bound before preview.");

        try
        {
            var frame=await _source.CaptureAsync(cancellationToken)
                ?? throw new InvalidOperationException("Acquisition source returned no preview frame.");

            if(!CapturedFrameValidationRuntime.IsValid(frame))
                throw new InvalidOperationException("Acquisition source returned an invalid preview frame.");

            _preview=new ClientAcquisitionPreviewSnapshot(
                frame.Metadata.Sequence,
                frame.Metadata.Width,
                frame.Metadata.Height,
                frame.Metadata.PixelFormat,
                frame.Metadata.CapturedAtUtc,
                frame.PayloadFingerprint,
                frame.Payload.ToArray());

            _lastError=null;
            Publish();
            return _preview;
        }
        catch(OperationCanceledException)
        {
            throw;
        }
        catch(Exception exception)
        {
            SetFault(exception.Message);
            throw;
        }
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
        _preview=null;
        Publish();
    }

    private void Publish() =>
        Changed?.Invoke(Snapshot);
}
