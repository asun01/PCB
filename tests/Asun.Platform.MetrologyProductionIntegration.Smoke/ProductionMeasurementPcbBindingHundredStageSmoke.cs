using Asun.Domain.Pcb;
using Asun.Metrology.Core;
using Asun.Platform.MetrologyProductionIntegration;

public static class ProductionMeasurementPcbBindingHundredStageSmoke
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
            PcbFeatureId.Create("C1"),
            "C1",
            "100nF",
            "0402",
            0,
            PcbLayerSide.Top,
            new PcbCoordinate(10,20),
            0);
        var observation=new CalibratedPcbPlacementObservation(
            new PcbPlacementObservation(
                component.Id,
                component.Designator,
                new MetrologyPoint2D(10,20),
                new MetrologyPoint2D(10.1,20.2),
                new MetrologyPoint2D(0.1,0.2),
                0.223606797749979),
            new MetrologyPoint2D(100,200),
            new string('a',64),
            new string('b',64));
        var fact=new ProductionMeasurementFact(
            7,
            new string('c',64),
            observation.Observation.ComponentId,
            observation.SourceMeasuredPosition,
            observation.Observation.MeasuredPosition,
            observation.Observation.ErrorDistance,
            observation.CalibrationFingerprint,
            observation.Fingerprint);
        var binding=ProductionMeasurementPcbBindingRuntime.Create(fact,observation);
        var copy=binding with {};
        var tampered=binding with {ObservationFingerprint=new string('d',64)};
        var descriptor=ProductionMeasurementPcbBindingRuntime.CreateReplayDescriptor(binding);

        for(var i=0;i<10;i++) Check(binding.Sequence==fact.Sequence,"Binding should preserve measurement sequence.");
        for(var i=0;i<10;i++) Check(binding.ProductionInputFingerprint==fact.ProductionInputFingerprint,"Binding should preserve Production input identity.");
        for(var i=0;i<10;i++) Check(binding.ComponentId==observation.Observation.ComponentId,"Binding should preserve PCB component identity.");
        for(var i=0;i<10;i++) Check(binding.Designator==observation.Observation.Designator,"Binding should preserve component designator.");
        for(var i=0;i<10;i++) Check(binding.CalibrationFingerprint==observation.CalibrationFingerprint,"Binding should preserve calibration identity.");
        for(var i=0;i<10;i++) Check(binding.ObservationFingerprint==observation.Fingerprint,"Binding should preserve calibrated observation identity.");
        for(var i=0;i<10;i++) Check(ProductionMeasurementPcbBindingRuntime.Validate(fact,observation,binding).Count==0,"Valid measurement PCB binding should validate.");
        for(var i=0;i<10;i++) Check(ProductionMeasurementPcbBindingRuntime.Validate(fact,observation,tampered).Count>0,"Observation tampering should be rejected.");
        for(var i=0;i<10;i++) Check(ProductionMeasurementPcbBindingRuntime.CreateCanonicalKey(binding).Length==64,"Measurement PCB canonical key should be fixed width.");
        for(var i=0;i<10;i++) Check(ProductionMeasurementPcbBindingRuntime.IsEquivalent(binding,copy),"Equivalent measurement PCB bindings should be recognized.");
        for(var i=0;i<10;i++) Check(descriptor.ComponentId==binding.ComponentId,"Replay descriptor should preserve PCB component identity.");

        assert(round==100,$"Measurement PCB binding smoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
