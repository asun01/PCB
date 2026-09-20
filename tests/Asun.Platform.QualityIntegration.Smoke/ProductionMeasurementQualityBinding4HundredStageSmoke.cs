using Asun.Domain.Pcb;
using Asun.Domain.Quality;
using Asun.Metrology.Core;
using Asun.Platform.MetrologyProductionIntegration;
using Asun.Platform.QualityIntegration;

public static class ProductionMeasurementQualityBinding4HundredStageSmoke
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
        var malformed=binding with {Fingerprint="bad"};
var upper=binding with {Fingerprint=new string('F',64)};
var badSequence=binding with {Sequence=8};
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityBindingRuntime.IsValid(fact,observation,measurementBinding,qualityEvaluation,malformed),"Malformed binding fingerprint should be rejected.");
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityBindingRuntime.IsValid(fact,observation,measurementBinding,qualityEvaluation,upper),"Uppercase binding fingerprint should be rejected.");
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityBindingRuntime.IsValid(fact,observation,measurementBinding,qualityEvaluation,badSequence),"Sequence tampering should be rejected.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityBindingRuntime.Validate(fact,observation,measurementBinding,qualityEvaluation,malformed).Count>0,"Malformed fingerprint should produce diagnostics.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityBindingRuntime.Validate(fact,observation,measurementBinding,qualityEvaluation,badSequence).Count>0,"Sequence tampering should produce diagnostics.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityBindingRuntime.IsValid(fact,observation,measurementBinding,qualityEvaluation,binding),"Baseline binding should remain valid.");
for(var i=0;i<10;i++) Check(binding.Sequence==fact.Sequence,"Baseline sequence should remain stable.");
for(var i=0;i<10;i++) Check(binding.QualitySnapshotId==qualityEvaluation.Result.SnapshotId,"Baseline Quality snapshot should remain stable.");
for(var i=0;i<10;i++) Check(binding.Fingerprint.All(Uri.IsHexDigit),"Baseline fingerprint should be hexadecimal.");
        assert(round==100,$"ProductionMeasurementQualityBinding4HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
