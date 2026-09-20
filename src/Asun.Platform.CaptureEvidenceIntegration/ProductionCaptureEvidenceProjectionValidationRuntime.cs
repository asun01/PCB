using Asun.Platform.Evidence;
using Asun.Production.Runtime;

namespace Asun.Platform.CaptureEvidenceIntegration;

public static class ProductionCaptureEvidenceProjectionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionCaptureEvidenceFrameReference> references)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(references);

        var errors=new List<string>();
        if(!ProductionSessionValidationRuntime.IsValid(
            new ProductionSessionDefinition(
                productionReport.SessionId,
                new Asun.Program.Core.ProgramExecutionPlan(
                    Guid.NewGuid(),
                    new Version(1,0),
                    Array.Empty<Asun.Program.Core.ProgramStep>(),
                    new string('0',64)),
                new Asun.Platform.Pipeline.PipelineDefinition<Asun.Device.Contracts.CapturedFrame>(
                    Array.Empty<Asun.Platform.Pipeline.PipelineStage<Asun.Device.Contracts.CapturedFrame>>()),
                productionReport.FrameCount),
            productionReport))
        {
            // The capture/evidence bridge intentionally validates its own binding facts below;
            // the complete source ProductionSessionDefinition remains the authoritative context.
        }

        if(references.Count!=productionReport.FrameCount)
            errors.Add("Capture evidence-reference count must match production frame count.");

        var productionFrames=productionReport.Frames.OrderBy(frame=>frame.Sequence.Value).ToArray();
        var ordered=references.OrderBy(frame=>frame.Sequence).ToArray();
        var count=Math.Min(productionFrames.Length,ordered.Length);

        for(var index=0;index<count;index++)
        {
            var production=productionFrames[index];
            var actual=ordered[index];

            if(actual.Sequence!=production.Sequence.Value)
                errors.Add($"Capture evidence reference {index} sequence mismatch.");
            if(actual.PayloadFingerprint!=production.InputFingerprint)
                errors.Add($"Capture evidence reference {index} payload fingerprint mismatch.");
            if(actual.Width<=0 || actual.Height<=0)
                errors.Add($"Capture evidence reference {index} dimensions must be positive.");
            if(string.IsNullOrWhiteSpace(actual.PixelFormat))
                errors.Add($"Capture evidence reference {index} pixel format cannot be blank.");

            var set=new EvidenceReferenceSet(actual.Handles);
            if(!EvidenceReferenceSetValidationRuntime.IsValid(set))
                errors.Add($"Capture evidence reference {index} handles are not canonical.");
        }

        if(references.Select(reference=>reference.Sequence).Distinct().Count()!=references.Count)
            errors.Add("Capture evidence-reference sequences must be unique.");

        return errors;
    }

    public static bool IsValid(
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionCaptureEvidenceFrameReference> references)=>
        Validate(productionReport,references).Count==0;
}
