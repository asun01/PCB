using System.Security.Cryptography;
using System.Text;

namespace Asun.Platform.Evidence;

public static class EvidenceCatalogQueryResultFingerprintRuntime
{
    public static string CreateFingerprint(
        EvidenceCatalogQueryResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var builder=new StringBuilder();
        builder.Append(result.SnapshotFingerprint.Length)
            .Append(':')
            .Append(result.SnapshotFingerprint)
            .Append('|');
        builder.Append(((int?)result.Query.Kind)?.ToString() ?? "null")
            .Append('|');
        builder.Append(result.Query.MediaType?.Length.ToString() ?? "null")
            .Append(':')
            .Append(result.Query.MediaType ?? string.Empty)
            .Append('|');

        foreach(var handle in result.Handles)
        {
            builder.Append(handle.Value.Length)
                .Append(':')
                .Append(handle.Value)
                .Append('|');
        }

        builder.Append(result.MatchCount);

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }
}
