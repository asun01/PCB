using System.Numerics;
using Asun.Device.Impl;
using Asun.Platform.RoiProductionIntegration;
using Asun.Production.Runtime;
using Asun.Program.Core;
using Asun.Platform.Pipeline;
using Asun.UI.Viewports;
using Asun.Device.Contracts;

namespace Asun.Platform.RoiProductionIntegration.Smoke;

internal static class ProductionRoiInteractionContextSmokeFixtures
{
    public static (
        ProductionSessionReport Production,
        ViewportRoiInputRecoverySnapshot RoiSnapshot)
        Create()
    {
        var program=new InspectionProgram(
            Guid.Parse("70000000-0000-0000-0000-000000000001"),
            "RoiProductionIntegrationProgram",
            new Version(1,0,0),
            new[]
            {
                new ProgramStep(
                    Guid.Parse("71000000-0000-0000-0000-000000000001"),
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
            Guid.Parse("72000000-0000-0000-0000-000000000001"),
            plan,
            pipeline,
            2);

        var production=ProductionSessionRuntime
            .RunAsync(definition,new SimulatedFrameSource(4,4))
            .AsTask()
            .GetAwaiter()
            .GetResult();

        using var roiRuntime=new ViewportRoiInputRecoveryRuntime(
            new Vector2(200,200),
            new Vector2(100,100),
            RoiEditorMode.Select);

        roiRuntime.RoiViewport.Document.Add(
            RoiGeometry.CreateRectangle(
                new Vector2(100,100),
                new Vector2(40,20)),
            Guid.Parse("73000000-0000-0000-0000-000000000001"));

        roiRuntime.TryStart(out _);
        roiRuntime.TrySubmitAndProcess(
            ViewportInputEventKind.PointerMove,
            new Vector2(50,50));
        var snapshot=roiRuntime.Capture();

        return (production,snapshot);
    }
}
