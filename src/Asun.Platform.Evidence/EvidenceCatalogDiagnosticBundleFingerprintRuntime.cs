using System.Security.Cryptography;
using System.Text;

namespace Asun.Platform.Evidence;

public static class EvidenceCatalogDiagnosticBundleFingerprintRuntime
{
    public static string CreateFingerprint(
        EvidenceCatalogDiagnosticBundle bundle)
    {
        ArgumentNullException.ThrowIfNull(bundle);

        var builder=new StringBuilder();
        var statisticsFingerprint=
            EvidenceCatalogStatisticsFingerprintRuntime.CreateFingerprint(
                bundle.Statistics);
        var queryBatchFingerprint=
            EvidenceCatalogQueryBatchFingerprintRuntime.CreateFingerprint(
                bundle.QueryBatch);
        var referenceFingerprint=
            EvidenceReferenceResolutionFingerprintRuntime.CreateFingerprint(
                bundle.ReferenceResolution);

        foreach(var value in new[]{
            bundle.SnapshotFingerprint,
            statisticsFingerprint,
            queryBatchFingerprint,
            referenceFingerprint})
        {
            builder.Append(value.Length)
                .Append(':')
                .Append(value)
                .Append('|');
        }

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }
}
