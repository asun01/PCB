using Asun.Platform.Evidence;
using Asun.Platform.PipelineProductionIntegration;
using Asun.Platform.QualityReleaseIntegration;
using Asun.Platform.RenderIntegration;
using Asun.Platform.ReplayIntegration;
using Asun.Platform.SimulationIntegration;

public static class UnifiedReplayClosure5HundredStageSmoke
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
        var reversed=renderDescriptors.Reverse().ToArray();
var equivalent=UnifiedReplayClosureRuntime.Create(pipelineIdentity,simulationDescriptor,reversed,qualityReleaseDescriptor,releaseBinding);
for(var i=0;i<10;i++) Check(equivalent.RenderEvidenceDescriptorFingerprint==closure.RenderEvidenceDescriptorFingerprint,"Render descriptor order should not change canonical fingerprint.");
for(var i=0;i<10;i++) Check(equivalent.Fingerprint==closure.Fingerprint,"Render descriptor order should not change unified closure identity.");
for(var i=0;i<10;i++) Check(UnifiedReplayClosureRuntime.IsValid(pipelineIdentity,simulationDescriptor,reversed,qualityReleaseDescriptor,releaseBinding,equivalent),"Reordered Render descriptors should validate.");
for(var i=0;i<10;i++) Check(closure.Fingerprint.All(Uri.IsHexDigit),"Unified closure fingerprint should be hexadecimal.");
for(var i=0;i<10;i++) Check(closure.SessionId!=Guid.Empty,"Unified closure session identity should be valid.");
for(var i=0;i<10;i++) Check(closure.ProgramFingerprint.Length==64,"Program identity should be fixed width.");
for(var i=0;i<10;i++) Check(closure.SimulationDescriptorFingerprint.Length==64,"Simulation identity should be fixed width.");
for(var i=0;i<10;i++) Check(closure.QualityReleaseDescriptorFingerprint.Length==64,"Quality/Release identity should be fixed width.");
for(var i=0;i<10;i++) Check(UnifiedReplayClosureRuntime.IsValid(pipelineIdentity,simulationDescriptor,renderDescriptors,qualityReleaseDescriptor,releaseBinding,closure),"Final unified closure validation should remain clean.");
        assert(round==100,$"UnifiedReplayClosure5HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
