namespace Asun.Platform.ClientIntegration;

public sealed record ClientQualityProviderDefinition(
    ClientQualityProviderDescriptor Descriptor,
    Func<IClientQualityRunProvider> CreateProvider);

public sealed class ClientQualityProviderCatalog
{
    private readonly Dictionary<string,ClientQualityProviderDefinition> _definitions=
        new(StringComparer.Ordinal);

    public IReadOnlyList<ClientQualityProviderDefinition> Providers =>
        _definitions.Values
            .OrderBy(definition=>definition.Descriptor.ProviderId,StringComparer.Ordinal)
            .ToArray();

    public void Register(ClientQualityProviderDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(definition.CreateProvider);

        if(string.IsNullOrWhiteSpace(definition.Descriptor.ProviderId))
            throw new ArgumentException("Quality provider id cannot be blank.",nameof(definition));

        if(string.IsNullOrWhiteSpace(definition.Descriptor.DisplayName))
            throw new ArgumentException("Quality provider display name cannot be blank.",nameof(definition));

        if(!_definitions.TryAdd(definition.Descriptor.ProviderId,definition))
            throw new InvalidOperationException(
                $"Quality provider '{definition.Descriptor.ProviderId}' is already registered.");
    }

    public bool TryCreate(
        string providerId,
        out IClientQualityRunProvider provider)
    {
        if(providerId is not null &&
           _definitions.TryGetValue(providerId,out var definition))
        {
            provider=definition.CreateProvider();
            return true;
        }

        provider=null!;
        return false;
    }
}
