using Asun.Production.Runtime;
using Asun.Release.Core;

namespace Asun.Platform.ReleaseIntegration;

public static class ProductionEvidenceReleaseProjectionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        ReleaseManifest manifest,
        ProductionSessionDefinition definition,
        ProductionSessionReport productionReport,
        ProductionEvidenceReferenceProjection evidenceProjection)
    {
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(evidenceProjection);

        var errors=new List<string>();
        if(!ReleaseManifestValidationRuntime.IsValid(manifest))
            errors.Add("Release manifest is invalid.");

        if(!ProductionSessionValidationRuntime.IsValid(definition,productionReport))
            errors.Add("Production report is invalid.");

        if(!ProductionEvidenceReferenceProjectionValidationRuntime.IsValid(productionReport,evidenceProjection))
            errors.Add("Evidence projection is invalid.");

        if(errors.Count>0)
            return errors;

        var productionArtifact=manifest.Artifacts.FirstOrDefault(
            artifact=>string.Equals(
                artifact.Path,
                manifest.Artifacts.Single(item=>item.Path.Contains("production",StringComparison.Ordinal)).Path,
                StringComparison.Ordinal));

        var evidenceArtifacts=manifest.Artifacts.Where(
            artifact=>artifact.Path.Contains("evidence",StringComparison.OrdinalIgnoreCase)).ToArray();

        if(evidenceArtifacts.Length!=1)
            errors.Add("Release manifest must contain exactly one evidence-reference projection artifact.");

        if(productionArtifact is null)
            errors.Add("Release manifest must contain a production artifact.");

        if(errors.Count>0)
            return errors;

        return errors;
    }

    public static bool IsValid(
        ReleaseManifest manifest,
        ProductionSessionDefinition definition,
        ProductionSessionReport productionReport,
        ProductionEvidenceReferenceProjection evidenceProjection)=>
        Validate(manifest,definition,productionReport,evidenceProjection).Count==0;
}
