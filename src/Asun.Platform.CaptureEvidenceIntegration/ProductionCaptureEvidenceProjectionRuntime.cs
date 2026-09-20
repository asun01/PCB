using Asun.Platform.Evidence;
using Asun.Production.Runtime;

namespace Asun.Platform.CaptureEvidenceIntegration;

public static class ProductionCaptureEvidenceProjectionRuntime
{
    public static IReadOnlyList<ProductionCaptureEvidenceFrameReference> Create(
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionFrameProvenance> provenance,
        IReadOnlyList<ProductionEvidenceFrameReference> evidenceFrames)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(provenance);
        ArgumentNullException.ThrowIfNull(evidenceFrames);

        if(productionReport.FrameCount!=provenance.Count ||
           productionReport.FrameCount!=evidenceFrames.Count)
        {
            throw new ArgumentException("Production, provenance, and evidence-reference counts must match.");
        }

        if(!ProductionFrameProvenanceRuntime.IsValid(productionReport,provenance))
            throw new ArgumentException("Production frame provenance is invalid.",nameof(provenance));

        var productionFrames=productionReport.Frames.OrderBy(frame=>frame.Sequence.Value).ToArray();
        var captureFrames=provenance.OrderBy(frame=>frame.Sequence.Value).ToArray();
        var evidence= evidenceFrames.OrderBy(frame=>frame.Sequence).ToArray();
        var result=new List<ProductionCaptureEvidenceFrameReference>(evidence.Count);

        for(var index=0;index<evidence.Count;index++)
        {
            var production=productionFrames[index];
            var capture=captureFrames[index];
            var reference=evidence[index];

            if(reference.Sequence!=production.Sequence.Value ||
               reference.Sequence!=capture.Sequence.Value)
            {
                throw new ArgumentException($"Evidence reference sequence {reference.Sequence} does not match capture sequence {capture.Sequence.Value}.");
            }

            if(reference.Handles is null)
                throw new ArgumentException("Evidence handle collection cannot be null.");

            var set=new EvidenceReferenceSet(reference.Handles);
            if(!EvidenceReferenceSetValidationRuntime.IsValid(set))
                throw new ArgumentException($"Evidence handles for sequence {reference.Sequence} are not canonical.");

            if(capture.PayloadFingerprint!=production.InputFingerprint)
                throw new ArgumentException("Capture provenance payload fingerprint does not match production input fingerprint.");

            result.Add(
                new ProductionCaptureEvidenceFrameReference(
                    capture.Sequence.Value,
                    capture.PayloadFingerprint,
                    capture.Width,
                    capture.Height,
                    capture.PixelFormat,
                    reference.Handles.ToArray()));
        }

        return result;
    }

    public static IReadOnlyList<string> Validate(
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionCaptureEvidenceFrameReference> references)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(references);

        var errors=new List<string>();
        if(productionReport.FrameCount!=references.Count)
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

        if(references.Select(frame=>frame.Sequence).Distinct().Count()!=references.Count)
            errors.Add("Capture evidence-reference sequences must be unique.");

        return errors;
    }

    public static bool IsValid(
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionCaptureEvidenceFrameReference> references)=>
        Validate(productionReport,references).Count==0;
}
