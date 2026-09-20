using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientProductionWorkspaceEvent1HundredStageSmoke
{
    public static async Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var definition=ClientWorkspaceSmokeFixture.CreateDefinition(frameCount:3);
            var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var states=new List<ClientExecutionStatus>();
            workspace.Changed+=snapshot=>states.Add(snapshot.Status);
            workspace.Load(definition);
            Check(states.Count==1 &&
                  states[0]==ClientExecutionStatus.Ready &&
                  workspace.Snapshot.TargetFrameCount==3,
                  "client Production workspace must publish Ready state when a session is loaded");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var definition=ClientWorkspaceSmokeFixture.CreateDefinition(frameCount:3);
            var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var states=new List<ClientExecutionStatus>();
            workspace.Changed+=snapshot=>states.Add(snapshot.Status);
            workspace.Load(definition);
            Check(states.Count==1 &&
                  states[0]==ClientExecutionStatus.Ready &&
                  workspace.Snapshot.TargetFrameCount==3,
                  "client Production workspace must publish Ready state when a session is loaded");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var definition=ClientWorkspaceSmokeFixture.CreateDefinition(frameCount:3);
            var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var states=new List<ClientExecutionStatus>();
            workspace.Changed+=snapshot=>states.Add(snapshot.Status);
            workspace.Load(definition);
            Check(states.Count==1 &&
                  states[0]==ClientExecutionStatus.Ready &&
                  workspace.Snapshot.TargetFrameCount==3,
                  "client Production workspace must publish Ready state when a session is loaded");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var definition=ClientWorkspaceSmokeFixture.CreateDefinition(frameCount:3);
            var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var states=new List<ClientExecutionStatus>();
            workspace.Changed+=snapshot=>states.Add(snapshot.Status);
            workspace.Load(definition);
            Check(states.Count==1 &&
                  states[0]==ClientExecutionStatus.Ready &&
                  workspace.Snapshot.TargetFrameCount==3,
                  "client Production workspace must publish Ready state when a session is loaded");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var definition=ClientWorkspaceSmokeFixture.CreateDefinition(frameCount:3);
            var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var states=new List<ClientExecutionStatus>();
            workspace.Changed+=snapshot=>states.Add(snapshot.Status);
            workspace.Load(definition);
            Check(states.Count==1 &&
                  states[0]==ClientExecutionStatus.Ready &&
                  workspace.Snapshot.TargetFrameCount==3,
                  "client Production workspace must publish Ready state when a session is loaded");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var definition=ClientWorkspaceSmokeFixture.CreateDefinition(frameCount:3);
            var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var states=new List<ClientExecutionStatus>();
            workspace.Changed+=snapshot=>states.Add(snapshot.Status);
            workspace.Load(definition);
            Check(states.Count==1 &&
                  states[0]==ClientExecutionStatus.Ready &&
                  workspace.Snapshot.TargetFrameCount==3,
                  "client Production workspace must publish Ready state when a session is loaded");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var definition=ClientWorkspaceSmokeFixture.CreateDefinition(frameCount:3);
            var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var states=new List<ClientExecutionStatus>();
            workspace.Changed+=snapshot=>states.Add(snapshot.Status);
            workspace.Load(definition);
            Check(states.Count==1 &&
                  states[0]==ClientExecutionStatus.Ready &&
                  workspace.Snapshot.TargetFrameCount==3,
                  "client Production workspace must publish Ready state when a session is loaded");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var definition=ClientWorkspaceSmokeFixture.CreateDefinition(frameCount:3);
            var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var states=new List<ClientExecutionStatus>();
            workspace.Changed+=snapshot=>states.Add(snapshot.Status);
            workspace.Load(definition);
            Check(states.Count==1 &&
                  states[0]==ClientExecutionStatus.Ready &&
                  workspace.Snapshot.TargetFrameCount==3,
                  "client Production workspace must publish Ready state when a session is loaded");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var definition=ClientWorkspaceSmokeFixture.CreateDefinition(frameCount:3);
            var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var states=new List<ClientExecutionStatus>();
            workspace.Changed+=snapshot=>states.Add(snapshot.Status);
            workspace.Load(definition);
            Check(states.Count==1 &&
                  states[0]==ClientExecutionStatus.Ready &&
                  workspace.Snapshot.TargetFrameCount==3,
                  "client Production workspace must publish Ready state when a session is loaded");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var definition=ClientWorkspaceSmokeFixture.CreateDefinition(frameCount:3);
            var workspace=ClientWorkspaceSmokeFixture.CreateWorkspace();
            var states=new List<ClientExecutionStatus>();
            workspace.Changed+=snapshot=>states.Add(snapshot.Status);
            workspace.Load(definition);
            Check(states.Count==1 &&
                  states[0]==ClientExecutionStatus.Ready &&
                  workspace.Snapshot.TargetFrameCount==3,
                  "client Production workspace must publish Ready state when a session is loaded");
        }
        if(round==100)
            return;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
