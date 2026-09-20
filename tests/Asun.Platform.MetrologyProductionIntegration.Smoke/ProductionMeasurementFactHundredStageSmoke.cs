using Asun.Device.Impl;
using Asun.Device.Contracts;
using Asun.Domain.Pcb;
using Asun.Metrology.Core;
using Asun.Platform.MetrologyProductionIntegration;
using Asun.Platform.Pipeline;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class ProductionMeasurementFactHundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var board=new PcbBoardDefinition(
            Guid.Parse("A5000000-0000-0000-0000-000000000001"),
            "MeasurementBoard",
            100,
            80,
            4);
        var component=new PcbComponentReference(
            PcbFeatureId.Create("M-R1"),
            "R1",
            "10k",
            "0402",
            0,
            PcbLayerSide.Top,
            new PcbCoordinate(12,23),
            0);
        var assembly=PcbAssemblySnapshotRuntime.Create(board,new[]{component});
        var correspondences=new[]
        {
            new CalibrationCorrespondence2D(new MetrologyPoint2D(0,0),new MetrologyPoint2D(12,23)),
            new CalibrationCorrespondence2D(new MetrologyPoint2D(1,0),new MetrologyPoint2D(14,23)),
            new CalibrationCorrespondence2D(new MetrologyPoint2D(0,1),new MetrologyPoint2D(12,26)),
            new CalibrationCorrespondence2D(new MetrologyPoint2D(2,2),new MetrologyPoint2D(16,29))
        };
        var calibration=AffineCalibrationRuntime.Fit(correspondences);
        var observation1=CalibratedPcbPlacementObservationRuntime.Measure(
            component,
            new MetrologyPoint2D(0,0),
            correspondences,
            calibration);
        var observation2=CalibratedPcbPlacementObservationRuntime.Measure(
            component,
            new MetrologyPoint2D(0.1,0.05),
            correspondences,
            calibration);

        var program=new InspectionProgram(
            Guid.Parse("A6000000-0000-0000-0000-000000000001"),
            "MetrologyProductionProgram",
            new Version(1,0,0),
            new[]
            {
                new ProgramStep(
                    Guid.Parse("A7000000-0000-0000-0000-000000000001"),
                    1,
                    ProgramStepKind.Acquire,
                    "Acquire",
                    Array.Empty<ProgramParameter>())
            });
        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(
            new[]
            {
                new PipelineStage<CapturedFrame>(1,"Acquire",frame=>frame)
            });
        var definition=new ProductionSessionDefinition(
            Guid.Parse("A8000000-0000-0000-0000-000000000001"),
            plan,
            pipeline,
            2);
        var recorder=new RecordingFrameSource(new SimulatedFrameSource(4,4));
        var production=await ProductionSessionRuntime.RunAsync(definition,recorder);

        var facts=ProductionMeasurementFactRuntime.Create(
            production,
            new long[]{1,2},
            new[]{observation1,observation2});
        var tampered=facts.ToArray();
        tampered[0]=tampered[0] with
        {
            ProductionInputFingerprint=new string('f',64)
        };
        var shifted=facts.ToArray();
        shifted[1]=shifted[1] with
        {
            Sequence=3
        };

        for(var i=0;i<10;i++) Check(production.FrameCount==2,"Production should retain two measurement frames.");
        for(var i=0;i<10;i++) Check(facts.Count==2,"Measurement fact projection should contain two facts.");
        for(var i=0;i<10;i++) Check(facts[0].Sequence==1 && facts[1].Sequence==2,"Measurement facts should preserve production sequence.");
        for(var i=0;i<10;i++) Check(facts.All(fact=>fact.ProductionInputFingerprint.Length==64),"Measurement facts should preserve production input fingerprints.");
        for(var i=0;i<10;i++) Check(facts.All(fact=>fact.CalibrationFingerprint==calibration.Fingerprint),"Measurement facts should preserve calibration identity.");
        for(var i=0;i<10;i++) Check(facts[0].ErrorDistance<=1e-12,"First calibrated measurement should land on the expected point.");
        for(var i=0;i<10;i++) Check(facts[1].ErrorDistance>0,"Second measurement should retain a non-zero geometric residual.");
        for(var i=0;i<10;i++) Check(ProductionMeasurementFactValidationRuntime.IsValid(production,facts),"Measurement facts should validate against production.");
        for(var i=0;i<10;i++) Check(!ProductionMeasurementFactValidationRuntime.IsValid(production,tampered),"Production input fingerprint tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionMeasurementFactValidationRuntime.IsValid(production,shifted),"Measurement sequence drift should be rejected.");

        assert(round==100,$"Production measurement fact smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
