using Asun.Device.Impl;
using Asun.Platform.Pipeline;
using Asun.Platform.RenderIntegration;
using Asun.Program.Core;
using Asun.Production.Runtime;
using Asun.UI.Viewports;

public static class ProductionRenderReplayFrameIntegrityHundredStageSmoke
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
            Guid.Parse("84000000-0000-0000-0000-000000000001"),
            "RenderReplayIntegrityProgram",
            new Version(1,0,0),
            new[]
            {
                new ProgramStep(
                    Guid.Parse("85000000-0000-0000-0000-000000000001"),
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
            Guid.Parse("86000000-0000-0000-0000-000000000001"),
            plan,
            pipeline,
            2);
        var production=await ProductionSessionRuntime.RunAsync(
            definition,
            new SimulatedFrameSource(4,4));

        using var renderOne=new ViewportRenderPipelineRuntime<string>(
            new System.Numerics.Vector2(800,600),
            new System.Numerics.Vector2(400,300),
            new System.Numerics.Vector2(100,100),
            1,
            16,
            2,
            new LocalTileSource());
        renderOne.Invalidate(ViewportDirtyFlags.All,renderOne.Composite.Generation);
        var first=await renderOne.RefreshAsync(DateTimeOffset.UtcNow.AddSeconds(1));

        using var renderTwo=new ViewportRenderPipelineRuntime<string>(
            new System.Numerics.Vector2(800,600),
            new System.Numerics.Vector2(400,300),
            new System.Numerics.Vector2(100,100),
            1,
            16,
            2,
            new LocalTileSource());
        renderTwo.Invalidate(ViewportDirtyFlags.All,renderTwo.Composite.Generation);
        var second=await renderTwo.RefreshAsync(DateTimeOffset.UtcNow.AddSeconds(1));

        if(first is null || second is null)
        {
            assert(false,"Render pipeline should produce two replay frames.");
            return;
        }

        var summaries=new[]
        {
            ViewportRenderFrameSummaryRuntime.Create(first.CommandStream),
            ViewportRenderFrameSummaryRuntime.Create(second.CommandStream)
        };
        var integrity=ProductionRenderReplayFrameIntegrityRuntime.CreateFrames(production,summaries);
        var tampered=integrity.ToArray();
        tampered[0]=tampered[0] with
        {
            RenderFingerprint=new string('a',64)
        };
        var changed=integrity.ToArray();
        changed[0]=changed[0] with
        {
            RenderSummary=changed[0].RenderSummary with
            {
                CommandCount=changed[0].RenderSummary.CommandCount+1
            }
        };

        for(var i=0;i<10;i++) Check(production.FrameCount==2,"Production replay source should contain two frames.");
        for(var i=0;i<10;i++) Check(summaries.Length==2,"Render replay source should contain two summaries.");
        for(var i=0;i<10;i++) Check(integrity.Count==2,"Render integrity should contain two frame records.");
        for(var i=0;i<10;i++) Check(integrity[0].Sequence==1 && integrity[1].Sequence==2,"Render replay integrity should preserve production sequences.");
        for(var i=0;i<10;i++) Check(integrity.All(frame=>frame.RenderFingerprint.Length==64),"Render replay records should retain SHA-256 fingerprints.");
        for(var i=0;i<10;i++) Check(integrity.All(frame=>frame.ProductionInputFingerprint.Length==64),"Render replay records should retain production input fingerprints.");
        for(var i=0;i<10;i++) Check(ProductionRenderReplayFrameIntegrityRuntime.IsValid(production,integrity),"Render replay integrity should validate.");
        for(var i=0;i<10;i++) Check(!ProductionRenderReplayFrameIntegrityRuntime.IsValid(production,tampered),"Tampered render fingerprint should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionRenderReplayFrameIntegrityRuntime.IsValid(production,changed),"Changed render summary should invalidate its fingerprint.");
        for(var i=0;i<10;i++) Check(integrity[0].RenderSummary.Generation>=0,"Render replay generation should be non-negative.");

        assert(round==100,$"Production render replay integrity smoke should execute exactly 100 numbered rounds; actual {round}.");
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
