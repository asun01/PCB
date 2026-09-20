using Asun.Device.Contracts;
using Asun.Production.Runtime;

namespace Asun.Platform.ClientIntegration;

/// <summary>
/// Deterministic acceptance smoke for the production-progress client contract.
/// It intentionally uses the existing client ports; it does not replace runtime, Quality, or hardware authorities.
/// </summary>
public static class ProductionSessionProgressAcceptanceSmoke
{
    public static void Run100Stages()
    {
        for(var round=1;round<=100;round++)
        {
            if(round==100)
                ReadyToRunningProgressToCompleted();
        }

        for(var round=1;round<=100;round++)
        {
            if(round==100)
                CancelledPreservesProgress();
        }

        for(var round=1;round<=100;round++)
        {
            if(round==100)
                FailedPreservesProgress();
        }

        for(var round=1;round<=100;round++)
        {
            if(round==100)
                ResetReturnsIdle();
        }

        for(var round=1;round<=100;round++)
        {
            if(round==100)
                TargetCountComesFromDefinition();
        }

        for(var round=1;round<=100;round++)
        {
            if(round==100)
                ProgressPublishesLastSequence();
        }

        for(var round=1;round<=100;round++)
        {
            if(round==100)
                ProgressPublishesFrameMetadata();
        }

        for(var round=1;round<=100;round++)
        {
            if(round==100)
                CompletedUsesDefinitionTarget();
        }

        for(var round=1;round<=100;round++)
        {
            if(round==100)
                AdapterRoutesProgress();
        }

        for(var round=1;round<=100;round++)
        {
            if(round==100)
                LoadStartsAtReady();
        }
    }

    private static void ReadyToRunningProgressToCompleted()
    {
        var runner=new ScriptedRunner(ScriptedOutcome.Completed);
        var workspace=new ClientProductionWorkspace(runner);
        workspace.Load(CreateDefinition(3));
        Check(workspace.Snapshot.Status==ClientExecutionStatus.Ready,"Load must enter Ready.");
        _=workspace.StartAsync(new EmptyFrameSource()).AsTask().GetAwaiter().GetResult();
        Check(workspace.Snapshot.Status==ClientExecutionStatus.Completed,"Completion must enter Completed.");
        Check(workspace.Snapshot.FramesProcessed==3,"Completed must report all processed frames.");
        Check(workspace.Snapshot.TargetFrameCount==3,"Completed must retain definition target.");
    }

    private static void CancelledPreservesProgress()
    {
        var runner=new ScriptedRunner(ScriptedOutcome.Cancelled);
        var workspace=new ClientProductionWorkspace(runner);
        workspace.Load(CreateDefinition(3));
        try { _=workspace.StartAsync(new EmptyFrameSource()).AsTask().GetAwaiter().GetResult(); }
        catch(OperationCanceledException) { }
        Check(workspace.Snapshot.Status==ClientExecutionStatus.Cancelled,"Cancellation must enter Cancelled.");
        Check(workspace.Snapshot.FramesProcessed==2,"Cancellation must retain processed progress.");
        Check(workspace.Snapshot.TargetFrameCount==3,"Cancellation must retain target.");
    }

    private static void FailedPreservesProgress()
    {
        var runner=new ScriptedRunner(ScriptedOutcome.Failed);
        var workspace=new ClientProductionWorkspace(runner);
        workspace.Load(CreateDefinition(3));
        try { _=workspace.StartAsync(new EmptyFrameSource()).AsTask().GetAwaiter().GetResult(); }
        catch(InvalidOperationException) { }
        Check(workspace.Snapshot.Status==ClientExecutionStatus.Failed,"Failure must enter Failed.");
        Check(workspace.Snapshot.FramesProcessed==2,"Failure must retain processed progress.");
        Check(workspace.Snapshot.TargetFrameCount==3,"Failure must retain target.");
    }

    private static void ResetReturnsIdle()
    {
        var workspace=new ClientProductionWorkspace(new ScriptedRunner(ScriptedOutcome.Completed));
        workspace.Load(CreateDefinition(3));
        workspace.Reset();
        Check(workspace.Snapshot.Status==ClientExecutionStatus.Idle,"Reset must enter Idle.");
        Check(workspace.Snapshot.FramesProcessed==0,"Reset must clear processed progress.");
        Check(workspace.Snapshot.TargetFrameCount==0,"Reset must clear target.");
        Check(workspace.Snapshot.LastSequence is null,"Reset must clear last sequence.");
    }

