using Asun.Device.Impl;
using Asun.Domain.Quality;
using Asun.Platform.Pipeline;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class ProductionQualityInspectionProjectionHundredStageSmoke
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
            Guid.Parse("66000000-0000-0000-0000-000000000001"),
            "QualityProjectionProgram",
            new Version(1,0,0),
            new[]{
                new ProgramStep(
                    Guid.Parse("67000000-0000-0000-0000-000000000001"),
                    1,
                    ProgramStepKind.Acquire,
                    "Acquire",
                    Array.Empty<ProgramParameter>()),
                new ProgramStep(
                    Guid.Parse("67000000-0000-0000-0000-000000000002"),
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
        var definition=new ProductionSessionDefinition(
            Guid.Parse("68000000-0000-0000-0000-000000000001"),
            plan,
            pipeline,
            2);
        var production=await ProductionSessionRuntime.RunAsync(
            definition,
            new SimulatedFrameSource(4,4));

        var qualityRun=QualityInspectionRunRuntime.Create(
            Guid.Parse("69000000-0000-0000-0000-000000000001"),
            new[]{
                new QualityInspectionResult(
                    Guid.Parse("6A000000-0000-0000-0000-000000000001"),
                    new QualityInspectionSnapshot(
                        Guid.Parse("6B000000-0000-0000-0000-000000000001"),
                        1,
                        new QualityFindingSet(Array.Empty<QualityFinding>()),
                        new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))),
                new QualityInspectionResult(
                    Guid.Parse("6A000000-0000-0000-0000-000000000002"),
                    new QualityInspectionSnapshot(
                        Guid.Parse("6B000000-0000-0000-0000-000000000002"),
                        2,
                        new QualityFindingSet(Array.Empty<QualityFinding>()),
                        new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))),
            });

        var projection=ProductionQualityInspectionProjectionRuntime.Create(
            production,
            qualityRun);
        var tampered=projection with {
            ProductionFingerprint=new string('c',64)
        };
        var shiftedQuality=QualityInspectionRunRuntime.Create(
            Guid.Parse("69000000-0000-0000-0000-000000000002"),
            new[]{
                new QualityInspectionResult(
                    Guid.Parse("6A000000-0000-0000-0000-000000000011"),
                    new QualityInspectionSnapshot(
                        Guid.Parse("6B000000-0000-0000-0000-000000000011"),
                        2,
                        new QualityFindingSet(Array.Empty<QualityFinding>()),
                        new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))),
                new QualityInspectionResult(
                    Guid.Parse("6A000000-0000-0000-0000-000000000012"),
                    new QualityInspectionSnapshot(
                        Guid.Parse("6B000000-0000-0000-0000-000000000012"),
                        3,
                        new QualityFindingSet(Array.Empty<QualityFinding>()),
                        new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))),
            });

        for(var i=0;i<10;i++) Check(production.FrameCount==2,"Production report should contain two frames for projection.");
        for(var i=0;i<10;i++) Check(qualityRun.ResultCount==2,"Quality run should contain two results.");
        for(var i=0;i<10;i++) Check(projection.ProductionSessionId==production.SessionId,"Projection should bind production session identity.");
        for(var i=0;i<10;i++) Check(projection.QualityRunId==qualityRun.RunId,"Projection should bind quality run identity.");
        for(var i=0;i<10;i++) Check(projection.Links.Count==2,"Projection should create one link per frame/result.");
        for(var i=0;i<10;i++) Check(projection.Links[0].Sequence==1 && projection.Links[1].Sequence==2,"Projection should preserve aligned sequence numbers.");
        for(var i=0;i<10;i++) Check(ProductionQualityInspectionProjectionValidationRuntime.IsValid(production,qualityRun,projection),"Projection should validate.");
        for(var i=0;i<10;i++) Check(!ProductionQualityInspectionProjectionValidationRuntime.IsValid(production,qualityRun,tampered),"Tampered production fingerprint should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionQualityInspectionProjectionValidationRuntime.IsValid(production,shiftedQuality,projection),"Quality sequence drift should be rejected.");
        for(var i=0;i<10;i++) Check(projection.Fingerprint.Length==64,"Projection fingerprint should be fixed width.");

        assert(round==100,$"Production-quality projection smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
