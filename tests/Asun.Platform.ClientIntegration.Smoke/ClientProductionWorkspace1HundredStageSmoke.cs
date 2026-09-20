using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientProductionWorkspace1HundredStageSmoke
{
    public static async Task RunAsync(Action<bool,string> Check)
    {
        var round=0;

        for(var i0=0;i0<10;i0++)
        {
            round++;
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                workspace.Load(definition);
                Check(workspace.Snapshot.Status==ClientExecutionStatus.Ready &&
                      workspace.Snapshot.ProgramId==definition.ProgramPlan.ProgramId &&
                      workspace.Snapshot.ProgramVersion==definition.ProgramPlan.Version &&
                      workspace.Snapshot.ActiveSessionId==definition.SessionId,
                      "loaded production session should produce a ready client workspace snapshot");
        }
        for(var i1=0;i1<10;i1++)
        {
            round++;
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                workspace.Load(definition);
                Check(workspace.Snapshot.Status==ClientExecutionStatus.Ready &&
                      workspace.Snapshot.ProgramId==definition.ProgramPlan.ProgramId &&
                      workspace.Snapshot.ProgramVersion==definition.ProgramPlan.Version &&
                      workspace.Snapshot.ActiveSessionId==definition.SessionId,
                      "loaded production session should produce a ready client workspace snapshot");
        }
        for(var i2=0;i2<10;i2++)
        {
            round++;
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                workspace.Load(definition);
                Check(workspace.Snapshot.Status==ClientExecutionStatus.Ready &&
                      workspace.Snapshot.ProgramId==definition.ProgramPlan.ProgramId &&
                      workspace.Snapshot.ProgramVersion==definition.ProgramPlan.Version &&
                      workspace.Snapshot.ActiveSessionId==definition.SessionId,
                      "loaded production session should produce a ready client workspace snapshot");
        }
        for(var i3=0;i3<10;i3++)
        {
            round++;
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                workspace.Load(definition);
                Check(workspace.Snapshot.Status==ClientExecutionStatus.Ready &&
                      workspace.Snapshot.ProgramId==definition.ProgramPlan.ProgramId &&
                      workspace.Snapshot.ProgramVersion==definition.ProgramPlan.Version &&
                      workspace.Snapshot.ActiveSessionId==definition.SessionId,
                      "loaded production session should produce a ready client workspace snapshot");
        }
        for(var i4=0;i4<10;i4++)
        {
            round++;
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                workspace.Load(definition);
                Check(workspace.Snapshot.Status==ClientExecutionStatus.Ready &&
                      workspace.Snapshot.ProgramId==definition.ProgramPlan.ProgramId &&
                      workspace.Snapshot.ProgramVersion==definition.ProgramPlan.Version &&
                      workspace.Snapshot.ActiveSessionId==definition.SessionId,
                      "loaded production session should produce a ready client workspace snapshot");
        }
        for(var i5=0;i5<10;i5++)
        {
            round++;
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                workspace.Load(definition);
                Check(workspace.Snapshot.Status==ClientExecutionStatus.Ready &&
                      workspace.Snapshot.ProgramId==definition.ProgramPlan.ProgramId &&
                      workspace.Snapshot.ProgramVersion==definition.ProgramPlan.Version &&
                      workspace.Snapshot.ActiveSessionId==definition.SessionId,
                      "loaded production session should produce a ready client workspace snapshot");
        }
        for(var i6=0;i6<10;i6++)
        {
            round++;
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                workspace.Load(definition);
                Check(workspace.Snapshot.Status==ClientExecutionStatus.Ready &&
                      workspace.Snapshot.ProgramId==definition.ProgramPlan.ProgramId &&
                      workspace.Snapshot.ProgramVersion==definition.ProgramPlan.Version &&
                      workspace.Snapshot.ActiveSessionId==definition.SessionId,
                      "loaded production session should produce a ready client workspace snapshot");
        }
        for(var i7=0;i7<10;i7++)
        {
            round++;
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                workspace.Load(definition);
                Check(workspace.Snapshot.Status==ClientExecutionStatus.Ready &&
                      workspace.Snapshot.ProgramId==definition.ProgramPlan.ProgramId &&
                      workspace.Snapshot.ProgramVersion==definition.ProgramPlan.Version &&
                      workspace.Snapshot.ActiveSessionId==definition.SessionId,
                      "loaded production session should produce a ready client workspace snapshot");
        }
        for(var i8=0;i8<10;i8++)
        {
            round++;
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                workspace.Load(definition);
                Check(workspace.Snapshot.Status==ClientExecutionStatus.Ready &&
                      workspace.Snapshot.ProgramId==definition.ProgramPlan.ProgramId &&
                      workspace.Snapshot.ProgramVersion==definition.ProgramPlan.Version &&
                      workspace.Snapshot.ActiveSessionId==definition.SessionId,
                      "loaded production session should produce a ready client workspace snapshot");
        }
        for(var i9=0;i9<10;i9++)
        {
            round++;
                var definition=ClientWorkspaceSmokeFixture.CreateDefinition();
                var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
                workspace.Load(definition);
                Check(workspace.Snapshot.Status==ClientExecutionStatus.Ready &&
                      workspace.Snapshot.ProgramId==definition.ProgramPlan.ProgramId &&
                      workspace.Snapshot.ProgramVersion==definition.ProgramPlan.Version &&
                      workspace.Snapshot.ActiveSessionId==definition.SessionId,
                      "loaded production session should produce a ready client workspace snapshot");
        }

        if(round==100)
            return;

        throw new InvalidOperationException("client workspace smoke must execute exactly 100 rounds");
    }
}
