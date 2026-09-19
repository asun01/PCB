namespace Asun.Metrology.Core;

public static class AffineCalibrationValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        IEnumerable<CalibrationCorrespondence2D> correspondences,
        AffineCalibrationResult2D result,
        double tolerance=1e-9)
    {
        ArgumentNullException.ThrowIfNull(correspondences);
        ArgumentNullException.ThrowIfNull(result);

        var errors=new List<string>();
        var points=correspondences.ToArray();

        if(points.Length<3)
            errors.Add("Affine calibration validation requires at least three correspondences.");

        if(points.Any(point=>!point.IsValid))
            errors.Add("Affine calibration validation received a non-finite correspondence.");

        if(!double.IsFinite(tolerance) || tolerance<=0)
            errors.Add("Affine calibration validation tolerance must be finite and positive.");

        if(result.PointCount!=points.Length)
            errors.Add("Calibration point count does not match the correspondence count.");

        if(!double.IsFinite(result.RootMeanSquareError) ||
           result.RootMeanSquareError<0)
        {
            errors.Add("Calibration RMS error must be finite and non-negative.");
        }

        if(!double.IsFinite(result.MaximumError) ||
           result.MaximumError<0)
        {
            errors.Add("Calibration maximum error must be finite and non-negative.");
        }

        if(result.Fingerprint.Length!=64 ||
           !result.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Calibration fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count>0)
            return errors;

        var expected=AffineCalibrationRuntime.Fit(points);

        if(!result.Transform.Equals(expected.Transform) &&
           Math.Abs(result.Transform.M11-expected.Transform.M11)>tolerance)
        {
            errors.Add("Calibration transform does not match the correspondences.");
        }

        if(Math.Abs(result.RootMeanSquareError-expected.RootMeanSquareError)>tolerance)
            errors.Add("Calibration RMS error does not match the correspondences.");

        if(Math.Abs(result.MaximumError-expected.MaximumError)>tolerance)
            errors.Add("Calibration maximum error does not match the correspondences.");

        if(result.Fingerprint!=expected.Fingerprint)
            errors.Add("Calibration fingerprint does not match the result.");

        return errors;
    }

    public static bool IsValid(
        IEnumerable<CalibrationCorrespondence2D> correspondences,
        AffineCalibrationResult2D result,
        double tolerance=1e-9)=>
        Validate(correspondences,result,tolerance).Count==0;
}
