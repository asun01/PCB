using Asun.Platform.Evidence;

namespace Asun.Production.Runtime;

public static class ProductionEvidenceReferenceProjectionValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        ProductionSessionReport productionReport,
        ProductionEvidenceReferenceProjection projection)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(projection);

        var errors=new List<string>();

        if(projection.ProductionSessionId!=productionReport.SessionId)
            errors.Add("Evidence projection production session id must match the report.");

        if(projection.ProductionFingerprint!=productionReport.Fingerprint)
            errors.Add("Evidence projection production fingerprint must match the report.");

        if(projection.Frames.Count!=productionReport.FrameCount)
            errors.Add("Evidence projection frame count must match the production report.");

        var productionFrames=productionReport.Frames
            .OrderBy(frame=>frame.Sequence.Value)
            .ToArray();
        var projectionFrames=projection.Frames
            .OrderBy(frame=>frame.Sequence)
            .ToArray();

        var count=Math.Min(productionFrames.Length,projectionFrames.Length);
        for(var index=0;index<count;index++)
        {
            var frame=productionFrames[index];
            var projectionFrame=projectionFrames[index];

            if(frame.Sequence.Value!=projectionFrame.Sequence)
                errors.Add($"Evidence projection frame {index} sequence mismatch.");

            var referenceSet=new EvidenceReferenceSet(projectionFrame.Handles);
            if(!EvidenceReferenceSetValidationRuntime.IsValid(referenceSet))
                errors.Add($"Evidence projection frame {index} handles are not canonical.");
        }

        if(projection.Fingerprint.Length!=64 ||
           !projection.Fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Evidence projection fingerprint must be 64 lowercase hexadecimal characters.");
        }

        if(errors.Count==0)
        {
            var expected=ProductionEvidenceReferenceProjectionRuntime.CreateFingerprint(
                productionReport.SessionId,
                productionReport.Fingerprint,
                projection.Frames);

            if(expected!=projection.Fingerprint)
                errors.Add("Evidence projection fingerprint does not match its canonical content.");
        }

        return errors;
    }

    public static bool IsValid(
        ProductionSessionReport productionReport,
        ProductionEvidenceReferenceProjection projection)=>
        Validate(productionReport,projection).Count==0;
}
