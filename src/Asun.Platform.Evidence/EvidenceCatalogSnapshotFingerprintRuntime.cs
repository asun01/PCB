using System.Security.Cryptography;
using System.Text;

namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotFingerprintRuntime
{
    public static string CreateFingerprint(
        EvidenceCatalogSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        if(!EvidenceCatalogSnapshotValidationRuntime.IsValid(snapshot))
            throw new ArgumentException(
                "Evidence catalog snapshot is invalid.",
                nameof(snapshot));

        var builder=new StringBuilder();

        foreach(var descriptor in snapshot.Descriptors)
        {
            var fingerprint=EvidenceDescriptorFingerprintRuntime.CreateFingerprint(descriptor);
            builder.Append(fingerprint.Length)
                .Append(':')
                .Append(fingerprint)
                .Append('|');
        }

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }
}
