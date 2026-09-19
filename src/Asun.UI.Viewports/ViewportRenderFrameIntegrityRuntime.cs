namespace Asun.UI.Viewports;

public static class ViewportRenderFrameIntegrityRuntime
{
    public static IReadOnlyList<string> Validate(
        ViewportRenderCommandStream stream,
        ViewportRenderFrameSummary summary,
        string fingerprint)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(summary);

        var errors=new List<string>(
            ViewportRenderFrameSummaryValidationRuntime.Validate(
                stream,
                summary));

        if(fingerprint is null ||
           fingerprint.Length!=64 ||
           !fingerprint.All(character=>
               Uri.IsHexDigit(character) &&
               char.ToLowerInvariant(character)==character))
        {
            errors.Add("Render frame fingerprint must be 64 lowercase hexadecimal characters.");
        }
        else if(ViewportRenderFrameFingerprintRuntime.CreateFingerprint(summary)!=fingerprint)
        {
            errors.Add("Render frame fingerprint does not match the summary.");
        }

        return errors;
    }

    public static bool IsValid(
        ViewportRenderCommandStream stream,
        ViewportRenderFrameSummary summary,
        string fingerprint)=>
        Validate(stream,summary,fingerprint).Count==0;
}
