using System.Security.Cryptography;
using System.Text;
using Asun.Platform.CaptureEvidenceIntegration;
using Asun.Release.Core;

namespace Asun.Platform.ReplayIntegration;

public sealed record ProductionCaptureEvidenceReleaseReplayBinding(
    Guid ProductionSessionId,
    string CaptureEvidenceReplayFingerprint,
    string ReleaseManifestFingerprint,
    bool ReleaseReady,
    string Fingerprint);

public static class ProductionCaptureEvidenceReleaseReplayBindingRuntime
{
    public static ProductionCaptureEvidenceReleaseReplayBinding Create(
        ProductionCaptureEvidenceReplayBinding replayBinding,
        ReleaseManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(replayBinding);
        ArgumentNullException.ThrowIfNull(manifest);

        var errors=ValidateInputs(replayBinding,manifest);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var readiness=ReleaseReadinessRuntime.Evaluate(manifest);
        var fingerprint=CreateFingerprint(
            replayBinding,
            manifest,
            readiness.Ready);

        return new ProductionCaptureEvidenceReleaseReplayBinding(
            replayBinding.ProductionSessionId,
            replayBinding.Fingerprint,
            manifest.Fingerprint,
            readiness.Ready,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        ProductionCaptureEvidenceReplayBinding replayBinding,
        ReleaseManifest manifest,
        ProductionCaptureEvidenceReleaseReplayBinding binding)
    {
        ArgumentNullException.ThrowIfNull(replayBinding);
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentNullException.ThrowIfNull(binding);

        var errors=ValidateInputs(replayBinding,manifest);
        if(errors.Count>0)
            return errors;

        var readiness=ReleaseReadinessRuntime.Evaluate(manifest);

        if(binding.ProductionSessionId!=replayBinding.ProductionSessionId)
            errors.Add("Release replay binding Production session identity must match.");
        if(binding.CaptureEvidenceReplayFingerprint!=replayBinding.Fingerprint)
            errors.Add("Release replay binding capture/evidence replay identity must match.");
        if(binding.ReleaseManifestFingerprint!=manifest.Fingerprint)
            errors.Add("Release replay binding Release manifest identity must match.");
        if(binding.ReleaseReady!=readiness.Ready)
            errors.Add("Release replay binding readiness must match canonical Release readiness.");
        if(binding.Fingerprint.Length!=64 || !IsLowerHex(binding.Fingerprint))
            errors.Add("Release replay binding fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count==0)
        {
            var expected=CreateFingerprint(replayBinding,manifest,readiness.Ready);
            if(expected!=binding.Fingerprint)
                errors.Add("Release replay binding fingerprint does not match canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        ProductionCaptureEvidenceReplayBinding replayBinding,
        ReleaseManifest manifest,
        ProductionCaptureEvidenceReleaseReplayBinding binding)=>
        Validate(replayBinding,manifest,binding).Count==0;

    public static bool IsEquivalent(
        ProductionCaptureEvidenceReleaseReplayBinding left,
        ProductionCaptureEvidenceReleaseReplayBinding right)=>
        left.Fingerprint==right.Fingerprint;

    private static IReadOnlyList<string> ValidateInputs(
        ProductionCaptureEvidenceReplayBinding replayBinding,
        ReleaseManifest manifest)
    {
        var errors=new List<string>();
        if(replayBinding.ProductionSessionId==Guid.Empty)
            errors.Add("Replay binding Production session identity cannot be empty.");
        if(replayBinding.Fingerprint.Length!=64 || !IsLowerHex(replayBinding.Fingerprint))
            errors.Add("Replay binding fingerprint is malformed.");
        if(!ReleaseManifestValidationRuntime.IsValid(manifest))
            errors.Add("Release manifest is invalid.");
        return errors;
    }

    private static string CreateFingerprint(
        ProductionCaptureEvidenceReplayBinding replayBinding,
        ReleaseManifest manifest,
        bool ready)
    {
        var canonical=string.Join(
            "|",
            replayBinding.ProductionSessionId,
            replayBinding.Fingerprint.Length,
            replayBinding.Fingerprint,
            manifest.Fingerprint.Length,
            manifest.Fingerprint,
            ready);

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();
    }

    private static bool IsLowerHex(string value)=>
        value.Length==64 &&
        value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c);
}
