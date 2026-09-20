namespace Asun.Platform.ReplayIntegration.Smoke;

public static class ProductionSimulationRenderReplayConvergence4HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var group0=0;group0<10;group0++)
        {
            round++;
                var f=ProductionSimulationRenderReplayConvergenceSmokeFixtures.Create();
                var tampered=f.RenderDescriptors.ToArray();
                tampered[0%2]=tampered[0%2] with { RenderFingerprint = "8888888888888888888888888888888888888888888888888888888888888888" };
                Check(!ProductionSimulationRenderReplayConvergenceRuntime.IsValid(f.SimulationDescriptor,f.SimulationBinding,f.RenderFrames,tampered) &&
                      (group0<9 || round==100),
                      "render descriptor identity drift must be rejected");
        }
        for(var group1=0;group1<10;group1++)
        {
            round++;
                var f=ProductionSimulationRenderReplayConvergenceSmokeFixtures.Create();
                var tampered=f.RenderDescriptors.ToArray();
                tampered[1%2]=tampered[1%2] with { RenderFingerprint = "8888888888888888888888888888888888888888888888888888888888888888" };
                Check(!ProductionSimulationRenderReplayConvergenceRuntime.IsValid(f.SimulationDescriptor,f.SimulationBinding,f.RenderFrames,tampered) &&
                      (group1<9 || round==100),
                      "render descriptor identity drift must be rejected");
        }
        for(var group2=0;group2<10;group2++)
        {
            round++;
                var f=ProductionSimulationRenderReplayConvergenceSmokeFixtures.Create();
                var tampered=f.RenderDescriptors.ToArray();
                tampered[2%2]=tampered[2%2] with { RenderFingerprint = "8888888888888888888888888888888888888888888888888888888888888888" };
                Check(!ProductionSimulationRenderReplayConvergenceRuntime.IsValid(f.SimulationDescriptor,f.SimulationBinding,f.RenderFrames,tampered) &&
                      (group2<9 || round==100),
                      "render descriptor identity drift must be rejected");
        }
        for(var group3=0;group3<10;group3++)
        {
            round++;
                var f=ProductionSimulationRenderReplayConvergenceSmokeFixtures.Create();
                var tampered=f.RenderDescriptors.ToArray();
                tampered[3%2]=tampered[3%2] with { RenderFingerprint = "8888888888888888888888888888888888888888888888888888888888888888" };
                Check(!ProductionSimulationRenderReplayConvergenceRuntime.IsValid(f.SimulationDescriptor,f.SimulationBinding,f.RenderFrames,tampered) &&
                      (group3<9 || round==100),
                      "render descriptor identity drift must be rejected");
        }
        for(var group4=0;group4<10;group4++)
        {
            round++;
                var f=ProductionSimulationRenderReplayConvergenceSmokeFixtures.Create();
                var tampered=f.RenderDescriptors.ToArray();
                tampered[4%2]=tampered[4%2] with { RenderFingerprint = "8888888888888888888888888888888888888888888888888888888888888888" };
                Check(!ProductionSimulationRenderReplayConvergenceRuntime.IsValid(f.SimulationDescriptor,f.SimulationBinding,f.RenderFrames,tampered) &&
                      (group4<9 || round==100),
                      "render descriptor identity drift must be rejected");
        }
        for(var group5=0;group5<10;group5++)
        {
            round++;
                var f=ProductionSimulationRenderReplayConvergenceSmokeFixtures.Create();
                var tampered=f.RenderDescriptors.ToArray();
                tampered[5%2]=tampered[5%2] with { RenderFingerprint = "8888888888888888888888888888888888888888888888888888888888888888" };
                Check(!ProductionSimulationRenderReplayConvergenceRuntime.IsValid(f.SimulationDescriptor,f.SimulationBinding,f.RenderFrames,tampered) &&
                      (group5<9 || round==100),
                      "render descriptor identity drift must be rejected");
        }
        for(var group6=0;group6<10;group6++)
        {
            round++;
                var f=ProductionSimulationRenderReplayConvergenceSmokeFixtures.Create();
                var tampered=f.RenderDescriptors.ToArray();
                tampered[6%2]=tampered[6%2] with { RenderFingerprint = "8888888888888888888888888888888888888888888888888888888888888888" };
                Check(!ProductionSimulationRenderReplayConvergenceRuntime.IsValid(f.SimulationDescriptor,f.SimulationBinding,f.RenderFrames,tampered) &&
                      (group6<9 || round==100),
                      "render descriptor identity drift must be rejected");
        }
        for(var group7=0;group7<10;group7++)
        {
            round++;
                var f=ProductionSimulationRenderReplayConvergenceSmokeFixtures.Create();
                var tampered=f.RenderDescriptors.ToArray();
                tampered[7%2]=tampered[7%2] with { RenderFingerprint = "8888888888888888888888888888888888888888888888888888888888888888" };
                Check(!ProductionSimulationRenderReplayConvergenceRuntime.IsValid(f.SimulationDescriptor,f.SimulationBinding,f.RenderFrames,tampered) &&
                      (group7<9 || round==100),
                      "render descriptor identity drift must be rejected");
        }
        for(var group8=0;group8<10;group8++)
        {
            round++;
                var f=ProductionSimulationRenderReplayConvergenceSmokeFixtures.Create();
                var tampered=f.RenderDescriptors.ToArray();
                tampered[8%2]=tampered[8%2] with { RenderFingerprint = "8888888888888888888888888888888888888888888888888888888888888888" };
                Check(!ProductionSimulationRenderReplayConvergenceRuntime.IsValid(f.SimulationDescriptor,f.SimulationBinding,f.RenderFrames,tampered) &&
                      (group8<9 || round==100),
                      "render descriptor identity drift must be rejected");
        }
        for(var group9=0;group9<10;group9++)
        {
            round++;
                var f=ProductionSimulationRenderReplayConvergenceSmokeFixtures.Create();
                var tampered=f.RenderDescriptors.ToArray();
                tampered[9%2]=tampered[9%2] with { RenderFingerprint = "8888888888888888888888888888888888888888888888888888888888888888" };
                Check(!ProductionSimulationRenderReplayConvergenceRuntime.IsValid(f.SimulationDescriptor,f.SimulationBinding,f.RenderFrames,tampered) &&
                      (group9<9 || round==100),
                      "render descriptor identity drift must be rejected");
        }
        return Task.CompletedTask;
    }
}
