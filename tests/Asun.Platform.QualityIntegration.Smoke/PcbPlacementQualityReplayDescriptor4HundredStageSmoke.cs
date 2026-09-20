using Asun.Domain.Pcb;
using Asun.Domain.Quality;
using Asun.Metrology.Core;
using Asun.Platform.QualityIntegration;

public static class PcbPlacementQualityReplayDescriptor4HundredStageSmoke
{
 public static ValueTask RunAsync(Action<bool,string> assert)
 {
  var round=0;
  void Check(bool c,string m){round++;assert(c,$"Round {round}: {m}");}
  var component=new PcbComponentReference(PcbFeatureId.Create("Q-OBS-001"),"C201","100nF","0402",0,PcbLayerSide.Top,new PcbCoordinate(40,50),0);
  var observation=PcbPlacementObservationRuntime.Measure(component,new MetrologyPoint2D(40.25,50.10));
  var evaluation=PcbPlacementQualityEvaluationRuntime.Evaluate(observation,1,Guid.Parse("72000000-0000-0000-0000-000000000001"),Guid.Parse("73000000-0000-0000-0000-000000000001"),placement=>new QualityFinding(QualityFindingId.Create("PCB.PLACEMENT.OBSERVED"),QualityOutcome.Informational,QualitySeverity.Information,$"Observed placement error {placement.ErrorDistance:R}"));
  var descriptor=PcbPlacementQualityReplayDescriptorRuntime.Create(evaluation);
  for(var i=0;i<10;i++) Check(descriptor.ComponentId==evaluation.Observation.ComponentId,"component identity");
  for(var i=0;i<10;i++) Check(descriptor.Sequence==evaluation.Result.Sequence,"sequence identity");
  for(var i=0;i<10;i++) Check(descriptor.ResultId==evaluation.Result.ResultId,"result identity");
  for(var i=0;i<10;i++) Check(descriptor.SnapshotId==evaluation.Result.SnapshotId,"snapshot identity");
  for(var i=0;i<10;i++) Check(descriptor.EvaluationFingerprint==evaluation.Fingerprint,"evaluation fingerprint");
  for(var i=0;i<10;i++) Check(descriptor.DescriptorFingerprint.Length==64,"descriptor fingerprint width");
  for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.IsValid(evaluation,descriptor),"descriptor valid");
  for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.IsEquivalent(descriptor,descriptor),"descriptor self equivalence");
  for(var i=0;i<10;i++) Check(PcbPlacementQualityReplayDescriptorRuntime.Create(evaluation).DescriptorFingerprint==descriptor.DescriptorFingerprint,"deterministic fingerprint");
  for(var i=0;i<10;i++) Check(descriptor.ComponentId.IsValid && descriptor.ResultId!=Guid.Empty && descriptor.SnapshotId!=Guid.Empty,"identity invariants");
  assert(round==100,$"Smoke should execute exactly 100 numbered rounds; actual {round}.");
  return ValueTask.CompletedTask;
 }
}