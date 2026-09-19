using Asun.Device.Impl;
using Asun.Platform.Pipeline;
using Asun.Platform.RenderIntegration;
using Asun.Program.Core;
using Asun.Production.Runtime;
using Asun.UI.Viewports;

public static class ProductionRenderFrameProjectionHundredStageSmoke
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
            Guid.Parse("6F000000-0000-0000-0000-000000000001"),
            "RenderProjectionProgram",
            new Version(1,0,0),
            new[]{
                new ProgramStep(Guid.Parse("70000000-0000-0000-0000-000000000001"),1,ProgramStepKind.Acquire,"Acquire",Array.Empty<ProgramParameter>()),
                new ProgramStep(Guid.Parse("70000000-0000-0000-0000-000000000002"),2,ProgramStepKind.Measure,"Measure",Array.Empty<ProgramParameter>())
            });
        var plan=ProgramExecutionPlanRuntime.Create(program);
        var pipeline=PipelineDefinitionRuntime.Create(new[]{
            new PipelineStage<Asun.Device.Contracts.CapturedFrame>(1,"Acquire",frame=>frame),
            new PipelineStage<Asun.Device.Contracts.CapturedFrame>(2,"Measure",frame=>frame)
        });
        var definition=new ProductionSessionDefinition(
            Guid.Parse("71000000-0000-0000-0000-000000000001"),
            plan,
            pipeline,
            2);
        var production=await ProductionSessionRuntime.RunAsync(
            definition,
            new SimulatedFrameSource(4,4));

        var summaries=new List<ViewportRenderFrameSummary>();
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
            assert(false,"Real render pipelines should each produce a frame.");
            return;
        }

        summaries.Add(ViewportRenderFrameSummaryRuntime.Create(frameOne.CommandStream));
        summaries.Add(ViewportRenderFrameSummaryRuntime.Create(frameTwo.CommandStream));
        var projection=ProductionRenderFrameProjectionRuntime.Create(production,summaries);
        var tampered=projection with {ProductionFingerprint=new string('e',64)};
        var invalidSummary=summaries[0] with {CommandCount=summaries[0].CommandCount+1};
        var invalidProjection=projection with {Frames=new[]{projection.Frames[0] with {RenderSummary=invalidSummary},projection.Frames[1]}};

        for(var i=0;i<10;i++) Check(production.FrameCount==2,"Production report should contain two frames for render projection.");
        for(var i=0;i<10;i++) Check(summaries.Count==2,"Real render pipeline execution should yield two summaries.");
        for(var i=0;i<10;i++) Check(summaries.All(summary=>summary.CommandCount>=0),"Render summaries should contain non-negative command counts.");
        for(var i=0;i<10;i++) Check(projection.ProductionSessionId==production.SessionId,"Render projection should bind production session identity.");
        for(var i=0;i<10;i++) Check(projection.Frames.Count==2,"Render projection should contain one entry per production frame.");
        for(var i=0;i<10;i++) Check(projection.Frames[0].Sequence==1 && projection.Frames[1].Sequence==2,"Render projection should preserve production sequence.");
        for(var i=0;i<10;i++) Check(projection.Frames.All(frame=>frame.RenderSummary.CommandCount>=0),"Projected render summaries should retain factual counts.");
        for(var i=0;i<10;i++) Check(ProductionRenderFrameProjectionValidationRuntime.IsValid(production,projection),"Render projection should validate.");
        for(var i=0;i<10;i++) Check(!ProductionRenderFrameProjectionValidationRuntime.IsValid(production,tampered),"Tampered production fingerprint should be rejected.");
        for(var i=0;i<10;i++) Check(!ProductionRenderFrameProjectionValidationRuntime.IsValid(production,invalidProjection),"Invalid render summary facts should be rejected.");

        assert(round==100,$"Production render projection smoke should execute exactly 100 numbered rounds; actual {round}.");
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
