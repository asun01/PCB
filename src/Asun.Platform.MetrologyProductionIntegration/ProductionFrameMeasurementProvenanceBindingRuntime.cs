using System.Security.Cryptography;
using System.Text;

namespace Asun.Platform.MetrologyProductionIntegration;

public sealed record ProductionFrameMeasurementProvenanceBinding(
    long Sequence,
    string ProductionInputFingerprint,
    long Width,
    long Height,
    string PixelFormat,
    DateTimeOffset CapturedAtUtc,
    double MeasuredX,
    double MeasuredY,
    double ErrorDistance,
    string CalibrationFingerprint,
    string ObservationFingerprint,
    string BindingFingerprint);

public static class ProductionFrameMeasurementProvenanceBindingRuntime
{
    public static ProductionFrameMeasurementProvenanceBinding Create(
        ProductionFrameProvenance provenance,
        ProductionMeasurementFact measurement)
    {
        ArgumentNullException.ThrowIfNull(provenance);

        var errors=Validate(provenance,measurement);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var canonical=string.Join("|",
            provenance.Sequence.Value,
            provenance.PayloadFingerprint,
            provenance.Width,
            provenance.Height,
            provenance.PixelFormat.Length,
            provenance.PixelFormat,
            provenance.CapturedAtUtc.UtcTicks,
            measurement.MeasuredPosition.X.ToString("R"),
            measurement.MeasuredPosition.Y.ToString("R"),
            measurement.ErrorDistance.ToString("R"),
            measurement.CalibrationFingerprint,
            measurement.ObservationFingerprint);

        return new ProductionFrameMeasurementProvenanceBinding(
            provenance.Sequence.Value,
            measurement.ProductionInputFingerprint,
            provenance.Width,
            provenance.Height,
            provenance.PixelFormat,
            provenance.CapturedAtUtc,
            measurement.MeasuredPosition.X,
            measurement.MeasuredPosition.Y,
            measurement.ErrorDistance,
            measurement.CalibrationFingerprint,
            measurement.ObservationFingerprint,
            Hash(canonical));
    }

    public static IReadOnlyList<string> Validate(
        ProductionFrameProvenance provenance,
        ProductionMeasurementFact measurement)
    {
        ArgumentNullException.ThrowIfNull(provenance);

        var errors=new List<string>();
        if(provenance.Sequence.Value<=0)
            errors.Add("Production provenance sequence must be positive.");
        if(measurement.Sequence<=0)
            errors.Add("Measurement sequence must be positive.");
        if(provenance.Sequence.Value!=measurement.Sequence)
            errors.Add("Frame provenance and measurement sequence must match.");
        if(!IsLowerHex(provenance.PayloadFingerprint))
            errors.Add("Production provenance payload fingerprint is malformed.");
        if(!IsLowerHex(measurement.ProductionInputFingerprint))
            errors.Add("Measurement Production input fingerprint is malformed.");
        if(provenance.PayloadFingerprint!=measurement.ProductionInputFingerprint)
            errors.Add("Frame provenance and measurement Production input fingerprints must match.");
        if(provenance.Width<=0 || provenance.Height<=0)
            errors.Add("Production provenance dimensions must be positive.");
        if(string.IsNullOrWhiteSpace(provenance.PixelFormat))
            errors.Add("Production provenance pixel format cannot be blank.");
        if(!measurement.SourceMeasuredPosition.IsFinite ||
           !measurement.MeasuredPosition.IsFinite)
            errors.Add("Measurement points must be finite.");
        if(!double.IsFinite(measurement.ErrorDistance) || measurement.ErrorDistance<0)
            errors.Add("Measurement error distance must be finite and non-negative.");
        if(!IsLowerHex(measurement.CalibrationFingerprint))
            errors.Add("Measurement calibration fingerprint is malformed.");
        if(!IsLowerHex(measurement.ObservationFingerprint))
            errors.Add("Measurement observation fingerprint is malformed.");

        return errors;
    }

    public static bool IsValid(
        ProductionFrameProvenance provenance,
        ProductionMeasurementFact measurement)=>
        Validate(provenance,measurement).Count==0;

    public static IReadOnlyList<string> ValidateBinding(
        ProductionFrameMeasurementProvenanceBinding binding)
    {
        ArgumentNullException.ThrowIfNull(binding);

        var errors=new List<string>();
        if(binding.Sequence<=0)
            errors.Add("Binding sequence must be positive.");
        if(!IsLowerHex(binding.ProductionInputFingerprint))
            errors.Add("Binding Production input fingerprint is malformed.");
        if(binding.Width<=0 || binding.Height<=0)
            errors.Add("Binding dimensions must be positive.");
        if(string.IsNullOrWhiteSpace(binding.PixelFormat))
            errors.Add("Binding pixel format cannot be blank.");
        if(!double.IsFinite(binding.MeasuredX) ||
           !double.IsFinite(binding.MeasuredY) ||
           !double.IsFinite(binding.ErrorDistance) ||
           binding.ErrorDistance<0)
            errors.Add("Binding measurement values are invalid.");
        if(!IsLowerHex(binding.CalibrationFingerprint))
            errors.Add("Binding calibration fingerprint is malformed.");
        if(!IsLowerHex(binding.ObservationFingerprint))
            errors.Add("Binding observation fingerprint is malformed.");
        if(!IsLowerHex(binding.BindingFingerprint))
            errors.Add("Binding fingerprint is malformed.");
        return errors;
    }

    public static bool IsValidBinding(
        ProductionFrameMeasurementProvenanceBinding binding)=>
        ValidateBinding(binding).Count==0;

    public static bool IsEquivalent(
        ProductionFrameMeasurementProvenanceBinding left,
        ProductionFrameMeasurementProvenanceBinding right)=>
        left.BindingFingerprint==right.BindingFingerprint;

    private static bool IsLowerHex(string value)=>
        value.Length==64 &&
        value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c);

    private static string Hash(string value)=>
        Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(value)))
        .ToLowerInvariant();
}
