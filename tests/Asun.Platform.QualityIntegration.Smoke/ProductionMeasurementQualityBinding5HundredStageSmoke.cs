using Asun.Domain.Pcb;
using Asun.Domain.Quality;
using Asun.Metrology.Core;
using Asun.Platform.MetrologyProductionIntegration;
using Asun.Platform.QualityIntegration;

public static class ProductionMeasurementQualityBinding5HundredStageSmoke
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
        var changedEvaluation=qualityEvaluation with {Fingerprint=new string('f',64)};
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityBindingRuntime.IsValid(fact,observation,measurementBinding,changedEvaluation,binding),"Changed Quality evaluation should invalidate the binding.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityBindingRuntime.Validate(fact,observation,measurementBinding,changedEvaluation,binding).Count>0,"Changed Quality evaluation should produce diagnostics.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityBindingRuntime.Create(fact,observation,measurementBinding,qualityEvaluation).Fingerprint==binding.Fingerprint,"Original binding creation should be deterministic.");
for(var i=0;i<10;i++) Check(binding.QualityEvaluationFingerprint==qualityEvaluation.Fingerprint,"Original Quality evaluation identity should remain stable.");
for(var i=0;i<10;i++) Check(binding.QualityResultId==qualityEvaluation.Result.ResultId,"Original Quality result identity should remain stable.");
for(var i=0;i<10;i++) Check(binding.QualitySnapshotId==qualityEvaluation.Result.SnapshotId,"Original Quality snapshot identity should remain stable.");
for(var i=0;i<10;i++) Check(binding.ComponentId==observation.Observation.ComponentId,"Original component identity should remain stable.");
for(var i=0;i<10;i++) Check(binding.CalibrationFingerprint==observation.CalibrationFingerprint,"Original calibration identity should remain stable.");
for(var i=0;i<10;i++) Check(binding.Fingerprint.Length==64,"Original binding fingerprint should remain fixed width.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityBindingRuntime.IsValid(fact,observation,measurementBinding,qualityEvaluation,binding),"Final baseline validation should remain clean.");
        assert(round==100,$"ProductionMeasurementQualityBinding5HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
