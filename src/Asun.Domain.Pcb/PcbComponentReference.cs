namespace Asun.Domain.Pcb;

public sealed record PcbComponentReference(
    PcbFeatureId Id,
    string Designator,
    string Value,
    string PackageName,
    int LayerIndex,
    PcbLayerSide Side,
    PcbCoordinate Position,
    double RotationRadians)
{
    public bool IsValid(PcbBoardDefinition? board=null)=>
        Id.IsValid &&
        !string.IsNullOrWhiteSpace(Designator) &&
        !string.IsNullOrWhiteSpace(Value) &&
        !string.IsNullOrWhiteSpace(PackageName) &&
        LayerIndex>=0 &&
        (board is null || !board.IsValid || LayerIndex<board.LayerCount) &&
        Position.IsFinite &&
        double.IsFinite(RotationRadians);
}
