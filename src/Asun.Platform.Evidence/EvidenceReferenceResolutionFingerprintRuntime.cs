using System.Security.Cryptography;
using System.Text;

namespace Asun.Platform.Evidence;

public static class EvidenceReferenceResolutionFingerprintRuntime
{
    public static string CreateFingerprint(
        EvidenceReferenceResolution resolution)
    {
        ArgumentNullException.ThrowIfNull(resolution);

        var builder=new StringBuilder();
        builder.Append(resolution.SnapshotFingerprint.Length)
            .Append(':')
            .Append(resolution.SnapshotFingerprint)
            .Append("|found|");

        foreach(var handle in resolution.FoundHandles)
        {
            builder.Append(handle.Value.Length)
                .Append(':')
                .Append(handle.Value)
                .Append('|');
        }

        builder.Append("|missing|");

        foreach(var handle in resolution.MissingHandles)
        {
            builder.Append(handle.Value.Length)
                .Append(':')
                .Append(handle.Value)
                .Append('|');
        }

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }
}
