using System.Security.Cryptography;
using System.Text;

namespace Asun.Platform.Evidence;

public static class EvidenceCatalogQueryBatchFingerprintRuntime
{
    public static string CreateFingerprint(
        EvidenceCatalogQueryBatch batch)
    {
        ArgumentNullException.ThrowIfNull(batch);

        var builder=new StringBuilder();
        builder.Append(batch.SnapshotFingerprint.Length)
            .Append(':')
            .Append(batch.SnapshotFingerprint)
            .Append('|');

        foreach(var result in batch.Results)
        {
            var fingerprint=EvidenceCatalogQueryResultFingerprintRuntime.CreateFingerprint(result);
            builder.Append(fingerprint.Length)
                .Append(':')
                .Append(fingerprint)
                .Append('|');
        }

        builder.Append(batch.QueryCount);

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }
}
