using System.Security.Cryptography;
using System.Text;
using Asun.Platform.CaptureEvidenceIntegration;
using Asun.Platform.Evidence;
using Asun.Production.Runtime;

namespace Asun.Platform.ReplayIntegration;

public sealed record ProductionCaptureEvidenceReplayBinding(
    Guid ProductionSessionId,
    int FrameCount,
    string ProductionFingerprint,
    string CaptureEvidenceProjectionFingerprint,
    string Fingerprint);

public static class ProductionCaptureEvidenceReplayBindingRuntime
{
    public static ProductionCaptureEvidenceReplayBinding Create(
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionCaptureEvidenceFrameReference> references)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(references);

        var errors=Validate(productionReport,references);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var projectionFingerprint=ProductionCaptureEvidenceCanonicalRuntime.CreateProjectionFingerprint(references);
        var fingerprint=CreateFingerprint(
            productionReport.SessionId,
            productionReport.FrameCount,
            productionReport.Fingerprint,
            projectionFingerprint);

        return new ProductionCaptureEvidenceReplayBinding(
            productionReport.SessionId,
            productionReport.FrameCount,
            productionReport.Fingerprint,
            projectionFingerprint,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionCaptureEvidenceFrameReference> references,
        ProductionCaptureEvidenceReplayBinding binding)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(references);
        ArgumentNullException.ThrowIfNull(binding);

        var errors=Validate(productionReport,references);
        if(errors.Count>0)
            return errors;

        var projectionFingerprint=ProductionCaptureEvidenceCanonicalRuntime.CreateProjectionFingerprint(references);
        if(binding.ProductionSessionId!=productionReport.SessionId)
            errors.Add("Replay binding Production session identity must match.");
        if(binding.FrameCount!=productionReport.FrameCount)
            errors.Add("Replay binding frame count must match.");
        if(binding.ProductionFingerprint!=productionReport.Fingerprint)
            errors.Add("Replay binding Production fingerprint must match.");
        if(binding.CaptureEvidenceProjectionFingerprint!=projectionFingerprint)
            errors.Add("Replay binding Capture/Evidence projection fingerprint must match.");
        if(binding.Fingerprint.Length!=64 || !IsLowerHex(binding.Fingerprint))
            errors.Add("Replay binding fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count==0)
        {
            var expected=CreateFingerprint(
                productionReport.SessionId,
                productionReport.FrameCount,
                productionReport.Fingerprint,
                projectionFingerprint);
            if(expected!=binding.Fingerprint)
                errors.Add("Replay binding fingerprint does not match canonical content.");
        }

        return errors;
    }

    public static IReadOnlyList<string> Validate(
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionCaptureEvidenceFrameReference> references)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(references);

        var errors=new List<string>();

        if(productionReport.SessionId==Guid.Empty)
            errors.Add("Production session identity cannot be empty.");
        if(productionReport.FrameCount<=0)
            errors.Add("Production frame count must be positive.");
        if(string.IsNullOrWhiteSpace(productionReport.Fingerprint))
            errors.Add("Production fingerprint cannot be blank.");

        errors.AddRange(
            ProductionCaptureEvidenceCanonicalRuntime.ValidateCanonical(
                productionReport,
                references));

        if(references.Count!=productionReport.FrameCount)
            errors.Add("Capture/Evidence frame count must match Production.");

        return errors;
    }

    public static bool IsValid(
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionCaptureEvidenceFrameReference> references,
        ProductionCaptureEvidenceReplayBinding binding)=>
        Validate(productionReport,references,binding).Count==0;

    public static bool IsEquivalent(
        ProductionCaptureEvidenceReplayBinding left,
        ProductionCaptureEvidenceReplayBinding right)=>
        left.Fingerprint==right.Fingerprint;

    internal static string CreateFingerprint(
        Guid productionSessionId,
        int frameCount,
        string productionFingerprint,
        string captureEvidenceProjectionFingerprint)
    {
        var canonical=string.Join(
            "|",
            productionSessionId,
            frameCount,
            productionFingerprint.Length,
            productionFingerprint,
            captureEvidenceProjectionFingerprint.Length,
            captureEvidenceProjectionFingerprint);

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();
    }

    private static bool IsLowerHex(string value)=>
        value.Length==64 &&
        value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c);
}
