using Asun.Domain.Quality;
using Asun.Platform.Evidence;
using Asun.Platform.QualityEvidenceIntegration;

public static class ProductionMeasurementQualityEvidenceBinding1HundredStageSmoke
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
        for(var i=0;i<10;i++) Check(binding.Sequence==qualityBinding.Sequence,"Binding should preserve sequence.");
for(var i=0;i<10;i++) Check(binding.ProductionInputFingerprint==qualityBinding.ProductionInputFingerprint,"Binding should preserve Production input identity.");
for(var i=0;i<10;i++) Check(binding.QualityResultId==qualityBinding.QualityResultId,"Binding should preserve Quality result identity.");
for(var i=0;i<10;i++) Check(binding.FindingId==findingId,"Binding should preserve Quality finding identity.");
for(var i=0;i<10;i++) Check(binding.ComponentId==componentId.Value,"Binding should preserve PCB component identity.");
for(var i=0;i<10;i++) Check(binding.CalibrationFingerprint==qualityBinding.CalibrationFingerprint,"Binding should preserve calibration identity.");
for(var i=0;i<10;i++) Check(binding.EvidenceHandleCount==1,"Binding should preserve opaque Evidence count.");
for(var i=0;i<10;i++) Check(binding.EvidenceFingerprint.Length==64,"Evidence fingerprint should be fixed width.");
for(var i=0;i<10;i++) Check(binding.Fingerprint.Length==64,"Binding fingerprint should be fixed width.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceBindingRuntime.IsValid(qualityBinding,qualityRun,evidenceBindings,binding),"Canonical measurement-quality-evidence binding should validate.");
        assert(round==100,$"ProductionMeasurementQualityEvidenceBinding1HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
