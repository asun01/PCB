using Asun.Domain.Quality;
using Asun.Platform.Evidence;
using Asun.Platform.QualityEvidenceIntegration;

public static class ProductionMeasurementQualityEvidenceBinding5HundredStageSmoke
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
        var changedEvidence=new[]{new QualityEvidenceHandleBinding(findingId,evidenceKey,EvidenceHandle.Create("measurement/7/changed"))};
var changed=ProductionMeasurementQualityEvidenceBindingRuntime.Create(qualityBinding,qualityRun,changedEvidence);
for(var i=0;i<10;i++) Check(changed.EvidenceFingerprint!=binding.EvidenceFingerprint,"Changed opaque Evidence identity should alter Evidence fingerprint.");
for(var i=0;i<10;i++) Check(changed.Fingerprint!=binding.Fingerprint,"Changed opaque Evidence identity should alter binding fingerprint.");
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityEvidenceBindingRuntime.IsValid(qualityBinding,qualityRun,evidenceBindings,changed),"Changed evidence binding should fail against original evidence.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceBindingRuntime.IsValid(qualityBinding,qualityRun,changedEvidence,changed),"Changed evidence binding should validate against changed evidence.");
for(var i=0;i<10;i++) Check(binding.EvidenceHandleCount==1,"Original Evidence count should remain stable.");
for(var i=0;i<10;i++) Check(binding.ComponentId==componentId.Value,"Original component identity should remain stable.");
for(var i=0;i<10;i++) Check(binding.CalibrationFingerprint==qualityBinding.CalibrationFingerprint,"Original calibration identity should remain stable.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceBindingRuntime.Create(qualityBinding,qualityRun,evidenceBindings).Fingerprint==binding.Fingerprint,"Original binding creation should remain deterministic.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceBindingRuntime.IsValid(qualityBinding,qualityRun,evidenceBindings,binding),"Final baseline validation should remain clean.");
        assert(round==100,$"ProductionMeasurementQualityEvidenceBinding5HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