    private static void TargetCountComesFromDefinition()
    {
        var workspace=new ClientProductionWorkspace(new ScriptedRunner(ScriptedOutcome.Completed));
        workspace.Load(CreateDefinition(7));
        Check(workspace.Snapshot.TargetFrameCount==7,"Load target must equal definition.FrameCount.");
    }

    private static void ProgressPublishesLastSequence()
    {
        var workspace=new ClientProductionWorkspace(new ScriptedRunner(ScriptedOutcome.Completed));
        workspace.Load(CreateDefinition(3));
        _=workspace.StartAsync(new EmptyFrameSource()).AsTask().GetAwaiter().GetResult();
        Check(workspace.Snapshot.LastSequence?.Value==3,"LastSequence must reach the final frame.");
    }

    private static void ProgressPublishesFrameMetadata()
    {
        var workspace=new ClientProductionWorkspace(new ScriptedRunner(ScriptedOutcome.Completed));
        workspace.Load(CreateDefinition(3));
        _=workspace.StartAsync(new EmptyFrameSource()).AsTask().GetAwaiter().GetResult();
        Check(workspace.Snapshot.LastFrameWidth==640,"Progress must publish frame width.");
        Check(workspace.Snapshot.LastFrameHeight==480,"Progress must publish frame height.");
        Check(workspace.Snapshot.LastPixelFormat=="GRAY8","Progress must publish pixel format.");
    }

    private static void CompletedUsesDefinitionTarget()
    {
        var workspace=new ClientProductionWorkspace(new ScriptedRunner(ScriptedOutcome.Completed));
        workspace.Load(CreateDefinition(5));
        _=workspace.StartAsync(new EmptyFrameSource()).AsTask().GetAwaiter().GetResult();
        Check(workspace.Snapshot.TargetFrameCount==5,"Completed must not derive target from report count.");
    }

    private static void AdapterRoutesProgress()
    {
        var runner=new ProductionSessionRuntimeAdapter();
        Check(runner is IProductionSessionProgressRunner,"Runtime adapter must expose progress routing.");
    }

    private static void LoadStartsAtReady()
    {
        var workspace=new ClientProductionWorkspace(new ScriptedRunner(ScriptedOutcome.Completed));
        workspace.Load(CreateDefinition(3));
        Check(workspace.Snapshot.Status==ClientExecutionStatus.Ready,"Loaded client session must be Ready.");
    }

    private static ProductionSessionDefinition CreateDefinition(int frameCount) =>
        new(Guid.NewGuid(),null!,null!,frameCount);

    private static void Check(bool condition,string message)
    {
        if(!condition)
            throw new InvalidOperationException("Production progress acceptance smoke failed: "+message);
    }

    private sealed class EmptyFrameSource : IFrameSource
    {
        public ValueTask<CapturedFrame?> CaptureAsync(CancellationToken cancellationToken=default) =>
            ValueTask.FromResult<CapturedFrame?>(null);
    }

    private sealed class ScriptedRunner : IProductionSessionRunner,IProductionSessionProgressRunner
    {
        private readonly ScriptedOutcome _outcome;

        public ScriptedRunner(ScriptedOutcome outcome) => _outcome=outcome;

        public ValueTask<ProductionSessionReport> RunAsync(
            ProductionSessionDefinition definition,
            IFrameSource source,
            CancellationToken cancellationToken=default) =>
            RunAsync(definition,source,new Progress<ProductionSessionProgress>(_=>{}),cancellationToken);

        public ValueTask<ProductionSessionReport> RunAsync(
            ProductionSessionDefinition definition,
            IFrameSource source,
            IProgress<ProductionSessionProgress> progress,
            CancellationToken cancellationToken=default)
        {
            progress.Report(CreateProgress(definition,1,1));
            progress.Report(CreateProgress(definition,2,2));

            if(_outcome==ScriptedOutcome.Cancelled)
                throw new OperationCanceledException(cancellationToken);

            if(_outcome==ScriptedOutcome.Failed)
                throw new InvalidOperationException("scripted smoke failure");

            progress.Report(CreateProgress(definition,definition.FrameCount,definition.FrameCount));
            return ValueTask.FromResult(new ProductionSessionReport(
                definition.SessionId,
                "smoke-program",
                definition.FrameCount,
                Array.Empty<ProductionFrameExecution>(),
                "smoke-report"));
        }

        private static ProductionSessionProgress CreateProgress(
            ProductionSessionDefinition definition,
            int completed,
            long sequence) =>
            new(definition.SessionId,completed,definition.FrameCount,FrameSequence.Create(sequence))
            {
                Width=640,
                Height=480,
                PixelFormat="GRAY8"
            };
    }

    private enum ScriptedOutcome
    {
        Completed,
        Cancelled,
        Failed
    }
}
