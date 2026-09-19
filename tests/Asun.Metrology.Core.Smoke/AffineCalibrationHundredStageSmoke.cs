using Asun.Metrology.Core;

public static class AffineCalibrationHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var correspondences=new[]{
            new CalibrationCorrespondence2D(
                new MetrologyPoint2D(0,0),
                new MetrologyPoint2D(5,-2)),
            new CalibrationCorrespondence2D(
                new MetrologyPoint2D(10,0),
                new MetrologyPoint2D(25,3)),
            new CalibrationCorrespondence2D(
                new MetrologyPoint2D(0,20),
                new MetrologyPoint2D(1,38)),
            new CalibrationCorrespondence2D(
                new MetrologyPoint2D(20,30),
                new MetrologyPoint2D(45,61))
        };

        var result=AffineCalibrationRuntime.Fit(correspondences);
        var tampered=result with
        {
            MaximumError=result.MaximumError+1
        };
        var inverseOk=result.Transform.TryInvert(out var inverse);

        for(var i=0;i<10;i++) Check(result.PointCount==4,"Calibration should preserve correspondence count.");
        for(var i=0;i<10;i++) Check(Math.Abs(result.RootMeanSquareError)<1e-9,"Exact affine correspondence should have near-zero RMS error.");
        for(var i=0;i<10;i++) Check(Math.Abs(result.MaximumError)<1e-9,"Exact affine correspondence should have near-zero maximum error.");
        for(var i=0;i<10;i++) Check(AffineCalibrationValidationRuntime.IsValid(correspondences,result),"Calibration result should validate.");
        for(var i=0;i<10;i++) Check(!AffineCalibrationValidationRuntime.IsValid(correspondences,tampered),"Tampered calibration error should be rejected.");
        for(var i=0;i<10;i++) Check(result.Fingerprint.Length==64,"Calibration fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(result.Fingerprint.All(Uri.IsHexDigit),"Calibration fingerprint should be hexadecimal.");
        for(var i=0;i<10;i++) Check(inverseOk && inverse.Transform(result.Transform.Transform(new MetrologyPoint2D(20,30)))==new MetrologyPoint2D(20,30),"Calibration transform should be invertible.");
        for(var i=0;i<10;i++) Check(AffineCalibrationRuntime.Fit(correspondences)==result,"Calibration should be deterministic.");
        for(var i=0;i<10;i++) Check(correspondences.All(item=>item.IsValid),"Calibration correspondences should remain valid.");

        assert(round==100,$"Affine calibration smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
