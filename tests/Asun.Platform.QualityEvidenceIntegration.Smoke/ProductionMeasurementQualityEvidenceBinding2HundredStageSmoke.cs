using Asun.Domain.Quality;
using Asun.Platform.Evidence;
using Asun.Platform.QualityEvidenceIntegration;

public static class ProductionMeasurementQualityEvidenceBinding2HundredStageSmoke
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
        var badFinding=binding with {FindingId=QualityFindingId.Create("OTHER")};
var badEvidence=binding with {EvidenceFingerprint=new string('d',64)};
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityEvidenceBindingRuntime.IsValid(qualityBinding,qualityRun,evidenceBindings,badFinding),"Finding tampering should be rejected.");
for(var i=0;i<10;i++) Check(!ProductionMeasurementQualityEvidenceBindingRuntime.IsValid(qualityBinding,qualityRun,evidenceBindings,badEvidence),"Evidence fingerprint tampering should be rejected.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceBindingRuntime.Validate(qualityBinding,qualityRun,evidenceBindings,badFinding).Count>0,"Finding tampering should produce diagnostics.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceBindingRuntime.Validate(qualityBinding,qualityRun,evidenceBindings,badEvidence).Count>0,"Evidence tampering should produce diagnostics.");
for(var i=0;i<10;i++) Check(binding.FindingId==findingId,"Baseline finding identity should remain stable.");
for(var i=0;i<10;i++) Check(binding.EvidenceFingerprint.Length==64,"Baseline evidence identity should remain fixed width.");
for(var i=0;i<10;i++) Check(ProductionMeasurementQualityEvidenceBindingRuntime.IsValid(qualityBinding,qualityRun,evidenceBindings,binding),"Baseline binding should remain valid.");
for(var i=0;i<10;i++) Check(binding.ComponentId==componentId.Value,"Baseline component identity should remain stable.");
        assert(round==100,$"ProductionMeasurementQualityEvidenceBinding2HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
