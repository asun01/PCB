namespace Asun.Vision.Contracts;

public static class NumericToleranceValidationRuntime
{
    public static IReadOnlyList<string> Validate(NumericTolerance tolerance)
    {
        var errors = new List<string>();

        if (!tolerance.IsValid)
            errors.Add("Numeric tolerance must contain finite non-negative components.");

        return errors;
    }

    public static bool IsValid(NumericTolerance tolerance) =>
        Validate(tolerance).Count == 0;
}
