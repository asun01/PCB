using System.Security.Cryptography;
using System.Text;
namespace Asun.Platform.QualityEvidenceIntegration;

public sealed record ProductionFrameMeasurementQualityEvidenceProvenanceBinding(
    long Sequence,
    string ProductionInputFingerprint,
    Guid QualityResultId,
    string FindingId,
    string EvidenceFingerprint,
    string QualityEvaluationFingerprint,
    string FrameMeasurementQualityBindingFingerprint,
    string BindingFingerprint);

public static class ProductionFrameMeasurementQualityEvidenceProvenanceBindingRuntime
{
    public static ProductionFrameMeasurementQualityEvidenceProvenanceBinding Create(
        ProductionFrameMeasurementQualityProvenanceBinding qualityProvenanceBinding,
        ProductionMeasurementQualityEvidenceBinding evidenceBinding)
    {
        ArgumentNullException.ThrowIfNull(qualityProvenanceBinding);
        ArgumentNullException.ThrowIfNull(evidenceBinding);

        var errors=Validate(qualityProvenanceBinding,evidenceBinding);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var canonical=string.Join("|",
            qualityProvenanceBinding.Sequence,
            qualityProvenanceBinding.ProductionInputFingerprint,
            qualityProvenanceBinding.QualityResultId,
            evidenceBinding.FindingId.Value,
            evidenceBinding.EvidenceFingerprint,
            qualityProvenanceBinding.QualityEvaluationFingerprint,
            qualityProvenanceBinding.BindingFingerprint);

        return new ProductionFrameMeasurementQualityEvidenceProvenanceBinding(
            qualityProvenanceBinding.Sequence,
            qualityProvenanceBinding.ProductionInputFingerprint,
            qualityProvenanceBinding.QualityResultId,
            evidenceBinding.FindingId.Value,
            evidenceBinding.EvidenceFingerprint,
            qualityProvenanceBinding.QualityEvaluationFingerprint,
            qualityProvenanceBinding.BindingFingerprint,
            Hash(canonical));
    }

    public static IReadOnlyList<string> Validate(
        ProductionFrameMeasurementQualityProvenanceBinding qualityProvenanceBinding,
        ProductionMeasurementQualityEvidenceBinding evidenceBinding)
    {
        ArgumentNullException.ThrowIfNull(qualityProvenanceBinding);
        ArgumentNullException.ThrowIfNull(evidenceBinding);

        var errors=new List<string>();
        if(qualityProvenanceBinding.Sequence!=evidenceBinding.Sequence)
            errors.Add("Quality/evidence sequence must match frame/measurement/quality provenance.");
        if(qualityProvenanceBinding.ProductionInputFingerprint!=evidenceBinding.ProductionInputFingerprint)
            errors.Add("Quality/evidence Production input fingerprint must match.");
        if(qualityProvenanceBinding.QualityResultId!=evidenceBinding.QualityResultId)
            errors.Add("Quality/evidence Quality result identity must match.");
        if(qualityProvenanceBinding.QualityEvaluationFingerprint.Length!=64 ||
           !IsLowerHex(qualityProvenanceBinding.QualityEvaluationFingerprint))
            errors.Add("Quality evaluation fingerprint is malformed.");
        if(qualityProvenanceBinding.BindingFingerprint.Length!=64 ||
           !IsLowerHex(qualityProvenanceBinding.BindingFingerprint))
            errors.Add("Frame/measurement/quality binding fingerprint is malformed.");
        if(evidenceBinding.EvidenceFingerprint.Length!=64 ||
           !IsLowerHex(evidenceBinding.EvidenceFingerprint))
            errors.Add("Evidence fingerprint is malformed.");
        if(evidenceBinding.Fingerprint.Length!=64 ||
           !IsLowerHex(evidenceBinding.Fingerprint))
            errors.Add("Quality/evidence binding fingerprint is malformed.");

        return errors;
    }

    public static bool IsValid(
        ProductionFrameMeasurementQualityProvenanceBinding qualityProvenanceBinding,
        ProductionMeasurementQualityEvidenceBinding evidenceBinding)=>
        Validate(qualityProvenanceBinding,evidenceBinding).Count==0;

    public static bool IsEquivalent(
        ProductionFrameMeasurementQualityEvidenceProvenanceBinding left,
        ProductionFrameMeasurementQualityEvidenceProvenanceBinding right)=>
        left.BindingFingerprint==right.BindingFingerprint;

    private static bool IsLowerHex(string value)=>
        value.Length==64 && value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c);

    private static string Hash(string value)=>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

}
