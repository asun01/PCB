using Asun.Platform.Evidence;
using Asun.Platform.PipelineProductionIntegration;
using Asun.Platform.QualityReleaseIntegration;
using Asun.Platform.RenderIntegration;
using Asun.Platform.ReplayIntegration;
using Asun.Platform.SimulationIntegration;

public static class UnifiedReplayClosure3HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}
        var sessionId=Guid.Parse("C9000000-0000-0000-0000-000000000001");
        var qualityRunId=Guid.Parse("C9100000-0000-0000-0000-000000000001");
        var fpA=new string('a',64);
        var fpB=new string('b',64);
        var fpC=new string('c',64);
        var fpD=new string('d',64);
        var fpE=new string('e',64);
        var pipelineIdentity=new ProductionPipelineExecutionIdentity(sessionId,fpA,fpB,2,fpC,fpD,fpE);
        var simulationDescriptor=new ProductionSimulationReplayDescriptor(sessionId,fpC,2,fpD,new string('f',64));
        var renderDescriptors=new[]{
            new ProductionRenderEvidenceReplayDescriptor(1,new string('1',64),EvidenceHandle.Create("unified/render/1"),new string('1',64)),
            new ProductionRenderEvidenceReplayDescriptor(2,new string('2',64),EvidenceHandle.Create("unified/render/2"),new string('2',64))};
        var qualityReleaseDescriptor=new QualityReleaseReplayDescriptor(qualityRunId,new string('3',64),new string('4',64),true,new string('5',64),new string('6',64));
        var releaseBinding=new ProductionQualityEvidenceReleaseReplayBinding(sessionId,qualityRunId,new string('7',64),new string('4',64),true,new string('8',64));
        var closure=UnifiedReplayClosureRuntime.Create(pipelineIdentity,simulationDescriptor,renderDescriptors,qualityReleaseDescriptor,releaseBinding);
        var changedRender=renderDescriptors.Select(item=>item.Sequence==2?item with {DescriptorFingerprint=new string('9',64)}:item).ToArray();
var changedQuality=qualityReleaseDescriptor with {DescriptorFingerprint=new string('0',64)};
for(var i=0;i<10;i++) Check(!UnifiedReplayClosureRuntime.IsValid(pipelineIdentity,simulationDescriptor,changedRender,qualityReleaseDescriptor,releaseBinding,closure),"Render descriptor tampering should be rejected.");
for(var i=0;i<10;i++) Check(!UnifiedReplayClosureRuntime.IsValid(pipelineIdentity,simulationDescriptor,renderDescriptors,changedQuality,releaseBinding,closure),"Quality/Release identity tampering should be rejected.");
for(var i=0;i<10;i++) Check(UnifiedReplayClosureRuntime.Validate(pipelineIdentity,simulationDescriptor,changedRender,qualityReleaseDescriptor,releaseBinding,closure).Count>0,"Render tampering should produce diagnostics.");
for(var i=0;i<10;i++) Check(UnifiedReplayClosureRuntime.Validate(pipelineIdentity,simulationDescriptor,renderDescriptors,changedQuality,releaseBinding,closure).Count>0,"Quality tampering should produce diagnostics.");
for(var i=0;i<10;i++) Check(UnifiedReplayClosureRuntime.IsValid(pipelineIdentity,simulationDescriptor,renderDescriptors,qualityReleaseDescriptor,releaseBinding,closure),"Baseline closure should remain valid.");
for(var i=0;i<10;i++) Check(closure.QualityReleaseDescriptorFingerprint==qualityReleaseDescriptor.DescriptorFingerprint,"Baseline Quality/Release identity should remain stable.");
for(var i=0;i<10;i++) Check(closure.RenderEvidenceDescriptorFingerprint==UnifiedReplayClosureRuntime.CreateRenderDescriptorFingerprint(renderDescriptors),"Baseline Render fingerprint should remain stable.");
for(var i=0;i<10;i++) Check(closure.BundleReleaseBindingFingerprint==releaseBinding.Fingerprint,"Baseline Release binding identity should remain stable.");
for(var i=0;i<10;i++) Check(closure.Fingerprint.Length==64,"Baseline closure fingerprint should remain fixed width.");
for(var i=0;i<10;i++) Check(closure.QualityReleaseDescriptorFingerprint==qualityReleaseDescriptor.DescriptorFingerprint,"Baseline Quality/Release identity should remain aligned.");

        assert(round==100,$"UnifiedReplayClosure3HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
