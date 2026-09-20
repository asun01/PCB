using Asun.Platform.Evidence;
using Asun.Platform.PcbExecutionIntegration;
using Asun.Production.Runtime;

namespace Asun.Platform.PcbEvidenceResolutionIntegration;

public static class PcbExecutionEvidenceResolutionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        PcbExecutionSnapshot executionSnapshot,
        ProductionEvidenceReferenceProjection evidenceProjection,
        EvidenceCatalogSnapshot catalogSnapshot,
        PcbExecutionEvidenceResolution resolution)
    {
        ArgumentNullException.ThrowIfNull(executionSnapshot);
        ArgumentNullException.ThrowIfNull(evidenceProjection);
        ArgumentNullException.ThrowIfNull(catalogSnapshot);
        ArgumentNullException.ThrowIfNull(resolution);

        var errors=new List<string>();
        if(!EvidenceCatalogSnapshotValidationRuntime.IsValid(catalogSnapshot))
            errors.Add("Evidence catalog snapshot is invalid.");
        if(evidenceProjection.ProductionFingerprint!=executionSnapshot.ProductionFingerprint)
            errors.Add("Evidence projection production fingerprint must match execution snapshot.");
        if(resolution.ExecutionSnapshotFingerprint!=executionSnapshot.Fingerprint)
            errors.Add("Resolution execution snapshot fingerprint must match.");
        if(resolution.EvidenceProjectionFingerprint!=evidenceProjection.Fingerprint)
            errors.Add("Resolution evidence projection fingerprint must match.");

        var requested=evidenceProjection.Frames
            .SelectMany(frame=>frame.Handles)
            .Distinct()
            .OrderBy(handle=>handle.Value,StringComparer.Ordinal)
            .ToArray();
        if(!resolution.RequestedHandles.SequenceEqual(requested))
            errors.Add("Resolution requested handles must match the evidence projection.");

        var expected=EvidenceReferenceResolutionRuntime.Resolve(
            catalogSnapshot,
            new EvidenceReferenceSet(requested));
        if(!resolution.FoundHandles.SequenceEqual(expected.FoundHandles))
            errors.Add("Resolution found handles do not match the catalog snapshot.");
        if(!resolution.MissingHandles.SequenceEqual(expected.MissingHandles))
            errors.Add("Resolution missing handles do not match the catalog snapshot.");

        if(resolution.Fingerprint.Length!=64 ||
           !resolution.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Resolution fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count>0)
            return errors;

        var expectedFingerprint=PcbExecutionEvidenceResolutionRuntime.CreateFingerprint(
            executionSnapshot.Fingerprint,
            evidenceProjection.Fingerprint,
            expected);
        if(expectedFingerprint!=resolution.Fingerprint)
            errors.Add("Resolution fingerprint does not match canonical content.");

        return errors;
    }

    public static bool IsValid(
        PcbExecutionSnapshot executionSnapshot,
        ProductionEvidenceReferenceProjection evidenceProjection,
        EvidenceCatalogSnapshot catalogSnapshot,
        PcbExecutionEvidenceResolution resolution)=>
        Validate(executionSnapshot,evidenceProjection,catalogSnapshot,resolution).Count==0;
}
