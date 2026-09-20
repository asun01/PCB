using System.Security.Cryptography;
using System.Text;

namespace Asun.Platform.MeasurementQualityIntegration;

public sealed record ProductionFrameMeasurementQualityProvenanceBinding(
    long Sequence,
    string ProductionInputFingerprint,
    Guid QualityResultId,
    Guid QualitySnapshotId,
    string FrameMeasurementBindingFingerprint,
    string QualityEvaluationFingerprint,
    string BindingFingerprint);

public static class ProductionFrameMeasurementQualityProvenanceBindingRuntime
{
    public static ProductionFrameMeasurementQualityProvenanceBinding Create(
        ProductionFrameMeasurementProvenanceBinding frameMeasurementBinding,
        MeasurementQualityEvaluation qualityEvaluation)
    {
        ArgumentNullException.ThrowIfNull(frameMeasurementBinding);
        ArgumentNullException.ThrowIfNull(qualityEvaluation);

        var errors=Validate(frameMeasurementBinding,qualityEvaluation);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var canonical=string.Join("|",
            frameMeasurementBinding.Sequence,
            frameMeasurementBinding.ProductionInputFingerprint,
            frameMeasurementBinding.BindingFingerprint,
            qualityEvaluation.Measurement.Sequence,
            qualityEvaluation.Measurement.ProductionInputFingerprint,
            qualityEvaluation.Result.ResultId,
            qualityEvaluation.Result.SnapshotId,
            qualityEvaluation.Result.Sequence,
            qualityEvaluation.Fingerprint);

        return new ProductionFrameMeasurementQualityProvenanceBinding(
            frameMeasurementBinding.Sequence,
            frameMeasurementBinding.ProductionInputFingerprint,
            qualityEvaluation.Result.ResultId,
            qualityEvaluation.Result.SnapshotId,
            frameMeasurementBinding.BindingFingerprint,
            qualityEvaluation.Fingerprint,
            Hash(canonical));
    }

    public static IReadOnlyList<string> Validate(
        ProductionFrameMeasurementProvenanceBinding frameMeasurementBinding,
        MeasurementQualityEvaluation qualityEvaluation)
    {
        ArgumentNullException.ThrowIfNull(frameMeasurementBinding);
        ArgumentNullException.ThrowIfNull(qualityEvaluation);

        var errors=new List<string>();
        if(!ProductionFrameMeasurementProvenanceBindingRuntime.IsValidBinding(frameMeasurementBinding))
            errors.Add("Frame/measurement provenance binding is invalid.");
        if(!MeasurementQualityEvaluationValidationRuntime.IsValid(qualityEvaluation))
            errors.Add("Measurement quality evaluation is invalid.");

        if(qualityEvaluation.Measurement.Sequence!=frameMeasurementBinding.Sequence)
            errors.Add("Quality evaluation measurement sequence must match frame/measurement provenance.");
        if(qualityEvaluation.Measurement.ProductionInputFingerprint!=frameMeasurementBinding.ProductionInputFingerprint)
            errors.Add("Quality evaluation Production input fingerprint must match frame/measurement provenance.");
        if(qualityEvaluation.Measurement.CalibrationFingerprint!=frameMeasurementBinding.CalibrationFingerprint)
            errors.Add("Quality evaluation calibration fingerprint must match frame/measurement provenance.");
        if(qualityEvaluation.Measurement.ObservationFingerprint!=frameMeasurementBinding.ObservationFingerprint)
            errors.Add("Quality evaluation observation fingerprint must match frame/measurement provenance.");
        if(qualityEvaluation.Result.Sequence!=qualityEvaluation.Measurement.Sequence)
            errors.Add("Quality result sequence must match its measurement sequence.");
        if(qualityEvaluation.Result.ResultId==Guid.Empty)
            errors.Add("Quality result id cannot be empty.");
        if(qualityEvaluation.Result.SnapshotId==Guid.Empty)
            errors.Add("Quality snapshot id cannot be empty.");

        return errors;
    }

    public static bool IsValid(
        ProductionFrameMeasurementProvenanceBinding frameMeasurementBinding,
        MeasurementQualityEvaluation qualityEvaluation)=>
        Validate(frameMeasurementBinding,qualityEvaluation).Count==0;

    public static bool IsEquivalent(
        ProductionFrameMeasurementQualityProvenanceBinding left,
        ProductionFrameMeasurementQualityProvenanceBinding right)=>
        left.BindingFingerprint==right.BindingFingerprint;

    private static string Hash(string value)=>
        Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(value)))
        .ToLowerInvariant();
}
