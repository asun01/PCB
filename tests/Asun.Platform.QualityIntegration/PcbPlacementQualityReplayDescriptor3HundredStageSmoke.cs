using Asun.Domain.Pcb;
using Asun.Domain.Quality;
using Asun.Metrology.Core;
using Asun.Platform.QualityIntegration;

public static class PcbPlacementQualityReplayDescriptor3HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var component=new PcbComponentReference(PcbFeatureId.Create("Q-OBS-001"),"C201","100nF","0402",0,PcbLayerSide.Top,new PcbCoordinate(40,50),0);
        var observation=PcbPlacementObservationRuntime.Measure(component,new MetrologyPoint2D(40.25,50.10));
        var evaluation=PcbPlacementQualityEvaluationRuntime.Evaluate(observation,1,Guid.Parse("72000000-0000-0000-0000-000000000001"),Guid.Parse("73000000-0000-0000-0000-000000000001"),placement=>new QualityFinding(QualityFindingId.Create("PCB.PLACEMENT.OBSERVED"),QualityOutcome.Informational,QualitySeverity.Information,$"Observed placement error {placement.ErrorDistance:R}"));
        var descriptor=PcbPlacementQualityReplayDescriptorRuntime.Create(evaluation);
        var bad=descriptor with {EvaluationFingerprint=new string('f',64)};
        var badComponent=descriptor with {ComponentId=PcbFeatureId.Create("Q-OBS-OTHER")};
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.IsValid(evaluation,descriptor),"Baseline descriptor should validate.");
        for(var i=0;i<10;i++) Check(!PcbPlacementQualityReplayDescriptorRuntime.IsValid(evaluation,bad),"Evaluation fingerprint tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!PcbPlacementQualityReplayDescriptorRuntime.IsValid(evaluation,badComponent),"Component identity tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!PcbPlacementQualityReplayDescriptorRuntime.IsEquivalent(descriptor,bad),"Evaluation fingerprint mutation should alter identity.");
        for(var i=0;i<10;i++) Check(!PcbPlacementQualityReplayDescriptorRuntime.IsEquivalent(descriptor,badComponent),"Component mutation should alter identity.");
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.Validate(evaluation,bad).Count>0,"Fingerprint mutation should be reported.");
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.Validate(evaluation,badComponent).Count>0,"Component mutation should be reported.");
        for(var i=0;i<10;i++) Check(descriptor.EvaluationFingerprint==evaluation.Fingerprint,"Baseline evaluation fingerprint should remain intact.");
        for(var i=0;i<10;i++) Check(descriptor.ComponentId==evaluation.Observation.ComponentId,"Baseline component identity should remain intact.");
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.IsValid(evaluation,descriptor),"Baseline descriptor should remain valid.");
        assert(round==100,$"PcbPlacementQualityReplayDescriptor3HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
