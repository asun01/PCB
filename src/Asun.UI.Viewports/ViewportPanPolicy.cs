using System.Numerics;

namespace Asun.UI.Viewports;

public readonly record struct ViewportPanPolicy(
    bool ClampToImage,
    bool AllowOverscroll,
    float OverscrollPixels);

public static class ViewportPanPolicyRuntime
{
    public static Vector2 Apply(
        ViewportTransform transform,
        Vector2 requestedTranslation,
        ViewportPanPolicy policy)
    {
        if (!float.IsFinite(requestedTranslation.X) || !float.IsFinite(requestedTranslation.Y))
            throw new ArgumentOutOfRangeException(nameof(requestedTranslation));
        if (!float.IsFinite(policy.OverscrollPixels) || policy.OverscrollPixels < 0)
            throw new ArgumentOutOfRangeException(nameof(policy));

        if (!policy.ClampToImage)
            return requestedTranslation;

        var clamped = transform.WithTranslationClamped(requestedTranslation).Translation;
        if (!policy.AllowOverscroll)
            return clamped;

        var delta = requestedTranslation - clamped;
        if (delta.Length() <= policy.OverscrollPixels)
            return requestedTranslation;

        var direction = Vector2.Normalize(delta);
        return clamped + direction * policy.OverscrollPixels;
    }
}
