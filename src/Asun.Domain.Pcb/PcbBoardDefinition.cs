namespace Asun.Domain.Pcb;

public sealed record PcbBoardDefinition(
    Guid BoardId,
    string Name,
    double WidthMm,
    double HeightMm,
    int LayerCount)
{
    public bool IsValid =>
        BoardId != Guid.Empty &&
        !string.IsNullOrWhiteSpace(Name) &&
        double.IsFinite(WidthMm) &&
        double.IsFinite(HeightMm) &&
        WidthMm > 0 &&
        HeightMm > 0 &&
        LayerCount > 0;

    public PcbCoordinate Center =>
        IsValid
            ? new PcbCoordinate(WidthMm / 2d, HeightMm / 2d)
            : throw new InvalidOperationException("Board definition is invalid.");
}
