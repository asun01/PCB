using Asun.Domain.Pcb;
using Asun.Domain.Quality;
using Asun.Metrology.Core;
using Asun.Platform.QualityIntegration;

public static class PcbPlacementQualityReplayDescriptor1HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var component=new PcbComponentReference(PcbFeatureId.Create("Q-OBS-001"),"C201","100nF","0402",0,PcbLayerSide.Top,new PcbCoordinate(40,50),0);
        var observation=PcbPlacementObservationRuntime.Measure(component,new MetrologyPoint2D(40.25,50.10));
        var evaluation=PcbPlacementQualityEvaluationRuntime.Evaluate(observation,1,Guid.Parse("72000000-0000-0000-0000-000000000001"),Guid.Parse("73000000-0000-0000-0000-000000000001"),placement=>new QualityFinding(QualityFindingId.Create("PCB.PLACEMENT.OBSERVED"),QualityOutcome.Informational,QualitySeverity.Information,$"Observed placement error {placement.ErrorDistance:R}"));
        var descriptor=PcbPlacementQualityReplayDescriptorRuntime.Create(evaluation);
        for(var i=0;i<10;i++) Check(descriptor.ComponentId==evaluation.Observation.ComponentId,"Replay descriptor should preserve PCB component identity.");
        for(var i=0;i<10;i++) Check(descriptor.Sequence==evaluation.Result.Sequence,"Replay descriptor should preserve quality sequence.");
        for(var i=0;i<10;i++) Check(descriptor.ResultId==evaluation.Result.ResultId,"Replay descriptor should preserve result identity.");
        for(var i=0;i<10;i++) Check(descriptor.SnapshotId==evaluation.Result.SnapshotId,"Replay descriptor should preserve snapshot identity.");
        for(var i=0;i<10;i++) Check(descriptor.EvaluationFingerprint==evaluation.Fingerprint,"Replay descriptor should preserve evaluation fingerprint.");
        for(var i=0;i<10;i++) Check(descriptor.DescriptorFingerprint.Length==64,"Replay descriptor fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.IsValid(evaluation,descriptor),"Canonical replay descriptor should validate.");
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.IsEquivalent(descriptor,descriptor),"Descriptor should be equivalent to itself.");
        for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.IsValid(evaluation,descriptor),"Repeated descriptor validation should remain deterministic.");
        for(var i=0;i<10;i++) Check(descriptor.DescriptorFingerprint.All(Uri.IsHexDigit),"Descriptor fingerprint should be hexadecimal.");
        assert(round==100,$"PcbPlacementQualityReplayDescriptor1HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
