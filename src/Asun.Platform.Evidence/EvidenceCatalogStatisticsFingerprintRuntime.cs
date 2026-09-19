using System.Security.Cryptography;
using System.Text;

namespace Asun.Platform.Evidence;

public static class EvidenceCatalogStatisticsFingerprintRuntime
{
    public static string CreateFingerprint(
        EvidenceCatalogStatistics statistics)
    {
        ArgumentNullException.ThrowIfNull(statistics);

        var builder=new StringBuilder();
        builder.Append(statistics.DescriptorCount).Append('|');

        foreach(var item in statistics.KindCounts)
        {
            builder.Append((int)item.Kind)
                .Append(':')
                .Append(item.Count)
                .Append('|');
        }

        builder.Append("|media|");

        foreach(var item in statistics.MediaTypeCounts)
        {
            builder.Append(item.MediaType.Length)
                .Append(':')
                .Append(item.MediaType)
                .Append(':')
                .Append(item.Count)
                .Append('|');
        }

        builder.Append("|bytes|")
            .Append(statistics.KnownByteLengthTotal);

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }
}
