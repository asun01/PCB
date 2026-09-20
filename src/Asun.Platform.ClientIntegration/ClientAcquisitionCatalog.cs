using Asun.Device.Contracts;

namespace Asun.Platform.ClientIntegration;

public sealed record ClientAcquisitionSourceDefinition(
    ClientAcquisitionDescriptor Descriptor,
    Func<IFrameSource> CreateSource);

public sealed class ClientAcquisitionCatalog
{
    private readonly Dictionary<string,ClientAcquisitionSourceDefinition> _definitions=
        new(StringComparer.Ordinal);

    public IReadOnlyList<ClientAcquisitionSourceDefinition> Sources =>
        _definitions.Values
            .OrderBy(definition=>definition.Descriptor.SourceId,StringComparer.Ordinal)
            .ToArray();

    public void Register(ClientAcquisitionSourceDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(definition.CreateSource);
        if(string.IsNullOrWhiteSpace(definition.Descriptor.SourceId))
            throw new ArgumentException("Acquisition source id cannot be blank.",nameof(definition));
        if(!_definitions.TryAdd(definition.Descriptor.SourceId,definition))
            throw new InvalidOperationException(
                $"Acquisition source '{definition.Descriptor.SourceId}' is already registered.");
    }

    public bool TryCreate(
        string sourceId,
        out IFrameSource source,
        out ClientAcquisitionDescriptor? descriptor)
    {
        if(sourceId is not null &&
           _definitions.TryGetValue(sourceId,out var definition))
        {
            source=definition.CreateSource();
            descriptor=definition.Descriptor;
            return true;
        }

        source=null!;
        descriptor=null;
        return false;
    }

    public bool Contains(string sourceId)=>
        sourceId is not null && _definitions.ContainsKey(sourceId);
}
