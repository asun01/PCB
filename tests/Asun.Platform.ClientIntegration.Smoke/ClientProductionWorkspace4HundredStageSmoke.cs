using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientProductionWorkspace4HundredStageSmoke
{
    public static async Task RunAsync(Action<bool,string> Check)
    {
        var round=0;

        for(var i0=0;i0<10;i0++)
        {
            round++;
                var first=ClientWorkspaceSmokeFixture.CreateDefinition();
                var second=ClientWorkspaceSmokeFixture.CreateDefinition(
                    Guid.Parse("72000000-0000-0000-0000-000000000202"));
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                workspace.Load(first);
                workspace.Load(second);
                Check(workspace.Snapshot.ActiveSessionId==second.SessionId &&
                      workspace.Snapshot.ProgramId==second.ProgramPlan.ProgramId &&
                      workspace.Snapshot.Status==ClientExecutionStatus.Ready,
                      "loading a new session should replace the previous client projection before execution");
        }
        for(var i1=0;i1<10;i1++)
        {
            round++;
                var first=ClientWorkspaceSmokeFixture.CreateDefinition();
                var second=ClientWorkspaceSmokeFixture.CreateDefinition(
                    Guid.Parse("72000000-0000-0000-0000-000000000202"));
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                workspace.Load(first);
                workspace.Load(second);
                Check(workspace.Snapshot.ActiveSessionId==second.SessionId &&
                      workspace.Snapshot.ProgramId==second.ProgramPlan.ProgramId &&
                      workspace.Snapshot.Status==ClientExecutionStatus.Ready,
                      "loading a new session should replace the previous client projection before execution");
        }
        for(var i2=0;i2<10;i2++)
        {
            round++;
                var first=ClientWorkspaceSmokeFixture.CreateDefinition();
                var second=ClientWorkspaceSmokeFixture.CreateDefinition(
                    Guid.Parse("72000000-0000-0000-0000-000000000202"));
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                workspace.Load(first);
                workspace.Load(second);
                Check(workspace.Snapshot.ActiveSessionId==second.SessionId &&
                      workspace.Snapshot.ProgramId==second.ProgramPlan.ProgramId &&
                      workspace.Snapshot.Status==ClientExecutionStatus.Ready,
                      "loading a new session should replace the previous client projection before execution");
        }
        for(var i3=0;i3<10;i3++)
        {
            round++;
                var first=ClientWorkspaceSmokeFixture.CreateDefinition();
                var second=ClientWorkspaceSmokeFixture.CreateDefinition(
                    Guid.Parse("72000000-0000-0000-0000-000000000202"));
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                workspace.Load(first);
                workspace.Load(second);
                Check(workspace.Snapshot.ActiveSessionId==second.SessionId &&
                      workspace.Snapshot.ProgramId==second.ProgramPlan.ProgramId &&
                      workspace.Snapshot.Status==ClientExecutionStatus.Ready,
                      "loading a new session should replace the previous client projection before execution");
        }
        for(var i4=0;i4<10;i4++)
        {
            round++;
                var first=ClientWorkspaceSmokeFixture.CreateDefinition();
                var second=ClientWorkspaceSmokeFixture.CreateDefinition(
                    Guid.Parse("72000000-0000-0000-0000-000000000202"));
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                workspace.Load(first);
                workspace.Load(second);
                Check(workspace.Snapshot.ActiveSessionId==second.SessionId &&
                      workspace.Snapshot.ProgramId==second.ProgramPlan.ProgramId &&
                      workspace.Snapshot.Status==ClientExecutionStatus.Ready,
                      "loading a new session should replace the previous client projection before execution");
        }
        for(var i5=0;i5<10;i5++)
        {
            round++;
                var first=ClientWorkspaceSmokeFixture.CreateDefinition();
                var second=ClientWorkspaceSmokeFixture.CreateDefinition(
                    Guid.Parse("72000000-0000-0000-0000-000000000202"));
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                workspace.Load(first);
                workspace.Load(second);
                Check(workspace.Snapshot.ActiveSessionId==second.SessionId &&
                      workspace.Snapshot.ProgramId==second.ProgramPlan.ProgramId &&
                      workspace.Snapshot.Status==ClientExecutionStatus.Ready,
                      "loading a new session should replace the previous client projection before execution");
        }
        for(var i6=0;i6<10;i6++)
        {
            round++;
                var first=ClientWorkspaceSmokeFixture.CreateDefinition();
                var second=ClientWorkspaceSmokeFixture.CreateDefinition(
                    Guid.Parse("72000000-0000-0000-0000-000000000202"));
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                workspace.Load(first);
                workspace.Load(second);
                Check(workspace.Snapshot.ActiveSessionId==second.SessionId &&
                      workspace.Snapshot.ProgramId==second.ProgramPlan.ProgramId &&
                      workspace.Snapshot.Status==ClientExecutionStatus.Ready,
                      "loading a new session should replace the previous client projection before execution");
        }
        for(var i7=0;i7<10;i7++)
        {
            round++;
                var first=ClientWorkspaceSmokeFixture.CreateDefinition();
                var second=ClientWorkspaceSmokeFixture.CreateDefinition(
                    Guid.Parse("72000000-0000-0000-0000-000000000202"));
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                workspace.Load(first);
                workspace.Load(second);
                Check(workspace.Snapshot.ActiveSessionId==second.SessionId &&
                      workspace.Snapshot.ProgramId==second.ProgramPlan.ProgramId &&
                      workspace.Snapshot.Status==ClientExecutionStatus.Ready,
                      "loading a new session should replace the previous client projection before execution");
        }
        for(var i8=0;i8<10;i8++)
        {
            round++;
                var first=ClientWorkspaceSmokeFixture.CreateDefinition();
                var second=ClientWorkspaceSmokeFixture.CreateDefinition(
                    Guid.Parse("72000000-0000-0000-0000-000000000202"));
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                workspace.Load(first);
                workspace.Load(second);
                Check(workspace.Snapshot.ActiveSessionId==second.SessionId &&
                      workspace.Snapshot.ProgramId==second.ProgramPlan.ProgramId &&
                      workspace.Snapshot.Status==ClientExecutionStatus.Ready,
                      "loading a new session should replace the previous client projection before execution");
        }
        for(var i9=0;i9<10;i9++)
        {
            round++;
                var first=ClientWorkspaceSmokeFixture.CreateDefinition();
                var second=ClientWorkspaceSmokeFixture.CreateDefinition(
                    Guid.Parse("72000000-0000-0000-0000-000000000202"));
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                workspace.Load(first);
                workspace.Load(second);
                Check(workspace.Snapshot.ActiveSessionId==second.SessionId &&
                      workspace.Snapshot.ProgramId==second.ProgramPlan.ProgramId &&
                      workspace.Snapshot.Status==ClientExecutionStatus.Ready,
                      "loading a new session should replace the previous client projection before execution");
        }

        if(round==100)
            return;

        throw new InvalidOperationException("client workspace smoke must execute exactly 100 rounds");
    }
}
