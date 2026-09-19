using Asun.UI.Viewports;

public static class ViewportRenderFrameSummaryHundredStageSmoke
{
    public static void Run(Action<bool,string> assert)
    {
        var round=0;

        void Check(bool condition,string message)
        {
            round++;
            assert(condition,$"Round {round}: {message}");
        }

        var stream=CreateStream();
        var summary=ViewportRenderFrameSummaryRuntime.Create(stream);
        var fingerprint=ViewportRenderFrameFingerprintRuntime.CreateFingerprint(summary);
        var invalid=summary with {TileCount=summary.TileCount+1};

        for(var i=0;i<10;i++) Check(summary.Generation==stream.Generation,"Frame summary generation should match.");
        for(var i=0;i<10;i++) Check(summary.CommandCount==stream.CommandCount,"Frame summary command count should match.");
        for(var i=0;i<10;i++) Check(summary.RegionCount==stream.RegionCount,"Frame summary region count should match.");
        for(var i=0;i<10;i++) Check(summary.TileCount==stream.TileCount,"Frame summary tile count should match.");
        for(var i=0;i<10;i++) Check(summary.RoiCount==stream.RoiCount,"Frame summary ROI count should match.");
        for(var i=0;i<10;i++) Check(summary.OverlayCount==stream.OverlayCount,"Frame summary overlay count should match.");
        for(var i=0;i<10;i++) Check(summary.InvalidationCount==stream.InvalidationCount,"Frame summary invalidation count should match.");
        for(var i=0;i<10;i++) Check(summary.FullSurfaceCount==stream.FullSurfaceCount,"Frame summary full-surface count should match.");
        for(var i=0;i<10;i++) Check(ViewportRenderFrameIntegrityRuntime.IsValid(stream,summary,fingerprint),"Frame summary integrity should validate.");
        for(var i=0;i<10;i++) Check(!ViewportRenderFrameIntegrityRuntime.IsValid(stream,invalid,fingerprint),"Frame summary mutation should invalidate the fingerprint.");

        assert(round==100,$"Viewport render frame summary smoke should execute exactly 100 numbered rounds; actual {round}.");
    }

    private static ViewportRenderCommandStream CreateStream()
    {
        var workItems=new[]
        {
            ViewportRenderWorkItem.CreateTile(1,1,1),
            ViewportRenderWorkItem.CreateRoi(2,false),
            ViewportRenderWorkItem.CreateOverlay(3),
            ViewportRenderWorkItem.CreateRoi(4,true)
        };

        var batch=ViewportRenderBatch.Create(42,workItems,new[]{
            new System.Drawing.RectangleF(0,0,100,100)
        });

        return ViewportRenderCommandStreamRuntime.Build(batch);
    }
}
