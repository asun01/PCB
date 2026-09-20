using Asun.Device.Contracts;
using Asun.Device.Impl;
using Asun.Platform.Pipeline;
using Asun.Platform.RenderIntegration;
using Asun.Program.Core;
using Asun.Production.Runtime;
using Asun.UI.Viewports;

public static class ProductionCaptureRenderProvenanceHundredStageSmoke
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
            Guid.Parse("94000000-0000-0000-0000-000000000001"),
            "CaptureRenderProgram",
            new Version(1,0,0),
            new[]
            {
                new ProgramStep(
                    Guid.Parse("95000000-0000-0000-0000-000000000001"),
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
            Guid.Parse("96000000-0000-0000-0000-000000000001"),
            plan,
            pipeline,
            2);
        var recorder=new RecordingFrameSource(
            new SimulatedFrameSource(10,5,"Gray8"));
        var production=await ProductionSessionRuntime.RunAsync(
            definition,
            recorder);
        var captureProvenance=ProductionFrameProvenanceRuntime.Create(
            production,
            recorder.Frames);

        using var renderOne=new ViewportRenderPipelineRuntime<string>(
            new System.Numerics.Vector2(800,600),
            new System.Numerics.Vector2(400,300),
            new System.Numerics.Vector2(100,100),
            1,
            16,
            2,
            new LocalTileSource());
        renderOne.Invalidate(ViewportDirtyFlags.All,renderOne.Composite.Generation);
        var frameOne=await renderOne.RefreshAsync(DateTimeOffset.UtcNow.AddSeconds(1));

        using var renderTwo=new ViewportRenderPipelineRuntime<string>(
            new System.Numerics.Vector2(800,600),
            new System.Numerics.Vector2(400,300),
            new System.Numerics.Vector2(100,100),
            1,
            16,
            2,
            new LocalTileSource());
        renderTwo.Invalidate(ViewportDirtyFlags.All,renderTwo.Composite.Generation);
        var frameTwo=await renderTwo.RefreshAsync(DateTimeOffset.UtcNow.AddSeconds(1));

        if(frameOne is null || frameTwo is null)
        {
            assert(false,"Render pipeline should produce both frames.");
            return;
        }

        var summaries=new[]
        {
            ViewportRenderFrameSummaryRuntime.Create(frameOne.CommandStream),
            ViewportRenderFrameSummaryRuntime.Create(frameTwo.CommandStream)
        };
        var integrated=ProductionCaptureRenderProvenanceRuntime.Create(
            production,
            captureProvenance,
            summaries);
        var tampered=integrated.ToArray();
        tampered[0]=tampered[0] with
        {
            Width=999
        };
        var changedRender=integrated.ToArray();
        changedRender[1]=changedRender[1] with
        {
            RenderSummary=changedRender[1].RenderSummary with
            {
                RoiCount=changedRender[1].RenderSummary.RoiCount+1
            }
        };

        for(var i=0;i<10;i++) Check(production.FrameCount==2,"Production should retain two captured frames.");
        for(var i=0;i<10;i++) Check(captureProvenance.Count==2,"Capture provenance should retain two frames.");
        for(var i=0;i<10;i++) Check(integrated.Count==2,"Capture-to-render provenance should retain two links.");
        for(var i=0;i<10;i++) Check(integrated[0].Sequence==1 && integrated[1].Sequence==2,"Capture-to-render provenance should preserve frame sequence.");
        for(var i=0;i<10;i++) Check(integrated.All(frame=>frame.PayloadFingerprint.Length==64),"Capture-to-render provenance should preserve source payload SHA-256.");
        for(var i=0;i<10;i++) Check(integrated.All(frame=>frame.Width==10 && frame.Height==5),"Capture-to-render provenance should preserve capture dimensions.");
        for(var i=0;i<10;i++) Check(integrated.All(frame=>frame.RenderFingerprint.Length==64),"Capture-to-render provenance should preserve render fingerprints.");
        for(var i=0;i<10;i++) Check(ProductionCaptureRenderProvenanceRuntime.IsValid(production,integrated),"Capture-to-render provenance should validate.");
        for(var i=0;i<10;i++) Check(!ProductionCaptureRenderProvenanceRuntime.IsValid(production,tampered),"Capture metadata tampering should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionCaptureRenderProvenanceRuntime.IsValid(production,changedRender),"Changed render summary should be rejected.");

        assert(round==100,$"Production capture/render provenance smoke should execute exactly 100 numbered rounds; actual {round}.");
    }

    private sealed class LocalTileSource : ITileSource<string>
    {
        public ValueTask<string> LoadAsync(
            TileRequest request,
            System.Drawing.RectangleF imageRectangle,
            CancellationToken cancellationToken=default)=>
            ValueTask.FromResult($"tile:{request.Index.X},{request.Index.Y}");
    }
}
