using System.Security.Cryptography;
using System.Text;

namespace Asun.Platform.Evidence;

public static class EvidenceCatalogWindowDiagnosticBundleFingerprintRuntime
{
    public static string CreateFingerprint(
        EvidenceCatalogWindowDiagnosticBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(bundle);

        var queryFingerprint=
            EvidenceCatalogSnapshotWindowQueryFingerprintRuntime.CreateFingerprint(
                bundle.QueryResultSet);

        var builder=new StringBuilder();
        foreach(var value in new[]{bundle.WindowFingerprint,queryFingerprint})
        {
            builder.Append(value.Length)
                .Append(':')
                .Append(value)
                .Append('|');
        }

        builder.Append(bundle.WindowSummary.EntryCount)
            .Append('|')
            .Append(bundle.WindowSummary.FirstSequence?.ToString() ?? "null")
            .Append('|')
            .Append(bundle.WindowSummary.LastSequence?.ToString() ?? "null");

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }
}
