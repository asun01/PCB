using Asun.Platform.Evidence;
using Asun.Platform.RoiProductionIntegration;

namespace Asun.Platform.RoiProductionIntegration.Smoke;

public static class ProductionRoiRenderEvidenceReplayContext4HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var group0=0;group0<10;group0++)
        {
            round++;
                var f=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var index=group0%2;
                var tampered=f.EvidenceDescriptors.ToArray();
                tampered[index]=tampered[index] with { DescriptorFingerprint=new string('c',64) };
                Check(!ProductionRoiRenderEvidenceReplayContextRuntime.IsValid(f.Production,f.RoiContext,f.RenderFrames,tampered) &&
                      (group0<9 || round==100),
                      "evidence descriptor integrity drift must be rejected");
        }
        for(var group1=0;group1<10;group1++)
        {
            round++;
                var f=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var index=group1%2;
                var tampered=f.EvidenceDescriptors.ToArray();
                tampered[index]=tampered[index] with { DescriptorFingerprint=new string('c',64) };
                Check(!ProductionRoiRenderEvidenceReplayContextRuntime.IsValid(f.Production,f.RoiContext,f.RenderFrames,tampered) &&
                      (group1<9 || round==100),
                      "evidence descriptor integrity drift must be rejected");
        }
        for(var group2=0;group2<10;group2++)
        {
            round++;
                var f=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var index=group2%2;
                var tampered=f.EvidenceDescriptors.ToArray();
                tampered[index]=tampered[index] with { DescriptorFingerprint=new string('c',64) };
                Check(!ProductionRoiRenderEvidenceReplayContextRuntime.IsValid(f.Production,f.RoiContext,f.RenderFrames,tampered) &&
                      (group2<9 || round==100),
                      "evidence descriptor integrity drift must be rejected");
        }
        for(var group3=0;group3<10;group3++)
        {
            round++;
                var f=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var index=group3%2;
                var tampered=f.EvidenceDescriptors.ToArray();
                tampered[index]=tampered[index] with { DescriptorFingerprint=new string('c',64) };
                Check(!ProductionRoiRenderEvidenceReplayContextRuntime.IsValid(f.Production,f.RoiContext,f.RenderFrames,tampered) &&
                      (group3<9 || round==100),
                      "evidence descriptor integrity drift must be rejected");
        }
        for(var group4=0;group4<10;group4++)
        {
            round++;
                var f=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var index=group4%2;
                var tampered=f.EvidenceDescriptors.ToArray();
                tampered[index]=tampered[index] with { DescriptorFingerprint=new string('c',64) };
                Check(!ProductionRoiRenderEvidenceReplayContextRuntime.IsValid(f.Production,f.RoiContext,f.RenderFrames,tampered) &&
                      (group4<9 || round==100),
                      "evidence descriptor integrity drift must be rejected");
        }
        for(var group5=0;group5<10;group5++)
        {
            round++;
                var f=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var index=group5%2;
                var tampered=f.EvidenceDescriptors.ToArray();
                tampered[index]=tampered[index] with { DescriptorFingerprint=new string('c',64) };
                Check(!ProductionRoiRenderEvidenceReplayContextRuntime.IsValid(f.Production,f.RoiContext,f.RenderFrames,tampered) &&
                      (group5<9 || round==100),
                      "evidence descriptor integrity drift must be rejected");
        }
        for(var group6=0;group6<10;group6++)
        {
            round++;
                var f=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var index=group6%2;
                var tampered=f.EvidenceDescriptors.ToArray();
                tampered[index]=tampered[index] with { DescriptorFingerprint=new string('c',64) };
                Check(!ProductionRoiRenderEvidenceReplayContextRuntime.IsValid(f.Production,f.RoiContext,f.RenderFrames,tampered) &&
                      (group6<9 || round==100),
                      "evidence descriptor integrity drift must be rejected");
        }
        for(var group7=0;group7<10;group7++)
        {
            round++;
                var f=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var index=group7%2;
                var tampered=f.EvidenceDescriptors.ToArray();
                tampered[index]=tampered[index] with { DescriptorFingerprint=new string('c',64) };
                Check(!ProductionRoiRenderEvidenceReplayContextRuntime.IsValid(f.Production,f.RoiContext,f.RenderFrames,tampered) &&
                      (group7<9 || round==100),
                      "evidence descriptor integrity drift must be rejected");
        }
        for(var group8=0;group8<10;group8++)
        {
            round++;
                var f=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var index=group8%2;
                var tampered=f.EvidenceDescriptors.ToArray();
                tampered[index]=tampered[index] with { DescriptorFingerprint=new string('c',64) };
                Check(!ProductionRoiRenderEvidenceReplayContextRuntime.IsValid(f.Production,f.RoiContext,f.RenderFrames,tampered) &&
                      (group8<9 || round==100),
                      "evidence descriptor integrity drift must be rejected");
        }
        for(var group9=0;group9<10;group9++)
        {
            round++;
                var f=ProductionRoiRenderEvidenceReplaySmokeFixtures.Create();
                var index=group9%2;
                var tampered=f.EvidenceDescriptors.ToArray();
                tampered[index]=tampered[index] with { DescriptorFingerprint=new string('c',64) };
                Check(!ProductionRoiRenderEvidenceReplayContextRuntime.IsValid(f.Production,f.RoiContext,f.RenderFrames,tampered) &&
                      (group9<9 || round==100),
                      "evidence descriptor integrity drift must be rejected");
        }
        return Task.CompletedTask;
    }
}
