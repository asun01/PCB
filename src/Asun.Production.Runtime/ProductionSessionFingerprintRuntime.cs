using System.Security.Cryptography;
using System.Text;

namespace Asun.Production.Runtime;

public static class ProductionSessionFingerprintRuntime
{
    public static string CreateFingerprint(
        ProductionSessionDefinition definition,
        IReadOnlyList<ProductionFrameExecution> frames)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(frames);

        var builder=new StringBuilder();
        builder.Append(definition.SessionId).Append('|')
            .Append(definition.ProgramPlan.Fingerprint).Append('|')
            .Append(definition.FrameCount).Append('|');

        foreach(var frame in frames)
        {
            builder.Append(frame.Sequence.Value).Append('|')
                .Append(frame.InputFingerprint).Append('|')
                .Append(frame.PipelineReport.Fingerprint).Append('|');
        }

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }
}
