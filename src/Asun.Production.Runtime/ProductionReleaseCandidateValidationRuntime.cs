using Asun.Release.Core;

namespace Asun.Production.Runtime;

public static class ProductionReleaseCandidateValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        ReleaseManifest manifest,
        ProductionSessionDefinition definition,
        ProductionSessionReport report)
    {
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(report);

        var errors=new List<string>(
            ReleaseManifestValidationRuntime.Validate(manifest));

        errors.AddRange(
            ProductionSessionValidationRuntime.Validate(
                definition,
                report));

        if(errors.Count>0)
            return errors;

        if(manifest.Artifacts.Count!=1)
        {
            errors.Add("Production release candidate must contain exactly one logical artifact.");
            return errors;
        }

        var expected=ProductionReleaseCandidateRuntime.Create(
            manifest.Identity,
            definition,
            report,
            manifest.Artifacts[0].Path);

        if(expected.Fingerprint!=manifest.Fingerprint)
            errors.Add("Production release candidate manifest fingerprint does not match the production session.");

        return errors;
    }

    public static bool IsValid(
        ReleaseManifest manifest,
        ProductionSessionDefinition definition,
        ProductionSessionReport report)=>
        Validate(manifest,definition,report).Count==0;
}
