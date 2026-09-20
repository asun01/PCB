using System.Security.Cryptography;
using System.Text;
using Asun.Domain.Quality;
using Asun.Release.Core;

namespace Asun.Platform.QualityReleaseIntegration;

public sealed record QualityReleaseReplayDescriptor(
    Guid QualityRunId,
    string QualitySummaryFingerprint,
    string ReleaseManifestFingerprint,
    bool ReleaseReady,
    string ProjectionFingerprint,
    string DescriptorFingerprint);

public static class QualityReleaseReplayDescriptorRuntime
{
    public static QualityReleaseReplayDescriptor Create(
        QualityInspectionRun run,
        ReleaseManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(run);
        ArgumentNullException.ThrowIfNull(manifest);

        var projection=QualityReleaseFactProjectionRuntime.Create(run,manifest);
        var canonical=string.Join(
            "|",
            projection.QualityRunId,
            projection.QualitySummaryFingerprint,
            projection.ReleaseManifestFingerprint,
            projection.ReleaseReady,
            projection.Fingerprint);
        var descriptorFingerprint=Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();

        return new QualityReleaseReplayDescriptor(
            projection.QualityRunId,
            projection.QualitySummaryFingerprint,
            projection.ReleaseManifestFingerprint,
            projection.ReleaseReady,
            projection.Fingerprint,
            descriptorFingerprint);
    }

    public static IReadOnlyList<string> Validate(
        QualityInspectionRun run,
        ReleaseManifest manifest,
        QualityReleaseReplayDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(run);
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentNullException.ThrowIfNull(descriptor);

        var errors=new List<string>();
        QualityReleaseFactProjection projection;
        try
        {
            projection=QualityReleaseFactProjectionRuntime.Create(run,manifest);
        }
        catch(ArgumentException exception)
        {
            errors.Add(exception.Message);
            return errors;
        }

        if(descriptor.QualityRunId!=projection.QualityRunId)
            errors.Add("Quality release replay descriptor run identity must match.");
        if(descriptor.QualitySummaryFingerprint!=projection.QualitySummaryFingerprint)
            errors.Add("Quality release replay descriptor summary fingerprint must match.");
        if(descriptor.ReleaseManifestFingerprint!=projection.ReleaseManifestFingerprint)
            errors.Add("Quality release replay descriptor manifest fingerprint must match.");
        if(descriptor.ReleaseReady!=projection.ReleaseReady)
            errors.Add("Quality release replay descriptor readiness must match.");
        if(descriptor.ProjectionFingerprint!=projection.Fingerprint)
            errors.Add("Quality release replay descriptor projection fingerprint must match.");
        if(descriptor.DescriptorFingerprint.Length!=64 || !descriptor.DescriptorFingerprint.All(Uri.IsHexDigit))
            errors.Add("Quality release replay descriptor fingerprint must be 64 hexadecimal characters.");

        if(errors.Count>0)
            return errors;

        var expected=Create(run,manifest);
        if(expected.DescriptorFingerprint!=descriptor.DescriptorFingerprint)
            errors.Add("Quality release replay descriptor fingerprint does not match canonical content.");

        return errors;
    }

    public static bool IsValid(
        QualityInspectionRun run,
        ReleaseManifest manifest,
        QualityReleaseReplayDescriptor descriptor)=>
        Validate(run,manifest,descriptor).Count==0;

    public static bool IsEquivalent(
        QualityReleaseReplayDescriptor left,
        QualityReleaseReplayDescriptor right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        return left.DescriptorFingerprint==right.DescriptorFingerprint;
    }
}
