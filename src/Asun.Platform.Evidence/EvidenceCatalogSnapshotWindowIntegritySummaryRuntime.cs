using System.Security.Cryptography;
using System.Text;

namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowIntegritySummaryRuntime
{
    public static EvidenceCatalogSnapshotWindowIntegritySummary Create(
        EvidenceCatalogSnapshotWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        if(!EvidenceCatalogSnapshotWindowValidationRuntime.IsValid(window))
            throw new ArgumentException(
                "Evidence catalog snapshot window is invalid.",
                nameof(window));

        var builder=new StringBuilder();

        foreach(var entry in window.Entries)
        {
            builder.Append(entry.Sequence)
                .Append(':')
                .Append(entry.Envelope.Fingerprint)
                .Append('|');
        }

        var fingerprint=Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();

        return new EvidenceCatalogSnapshotWindowIntegritySummary(
            window.Count,
            window.Entries.Count==0 ? null : window.Entries[0].Sequence,
            window.Entries.Count==0 ? null : window.Entries[^1].Sequence,
            fingerprint);
    }
}
