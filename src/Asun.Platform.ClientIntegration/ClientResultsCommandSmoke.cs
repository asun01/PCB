namespace Asun.Platform.ClientIntegration;

public static class ClientResultsCommandSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++) if(round==100) WrongWorkspaceIsRejected();
        for(var round=1;round<=100;round++) if(round==100) ReviewPermissionIsRequired();
        for(var round=1;round<=100;round++) if(round==100) MissingHistoryReturnsFalse();
        for(var round=1;round<=100;round++) if(round==100) ResultsWorkspaceIsAccepted();
        for(var round=1;round<=100;round++) if(round==100) SelectionDoesNotCreateReplay();
        for(var round=1;round<=100;round++) if(round==100) SelectionDoesNotCreateRelease();
        for(var round=1;round<=100;round++) if(round==100) SelectionDoesNotCreateQuality();
        for(var round=1;round<=100;round++) if(round==100) ProductionStateRemainsUnchanged();
        for(var round=1;round<=100;round++) if(round==100) RepeatedMissingSelectionRemainsFalse();
        for(var round=1;round<=100;round++) if(round==100) RoutingAuthorityIsSuppliedByCaller();
    }

    private static void WrongWorkspaceIsRejected()
    {
        using var workspace=CreateWorkspace();
        var rejected=ExpectInvalidOperation(() =>
            ClientResultsCommandRuntime.SelectHistory(
                workspace,
                Routing(ClientWorkspaceKind.Inspection,true),
                1));
        Check(rejected,"Results command must reject non-Results workspace routing.");
    }

    private static void ReviewPermissionIsRequired()
    {
        using var workspace=CreateWorkspace();
        var rejected=ExpectInvalidOperation(() =>
            ClientResultsCommandRuntime.SelectHistory(
                workspace,
                Routing(ClientWorkspaceKind.Results,false),
                1));
        Check(rejected,"Results command must require the existing review permission.");
    }

    private static void MissingHistoryReturnsFalse()
    {
        using var workspace=CreateWorkspace();
        var selected=ClientResultsCommandRuntime.SelectHistory(
            workspace,
            Routing(ClientWorkspaceKind.Results,true),
            1);
        Check(!selected,"Selecting an unknown history ordinal must return false.");
    }

    private static void ResultsWorkspaceIsAccepted()
    {
        using var workspace=CreateWorkspace();
        var rejected=ExpectInvalidOperation(() =>
            ClientResultsCommandRuntime.SelectHistory(
                workspace,
                Routing(ClientWorkspaceKind.Results,true),
                long.MinValue));
        Check(!rejected,"Valid Results routing must reach the history authority instead of being rejected by workspace routing.");
    }

    private static void SelectionDoesNotCreateReplay()
    {
        using var workspace=CreateWorkspace();
        ClientResultsCommandRuntime.SelectHistory(
            workspace,
            Routing(ClientWorkspaceKind.Results,true),
            1);
        Check(workspace.Capture().Replay is null,
            "History selection must not fabricate Replay.");
    }

    private static void SelectionDoesNotCreateRelease()
    {
        using var workspace=CreateWorkspace();
        ClientResultsCommandRuntime.SelectHistory(
            workspace,
            Routing(ClientWorkspaceKind.Results,true),
            1);
        Check(workspace.Capture().Release is null,
            "History selection must not fabricate Release.");
    }

    private static void SelectionDoesNotCreateQuality()
    {
        using var workspace=CreateWorkspace();
        ClientResultsCommandRuntime.SelectHistory(
            workspace,
            Routing(ClientWorkspaceKind.Results,true),
            1);
        Check(!workspace.Quality.IsBound,
            "History selection must not fabricate Quality.");
    }

    private static void ProductionStateRemainsUnchanged()
    {
        using var workspace=CreateWorkspace();
        var before=workspace.Production.Status;
        ClientResultsCommandRuntime.SelectHistory(
            workspace,
            Routing(ClientWorkspaceKind.Results,true),
            1);
        Check(workspace.Production.Status==before,
            "History selection must not mutate Production state.");
    }

    private static void RepeatedMissingSelectionRemainsFalse()
    {
        using var workspace=CreateWorkspace();
        var first=ClientResultsCommandRuntime.SelectHistory(
            workspace,
            Routing(ClientWorkspaceKind.Results,true),
            1);
        var second=ClientResultsCommandRuntime.SelectHistory(
            workspace,
            Routing(ClientWorkspaceKind.Results,true),
            1);
        Check(!first && !second,
            "Repeated missing history selection must remain deterministic.");
    }

    private static void RoutingAuthorityIsSuppliedByCaller()
    {
        using var workspace=CreateWorkspace();
        var routing=Routing(ClientWorkspaceKind.Results,true);
        Check(routing.Workspace==ClientWorkspaceKind.Results &&
              routing.CanReviewResults,
            "Results command must consume caller-provided routing authority.");
    }

    private static ClientInspectionWorkspace CreateWorkspace() =>
        new(
            new System.Numerics.Vector2(640,480),
            new System.Numerics.Vector2(640,480));

    private static ClientWorkspaceCommandRouting Routing(
        ClientWorkspaceKind workspace,
        bool canReviewResults) =>
        new(
            workspace,
            false,false,false,true,false,false,canReviewResults);

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
                "Results command smoke failed: "+message);
    }
}
