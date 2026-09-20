using Asun.Device.Contracts;
using Asun.Device.Impl;
using Asun.Domain.Pcb;
using Asun.Platform.Evidence;
using Asun.Platform.PcbExecutionIntegration;
using Asun.Platform.Pipeline;
using Asun.Platform.PipelineProductionIntegration;
using Asun.Platform.QualityReleaseIntegration;
using Asun.Platform.QualityEvidenceIntegration;
using Asun.Platform.ReleaseIntegration;
using Asun.Platform.RenderIntegration;
using Asun.Platform.ReplayIntegration;
using Asun.Platform.SimulationIntegration;
using Asun.Production.Runtime;
using Asun.Program.Core;
using Asun.Release.Core;

public static class ProductionExecutionReleaseCandidateAuditClosure2HundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;
        void Check(bool condition,string message){round++;assert(condition,$"Round {round}: {message}");}

        var sessionId=Guid.Parse("E1000000-0000-0000-0000-000000000001");
        var qualityRunId=Guid.Parse("E2000000-0000-0000-0000-000000000001");

        var board=new PcbBoardDefinition(
            Guid.Parse("E0000000-0000-0000-0000-000000000001"),
            "RELEASE-CANDIDATE-BOARD",
            100,
            80,
            4);
        var assembly=PcbAssemblySnapshotRuntime.Create(board,Array.Empty<PcbComponentReference>());
        var program=new InspectionProgram(
            Guid.Parse("E3000000-0000-0000-0000-000000000001"),
            "ReleaseCandidateProgram",
            new Version(1,0,0),
            new[]
            {
                new ProgramStep(
                    Guid.Parse("E4000000-0000-0000-0000-000000000001"),
                    1,
                    ProgramStepKind.Acquire,
                    "Acquire",
                    Array.Empty<ProgramParameter>())
            });
        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(
            new[]
            {
                new PipelineStage<CapturedFrame>(1,"Acquire",frame=>frame)
            });
        var definition=new ProductionSessionDefinition(
            sessionId,
            plan,
            pipeline,
            1);
        var recorder=new RecordingFrameSource(new SimulatedFrameSource(4,4));
        var production=await ProductionSessionRuntime.RunAsync(definition,recorder);
        var provenance=ProductionFrameProvenanceRuntime.Create(production,recorder.Frames);

        var identity=new ReleaseIdentity(
            "Asun PCB Release Candidate",
            new Version(1,0,0),
            "audit");
        var manifest=ProductionReleaseCandidateRuntime.Create(
            identity,
            definition,
            production,
            "release/audit");

        var boardExecution=new PcbExecutionSnapshot(
            assembly.Fingerprint,
            sessionId,
            production.Fingerprint,
            new string('c',64),
            production.FrameCount,
            1,
            qualityRunId,
            new string('d',64),
            new string('e',64));
        var boardBinding=PcbExecutionBoardBindingRuntime.Create(assembly,boardExecution);

        var pipelineIdentity=new ProductionPipelineExecutionIdentity(
            sessionId,
            new string('a',64),
            new string('b',64),
            production.FrameCount,
            production.Fingerprint,
            new string('c',64),
            new string('d',64));
        var simulationDescriptor=new ProductionSimulationReplayDescriptor(
            sessionId,
            production.Fingerprint,
            production.FrameCount,
            new string('c',64),
            new string('f',64));
        var renderDescriptors=new[]
        {
            new ProductionRenderEvidenceReplayDescriptor(
                1,
                provenance[0].PayloadFingerprint,
                EvidenceHandle.Create("release-candidate/render/1"),
                new string('1',64))
        };
        var qualityReleaseDescriptor=new QualityReleaseReplayDescriptor(
            qualityRunId,
            new string('2',64),
            manifest.Fingerprint,
            ReleaseReadinessRuntime.Evaluate(manifest).Ready,
            new string('3',64),
            new string('4',64));
        var replayBinding=new ProductionQualityEvidenceReleaseReplayBinding(
            sessionId,
            qualityRunId,
            new string('5',64),
            manifest.Fingerprint,
            ReleaseReadinessRuntime.Evaluate(manifest).Ready,
            new string('6',64));
        var unifiedClosure=UnifiedReplayClosureRuntime.Create(
            pipelineIdentity,
            simulationDescriptor,
            renderDescriptors,
            qualityReleaseDescriptor,
            replayBinding);

        var measurementReleaseBinding=new ProductionMeasurementQualityEvidenceReleaseBinding(
            provenance[0].Sequence.Value,
            provenance[0].PayloadFingerprint,
            Guid.Parse("E5000000-0000-0000-0000-000000000001"),
            "C1",
            new string('7',64),
            manifest.Fingerprint,
            ReleaseReadinessRuntime.Evaluate(manifest).Ready,
            new string('8',64));
        var replayDescriptor=ProductionMeasurementQualityEvidenceReleaseReplayDescriptorRuntime.Create(
            measurementReleaseBinding,
            replayBinding);
        var provenanceDescriptor=ProductionMeasurementQualityEvidenceReleaseProvenanceDescriptorRuntime.Create(
            provenance[0],
            replayDescriptor);
        var auditClosure=ProductionExecutionProvenanceAuditClosureRuntime.Create(
            boardBinding,
            unifiedClosure,
            provenanceDescriptor,
            replayDescriptor);
        var closure=ProductionExecutionReleaseCandidateAuditClosureRuntime.Create(
            auditClosure,
            manifest,
            definition,
            production);

        var badManifest=manifest with {Fingerprint=new string('9',64)};
var badClosure=closure with {ReleaseManifestFingerprint=new string('8',64)};
for(var i=0;i<10;i++) Check(!ProductionExecutionReleaseCandidateAuditClosureRuntime.IsValid(auditClosure,badManifest,definition,production,closure),"Manifest tampering should be rejected.");
for(var i=0;i<10;i++) Check(!ProductionExecutionReleaseCandidateAuditClosureRuntime.IsValid(auditClosure,manifest,definition,production,badClosure),"Closure manifest identity tampering should be rejected.");
for(var i=0;i<10;i++) Check(ProductionExecutionReleaseCandidateAuditClosureRuntime.Validate(auditClosure,badManifest,definition,production,closure).Count>0,"Manifest tampering should emit diagnostics.");
for(var i=0;i<10;i++) Check(ProductionExecutionReleaseCandidateAuditClosureRuntime.Validate(auditClosure,manifest,definition,production,badClosure).Count>0,"Closure manifest tampering should emit diagnostics.");
for(var i=0;i<10;i++) Check(closure.ProductionSessionId==production.SessionId,"Baseline Production session identity remains stable.");
for(var i=0;i<10;i++) Check(closure.AuditClosureFingerprint==auditClosure.Fingerprint,"Baseline audit identity remains stable.");
for(var i=0;i<10;i++) Check(closure.ReleaseManifestFingerprint==manifest.Fingerprint,"Baseline manifest identity remains stable.");
for(var i=0;i<10;i++) Check(closure.ArtifactPath==manifest.Artifacts[0].Path,"Baseline artifact identity remains stable.");
for(var i=0;i<10;i++) Check(closure.Fingerprint.All(Uri.IsHexDigit),"Baseline audit fingerprint remains hexadecimal.");
for(var i=0;i<10;i++) Check(ProductionExecutionReleaseCandidateAuditClosureRuntime.IsValid(auditClosure,manifest,definition,production,closure),"Baseline Release candidate audit remains valid.");

        assert(round==100,$"ProductionExecutionReleaseCandidateAuditClosure2HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
