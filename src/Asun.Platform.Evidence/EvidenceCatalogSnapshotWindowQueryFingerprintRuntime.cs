using System.Security.Cryptography;
using System.Text;

namespace Asun.Platform.Evidence;

public static class EvidenceCatalogSnapshotWindowQueryFingerprintRuntime
{
    public static string CreateFingerprint(
        EvidenceCatalogSnapshotWindowQueryResultSet resultSet)
    {
        ArgumentNullException.ThrowIfNull(resultSet);

        var builder=new StringBuilder();
        builder.Append(resultSet.WindowFingerprint.Length)
            .Append(':')
            .Append(resultSet.WindowFingerprint)
            .Append('|');
        builder.Append(((int?)resultSet.Query.Kind)?.ToString() ?? "null")
            .Append('|');
        builder.Append(resultSet.Query.MediaType?.Length.ToString() ?? "null")
            .Append(':')
            .Append(resultSet.Query.MediaType ?? string.Empty)
            .Append('|');

        foreach(var item in resultSet.Results)
        {
            var resultFingerprint=
                EvidenceCatalogQueryResultFingerprintRuntime.CreateFingerprint(item.Result);
            builder.Append(item.Sequence)
                .Append(':')
                .Append(resultFingerprint.Length)
                .Append(':')
                .Append(resultFingerprint)
                .Append('|');
        }

        builder.Append(resultSet.EntryCount);

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }
}
