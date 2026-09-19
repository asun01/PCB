namespace Asun.Domain.Pcb;

public static class PcbFeatureIdValidationRuntime
{
    public static IReadOnlyList<string> Validate(PcbFeatureId id)
    {
        var errors = new List<string>();

        if (!id.IsValid)
            errors.Add("PCB feature ids must be non-blank.");

        return errors;
    }

    public static bool IsValid(PcbFeatureId id) =>
        Validate(id).Count == 0;
}
