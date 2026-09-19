using Asun.Device.Impl;
using Asun.Domain.Quality;
using Asun.Platform.Evidence;
using Asun.Platform.Pipeline;
using Asun.Platform.ReplayIntegration;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class ProductionQualityEvidenceReplayBundleHundredStageSmoke
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
            Guid.Parse("7C000000-0000-0000-0000-000000000001"),
            "ReplayBundleProgram",
            new Version(1,0,0),
            new[]
            {
                new ProgramStep(
                    Guid.Parse("7D000000-0000-0000-0000-000000000001"),
                    1,
                    ProgramStepKind.Acquire,
                    "Acquire",
                    Array.Empty<ProgramParameter>()),
                new ProgramStep(
                    Guid.Parse("7D000000-0000-0000-0000-000000000002"),
                    2,
                    ProgramStepKind.Measure,
                    "Measure",
                    Array.Empty<ProgramParameter>())
            });

        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(
            new[]
            {
                new PipelineStage<Asun.Device.Contracts.CapturedFrame>(
                    1,
                    "Acquire",
                    frame=>frame),
                new PipelineStage<Asun.Device.Contracts.CapturedFrame>(
                    2,
                    "Measure",
                    frame=>frame)
            });

        var definition=new ProductionSessionDefinition(
            Guid.Parse("7E000000-0000-0000-0000-000000000001"),
            plan,
            pipeline,
            2);

        var production=await ProductionSessionRuntime.RunAsync(
            definition,
            new SimulatedFrameSource(4,4));

        var qualityRun=QualityInspectionRunRuntime.Create(
            Guid.Parse("7F000000-0000-0000-0000-000000000001"),
            new[]
            {
                new QualityInspectionResult(
                    Guid.Parse("80000000-0000-0000-0000-000000000001"),
                    new QualityInspectionSnapshot(
                        Guid.Parse("81000000-0000-0000-0000-000000000001"),
                        1,
                        new QualityFindingSet(
                            new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("REPLAY.INFO"),
                                    QualityOutcome.Informational,
                                    QualitySeverity.Information,
                                    "Replay frame 1 observed")
                            }),
                        new QualityFindingEvidenceSet(
                            Array.Empty<QualityFindingEvidenceLink>()))),
                new QualityInspectionResult(
                    Guid.Parse("80000000-0000-0000-0000-000000000002"),
                    new QualityInspectionSnapshot(
                        Guid.Parse("81000000-0000-0000-0000-000000000002"),
                        2,
                        new QualityFindingSet(
                            new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("REPLAY.INFO"),
                                    QualityOutcome.Informational,
                                    QualitySeverity.Information,
                                    "Replay frame 2 observed")
                            }),
                        new QualityFindingEvidenceSet(
                            Array.Empty<QualityFindingEvidenceLink>())))
            });

        var evidence=ProductionEvidenceReferenceProjectionRuntime.Create(
            production,
            new[]
            {
                new ProductionEvidenceFrameReference(
                    1,
                    new[]
                    {
                        EvidenceHandle.Create("replay/frame/1/input")
                    }),
                new ProductionEvidenceFrameReference(
                    2,
                    new[]
                    {
                        EvidenceHandle.Create("replay/frame/2/input")
                    })
            });

        var bundle=ProductionQualityEvidenceReplayBundleRuntime.Create(
            definition,
            production,
            qualityRun,
            evidence);

        var tampered=bundle with
        {
            EvidenceProjectionFingerprint=new string('a',64)
        };

        var shiftedQuality=QualityInspectionRunRuntime.Create(
            Guid.Parse("7F000000-0000-0000-0000-000000000002"),
            new[]
            {
                new QualityInspectionResult(
                    Guid.Parse("82000000-0000-0000-0000-000000000001"),
                    new QualityInspectionSnapshot(
                        Guid.Parse("83000000-0000-0000-0000-000000000001"),
                        2,
                        new QualityFindingSet(
                            new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("REPLAY.INFO"),
                                    QualityOutcome.Informational,
                                    QualitySeverity.Information,
                                    "Shifted")
                            }),
                        new QualityFindingEvidenceSet(
                            Array.Empty<QualityFindingEvidenceLink>()))),
                new QualityInspectionResult(
                    Guid.Parse("82000000-0000-0000-0000-000000000002"),
                    new QualityInspectionSnapshot(
                        Guid.Parse("83000000-0000-0000-0000-000000000002"),
                        3,
                        new QualityFindingSet(
                            new[]
                            {
                                new QualityFinding(
                                    QualityFindingId.Create("REPLAY.INFO"),
                                    QualityOutcome.Informational,
                                    QualitySeverity.Information,
                                    "Shifted")
                            }),
                        new QualityFindingEvidenceSet(
                            Array.Empty<QualityFindingEvidenceLink>())))
            });

        for(var i=0;i<10;i++) Check(production.FrameCount==2,"Replay bundle source production should contain two frames.");
        for(var i=0;i<10;i++) Check(qualityRun.ResultCount==2,"Replay bundle source quality run should contain two results.");
        for(var i=0;i<10;i++) Check(evidence.Frames.Count==2,"Replay bundle source evidence projection should contain two frames.");
        for(var i=0;i<10;i++) Check(bundle.ProductionSessionId==production.SessionId,"Replay bundle should retain production identity.");
        for(var i=0;i<10;i++) Check(bundle.QualityRunId==qualityRun.RunId,"Replay bundle should retain Quality run identity.");
        for(var i=0;i<10;i++) Check(bundle.QualityResultCount==2,"Replay bundle should retain Quality result count.");
        for(var i=0;i<10;i++) Check(bundle.EvidenceFrameCount==2,"Replay bundle should retain evidence frame count.");
        for(var i=0;i<10;i++) Check(ProductionQualityEvidenceReplayBundleValidationRuntime.IsValid(definition,production,qualityRun,evidence,bundle),"Replay bundle should validate end-to-end.");
        for(var i=0;i<10;i++) Check(!ProductionQualityEvidenceReplayBundleValidationRuntime.IsValid(definition,production,qualityRun,evidence,tampered),"Evidence fingerprint tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionQualityEvidenceReplayBundleValidationRuntime.IsValid(definition,production,shiftedQuality,evidence,bundle),"Quality sequence drift should be rejected.");

        assert(round==100,$"Production quality evidence replay smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
