using Asun.Device.Impl;
using Asun.Domain.Pcb;
using Asun.Domain.Quality;
using Asun.Platform.PcbProductionIntegration;
using Asun.Platform.Pipeline;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class PcbProductionQualityProvenanceBundleHundredStageSmoke
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
            Guid.Parse("8A000000-0000-0000-0000-000000000001"),
            "IntegratedBoard",
            120,
            80,
            4);
        var component=new PcbComponentReference(
            PcbFeatureId.Create("INTEGRATION-R1"),
            "R1",
            "10k",
            "0402",
            0,
            PcbLayerSide.Top,
            new PcbCoordinate(20,30),
            0);
        var assembly=PcbAssemblySnapshotRuntime.Create(board,new[]{component});

        var program=new InspectionProgram(
            Guid.Parse("8B000000-0000-0000-0000-000000000001"),
            "IntegratedProgram",
            new Version(1,0,0),
            new[]
            {
                new ProgramStep(
                    Guid.Parse("8C000000-0000-0000-0000-000000000001"),
                    1,
                    ProgramStepKind.Acquire,
                    "Acquire",
                    Array.Empty<ProgramParameter>())
            });
        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(
            new[]
            {
                new PipelineStage<Asun.Device.Contracts.CapturedFrame>(
                    1,
                    "Acquire",
                    frame=>frame)
            });
        var definition=new ProductionSessionDefinition(
            Guid.Parse("8D000000-0000-0000-0000-000000000001"),
            plan,
            pipeline,
            2);
        var recorder=new RecordingFrameSource(new SimulatedFrameSource(6,4,"Gray8"));
        var production=await ProductionSessionRuntime.RunAsync(definition,recorder);
        var provenance=ProductionFrameProvenanceRuntime.Create(production,recorder.Frames);

        var qualityRun=QualityInspectionRunRuntime.Create(
            Guid.Parse("8E000000-0000-0000-0000-000000000001"),
            new[]
            {
                new QualityInspectionResult(
                    Guid.Parse("8F000000-0000-0000-0000-000000000001"),
                    new QualityInspectionSnapshot(
                        Guid.Parse("90000000-0000-0000-0000-000000000001"),
                        1,
                        new QualityFindingSet(Array.Empty<QualityFinding>()),
                        new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))),
                new QualityInspectionResult(
                    Guid.Parse("8F000000-0000-0000-0000-000000000002"),
                    new QualityInspectionSnapshot(
                        Guid.Parse("90000000-0000-0000-0000-000000000002"),
                        2,
                        new QualityFindingSet(Array.Empty<QualityFinding>()),
                        new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())))
            });
        var bundle=PcbProductionQualityProvenanceBundleRuntime.Create(
            assembly,
            definition,
            production,
            provenance,
            qualityRun);
        var tampered=bundle with {AssemblyFingerprint=new string('a',64)};
        var shifted=provenance.ToArray();
        shifted[1]=shifted[1] with {Width=999};

        for(var i=0;i<10;i++) Check(assembly.Components.Count==1,"Assembly provenance should contain one component.");
        for(var i=0;i<10;i++) Check(production.FrameCount==2,"Integrated production should contain two frames.");
        for(var i=0;i<10;i++) Check(provenance.Count==2,"Integrated provenance should contain two frame facts.");
        for(var i=0;i<10;i++) Check(qualityRun.ResultCount==2,"Integrated Quality run should contain two results.");
        for(var i=0;i<10;i++) Check(bundle.AssemblyFingerprint==assembly.Fingerprint,"Bundle should retain assembly identity.");
        for(var i=0;i<10;i++) Check(bundle.ProductionSessionId==production.SessionId,"Bundle should retain production identity.");
        for(var i=0;i<10;i++) Check(bundle.QualityRunId==qualityRun.RunId,"Bundle should retain Quality run identity.");
        for(var i=0;i<10;i++) Check(PcbProductionQualityProvenanceBundleValidationRuntime.IsValid(assembly,definition,production,provenance,qualityRun,bundle),"Combined PCB provenance should validate.");
        for(var i=0;i<10;i++) Check(!PcbProductionQualityProvenanceBundleValidationRuntime.IsValid(assembly,definition,production,provenance,qualityRun,tampered),"Assembly fingerprint tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!PcbProductionQualityProvenanceBundleValidationRuntime.IsValid(assembly,definition,production,shifted,qualityRun,bundle),"Provenance metadata drift should be rejected.");

        assert(round==100,$"PCB production quality provenance smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
