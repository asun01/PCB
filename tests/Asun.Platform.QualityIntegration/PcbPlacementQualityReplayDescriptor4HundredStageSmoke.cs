using Asun.Domain.Pcb;
using Asun.Domain.Quality;
using Asun.Metrology.Core;
using Asun.Platform.QualityIntegration;

public static class PcbPlacementQualityReplayDescriptor4HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var component=new PcbComponentReference(PcbFeatureId.Create("Q-OBS-001"),"C201","100nF","0402",0,PcbLayerSide.Top,new PcbCoordinate(40,50),0);
        var observation=PcbPlacementObservationRuntime.Measure(component,new MetrologyPoint2D(40.25,50.10));
        var evaluation=PcbPlacementQualityEvaluationRuntime.Evaluate(observation,1,Guid.Parse("72000000-0000-0000-0000-000000000001"),Guid.Parse("73000000-0000-0000-0000-000000000001"),placement=>new QualityFinding(QualityFindingId.Create("PCB.PLACEMENT.OBSERVED"),QualityOutcome.Informational,QualitySeverity.Information,$"Observed placement error {placement.ErrorDistance:R}"));
        var descriptor=PcbPlacementQualityReplayDescriptorRuntime.Create(evaluation);
        var badSequence=descriptor with {Sequence=99};
        var badFingerprint=descriptor with {DescriptorFingerprint="bad"};
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.IsValid(evaluation,descriptor),"Canonical descriptor should validate.");
        for(var i=0;i<10;i++) Check(!PcbPlacementQualityReplayDescriptorRuntime.IsValid(evaluation,badSequence),"Sequence tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!PcbPlacementQualityReplayDescriptorRuntime.IsValid(evaluation,badFingerprint),"Malformed descriptor fingerprint should be rejected.");
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.Validate(evaluation,badSequence).Count>0,"Sequence tampering should produce errors.");
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.Validate(evaluation,badFingerprint).Count>0,"Malformed fingerprint should produce errors.");
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.Create(evaluation).Sequence==descriptor.Sequence,"Descriptor sequence should be deterministic.");
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.Create(evaluation).ResultId==descriptor.ResultId,"Descriptor result identity should be deterministic.");
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.Create(evaluation).SnapshotId==descriptor.SnapshotId,"Descriptor snapshot identity should be deterministic.");
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.IsEquivalent(descriptor,PcbPlacementQualityReplayDescriptorRuntime.Create(evaluation)),"Repeated creation should be equivalent.");
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.IsValid(evaluation,descriptor),"Baseline descriptor should remain valid.");
        assert(round==100,$"PcbPlacementQualityReplayDescriptor4HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
