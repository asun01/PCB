using System.Security.Cryptography;
using System.Text;
using Asun.Platform.DeviceProductionIntegration;
using Asun.Production.Runtime;

namespace Asun.Platform.DeviceProductionEvidenceIntegration;

public sealed record CaptureSessionProductionEvidenceBinding(
    Guid ProductionSessionId,
    string ProductionBindingFingerprint,
    string EvidenceCaptureProjectionFingerprint,
    int FrameCount,
    string Fingerprint);

public static class CaptureSessionProductionEvidenceBindingRuntime
{
    public static CaptureSessionProductionEvidenceBinding Create(
        CaptureSessionProductionBinding productionBinding,
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionFrameProvenance> provenance,
        IReadOnlyList<ProductionCaptureEvidenceFrameReference> references)
    {
        ArgumentNullException.ThrowIfNull(productionBinding);
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(provenance);
        ArgumentNullException.ThrowIfNull(references);

        var errors=ValidateInputs(
            productionBinding,
            productionReport,
            provenance,
            references);

        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors),nameof(references));

        var projectionFingerprint=
            ProductionCaptureEvidenceCanonicalRuntime.CreateProjectionFingerprint(
                references);

        var fingerprint=CreateFingerprint(
            productionReport.SessionId,
            productionBinding.Fingerprint,
            projectionFingerprint,
            references.Count);

        return new CaptureSessionProductionEvidenceBinding(
            productionReport.SessionId,
            productionBinding.Fingerprint,
            projectionFingerprint,
            references.Count,
            fingerprint);
    }

    public static IReadOnlyList<string> Validate(
        CaptureSessionProductionBinding productionBinding,
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionFrameProvenance> provenance,
        IReadOnlyList<ProductionCaptureEvidenceFrameReference> references,
        CaptureSessionProductionEvidenceBinding binding)
    {
        ArgumentNullException.ThrowIfNull(productionBinding);
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(provenance);
        ArgumentNullException.ThrowIfNull(references);
        ArgumentNullException.ThrowIfNull(binding);

        var errors=ValidateInputs(
            productionBinding,
            productionReport,
            provenance,
            references);

        if(binding.ProductionSessionId!=productionReport.SessionId)
            errors.Add("Capture/Production/Evidence session identity must match.");
        if(binding.ProductionBindingFingerprint!=productionBinding.Fingerprint)
            errors.Add("Capture/Production/Evidence Production binding identity must match.");

        var projectionFingerprint=
            ProductionCaptureEvidenceCanonicalRuntime.CreateProjectionFingerprint(
                references);

        if(binding.EvidenceCaptureProjectionFingerprint!=projectionFingerprint)
            errors.Add("Capture/Production/Evidence projection fingerprint must match.");
        if(binding.FrameCount!=references.Count)
            errors.Add("Capture/Production/Evidence frame count must match.");
        if(binding.Fingerprint.Length!=64 || !IsLowerHex(binding.Fingerprint))
            errors.Add("Capture/Production/Evidence binding fingerprint must be 64 lowercase hexadecimal characters.");

        if(errors.Count==0)
        {
            var expected=CreateFingerprint(
                productionReport.SessionId,
                productionBinding.Fingerprint,
                projectionFingerprint,
                references.Count);

            if(expected!=binding.Fingerprint)
                errors.Add("Capture/Production/Evidence binding fingerprint does not match canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        CaptureSessionProductionBinding productionBinding,
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionFrameProvenance> provenance,
        IReadOnlyList<ProductionCaptureEvidenceFrameReference> references,
        CaptureSessionProductionEvidenceBinding binding)=>
        Validate(
            productionBinding,
            productionReport,
            provenance,
            references,
            binding).Count==0;

    private static List<string> ValidateInputs(
        CaptureSessionProductionBinding productionBinding,
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionFrameProvenance> provenance,
        IReadOnlyList<ProductionCaptureEvidenceFrameReference> references)
    {
        var errors=new List<string>();

        if(productionBinding.ProductionSessionId!=productionReport.SessionId)
            errors.Add("Production binding session identity must match Production report.");
        if(productionBinding.CapturedCount!=productionReport.FrameCount)
            errors.Add("Production binding frame count must match Production report.");

        if(!ProductionCaptureEvidenceCanonicalRuntime.IsCanonical(
               productionReport,
               references))
        {
            errors.Add("Capture evidence projection is not canonical for the supplied Production report.");
        }

        if(provenance.Count!=productionReport.FrameCount)
            errors.Add("Production provenance count must match Production report.");
        if(references.Count!=productionReport.FrameCount)
            errors.Add("Capture evidence reference count must match Production report.");

        return errors;
    }

    private static string CreateFingerprint(
        Guid productionSessionId,
        string productionBindingFingerprint,
        string evidenceProjectionFingerprint,
        int frameCount)=>
        Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(
                    string.Join(
                        "|",
                        productionSessionId,
                        productionBindingFingerprint,
                        evidenceProjectionFingerprint,
                        frameCount))))
            .ToLowerInvariant();

    private static bool IsLowerHex(string value)=>
        value.Length==64 &&
        value.All(character=>
            Uri.IsHexDigit(character) &&
            char.ToLowerInvariant(character)==character);
}
