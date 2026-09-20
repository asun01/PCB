namespace Asun.Platform.RoiProductionIntegration.Smoke;

public static class ProductionRoiInputRecoveryRenderReplayBinding2HundredStageSmoke
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
                var render=fixture.RenderContext with { ProductionSessionId=Guid.Parse("d3000000-0000-0000-0000-000000000001") };
                Check(
                    !ProductionRoiInputRecoveryRenderReplayBindingRuntime.IsValid(fixture.RoiContext,render) &&
                    (group0<9 || round==100),
                    "render replay session drift must be rejected");
            }
        }
        for(var group1=0;group1<10;group1++)
        {
            for(var i1=0;i1<10;i1++)
            {
                round++;
                var fixture=RoiInputRenderReplayFixtureRuntime.Create();
                var render=fixture.RenderContext with { ProductionSessionId=Guid.Parse("d3000000-0000-0000-0000-000000000001") };
                Check(
                    !ProductionRoiInputRecoveryRenderReplayBindingRuntime.IsValid(fixture.RoiContext,render) &&
                    (group1<9 || round==100),
                    "render replay session drift must be rejected");
            }
        }
        for(var group2=0;group2<10;group2++)
        {
            for(var i2=0;i2<10;i2++)
            {
                round++;
                var fixture=RoiInputRenderReplayFixtureRuntime.Create();
                var render=fixture.RenderContext with { ProductionSessionId=Guid.Parse("d3000000-0000-0000-0000-000000000001") };
                Check(
                    !ProductionRoiInputRecoveryRenderReplayBindingRuntime.IsValid(fixture.RoiContext,render) &&
                    (group2<9 || round==100),
                    "render replay session drift must be rejected");
            }
        }
        for(var group3=0;group3<10;group3++)
        {
            for(var i3=0;i3<10;i3++)
            {
                round++;
                var fixture=RoiInputRenderReplayFixtureRuntime.Create();
                var render=fixture.RenderContext with { ProductionSessionId=Guid.Parse("d3000000-0000-0000-0000-000000000001") };
                Check(
                    !ProductionRoiInputRecoveryRenderReplayBindingRuntime.IsValid(fixture.RoiContext,render) &&
                    (group3<9 || round==100),
                    "render replay session drift must be rejected");
            }
        }
        for(var group4=0;group4<10;group4++)
        {
            for(var i4=0;i4<10;i4++)
            {
                round++;
                var fixture=RoiInputRenderReplayFixtureRuntime.Create();
                var render=fixture.RenderContext with { ProductionSessionId=Guid.Parse("d3000000-0000-0000-0000-000000000001") };
                Check(
                    !ProductionRoiInputRecoveryRenderReplayBindingRuntime.IsValid(fixture.RoiContext,render) &&
                    (group4<9 || round==100),
                    "render replay session drift must be rejected");
            }
        }
        for(var group5=0;group5<10;group5++)
        {
            for(var i5=0;i5<10;i5++)
            {
                round++;
                var fixture=RoiInputRenderReplayFixtureRuntime.Create();
                var render=fixture.RenderContext with { ProductionSessionId=Guid.Parse("d3000000-0000-0000-0000-000000000001") };
                Check(
                    !ProductionRoiInputRecoveryRenderReplayBindingRuntime.IsValid(fixture.RoiContext,render) &&
                    (group5<9 || round==100),
                    "render replay session drift must be rejected");
            }
        }
        for(var group6=0;group6<10;group6++)
        {
            for(var i6=0;i6<10;i6++)
            {
                round++;
                var fixture=RoiInputRenderReplayFixtureRuntime.Create();
                var render=fixture.RenderContext with { ProductionSessionId=Guid.Parse("d3000000-0000-0000-0000-000000000001") };
                Check(
                    !ProductionRoiInputRecoveryRenderReplayBindingRuntime.IsValid(fixture.RoiContext,render) &&
                    (group6<9 || round==100),
                    "render replay session drift must be rejected");
            }
        }
        for(var group7=0;group7<10;group7++)
        {
            for(var i7=0;i7<10;i7++)
            {
                round++;
                var fixture=RoiInputRenderReplayFixtureRuntime.Create();
                var render=fixture.RenderContext with { ProductionSessionId=Guid.Parse("d3000000-0000-0000-0000-000000000001") };
                Check(
                    !ProductionRoiInputRecoveryRenderReplayBindingRuntime.IsValid(fixture.RoiContext,render) &&
                    (group7<9 || round==100),
                    "render replay session drift must be rejected");
            }
        }
        for(var group8=0;group8<10;group8++)
        {
            for(var i8=0;i8<10;i8++)
            {
                round++;
                var fixture=RoiInputRenderReplayFixtureRuntime.Create();
                var render=fixture.RenderContext with { ProductionSessionId=Guid.Parse("d3000000-0000-0000-0000-000000000001") };
                Check(
                    !ProductionRoiInputRecoveryRenderReplayBindingRuntime.IsValid(fixture.RoiContext,render) &&
                    (group8<9 || round==100),
                    "render replay session drift must be rejected");
            }
        }
        for(var group9=0;group9<10;group9++)
        {
            for(var i9=0;i9<10;i9++)
            {
                round++;
                var fixture=RoiInputRenderReplayFixtureRuntime.Create();
                var render=fixture.RenderContext with { ProductionSessionId=Guid.Parse("d3000000-0000-0000-0000-000000000001") };
                Check(
                    !ProductionRoiInputRecoveryRenderReplayBindingRuntime.IsValid(fixture.RoiContext,render) &&
                    (group9<9 || round==100),
                    "render replay session drift must be rejected");
            }
        }
        return Task.CompletedTask;
    }
}
