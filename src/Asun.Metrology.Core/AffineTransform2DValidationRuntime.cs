namespace Asun.Metrology.Core;

public static class AffineTransform2DValidationRuntime
{
    public static IReadOnlyList<string> Validate(
        AffineTransform2D transform,
        double determinantThreshold=1e-12)
    {
        var errors=new List<string>();

        if(!transform.IsFinite)
            errors.Add("Affine transform coefficients must be finite.");

        if(!double.IsFinite(determinantThreshold) ||
           determinantThreshold<=0)
        {
            errors.Add("Affine transform determinant threshold must be finite and positive.");
        }
        else if(transform.IsFinite &&
                Math.Abs(transform.Determinant)<determinantThreshold)
        {
            errors.Add("Affine transform determinant is below the inversion threshold.");
        }

        return errors;
    }

    public static bool IsValid(
        AffineTransform2D transform,
        double determinantThreshold=1e-12)=>
        Validate(transform,determinantThreshold).Count==0;
}
