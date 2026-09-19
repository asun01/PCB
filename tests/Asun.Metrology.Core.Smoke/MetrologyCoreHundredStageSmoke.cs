using Asun.Metrology.Core;

public static class MetrologyCoreHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var first=new MetrologyPoint2D(10,20);
        var second=new MetrologyPoint2D(40,60);
        var transform=new AffineTransform2D(2,0,0,2,5,-3);
        var transformed=transform.Transform(first);
        var inverseOk=transform.TryInvert(out var inverse);
        var coordinateSystem=new MetrologyCoordinateSystem2D(
            new MetrologyPoint2D(100,50),
            new MetrologyPoint2D(1,0),
            new MetrologyPoint2D(0,1));
        var segment=new MetrologySegment2D(first,second);
        var measurement=DistanceMeasurementRuntime.Measure(first,second,"mm");

        for(var i=0;i<10;i++) Check(first.IsFinite && second.IsFinite,"Metrology points should remain finite.");
        for(var i=0;i<10;i++) Check(Math.Abs(first.DistanceTo(second)-50)<1e-12,"Point distance should be 50.");
        for(var i=0;i<10;i++) Check(transformed==new MetrologyPoint2D(25,37),"Affine transform should map points deterministically.");
        for(var i=0;i<10;i++) Check(inverseOk && inverse.Transform(transformed)==first,"Affine inversion should recover the original point.");
        for(var i=0;i<10;i++) Check(coordinateSystem.IsValid,"Coordinate system should be orthonormal.");
        for(var i=0;i<10;i++) Check(coordinateSystem.ToLocal(new MetrologyPoint2D(110,70))==new MetrologyPoint2D(10,20),"World-to-local conversion should be deterministic.");
        for(var i=0;i<10;i++) Check(coordinateSystem.ToWorld(new MetrologyPoint2D(10,20))==new MetrologyPoint2D(110,70),"Local-to-world conversion should be deterministic.");
        for(var i=0;i<10;i++) Check(Math.Abs(segment.Length-50)<1e-12 && segment.Midpoint==new MetrologyPoint2D(25,40),"Segment geometry should be deterministic.");
        for(var i=0;i<10;i++) Check(DistanceMeasurementResultValidationRuntime.IsValid(measurement),"Distance measurement result should validate.");
        for(var i=0;i<10;i++) Check(measurement.ResultFingerprint.Length==64 && measurement.Unit=="mm","Measurement result integrity should remain stable.");

        assert(round==100,$"Metrology core smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
