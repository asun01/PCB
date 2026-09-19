using Asun.Device.Contracts;

namespace Asun.Production.Runtime;

public static class ProductionFrameProvenanceRuntime
{
    public static IReadOnlyList<ProductionFrameProvenance> Create(
        ProductionSessionReport productionReport,
        IReadOnlyList<CapturedFrame> capturedFrames)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(capturedFrames);

        if(productionReport.FrameCount!=capturedFrames.Count)
            throw new ArgumentException(
                "Captured frame count must match production frame count.",
                nameof(capturedFrames));

        var productionFrames=productionReport.Frames
            .OrderBy(frame=>frame.Sequence.Value)
            .ToArray();
        var result=new List<ProductionFrameProvenance>(capturedFrames.Count);

        for(var index=0;index<capturedFrames.Count;index++)
        {
            var captured=capturedFrames[index] ?? throw new ArgumentException(
                "Captured frame collection cannot contain null entries.",
                nameof(capturedFrames));
            if(!CapturedFrameValidationRuntime.IsValid(captured))
                throw new ArgumentException(
                    $"Captured frame {index} is invalid.",
                    nameof(capturedFrames));

            var production=productionFrames[index];
            if(captured.Metadata.Sequence!=production.Sequence)
                throw new ArgumentException(
                    $"Captured frame sequence {captured.Metadata.Sequence.Value} does not match production sequence {production.Sequence.Value}.");

            if(captured.PayloadFingerprint!=production.InputFingerprint)
                throw new ArgumentException(
                    $"Captured frame fingerprint for sequence {captured.Metadata.Sequence.Value} does not match the production report.");

            result.Add(
                new ProductionFrameProvenance(
                    captured.Metadata.Sequence,
                    captured.Metadata.Width,
                    captured.Metadata.Height,
                    captured.Metadata.PixelFormat,
                    captured.Metadata.CapturedAtUtc,
                    captured.PayloadFingerprint));
        }

        return result;
    }

    public static IReadOnlyList<string> Validate(
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionFrameProvenance> provenance)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(provenance);

        var errors=new List<string>();
        if(productionReport.FrameCount!=provenance.Count)
            errors.Add("Frame provenance count must match production frame count.");

        var productionFrames=productionReport.Frames
            .OrderBy(frame=>frame.Sequence.Value)
            .ToArray();
        var ordered=provenance
            .OrderBy(frame=>frame.Sequence.Value)
            .ToArray();
        var count=Math.Min(productionFrames.Length,ordered.Length);

        for(var index=0;index<count;index++)
        {
            var production=productionFrames[index];
            var actual=ordered[index];

            if(actual.Sequence!=production.Sequence)
                errors.Add($"Frame provenance {index} sequence mismatch.");

            if(actual.PayloadFingerprint!=production.InputFingerprint)
                errors.Add($"Frame provenance {index} payload fingerprint mismatch.");

            if(actual.Width<=0 || actual.Height<=0)
                errors.Add($"Frame provenance {index} dimensions must be positive.");

            if(string.IsNullOrWhiteSpace(actual.PixelFormat))
                errors.Add($"Frame provenance {index} pixel format cannot be blank.");

            if(actual.PayloadFingerprint.Length!=64 ||
               !actual.PayloadFingerprint.All(character=>
                   Uri.IsHexDigit(character) &&
                   char.ToLowerInvariant(character)==character))
            {
                errors.Add($"Frame provenance {index} payload fingerprint is invalid.");
            }
        }

        if(provenance.Select(frame=>frame.Sequence.Value).Distinct().Count()!=provenance.Count)
            errors.Add("Frame provenance sequences must be unique.");

        return errors;
    }

    public static bool IsValid(
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionFrameProvenance> provenance)=>
        Validate(productionReport,provenance).Count==0;
}
