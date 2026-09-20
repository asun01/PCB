namespace Asun.Platform.ReplayIntegration.Smoke;

public static class ProductionSimulationRenderReplayConvergence5HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var group0=0;group0<10;group0++)
        {
            round++;
                var f=ProductionSimulationRenderReplayConvergenceSmokeFixtures.Create();
                var tamperedDescriptor=f.SimulationDescriptor with { FrameCount=1 };
                var tamperedBinding=f.SimulationBinding with { Frames=f.SimulationBinding.Frames.Take(1).ToArray() };
                Check(!ProductionSimulationRenderReplayConvergenceRuntime.IsValid(tamperedDescriptor,f.SimulationBinding,f.RenderFrames,f.RenderDescriptors) &&
                      !ProductionSimulationRenderReplayConvergenceRuntime.IsValid(f.SimulationDescriptor,tamperedBinding,f.RenderFrames,f.RenderDescriptors) &&
                      (group0<9 || round==100),
                      "simulation frame-count and binding-count drift must be rejected");
        }
        for(var group1=0;group1<10;group1++)
        {
            round++;
                var f=ProductionSimulationRenderReplayConvergenceSmokeFixtures.Create();
                var tamperedDescriptor=f.SimulationDescriptor with { FrameCount=1 };
                var tamperedBinding=f.SimulationBinding with { Frames=f.SimulationBinding.Frames.Take(1).ToArray() };
                Check(!ProductionSimulationRenderReplayConvergenceRuntime.IsValid(tamperedDescriptor,f.SimulationBinding,f.RenderFrames,f.RenderDescriptors) &&
                      !ProductionSimulationRenderReplayConvergenceRuntime.IsValid(f.SimulationDescriptor,tamperedBinding,f.RenderFrames,f.RenderDescriptors) &&
                      (group1<9 || round==100),
                      "simulation frame-count and binding-count drift must be rejected");
        }
        for(var group2=0;group2<10;group2++)
        {
            round++;
                var f=ProductionSimulationRenderReplayConvergenceSmokeFixtures.Create();
                var tamperedDescriptor=f.SimulationDescriptor with { FrameCount=1 };
                var tamperedBinding=f.SimulationBinding with { Frames=f.SimulationBinding.Frames.Take(1).ToArray() };
                Check(!ProductionSimulationRenderReplayConvergenceRuntime.IsValid(tamperedDescriptor,f.SimulationBinding,f.RenderFrames,f.RenderDescriptors) &&
                      !ProductionSimulationRenderReplayConvergenceRuntime.IsValid(f.SimulationDescriptor,tamperedBinding,f.RenderFrames,f.RenderDescriptors) &&
                      (group2<9 || round==100),
                      "simulation frame-count and binding-count drift must be rejected");
        }
        for(var group3=0;group3<10;group3++)
        {
            round++;
                var f=ProductionSimulationRenderReplayConvergenceSmokeFixtures.Create();
                var tamperedDescriptor=f.SimulationDescriptor with { FrameCount=1 };
                var tamperedBinding=f.SimulationBinding with { Frames=f.SimulationBinding.Frames.Take(1).ToArray() };
                Check(!ProductionSimulationRenderReplayConvergenceRuntime.IsValid(tamperedDescriptor,f.SimulationBinding,f.RenderFrames,f.RenderDescriptors) &&
                      !ProductionSimulationRenderReplayConvergenceRuntime.IsValid(f.SimulationDescriptor,tamperedBinding,f.RenderFrames,f.RenderDescriptors) &&
                      (group3<9 || round==100),
                      "simulation frame-count and binding-count drift must be rejected");
        }
        for(var group4=0;group4<10;group4++)
        {
            round++;
                var f=ProductionSimulationRenderReplayConvergenceSmokeFixtures.Create();
                var tamperedDescriptor=f.SimulationDescriptor with { FrameCount=1 };
                var tamperedBinding=f.SimulationBinding with { Frames=f.SimulationBinding.Frames.Take(1).ToArray() };
                Check(!ProductionSimulationRenderReplayConvergenceRuntime.IsValid(tamperedDescriptor,f.SimulationBinding,f.RenderFrames,f.RenderDescriptors) &&
                      !ProductionSimulationRenderReplayConvergenceRuntime.IsValid(f.SimulationDescriptor,tamperedBinding,f.RenderFrames,f.RenderDescriptors) &&
                      (group4<9 || round==100),
                      "simulation frame-count and binding-count drift must be rejected");
        }
        for(var group5=0;group5<10;group5++)
        {
            round++;
                var f=ProductionSimulationRenderReplayConvergenceSmokeFixtures.Create();
                var tamperedDescriptor=f.SimulationDescriptor with { FrameCount=1 };
                var tamperedBinding=f.SimulationBinding with { Frames=f.SimulationBinding.Frames.Take(1).ToArray() };
                Check(!ProductionSimulationRenderReplayConvergenceRuntime.IsValid(tamperedDescriptor,f.SimulationBinding,f.RenderFrames,f.RenderDescriptors) &&
                      !ProductionSimulationRenderReplayConvergenceRuntime.IsValid(f.SimulationDescriptor,tamperedBinding,f.RenderFrames,f.RenderDescriptors) &&
                      (group5<9 || round==100),
                      "simulation frame-count and binding-count drift must be rejected");
        }
        for(var group6=0;group6<10;group6++)
        {
            round++;
                var f=ProductionSimulationRenderReplayConvergenceSmokeFixtures.Create();
                var tamperedDescriptor=f.SimulationDescriptor with { FrameCount=1 };
                var tamperedBinding=f.SimulationBinding with { Frames=f.SimulationBinding.Frames.Take(1).ToArray() };
                Check(!ProductionSimulationRenderReplayConvergenceRuntime.IsValid(tamperedDescriptor,f.SimulationBinding,f.RenderFrames,f.RenderDescriptors) &&
                      !ProductionSimulationRenderReplayConvergenceRuntime.IsValid(f.SimulationDescriptor,tamperedBinding,f.RenderFrames,f.RenderDescriptors) &&
                      (group6<9 || round==100),
                      "simulation frame-count and binding-count drift must be rejected");
        }
        for(var group7=0;group7<10;group7++)
        {
            round++;
                var f=ProductionSimulationRenderReplayConvergenceSmokeFixtures.Create();
                var tamperedDescriptor=f.SimulationDescriptor with { FrameCount=1 };
                var tamperedBinding=f.SimulationBinding with { Frames=f.SimulationBinding.Frames.Take(1).ToArray() };
                Check(!ProductionSimulationRenderReplayConvergenceRuntime.IsValid(tamperedDescriptor,f.SimulationBinding,f.RenderFrames,f.RenderDescriptors) &&
                      !ProductionSimulationRenderReplayConvergenceRuntime.IsValid(f.SimulationDescriptor,tamperedBinding,f.RenderFrames,f.RenderDescriptors) &&
                      (group7<9 || round==100),
                      "simulation frame-count and binding-count drift must be rejected");
        }
        for(var group8=0;group8<10;group8++)
        {
            round++;
                var f=ProductionSimulationRenderReplayConvergenceSmokeFixtures.Create();
                var tamperedDescriptor=f.SimulationDescriptor with { FrameCount=1 };
                var tamperedBinding=f.SimulationBinding with { Frames=f.SimulationBinding.Frames.Take(1).ToArray() };
                Check(!ProductionSimulationRenderReplayConvergenceRuntime.IsValid(tamperedDescriptor,f.SimulationBinding,f.RenderFrames,f.RenderDescriptors) &&
                      !ProductionSimulationRenderReplayConvergenceRuntime.IsValid(f.SimulationDescriptor,tamperedBinding,f.RenderFrames,f.RenderDescriptors) &&
                      (group8<9 || round==100),
                      "simulation frame-count and binding-count drift must be rejected");
        }
        for(var group9=0;group9<10;group9++)
        {
            round++;
                var f=ProductionSimulationRenderReplayConvergenceSmokeFixtures.Create();
                var tamperedDescriptor=f.SimulationDescriptor with { FrameCount=1 };
                var tamperedBinding=f.SimulationBinding with { Frames=f.SimulationBinding.Frames.Take(1).ToArray() };
                Check(!ProductionSimulationRenderReplayConvergenceRuntime.IsValid(tamperedDescriptor,f.SimulationBinding,f.RenderFrames,f.RenderDescriptors) &&
                      !ProductionSimulationRenderReplayConvergenceRuntime.IsValid(f.SimulationDescriptor,tamperedBinding,f.RenderFrames,f.RenderDescriptors) &&
                      (group9<9 || round==100),
                      "simulation frame-count and binding-count drift must be rejected");
        }
        return Task.CompletedTask;
    }
}
