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

public static class ProductionExecutionReleaseCandidateAuditClosure1HundredStageSmoke
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

        for(var i=0;i<10;i++) Check(closure.ProductionSessionId==production.SessionId,"Release candidate audit should preserve Production session identity.");
for(var i=0;i<10;i++) Check(closure.AuditClosureFingerprint==auditClosure.Fingerprint,"Release candidate audit should preserve execution audit identity.");
for(var i=0;i<10;i++) Check(closure.ReleaseManifestFingerprint==manifest.Fingerprint,"Release candidate audit should preserve Release manifest identity.");
for(var i=0;i<10;i++) Check(closure.ArtifactPath==manifest.Artifacts[0].Path,"Release candidate audit should preserve logical artifact path.");
for(var i=0;i<10;i++) Check(closure.ReleaseReady==ReleaseReadinessRuntime.Evaluate(manifest).Ready,"Release candidate audit should preserve factual readiness.");
for(var i=0;i<10;i++) Check(ProductionExecutionReleaseCandidateAuditClosureRuntime.IsValid(auditClosure,manifest,definition,production,closure),"Canonical Release candidate audit should validate.");
for(var i=0;i<10;i++) Check(ProductionReleaseCandidateValidationRuntime.IsValid(manifest,definition,production),"Production Release candidate should remain valid.");
for(var i=0;i<10;i++) Check(manifest.Artifacts.Count==1,"Release candidate should expose one logical artifact.");
for(var i=0;i<10;i++) Check(closure.Fingerprint.Length==64,"Release candidate audit fingerprint should be fixed width.");
for(var i=0;i<10;i++) Check(ProductionExecutionReleaseCandidateAuditClosureRuntime.Create(auditClosure,manifest,definition,production).Fingerprint==closure.Fingerprint,"Release candidate audit creation should be deterministic.");

        assert(round==100,$"ProductionExecutionReleaseCandidateAuditClosure1HundredStageSmoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
