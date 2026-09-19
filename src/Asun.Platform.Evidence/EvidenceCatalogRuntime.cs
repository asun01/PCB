namespace Asun.Platform.Evidence;

public static class EvidenceCatalogRuntime
{
    public static async ValueTask<EvidenceDescriptor?> GetValidatedAsync(
        IEvidenceCatalog catalog,
        EvidenceHandle handle,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        if(!handle.IsValid)
            throw new ArgumentException(
                "Evidence handle must be valid.",
                nameof(handle));

        var descriptor=await catalog.GetAsync(handle,cancellationToken);

        if(descriptor is null)
            return null;

        if(descriptor.Handle!=handle)
            throw new InvalidOperationException(
                "Evidence catalog returned a descriptor for a different handle.");

        if(!EvidenceDescriptorValidationRuntime.IsValid(descriptor))
            throw new InvalidOperationException(
                "Evidence catalog returned an invalid descriptor.");

        return descriptor;
    }
}
