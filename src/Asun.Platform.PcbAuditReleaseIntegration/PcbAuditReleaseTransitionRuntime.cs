using System.Security.Cryptography;
using System.Text;

namespace Asun.Platform.PcbAuditReleaseIntegration;

public sealed record PcbAuditReleaseReplayDescriptor(
    string TransitionFingerprint,
    string EnvelopeFingerprint,
    Guid QualityRunId,
    string AuditWindowFingerprint,
    string ReleaseManifestFingerprint);

public static class PcbAuditReleaseTransitionRuntime
{
    public static string CreateTransitionKey(
        PcbAuditReleaseTransitionProjection projection)
    {
        ArgumentNullException.ThrowIfNull(projection);

        var canonical=string.Join(
            "|",
            projection.EnvelopeFingerprint,
            projection.QualityRunId,
            projection.AuditWindowFingerprint,
            projection.ReleaseManifestFingerprint);

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();
    }

    public static string CreateCanonicalIdentity(
        PcbAuditReleaseTransitionProjection projection)
    {
        ArgumentNullException.ThrowIfNull(projection);

        return string.Join(
            "|",
            projection.EnvelopeFingerprint,
            projection.QualityRunId,
            projection.AuditWindowFingerprint,
            projection.ReleaseManifestFingerprint,
            projection.AuditCount,
            projection.ReleaseReady,
            projection.Fingerprint);
    }

    public static bool IsEquivalent(
        PcbAuditReleaseTransitionProjection left,
        PcbAuditReleaseTransitionProjection right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        return left.Fingerprint==right.Fingerprint;
    }

    public static PcbAuditReleaseReplayDescriptor CreateReplayDescriptor(
        PcbAuditReleaseTransitionProjection projection)
    {
        ArgumentNullException.ThrowIfNull(projection);

        return new PcbAuditReleaseReplayDescriptor(
            projection.Fingerprint,
            projection.EnvelopeFingerprint,
            projection.QualityRunId,
            projection.AuditWindowFingerprint,
            projection.ReleaseManifestFingerprint);
    }
}
