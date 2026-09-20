using System.Security.Cryptography;
using System.Text;
using Asun.Domain.Quality;
using Asun.Platform.MetrologyProductionIntegration;

namespace Asun.Platform.MeasurementQualityIntegration;

public static class MeasurementQualityEvaluationRuntime
{
    public static MeasurementQualityEvaluation Evaluate(
        ProductionMeasurementFact measurement,
        long sequence,
        Guid resultId,
        Guid snapshotId,
        Func<ProductionMeasurementFact,QualityFinding> ruleEvaluator)
    {
        ArgumentNullException.ThrowIfNull(ruleEvaluator);

        if(!IsValidMeasurementFact(measurement))
            throw new ArgumentException("Measurement fact is invalid.",nameof(measurement));
        if(sequence<0)
            throw new ArgumentOutOfRangeException(nameof(sequence));
        if(resultId==Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(resultId));
        if(snapshotId==Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(snapshotId));

        var finding=ruleEvaluator(measurement);
        if(finding is null || !finding.IsValid)
            throw new ArgumentException("Measurement rule must return a valid QualityFinding.",nameof(ruleEvaluator));

        var result=new QualityInspectionResult(
            resultId,
            new QualityInspectionSnapshot(
                snapshotId,
                sequence,
                new QualityFindingSet(new[]{finding}),
                new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())));
        var fingerprint=CreateFingerprint(measurement,result);

        return new MeasurementQualityEvaluation(measurement,result,fingerprint);
    }

    internal static string CreateFingerprint(
        ProductionMeasurementFact measurement,
        QualityInspectionResult result)
    {
        var finding=result.Findings.Findings.Single();
        var canonical=string.Join(
            "|",
            measurement.Sequence,
            measurement.ProductionInputFingerprint,
            measurement.SourceMeasuredPosition.X.ToString("R"),
            measurement.SourceMeasuredPosition.Y.ToString("R"),
            measurement.MeasuredPosition.X.ToString("R"),
            measurement.MeasuredPosition.Y.ToString("R"),
            measurement.ErrorDistance.ToString("R"),
            measurement.CalibrationFingerprint,
            measurement.ObservationFingerprint,
            result.ResultId,
            result.SnapshotId,
            result.Sequence,
            finding.Id,
            (int)finding.Outcome,
            (int)finding.Severity,
            finding.RuleCode,
            finding.Message);

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();
    }

    private static bool IsValidMeasurementFact(ProductionMeasurementFact measurement)=>
        measurement.Sequence>0 &&
        measurement.SourceMeasuredPosition.IsFinite &&
        measurement.MeasuredPosition.IsFinite &&
        double.IsFinite(measurement.ErrorDistance) &&
        measurement.ErrorDistance>=0 &&
        measurement.ProductionInputFingerprint.Length==64 &&
        measurement.ProductionInputFingerprint.All(character=>
            Uri.IsHexDigit(character) &&
            char.ToLowerInvariant(character)==character) &&
        measurement.CalibrationFingerprint.Length==64 &&
        measurement.CalibrationFingerprint.All(character=>
            Uri.IsHexDigit(character) &&
            char.ToLowerInvariant(character)==character) &&
        measurement.ObservationFingerprint.Length==64 &&
        measurement.ObservationFingerprint.All(character=>
            Uri.IsHexDigit(character) &&
            char.ToLowerInvariant(character)==character);
}
