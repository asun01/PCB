using System.Security.Cryptography;
using System.Text;

namespace Asun.Platform.RoiProductionIntegration;

public sealed record ProductionRoiInputRecoveryRenderReplayBinding(
    Guid ProductionSessionId,
    string InputRecoveryFingerprint,
    string RoiFingerprint,
    string RenderFingerprint,
    int RenderFrameCount,
    long FirstSequence,
    long LastSequence,
    string BindingFingerprint);

public static class ProductionRoiInputRecoveryRenderReplayBindingRuntime
{
    public static ProductionRoiInputRecoveryRenderReplayBinding Create(
        ProductionRoiInteractionContext roiContext,
        ProductionRoiRenderReplayContext renderContext)
    {
        ArgumentNullException.ThrowIfNull(roiContext);
        ArgumentNullException.ThrowIfNull(renderContext);

        var errors=Validate(roiContext,renderContext);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var canonical=string.Join("|",
            roiContext.ProductionSessionId,
            roiContext.InputRecoveryFingerprint,
            roiContext.RoiFingerprint,
            renderContext.RenderFingerprint,
            renderContext.RenderFrameCount,
            renderContext.FirstSequence,
            renderContext.LastSequence,
            renderContext.BindingFingerprint);

        return new ProductionRoiInputRecoveryRenderReplayBinding(
            roiContext.ProductionSessionId,
            roiContext.InputRecoveryFingerprint,
            roiContext.RoiFingerprint,
            renderContext.RenderFingerprint,
            renderContext.RenderFrameCount,
            renderContext.FirstSequence,
            renderContext.LastSequence,
            Hash(canonical));
    }

    public static IReadOnlyList<string> Validate(
        ProductionRoiInteractionContext roiContext,
        ProductionRoiRenderReplayContext renderContext)
    {
        ArgumentNullException.ThrowIfNull(roiContext);
        ArgumentNullException.ThrowIfNull(renderContext);

        var errors=new List<string>();
        errors.AddRange(ProductionRoiInteractionContextRuntime.ValidateBinding(roiContext));

        if(renderContext.ProductionSessionId!=roiContext.ProductionSessionId)
            errors.Add("ROI input and render replay Production session identities must match.");
        if(renderContext.RoiContextBindingFingerprint!=roiContext.BindingFingerprint)
            errors.Add("ROI render replay context must belong to the ROI interaction context.");
        if(renderContext.RenderFrameCount<=0)
            errors.Add("ROI render replay frame count must be positive.");
        if(renderContext.RenderFrameCount!=roiContext.ProductionFrameCount)
            errors.Add("ROI render replay frame count must match the Production frame count.");
        if(renderContext.FirstSequence<roiContext.FirstProductionSequence ||
           renderContext.LastSequence>roiContext.LastProductionSequence)
            errors.Add("ROI render replay sequence bounds must remain inside the Production context bounds.");
        if(renderContext.LastSequence<renderContext.FirstSequence)
            errors.Add("ROI render replay sequence bounds are invalid.");
        if(renderContext.RenderFingerprint.Length!=64 || !IsLowerHex(renderContext.RenderFingerprint))
            errors.Add("ROI render replay fingerprint is malformed.");
        if(renderContext.BindingFingerprint.Length!=64 || !IsLowerHex(renderContext.BindingFingerprint))
            errors.Add("ROI render replay binding fingerprint is malformed.");

        return errors;
    }

    public static bool IsValid(
        ProductionRoiInteractionContext roiContext,
        ProductionRoiRenderReplayContext renderContext)=>
        Validate(roiContext,renderContext).Count==0;

    public static bool IsEquivalent(
        ProductionRoiInputRecoveryRenderReplayBinding left,
        ProductionRoiInputRecoveryRenderReplayBinding right)=>
        left.BindingFingerprint==right.BindingFingerprint;

    private static bool IsLowerHex(string value)=>
        value.Length==64 && value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c);

    private static string Hash(string value)=>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
}
