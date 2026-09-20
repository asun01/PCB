using System.Security.Cryptography;
using System.Text;
using Asun.Platform.Evidence;
using Asun.Production.Runtime;

namespace Asun.Platform.CaptureEvidenceIntegration;

public static class ProductionCaptureEvidenceCanonicalRuntime
{
    public static string CreateFrameFingerprint(ProductionCaptureEvidenceFrameReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);
        var handles=reference.Handles?.Select(handle=>handle.Value).OrderBy(value=>value,StringComparer.Ordinal).ToArray()
            ?? throw new ArgumentException("Evidence handles cannot be null.",nameof(reference));
        var canonical=string.Join("|",
            reference.Sequence,
            reference.PayloadFingerprint,
            reference.Width,
            reference.Height,
            reference.PixelFormat,
            string.Join(",",handles));
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }

    public static IReadOnlyList<string> CreateFingerprintList(IReadOnlyList<ProductionCaptureEvidenceFrameReference> references)
    {
        ArgumentNullException.ThrowIfNull(references);
        return references.OrderBy(reference=>reference.Sequence)
            .Select(CreateFrameFingerprint)
            .ToArray();
    }

    public static string CreateProjectionFingerprint(IReadOnlyList<ProductionCaptureEvidenceFrameReference> references)
    {
        var entries=CreateFingerprintList(references);
        var canonical=string.Join("\n",entries);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical))).ToLowerInvariant();
    }

    public static IReadOnlyList<string> ValidateCanonical(
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionCaptureEvidenceFrameReference> references)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(references);
        var errors=new List<string>();
        errors.AddRange(ProductionCaptureEvidenceProjectionRuntime.Validate(productionReport,references));
        foreach(var reference in references)
        {
            if(reference.PayloadFingerprint.Length!=64 || !reference.PayloadFingerprint.All(Uri.IsHexDigit))
                errors.Add($"Capture evidence reference {reference.Sequence} payload fingerprint must be 64 hexadecimal characters.");
            if(reference.Width<=0 || reference.Height<=0)
                errors.Add($"Capture evidence reference {reference.Sequence} dimensions must be positive.");
            if(string.IsNullOrWhiteSpace(reference.PixelFormat))
                errors.Add($"Capture evidence reference {reference.Sequence} pixel format cannot be blank.");
            if(reference.Handles is null || reference.Handles.Any(handle=>!handle.IsValid))
                errors.Add($"Capture evidence reference {reference.Sequence} contains invalid evidence handles.");
        }
        if(CreateFingerprintList(references).Distinct(StringComparer.Ordinal).Count()!=references.Count)
            errors.Add("Capture evidence frame fingerprints must be unique.");
        return errors;
    }

    public static bool IsCanonical(
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionCaptureEvidenceFrameReference> references)=>
        ValidateCanonical(productionReport,references).Count==0;

    public static bool IsEquivalent(
        IReadOnlyList<ProductionCaptureEvidenceFrameReference> left,
        IReadOnlyList<ProductionCaptureEvidenceFrameReference> right)=>
        CreateProjectionFingerprint(left)==CreateProjectionFingerprint(right);
}
