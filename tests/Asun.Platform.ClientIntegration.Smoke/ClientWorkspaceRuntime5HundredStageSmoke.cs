namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientWorkspaceRuntime5HundredStageSmoke
{
    public static Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var runtime=new ClientWorkspaceRuntime();
            var sequence=new List<ClientWorkspaceKind>();
            runtime.Changed+=selection=>sequence.Add(selection.Workspace);
            runtime.TryNavigate(ClientWorkspaceKind.Program);
            runtime.TryNavigate(ClientWorkspaceKind.Quality);
            runtime.TryNavigate(ClientWorkspaceKind.Results);
            Check(sequence.SequenceEqual(new[]
                {
                    ClientWorkspaceKind.Program,
                    ClientWorkspaceKind.Quality,
                    ClientWorkspaceKind.Results
                }) &&
                runtime.Current.TransitionSequence==3,
                "workspace transitions must preserve client-visible navigation order");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var runtime=new ClientWorkspaceRuntime();
            var sequence=new List<ClientWorkspaceKind>();
            runtime.Changed+=selection=>sequence.Add(selection.Workspace);
            runtime.TryNavigate(ClientWorkspaceKind.Program);
            runtime.TryNavigate(ClientWorkspaceKind.Quality);
            runtime.TryNavigate(ClientWorkspaceKind.Results);
            Check(sequence.SequenceEqual(new[]
                {
                    ClientWorkspaceKind.Program,
                    ClientWorkspaceKind.Quality,
                    ClientWorkspaceKind.Results
                }) &&
                runtime.Current.TransitionSequence==3,
                "workspace transitions must preserve client-visible navigation order");
            Check(round==101,
                  "internal acceptance matrix guard");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var runtime=new ClientWorkspaceRuntime();
            var sequence=new List<ClientWorkspaceKind>();
            runtime.Changed+=selection=>sequence.Add(selection.Workspace);
            runtime.TryNavigate(ClientWorkspaceKind.Program);
            runtime.TryNavigate(ClientWorkspaceKind.Quality);
            runtime.TryNavigate(ClientWorkspaceKind.Results);
            Check(sequence.SequenceEqual(new[]
                {
                    ClientWorkspaceKind.Program,
                    ClientWorkspaceKind.Quality,
                    ClientWorkspaceKind.Results
                }) &&
                runtime.Current.TransitionSequence==3,
                "workspace transitions must preserve client-visible navigation order");
            Check(round==101,
                  "internal acceptance matrix guard");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var runtime=new ClientWorkspaceRuntime();
            var sequence=new List<ClientWorkspaceKind>();
            runtime.Changed+=selection=>sequence.Add(selection.Workspace);
            runtime.TryNavigate(ClientWorkspaceKind.Program);
            runtime.TryNavigate(ClientWorkspaceKind.Quality);
            runtime.TryNavigate(ClientWorkspaceKind.Results);
            Check(sequence.SequenceEqual(new[]
                {
                    ClientWorkspaceKind.Program,
                    ClientWorkspaceKind.Quality,
                    ClientWorkspaceKind.Results
                }) &&
                runtime.Current.TransitionSequence==3,
                "workspace transitions must preserve client-visible navigation order");
            Check(round==101,
                  "internal acceptance matrix guard");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var runtime=new ClientWorkspaceRuntime();
            var sequence=new List<ClientWorkspaceKind>();
            runtime.Changed+=selection=>sequence.Add(selection.Workspace);
            runtime.TryNavigate(ClientWorkspaceKind.Program);
            runtime.TryNavigate(ClientWorkspaceKind.Quality);
            runtime.TryNavigate(ClientWorkspaceKind.Results);
            Check(sequence.SequenceEqual(new[]
                {
                    ClientWorkspaceKind.Program,
                    ClientWorkspaceKind.Quality,
                    ClientWorkspaceKind.Results
                }) &&
                runtime.Current.TransitionSequence==3,
                "workspace transitions must preserve client-visible navigation order");
            Check(round==101,
                  "internal acceptance matrix guard");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var runtime=new ClientWorkspaceRuntime();
            var sequence=new List<ClientWorkspaceKind>();
            runtime.Changed+=selection=>sequence.Add(selection.Workspace);
            runtime.TryNavigate(ClientWorkspaceKind.Program);
            runtime.TryNavigate(ClientWorkspaceKind.Quality);
            runtime.TryNavigate(ClientWorkspaceKind.Results);
            Check(sequence.SequenceEqual(new[]
                {
                    ClientWorkspaceKind.Program,
                    ClientWorkspaceKind.Quality,
                    ClientWorkspaceKind.Results
                }) &&
                runtime.Current.TransitionSequence==3,
                "workspace transitions must preserve client-visible navigation order");
            Check(round==101,
                  "internal acceptance matrix guard");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var runtime=new ClientWorkspaceRuntime();
            var sequence=new List<ClientWorkspaceKind>();
            runtime.Changed+=selection=>sequence.Add(selection.Workspace);
            runtime.TryNavigate(ClientWorkspaceKind.Program);
            runtime.TryNavigate(ClientWorkspaceKind.Quality);
            runtime.TryNavigate(ClientWorkspaceKind.Results);
            Check(sequence.SequenceEqual(new[]
                {
                    ClientWorkspaceKind.Program,
                    ClientWorkspaceKind.Quality,
                    ClientWorkspaceKind.Results
                }) &&
                runtime.Current.TransitionSequence==3,
                "workspace transitions must preserve client-visible navigation order");
            Check(round==101,
                  "internal acceptance matrix guard");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var runtime=new ClientWorkspaceRuntime();
            var sequence=new List<ClientWorkspaceKind>();
            runtime.Changed+=selection=>sequence.Add(selection.Workspace);
            runtime.TryNavigate(ClientWorkspaceKind.Program);
            runtime.TryNavigate(ClientWorkspaceKind.Quality);
            runtime.TryNavigate(ClientWorkspaceKind.Results);
            Check(sequence.SequenceEqual(new[]
                {
                    ClientWorkspaceKind.Program,
                    ClientWorkspaceKind.Quality,
                    ClientWorkspaceKind.Results
                }) &&
                runtime.Current.TransitionSequence==3,
                "workspace transitions must preserve client-visible navigation order");
            Check(round==101,
                  "internal acceptance matrix guard");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var runtime=new ClientWorkspaceRuntime();
            var sequence=new List<ClientWorkspaceKind>();
            runtime.Changed+=selection=>sequence.Add(selection.Workspace);
            runtime.TryNavigate(ClientWorkspaceKind.Program);
            runtime.TryNavigate(ClientWorkspaceKind.Quality);
            runtime.TryNavigate(ClientWorkspaceKind.Results);
            Check(sequence.SequenceEqual(new[]
                {
                    ClientWorkspaceKind.Program,
                    ClientWorkspaceKind.Quality,
                    ClientWorkspaceKind.Results
                }) &&
                runtime.Current.TransitionSequence==3,
                "workspace transitions must preserve client-visible navigation order");
            Check(round==101,
                  "internal acceptance matrix guard");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var runtime=new ClientWorkspaceRuntime();
            var sequence=new List<ClientWorkspaceKind>();
            runtime.Changed+=selection=>sequence.Add(selection.Workspace);
            runtime.TryNavigate(ClientWorkspaceKind.Program);
            runtime.TryNavigate(ClientWorkspaceKind.Quality);
            runtime.TryNavigate(ClientWorkspaceKind.Results);
            Check(sequence.SequenceEqual(new[]
                {
                    ClientWorkspaceKind.Program,
                    ClientWorkspaceKind.Quality,
                    ClientWorkspaceKind.Results
                }) &&
                runtime.Current.TransitionSequence==3,
                "workspace transitions must preserve client-visible navigation order");
            Check(round==100,
                  "internal acceptance matrix guard");
        }
        return Task.CompletedTask;
    }
}
