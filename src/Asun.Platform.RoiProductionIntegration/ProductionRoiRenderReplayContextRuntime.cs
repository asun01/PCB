using System.Security.Cryptography;
using System.Text;
using Asun.Platform.RenderIntegration;
using Asun.Production.Runtime;

namespace Asun.Platform.RoiProductionIntegration;

public sealed record ProductionRoiRenderReplayContext(
    Guid ProductionSessionId,
    string RoiContextBindingFingerprint,
    int RenderFrameCount,
    long FirstSequence,
    long LastSequence,
    string RenderFingerprint,
    string BindingFingerprint);

public static class ProductionRoiRenderReplayContextRuntime
{
    public static ProductionRoiRenderReplayContext Create(
        ProductionSessionReport productionReport,
        ProductionRoiInteractionContext roiContext,
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> renderFrames)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(roiContext);
        ArgumentNullException.ThrowIfNull(renderFrames);

        var errors=Validate(productionReport,roiContext,renderFrames);
        if(errors.Count>0)
            throw new ArgumentException(string.Join(" ",errors));

        var ordered=renderFrames.OrderBy(frame=>frame.Sequence).ToArray();
        var renderFingerprint=CreateRenderFingerprint(ordered);
        var canonical=string.Join("|",
            productionReport.SessionId,
            roiContext.BindingFingerprint,
            ordered.Length,
            ordered.First().Sequence,
            ordered.Last().Sequence,
            renderFingerprint);

        return new ProductionRoiRenderReplayContext(
            productionReport.SessionId,
            roiContext.BindingFingerprint,
            ordered.Length,
            ordered.First().Sequence,
            ordered.Last().Sequence,
            renderFingerprint,
            Hash(canonical));
    }

    public static IReadOnlyList<string> Validate(
        ProductionSessionReport productionReport,
        ProductionRoiInteractionContext roiContext,
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> renderFrames)
    {
        ArgumentNullException.ThrowIfNull(productionReport);
        ArgumentNullException.ThrowIfNull(roiContext);
        ArgumentNullException.ThrowIfNull(renderFrames);

        var errors=new List<string>();
        if(!ProductionRoiInteractionContextRuntime.IsValidBinding(roiContext))
            errors.AddRange(ProductionRoiInteractionContextRuntime.ValidateBinding(roiContext));

        errors.AddRange(
            ProductionRenderReplayFrameIntegrityRuntime.Validate(
                productionReport,
                renderFrames));

        if(roiContext.ProductionSessionId!=productionReport.SessionId)
            errors.Add("ROI context session identity must match production session identity.");

        var ordered=renderFrames.OrderBy(frame=>frame.Sequence).ToArray();
        if(ordered.Length==0)
            errors.Add("Render replay frame collection cannot be empty.");

        if(ordered.Length>0)
        {
            if(ordered.First().Sequence<=0)
                errors.Add("Render replay first sequence must be positive.");
            if(ordered.Last().Sequence<ordered.First().Sequence)
                errors.Add("Render replay sequence bounds are invalid.");
        }

        return errors;
    }

    public static bool IsValid(
        ProductionSessionReport productionReport,
        ProductionRoiInteractionContext roiContext,
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> renderFrames)=>
        Validate(productionReport,roiContext,renderFrames).Count==0;

    public static bool IsEquivalent(
        ProductionRoiRenderReplayContext left,
        ProductionRoiRenderReplayContext right)=>
        left.BindingFingerprint==right.BindingFingerprint;

    private static string CreateRenderFingerprint(
        IReadOnlyList<ProductionRenderReplayFrameIntegrity> frames)
    {
        var canonical=string.Join(
            "
",
            frames.OrderBy(frame=>frame.Sequence)
                .Select(frame=>$"{frame.Sequence}|{frame.ProductionInputFingerprint}|{frame.RenderFingerprint}"));
        return Hash(canonical);
    }

    private static string Hash(string value)=>
        Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(value)))
        .ToLowerInvariant();
}
