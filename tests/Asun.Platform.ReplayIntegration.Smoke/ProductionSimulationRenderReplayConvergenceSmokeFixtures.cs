using Asun.Device.Contracts;
using Asun.Device.Impl;
using Asun.Domain.Pcb;
using Asun.Platform.Evidence;
using Asun.Platform.Pipeline;
using Asun.Platform.RenderIntegration;
using Asun.Platform.SimulationIntegration;
using Asun.Production.Runtime;
using Asun.Program.Core;
using Asun.Simulation.Core;
using Asun.Metrology.Core;

namespace Asun.Platform.ReplayIntegration.Smoke;

internal static class ProductionSimulationRenderReplayConvergenceSmokeFixtures
{
    public static (
        ProductionSimulationReplayDescriptor SimulationDescriptor,
        ProductionSimulationReplayBinding SimulationBinding,
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> RenderFrames,
        IReadOnlyList<ProductionRenderEvidenceReplayDescriptor> RenderDescriptors)
        Create()
    {
        var board=new PcbBoardDefinition(
            Guid.Parse("60000000-0000-0000-0000-000000000001"),
            "SimulationRenderConvergenceBoard",
            100,
            80,
            4);

        var component=new PcbComponentReference(
            PcbFeatureId.Create("SIM-RENDER-R1"),
            "R1",
            "10k",
            "0402",
            0,
            PcbLayerSide.Top,
            new PcbCoordinate(20,30),
            0);

        var assembly=PcbAssemblySnapshotRuntime.Create(board,new[]{component});
        var scenario=SimulationScenarioRuntime.Create(assembly,42);
        var observations=SimulationSessionRuntime.Run(scenario,2);

        var program=new InspectionProgram(
            Guid.Parse("61000000-0000-0000-0000-000000000001"),
            "SimulationRenderConvergenceProgram",
            new Version(1,0,0),
            new[]
            {
                new ProgramStep(
                    Guid.Parse("62000000-0000-0000-0000-000000000001"),
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
            Guid.Parse("63000000-0000-0000-0000-000000000001"),
            plan,
            pipeline,
            2);

        var production=ProductionSessionRuntime.RunAsync(
            definition,
            new SimulatedFrameSource(4,4)).AsTask().GetAwaiter().GetResult();

        var binding=ProductionSimulationReplayBindingRuntime.Create(
            production,
            observations);

        var simulationDescriptor=ProductionSimulationReplayDescriptorRuntime.Create(
            production,
            observations,
            binding);

        var summaries=new[]
        {
            new Asun.UI.Viewports.ViewportRenderFrameSummary(1,2,1,1,0,0,1,0),
            new Asun.UI.Viewports.ViewportRenderFrameSummary(2,3,1,1,1,1,2,1)
        };

        var renderFrames=ProductionRenderReplayFrameIntegrityRuntime.CreateFrames(
            production,
            summaries);

        var handles=new[]
        {
            EvidenceHandle.Create("simulation-render/1"),
            EvidenceHandle.Create("simulation-render/2")
        };

        var descriptors=ProductionRenderEvidenceReplayDescriptorRuntime.Create(
            renderFrames,
            handles);

        return (simulationDescriptor,binding,renderFrames,descriptors);
    }
}
