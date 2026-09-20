using System.Security.Cryptography;
using System.Text;
using Asun.Device.Contracts;
using Asun.Platform.Pipeline;
using Asun.Production.Runtime;

namespace Asun.Platform.PipelineProductionIntegration;

public static class ProductionPipelineReplayAuditRuntime
{
    public static ProductionPipelineReplayAudit Create(
        ProductionSessionDefinition definition,
        ProductionSessionReport report)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(report);

        if(!ProductionSessionValidationRuntime.IsValid(definition,report))
            throw new ArgumentException("Production session is invalid.",nameof(report));

        var frames=report.Frames.OrderBy(frame=>frame.Sequence.Value).ToArray();
        var audits=new List<ProductionPipelineReplayFrameAudit>(frames.Length);

        foreach(var frame in frames)
        {
            if(!PipelineExecutionReportValidationRuntime.IsValid(frame.PipelineReport))
                throw new ArgumentException($"Pipeline report for sequence {frame.Sequence.Value} is invalid.");

            if(!frame.PipelineReport.ExecutedStages.SequenceEqual(
                definition.Pipeline.Stages.OrderBy(stage=>stage.Order).Select(stage=>stage.Name)))
            {
                throw new ArgumentException($"Pipeline stage sequence for frame {frame.Sequence.Value} does not match the production definition.");
            }

            audits.Add(
                new ProductionPipelineReplayFrameAudit(
                    frame.Sequence.Value,
                    frame.InputFingerprint,
                    frame.PipelineReport.StageCount,
                    frame.PipelineReport.ExecutedStages.ToArray(),
                    frame.PipelineReport.Fingerprint));
        }

        var fingerprint=CreateFingerprint(
            report.SessionId,
            report.ProgramFingerprint,
            audits);

        return new ProductionPipelineReplayAudit(
            report.SessionId,
            report.ProgramFingerprint,
            audits,
            fingerprint);
    }

    internal static string CreateFingerprint(
        Guid sessionId,
        string programFingerprint,
        IReadOnlyList<ProductionPipelineReplayFrameAudit> frames)
    {
        var builder=new StringBuilder();
        builder.Append(sessionId).Append('|').Append(programFingerprint).Append('|');
        foreach(var frame in frames.OrderBy(frame=>frame.Sequence))
        {
            builder.Append(frame.Sequence).Append('|')
                .Append(frame.InputFingerprint).Append('|')
                .Append(frame.StageCount).Append('|')
                .Append(frame.PipelineReportFingerprint).Append('|');
            foreach(var stage in frame.ExecutedStages)
                builder.Append(stage.Length).Append(':').Append(stage).Append('|');
        }

        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString())))
            .ToLowerInvariant();
    }
}
