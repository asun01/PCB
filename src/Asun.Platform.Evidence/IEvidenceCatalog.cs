namespace Asun.Platform.Evidence;

public interface IEvidenceCatalog
{
    ValueTask<EvidenceDescriptor?> GetAsync(
        EvidenceHandle handle,
        CancellationToken cancellationToken = default);
}
