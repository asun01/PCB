using Asun.Domain.Pcb;
using Asun.Domain.Quality;
using Asun.Metrology.Core;
using Asun.Platform.MetrologyProductionIntegration;
using Asun.Platform.QualityIntegration;

public static class ProductionMeasurementQualityBinding1HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var component=new PcbComponentReference(
            PcbFeatureId.Create("C1"),"C1","100nF","0402",0,PcbLayerSide.Top,new PcbCoordinate(10,20),0);
        var observation=new CalibratedPcbPlacementObservation(
            new PcbPlacementObservation(
                component.Id,component.Designator,
                new MetrologyPoint2D(10,20),
                new MetrologyPoint2D(10.1,20.2),
                new MetrologyPoint2D(0.1,0.2),
                0.223606797749979),
            new MetrologyPoint2D(100,200),
            new string('a',64),
            new string('b',64));
        var fact=new ProductionMeasurementFact(
            7,new string('c',64),observation.Observation.ComponentId,
            observation.SourceMeasuredPosition,
            observation.Observation.MeasuredPosition,
            observation.Observation.ErrorDistance,
            observation.CalibrationFingerprint,
            observation.Fingerprint);
        var measurementBinding=ProductionMeasurementPcbBindingRuntime.Create(fact,observation);
        var qualityEvaluation=PcbPlacementQualityEvaluationRuntime.Evaluate(
            observation,7,
            Guid.Parse("D7000000-0000-0000-0000-000000000001"),
            Guid.Parse("D7100000-0000-0000-0000-000000000001"),
            placement=>new QualityFinding(
                QualityFindingId.Create("PCB.PLACEMENT.OBSERVED"),
                QualityOutcome.Informational,
                QualitySeverity.Information,
                $"Observed placement error {placement.ErrorDistance:R}"));
        var binding=ProductionMeasurementQualityBindingRuntime.Create(fact,observation,measurementBinding,qualityEvaluation);
        for(var i=0;i<10;i++) Check(binding.Sequence==fact.Sequence,"Binding should preserve measurement sequence.");
for(var i=0;i<10;i++) Check(binding.ProductionInputFingerprint==fact.ProductionInputFingerprint,"Binding should preserve Production input identity.");
for(var i=0;i<10;i++) Check(binding.ComponentId==observation.Observation.ComponentId,"Binding should preserve PCB component identity.");
for(var i=0;i<10;i++) Check(binding.CalibrationFingerprint==observation.CalibrationFingerprint,"Binding should preserve calibration identity.");
for(var i=0;i<10;i++) Check(binding.MeasurementObservationFingerprint==observation.Fingerprint,"Binding should preserve metrology observation identity.");
for(var i=0;i<10;i++) Check(binding.QualityResultId==qualityEvaluation.Result.ResultId,"Binding should preserve Quality result identity.");
for(var i=0;i<10;i++) Check(binding.QualitySnapshotId==qualityEvaluation.Result.SnapshotId,"Binding should preserve Quality snapshot identity.");
for(var i=0;i<10;i++) Check(binding.QualityEvaluationFingerprint==qualityEvaluation.Fingerprint,"Binding should preserve Quality evaluation identity.");
for(var i=0;i<10;i++) Check(binding.Fingerprint.Length==64,"Binding fingerprint should be fixed width.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityBindingRuntime.IsValid(fact,observation,measurementBinding,qualityEvaluation,binding),"Canonical measurement-quality binding should validate.");
        assert(round==100,$"ProductionMeasurementQualityBinding1HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
