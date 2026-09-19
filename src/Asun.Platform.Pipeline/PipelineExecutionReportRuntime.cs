using System.Security.Cryptography;
using System.Text;

namespace Asun.Platform.Pipeline;

public static class PipelineExecutionReportRuntime
{
    public static PipelineExecutionReport Create<T>(
        PipelineDefinition<T> pipeline,
        PipelineExecutionTrace<T> trace)
    {
        if(!PipelineExecutionTraceValidationRuntime.IsValid(pipeline,trace))
            throw new ArgumentException("Pipeline execution trace is invalid.",nameof(trace));

        var canonical=string.Join("|",trace.ExecutedStages);
        var fingerprint=Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();

        return new PipelineExecutionReport(
            trace.ExecutedStages.Count,
            trace.ExecutedStages.ToArray(),
            fingerprint);
    }
}
