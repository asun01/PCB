using System.Security.Cryptography;
using System.Text;
using Asun.Platform.RenderIntegration;

namespace Asun.Platform.RoiProductionIntegration;

public sealed record ProductionRoiRenderFrameProvenanceBinding(
    Guid ProductionSessionId,
    long Sequence,
    string ProductionInputFingerprint,
    string RoiBindingFingerprint,
    string RenderFingerprint,
    long Width,
    long Height,
    string PixelFormat,
    DateTimeOffset CapturedAtUtc,
    string BindingFingerprint);

public static class ProductionRoiRenderFrameProvenanceBindingRuntime
{
    public static ProductionRoiRenderFrameProvenanceBinding Create(
        ProductionSessionReport productionReport,
        ProductionRoiInteractionContext roiContext,
        ProductionFrameProvenance provenance,
        ProductionRenderReplayFrameIntegrity renderFrame)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(roiContext);

        var errors=Validate(productionReport,roiContext,provenance,renderFrame);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var canonical=string.Join("|",
            productionReport.SessionId,
            provenance.Sequence.Value,
            provenance.PayloadFingerprint,
            roiContext.BindingFingerprint,
            renderFrame.RenderFingerprint,
            provenance.Width,
            provenance.Height,
            provenance.PixelFormat.Length,
            provenance.PixelFormat,
            provenance.CapturedAtUtc.UtcTicks);

        return new ProductionRoiRenderFrameProvenanceBinding(
            productionReport.SessionId,
            provenance.Sequence.Value,
            provenance.PayloadFingerprint,
            roiContext.BindingFingerprint,
            renderFrame.RenderFingerprint,
            provenance.Width,
            provenance.Height,
            provenance.PixelFormat,
            provenance.CapturedAtUtc,
            Hash(canonical));
    }

    public static IReadOnlyList<string> Validate(
        ProductionSessionReport productionReport,
        ProductionRoiInteractionContext roiContext,
        ProductionFrameProvenance provenance,
        ProductionRenderReplayFrameIntegrity renderFrame)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(roiContext);
        ArgumentNullException.ThrowIfNull(provenance);
        ArgumentNullException.ThrowIfNull(renderFrame);

        var errors=new List<string>();
        if(!ProductionRoiInteractionContextRuntime.IsValidBinding(roiContext))
            errors.AddRange(ProductionRoiInteractionContextRuntime.ValidateBinding(roiContext));

        if(!ProductionRenderReplayFrameIntegrityRuntime.IsValid(productionReport,new[]{renderFrame}))
            errors.Add("Render replay frame integrity is invalid.");

        if(provenance.Sequence.Value<=0)
            errors.Add("Frame provenance sequence must be positive.");
        if(renderFrame.Sequence!=provenance.Sequence.Value)
            errors.Add("Render replay and frame provenance sequences must match.");
        if(!IsLowerHex(provenance.PayloadFingerprint))
            errors.Add("Frame provenance payload fingerprint is malformed.");
        if(!IsLowerHex(renderFrame.ProductionInputFingerprint))
            errors.Add("Render replay Production input fingerprint is malformed.");
        if(provenance.PayloadFingerprint!=renderFrame.ProductionInputFingerprint)
            errors.Add("Render replay Production input fingerprint must match frame provenance.");
        if(roiContext.ProductionSessionId!=productionReport.SessionId)
            errors.Add("ROI Production session identity must match.");
        if(roiContext.ProductionFrameCount!=productionReport.FrameCount)
            errors.Add("ROI Production frame count must match.");
        if(provenance.Sequence.Value<roiContext.FirstProductionSequence ||
           provenance.Sequence.Value>roiContext.LastProductionSequence)
            errors.Add("Frame provenance sequence must lie within the ROI Production context bounds.");

        return errors;
    }

    public static bool IsValid(
        ProductionSessionReport productionReport,
        ProductionRoiInteractionContext roiContext,
        ProductionFrameProvenance provenance,
        ProductionRenderReplayFrameIntegrity renderFrame)=>
        Validate(productionReport,roiContext,provenance,renderFrame).Count==0;

    public static bool IsEquivalent(
        ProductionRoiRenderFrameProvenanceBinding left,
        ProductionRoiRenderFrameProvenanceBinding right)=>
        left.BindingFingerprint==right.BindingFingerprint;

    private static bool IsLowerHex(string value)=>
        value.Length==64 && value.All(c=>Uri.IsHexDigit(c) && char.ToLowerInvariant(c)==c);

    private static string Hash(string value)=>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();
}
