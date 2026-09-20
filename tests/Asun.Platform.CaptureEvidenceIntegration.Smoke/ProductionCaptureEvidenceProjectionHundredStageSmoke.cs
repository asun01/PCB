using Asun.Device.Impl;
using Asun.Device.Contracts;
using Asun.Platform.CaptureEvidenceIntegration;
using Asun.Platform.Evidence;
using Asun.Platform.Pipeline;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class ProductionCaptureEvidenceProjectionHundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var program=new InspectionProgram(
            Guid.Parse("97000000-0000-0000-0000-000000000001"),
            "CaptureEvidenceProgram",
            new Version(1,0,0),
            new[]
            {
                new ProgramStep(
                    Guid.Parse("98000000-0000-0000-0000-000000000001"),
                    1,
                    ProgramStepKind.Acquire,
                    "Acquire",
                    Array.Empty<ProgramParameter>())
            });
        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(
            new[]
            {
                new PipelineStage<CapturedFrame>(
                    1,
                    "Acquire",
                    frame=>frame)
            });
        var definition=new ProductionSessionDefinition(
            Guid.Parse("99000000-0000-0000-0000-000000000001"),
            plan,
            pipeline,
            2);
        var recorder=new RecordingFrameSource(new SimulatedFrameSource(5,5,"Gray8"));
        var production=await ProductionSessionRuntime.RunAsync(definition,recorder);
        var provenance=ProductionFrameProvenanceRuntime.Create(production,recorder.Frames);
        var evidenceFrames=new[]
        {
            new ProductionEvidenceFrameReference(
                1,
                new[]
                {
                    EvidenceHandle.Create("capture/1/raw"),
                    EvidenceHandle.Create("capture/1/metadata")
                }),
            new ProductionEvidenceFrameReference(
                2,
                new[]
                {
                    EvidenceHandle.Create("capture/2/raw"),
                    EvidenceHandle.Create("capture/2/metadata")
                })
        };
        var references=ProductionCaptureEvidenceProjectionRuntime.Create(
            production,
            provenance,
            evidenceFrames);
        var tampered=references.ToArray();
        tampered[0]=tampered[0] with
        {
            Width=99
        };
        var unsorted=references.ToArray();
        unsorted[1]=unsorted[1] with
        {
            Handles=unsorted[1].Handles.Reverse().ToArray()
        };

        for(var i=0;i<10;i++) Check(production.FrameCount==2,"Production should retain two source frames.");
        for(var i=0;i<10;i++) Check(provenance.Count==2,"Capture provenance should retain two frames.");
        for(var i=0;i<10;i++) Check(references.Count==2,"Capture evidence projection should retain two frame references.");
        for(var i=0;i<10;i++) Check(references[0].Sequence==1 && references[1].Sequence==2,"Capture evidence references should preserve sequence.");
        for(var i=0;i<10;i++) Check(references.All(frame=>frame.PayloadFingerprint.Length==64),"Capture evidence references should retain payload SHA-256.");
        for(var i=0;i<10;i++) Check(references.All(frame=>frame.Width==5 && frame.Height==5),"Capture evidence references should preserve capture dimensions.");
        for(var i=0;i<10;i++) Check(references.All(frame=>frame.Handles.Count==2),"Each frame should retain two opaque evidence handles.");
        for(var i=0;i<10;i++) Check(ProductionCaptureEvidenceProjectionRuntime.IsValid(production,references),"Capture evidence projection should validate.");
        for(var i=0;i<10;i++) Check(!ProductionCaptureEvidenceProjectionRuntime.IsValid(production,tampered),"Capture metadata tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionCaptureEvidenceProjectionRuntime.IsValid(production,unsorted),"Non-canonical evidence handle ordering should be rejected.");

        assert(round==100,$"Production capture evidence smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
