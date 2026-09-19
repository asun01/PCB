using System.Security.Cryptography;
using System.Text;

namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowReferenceResolutionFingerprintRuntime
{
    public static string CreateFingerprint(
        EvidenceCatalogSnapshotWindowReferenceResolution result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var builder=new StringBuilder();
        builder.Append(result.WindowFingerprint.Length)
            .Append(':')
            .Append(result.WindowFingerprint)
            .Append("|refs|");

        foreach(var handle in result.ReferenceSet.Handles)
        {
            builder.Append(handle.Value.Length)
                .Append(':')
                .Append(handle.Value)
                .Append('|');
        }

        builder.Append("|entries|");

        foreach(var entry in result.Entries)
        {
            var resolutionFingerprint=
                EvidenceReferenceResolutionFingerprintRuntime.CreateFingerprint(
                    entry.Resolution);

            builder.Append(entry.Sequence)
                .Append(':')
                .Append(resolutionFingerprint.Length)
                .Append(':')
                .Append(resolutionFingerprint)
                .Append('|');
        }

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }
}
