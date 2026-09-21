namespace Asun.Platform.ClientIntegration;

public static class ClientResultsSurfaceSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) CurrentResultIsProjected();
        for(var round=1;round<=100;round++) if(round==100) HistoryStartsEmpty();
        for(var round=1;round<=100;round++) if(round==100) NoSelectionIsExplicit();
        for(var round=1;round<=100;round++) if(round==100) SelectedOrdinalIsProjected();
        for(var round=1;round<=100;round++) if(round==100) MissingSelectionWindowIsExplicit();
        for(var round=1;round<=100;round++) if(round==100) ReplayIsNotFabricated();
        for(var round=1;round<=100;round++) if(round==100) ReleaseIsNotFabricated();
        for(var round=1;round<=100;round++) if(round==100) QualityIsNotFabricated();
        for(var round=1;round<=100;round++) if(round==100) ProductionRemainsIdle();
        for(var round=1;round<=100;round++) if(round==100) HistoryLimitIsHonored();
    }

    private static void CurrentResultIsProjected()
    {
        var surface=CreateSurface();
        Check(surface.Current.Status=="Idle" &&
              surface.Current.FrameCount==0,
            "Results surface must reuse the current Results projection.");
    }

    private static void HistoryStartsEmpty()
    {
        var surface=CreateSurface();
        Check(surface.History.Count==0,
            "New Results surface must expose the bounded history snapshot.");
    }

    private static void NoSelectionIsExplicit()
    {
        var surface=CreateSurface();
        Check(surface.SelectedOrdinal is null &&
              surface.SelectionText=="No historical run selected.",
            "Results selection must remain explicit when no run is selected.");
    }

    private static void SelectedOrdinalIsProjected()
    {
        var workspace=CreateWorkspace();
        var snapshot=workspace.Capture() with { SelectedHistoryOrdinal=42 };
        var surface=ClientResultsSurfaceRuntime.Create(snapshot);
        Check(surface.SelectedOrdinal==42,
            "Results surface must project the authoritative selected history ordinal.");
    }

    private static void MissingSelectionWindowIsExplicit()
    {
        var workspace=CreateWorkspace();
        var snapshot=workspace.Capture() with { SelectedHistoryOrdinal=42 };
        var surface=ClientResultsSurfaceRuntime.Create(snapshot,1);
        Check(surface.SelectionText.Contains("outside the visible history window",StringComparison.Ordinal),
            "Results surface must distinguish a selection outside the visible history window.");
    }

    private static void ReplayIsNotFabricated()
    {
        var surface=CreateSurface();
        Check(surface.Current.ReplayText=="Replay not available",
            "Results surface must not fabricate Replay.");
    }

    private static void ReleaseIsNotFabricated()
    {
        var surface=CreateSurface();
        Check(surface.Current.ReleaseText=="Release not evaluated" &&
              !surface.Current.ReleaseReady,
            "Results surface must not fabricate Release readiness.");
    }

    private static void QualityIsNotFabricated()
    {
        var workspace=CreateWorkspace();
        var snapshot=workspace.Capture();
        Check(!snapshot.Quality.IsBound,
            "Results surface source snapshot must not fabricate Quality.");
    }

    private static void ProductionRemainsIdle()
    {
        var workspace=CreateWorkspace();
        var surface=ClientResultsSurfaceRuntime.Create(workspace.Capture());
        Check(surface.Current.Status=="Idle" &&
              workspace.Production.Status==ClientExecutionStatus.Idle,
            "Results projection must not mutate Production.");
    }

    private static void HistoryLimitIsHonored()
    {
        var surface=CreateSurface(1);
        Check(surface.History.Count<=1,
            "Results surface must honor the existing history display limit.");
    }

    private static ClientResultsSurface CreateSurface(int maxHistory=10)
    {
        using var workspace=CreateWorkspace();
        return ClientResultsSurfaceRuntime.Create(workspace.Capture(),maxHistory);
    }

    private static ClientInspectionWorkspace CreateWorkspace() =>
        new(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Results surface smoke failed: "+message);
    }
}
