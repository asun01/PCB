namespace Asun.UI.Viewports;

public readonly record struct ViewportZoomProfile(
    double MinimumScale,
    double MaximumScale,
    double WheelStep,
    double FitPadding);

public static class ViewportZoomProfileRuntime
{
    public static ViewportZoomProfile Default => new(0.05, 64, 1.15, 0.9);

    public static ViewportZoomProfile Validate(ViewportZoomProfile value)
    {
        if (!double.IsFinite(value.MinimumScale) || value.MinimumScale <= 0 ||
            !double.IsFinite(value.MaximumScale) || value.MaximumScale < value.MinimumScale ||
            !double.IsFinite(value.WheelStep) || value.WheelStep <= 1 ||
            !double.IsFinite(value.FitPadding) || value.FitPadding <= 0 || value.FitPadding > 1)
            throw new ArgumentOutOfRangeException(nameof(value));
        return value;
    }

    public static double StepScale(double scale, int wheelTicks, ViewportZoomProfile profile)
    {
        Validate(profile);
        if (!double.IsFinite(scale) || scale <= 0) throw new ArgumentOutOfRangeException(nameof(scale));
        return ViewportTransform.ClampScale(
            scale * Math.Pow(profile.WheelStep, wheelTicks / 120d),
            profile.MinimumScale,
            profile.MaximumScale);
    }
}
