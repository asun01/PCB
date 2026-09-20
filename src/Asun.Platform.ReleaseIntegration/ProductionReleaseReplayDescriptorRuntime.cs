using System.Security.Cryptography;
using System.Text;
using Asun.Release.Core;

namespace Asun.Platform.ReleaseIntegration;

public sealed record ProductionReleaseReplayDescriptor(
    string ReleaseManifestFingerprint,
    int ArtifactCount,
    bool ReleaseReady,
    string DescriptorFingerprint);

public static class ProductionReleaseReplayDescriptorRuntime
{
    public static ProductionReleaseReplayDescriptor Create(ReleaseManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);
        if(!ReleaseManifestValidationRuntime.IsValid(manifest))
            throw new ArgumentException("Release manifest is invalid.",nameof(manifest));

        var canonical=string.Join(
            "|",
            manifest.Fingerprint,
            manifest.Artifacts.Count,
            ReleaseReadinessRuntime.Evaluate(manifest).Ready);
        var fingerprint=Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();

        return new ProductionReleaseReplayDescriptor(
            manifest.Fingerprint,
            manifest.Artifacts.Count,
            ReleaseReadinessRuntime.Evaluate(manifest).Ready,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        ReleaseManifest manifest,
        ProductionReleaseReplayDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentNullException.ThrowIfNull(descriptor);

        var errors=new List<string>();
        if(!ReleaseManifestValidationRuntime.IsValid(manifest))
            errors.Add("Release manifest is invalid.");
        var readiness=ReleaseReadinessRuntime.Evaluate(manifest);
        if(descriptor.ReleaseManifestFingerprint!=manifest.Fingerprint)
            errors.Add("Release replay descriptor manifest fingerprint must match.");
        if(descriptor.ArtifactCount!=manifest.Artifacts.Count)
            errors.Add("Release replay descriptor artifact count must match.");
        if(descriptor.ReleaseReady!=readiness.Ready)
            errors.Add("Release replay descriptor readiness must match.");
        if(descriptor.DescriptorFingerprint.Length!=64 || !descriptor.DescriptorFingerprint.All(Uri.IsHexDigit))
            errors.Add("Release replay descriptor fingerprint must be 64 hexadecimal characters.");

        if(errors.Count>0)
            return errors;

        var expected=Create(manifest);
        if(expected.DescriptorFingerprint!=descriptor.DescriptorFingerprint)
            errors.Add("Release replay descriptor fingerprint does not match canonical content.");

        return errors;
    }

    public static bool IsValid(ReleaseManifest manifest,ProductionReleaseReplayDescriptor descriptor)=>
        Validate(manifest,descriptor).Count==0;

    public static bool IsEquivalent(
        ProductionReleaseReplayDescriptor left,
        ProductionReleaseReplayDescriptor right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        return left.DescriptorFingerprint==right.DescriptorFingerprint;
    }
}
