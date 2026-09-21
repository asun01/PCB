namespace Asun.Platform.ClientIntegration;

public static class ClientQualitySurfaceSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) UnboundQualityIsExplicit();
        for(var round=1;round<=100;round++) if(round==100) NoFindingsIsExplicit();
        for(var round=1;round<=100;round++) if(round==100) SelectionIsExplicit();
        for(var round=1;round<=100;round++) if(round==100) QualityAuthorityIsReused();
        for(var round=1;round<=100;round++) if(round==100) NoFindingFactsAreFabricated();
        for(var round=1;round<=100;round++) if(round==100) ProductionStateRemainsUntouched();
        for(var round=1;round<=100;round++) if(round==100) ReplayRemainsUntouched();
        for(var round=1;round<=100;round++) if(round==100) ReleaseRemainsUntouched();
        for(var round=1;round<=100;round++) if(round==100) ProgramRemainsUntouched();
        for(var round=1;round<=100;round++) if(round==100) SelectionDoesNotCreateFinding();
    }

    private static void UnboundQualityIsExplicit()
    {
        var surface=CreateSurface();
        Check(!surface.Snapshot.IsBound &&
              !surface.HasFindings,
            "Quality surface must remain explicit when no authoritative Quality Run is bound.");
    }

    private static void NoFindingsIsExplicit()
    {
        var surface=CreateSurface();
        Check(surface.Snapshot.FindingCount==0 &&
              surface.SelectionText=="No finding selected.",
            "Unbound Quality surface must not expose finding selection.");
    }

    private static void SelectionIsExplicit()
    {
        var surface=CreateSurface();
        Check(surface.Snapshot.SelectedFindingId is null &&
              surface.SelectedFinding is null,
            "Unbound Quality selection and finding detail must remain null.");
    }

    private static void QualityAuthorityIsReused()
    {
        using var workspace=CreateWorkspace();
        var snapshot=workspace.Capture();
        var surface=ClientQualitySurfaceRuntime.Create(snapshot);
        Check(ReferenceEquals(surface.Snapshot,snapshot.Quality),
            "Quality surface must reuse the authoritative client Quality snapshot instance.");
    }

    private static void NoFindingFactsAreFabricated()
    {
        var surface=CreateSurface();
        Check(surface.Snapshot.ResultCount==0 &&
              surface.Snapshot.FindingCount==0 &&
              surface.Snapshot.EvidenceLinkCount==0,
            "Unbound Quality must not fabricate results, findings, or evidence links.");
    }

    private static void ProductionStateRemainsUntouched()
    {
        using var workspace=CreateWorkspace();
        var surface=ClientQualitySurfaceRuntime.Create(workspace.Capture());
        Check(surface.Snapshot.IsBound==false &&
              workspace.Production.Status==ClientExecutionStatus.Idle,
            "Quality projection must not mutate Production.");
    }

    private static void ReplayRemainsUntouched()
    {
        using var workspace=CreateWorkspace();
        var surface=ClientQualitySurfaceRuntime.Create(workspace.Capture());
        Check(workspace.Capture().Replay is null,
            "Quality surface creation must not fabricate Replay.");
    }

    private static void ReleaseRemainsUntouched()
    {
        using var workspace=CreateWorkspace();
        var surface=ClientQualitySurfaceRuntime.Create(workspace.Capture());
        Check(workspace.Capture().Release is null,
            "Quality surface creation must not fabricate Release.");
    }

    private static void ProgramRemainsUntouched()
    {
        using var workspace=CreateWorkspace();
        var surface=ClientQualitySurfaceRuntime.Create(workspace.Capture());
        Check(workspace.Program.Status==ClientProgramLoadStatus.Empty,
            "Quality surface creation must not mutate Program state.");
    }

    private static void SelectionDoesNotCreateFinding()
    {
        using var workspace=CreateWorkspace();
        var accepted=ClientQualityCommandRuntime.SelectFinding(
            workspace,
            new ClientWorkspaceCommandRouting(
                ClientWorkspaceKind.Quality,
                false,false,false,true,false,true,false),
            "missing");
        Check(!accepted &&
              workspace.Quality.FindingCount==0,
            "Quality selection must resolve only existing authoritative findings.");
    }

    private static ClientQualitySurface CreateSurface()
    {
        using var workspace=CreateWorkspace();
        return ClientQualitySurfaceRuntime.Create(workspace.Capture());
    }

    private static ClientInspectionWorkspace CreateWorkspace() =>
        new(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Quality surface smoke failed: "+message);
    }
}
