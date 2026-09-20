using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientProductionWorkspaceEvent3HundredStageSmoke
{
    public static async Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var definition=ClientWorkspaceSmokeFixture.CreateDefinition(frameCount:2);
            var runner=new ProgressAwareProductionRunner();
            var workspace=new ClientProductionWorkspace(runner);
            var last=ClientExecutionStatus.Idle;
            workspace.Changed+=snapshot=>last=snapshot.Status;
            workspace.Load(definition);
            var execution=workspace.StartAsync(new NeverFrameSource());
            await runner.ProgressReached.WaitAsync(TimeSpan.FromSeconds(2));
            Check(last==ClientExecutionStatus.Running,
                  "client Production workspace event stream must reach Running state");
            runner.Release();
            await execution;
            Check(last==ClientExecutionStatus.Completed,
                  "client Production workspace event stream must reach Completed state");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var definition=ClientWorkspaceSmokeFixture.CreateDefinition(frameCount:2);
            var runner=new ProgressAwareProductionRunner();
            var workspace=new ClientProductionWorkspace(runner);
            var last=ClientExecutionStatus.Idle;
            workspace.Changed+=snapshot=>last=snapshot.Status;
            workspace.Load(definition);
            var execution=workspace.StartAsync(new NeverFrameSource());
            await runner.ProgressReached.WaitAsync(TimeSpan.FromSeconds(2));
            Check(last==ClientExecutionStatus.Running,
                  "client Production workspace event stream must reach Running state");
            runner.Release();
            await execution;
            Check(last==ClientExecutionStatus.Completed,
                  "client Production workspace event stream must reach Completed state");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var definition=ClientWorkspaceSmokeFixture.CreateDefinition(frameCount:2);
            var runner=new ProgressAwareProductionRunner();
            var workspace=new ClientProductionWorkspace(runner);
            var last=ClientExecutionStatus.Idle;
            workspace.Changed+=snapshot=>last=snapshot.Status;
            workspace.Load(definition);
            var execution=workspace.StartAsync(new NeverFrameSource());
            await runner.ProgressReached.WaitAsync(TimeSpan.FromSeconds(2));
            Check(last==ClientExecutionStatus.Running,
                  "client Production workspace event stream must reach Running state");
            runner.Release();
            await execution;
            Check(last==ClientExecutionStatus.Completed,
                  "client Production workspace event stream must reach Completed state");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var definition=ClientWorkspaceSmokeFixture.CreateDefinition(frameCount:2);
            var runner=new ProgressAwareProductionRunner();
            var workspace=new ClientProductionWorkspace(runner);
            var last=ClientExecutionStatus.Idle;
            workspace.Changed+=snapshot=>last=snapshot.Status;
            workspace.Load(definition);
            var execution=workspace.StartAsync(new NeverFrameSource());
            await runner.ProgressReached.WaitAsync(TimeSpan.FromSeconds(2));
            Check(last==ClientExecutionStatus.Running,
                  "client Production workspace event stream must reach Running state");
            runner.Release();
            await execution;
            Check(last==ClientExecutionStatus.Completed,
                  "client Production workspace event stream must reach Completed state");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var definition=ClientWorkspaceSmokeFixture.CreateDefinition(frameCount:2);
            var runner=new ProgressAwareProductionRunner();
            var workspace=new ClientProductionWorkspace(runner);
            var last=ClientExecutionStatus.Idle;
            workspace.Changed+=snapshot=>last=snapshot.Status;
            workspace.Load(definition);
            var execution=workspace.StartAsync(new NeverFrameSource());
            await runner.ProgressReached.WaitAsync(TimeSpan.FromSeconds(2));
            Check(last==ClientExecutionStatus.Running,
                  "client Production workspace event stream must reach Running state");
            runner.Release();
            await execution;
            Check(last==ClientExecutionStatus.Completed,
                  "client Production workspace event stream must reach Completed state");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var definition=ClientWorkspaceSmokeFixture.CreateDefinition(frameCount:2);
            var runner=new ProgressAwareProductionRunner();
            var workspace=new ClientProductionWorkspace(runner);
            var last=ClientExecutionStatus.Idle;
            workspace.Changed+=snapshot=>last=snapshot.Status;
            workspace.Load(definition);
            var execution=workspace.StartAsync(new NeverFrameSource());
            await runner.ProgressReached.WaitAsync(TimeSpan.FromSeconds(2));
            Check(last==ClientExecutionStatus.Running,
                  "client Production workspace event stream must reach Running state");
            runner.Release();
            await execution;
            Check(last==ClientExecutionStatus.Completed,
                  "client Production workspace event stream must reach Completed state");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var definition=ClientWorkspaceSmokeFixture.CreateDefinition(frameCount:2);
            var runner=new ProgressAwareProductionRunner();
            var workspace=new ClientProductionWorkspace(runner);
            var last=ClientExecutionStatus.Idle;
            workspace.Changed+=snapshot=>last=snapshot.Status;
            workspace.Load(definition);
            var execution=workspace.StartAsync(new NeverFrameSource());
            await runner.ProgressReached.WaitAsync(TimeSpan.FromSeconds(2));
            Check(last==ClientExecutionStatus.Running,
                  "client Production workspace event stream must reach Running state");
            runner.Release();
            await execution;
            Check(last==ClientExecutionStatus.Completed,
                  "client Production workspace event stream must reach Completed state");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var definition=ClientWorkspaceSmokeFixture.CreateDefinition(frameCount:2);
            var runner=new ProgressAwareProductionRunner();
            var workspace=new ClientProductionWorkspace(runner);
            var last=ClientExecutionStatus.Idle;
            workspace.Changed+=snapshot=>last=snapshot.Status;
            workspace.Load(definition);
            var execution=workspace.StartAsync(new NeverFrameSource());
            await runner.ProgressReached.WaitAsync(TimeSpan.FromSeconds(2));
            Check(last==ClientExecutionStatus.Running,
                  "client Production workspace event stream must reach Running state");
            runner.Release();
            await execution;
            Check(last==ClientExecutionStatus.Completed,
                  "client Production workspace event stream must reach Completed state");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var definition=ClientWorkspaceSmokeFixture.CreateDefinition(frameCount:2);
            var runner=new ProgressAwareProductionRunner();
            var workspace=new ClientProductionWorkspace(runner);
            var last=ClientExecutionStatus.Idle;
            workspace.Changed+=snapshot=>last=snapshot.Status;
            workspace.Load(definition);
            var execution=workspace.StartAsync(new NeverFrameSource());
            await runner.ProgressReached.WaitAsync(TimeSpan.FromSeconds(2));
            Check(last==ClientExecutionStatus.Running,
                  "client Production workspace event stream must reach Running state");
            runner.Release();
            await execution;
            Check(last==ClientExecutionStatus.Completed,
                  "client Production workspace event stream must reach Completed state");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var definition=ClientWorkspaceSmokeFixture.CreateDefinition(frameCount:2);
            var runner=new ProgressAwareProductionRunner();
            var workspace=new ClientProductionWorkspace(runner);
            var last=ClientExecutionStatus.Idle;
            workspace.Changed+=snapshot=>last=snapshot.Status;
            workspace.Load(definition);
            var execution=workspace.StartAsync(new NeverFrameSource());
            await runner.ProgressReached.WaitAsync(TimeSpan.FromSeconds(2));
            Check(last==ClientExecutionStatus.Running,
                  "client Production workspace event stream must reach Running state");
            runner.Release();
            await execution;
            Check(last==ClientExecutionStatus.Completed,
                  "client Production workspace event stream must reach Completed state");
        }
        if(round==100)
            return;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
