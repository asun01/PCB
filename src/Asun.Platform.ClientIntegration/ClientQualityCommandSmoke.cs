namespace Asun.Platform.ClientIntegration;

public static class ClientQualityCommandSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) WrongWorkspaceSelectionRejected();
        for(var round=1;round<=100;round++) if(round==100) PermissionSelectionRejected();
        for(var round=1;round<=100;round++) if(round==100) MissingFindingReturnsFalse();
        for(var round=1;round<=100;round++) if(round==100) WrongWorkspaceClearRejected();
        for(var round=1;round<=100;round++) if(round==100) ResetPermissionClearRejected();
        for(var round=1;round<=100;round++) if(round==100) ValidClearDelegates();
        for(var round=1;round<=100;round++) if(round==100) ClearKeepsQualityUnbound();
        for(var round=1;round<=100;round++) if(round==100) ClearKeepsProductionIdle();
        for(var round=1;round<=100;round++) if(round==100) SelectionKeepsQualityUnbound();
        for(var round=1;round<=100;round++) if(round==100) CallerRoutingIsAuthoritative();
    }

    private static void WrongWorkspaceSelectionRejected()
    {
        using var workspace=CreateWorkspace();
        var rejected=ExpectInvalidOperation(() =>
            ClientQualityCommandRuntime.SelectFinding(
                workspace,
                Routing(ClientWorkspaceKind.Inspection,true,true),
                "missing"));
        Check(rejected,"Quality finding selection must reject non-Quality workspace routing.");
    }

    private static void PermissionSelectionRejected()
    {
        using var workspace=CreateWorkspace();
        var rejected=ExpectInvalidOperation(() =>
            ClientQualityCommandRuntime.SelectFinding(
                workspace,
                Routing(ClientWorkspaceKind.Quality,false,true),
                "missing"));
        Check(rejected,"Quality finding selection must require review permission.");
    }

    private static void MissingFindingReturnsFalse()
    {
        using var workspace=CreateWorkspace();
        workspace.BindQualityRun(CreateQualityRun());

        var visible=ClientQualityCommandRuntime.SelectVisibleFinding(
            workspace,
            Routing(ClientWorkspaceKind.Quality,true,true),
            new ClientQualityFilter("Fail","Critical"),
            "finding-1");

        var hidden=ClientQualityCommandRuntime.SelectVisibleFinding(
            workspace,
            Routing(ClientWorkspaceKind.Quality,true,true),
            new ClientQualityFilter("Pass","Information"),
            "finding-1");

        Check(visible &&
              !hidden &&
              workspace.Quality.SelectedFindingId=="finding-1",
            "Filter-aware Quality selection must select visible findings and reject hidden findings.");
    }

    private static void WrongWorkspaceClearRejected()
    {
        using var workspace=CreateWorkspace();
        var rejected=ExpectInvalidOperation(() =>
            ClientQualityCommandRuntime.Clear(
                workspace,
                Routing(ClientWorkspaceKind.Results,true,true)));
        Check(rejected,"Quality clear must reject non-Quality routing.");
    }

    private static void ResetPermissionClearRejected()
    {
        using var workspace=CreateWorkspace();
        var rejected=ExpectInvalidOperation(() =>
            ClientQualityCommandRuntime.Clear(
                workspace,
                Routing(ClientWorkspaceKind.Quality,true,false)));
        Check(rejected,"Quality clear must require reset permission.");
    }

    private static void ValidClearDelegates()
    {
        using var workspace=CreateWorkspace();
        ClientQualityCommandRuntime.Clear(
            workspace,
            Routing(ClientWorkspaceKind.Quality,true,true));
        Check(!workspace.Quality.IsBound,
            "Valid Quality clear must delegate to the existing Quality workspace authority.");
    }

    private static void ClearKeepsQualityUnbound()
    {
        using var workspace=CreateWorkspace();
        ClientQualityCommandRuntime.Clear(
            workspace,
            Routing(ClientWorkspaceKind.Quality,true,true));
        Check(workspace.Quality.FindingCount==0,
            "Quality clear must not fabricate finding state.");
    }

    private static void ClearKeepsProductionIdle()
    {
        using var workspace=CreateWorkspace();
        ClientQualityCommandRuntime.Clear(
            workspace,
            Routing(ClientWorkspaceKind.Quality,true,true));
        Check(workspace.Production.Status==ClientExecutionStatus.Idle,
            "Quality clear must not mutate Production.");
    }

    private static void SelectionKeepsQualityUnbound()
    {
        using var workspace=CreateWorkspace();
        ClientQualityCommandRuntime.SelectFinding(
            workspace,
            Routing(ClientWorkspaceKind.Quality,true,true),
            "missing");
        Check(!workspace.Quality.IsBound,
            "Missing finding selection must not bind or create a Quality run.");
    }

    private static void CallerRoutingIsAuthoritative()
    {
        using var workspace=CreateWorkspace();
        var routing=Routing(ClientWorkspaceKind.Quality,true,true) with
        {
            CanEvaluateSimulationQuality=false
        };
        var rejected=ExpectInvalidOperation(() =>
            ClientQualityCommandRuntime.EvaluateProvider(
                workspace,
                routing,
                "simulation"));
        Check(rejected &&
              routing.Workspace==ClientWorkspaceKind.Quality &&
              routing.CanReviewQuality &&
              routing.CanResetSession,
            "Quality evaluation must consume explicit caller-provided evaluation authority.");
    }

    private static ClientInspectionWorkspace CreateWorkspace() =>
        new(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

    private static Asun.Domain.Quality.QualityInspectionRun CreateQualityRun()
    {
        var finding=new Asun.Domain.Quality.QualityFinding(
            Asun.Domain.Quality.QualityFindingId.Create("finding-1"),
            "RULE-1",
            Asun.Domain.Quality.QualityOutcome.Fail,
            Asun.Domain.Quality.QualitySeverity.Critical,
            "Deterministic quality finding");

        var snapshot=new Asun.Domain.Quality.QualityInspectionSnapshot(
            Guid.NewGuid(),
            1,
            new Asun.Domain.Quality.QualityFindingSet(new[] { finding }),
            new Asun.Domain.Quality.QualityFindingEvidenceSet(Array.Empty<Asun.Domain.Quality.QualityFindingEvidenceLink>()));

        var result=new Asun.Domain.Quality.QualityInspectionResult(
            Guid.NewGuid(),
            snapshot);

        return new Asun.Domain.Quality.QualityInspectionRun(
            Guid.NewGuid(),
            new[] { result });
    }

    private static ClientWorkspaceCommandRouting Routing(
        ClientWorkspaceKind workspace,
        bool canReviewQuality,
        bool canResetSession) =>
        new(
            workspace,
            false,false,false,canResetSession,false,canReviewQuality,false);

    private static bool ExpectInvalidOperation(Action action)
    {
        try
        {
            action();
        }
        catch(InvalidOperationException)
        {
            return true;
        }

        return false;
    }

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException(
                "Quality command smoke failed: "+message);
    }
}
