namespace Asun.Metrology.Core;

public static class MetrologyPoint2DValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        MetrologyPoint2D point)
    {
        var errors=new List<string>();

        if(!point.IsFinite)
            errors.Add("Metrology point coordinates must be finite.");

        return errors;
    }

    public static bool IsValid(
        MetrologyPoint2D point)=>
        Validate(point).Count==0;
}
