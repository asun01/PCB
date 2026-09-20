using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientWorkspaceLifecycle4HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var runtime=new ClientWorkspaceRuntime();
            var first=runtime.Current;
            runtime.TryNavigate(ClientWorkspaceKind.Results);
            var second=runtime.Current;
            runtime.Dispose();
            Check(first.TransitionSequence==0 &&
                  second.TransitionSequence==1 &&
                  second.Workspace==ClientWorkspaceKind.Results,
                  "workspace transition sequence must remain monotonic before disposal");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var runtime=new ClientWorkspaceRuntime();
            var first=runtime.Current;
            runtime.TryNavigate(ClientWorkspaceKind.Results);
            var second=runtime.Current;
            runtime.Dispose();
            Check(first.TransitionSequence==0 &&
                  second.TransitionSequence==1 &&
                  second.Workspace==ClientWorkspaceKind.Results,
                  "workspace transition sequence must remain monotonic before disposal");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var runtime=new ClientWorkspaceRuntime();
            var first=runtime.Current;
            runtime.TryNavigate(ClientWorkspaceKind.Results);
            var second=runtime.Current;
            runtime.Dispose();
            Check(first.TransitionSequence==0 &&
                  second.TransitionSequence==1 &&
                  second.Workspace==ClientWorkspaceKind.Results,
                  "workspace transition sequence must remain monotonic before disposal");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var runtime=new ClientWorkspaceRuntime();
            var first=runtime.Current;
            runtime.TryNavigate(ClientWorkspaceKind.Results);
            var second=runtime.Current;
            runtime.Dispose();
            Check(first.TransitionSequence==0 &&
                  second.TransitionSequence==1 &&
                  second.Workspace==ClientWorkspaceKind.Results,
                  "workspace transition sequence must remain monotonic before disposal");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var runtime=new ClientWorkspaceRuntime();
            var first=runtime.Current;
            runtime.TryNavigate(ClientWorkspaceKind.Results);
            var second=runtime.Current;
            runtime.Dispose();
            Check(first.TransitionSequence==0 &&
                  second.TransitionSequence==1 &&
                  second.Workspace==ClientWorkspaceKind.Results,
                  "workspace transition sequence must remain monotonic before disposal");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var runtime=new ClientWorkspaceRuntime();
            var first=runtime.Current;
            runtime.TryNavigate(ClientWorkspaceKind.Results);
            var second=runtime.Current;
            runtime.Dispose();
            Check(first.TransitionSequence==0 &&
                  second.TransitionSequence==1 &&
                  second.Workspace==ClientWorkspaceKind.Results,
                  "workspace transition sequence must remain monotonic before disposal");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var runtime=new ClientWorkspaceRuntime();
            var first=runtime.Current;
            runtime.TryNavigate(ClientWorkspaceKind.Results);
            var second=runtime.Current;
            runtime.Dispose();
            Check(first.TransitionSequence==0 &&
                  second.TransitionSequence==1 &&
                  second.Workspace==ClientWorkspaceKind.Results,
                  "workspace transition sequence must remain monotonic before disposal");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var runtime=new ClientWorkspaceRuntime();
            var first=runtime.Current;
            runtime.TryNavigate(ClientWorkspaceKind.Results);
            var second=runtime.Current;
            runtime.Dispose();
            Check(first.TransitionSequence==0 &&
                  second.TransitionSequence==1 &&
                  second.Workspace==ClientWorkspaceKind.Results,
                  "workspace transition sequence must remain monotonic before disposal");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var runtime=new ClientWorkspaceRuntime();
            var first=runtime.Current;
            runtime.TryNavigate(ClientWorkspaceKind.Results);
            var second=runtime.Current;
            runtime.Dispose();
            Check(first.TransitionSequence==0 &&
                  second.TransitionSequence==1 &&
                  second.Workspace==ClientWorkspaceKind.Results,
                  "workspace transition sequence must remain monotonic before disposal");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var runtime=new ClientWorkspaceRuntime();
            var first=runtime.Current;
            runtime.TryNavigate(ClientWorkspaceKind.Results);
            var second=runtime.Current;
            runtime.Dispose();
            Check(first.TransitionSequence==0 &&
                  second.TransitionSequence==1 &&
                  second.Workspace==ClientWorkspaceKind.Results,
                  "workspace transition sequence must remain monotonic before disposal");
        }
        if(round==100)
            return Task.CompletedTask;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
