using System.Security.Cryptography;
using System.Text;
using Asun.Release.Core;

namespace Asun.Production.Runtime;

public static class ProductionReleaseCandidateRuntime
{
    public static ReleaseManifest Create(
        ReleaseIdentity identity,
        ProductionSessionDefinition definition,
        ProductionSessionReport report,
        string artifactPath)
    {
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(report);

        if(!ProductionSessionValidationRuntime.IsValid(definition,report))
            throw new ArgumentException("Production session report is invalid.",nameof(report));

        if(string.IsNullOrWhiteSpace(artifactPath))
            throw new ArgumentException("Release artifact path cannot be blank.",nameof(artifactPath));

        var canonical=string.Join(
            "|",
            new[]
            {
                report.SessionId.ToString(),
                report.ProgramFingerprint,
                report.FrameCount.ToString(),
                report.Fingerprint
            }.Concat(
                report.Frames.SelectMany(frame=>new[]
                {
                    frame.Sequence.Value.ToString(),
                    frame.InputFingerprint,
                    frame.PipelineReport.Fingerprint
                })));

        var bytes=Encoding.UTF8.GetBytes(canonical);
        var sha256=Convert.ToHexString(
            SHA256.HashData(bytes))
            .ToLowerInvariant();

        var artifact=new ReleaseArtifact(
            artifactPath.Trim(),
            sha256,
            bytes.LongLength);

        return ReleaseManifestRuntime.Create(
            identity,
            new[]{artifact});
    }
}
