using System.Security.Cryptography;
using System.Text;
using Asun.Device.Contracts;
using Asun.Program.Core;
using Asun.Platform.Pipeline;

namespace Asun.Production.Runtime;

public static class ProductionSessionRuntime
{
    public static async ValueTask<ProductionSessionReport> RunAsync(
        ProductionSessionDefinition definition,
        IFrameSource source,
        CancellationToken cancellationToken=default)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(source);

        if(definition.SessionId==Guid.Empty)
            throw new ArgumentException("Production session id cannot be empty.",nameof(definition));

        if(definition.FrameCount<=0)
            throw new ArgumentOutOfRangeException(nameof(definition.FrameCount));

        if(!ProgramExecutionPlanValidationRuntime.IsValid(
            CreateSourceProgram(definition.ProgramPlan),
            definition.ProgramPlan))
        {
            throw new ArgumentException(
                "Production session program plan is invalid.",
                nameof(definition));
        }

        if(definition.Pipeline.Stages.Count==0)
            throw new ArgumentException(
                "Production session pipeline cannot be empty.",
                nameof(definition));

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
        }

        var builder=new StringBuilder();
        builder.Append(definition.SessionId).Append('|')
            .Append(definition.ProgramPlan.Fingerprint).Append('|')
            .Append(definition.FrameCount).Append('|');

        foreach(var execution in frameExecutions)
        {
            builder.Append(execution.Sequence.Value).Append('|')
                .Append(execution.InputFingerprint).Append('|')
                .Append(execution.PipelineReport.Fingerprint).Append('|');
        }

        var fingerprint=Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();

        return new ProductionSessionReport(
            definition.SessionId,
            definition.ProgramPlan.Fingerprint,
            frameExecutions.Count,
            frameExecutions,
            fingerprint);
    }

    private static InspectionProgram CreateSourceProgram(
        ProgramExecutionPlan plan)=>
        new(
            plan.ProgramId,
            "validated-source-program",
            plan.Version,
            plan.Steps);
}
