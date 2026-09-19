using Asun.Device.Impl;
using Asun.Device.Contracts;
using Asun.Platform.Pipeline;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class ProductionFrameProvenanceHundredStageSmoke
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
            Guid.Parse("87000000-0000-0000-0000-000000000001"),
            "ProvenanceProgram",
            new Version(1,0,0),
            new[]
            {
                new ProgramStep(
                    Guid.Parse("88000000-0000-0000-0000-000000000001"),
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
            Guid.Parse("89000000-0000-0000-0000-000000000001"),
            plan,
            pipeline,
            2);
        var recorder=new RecordingFrameSource(new SimulatedFrameSource(8,6,"Gray8"));
        var production=await ProductionSessionRuntime.RunAsync(definition,recorder);
        var provenance=ProductionFrameProvenanceRuntime.Create(production,recorder.Frames);
        var tampered=provenance.ToArray();
        tampered[0]=tampered[0] with
        {
            Width=999
        };
        var shifted=recorder.Frames.ToArray();
        shifted[1]=CapturedFrame.Create(
            shifted[1].Metadata with {Sequence=FrameSequence.Create(3)},
            shifted[1].Payload);

        for(var i=0;i<10;i++) Check(recorder.Frames.Count==2,"Recording source should retain both captured frames.");
        for(var i=0;i<10;i++) Check(production.FrameCount==2,"Production session should retain two frames.");
        for(var i=0;i<10;i++) Check(provenance.Count==2,"Production provenance should retain one entry per frame.");
        for(var i=0;i<10;i++) Check(provenance[0].Sequence.Value==1 && provenance[1].Sequence.Value==2,"Provenance should preserve capture sequences.");
        for(var i=0;i<10;i++) Check(provenance.All(frame=>frame.Width==8 && frame.Height==6),"Provenance should preserve captured dimensions.");
        for(var i=0;i<10;i++) Check(provenance.All(frame=>frame.PixelFormat=="Gray8"),"Provenance should preserve pixel format.");
        for(var i=0;i<10;i++) Check(provenance.All(frame=>frame.PayloadFingerprint.Length==64),"Provenance should preserve payload fingerprints.");
        for(var i=0;i<10;i++) Check(ProductionFrameProvenanceRuntime.IsValid(production,provenance),"Frame provenance should validate against production.");
        for(var i=0;i<10;i++) Check(!ProductionFrameProvenanceRuntime.IsValid(production,tampered),"Tampered frame dimensions should be rejected.");
        for(var i=0;i<10;i++) Check(shifted[1].Metadata.Sequence.Value==3,"Changed source frame sequence should be observable before provenance creation.");

        assert(round==100,$"Production frame provenance smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
