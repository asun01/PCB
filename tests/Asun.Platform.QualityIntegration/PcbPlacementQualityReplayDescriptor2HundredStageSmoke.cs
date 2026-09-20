using Asun.Domain.Pcb;
using Asun.Domain.Quality;
using Asun.Metrology.Core;
using Asun.Platform.QualityIntegration;

public static class PcbPlacementQualityReplayDescriptor2HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var component=new PcbComponentReference(PcbFeatureId.Create("Q-OBS-001"),"C201","100nF","0402",0,PcbLayerSide.Top,new PcbCoordinate(40,50),0);
        var observation=PcbPlacementObservationRuntime.Measure(component,new MetrologyPoint2D(40.25,50.10));
        var evaluation=PcbPlacementQualityEvaluationRuntime.Evaluate(observation,1,Guid.Parse("72000000-0000-0000-0000-000000000001"),Guid.Parse("73000000-0000-0000-0000-000000000001"),placement=>new QualityFinding(QualityFindingId.Create("PCB.PLACEMENT.OBSERVED"),QualityOutcome.Informational,QualitySeverity.Information,$"Observed placement error {placement.ErrorDistance:R}"));
        var descriptor=PcbPlacementQualityReplayDescriptorRuntime.Create(evaluation);
        var bad=descriptor with {ResultId=Guid.NewGuid()};
        var badSnapshot=descriptor with {SnapshotId=Guid.NewGuid()};
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.IsValid(evaluation,descriptor),"Baseline descriptor should validate.");
        for(var i=0;i<10;i++) Check(!PcbPlacementQualityReplayDescriptorRuntime.IsValid(evaluation,bad),"Result identity tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!PcbPlacementQualityReplayDescriptorRuntime.IsValid(evaluation,badSnapshot),"Snapshot identity tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!PcbPlacementQualityReplayDescriptorRuntime.IsEquivalent(descriptor,bad),"Result identity mutation should alter descriptor identity.");
        for(var i=0;i<10;i++) Check(!PcbPlacementQualityReplayDescriptorRuntime.IsEquivalent(descriptor,badSnapshot),"Snapshot identity mutation should alter descriptor identity.");
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.Validate(evaluation,bad).Count>0,"Result mutation should produce validation errors.");
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.Validate(evaluation,badSnapshot).Count>0,"Snapshot mutation should produce validation errors.");
        for(var i=0;i<10;i++) Check(descriptor.DescriptorFingerprint.Length==64,"Baseline descriptor fingerprint remains valid.");
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.IsValid(evaluation,descriptor),"Negative cases must not mutate baseline.");
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.Create(evaluation).DescriptorFingerprint==descriptor.DescriptorFingerprint,"Descriptor creation should be deterministic.");
        assert(round==100,$"PcbPlacementQualityReplayDescriptor2HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
