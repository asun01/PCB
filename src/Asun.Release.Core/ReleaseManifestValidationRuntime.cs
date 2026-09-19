namespace Asun.Release.Core;

public static class ReleaseManifestValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        ReleaseManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        var errors=new List<string>();

        if(!manifest.Identity.IsValid)
            errors.Add("Release identity is invalid.");

        if(manifest.Artifacts.Any(artifact=>!artifact.IsValid))
            errors.Add("Release manifest contains an invalid artifact.");

        if(manifest.Artifacts.GroupBy(artifact=>artifact.Path,StringComparer.Ordinal).Any(group=>group.Count()>1))
            errors.Add("Release artifact paths must be unique.");

        if(!manifest.Artifacts.SequenceEqual(
            manifest.Artifacts.OrderBy(artifact=>artifact.Path,StringComparer.Ordinal)))
        {
            errors.Add("Release artifacts must use canonical path ordering.");
        }

        var fingerprintShapeValid=
            manifest.Fingerprint.Length==64 &&
            manifest.Fingerprint.All(character=>
                Uri.IsHexDigit(character) &&
                char.ToLowerInvariant(character)==character);

        if(!fingerprintShapeValid)
            errors.Add("Release manifest fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count>0)
            return errors;

        var expected=ReleaseManifestRuntime.Create(
            manifest.Identity,
            manifest.Artifacts);

        if(expected.Fingerprint!=manifest.Fingerprint)
            errors.Add("Release manifest fingerprint does not match the manifest.");

        return errors;
    }

    public static bool IsValid(
        ReleaseManifest manifest)=>
        Validate(manifest).Count==0;
}
