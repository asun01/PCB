using Asun.Platform.Pipeline;
using Asun.Production.Runtime;

namespace Asun.Platform.PipelineProductionIntegration;

public static class ProductionPipelineReplayAuditValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        ProductionSessionDefinition definition,
        ProductionSessionReport report,
        ProductionPipelineReplayAudit audit)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(report);
        ArgumentNullException.ThrowIfNull(audit);

        var errors=new List<string>();
        if(!ProductionSessionValidationRuntime.IsValid(definition,report))
            errors.Add("Production session is invalid.");

        if(audit.SessionId!=report.SessionId)
            errors.Add("Pipeline replay audit session id must match production report.");
        if(audit.ProgramFingerprint!=report.ProgramFingerprint)
            errors.Add("Pipeline replay audit program fingerprint must match production report.");
        if(audit.Frames.Count!=report.FrameCount)
            errors.Add("Pipeline replay audit frame count must match production report.");

        var productionFrames=report.Frames.OrderBy(frame=>frame.Sequence.Value).ToArray();
        var ordered=audit.Frames.OrderBy(frame=>frame.Sequence).ToArray();
        var expectedStages=definition.Pipeline.Stages.OrderBy(stage=>stage.Order).Select(stage=>stage.Name).ToArray();
        var count=Math.Min(productionFrames.Length,ordered.Length);

        for(var index=0;index<count;index++)
        {
            var production=productionFrames[index];
            var actual=ordered[index];
            if(actual.Sequence!=production.Sequence.Value)
                errors.Add($"Pipeline replay audit frame {index} sequence mismatch.");
            if(actual.InputFingerprint!=production.InputFingerprint)
                errors.Add($"Pipeline replay audit frame {index} input fingerprint mismatch.");
            if(actual.StageCount!=actual.ExecutedStages.Count)
                errors.Add($"Pipeline replay audit frame {index} stage count mismatch.");
            if(!actual.ExecutedStages.SequenceEqual(expectedStages))
                errors.Add($"Pipeline replay audit frame {index} stage order mismatch.");
            if(!PipelineExecutionReportValidationRuntime.IsValid(
                new PipelineExecutionReport(
                    actual.StageCount,
                    actual.ExecutedStages,
                    actual.PipelineReportFingerprint)))
            {
                errors.Add($"Pipeline replay audit frame {index} report is invalid.");
            }
        }

        if(audit.Frames.Select(frame=>frame.Sequence).Distinct().Count()!=audit.Frames.Count)
            errors.Add("Pipeline replay audit sequences must be unique.");

        if(audit.Fingerprint.Length!=64 ||
           !audit.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Pipeline replay audit fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count>0)
            return errors;

        var expected=ProductionPipelineReplayAuditRuntime.CreateFingerprint(
            report.SessionId,
            report.ProgramFingerprint,
            audit.Frames);
        if(expected!=audit.Fingerprint)
            errors.Add("Pipeline replay audit fingerprint does not match canonical content.");

        return errors;
    }

    public static bool IsValid(
        ProductionSessionDefinition definition,
        ProductionSessionReport report,
        ProductionPipelineReplayAudit audit)=>
        Validate(definition,report,audit).Count==0;
}
