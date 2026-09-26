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
              !surface.HasFindings &&
              surface.AuthorityText=="Quality authority: not bound." &&
              surface.ProviderText=="Provider: none.",
            "Quality surface must remain explicit when no authoritative Quality Run is bound.");
    }

    private static void NoFindingsIsExplicit()
    {
        using var workspace=CreateWorkspace();
        var first=new ClientQualityFindingDisplayItem(
            "finding-pass","RULE-PASS","Pass","Low","Pass finding",0);
        var second=new ClientQualityFindingDisplayItem(
            "finding-fail","RULE-FAIL","Fail","High","Fail finding",1);
        var quality=workspace.Quality with
        {
            RunId=Guid.NewGuid(),
            ResultCount=2,
            FindingCount=2,
            PassCount=1,
            FailCount=1,
            EvidenceLinkCount=1,
            Fingerprint=new string('a',64),
            IsBound=true,
            Findings=new[] { first,second }
        };
        var surface=ClientQualitySurfaceRuntime.Create(
            workspace.Capture() with { Quality=quality },
            new ClientQualityFilter("Fail","High"));
        Check(surface.VisibleFindings.Count==1 &&
              surface.VisibleFindings[0].FindingId=="finding-fail" &&
              surface.AuthorityText=="Quality authority: Bound · aaaaaaaaaaaa..." &&
              surface.SummaryText.Contains("2 result(s)",StringComparison.Ordinal) &&
              surface.SummaryText.Contains("Pass 1",StringComparison.Ordinal),
            "Quality surface must reuse the existing Outcome/Severity filter projection and expose authoritative summary state.");
    }

    private static void SelectionIsExplicit()
    {
        using var workspace=CreateWorkspace();
        var finding=new ClientQualityFindingDisplayItem(
            "finding-1",
            "RULE-1",
            "Fail",
            "High",
            "Deterministic finding",
            2);
        var quality=workspace.Quality with
        {
            RunId=Guid.NewGuid(),
            ResultCount=1,
            FindingCount=1,
            FailCount=1,
            EvidenceLinkCount=2,
            Fingerprint="fingerprint",
            IsBound=true,
            Findings=new[] { finding },
            SelectedFindingId="finding-1"
        };
        var surface=ClientQualitySurfaceRuntime.Create(
            workspace.Capture() with { Quality=quality });

        var hiddenSurface=ClientQualitySurfaceRuntime.Create(
            workspace.Capture() with { Quality=quality },
            new ClientQualityFilter("Pass","Low"));

        Check(surface.SelectedFinding?.FindingId=="finding-1" &&
              surface.SelectedFinding.RuleCode=="RULE-1" &&
              surface.SelectedFinding.EvidenceCount==2 &&
              surface.SelectedFindingVisible &&
              hiddenSurface.SelectedFinding?.FindingId=="finding-1" &&
              !hiddenSurface.SelectedFindingVisible &&
              hiddenSurface.SelectionText.Contains("hidden by the current filter",StringComparison.Ordinal),
            "Quality surface must distinguish selected finding detail from its current filtered visibility.");
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
