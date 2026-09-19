namespace Asun.Domain.Pcb;

public static class PcbCoordinateValidationRuntime
{
    public static IReadOnlyList<string> Validate(PcbCoordinate coordinate)
    {
        var errors = new List<string>();

        if (!coordinate.IsFinite)
            errors.Add("PCB coordinates must remain finite.");

        return errors;
    }

    public static bool IsValid(PcbCoordinate coordinate) =>
        Validate(coordinate).Count == 0;
}
