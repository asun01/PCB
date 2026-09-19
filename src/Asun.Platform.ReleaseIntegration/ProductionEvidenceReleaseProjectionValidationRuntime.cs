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

        var productionArtifacts=manifest.Artifacts.Where(
            artifact=>artifact.Path.Contains("production",StringComparison.OrdinalIgnoreCase)).ToArray();
        var evidenceArtifacts=manifest.Artifacts.Where(
            artifact=>artifact.Path.Contains("evidence",StringComparison.OrdinalIgnoreCase)).ToArray();

        if(productionArtifacts.Length!=1)
            errors.Add("Release manifest must contain exactly one production artifact.");

        if(evidenceArtifacts.Length!=1)
            errors.Add("Release manifest must contain exactly one evidence-reference projection artifact.");

        return errors;
    }

    public static bool IsValid(
        ReleaseManifest manifest,
        ProductionSessionDefinition definition,
        ProductionSessionReport productionReport,
        ProductionEvidenceReferenceProjection evidenceProjection)=>
        Validate(manifest,definition,productionReport,evidenceProjection).Count==0;
}
