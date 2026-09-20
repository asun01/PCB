using Asun.Domain.Pcb;
using Asun.Domain.Quality;
using Asun.Metrology.Core;
using Asun.Platform.QualityIntegration;

public static class PcbPlacementQualityReplayDescriptor5HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var component=new PcbComponentReference(PcbFeatureId.Create("Q-OBS-001"),"C201","100nF","0402",0,PcbLayerSide.Top,new PcbCoordinate(40,50),0);
        var observation=PcbPlacementObservationRuntime.Measure(component,new MetrologyPoint2D(40.25,50.10));
        var evaluation=PcbPlacementQualityEvaluationRuntime.Evaluate(observation,1,Guid.Parse("72000000-0000-0000-0000-000000000001"),Guid.Parse("73000000-0000-0000-0000-000000000001"),placement=>new QualityFinding(QualityFindingId.Create("PCB.PLACEMENT.OBSERVED"),QualityOutcome.Informational,QualitySeverity.Information,$"Observed placement error {placement.ErrorDistance:R}"));
        var descriptor=PcbPlacementQualityReplayDescriptorRuntime.Create(evaluation);
        var malformed=descriptor with {DescriptorFingerprint=new string('Z',64)};
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.IsValid(evaluation,descriptor),"Baseline descriptor should validate.");
        for(var i=0;i<10;i++) Check(!PcbPlacementQualityReplayDescriptorRuntime.IsValid(evaluation,malformed),"Uppercase descriptor fingerprint should be rejected by canonical validation.");
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.Validate(evaluation,malformed).Count>0,"Malformed fingerprint should be diagnosed.");
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.Create(evaluation).DescriptorFingerprint==descriptor.DescriptorFingerprint,"Descriptor fingerprint should be deterministic.");
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.IsEquivalent(descriptor,PcbPlacementQualityReplayDescriptorRuntime.Create(evaluation)),"Repeated descriptor should remain equivalent.");
        for(var i=0;i<10;i++) Check(descriptor.ComponentId.IsValid,"Descriptor component id should remain valid.");
        for(var i=0;i<10;i++) Check(descriptor.ResultId!=Guid.Empty,"Descriptor result id should remain valid.");
        for(var i=0;i<10;i++) Check(descriptor.SnapshotId!=Guid.Empty,"Descriptor snapshot id should remain valid.");
        for(var i=0;i<10;i++) Check(descriptor.Sequence>=0,"Descriptor sequence should remain non-negative.");
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.IsValid(evaluation,descriptor),"Final baseline validation should remain clean.");
        assert(round==100,$"PcbPlacementQualityReplayDescriptor5HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
