using Asun.Device.Contracts;
using Asun.Production.Runtime;
using Asun.Platform.ClientIntegration;

namespace Asun.Platform.ClientIntegration.Smoke;

public static class ClientProductionProgress2HundredStageSmoke
{
    private class BlockingProgressRunner : IProductionSessionRunner, IProductionSessionProgressRunner
    {
        public readonly TaskCompletionSource<bool> Reported =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public TaskCompletionSource<bool>? Gate { get; set; }

        public ValueTask<ProductionSessionReport> RunAsync(
            ProductionSessionDefinition definition,
            IFrameSource source,
            CancellationToken cancellationToken=default) =>
            RunAsync(definition,source,new Progress<ProductionSessionProgress>(_=>{}),cancellationToken);

        public async ValueTask<ProductionSessionReport> RunAsync(
            ProductionSessionDefinition definition,
            IFrameSource source,
            IProgress<ProductionSessionProgress> progress,
            CancellationToken cancellationToken=default)
        {
            progress.Report(new ProductionSessionProgress(
                definition.SessionId,
                1,
                definition.FrameCount,
                FrameSequence.Create(1)));
            Reported.TrySetResult(true);

            if(Gate is not null)
                await Gate.Task.WaitAsync(cancellationToken);

            progress.Report(new ProductionSessionProgress(
                definition.SessionId,
                definition.FrameCount,
                definition.FrameCount,
                FrameSequence.Create(definition.FrameCount)));

            return FakeReport(definition);
        }
    }

    private sealed class ImmediateProgressRunner : BlockingProgressRunner
    {
    }

    private static ProductionSessionReport FakeReport(
        ProductionSessionDefinition definition)
    {
        var frames=Enumerable.Range(1,definition.FrameCount)
            .Select(i=>new ProductionFrameExecution(
                FrameSequence.Create(i),
                new string((char)('a'+i),64),
                new Asun.Platform.Pipeline.PipelineExecutionReport(
                    1,
                    new[]{"Stage"},
                    new string((char)('b'+i),64))))
            .ToArray();

        return new ProductionSessionReport(
            definition.SessionId,
            definition.ProgramPlan.Fingerprint,
            frames.Length,
            frames,
            new string('f',64));
    }


