namespace Asun.Platform.Evidence;

public sealed record EvidenceDescriptorQuery(
    EvidenceKind? Kind,
    string? MediaType)
{
    public bool HasCriteria=>
        Kind is not null ||
        !string.IsNullOrWhiteSpace(MediaType);
}
