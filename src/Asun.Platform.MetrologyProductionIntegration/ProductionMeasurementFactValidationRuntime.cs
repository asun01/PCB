namespace Asun.Platform.MetrologyProductionIntegration;

public static class ProductionMeasurementFactValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionMeasurementFact> facts)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(facts);

        var errors=new List<string>();
        if(productionReport.FrameCount!=facts.Count)
            errors.Add("Measurement fact count must match production frame count.");

        var productionFrames=productionReport.Frames.OrderBy(frame=>frame.Sequence.Value).ToArray();
        var ordered=facts.OrderBy(fact=>fact.Sequence).ToArray();
        var count=Math.Min(productionFrames.Length,ordered.Length);

        for(var index=0;index<count;index++)
        {
            var production=productionFrames[index];
            var fact=ordered[index];
            if(fact.Sequence!=production.Sequence.Value)
                errors.Add($"Measurement fact {index} sequence mismatch.");
            if(fact.ProductionInputFingerprint!=production.InputFingerprint)
                errors.Add($"Measurement fact {index} production input fingerprint mismatch.");
            if(!fact.SourceMeasuredPosition.IsFinite ||
               !fact.MeasuredPosition.IsFinite)
                errors.Add($"Measurement fact {index} contains a non-finite point.");
            if(!double.IsFinite(fact.ErrorDistance) || fact.ErrorDistance<0)
                errors.Add($"Measurement fact {index} error distance is invalid.");
            if(fact.CalibrationFingerprint.Length!=64 ||
               !fact.CalibrationFingerprint.All(character=>
                   Uri.IsHexDigit(character) &&
                   char.ToLowerInvariant(character)==character))
                errors.Add($"Measurement fact {index} calibration fingerprint is invalid.");
            if(fact.ObservationFingerprint.Length!=64 ||
               !fact.ObservationFingerprint.All(character=>
                   Uri.IsHexDigit(character) &&
                   char.ToLowerInvariant(character)==character))
                errors.Add($"Measurement fact {index} observation fingerprint is invalid.");
        }

        if(facts.Select(fact=>fact.Sequence).Distinct().Count()!=facts.Count)
            errors.Add("Measurement fact sequences must be unique.");

        return errors;
    }

    public static bool IsValid(
        ProductionSessionReport productionReport,
        IReadOnlyList<ProductionMeasurementFact> facts)=>
        Validate(productionReport,facts).Count==0;
}
