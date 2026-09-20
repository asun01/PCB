using Asun.Platform.Evidence;
using Asun.Platform.RoiProductionIntegration;

namespace Asun.Platform.RoiProductionIntegration.Smoke;

public static class ProductionRoiRenderEvidenceReplayContext5HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var group0=0;group0<10;group0++)
        {
            round++;
                var left=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var right=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var a=ProductionRoiRenderEvidenceReplayContextRuntime.Create(left.Production,left.RoiContext,left.RenderFrames,left.EvidenceDescriptors);
                var b=ProductionRoiRenderEvidenceReplayContextRuntime.Create(right.Production,right.RoiContext,right.RenderFrames,right.EvidenceDescriptors);
                Check(ProductionRoiRenderEvidenceReplayContextRuntime.IsEquivalent(a,b) &&
                      a.EvidenceFingerprint==b.EvidenceFingerprint &&
                      a.BindingFingerprint==b.BindingFingerprint &&
                      (group0<9 || round==100),
                      "equivalent ROI render evidence contexts must converge deterministically");
        }
        for(var group1=0;group1<10;group1++)
        {
            round++;
                var left=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var right=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var a=ProductionRoiRenderEvidenceReplayContextRuntime.Create(left.Production,left.RoiContext,left.RenderFrames,left.EvidenceDescriptors);
                var b=ProductionRoiRenderEvidenceReplayContextRuntime.Create(right.Production,right.RoiContext,right.RenderFrames,right.EvidenceDescriptors);
                Check(ProductionRoiRenderEvidenceReplayContextRuntime.IsEquivalent(a,b) &&
                      a.EvidenceFingerprint==b.EvidenceFingerprint &&
                      a.BindingFingerprint==b.BindingFingerprint &&
                      (group1<9 || round==100),
                      "equivalent ROI render evidence contexts must converge deterministically");
        }
        for(var group2=0;group2<10;group2++)
        {
            round++;
                var left=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var right=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var a=ProductionRoiRenderEvidenceReplayContextRuntime.Create(left.Production,left.RoiContext,left.RenderFrames,left.EvidenceDescriptors);
                var b=ProductionRoiRenderEvidenceReplayContextRuntime.Create(right.Production,right.RoiContext,right.RenderFrames,right.EvidenceDescriptors);
                Check(ProductionRoiRenderEvidenceReplayContextRuntime.IsEquivalent(a,b) &&
                      a.EvidenceFingerprint==b.EvidenceFingerprint &&
                      a.BindingFingerprint==b.BindingFingerprint &&
                      (group2<9 || round==100),
                      "equivalent ROI render evidence contexts must converge deterministically");
        }
        for(var group3=0;group3<10;group3++)
        {
            round++;
                var left=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var right=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var a=ProductionRoiRenderEvidenceReplayContextRuntime.Create(left.Production,left.RoiContext,left.RenderFrames,left.EvidenceDescriptors);
                var b=ProductionRoiRenderEvidenceReplayContextRuntime.Create(right.Production,right.RoiContext,right.RenderFrames,right.EvidenceDescriptors);
                Check(ProductionRoiRenderEvidenceReplayContextRuntime.IsEquivalent(a,b) &&
                      a.EvidenceFingerprint==b.EvidenceFingerprint &&
                      a.BindingFingerprint==b.BindingFingerprint &&
                      (group3<9 || round==100),
                      "equivalent ROI render evidence contexts must converge deterministically");
        }
        for(var group4=0;group4<10;group4++)
        {
            round++;
                var left=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var right=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var a=ProductionRoiRenderEvidenceReplayContextRuntime.Create(left.Production,left.RoiContext,left.RenderFrames,left.EvidenceDescriptors);
                var b=ProductionRoiRenderEvidenceReplayContextRuntime.Create(right.Production,right.RoiContext,right.RenderFrames,right.EvidenceDescriptors);
                Check(ProductionRoiRenderEvidenceReplayContextRuntime.IsEquivalent(a,b) &&
                      a.EvidenceFingerprint==b.EvidenceFingerprint &&
                      a.BindingFingerprint==b.BindingFingerprint &&
                      (group4<9 || round==100),
                      "equivalent ROI render evidence contexts must converge deterministically");
        }
        for(var group5=0;group5<10;group5++)
        {
            round++;
                var left=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var right=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var a=ProductionRoiRenderEvidenceReplayContextRuntime.Create(left.Production,left.RoiContext,left.RenderFrames,left.EvidenceDescriptors);
                var b=ProductionRoiRenderEvidenceReplayContextRuntime.Create(right.Production,right.RoiContext,right.RenderFrames,right.EvidenceDescriptors);
                Check(ProductionRoiRenderEvidenceReplayContextRuntime.IsEquivalent(a,b) &&
                      a.EvidenceFingerprint==b.EvidenceFingerprint &&
                      a.BindingFingerprint==b.BindingFingerprint &&
                      (group5<9 || round==100),
                      "equivalent ROI render evidence contexts must converge deterministically");
        }
        for(var group6=0;group6<10;group6++)
        {
            round++;
                var left=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var right=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var a=ProductionRoiRenderEvidenceReplayContextRuntime.Create(left.Production,left.RoiContext,left.RenderFrames,left.EvidenceDescriptors);
                var b=ProductionRoiRenderEvidenceReplayContextRuntime.Create(right.Production,right.RoiContext,right.RenderFrames,right.EvidenceDescriptors);
                Check(ProductionRoiRenderEvidenceReplayContextRuntime.IsEquivalent(a,b) &&
                      a.EvidenceFingerprint==b.EvidenceFingerprint &&
                      a.BindingFingerprint==b.BindingFingerprint &&
                      (group6<9 || round==100),
                      "equivalent ROI render evidence contexts must converge deterministically");
        }
        for(var group7=0;group7<10;group7++)
        {
            round++;
                var left=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var right=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var a=ProductionRoiRenderEvidenceReplayContextRuntime.Create(left.Production,left.RoiContext,left.RenderFrames,left.EvidenceDescriptors);
                var b=ProductionRoiRenderEvidenceReplayContextRuntime.Create(right.Production,right.RoiContext,right.RenderFrames,right.EvidenceDescriptors);
                Check(ProductionRoiRenderEvidenceReplayContextRuntime.IsEquivalent(a,b) &&
                      a.EvidenceFingerprint==b.EvidenceFingerprint &&
                      a.BindingFingerprint==b.BindingFingerprint &&
                      (group7<9 || round==100),
                      "equivalent ROI render evidence contexts must converge deterministically");
        }
        for(var group8=0;group8<10;group8++)
        {
            round++;
                var left=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var right=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var a=ProductionRoiRenderEvidenceReplayContextRuntime.Create(left.Production,left.RoiContext,left.RenderFrames,left.EvidenceDescriptors);
                var b=ProductionRoiRenderEvidenceReplayContextRuntime.Create(right.Production,right.RoiContext,right.RenderFrames,right.EvidenceDescriptors);
                Check(ProductionRoiRenderEvidenceReplayContextRuntime.IsEquivalent(a,b) &&
                      a.EvidenceFingerprint==b.EvidenceFingerprint &&
                      a.BindingFingerprint==b.BindingFingerprint &&
                      (group8<9 || round==100),
                      "equivalent ROI render evidence contexts must converge deterministically");
        }
        for(var group9=0;group9<10;group9++)
        {
            round++;
                var left=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var right=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var a=ProductionRoiRenderEvidenceReplayContextRuntime.Create(left.Production,left.RoiContext,left.RenderFrames,left.EvidenceDescriptors);
                var b=ProductionRoiRenderEvidenceReplayContextRuntime.Create(right.Production,right.RoiContext,right.RenderFrames,right.EvidenceDescriptors);
                Check(ProductionRoiRenderEvidenceReplayContextRuntime.IsEquivalent(a,b) &&
                      a.EvidenceFingerprint==b.EvidenceFingerprint &&
                      a.BindingFingerprint==b.BindingFingerprint &&
                      (group9<9 || round==100),
                      "equivalent ROI render evidence contexts must converge deterministically");
        }
        return Task.CompletedTask;
    }
}
