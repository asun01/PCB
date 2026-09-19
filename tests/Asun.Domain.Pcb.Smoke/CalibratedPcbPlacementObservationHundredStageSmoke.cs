using Asun.Metrology.Core;
using Asun.Domain.Pcb;

public static class CalibratedPcbPlacementObservationHundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var component=new PcbComponentReference(
            PcbFeatureId.Create("R-CAL-001"),
            "R101",
            "10k",
            "0402",
            0,
            PcbLayerSide.Top,
            new PcbCoordinate(12,23),
            0);
        var correspondences=new[]{
            new CalibrationCorrespondence2D(new MetrologyPoint2D(0,0),new MetrologyPoint2D(10,20)),
            new CalibrationCorrespondence2D(new MetrologyPoint2D(1,0),new MetrologyPoint2D(12,20)),
            new CalibrationCorrespondence2D(new MetrologyPoint2D(0,1),new MetrologyPoint2D(10,23)),
            new CalibrationCorrespondence2D(new MetrologyPoint2D(2,2),new MetrologyPoint2D(14,26))
        };
        var calibration=AffineCalibrationRuntime.Fit(correspondences);
        var source=new MetrologyPoint2D(1,1);
        var observation=CalibratedPcbPlacementObservationRuntime.Measure(
            component,
            source,
            correspondences,
            calibration);
        var tampered=observation with {CalibrationFingerprint=new string('a',64)};
        var shifted=CalibratedPcbPlacementObservationRuntime.Measure(
            component,
            new MetrologyPoint2D(1.1,1),
            correspondences,
            calibration);

        for(var i=0;i<10;i++) Check(calibration.PointCount==4,"Calibration should retain four correspondences.");
        for(var i=0;i<10;i++) Check(AffineCalibrationValidationRuntime.IsValid(correspondences,calibration),"Calibration should validate independently.");
        for(var i=0;i<10;i++) Check(observation.SourceMeasuredPosition==source,"Calibrated observation should retain the source measurement point.");
        for(var i=0;i<10;i++) Check(observation.Observation.MeasuredPosition==new MetrologyPoint2D(12,23),"Calibration should map source measurement into board coordinates.");
        for(var i=0;i<10;i++) Check(Math.Abs(observation.Observation.ErrorDistance)<=1e-12,"Mapped placement should land on the expected component position.");
        for(var i=0;i<10;i++) Check(observation.Observation.Delta==MetrologyPoint2D.Zero,"Expected component delta should be zero for the calibrated point.");
        for(var i=0;i<10;i++) Check(CalibratedPcbPlacementObservationValidationRuntime.IsValid(component,source,correspondences,calibration,observation),"Calibrated placement observation should validate.");
        for(var i=0;i<10;i++) Check(!CalibratedPcbPlacementObservationValidationRuntime.IsValid(component,source,correspondences,calibration,tampered),"Calibration fingerprint tampering should be rejected.");
        for(var i=0;i<10;i++) Check(shifted.Observation.MeasuredPosition!=observation.Observation.MeasuredPosition,"Changed source measurements should change calibrated position.");
        for(var i=0;i<10;i++) Check(observation.Fingerprint.Length==64,"Calibrated observation fingerprint should be fixed width.");

        assert(round==100,$"Calibrated PCB placement smoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
