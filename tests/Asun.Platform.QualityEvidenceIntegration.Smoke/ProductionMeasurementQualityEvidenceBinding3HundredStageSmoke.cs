using Asun.Domain.Quality;
using Asun.Platform.Evidence;
using Asun.Platform.QualityEvidenceIntegration;

public static class ProductionMeasurementQualityEvidenceBinding3HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var componentId=Asun.Domain.Pcb.PcbFeatureId.Create("C1");
        var observation=new Asun.Metrology.Core.CalibratedPcbPlacementObservation(
            new Asun.Metrology.Core.PcbPlacementObservation(
                componentId,"C1",
                new Asun.Metrology.Core.MetrologyPoint2D(10,20),
                new Asun.Metrology.Core.MetrologyPoint2D(10.1,20.2),
                new Asun.Metrology.Core.MetrologyPoint2D(0.1,0.2),0.223606797749979),
            new Asun.Metrology.Core.MetrologyPoint2D(100,200),
            new string('a',64),new string('b',64));
        var fact=new Asun.Platform.MetrologyProductionIntegration.ProductionMeasurementFact(
            7,new string('c',64),componentId,
            observation.SourceMeasuredPosition,observation.Observation.MeasuredPosition,
            observation.Observation.ErrorDistance,
            observation.CalibrationFingerprint,observation.Fingerprint);
        var measurementBinding=Asun.Platform.MetrologyProductionIntegration.ProductionMeasurementPcbBindingRuntime.Create(fact,observation);
        var qualityEvaluation=PcbPlacementQualityEvaluationRuntime.Evaluate(
            observation,7,
            Guid.Parse("E7000000-0000-0000-0000-000000000001"),
            Guid.Parse("E7100000-0000-0000-0000-000000000001"),
            placement=>new QualityFinding(
                QualityFindingId.Create("PCB.PLACEMENT.OBSERVED"),
                QualityOutcome.Informational,
                QualitySeverity.Information,
                "Observed placement"));
        var qualityBinding=ProductionMeasurementQualityBindingRuntime.Create(fact,observation,measurementBinding,qualityEvaluation);
        var findingId=qualityEvaluation.Result.Findings.Findings[0].Id;
        var evidenceKey=QualityEvidenceKey.Create("measurement/7/placement");
        var qualityRun=QualityInspectionRunRuntime.Create(
            Guid.Parse("E7200000-0000-0000-0000-000000000001"),
            new[]{qualityEvaluation.Result with
            {
                Snapshot=qualityEvaluation.Result.Snapshot with
                {
                    Findings=qualityEvaluation.Result.Findings,
                    Evidence=new QualityFindingEvidenceSet(new[]{new QualityFindingEvidenceLink(findingId,evidenceKey)})
                }
            }});
        var evidenceBindings=new[]{
            new QualityEvidenceHandleBinding(findingId,evidenceKey,EvidenceHandle.Create("measurement/7/placement"))};
        var binding=ProductionMeasurementQualityEvidenceBindingRuntime.Create(qualityBinding,qualityRun,evidenceBindings);
        var badComponent=binding with {ComponentId="OTHER"};
var badCalibration=binding with {CalibrationFingerprint=new string('e',64)};
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityEvidenceBindingRuntime.IsValid(qualityBinding,qualityRun,evidenceBindings,badComponent),"Component tampering should be rejected.");
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityEvidenceBindingRuntime.IsValid(qualityBinding,qualityRun,evidenceBindings,badCalibration),"Calibration tampering should be rejected.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceBindingRuntime.Validate(qualityBinding,qualityRun,evidenceBindings,badComponent).Count>0,"Component tampering should produce diagnostics.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceBindingRuntime.Validate(qualityBinding,qualityRun,evidenceBindings,badCalibration).Count>0,"Calibration tampering should produce diagnostics.");
for(var i=0;i<10;i++) Check(binding.ProductionInputFingerprint==qualityBinding.ProductionInputFingerprint,"Baseline Production identity should remain stable.");
for(var i=0;i<10;i++) Check(binding.CalibrationFingerprint==qualityBinding.CalibrationFingerprint,"Baseline calibration identity should remain stable.");
for(var i=0;i<10;i++) Check(binding.EvidenceHandleCount==evidenceBindings.Length,"Baseline evidence count should remain aligned.");
for(var i=0;i<10;i++) Check(binding.Fingerprint.Length==64,"Baseline binding fingerprint should remain fixed width.");
        assert(round==100,$"ProductionMeasurementQualityEvidenceBinding3HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
