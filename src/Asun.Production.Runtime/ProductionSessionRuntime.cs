using Asun.Device.Contracts;
using Asun.Program.Core;
using Asun.Platform.Pipeline;

namespace Asun.Production.Runtime;

public sealed record ProductionSessionProgress(
    Guid SessionId,
    int CompletedFrames,
    int TotalFrames,
    FrameSequence? LastSequence)
{
    public long Width { get; init; }
    public long Height { get; init; }
    public string PixelFormat { get; init; }="";
};

public static class ProductionSessionRuntime
{
    public static async ValueTask<ProductionSessionReport> RunAsync(
        ProductionSessionDefinition definition,
        IFrameSource source,
        CancellationToken cancellationToken=default,
        IProgress<ProductionSessionProgress>? progress=null)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(source);

        if(definition.SessionId==Guid.Empty)
            throw new ArgumentException("Production session id cannot be empty.",nameof(definition));

        if(definition.FrameCount<=0)
            throw new ArgumentOutOfRangeException(nameof(definition.FrameCount));

        if(!ProductionProgramPipelineBindingValidationRuntime.IsValid(
            definition.ProgramPlan,
            definition.Pipeline,
            ProductionProgramPipelineBindingRuntime.Create(
                definition.ProgramPlan,
                definition.Pipeline)))
        {
            throw new ArgumentException(
                "Production session program/pipeline binding is invalid.",
                nameof(definition));
        }

        var frameExecutions=new List<ProductionFrameExecution>(
            definition.FrameCount);

        for(var i=0;i<definition.FrameCount;i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var frame=await source.CaptureAsync(cancellationToken);

            if(frame is null)
                throw new InvalidOperationException(
                    "Frame source returned no frame.");

            if(!CapturedFrameValidationRuntime.IsValid(frame))
                throw new InvalidOperationException(
                    "Frame source returned an invalid frame.");

            var trace=await PipelineExecutionRuntime.ExecuteAsync(
                definition.Pipeline,
                frame,
                cancellationToken);

            if(!PipelineExecutionTraceValidationRuntime.IsValid(
                definition.Pipeline,
                trace))
            {
                throw new InvalidOperationException(
                    "Production pipeline execution trace is invalid.");
            }

            var report=PipelineExecutionReportRuntime.Create(
                definition.Pipeline,
                trace);

            frameExecutions.Add(
                new ProductionFrameExecution(
                    frame.Metadata.Sequence,
                    frame.PayloadFingerprint,
                    report));

            progress?.Report(
                new ProductionSessionProgress(
                    definition.SessionId,
                    frameExecutions.Count,
                    definition.FrameCount,
                    frame.Metadata.Sequence)
                {
                    Width=frame.Metadata.Width,
                    Height=frame.Metadata.Height,
                    PixelFormat=frame.Metadata.PixelFormat
                });
        }

        var fingerprint=ProductionSessionFingerprintRuntime.CreateFingerprint(
            definition,
            frameExecutions);

        return new ProductionSessionReport(
            definition.SessionId,
            definition.ProgramPlan.Fingerprint,
            frameExecutions.Count,
            frameExecutions,
            fingerprint);
    }
}
