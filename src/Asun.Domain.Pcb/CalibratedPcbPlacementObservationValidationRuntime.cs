using Asun.Metrology.Core;

namespace Asun.Domain.Pcb;

public static class CalibratedPcbPlacementObservationValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        PcbComponentReference component,
        MetrologyPoint2D sourceMeasuredPosition,
        IEnumerable<CalibrationCorrespondence2D> correspondences,
        AffineCalibrationResult2D calibration,
        CalibratedPcbPlacementObservation observation)
    {
        ArgumentNullException.ThrowIfNull(component);
        ArgumentNullException.ThrowIfNull(correspondences);
        ArgumentNullException.ThrowIfNull(calibration);
        ArgumentNullException.ThrowIfNull(observation);

        var errors=new List<string>();
        var points=correspondences.ToArray();

        if(!PcbComponentReferenceValidationRuntime.IsValid(component))
            errors.Add("PCB component is invalid.");

        if(!sourceMeasuredPosition.IsFinite)
            errors.Add("Source measured position must be finite.");

        errors.AddRange(
            AffineCalibrationValidationRuntime.Validate(points,calibration));

        if(errors.Count>0)
            return errors;

        if(observation.SourceMeasuredPosition!=sourceMeasuredPosition)
            errors.Add("Calibrated observation source position must match the supplied point.");

        if(observation.CalibrationFingerprint!=calibration.Fingerprint)
            errors.Add("Calibrated observation calibration fingerprint must match the result.");

        if(observation.Fingerprint.Length!=64 ||
           !observation.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Calibrated observation fingerprint must be 64 lowercase hexadecimal characters.");
        }

        var expectedPosition=calibration.Transform.Transform(sourceMeasuredPosition);
        if(observation.Observation.MeasuredPosition!=expectedPosition)
            errors.Add("Calibrated placement observation measured position must equal the transformed source point.");

        if(observation.Observation.ComponentId!=component.Id ||
           observation.Observation.Designator!=component.Designator)
        {
            errors.Add("Calibrated placement observation component identity must match the source component.");
        }

        if(errors.Count==0)
        {
            var expected=CalibratedPcbPlacementObservationRuntime.CreateFingerprint(
                component,
                sourceMeasuredPosition,
                calibration,
                observation.Observation);

            if(expected!=observation.Fingerprint)
                errors.Add("Calibrated observation fingerprint does not match its canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        PcbComponentReference component,
        MetrologyPoint2D sourceMeasuredPosition,
        IEnumerable<CalibrationCorrespondence2D> correspondences,
        AffineCalibrationResult2D calibration,
        CalibratedPcbPlacementObservation observation)=>
        Validate(component,sourceMeasuredPosition,correspondences,calibration,observation).Count==0;
}