    public static async Task RunAsync(Action<bool,string> Check)
    {
        var round=0;
        for(var iteration0=0;iteration0<10;iteration0++)
        {
            round++;
            var definition=Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateDefinition(frameCount:3);
            var gate=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var runner=new BlockingProgressRunner();
            runner.Gate=gate;
            var workspace=new ClientProductionWorkspace(runner);
            workspace.Load(definition);
            var task=workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            await runner.Reported.Task;
            var running=workspace.Snapshot;
            gate.SetResult(true);
            await task;
            Check(running.Status==ClientExecutionStatus.Running &&
                  running.TargetFrameCount==3 &&
                  running.FramesProcessed==1 &&
                  running.LastSequence?.Value==1,
                  "client workspace must expose intermediate frame progress while Running");
        }
        for(var iteration1=0;iteration1<10;iteration1++)
        {
            round++;
            var definition=Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateDefinition(frameCount:3);
            var gate=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var runner=new BlockingProgressRunner();
            runner.Gate=gate;
            var workspace=new ClientProductionWorkspace(runner);
            workspace.Load(definition);
            var task=workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            await runner.Reported.Task;
            var running=workspace.Snapshot;
            gate.SetResult(true);
            await task;
            Check(running.Status==ClientExecutionStatus.Running &&
                  running.TargetFrameCount==3 &&
                  running.FramesProcessed==1 &&
                  running.LastSequence?.Value==1,
                  "client workspace must expose intermediate frame progress while Running");
        }
        for(var iteration2=0;iteration2<10;iteration2++)
        {
            round++;
            var definition=Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateDefinition(frameCount:3);
            var gate=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var runner=new BlockingProgressRunner();
            runner.Gate=gate;
            var workspace=new ClientProductionWorkspace(runner);
            workspace.Load(definition);
            var task=workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            await runner.Reported.Task;
            var running=workspace.Snapshot;
            gate.SetResult(true);
            await task;
            Check(running.Status==ClientExecutionStatus.Running &&
                  running.TargetFrameCount==3 &&
                  running.FramesProcessed==1 &&
                  running.LastSequence?.Value==1,
                  "client workspace must expose intermediate frame progress while Running");
        }
        for(var iteration3=0;iteration3<10;iteration3++)
        {
            round++;
            var definition=Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateDefinition(frameCount:3);
            var gate=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var runner=new BlockingProgressRunner();
            runner.Gate=gate;
            var workspace=new ClientProductionWorkspace(runner);
            workspace.Load(definition);
            var task=workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            await runner.Reported.Task;
            var running=workspace.Snapshot;
            gate.SetResult(true);
            await task;
            Check(running.Status==ClientExecutionStatus.Running &&
                  running.TargetFrameCount==3 &&
                  running.FramesProcessed==1 &&
                  running.LastSequence?.Value==1,
                  "client workspace must expose intermediate frame progress while Running");
        }
        for(var iteration4=0;iteration4<10;iteration4++)
        {
            round++;
            var definition=Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateDefinition(frameCount:3);
            var gate=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var runner=new BlockingProgressRunner();
            runner.Gate=gate;
            var workspace=new ClientProductionWorkspace(runner);
            workspace.Load(definition);
            var task=workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            await runner.Reported.Task;
            var running=workspace.Snapshot;
            gate.SetResult(true);
            await task;
            Check(running.Status==ClientExecutionStatus.Running &&
                  running.TargetFrameCount==3 &&
                  running.FramesProcessed==1 &&
                  running.LastSequence?.Value==1,
                  "client workspace must expose intermediate frame progress while Running");
        }
        for(var iteration5=0;iteration5<10;iteration5++)
        {
            round++;
            var definition=Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateDefinition(frameCount:3);
            var gate=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var runner=new BlockingProgressRunner();
            runner.Gate=gate;
            var workspace=new ClientProductionWorkspace(runner);
            workspace.Load(definition);
            var task=workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            await runner.Reported.Task;
            var running=workspace.Snapshot;
            gate.SetResult(true);
            await task;
            Check(running.Status==ClientExecutionStatus.Running &&
                  running.TargetFrameCount==3 &&
                  running.FramesProcessed==1 &&
                  running.LastSequence?.Value==1,
                  "client workspace must expose intermediate frame progress while Running");
        }
        for(var iteration6=0;iteration6<10;iteration6++)
        {
            round++;
            var definition=Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateDefinition(frameCount:3);
            var gate=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var runner=new BlockingProgressRunner();
            runner.Gate=gate;
            var workspace=new ClientProductionWorkspace(runner);
            workspace.Load(definition);
            var task=workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            await runner.Reported.Task;
            var running=workspace.Snapshot;
            gate.SetResult(true);
            await task;
            Check(running.Status==ClientExecutionStatus.Running &&
                  running.TargetFrameCount==3 &&
                  running.FramesProcessed==1 &&
                  running.LastSequence?.Value==1,
                  "client workspace must expose intermediate frame progress while Running");
        }
        for(var iteration7=0;iteration7<10;iteration7++)
        {
            round++;
            var definition=Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateDefinition(frameCount:3);
            var gate=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var runner=new BlockingProgressRunner();
            runner.Gate=gate;
            var workspace=new ClientProductionWorkspace(runner);
            workspace.Load(definition);
            var task=workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            await runner.Reported.Task;
            var running=workspace.Snapshot;
            gate.SetResult(true);
            await task;
            Check(running.Status==ClientExecutionStatus.Running &&
                  running.TargetFrameCount==3 &&
                  running.FramesProcessed==1 &&
                  running.LastSequence?.Value==1,
                  "client workspace must expose intermediate frame progress while Running");
        }
        for(var iteration8=0;iteration8<10;iteration8++)
        {
            round++;
            var definition=Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateDefinition(frameCount:3);
            var gate=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var runner=new BlockingProgressRunner();
            runner.Gate=gate;
            var workspace=new ClientProductionWorkspace(runner);
            workspace.Load(definition);
            var task=workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            await runner.Reported.Task;
            var running=workspace.Snapshot;
            gate.SetResult(true);
            await task;
            Check(running.Status==ClientExecutionStatus.Running &&
                  running.TargetFrameCount==3 &&
                  running.FramesProcessed==1 &&
                  running.LastSequence?.Value==1,
                  "client workspace must expose intermediate frame progress while Running");
        }
        for(var iteration9=0;iteration9<10;iteration9++)
        {
            round++;
            var definition=Asun.Platform.SimulationIntegration.ClientSimulationSessionFactory.CreateDefinition(frameCount:3);
            var gate=new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var runner=new BlockingProgressRunner();
            runner.Gate=gate;
            var workspace=new ClientProductionWorkspace(runner);
            workspace.Load(definition);
            var task=workspace.StartAsync(new Asun.Device.Impl.SimulatedFrameSource(8,8));
            await runner.Reported.Task;
            var running=workspace.Snapshot;
            gate.SetResult(true);
            await task;
            Check(running.Status==ClientExecutionStatus.Running &&
                  running.TargetFrameCount==3 &&
                  running.FramesProcessed==1 &&
                  running.LastSequence?.Value==1,
                  "client workspace must expose intermediate frame progress while Running");
        }
        if(round==100)
            return;

        throw new InvalidOperationException("acceptance matrix must execute exactly 100 rounds");
    }
}
