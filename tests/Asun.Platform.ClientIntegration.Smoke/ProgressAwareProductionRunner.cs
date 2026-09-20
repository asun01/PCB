using Asun.Device.Contracts;
using Asun.Platform.ClientIntegration;
using Asun.Platform.Pipeline;
using Asun.Program.Core;
using Asun.Production.Runtime;

namespace Asun.Platform.ClientIntegration.Smoke;

internal sealed class ProgressAwareProductionRunner : IProductionSessionProgressRunner
{
    private readonly TaskCompletionSource _progressReached=
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource _release=
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task ProgressReached=>_progressReached.Task;

    public void Release()=>_release.TrySetResult();

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

        _progressReached.TrySetResult();

        await _release.Task.WaitAsync(cancellationToken);

        var frames=Enumerable.Range(1,definition.FrameCount)
            .Select(i=>new ProductionFrameExecution(
                FrameSequence.Create(i),
                new string((char)('a'+i),64),
                new PipelineExecutionReport(
                    1,
                    new[]{"Stage-1"},
                    new string('b',64))))
            .ToArray();

        progress.Report(new ProductionSessionProgress(
            definition.SessionId,
            definition.FrameCount,
            definition.FrameCount,
            frames[^1].Sequence));

        return new ProductionSessionReport(
            definition.SessionId,
            definition.ProgramPlan.Fingerprint,
            frames.Length,
            frames,
            new string('c',64));
    }

    public ValueTask<ProductionSessionReport> RunAsync(
        ProductionSessionDefinition definition,
        IFrameSource source,
        CancellationToken cancellationToken=default)=>
        RunAsync(definition,source,new Progress<ProductionSessionProgress>(_=>{}),cancellationToken);
}
