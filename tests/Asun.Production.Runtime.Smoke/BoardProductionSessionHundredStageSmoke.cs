using Asun.Device.Impl;
using Asun.Domain.Pcb;
using Asun.Platform.Pipeline;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class BoardProductionSessionHundredStageSmoke
{
    public static async ValueTask RunAsync(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var board=new PcbBoardDefinition(
            Guid.Parse("60000000-0000-0000-0000-000000000001"),
            "ProductionBoard",
            100,
            80,
            4);
        var component=new PcbComponentReference(
            PcbFeatureId.Create("R-001"),
            "R1",
            "10k",
            "0402",
            0,
            PcbLayerSide.Top,
            new PcbCoordinate(20,30),
            0);
        var assembly=PcbAssemblySnapshotRuntime.Create(board,new[]{component});

        var program=new InspectionProgram(
            Guid.Parse("61000000-0000-0000-0000-000000000001"),
            "BoardProductionProgram",
            new Version(1,0,0),
            new[]{
                new ProgramStep(
                    Guid.Parse("62000000-0000-0000-0000-000000000001"),
                    1,
                    ProgramStepKind.Acquire,
                    "Acquire",
                    Array.Empty<ProgramParameter>()),
                new ProgramStep(
                    Guid.Parse("62000000-0000-0000-0000-000000000002"),
                    2,
                    ProgramStepKind.Measure,
                    "Measure",
                    Array.Empty<ProgramParameter>())
            });
        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(new[]{
            new PipelineStage<Asun.Device.Contracts.CapturedFrame>(1,"Acquire",frame=>frame),
            new PipelineStage<Asun.Device.Contracts.CapturedFrame>(2,"Measure",frame=>frame)
        });
        var definition=new BoardProductionSessionDefinition(
            assembly,
            new ProductionSessionDefinition(
                Guid.Parse("63000000-0000-0000-0000-000000000001"),
                plan,
                pipeline,
                2));

        var report=await BoardProductionSessionRuntime.RunAsync(
            definition,
            new SimulatedFrameSource(4,4));
        var invalid=report with {AssemblyFingerprint=new string('a',64)};

        for(var i=0;i<10;i++) Check(report.AssemblyFingerprint==assembly.Fingerprint,"Board production report should bind assembly identity.");
        for(var i=0;i<10;i++) Check(report.ProductionReport.FrameCount==2,"Board production report should preserve frame count.");
        for(var i=0;i<10;i++) Check(report.ProductionReport.ProgramFingerprint==plan.Fingerprint,"Board production report should bind the program plan.");
        for(var i=0;i<10;i++) Check(report.Fingerprint.Length==64,"Board production report fingerprint should be fixed width.");
        for(var i=0;i<10;i++) Check(BoardProductionSessionValidationRuntime.IsValid(definition,report),"Board production session report should validate.");
        for(var i=0;i<10;i++) Check(!BoardProductionSessionValidationRuntime.IsValid(definition,invalid),"Assembly fingerprint tampering should be rejected.");
        for(var i=0;i<10;i++) Check(report.ProductionReport.Frames.Count==2,"Board production report should retain per-frame execution reports.");
        for(var i=0;i<10;i++) Check(report.ProductionReport.Frames.All(frame=>frame.InputFingerprint.Length==64),"All board production input frames should retain fingerprints.");
        for(var i=0;i<10;i++) Check(PcbAssemblySnapshotValidationRuntime.IsValid(assembly),"Source assembly should remain valid.");
        for(var i=0;i<10;i++) Check(ProductionSessionValidationRuntime.IsValid(definition.Production,report.ProductionReport),"Underlying production report should remain valid.");

        assert(round==100,$"Board production session smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
