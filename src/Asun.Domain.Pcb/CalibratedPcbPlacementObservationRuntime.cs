using System.Security.Cryptography;
using System.Text;
using Asun.Metrology.Core;

namespace Asun.Domain.Pcb;

public static class CalibratedPcbPlacementObservationRuntime
{
    public static CalibratedPcbPlacementObservation Measure(
        PcbComponentReference component,
        MetrologyPoint2D sourceMeasuredPosition,
        IEnumerable<CalibrationCorrespondence2D> correspondences,
        AffineCalibrationResult2D calibration)
    {
        ArgumentNullException.ThrowIfNull(component);
        ArgumentNullException.ThrowIfNull(correspondences);
        ArgumentNullException.ThrowIfNull(calibration);

        if(!PcbComponentReferenceValidationRuntime.IsValid(component))
            throw new ArgumentException("PCB component is invalid.",nameof(component));

        if(!sourceMeasuredPosition.IsFinite)
            throw new ArgumentException("Source measured position must be finite.",nameof(sourceMeasuredPosition));

        var points=correspondences.ToArray();
        if(!AffineCalibrationValidationRuntime.IsValid(points,calibration))
            throw new ArgumentException("Affine calibration is invalid.",nameof(calibration));

        var calibratedPosition=calibration.Transform.Transform(sourceMeasuredPosition);
        var observation=PcbPlacementObservationRuntime.Measure(
            component,
            calibratedPosition);

        var fingerprint=CreateFingerprint(
            component,
            sourceMeasuredPosition,
            calibration,
            observation);

        return new CalibratedPcbPlacementObservation(
            observation,
            sourceMeasuredPosition,
            calibration.Fingerprint,
            fingerprint);
    }

    internal static string CreateFingerprint(
        PcbComponentReference component,
        MetrologyPoint2D sourceMeasuredPosition,
        AffineCalibrationResult2D calibration,
        PcbPlacementObservation observation)
    {
        var builder=new StringBuilder();
        builder.Append(component.Id).Append('|')
            .Append(component.Designator.Length).Append(':').Append(component.Designator).Append('|')
            .Append(sourceMeasuredPosition.X.ToString("R")).Append('|')
            .Append(sourceMeasuredPosition.Y.ToString("R")).Append('|')
            .Append(calibration.Fingerprint).Append('|')
            .Append(observation.MeasuredPosition.X.ToString("R")).Append('|')
            .Append(observation.MeasuredPosition.Y.ToString("R")).Append('|')
            .Append(observation.Delta.X.ToString("R")).Append('|')
            .Append(observation.Delta.Y.ToString("R")).Append('|')
            .Append(observation.ErrorDistance.ToString("R"));

        return Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }
}
