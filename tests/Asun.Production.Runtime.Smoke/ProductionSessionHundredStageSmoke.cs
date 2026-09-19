using Asun.Device.Contracts;
using Asun.Device.Impl;
using Asun.Platform.Pipeline;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class ProductionSessionHundredStageSmoke
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
            Guid.Parse("40000000-0000-0000-0000-000000000001"),
            "ProductionProgram",
            new Version(1,0,0),
            new[]{
                new ProgramStep(
                    Guid.Parse("41000000-0000-0000-0000-000000000001"),
                    1,
                    ProgramStepKind.Acquire,
                    "Acquire",
                    new[]{new ProgramParameter("source","simulated")}),
                new ProgramStep(
                    Guid.Parse("41000000-0000-0000-0000-000000000002"),
                    2,
                    ProgramStepKind.Measure,
                    "Measure",
                    new[]{new ProgramParameter("unit","mm")}),
                new ProgramStep(
                    Guid.Parse("41000000-0000-0000-0000-000000000003"),
                    3,
                    ProgramStepKind.Inspect,
                    "Inspect",
                    new[]{new ProgramParameter("rule","default")})
            });
        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(new[]{
            new PipelineStage<CapturedFrame>(
                1,
                "Acquire",
                frame=>CapturedFrame.Create(frame.Metadata,frame.Payload.ToArray())),
            new PipelineStage<CapturedFrame>(
                2,
                "Measure",
                frame=>frame),
            new PipelineStage<CapturedFrame>(
                3,
                "Inspect",
                frame=>frame)
        });
        var definition=new ProductionSessionDefinition(
            Guid.Parse("42000000-0000-0000-0000-000000000001"),
            plan,
            pipeline,
            3);

        var report=await ProductionSessionRuntime.RunAsync(
            definition,
            new SimulatedFrameSource(8,8));

        var tamperedCount=report with {FrameCount=2};
        var tamperedFingerprint=report with {Fingerprint=new string('a',64)};

        for(var i=0;i<10;i++) Check(report.SessionId==definition.SessionId,"Production report should retain session identity.");
        for(var i=0;i<10;i++) Check(report.ProgramFingerprint==plan.Fingerprint,"Production report should bind the program plan.");
        for(var i=0;i<10;i++) Check(report.FrameCount==3,"Production report should preserve three captured frames.");
        for(var i=0;i<10;i++) Check(report.Frames.Count==3,"Production report should contain three frame executions.");
        for(var i=0;i<10;i++) Check(report.Frames.Select(frame=>frame.Sequence.Value).SequenceEqual(new long[]{1,2,3}),"Production frame sequences should remain contiguous.");
        for(var i=0;i<10;i++) Check(report.Frames.All(frame=>frame.InputFingerprint.Length==64),"Every input frame should retain a SHA-256 payload fingerprint.");
        for(var i=0;i<10;i++) Check(report.Frames.All(frame=>frame.PipelineReport.StageCount==3),"Every frame should execute all three pipeline stages.");
        for(var i=0;i<10;i++) Check(ProductionSessionValidationRuntime.IsValid(definition,report),"Production session report should validate.");
        for(var i=0;i<10;i++) Check(
            !ProductionSessionValidationRuntime.IsValid(definition,tamperedCount) &&
            !ProductionSessionValidationRuntime.IsValid(definition,tamperedFingerprint),
            "Tampered production report count/fingerprint should be rejected.");
        for(var i=0;i<10;i++) Check(report.Fingerprint.Length==64,"Production report fingerprint should be fixed width.");

        assert(round==100,$"Production session smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
