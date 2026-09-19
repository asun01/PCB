using System.Security.Cryptography;
using System.Text;
using Asun.Production.Runtime;
using Asun.Release.Core;

namespace Asun.Platform.ReleaseIntegration;

public static class ProductionEvidenceReleaseProjectionRuntime
{
    public static ReleaseManifest Create(
        ReleaseIdentity identity,
        ProductionSessionDefinition definition,
        ProductionSessionReport productionReport,
        ProductionEvidenceReferenceProjection evidenceProjection,
        string productionArtifactPath,
        string evidenceReferenceArtifactPath)
    {
        ArgumentNullException.ThrowIfNull(identity);
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(evidenceProjection);

        if(!ProductionSessionValidationRuntime.IsValid(definition,productionReport))
            throw new ArgumentException("Production report is invalid.",nameof(productionReport));

        if(!ProductionEvidenceReferenceProjectionValidationRuntime.IsValid(
            productionReport,
            evidenceProjection))
        {
            throw new ArgumentException("Evidence projection is invalid.",nameof(evidenceProjection));
        }

        if(string.IsNullOrWhiteSpace(productionArtifactPath))
            throw new ArgumentException("Production artifact path cannot be blank.",nameof(productionArtifactPath));

        if(string.IsNullOrWhiteSpace(evidenceReferenceArtifactPath))
            throw new ArgumentException("Evidence reference artifact path cannot be blank.",nameof(evidenceReferenceArtifactPath));

        var productionCanonical=BuildProductionCanonical(productionReport);
        var productionBytes=Encoding.UTF8.GetBytes(productionCanonical);
        var productionSha=Hash(productionBytes);

        var evidenceCanonical=BuildEvidenceCanonical(evidenceProjection);
        var evidenceBytes=Encoding.UTF8.GetBytes(evidenceCanonical);
        var evidenceSha=Hash(evidenceBytes);

        return ReleaseManifestRuntime.Create(
            identity,
            new[]{
                new ReleaseArtifact(productionArtifactPath.Trim(),productionSha,productionBytes.LongLength),
                new ReleaseArtifact(evidenceReferenceArtifactPath.Trim(),evidenceSha,evidenceBytes.LongLength)
            });
    }

    private static string BuildProductionCanonical(
        ProductionSessionReport report)
    {
        var canonical=new StringBuilder();
        canonical.Append(report.SessionId).Append('|')
            .Append(report.ProgramFingerprint).Append('|')
            .Append(report.FrameCount).Append('|')
            .Append(report.Fingerprint).Append('|');

        foreach(var frame in report.Frames)
            canonical.Append(frame.Sequence.Value).Append('|')
                .Append(frame.InputFingerprint).Append('|')
                .Append(frame.PipelineReport.Fingerprint).Append('|');

        return canonical.ToString();
    }

    private static string BuildEvidenceCanonical(
        ProductionEvidenceReferenceProjection projection)
    {
        var canonical=new StringBuilder();
        canonical.Append(projection.ProductionSessionId).Append('|')
            .Append(projection.ProductionFingerprint).Append('|')
            .Append(projection.Fingerprint).Append('|');

        foreach(var frame in projection.Frames.OrderBy(item=>item.Sequence))
        {
            canonical.Append(frame.Sequence).Append('|');
            foreach(var handle in frame.Handles.OrderBy(item=>item.Value,StringComparer.Ordinal))
                canonical.Append(handle.Value.Length).Append(':').Append(handle.Value).Append('|');
        }

        return canonical.ToString();
    }

    private static string Hash(byte[] bytes)=>
        Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
}
