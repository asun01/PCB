using System.Security.Cryptography;
using System.Text;
using Asun.Domain.Quality;
using Asun.Platform.MeasurementQualityIntegration;
using Asun.Production.Runtime;

namespace Asun.Platform.RoiProductionIntegration;

public sealed record ProductionRoiMeasurementQualityContext(
    Guid ProductionSessionId,
    string RoiContextBindingFingerprint,
    long MeasurementSequence,
    string MeasurementFingerprint,
    Guid QualityResultId,
    Guid QualitySnapshotId,
    string BindingFingerprint);

public static class ProductionRoiMeasurementQualityContextRuntime
{
    public static ProductionRoiMeasurementQualityContext Create(
        ProductionSessionReport productionReport,
        ProductionRoiInteractionContext roiContext,
        MeasurementQualityEvaluation evaluation)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(roiContext);
        ArgumentNullException.ThrowIfNull(evaluation);

        var errors=Validate(productionReport,roiContext,evaluation);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var canonical=string.Join("|",
            productionReport.SessionId,
            roiContext.BindingFingerprint,
            evaluation.Measurement.Sequence,
            evaluation.Fingerprint,
            evaluation.Result.ResultId,
            evaluation.Result.SnapshotId);

        return new ProductionRoiMeasurementQualityContext(
            productionReport.SessionId,
            roiContext.BindingFingerprint,
            evaluation.Measurement.Sequence,
            evaluation.Fingerprint,
            evaluation.Result.ResultId,
            evaluation.Result.SnapshotId,
            Hash(canonical));
    }

    public static IReadOnlyList<string> Validate(
        ProductionSessionReport productionReport,
        ProductionRoiInteractionContext roiContext,
        MeasurementQualityEvaluation evaluation)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(roiContext);
        ArgumentNullException.ThrowIfNull(evaluation);

        var errors=new List<string>();
        errors.AddRange(ProductionRoiInteractionContextRuntime.ValidateBinding(roiContext));

        if(!MeasurementQualityEvaluationValidationRuntime.IsValid(evaluation))
            errors.AddRange(MeasurementQualityEvaluationValidationRuntime.Validate(evaluation));

        if(roiContext.ProductionSessionId!=productionReport.SessionId)
            errors.Add("ROI context session identity must match production session identity.");

        var frame=productionReport.Frames
            .SingleOrDefault(item=>item.Sequence.Value==evaluation.Measurement.Sequence);

        if(frame is null)
        {
            errors.Add("Measurement sequence must belong to the Production session.");
        }
        else if(frame.InputFingerprint!=evaluation.Measurement.ProductionInputFingerprint)
        {
            errors.Add("Measurement production input fingerprint must match the Production frame.");
        }

        if(evaluation.Result.ResultId==Guid.Empty)
            errors.Add("Quality result id cannot be empty.");
        if(evaluation.Result.SnapshotId==Guid.Empty)
            errors.Add("Quality snapshot id cannot be empty.");
        if(string.IsNullOrWhiteSpace(evaluation.Fingerprint) ||
           evaluation.Fingerprint.Length!=64 ||
           !IsLowerHex(evaluation.Fingerprint))
            errors.Add("Measurement quality fingerprint must be 64 lowercase hexadecimal characters.");

        return errors;
    }

    public static bool IsValid(
        ProductionSessionReport productionReport,
        ProductionRoiInteractionContext roiContext,
        MeasurementQualityEvaluation evaluation)=>
        Validate(productionReport,roiContext,evaluation).Count==0;

    public static bool IsEquivalent(
        ProductionRoiMeasurementQualityContext left,
        ProductionRoiMeasurementQualityContext right)=>
        left.BindingFingerprint==right.BindingFingerprint;

    private static bool IsLowerHex(string value)=>
        value.All(character=>Uri.IsHexDigit(character) &&
            char.ToLowerInvariant(character)==character);

    private static string Hash(string value)=>
        Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(value)))
        .ToLowerInvariant();
}
