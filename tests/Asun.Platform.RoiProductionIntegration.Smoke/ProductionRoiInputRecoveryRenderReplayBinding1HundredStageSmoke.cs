namespace Asun.Platform.RoiProductionIntegration.Smoke;

public static class ProductionRoiInputRecoveryRenderReplayBinding1HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var group0=0;group0<10;group0++)
        {
            for(var i0=0;i0<10;i0++)
            {
                round++;
                var fixture=RoiInputRenderReplayFixtureRuntime.Create();
                var binding=ProductionRoiInputRecoveryRenderReplayBindingRuntime.Create(fixture.RoiContext,fixture.RenderContext);
                Check(
                    ProductionRoiInputRecoveryRenderReplayBindingRuntime.IsValid(fixture.RoiContext,fixture.RenderContext) &&
                    binding.ProductionSessionId==fixture.RoiContext.ProductionSessionId &&
                    binding.RenderFrameCount==2 &&
                    binding.FirstSequence==1 &&
                    binding.LastSequence==2 &&
                    binding.BindingFingerprint.Length==64 &&
                    (group0<9 || round==100),
                    "valid ROI input/render replay binding must be accepted");
            }
        }
        for(var group1=0;group1<10;group1++)
        {
            for(var i1=0;i1<10;i1++)
            {
                round++;
                var fixture=RoiInputRenderReplayFixtureRuntime.Create();
                var binding=ProductionRoiInputRecoveryRenderReplayBindingRuntime.Create(fixture.RoiContext,fixture.RenderContext);
                Check(
                    ProductionRoiInputRecoveryRenderReplayBindingRuntime.IsValid(fixture.RoiContext,fixture.RenderContext) &&
                    binding.ProductionSessionId==fixture.RoiContext.ProductionSessionId &&
                    binding.RenderFrameCount==2 &&
                    binding.FirstSequence==1 &&
                    binding.LastSequence==2 &&
                    binding.BindingFingerprint.Length==64 &&
                    (group1<9 || round==100),
                    "valid ROI input/render replay binding must be accepted");
            }
        }
        for(var group2=0;group2<10;group2++)
        {
            for(var i2=0;i2<10;i2++)
            {
                round++;
                var fixture=RoiInputRenderReplayFixtureRuntime.Create();
                var binding=ProductionRoiInputRecoveryRenderReplayBindingRuntime.Create(fixture.RoiContext,fixture.RenderContext);
                Check(
                    ProductionRoiInputRecoveryRenderReplayBindingRuntime.IsValid(fixture.RoiContext,fixture.RenderContext) &&
                    binding.ProductionSessionId==fixture.RoiContext.ProductionSessionId &&
                    binding.RenderFrameCount==2 &&
                    binding.FirstSequence==1 &&
                    binding.LastSequence==2 &&
                    binding.BindingFingerprint.Length==64 &&
                    (group2<9 || round==100),
                    "valid ROI input/render replay binding must be accepted");
            }
        }
        for(var group3=0;group3<10;group3++)
        {
            for(var i3=0;i3<10;i3++)
            {
                round++;
                var fixture=RoiInputRenderReplayFixtureRuntime.Create();
                var binding=ProductionRoiInputRecoveryRenderReplayBindingRuntime.Create(fixture.RoiContext,fixture.RenderContext);
                Check(
                    ProductionRoiInputRecoveryRenderReplayBindingRuntime.IsValid(fixture.RoiContext,fixture.RenderContext) &&
                    binding.ProductionSessionId==fixture.RoiContext.ProductionSessionId &&
                    binding.RenderFrameCount==2 &&
                    binding.FirstSequence==1 &&
                    binding.LastSequence==2 &&
                    binding.BindingFingerprint.Length==64 &&
                    (group3<9 || round==100),
                    "valid ROI input/render replay binding must be accepted");
            }
        }
        for(var group4=0;group4<10;group4++)
        {
            for(var i4=0;i4<10;i4++)
            {
                round++;
                var fixture=RoiInputRenderReplayFixtureRuntime.Create();
                var binding=ProductionRoiInputRecoveryRenderReplayBindingRuntime.Create(fixture.RoiContext,fixture.RenderContext);
                Check(
                    ProductionRoiInputRecoveryRenderReplayBindingRuntime.IsValid(fixture.RoiContext,fixture.RenderContext) &&
                    binding.ProductionSessionId==fixture.RoiContext.ProductionSessionId &&
                    binding.RenderFrameCount==2 &&
                    binding.FirstSequence==1 &&
                    binding.LastSequence==2 &&
                    binding.BindingFingerprint.Length==64 &&
                    (group4<9 || round==100),
                    "valid ROI input/render replay binding must be accepted");
            }
        }
        for(var group5=0;group5<10;group5++)
        {
            for(var i5=0;i5<10;i5++)
            {
                round++;
                var fixture=RoiInputRenderReplayFixtureRuntime.Create();
                var binding=ProductionRoiInputRecoveryRenderReplayBindingRuntime.Create(fixture.RoiContext,fixture.RenderContext);
                Check(
                    ProductionRoiInputRecoveryRenderReplayBindingRuntime.IsValid(fixture.RoiContext,fixture.RenderContext) &&
                    binding.ProductionSessionId==fixture.RoiContext.ProductionSessionId &&
                    binding.RenderFrameCount==2 &&
                    binding.FirstSequence==1 &&
                    binding.LastSequence==2 &&
                    binding.BindingFingerprint.Length==64 &&
                    (group5<9 || round==100),
                    "valid ROI input/render replay binding must be accepted");
            }
        }
        for(var group6=0;group6<10;group6++)
        {
            for(var i6=0;i6<10;i6++)
            {
                round++;
                var fixture=RoiInputRenderReplayFixtureRuntime.Create();
                var binding=ProductionRoiInputRecoveryRenderReplayBindingRuntime.Create(fixture.RoiContext,fixture.RenderContext);
                Check(
                    ProductionRoiInputRecoveryRenderReplayBindingRuntime.IsValid(fixture.RoiContext,fixture.RenderContext) &&
                    binding.ProductionSessionId==fixture.RoiContext.ProductionSessionId &&
                    binding.RenderFrameCount==2 &&
                    binding.FirstSequence==1 &&
                    binding.LastSequence==2 &&
                    binding.BindingFingerprint.Length==64 &&
                    (group6<9 || round==100),
                    "valid ROI input/render replay binding must be accepted");
            }
        }
        for(var group7=0;group7<10;group7++)
        {
            for(var i7=0;i7<10;i7++)
            {
                round++;
                var fixture=RoiInputRenderReplayFixtureRuntime.Create();
                var binding=ProductionRoiInputRecoveryRenderReplayBindingRuntime.Create(fixture.RoiContext,fixture.RenderContext);
                Check(
                    ProductionRoiInputRecoveryRenderReplayBindingRuntime.IsValid(fixture.RoiContext,fixture.RenderContext) &&
                    binding.ProductionSessionId==fixture.RoiContext.ProductionSessionId &&
                    binding.RenderFrameCount==2 &&
                    binding.FirstSequence==1 &&
                    binding.LastSequence==2 &&
                    binding.BindingFingerprint.Length==64 &&
                    (group7<9 || round==100),
                    "valid ROI input/render replay binding must be accepted");
            }
        }
        for(var group8=0;group8<10;group8++)
        {
            for(var i8=0;i8<10;i8++)
            {
                round++;
                var fixture=RoiInputRenderReplayFixtureRuntime.Create();
                var binding=ProductionRoiInputRecoveryRenderReplayBindingRuntime.Create(fixture.RoiContext,fixture.RenderContext);
                Check(
                    ProductionRoiInputRecoveryRenderReplayBindingRuntime.IsValid(fixture.RoiContext,fixture.RenderContext) &&
                    binding.ProductionSessionId==fixture.RoiContext.ProductionSessionId &&
                    binding.RenderFrameCount==2 &&
                    binding.FirstSequence==1 &&
                    binding.LastSequence==2 &&
                    binding.BindingFingerprint.Length==64 &&
                    (group8<9 || round==100),
                    "valid ROI input/render replay binding must be accepted");
            }
        }
        for(var group9=0;group9<10;group9++)
        {
            for(var i9=0;i9<10;i9++)
            {
                round++;
                var fixture=RoiInputRenderReplayFixtureRuntime.Create();
                var binding=ProductionRoiInputRecoveryRenderReplayBindingRuntime.Create(fixture.RoiContext,fixture.RenderContext);
                Check(
                    ProductionRoiInputRecoveryRenderReplayBindingRuntime.IsValid(fixture.RoiContext,fixture.RenderContext) &&
                    binding.ProductionSessionId==fixture.RoiContext.ProductionSessionId &&
                    binding.RenderFrameCount==2 &&
                    binding.FirstSequence==1 &&
                    binding.LastSequence==2 &&
                    binding.BindingFingerprint.Length==64 &&
                    (group9<9 || round==100),
                    "valid ROI input/render replay binding must be accepted");
            }
        }
        return Task.CompletedTask;
    }
}
