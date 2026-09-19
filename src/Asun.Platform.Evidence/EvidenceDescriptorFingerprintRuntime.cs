using System.Security.Cryptography;
using System.Text;

namespace Asun.Platform.Evidence;

public static class EvidenceDescriptorFingerprintRuntime
{
    public static string CreateFingerprint(
        EvidenceDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        if(!EvidenceDescriptorValidationRuntime.IsValid(descriptor))
            throw new ArgumentException(
                "Evidence descriptor is invalid.",
                nameof(descriptor));

        var builder=new StringBuilder();
        Append(builder,descriptor.Handle.Value);
        Append(builder,((int)descriptor.Kind).ToString());
        Append(builder,descriptor.MediaType);
        Append(builder,descriptor.ByteLength?.ToString() ?? "null");
        Append(builder,descriptor.DisplayName ?? "null");

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }

    private static void Append(StringBuilder builder,string value)
    {
        builder.Append(value.Length).Append(':').Append(value).Append('|');
    }
}
