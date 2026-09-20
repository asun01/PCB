using Asun.Device.Impl;
using Asun.Device.Contracts;
using Asun.Domain.Pcb;
using Asun.Domain.Quality;
using Asun.Platform.Evidence;
using Asun.Metrology.Core;
using Asun.Platform.PcbExecutionIntegration;
using Asun.Platform.Pipeline;
using Asun.Program.Core;
using Asun.Production.Runtime;

public static class PcbExecutionSnapshotHundredStageSmoke
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
            Guid.Parse("AF000000-0000-0000-0000-000000000001"),
            "FullExecutionBoard",
            100,
            80,
            4);
        var component=new PcbComponentReference(
            PcbFeatureId.Create("FULL-R1"),
            "R1",
            "10k",
            "0402",
            0,
            PcbLayerSide.Top,
            new PcbCoordinate(12,23),
            0);
        var assembly=PcbAssemblySnapshotRuntime.Create(board,new[]{component});

        var correspondences=new[]
        {
            new CalibrationCorrespondence2D(new MetrologyPoint2D(0,0),new MetrologyPoint2D(12,23)),
            new CalibrationCorrespondence2D(new MetrologyPoint2D(1,0),new MetrologyPoint2D(14,23)),
            new CalibrationCorrespondence2D(new MetrologyPoint2D(0,1),new MetrologyPoint2D(12,26)),
            new CalibrationCorrespondence2D(new MetrologyPoint2D(2,2),new MetrologyPoint2D(16,29))
        };
        var calibration=AffineCalibrationRuntime.Fit(correspondences);
        var observation1=CalibratedPcbPlacementObservationRuntime.Measure(component,new MetrologyPoint2D(0,0),correspondences,calibration);
        var observation2=CalibratedPcbPlacementObservationRuntime.Measure(component,new MetrologyPoint2D(0.1,0.05),correspondences,calibration);

        var program=new InspectionProgram(
            Guid.Parse("B0000000-0000-0000-0000-000000000001"),
            "FullExecutionProgram",
            new Version(1,0,0),
            new[]
            {
                new ProgramStep(Guid.Parse("B1000000-0000-0000-0000-000000000001"),1,ProgramStepKind.Acquire,"Acquire",Array.Empty<ProgramParameter>()),
                new ProgramStep(Guid.Parse("B1000000-0000-0000-0000-000000000002"),2,ProgramStepKind.Measure,"Measure",Array.Empty<ProgramParameter>())
            });
        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(
            new[]
            {
                new PipelineStage<CapturedFrame>(1,"Acquire",frame=>frame),
                new PipelineStage<CapturedFrame>(2,"Measure",frame=>frame)
            });
        var definition=new ProductionSessionDefinition(
            Guid.Parse("B2000000-0000-0000-0000-000000000001"),
            plan,
            pipeline,
            2);
        var recorder=new RecordingFrameSource(new SimulatedFrameSource(4,4));
        var production=await ProductionSessionRuntime.RunAsync(definition,recorder);
        var provenance=ProductionFrameProvenanceRuntime.Create(production,recorder.Frames);
        var measurements=ProductionMeasurementFactRuntime.Create(production,new long[]{1,2},new[]{observation1,observation2});
        var qualityRun=QualityInspectionRunRuntime.Create(
            Guid.Parse("B3000000-0000-0000-0000-000000000001"),
            new[]
            {
                new QualityInspectionResult(Guid.Parse("B4000000-0000-0000-0000-000000000001"),new QualityInspectionSnapshot(Guid.Parse("B5000000-0000-0000-0000-000000000001"),1,new QualityFindingSet(Array.Empty<QualityFinding>()),new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>()))),
                new QualityInspectionResult(Guid.Parse("B4000000-0000-0000-0000-000000000002"),new QualityInspectionSnapshot(Guid.Parse("B5000000-0000-0000-0000-000000000002"),2,new QualityFindingSet(Array.Empty<QualityFinding>()),new QualityFindingEvidenceSet(Array.Empty<QualityFindingEvidenceLink>())))
            });
        var evidence=ProductionEvidenceReferenceProjectionRuntime.Create(
            production,
            new[]
            {
                new ProductionEvidenceFrameReference(1,new[]{EvidenceHandle.Create("full/frame/1")}),
                new ProductionEvidenceFrameReference(2,new[]{EvidenceHandle.Create("full/frame/2")})
            });
        var pipelineAudit=ProductionPipelineReplayAuditRuntime.Create(definition,production);
        var snapshot=PcbExecutionSnapshotRuntime.Create(
            assembly,
            definition,
            production,
            provenance,
            measurements,
            qualityRun,
            evidence,
            pipelineAudit);
        var tampered=snapshot with {AssemblyFingerprint=new string('f',64)};
        var tamperedEvidence=snapshot with {EvidenceProjectionFingerprint=new string('e',64)};

        for(var i=0;i<10;i++) Check(assembly.Components.Count==1,"Execution snapshot should retain one PCB component.");
        for(var i=0;i<10;i++) Check(production.FrameCount==2,"Execution snapshot should retain two production frames.");
        for(var i=0;i<10;i++) Check(provenance.Count==2,"Execution snapshot should retain two provenance facts.");
        for(var i=0;i<10;i++) Check(measurements.Count==2,"Execution snapshot should retain two measurement facts.");
        for(var i=0;i<10;i++) Check(qualityRun.ResultCount==2,"Execution snapshot should retain two Quality results.");
        for(var i=0;i<10;i++) Check(evidence.Frames.Count==2,"Execution snapshot should retain two Evidence frame projections.");
        for(var i=0;i<10;i++) Check(pipelineAudit.Frames.Count==2,"Execution snapshot should retain two pipeline audit frames.");
        for(var i=0;i<10;i++) Check(snapshot.AssemblyFingerprint==assembly.Fingerprint,"Execution snapshot should bind PCB assembly identity.");
        for(var i=0;i<10;i++) Check(PcbExecutionSnapshotValidationRuntime.IsValid(assembly,definition,production,provenance,measurements,qualityRun,evidence,pipelineAudit,snapshot),"Unified PCB execution snapshot should validate.");
        for(var i=0;i<10;i++) Check(!PcbExecutionSnapshotValidationRuntime.IsValid(assembly,definition,production,provenance,measurements,qualityRun,evidence,pipelineAudit,tampered),"Assembly identity tampering should be rejected.");

        assert(round==100,$"Unified PCB execution snapshot smoke should execute exactly 100 numbered rounds; actual {round}.");
    }
}
