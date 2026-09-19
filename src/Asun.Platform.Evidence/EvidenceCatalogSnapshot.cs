namespace Asun.Platform.Evidence;

public sealed class EvidenceCatalogSnapshot
{
    private readonly EvidenceDescriptor[] _descriptors;

    public EvidenceCatalogSnapshot(
        IEnumerable<EvidenceDescriptor> descriptors)
    {
        ArgumentNullException.ThrowIfNull(descriptors);

        _descriptors=descriptors.ToArray();
    }

    public int Count=>_descriptors.Length;

    public IReadOnlyList<EvidenceDescriptor> Descriptors =>
        Array.AsReadOnly(_descriptors);

    public EvidenceDescriptor? Find(EvidenceHandle handle) =>
        _descriptors.FirstOrDefault(
            descriptor=>descriptor.Handle==handle);
}
