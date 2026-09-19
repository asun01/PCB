using System.Security.Cryptography;
using System.Text;

namespace Asun.Release.Core;

public static class ReleaseManifestRuntime
{
    public static ReleaseManifest Create(
        ReleaseIdentity identity,
        IEnumerable<ReleaseArtifact> artifacts)
    {
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(artifacts);

        if(!identity.IsValid)
            throw new ArgumentException("Release identity is invalid.",nameof(identity));

        var materialized=artifacts.ToArray();

        if(materialized.Any(artifact=>!artifact.IsValid))
            throw new ArgumentException("Release artifact list contains an invalid artifact.",nameof(artifacts));

        if(materialized.GroupBy(artifact=>artifact.Path,StringComparer.Ordinal).Any(group=>group.Count()>1))
            throw new ArgumentException("Release artifact paths must be unique.",nameof(artifacts));

        var ordered=materialized
            .OrderBy(artifact=>artifact.Path,StringComparer.Ordinal)
            .ToArray();

        var canonical=new StringBuilder();
        canonical.Append(identity.ProductName.Length).Append(':').Append(identity.ProductName).Append('|')
            .Append(identity.Version).Append('|')
            .Append(identity.Channel.Length).Append(':').Append(identity.Channel).Append('|');

        foreach(var artifact in ordered)
        {
            canonical.Append(artifact.Path.Length).Append(':').Append(artifact.Path).Append('|')
                .Append(artifact.Sha256).Append('|')
                .Append(artifact.ByteLength).Append('|');
        }

        var fingerprint=Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(canonical.ToString())))
            .ToLowerInvariant();

        return new ReleaseManifest(identity,ordered,fingerprint);
    }
}
