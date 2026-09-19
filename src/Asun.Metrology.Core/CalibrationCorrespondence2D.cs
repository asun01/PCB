namespace Asun.Metrology.Core;

public readonly record struct CalibrationCorrespondence2D(
    MetrologyPoint2D Source,
    MetrologyPoint2D Target)
{
    public bool IsValid=>
        Source.IsFinite &&
        Target.IsFinite;
}
