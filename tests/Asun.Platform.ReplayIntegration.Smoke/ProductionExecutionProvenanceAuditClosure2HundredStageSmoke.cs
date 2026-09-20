using Asun.Domain.Pcb;
using Asun.Platform.PcbExecutionIntegration;
using Asun.Platform.QualityEvidenceIntegration;
using Asun.Platform.ReplayIntegration;
using Asun.Production.Runtime;

public static class ProductionExecutionProvenanceAuditClosure2HundredStageSmoke
{
    public static ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var sessionId=Guid.Parse("D1000000-0000-0000-0000-000000000001");
        var qualityRunId=Guid.Parse("D2000000-0000-0000-0000-000000000001");
        var board=new PcbBoardDefinition(
            Guid.Parse("D0000000-0000-0000-0000-000000000001"),
            "AUDIT-BOARD",
            100,
            80,
            4);
        var assembly=PcbAssemblySnapshotRuntime.Create(
            board,
            Array.Empty<PcbComponentReference>());
        var execution=new PcbExecutionSnapshot(
            assembly.Fingerprint,
            sessionId,
            new string('b',64),
            new string('c',64),
            1,
            1,
            qualityRunId,
            new string('d',64),
            new string('e',64));
        var boardBinding=PcbExecutionBoardBindingRuntime.Create(assembly,execution);

        var fpA=new string('a',64);
        var fpB=new string('b',64);
        var fpC=new string('c',64);
        var fpD=new string('d',64);
        var fpE=new string('e',64);
        var pipelineIdentity=new ProductionPipelineExecutionIdentity(sessionId,fpA,fpB,2,fpC,fpD,fpE);
        var simulationDescriptor=new ProductionSimulationReplayDescriptor(sessionId,fpC,2,fpD,new string('f',64));
        var renderDescriptors=new[]
        {
            new ProductionRenderEvidenceReplayDescriptor(1,new string('1',64),EvidenceHandle.Create("audit/render/1"),new string('1',64)),
            new ProductionRenderEvidenceReplayDescriptor(2,new string('2',64),EvidenceHandle.Create("audit/render/2"),new string('2',64))
        };
        var qualityReleaseDescriptor=new QualityReleaseReplayDescriptor(
            qualityRunId,
            new string('3',64),
            new string('4',64),
            true,
            new string('5',64),
            new string('6',64));
        var replayBinding=new ProductionQualityEvidenceReleaseReplayBinding(
            sessionId,
            qualityRunId,
            new string('7',64),
            new string('4',64),
            true,
            new string('8',64));
        var unifiedClosure=UnifiedReplayClosureRuntime.Create(
            pipelineIdentity,
            simulationDescriptor,
            renderDescriptors,
            qualityReleaseDescriptor,
            replayBinding);

        var measurementReleaseBinding=new ProductionMeasurementQualityEvidenceReleaseBinding(
            7,
            new string('b',64),
            Guid.Parse("D3000000-0000-0000-0000-000000000001"),
            "C1",
            new string('d',64),
            new string('e',64),
            true,
            new string('f',64));
        var replayDescriptor=ProductionMeasurementQualityEvidenceReleaseReplayDescriptorRuntime.Create(
            measurementReleaseBinding,
            replayBinding);
        var provenance=new ProductionFrameProvenance(
            new Asun.Device.Contracts.FrameSequence(7),
            640,
            480,
            "Mono8",
            DateTimeOffset.Parse("2026-09-20T05:00:00Z"),
            new string('b',64));
        var provenanceDescriptor=ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.Create(
            provenance,
            replayDescriptor);
        var closure=ProductionExecutionProvenanceAuditClosureRuntime.Create(
            boardBinding,
            unifiedClosure,
            provenanceDescriptor,
            replayDescriptor);

        var badSession=closure with {ProductionSessionId=Guid.NewGuid()};
var badAssembly=closure with {AssemblyFingerprint=new string('9',64)};
for(var i=0;i<10;i++) Check(!ProductionExecutionProvenanceAuditClosureRuntime.IsValid(boardBinding,unifiedClosure,provenanceDescriptor,replayDescriptor,badSession),"Session tampering should be rejected.");
for(var i=0;i<10;i++) Check(!ProductionExecutionProvenanceAuditClosureRuntime.IsValid(boardBinding,unifiedClosure,provenanceDescriptor,replayDescriptor,badAssembly),"Assembly tampering should be rejected.");
for(var i=0;i<10;i++) Check(ProductionExecutionProvenanceAuditClosureRuntime.Validate(boardBinding,unifiedClosure,provenanceDescriptor,replayDescriptor,badSession).Count>0,"Session tampering should emit diagnostics.");
for(var i=0;i<10;i++) Check(ProductionExecutionProvenanceAuditClosureRuntime.Validate(boardBinding,unifiedClosure,provenanceDescriptor,replayDescriptor,badAssembly).Count>0,"Assembly tampering should emit diagnostics.");
for(var i=0;i<10;i++) Check(closure.ProductionSessionId==boardBinding.ProductionSessionId,"Baseline session should remain stable.");
for(var i=0;i<10;i++) Check(closure.AssemblyFingerprint==boardBinding.AssemblyFingerprint,"Baseline assembly identity should remain stable.");
for(var i=0;i<10;i++) Check(closure.PcbExecutionBindingFingerprint==boardBinding.BindingFingerprint,"Baseline PCB binding should remain stable.");
for(var i=0;i<10;i++) Check(closure.Fingerprint.All(Uri.IsHexDigit),"Baseline audit fingerprint should remain hexadecimal.");
for(var i=0;i<10;i++) Check(ProductionExecutionProvenanceAuditClosureRuntime.IsValid(boardBinding,unifiedClosure,provenanceDescriptor,replayDescriptor,closure),"Baseline audit closure should remain valid.");
for(var i=0;i<10;i++) Check(closure.Fingerprint==ProductionExecutionProvenanceAuditClosureRuntime.Create(boardBinding,unifiedClosure,provenanceDescriptor,replayDescriptor).Fingerprint,"Baseline creation should remain deterministic.");

        assert(round==100,$"ProductionExecutionProvenanceAuditClosure2HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
        return ValueTask.CompletedTask;
    }
}
