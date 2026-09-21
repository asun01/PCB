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
        using var workspace=CreateWorkspace();
        var programId=Guid.NewGuid();
        var sessionId=Guid.NewGuid();
        var replayFingerprint=new string('a',64);
        var snapshot=workspace.Capture() with
        {
            Production=workspace.Production with
            {
                ProgramId=programId,
                ProgramVersion=new Version(1,0),
                ActiveSessionId=sessionId,
                Status=ClientExecutionStatus.Completed,
                LastFrameCount=3
            },
            Replay=new ClientProductionReplaySnapshot(
                programId,
                new Version(1,0),
                sessionId,
                ClientExecutionStatus.Completed,
                3,
                new string('b',64),
                replayFingerprint),
            Release=new ClientReleaseProjection(
                programId,
                sessionId,
                replayFingerprint,
                true,
                "artifact.bin",
                new string('c',64),
                new string('d',64))
        };
        var surface=ClientResultsSurfaceRuntime.Create(snapshot);

        Check(surface.Current.Status=="Completed" &&
              surface.Current.FrameCount==3 &&
              surface.ReleaseReplay.ReplayAvailable &&
              surface.ReleaseReplay.ReleaseReady &&
              surface.ReleaseReplay.ReplayFingerprint==replayFingerprint,
            "Results surface must reuse the structured current Replay/Release projections.");
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
        Check(surface.SelectedOrdinal==42 &&
              surface.SelectedHistoryItem is null,
            "Results surface must preserve an unavailable selected ordinal without fabricating history detail.");
    }

    private static void MissingSelectionWindowIsExplicit()
    {
        var workspace=CreateWorkspace();
        var snapshot=workspace.Capture() with
        {
            SelectedHistoryOrdinal=42,
            History=new ClientProductionRunHistorySnapshot(
                20,
                43,
                0,
                new[]
                {
                    new ClientProductionRunHistoryEntry(
                        42,
                        Guid.NewGuid(),
                        Guid.NewGuid(),
                        3,
                        new string('a',64),
                        true,
                        "artifact.bin")
                })
        };
        var surface=ClientResultsSurfaceRuntime.Create(snapshot,1);
        Check(surface.SelectedHistoryItem?.Ordinal==42 &&
              surface.SelectionText=="Selected Run 42",
            "Results surface must expose selected history detail independently of the visible list limit.");
    }

    private static void ReplayIsNotFabricated()
    {
        var surface=CreateSurface();
        var workspace=CreateWorkspace();
        var malformed=workspace.Capture() with
        {
            Replay=new ClientProductionReplaySnapshot(
                Guid.NewGuid(),
                new Version(1,0),
                Guid.NewGuid(),
                ClientExecutionStatus.Completed,
                1,
                new string('a',64),
                "short")
        };
        var malformedDisplay=ClientResultsPresentationRuntime.CreateCurrent(malformed);
        Check(surface.Current.ReplayText=="Replay not available" &&
              !surface.ReleaseReplay.ReplayAvailable &&
              malformedDisplay.ReplayText=="Replay fingerprint invalid",
            "Results surface must not fabricate Replay and must not throw on malformed replay evidence.");
    }

    private static void ReleaseIsNotFabricated()
    {
        var surface=CreateSurface();
        Check(surface.Current.ReleaseText=="Release not evaluated" &&
              !surface.Current.ReleaseReady &&
              !surface.ReleaseReplay.ReleaseAvailable &&
              !surface.ReleaseReplay.ReleaseReady,
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
